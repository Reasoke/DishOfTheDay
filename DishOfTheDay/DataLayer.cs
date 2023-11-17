using DishOfTheDay.Entity;
using DishOfTheDay.Repository;
using System.Collections.Generic;
using System.Linq;

namespace DishOfTheDay
{
    public class DataLayer
    {
        #region Singleton pattern

        private static DataLayer instance;
        public static DataLayer Instance { 
            get
            {
                if (instance == null)
                    instance = new DataLayer();
                return instance;
            } 
        }

        private DataLayer()
        {
            //private -> to be not allowed to create instance out of this class
        }

        #endregion

        #region Dish

        private readonly DishRepository dishRepository = new DishRepository();

        public List<DishEntity> GetDishes(string search, int sortIndex, bool sortAsc, int minCookingTime,int maxCookingTime,
            int minIngredientCount, int maxIngredientCount, int dishTypeId, int kitchenId, bool? hasPicture)
        {
            return dishRepository.GetAll(search, sortIndex, sortAsc, minCookingTime, maxCookingTime,
                        minIngredientCount, maxIngredientCount, dishTypeId, kitchenId, hasPicture).ToList();
        }

        public void SaveDish(DishEntity item, List<DishIngredientEntity> currentIngredients)
        {
            if (item.dish_id > 0)
                dishRepository.Update(item, currentIngredients);
            else
                dishRepository.Insert(item, currentIngredients);
        }

        public void DeleteDish(int id)
        {
            dishRepository.Delete(id);
        }

        internal List<DishIngredientEntity> GetIngredientsByDish(int dish_id)
        {
            return dishRepository.GetIngredients(dish_id).ToList();
        }

        #endregion

        #region Kitchen

        private readonly KitchenRepository kitchenRepository = new KitchenRepository();
        private List<KitchenEntity> kitchens;
        public List<KitchenEntity> Kitchens
        {
            get
            {
                if (kitchens == null)
                    kitchens = kitchenRepository.GetAll().ToList();
                return kitchens;
            }
        }

        private readonly KitchenEntity emptyKitchen = new KitchenEntity {kitchen_id = -1, name = ""};
        public List<KitchenEntity> KitchensFilter
        {
            get
            {
                var result = new List<KitchenEntity>();
                result.Add(emptyKitchen);
                result.AddRange(Kitchens);
                return result;
            }
        }

        public void RefreshKitchens()
        {
            kitchens = kitchenRepository.GetAll().ToList();
        }

        public List<KitchenEntity> GetKitchens(string search, int sortIndex, bool sortAsc)
        {
            return kitchenRepository.GetAll(search, sortIndex, sortAsc).ToList();
        }

        public void SaveKitchen(KitchenEntity item)
        {
            if (item.kitchen_id > 0)
                kitchenRepository.Update(item);
            else
                kitchenRepository.Insert(item);
            //update cachesd items
            RefreshKitchens();
        }

        public void DeleteKitchen(int id)
        {
            kitchenRepository.Delete(id);
            //update cachesd items
            RefreshKitchens();
        }

        #endregion

        #region DishType

        private readonly DishTypeRepository dishTypeRepository = new DishTypeRepository();
        private List<DishTypeEntity> dishTypes;
        public List<DishTypeEntity> DishTypes
        {
            get
            {
                if (dishTypes == null)
                    dishTypes = dishTypeRepository.GetAll().ToList();
                return dishTypes;
            }
        }

        private readonly DishTypeEntity emptyDishType = new DishTypeEntity {dish_type_id = -1, name = ""};
        public List<DishTypeEntity> DishTypesFilter
        {
            get
            {
                var result = new List<DishTypeEntity>();
                result.Add(emptyDishType);
                result.AddRange(DishTypes);
                return result;
            }
        }

        public void RefreshDishTypes()
        {
            dishTypes = dishTypeRepository.GetAll().ToList();
        }

        public List<DishTypeEntity> GetDithTypes(string search, int sortIndex, bool sortAsc)
        {
            return dishTypeRepository.GetAll(search, sortIndex, sortAsc).ToList();
        }

        public void SaveDishType(DishTypeEntity item)
        {
            if (item.dish_type_id > 0)
                dishTypeRepository.Update(item);
            else
                dishTypeRepository.Insert(item);
            //update cachesd items
            RefreshDishTypes();
        }

        public void DeleteDishType(int id)
        {
            dishTypeRepository.Delete(id);
            //update cachesd items
            RefreshDishTypes();
        }

        #endregion

        #region Client

        private readonly ClientRepository clientRepository = new ClientRepository();

        public List<ClientEntity> GetClients(string search, int sortIndex, bool sortAsc, bool? phone, bool? address, bool? desc,
            int minDishes, int maxDishes)
        {
            return clientRepository.GetAll(search, sortIndex, sortAsc, phone, address, desc, minDishes, maxDishes).ToList();
        }
        
        public void SaveClient(ClientEntity item)
        {
            if (item.client_id > 0)
                clientRepository.Update(item);
            else
                clientRepository.Insert(item);
        }

        public void DeleteClient(int id)
        {
            clientRepository.Delete(id);
        }

        #endregion

        #region Ingredient

        private readonly IngredientRepository ingredientRepository = new IngredientRepository();
        private List<IngredientEntity> ingredients;
        public List<IngredientEntity> Ingredients
        {
            get
            {
                if (ingredients == null)
                    ingredients = ingredientRepository.GetAll().ToList();
                return ingredients;
            }
        }

        public void RefreshIngredients()
        {
            ingredients = ingredientRepository.GetAll().ToList();
        }

        internal List<IngredientEntity> GetIngredients(string search, int sortIndex, bool sortAsc, int minPrice, int maxPrice, string units, string manufacturer)
        {
            return ingredientRepository.GetAll(search, sortIndex, sortAsc, minPrice, maxPrice, units, manufacturer).ToList();
        }

        public void SaveIngredient(IngredientEntity item)
        {
            if (item.ingredient_id > 0)
                ingredientRepository.Update(item);
            else
                ingredientRepository.Insert(item);
            //update cachesd items
            RefreshIngredients();
        }

        public void DeleteIngredient(int id)
        {
            ingredientRepository.Delete(id);
            //update cachesd items
            RefreshIngredients();
        }

        public string[] GetIngredientUnits()
        {
            return ingredientRepository.GetIngredientUnits().ToArray();
        }

        public string[] GetIngredientManufacturers()
        {
            return ingredientRepository.GetIngredientManufacturers().ToArray();
        }

        private readonly BaseRepository baseRepository = new BaseRepository();
        public StatisticsInfo GetStatistics()
        {
            return baseRepository.GetStatistics();
        }

        #endregion
    }
}
