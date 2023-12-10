using System;
using System.Collections.Generic;

namespace DishOfTheDay.Entity
{
    public class StatisticsInfo
    {
        public class UserValue
        {
            public string Name { get; set; }
            public int Value { get; set; }

            public UserValue()
            {
            }

            public UserValue(string name, int value)
            {
                Name = name;
                Value = value;
            }
        }
        
        public class PeriodStats
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public int Amount { get; set; }
        }

        public int TotalDishCount { get; set; }
        public int DishesWithImageCount { get; set; }
        public int DishesWithoutImageCount { get; set; }
        public DateTime? FirstCreated { get; set; }
        public DateTime? LastCreated { get; set; }
        
        public int SumOfUsage { get; set; }
        public int MinOfUsage { get; set; }
        public int MaxOfUsage { get; set; }
        public decimal AvgOfUsage { get; set; }

        public string PopularDishType { get; set; }
        public int PopularDishTypeCount { get; set; }
        
        public string PopularKitchen { get; set; }
        public int PopularKitchenCount { get; set; }

        public List<UserValue> MostActive { get; set; }
        
        public List<UserValue> MostHungry { get; set; }
        
        public List<PeriodStats> DishPeriods { get; set; }
        public List<PeriodStats> ClientPeriods { get; set; }
        
    }
    
    
}