using System.Collections.Generic;
using Dapper;
using DishOfTheDay.Entity;

namespace DishOfTheDay.Repository
{
    internal class KitchenRepository : BaseRepository, IGenericRepository<KitchenEntity>
    {
        public IEnumerable<KitchenEntity> GetAll()
        {
            return GetConnection().Query<KitchenEntity>("SELECT kitchen_id, name, country_flag FROM Kitchen");
        }

        public KitchenEntity GetById(int id)
        {
            return GetConnection().QueryFirstOrDefault<KitchenEntity>("SELECT kitchen_id, name, country_flag FROM Kitchen WHERE kitchen_id = @kitchen_id", 
                new { kitchen_id = id});
        }
        
        public int Insert(KitchenEntity item)
        {
            item.kitchen_id = GetConnection().ExecuteScalar<int>(@"INSERT INTO Kitchen (name, country_flag) 
                VALUES (@name, @country_flag);
                SELECT  SCOPE_IDENTITY();",
                item);
            return item.kitchen_id;
        }

        public void Update(KitchenEntity item)
        {
            GetConnection().Execute(@"UPDATE Kitchen SET name = @name, country_flag = @country_flag WHERE kitchen_id = @kitchen_id;", item);
        }

        public void Delete(int id)
        {
            GetConnection().Execute("DELETE From Kitchen  WHERE kitchen_id = @kitchen_id", new {kitchen_id = id});
        }
    }
}