using Dapper;
using DishOfTheDay.Entity;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using static DishOfTheDay.Entity.StatisticsInfo;

namespace DishOfTheDay.Repository
{
    internal class BaseRepository
    {
        private readonly string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Kitchen.Properties.Settings.KitchenConnectionString"].ConnectionString;

        protected DbConnection GetConnection()
        {
            return new System.Data.SqlClient.SqlConnection(connectionString);
        }

        public StatisticsInfo GetStatistics()
        {
            StatisticsInfo result = new StatisticsInfo();
            
            using (var cn = GetConnection())
            {
                cn.Open();
                using(var command = cn.CreateCommand())
                {
                    command.CommandText = @"SELECT COUNT(1) total, COUNT(picture) withImage, COUNT(iif(picture is null, 1, null)) withoutImage,
                        MIN(created) first, MAX(created) last
                        FROM Dish";
                    using (var reader = command.ExecuteReader())
                    {
                        
                        if (reader.Read())
                        {
                            var columnIndex = reader.GetOrdinal("total");
                            result.TotalDishCount = reader.GetInt32(columnIndex); //reader.GetInt32(0);
                            result.DishesWithImageCount = reader.GetInt32(1);
                            result.DishesWithoutImageCount = reader.GetInt32(2);
                            result.FirstCreated = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3);
                            result.LastCreated = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4);
                        }
                    }

                    command.CommandText = @"SELECT SUM(usage_count) sum, MIN(usage_count) min, MAX(usage_count) max, AVG(cast(usage_count as decimal)) as avg FROM ClientDish";
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result.SumOfUsage = reader.GetInt32(0);
                            result.MinOfUsage = reader.GetInt32(1);
                            result.MaxOfUsage = reader.GetInt32(2);
                            result.AvgOfUsage = reader.GetDecimal(3);
                        }
                    }
                    
                    command.CommandText = @"SELECT TOP 1 dt.name, COUNT(1) num
                        FROM Dish d
                                 JOIN DishType dt on d.dish_type = dt.dish_type_id
                        GROUP BY dt.dish_type_id, dt.name
                        ORDER BY num DESC";
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result.PopularDishType = reader.GetString(0);
                            result.PopularDishTypeCount = reader.GetInt32(1);
                        }
                    } 
                    
                    command.CommandText = @"SELECT TOP 1 k.name, COUNT(1) num
                        FROM Dish d
                                 JOIn Kitchen k on d.kitchen = k.kitchen_id
                        GROUP BY k.kitchen_id, k.name
                        ORDER BY num DESC";
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result.PopularKitchen = reader.GetString(0);
                            result.PopularKitchenCount = reader.GetInt32(1);
                        }
                    }

                    command.CommandText = @"SELECT TOP 3 COUNT(rating) ratedCount, c.first_name + ' ' + c.last_name fullName
                        FROM ClientDish cd
                            JOIN Client c on c.client_id = cd.client_id
                        GROUP BY c.client_id, c.first_name, c.last_name
                        ORDER BY ratedCount DESC";
                    using (var reader = command.ExecuteReader())
                    {
                        result.MostActive = new List<StatisticsInfo.UserValue>();
                        while (reader.Read())
                        {
                            var i = 0;
                            var ratedCount = reader.GetInt32(i++);
                            var fullName = reader.GetString(i++);
                            result.MostActive.Add(new StatisticsInfo.UserValue() {Name = fullName, Value = ratedCount});
                        }
                    }

                    command.CommandText = @"SELECT TOP 3 SUM(usage_count) totalUsage, c.first_name + ' ' + c.last_name fullName
                        FROM ClientDish cd
                            JOIN Client c on c.client_id = cd.client_id
                        GROUP BY c.client_id, c.first_name, c.last_name
                        ORDER BY totalUsage DESC";
                    using (var reader = command.ExecuteReader())
                    {
                        result.MostHungry = new List<StatisticsInfo.UserValue>();
                        while (reader.Read())
                        {
                            var i = 0;
                            var totalUsage = reader.GetInt32(i++);
                            var fullName = reader.GetString(i++);
                            result.MostHungry.Add(new StatisticsInfo.UserValue(fullName, totalUsage));
                        }
                    }
                }


                result.DishPeriods = cn.Query<PeriodStats>(
                    @"SELECT DATEPART(Year, d.created) Year, DATEPART(Month, d.created) Month, count(1) Amount
                    FROM Dish d
                    GROUP BY DATEPART(Year, d.created), DATEPART(Month, d.created)
                    ORDER BY Year, Month").ToList();

                result.ClientPeriods = cn.Query<PeriodStats>(
                    @"SELECT DATEPART(Year, d.created) Year, DATEPART(Month, d.created) Month, count(1) Amount
                    FROM Client d
                    GROUP BY DATEPART(Year, d.created), DATEPART(Month, d.created)
                    ORDER BY Year, Month").ToList();

                //var popularDishType = cn.ExecuteScalar<string>("select count(1) total from Dish");
                //var t = cn.Query<string>("SELECT TOP 5 rating FROM ClientDish");
                //foreach (var item in t)
                //{
                //    result.AppendLine(item);
                //}

            }

            //result.AppendLine("-------------------------------------------------");
            //result.AppendLine("| hello   |  cell2   | cell3      |");
            //result.AppendLine("-------------------------------------------------");

            return result;
        }
    }

}
