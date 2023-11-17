using System.Collections.Generic;
using Dapper;
using DishOfTheDay.Entity;

namespace DishOfTheDay.Repository
{
    internal class ClientRepository : BaseRepository
    {

        public IEnumerable<ClientEntity> GetAll(string search, int sortIndex, bool sortAsc, bool? phone, bool? address, bool? desc,
            int minDishes, int maxDishes)
        {
            var sql = @"SELECT c.client_id, c.first_name, c.last_name, c.email, c.phone, c.address, c.description
                    FROM Client c";

            sql += " WHERE 1=1";
            
            if (!string.IsNullOrEmpty(search))
            {
                sql += " AND c.first_name LIKE @search";
            }
            if(minDishes >= 0)           
                sql += " AND (select COUNT(*) from ClientDish cd where cd.client_id=c.client_id)  >= @minDishes";
            if(maxDishes >= 0)
                sql += " AND (select COUNT(*) from ClientDish cd where cd.client_id=c.client_id) <= @maxDishes";
            if (phone.HasValue)
            {
                if(phone.Value)
                    sql += " AND c.phone IS NOT NULL";
                else
                    sql += " AND c.phone IS NULL";
            }if (address.HasValue)
            {
                if(address.Value)
                    sql += " AND c.address IS NOT NULL";
                else
                    sql += " AND c.address IS NULL";
            }if (desc.HasValue)
            {
                if(desc.Value)
                    sql += " AND c.description IS NOT NULL";
                else
                    sql += " AND c.description IS NULL";
            }
            if (sortIndex >= 0)
            {
                sql += " ORDER BY ";
                switch (sortIndex)
                {
                    case 1:
                        sql += "c.first_name";
                        break;
                    case 2:
                        sql += "c.last_name";
                        break;
                    case 3:
                        sql += "c.email";
                        break;
                    case 0:
                    default:
                        sql += "c.first_name";
                        break;
            
                }
                if (sortAsc)
                    sql += " ASC";
                else
                    sql += " DESC";
            }
            return GetConnection().Query<ClientEntity>(sql, new
            {
                search = "%"+search+"%",
                phone,
                address,
                desc,
                minDishes,
                maxDishes,
            });
        }
        
        public ClientEntity GetById(int id)
        {
            return GetConnection().QueryFirstOrDefault<ClientEntity>("SELECT client_id, first_name, last_name, email, phone, address, description FROM Client WHERE client_id = @client_id",
                new { client_id = id });
        }

        //public IEnumerable<ClientEntity> GetGenders()
        //{
        //    return GetConnection().Query<ClientEntity>("SELECT name FROM Gender ORDER BY gender_id");
        //}

        public int Insert(ClientEntity item)
        {
            item.client_id = GetConnection().ExecuteScalar<int>(@"INSERT INTO [Client] 
                ([first_name], [last_name], [email], [phone], [address], [description])
                VALUES (@first_name, @last_name, @email, @phone, @address, @description); 
                SELECT  SCOPE_IDENTITY();",
                item);
            return item.client_id;
        }

        public void Update(ClientEntity item)
        {
            GetConnection().Execute(@"UPDATE Client
                SET [first_name]  = @first_name,
                    [last_name]   = @last_name,
                    [email]       = @email,
                    [phone]       = @phone,
                    [address]     = @address,
                    [description] = @description
                WHERE [client_id] = @client_id;",
                item);
        }

        public void Delete(int id)
        {
            GetConnection().Execute("DELETE From Client  WHERE client_id = @client_id", new { client_id = id });
        }
    }

}
