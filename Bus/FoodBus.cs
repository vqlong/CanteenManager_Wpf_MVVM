using Interface;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace Bus
{
    public class FoodBus
    {
        private FoodBus()
        {

        }

        private static readonly FoodBus instance = new FoodBus();
        public static FoodBus Instance => instance;
        IFoodData Food => Config.Container.Resolve<IFoodData>();

        public Food GetFoodById(int foodId) => Food.GetFoodById(foodId);
        public List<Food> GetListFood() => Food.GetListFood();
        public List<Food> GetListFoodByCategoryId(int categoryId) => GetListFoodByCategoryId(categoryId);
        public object GetRevenueByFoodAndDate(DateTime fromDate, DateTime toDate) => Food.GetRevenueByFoodAndDate(fromDate, toDate);
        public Food InsertFood(string name, int categoryId, double price) => Food.InsertFood(name, categoryId, price);
        public List<Food> SearchFood(string input, bool option) => Food.SearchFood(input, option);
        public bool UpdateFood(int id, string name, int categoryId, double price, UsingState foodStatus) => Food.UpdateFood(id, name, categoryId, price, foodStatus);
        public bool DeleteFood(int Id) => Food.DeleteFood(Id);
    }
}
