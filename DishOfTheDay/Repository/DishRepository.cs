using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using DishOfTheDay.Entity;

namespace DishOfTheDay.Repository
{
    internal class DishRepository : BaseRepository
    {
        public IEnumerable<DishEntity> GetAll(string search, int sortIndex, bool sortAsc, int minCookingTime, int maxCookingTime,
            int minIngredientCount, int maxIngredientCount, int dishTypeId, int kitchenId, bool? hasPicture, bool? myDishes, bool? myReviews, int client_id)
        {
            var sql = @"SELECT d.dish_id, d.name, d.kitchen, d.dish_type, d.cooking_time, d.recipe, d.picture, d.owner, k.name KitchenName, dt.name DishTypeName,
                count(DI.ingredient_id) ingredient_count,
                (SELECT AVG(CAST(rating AS decimal)) FROM ClientDish CD WHERE d.dish_id = CD.dish_id) rating,
                (SELECT SUM(usage_count) FROM ClientDish CD WHERE d.dish_id = CD.dish_id) usage_count
                FROM Dish d
                JOIN Kitchen k on d.kitchen = k.kitchen_id
                JOIN DishType dt on dt.dish_type_id = d.dish_type
                LEFT JOIN DishIngredient DI on d.dish_id = DI.dish_id
                LEFT JOIN ClientDish cd ON d.dish_id = cd.dish_id AND cd.client_id = @client_Id
                GROUP BY d.dish_id, d.name, d.kitchen, d.dish_type, d.cooking_time, d.recipe, d.picture, d.owner, k.name, dt.name, cd.rating
                HAVING 1=1";

            if (!string.IsNullOrEmpty(search))
            {
                sql += " AND d.name LIKE @search";
            }
            if(minCookingTime >= 0)           
                sql += " AND d.cooking_time >= @minCookingTime";
            if(maxCookingTime >= 0)
                sql += " AND d.cooking_time <= @maxCookingTime";
            if(minIngredientCount >= 0)           
                sql += " AND count(DI.ingredient_id) >= @minIngredientCount";
            if(maxIngredientCount >= 0)
                sql += " AND count(DI.ingredient_id) <= @maxIngredientCount";
            if(dishTypeId != -1)
                sql += " AND d.dish_type = @dishTypeId";
            if(kitchenId != -1)
                sql += " AND d.kitchen = @kitchenId";
            if (hasPicture.HasValue)
            {
                if(hasPicture.Value)
                    sql += " AND d.picture IS NOT NULL";
                else
                    sql += " AND d.picture IS NULL";
            }            
            if (myDishes.HasValue)
            {
                if(myDishes.Value)
                    sql += " AND d.owner = @client_id";
                else
                    sql += " AND d.owner <> @client_id";
            }            
            if (myReviews.HasValue)
            {
                if(myReviews.Value)
                    sql += " AND cd.rating IS NOT NULL";
                else
                    sql += " AND cd.rating IS NULL";
            }
            if (sortIndex >= 0)
            {
                sql += " ORDER BY ";
                switch (sortIndex)
                {
                    case 1:
                        sql += "d.name";
                        break;
                    case 2:
                        sql += "k.name";
                        break;
                    case 3:
                        sql += "dt.name";
                        break;
                    case 4:
                        sql += "cooking_time";
                        break;
                    case 0:
                    default:
                        sql += "d.name";
                        break;

                }
                if (sortAsc)
                    sql += " ASC";
                else
                    sql += " DESC";
            }
            return GetConnection().Query<DishEntity>(sql, new
            {
                search = "%"+search+"%",
                minCookingTime,
                maxCookingTime,
                minIngredientCount,
                maxIngredientCount,
                dishTypeId,
                kitchenId,
                client_id,
            });

        }

        internal IEnumerable<DishModel> GetModels(int[] ids)
        {
            var result = new List<DishModel>();

            var data = GetConnection().Query<DishModel, DishModel.IngredientModel, DishModel>(
                @"SELECT d.dish_id, d.name, d.cooking_time, d.recipe, d.picture, k.name KitchenName, dt.name DishTypeName,
                    i.ingredient_id, i.name ingredientName, i.units, di.count
                FROM Dish d
                JOIN Kitchen k on d.kitchen = k.kitchen_id
                JOIN DishType dt on dt.dish_type_id = d.dish_type
                LEFT JOIN DishIngredient DI on d.dish_id = DI.dish_id
                LEFT JOIN Ingredient i on DI.ingredient_id = i.ingredient_id
                WHERE d.dish_id IN @ids",
                (dish, ingredient) => {
                    var found = result.FirstOrDefault(d => d.dish_id == dish.dish_id);
                    if (found == null)
                    {
                        found = dish;
                        result.Add(dish);
                    }
                    found.ingredients.Add(ingredient);
                    return found; 
                }, 
                new { ids }, splitOn: "ingredient_id").ToList();
            return result;
        }

        public DishEntity GetById(int id)
        {
            return GetConnection().QueryFirstOrDefault<DishEntity>("SELECT dish_id, name, kitchen, dish_type, cooking_time, recipe, picture, owner FROM Dish WHERE dish_id = @dish_id", 
                new { dish_id = id});
        }
        
        public int Insert(DishEntity item, List<DishIngredientEntity> currentIngredients)
        {
            var cn = GetConnection();
            cn.Open();
            var t = cn.BeginTransaction();
            item.dish_id = cn.ExecuteScalar<int>(@"INSERT INTO Dish (name, kitchen, dish_type, cooking_time, recipe, picture, owner) 
                VALUES (@name, @kitchen, @dish_type, @cooking_time, @recipe, @picture, @owner);
                SELECT  SCOPE_IDENTITY();",
                item, t);

            if(currentIngredients != null && currentIngredients.Count > 0)
            {
                foreach (var i in currentIngredients)
                {
                    i.dish_id = item.dish_id;
                //    cn.ExecuteScalar<int>(@"INSERT INTO DishIngredient (dish_id, ingredient_id, [count]) 
                //VALUES (@dish_id, @ingredient_id, @count)", i, t);
                }
                cn.Execute(@"INSERT INTO DishIngredient (dish_id, ingredient_id, [count]) VALUES (@dish_id, @ingredient_id, @count)",
                    currentIngredients, t);

            }
            t.Commit();
            return item.dish_id;
        }

        public void Update(DishEntity item, List<DishIngredientEntity> currentIngredients)
        {
            var cn = GetConnection();
            cn.Open();
            var t = cn.BeginTransaction();
            cn.Execute(@"UPDATE Dish
                SET name         = @name,
                    kitchen      = @kitchen,
                    dish_type    = @dish_type,
                    cooking_time = @cooking_time,
                    recipe       = @recipe,
                    picture      = @picture
                WHERE dish_id = @dish_id;",
                item, t);

            cn.Execute("DELETE From DishIngredient  WHERE dish_id = @dish_id", new { dish_id = item.dish_id }, t);

            if (currentIngredients != null && currentIngredients.Count > 0)
            {
                cn.Execute(@"INSERT INTO DishIngredient (dish_id, ingredient_id, [count]) VALUES (@dish_id, @ingredient_id, @count)", 
                    currentIngredients, t);

                //foreach (var i in currentIngredients)
                //{
                //    //i.dish_id = item.dish_id;
                //    cn.ExecuteScalar<int>(@"INSERT INTO DishIngredient (dish_id, ingredient_id, [count]) 
                //VALUES (@dish_id, @ingredient_id, @count)", i, t);
                //}
            }
            t.Commit();
        }

        public void Delete(int id)
        {
            GetConnection().Execute("DELETE From Dish  WHERE dish_id = @dish_id", new {dish_id = id});
        }

        internal IEnumerable<DishIngredientEntity> GetIngredients(int dish_id)
        {
            return GetConnection().Query<DishIngredientEntity>(@"SELECT dish_id, di.ingredient_id, i.name ingredientName, count, units FROM DishIngredient di
                    JOIN Ingredient i on di.ingredient_id = i.ingredient_id
                    WHERE dish_id = @dish_id ORDER BY i.name",
                new { dish_id = dish_id });
        }
    }
}