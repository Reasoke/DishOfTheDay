using System;
using System.Collections.Generic;
using Dapper;
using DishOfTheDay.Entity;

namespace DishOfTheDay.Repository
{
    internal class DishTypeRepository : BaseRepository
    {
        public IEnumerable<DishTypeEntity> GetAll()
        {
            return GetConnection().Query<DishTypeEntity>("SELECT dish_type_id, name FROM DishType");
        }

        public IEnumerable<DishTypeEntity> GetAll(string search, int sortIndex, bool sortAsc)
        {
            var sql = @"SELECT name FROM DishType";

            sql += " WHERE 1=1";

            if (!string.IsNullOrEmpty(search))
            {
                sql += " AND name LIKE @search";
            }
            if (sortIndex >= 0)
            {
                sql += " ORDER BY ";
                switch (sortIndex)
                {
                    case 1:
                        sql += "name";
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
            return GetConnection().Query<DishTypeEntity>(sql, new
            {
                search = "%" + search + "%",
            });
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