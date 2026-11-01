using Interface;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Media;

namespace SQLiteDataAccess
{
    public class CategoryData : DataAccess, ICategoryData
    {
        private CategoryData() { }
        private static readonly CategoryData instance = new CategoryData();
        public static CategoryData Instance => instance;

        /// <summary>
        /// Lấy tất cả dữ liệu trong bảng FoodCategory từ database để tạo các đối tượng Category và đưa vào danh sách.
        /// </summary>
        /// <returns></returns>
        public List<Category> GetListCategory()
        {
            List<Category> listCategory = new List<Category>();

            DataTable data = DataProvider.Instance.ExecuteQuery("SELECT Id FROM FoodCategory");

            foreach (DataRow row in data.Rows)
            {
                var id = Convert.ToInt32(row["Id"]);
                var foods = FoodData.Instance.GetListFoodByCategoryId(id);
                if(foods.Count > 0) listCategory.Add(foods.First().Category);
            }

            return listCategory;
        }

        /// <summary>
        /// Tạo danh sách category từ danh sách food.
        /// </summary>
        /// <param name="foods"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<Category> GetListCategory(IEnumerable<Food> foods)
        {
            if (foods.Any(f => f.Category == null)) throw new Exception("Mỗi food phải bao gồm category.");

            List<Category> listCategory = new List<Category>();

            foreach (var food in foods)
            {
                if (listCategory.Contains(food.Category)) continue;
                else listCategory.Add(food.Category);
            }

            //DataTable data = DataProvider.Instance.ExecuteQuery("SELECT * FROM FoodCategory");

            //foreach (DataRow row in data.Rows)
            //{
            //    Category category = new Category(row);
            //    category.Foods = FoodData.Instance.GetListFoodByCategoryId(category.Id);
            //    listCategory.Add(category);
            //}

            return listCategory;
        }
        /// <summary>
        /// Trả về list các Category có trạng thái là đang phục vụ (UsingState.Serving).
        /// </summary>
        /// <returns></returns>
        public List<Category> GetListCategoryServing()
        {
            List<Category> listCategory = new List<Category>();

            string query = "SELECT * FROM FoodCategory WHERE CategoryStatus = 1";

            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach (DataRow row in data.Rows)
            {
                Category category = new Category(row);

                var foods = FoodData.Instance.GetListFoodByCategoryId(category.Id).Where(f => f.FoodStatus == UsingState.Serving).ToList();
                foods.ForEach(f => f.Category = category);
                category.Foods = foods;

                listCategory.Add(category);
            }

            return listCategory;
        }

        /// <summary>
        /// Tạo 1 đối tượng Category dựa vào Id.
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public Category GetCategoryById(int categoryId)
        {
            string query = "SELECT * FROM FoodCategory WHERE ID = " + categoryId;

            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            Category category = new Category(data.Rows[0]);
            var foods = FoodData.Instance.GetListFoodByCategoryId(category.Id);
            foods.ForEach(f => f.Category = category);
            category.Foods = foods;

            return category;
        }

        public Category InsertCategory(string name)
        {
            string query = $"INSERT INTO FoodCategory(Name) VALUES( @name )";

            int result = DataProvider.Instance.ExecuteNonQuery(query, new object[] {name});

            if (result == 1)
            {
                query = "SELECT * FROM FoodCategory WHERE Id = (SELECT Max(Id) FROM FoodCategory) ;";

                DataTable data = DataProvider.Instance.ExecuteQuery(query);

                if (data.Rows.Count > 0)
                {
                    Category category = new Category(data.Rows[0]);

                    return category;
                }
                return null;
            }

            return null;
        }

        public bool UpdateCategory(int id, string name, UsingState categoryStatus)
        {
            string query = $"UPDATE FoodCategory SET Name = @name, CategoryStatus = @categoryStatus WHERE ID = @id ";

            int result = DataProvider.Instance.ExecuteNonQuery(query, new object[] { name, (int)categoryStatus, id });

            if (result == 1)
            {
                query = $"UPDATE Food SET FoodStatus = @categoryStatus WHERE CategoryId = @categoryId ";
                result = DataProvider.Instance.ExecuteNonQuery(query, new object[] { (int)categoryStatus, id });
                if(result >= 1) return true;
                return false;
            }
            else
            {
                return false;
            }
            
        }

        public bool DeleteCategory(int id)
        {
            string query = @$"UPDATE [FoodCategory]
                                 SET [CategoryStatus] = 0 
                               WHERE [FoodCategory].[Id] = @id ;";

            int result = DataProvider.Instance.ExecuteNonQuery(query, new object[] { id });

            if (result == 1)
            {
                query = $"UPDATE Food SET FoodStatus = 0 WHERE CategoryId = @categoryId ";
                result = DataProvider.Instance.ExecuteNonQuery(query, new object[] { id });
                if (result >= 1) return true;

                return false;
            }

            return false;
        }

    }
}
