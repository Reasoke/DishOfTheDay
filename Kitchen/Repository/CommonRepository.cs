using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Kitchen.Repository
{
    internal class BaseRepository
    {
        private readonly string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Kitchen.Properties.Settings.KitchenConnectionString"].ConnectionString;

        protected DbConnection GetConnection()
        {
            return new System.Data.SqlClient.SqlConnection(connectionString);
        }
    }

    internal class CommonRepository : BaseRepository
    {
        public IEnumerable<string> GetGenders()
        {
            return GetConnection().Query<string>("SELECT name FROM Gender ORDER BY gender_id");
        }

        public int InsertClient(string firstName, string lastName, string email, string password, string phone, string address, string descriprion, DateTime dob, string gender)
        {
            return GetConnection().ExecuteScalar<int>(@"INSERT INTO [Client] 
                ([first_name], [last_name], [email], [password], [phone], [address], [description], [DOB], [gender])
                VALUES (@first_name, @last_name, @email, @password, @phone, @address, @description, @DOB, @gender); 
                SELECT  SCOPE_IDENTITY();",
                new
                {
                    first_name = firstName,
                    last_name = lastName,
                    email = email,
                    password = password,
                    phone = phone,
                    address = address,
                    description = descriprion,
                    DOB = dob,
                    gender = gender,
                });

        }

        public void UpdateClient(ClientEntity entity)
        {
            GetConnection().Execute(@"UPDATE [Client]
                SET [first_name]  = @first_name,
                    [last_name]   = @last_name,
                    [email]       = @email,
                    [password]    = @password,
                    [phone]       = @phone,
                    [address]     = @address,
                    [description] = @description,
                    [DOB]         = @DOB,
                    [gender]      = @gender
                WHERE [client_id] = @client_id;",
                entity);

        }

        public void DeleteClient(int id)
        {
            GetConnection().Execute("DELETE From Client  WHERE client_id = @client_id", new { client_id = id });
        }
    }

}
