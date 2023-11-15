using System.Collections.Generic;
using System.Data.Common;

namespace DishOfTheDay.Repository
{
    internal interface IGenericRepository<T> where T : new()
    {
        IEnumerable<T> GetAll();
        T GetById(int id);
        int Insert(T item);
        void Update(T item);
        void Delete(int id);
    }

    internal class BaseRepository
    {
        private readonly string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Kitchen.Properties.Settings.KitchenConnectionString"].ConnectionString;

        protected DbConnection GetConnection()
        {
            return new System.Data.SqlClient.SqlConnection(connectionString);
        }
    }

}
