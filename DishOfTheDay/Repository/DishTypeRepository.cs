using System.Collections.Generic;
using Dapper;
using DishOfTheDay.Entity;

namespace DishOfTheDay.Repository
{
    internal class DishTypeRepository : BaseRepository, IGenericRepository<DishTypeEntity>
    {
        public IEnumerable<DishTypeEntity> GetAll()
        {
            return GetConnection().Query<DishTypeEntity>("SELECT dish_type_id, name FROM DishType");
        }

        public DishTypeEntity GetById(int id)
        {
            return GetConnection().QueryFirstOrDefault<DishTypeEntity>("SELECT dish_type_id, name FROM DishType WHERE dish_type_id = @dish_type_id", 
                new { dish_type_id = id});
        }
        
        public int Insert(DishTypeEntity item)
        {
            item.dish_type_id = GetConnection().ExecuteScalar<int>("INSERT INTO DishType (name) VALUES (@name); SELECT  SCOPE_IDENTITY();", item);
            return item.dish_type_id;
        }

        public void Update(DishTypeEntity item)
        {
            GetConnection().Execute(@"UPDATE DishType SET name = @name WHERE dish_type_id = @dish_type_id;", item);
        }

        public void Delete(int id)
        {
            GetConnection().Execute("DELETE From DishType  WHERE dish_type_id = @dish_type_id", new {dish_type_id = id});
        }
    }    
}