using Dapper;
using DishOfTheDay.Entity;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace DishOfTheDay.Repository
{
    internal class BaseRepository
    {
        private readonly string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Kitchen.Properties.Settings.KitchenConnectionString"].ConnectionString;

        protected DbConnection GetConnection()
        {
            return new System.Data.SqlClient.SqlConnection(connectionString);
        }

        public string GetStatistics()
        {
            RtfBuilder result = new RtfBuilder();

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
                            var total = reader.GetInt32(columnIndex); //reader.GetInt32(0);
                            var withImage = reader.GetInt32(1);
                            var withoutImage = reader.GetInt32(2);
                            DateTime? first = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3);
                            DateTime? last = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4);
                            result.AppendBold("Dish information").AppendLine()
                                .AppendLine($"total = {total}")
                                .AppendLine($"withImage = {withImage}")
                                .AppendLine($"withoutImage = {withoutImage}")
                                .AppendLine($"first = {first}")
                                .AppendLine($"last = {last}");
                        }
                    }

                    command.CommandText = @"SELECT SUM(usage_count) sum, MIN(usage_count) min, MAX(usage_count) max, AVG(cast(usage_count as decimal)) as avg FROM ClientDish";
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var sum = reader.GetInt32(0);
                            var min = reader.GetInt32(1);
                            var max = reader.GetInt32(2);
                            var avg = reader.GetDecimal(3);
                            result.AppendLine().AppendBoldLine("DishCLient Information")
                                .AppendLine($"sum = {sum}")
                                .AppendLine($"min = {min}")
                                .AppendLine($"max = {max}")
                                .AppendLine($"avg = {avg}");
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
                            var popularDishType = reader.GetString(0);
                            var popularDishTypeCount = reader.GetInt32(1);
                            result.AppendLine().AppendBoldLine("The most popular DishType")
                                .AppendLine($"DishType = {popularDishType} in count - {popularDishTypeCount}");
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
                            var popularKitchen = reader.GetString(0);
                            var popularKitchenCount = reader.GetInt32(1);
                            result.AppendLine().AppendBoldLine("The most popular Kitchen")
                                .AppendLine($"Kitchen = {popularKitchen} in count - {popularKitchenCount}");
                        }
                    }

                    command.CommandText = @"SELECT TOP 3 COUNT(rating) ratedCount, c.first_name + ' ' + c.last_name fullName
                        FROM ClientDish cd
                            JOIN Client c on c.client_id = cd.client_id
                        GROUP BY c.client_id, c.first_name, c.last_name
                        ORDER BY ratedCount DESC";
                    using (var reader = command.ExecuteReader())
                    {
                        result.AppendLine().AppendBoldLine("The most active person");
                        while (reader.Read())
                        {
                            var i = 0;
                            var ratedCount = reader.GetInt32(i++);
                            var fullName = reader.GetString(i++);
                            result.AppendLine($"RatedCount = {ratedCount} from person - {fullName}");
                        }
                    }

                    command.CommandText = @"SELECT TOP 3 SUM(usage_count) totalUsage, c.first_name + ' ' + c.last_name fullName
                        FROM ClientDish cd
                            JOIN Client c on c.client_id = cd.client_id
                        GROUP BY c.client_id, c.first_name, c.last_name
                        ORDER BY totalUsage DESC";
                    using (var reader = command.ExecuteReader())
                    {
                        result.AppendLine().AppendBoldLine("The most hungry person");
                        while (reader.Read())
                        {
                            var i = 0;
                            var totalUsage = reader.GetInt32(i++);
                            var fullName = reader.GetString(i++);
                            result.AppendLine($"TotalUsage = {totalUsage} from person - {fullName}");
                        }
                    }


                }


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

            return result.ToRtf();

        }
    }

}
