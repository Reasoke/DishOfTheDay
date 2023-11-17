using System;
using System.Collections.Generic;
using Dapper;
using DishOfTheDay.Entity;

namespace DishOfTheDay.Repository
{
    internal class IngredientRepository : BaseRepository
    {
        public IEnumerable<IngredientEntity> GetAll()
        {
            return GetConnection().Query<IngredientEntity>("SELECT ingredient_id, name, price, units, expiration, manufacturer FROM Ingredient");
        }

        public IEnumerable<IngredientEntity> GetAll(string search, int sortIndex, bool sortAsc, int minPrice, int maxPrice, string units, string manufacturer)
        {
            var sql = @"SELECT ingredient_id, name, price, units, expiration, manufacturer FROM Ingredient";

            sql += " WHERE 1=1";

            if (!string.IsNullOrEmpty(search))
            {
                sql += " AND name LIKE @search";
            }
            if (minPrice >= 0)
                sql += " AND price >= @minPrice";
            if (maxPrice >= 0)
                sql += " AND price <= @maxPrice";
            if (!string.IsNullOrEmpty(units))
                sql += " AND units = @units";
            if (!string.IsNullOrEmpty(manufacturer))
                sql += " AND manufacturer = @manufacturer";
            if (sortIndex >= 0)
            {
                sql += " ORDER BY ";
                switch (sortIndex)
                {
                    case 1:
                        sql += "name";
                        break;
                    case 2:
                        sql += "price";
                        break;
                    case 3:
                        sql += "units";
                        break;
                    case 4:
                        sql += "expiration";
                        break;
                    case 5:
                        sql += "manufacturer";
                        break;
                    case 0:
                    default:
                        sql += "name";
                        break;

                }
                if (sortAsc)
                    sql += " ASC";
                else
                    sql += " DESC";
            }
            return GetConnection().Query<IngredientEntity>(sql, new
            {
                search = "%" + search + "%",
                minPrice,
                maxPrice,
                units,
                manufacturer,
            });
        }

        public IngredientEntity GetById(int id)
        {
            return GetConnection().QueryFirstOrDefault<IngredientEntity>("SELECT ingredient_id, name, price, units, expiration, manufacturer FROM Ingredient WHERE ingredient_id = @ingredient_id", 
                new { ingredient_id = id});
        }
        
        public int Insert(IngredientEntity item)
        {
            item.ingredient_id = GetConnection().ExecuteScalar<int>("INSERT INTO Ingredient (name, price, units, expiration, manufacturer) VALUES (@name, @price, @units, @expiration, @manufacturer); SELECT  SCOPE_IDENTITY();", item);
            return item.ingredient_id;
        }

        public void Update(IngredientEntity item)
        {
            GetConnection().Execute(@"UPDATE Ingredient SET name = @name, price = @price, units = @units, expiration = @expiration, manufacturer = @manufacturer WHERE ingredient_id = @ingredient_id;", item);
        }

        public void Delete(int id)
        {
            GetConnection().Execute("DELETE From Ingredient  WHERE ingredient_id = @ingredient_id", new { ingredient_id = id});
        }

        public IEnumerable<string> GetIngredientUnits()
        {
            return GetConnection().Query<string>("SELECT DISTINCT units FROM Ingredient");
        }

        public IEnumerable<string> GetIngredientManufacturers()
        {
            return GetConnection().Query<string>("SELECT DISTINCT manufacturer FROM Ingredient WHERE manufacturer IS NOT NULL");
        }
    }
}