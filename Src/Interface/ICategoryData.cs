using Model;
using System.Collections.Generic;

namespace Interface
{
    public interface ICategoryData
    {
        Category GetCategoryById(int categoryId);
        List<Category> GetListCategory();
        List<Category> GetListCategory(IEnumerable<Food> foods);
        List<Category> GetListCategoryServing();
        Category InsertCategory(string name);
        bool UpdateCategory(int id, string name, UsingState categoryStatus);
        bool DeleteCategory(int id);
    }
}