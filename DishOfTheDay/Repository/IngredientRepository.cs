using System.Collections.Generic;
using Dapper;
using DishOfTheDay.Entity;

namespace DishOfTheDay.Repository
{
    internal class IngredientRepository : BaseRepository, IGenericRepository<IngredientEntity>
    {
        public IEnumerable<IngredientEntity> GetAll()
        {
            return GetConnection().Query<IngredientEntity>("SELECT ingredient_id, name, price, units, expiration, manufacturer FROM Ingredient");
        }

        public IngredientEntity GetById(int id)
        {
            return GetConnection().QueryFirstOrDefault<IngredientEntity>("SELECT ingredient_id, name, price, units, expiration, manufacturer FROM Ingredient WHERE ingredient_id = @ingredient_id", 
                new { ingredient_id = id});
        }
        
        public int Insert(IngredientEntity item)
        {
            item.ingredient_id = GetConnection().ExecuteScalar<int>("INSERT INTO Ingredient (name) VALUES (@name); SELECT  SCOPE_IDENTITY();", item);
            return item.ingredient_id;
        }

        public void Update(IngredientEntity item)
        {
            GetConnection().Execute(@"UPDATE Ingredient SET name = @name WHERE ingredient_id = @ingredient_id;", item);
        }

        public void Delete(int id)
        {
            GetConnection().Execute("DELETE From Ingredient  WHERE ingredient_id = @ingredient_id", new { ingredient_id = id});
        }
    }
}