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
    public class CategoryBus
    {
        private CategoryBus()
        {

        }

        private static readonly CategoryBus instance = new CategoryBus();
        public static CategoryBus Instance => instance;
        ICategoryData Category => Config.Container.Resolve<ICategoryData>();

        public Category GetCategoryById(int categoryId) => Category.GetCategoryById(categoryId);
        public List<Category> GetListCategory() => Category.GetListCategory();
        public List<Category> GetListCategory(IEnumerable<Food> foods) => Category.GetListCategory(foods);
        public List<Category> GetListCategoryServing()=> Category.GetListCategoryServing();
        public Category InsertCategory(string name) => Category.InsertCategory(name);
        public bool UpdateCategory(int id, string name, UsingState categoryStatus) => Category.UpdateCategory(id, name, categoryStatus);
        public bool DeleteCategory(int id) => Category.DeleteCategory(id);
    }
}
