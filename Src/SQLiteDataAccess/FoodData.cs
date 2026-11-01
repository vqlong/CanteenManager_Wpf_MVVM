using Interface;
using Model;
using System;
using System.Collections.Generic;
using System.Data;

namespace SQLiteDataAccess
{
    public class FoodData : DataAccess, IFoodData
    {
        private FoodData() { }
        private static readonly FoodData instance = new FoodData();
        public static FoodData Instance => instance;

        /// <summary>
        /// Lấy danh sách các Food dựa theo 1 CategoryID.
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public List<Food> GetListFoodByCategoryId(int categoryId)
        { 
            DataTable data1 = DataProvider.Instance.ExecuteQuery("SELECT * FROM FoodCategory WHERE ID = " + categoryId);

            Category category = new Category(data1.Rows[0]);

            List<Food> listFood = new List<Food>();
 
            DataTable data = DataProvider.Instance.ExecuteQuery("select * from Food where FoodStatus = 1 and CategoryId = " + categoryId);
            foreach (DataRow row in data.Rows)
            {
                Food food = new Food(row);
                food.Category = category;
                category.Foods.Add(food);
                listFood.Add(food);
            }

            return listFood;

            //List<Food> foods = GetListFood();
            //foreach (var food in foods)
            //{
            //    if (food.CategoryId != categoryId) foods.Remove(food);
            //}
            //return foods;
        }

        /// <summary>
        /// Lấy danh sách các Food kèm theo Category.
        /// </summary>
        /// <returns></returns>
        public List<Food> GetListFood()
        {
            List<Category> listCategory = new List<Category>();

            DataTable data1 = DataProvider.Instance.ExecuteQuery("SELECT * FROM FoodCategory");

            foreach (DataRow row in data1.Rows)
            {
                Category category = new Category(row);
                listCategory.Add(category);
            }

            List<Food> listFood = new List<Food>(); 

            DataTable data2 = DataProvider.Instance.ExecuteQuery("SELECT * FROM Food ORDER BY CategoryId ASC");

            foreach (DataRow row in data2.Rows)
            {
                Food food = new Food(row);
                foreach (var category in listCategory)
                {
                    if (category.Id == food.CategoryId)
                    {
                        category.Foods.Add(food);
                        food.Category = category;
                        break;
                    }
                }
                listFood.Add(food);
            }

            return listFood;
        }

        public Food InsertFood(string name, int categoryId, double price)
        {
            string query = $"INSERT INTO Food(Name, CategoryId, Price) VALUES( @name , @categoryId , @price ) ;";

            int result = DataProvider.Instance.ExecuteNonQuery(query, new object[] {name, categoryId, price});
            
            if (result == 1)
            { 
                DataTable data = DataProvider.Instance.ExecuteQuery("SELECT * FROM Food WHERE Id = (SELECT Max(Id) FROM Food) ;");

                if(data.Rows.Count > 0)
                {
                    Food food = new Food(data.Rows[0]);

                    return food;
                }
                return null;
            }

            return null;
        }

        public bool UpdateFood(int id, string name, int categoryId, double price, UsingState foodStatus)
        {
            var category = CategoryData.Instance.GetCategoryById(categoryId);
            if (category.CategoryStatus == UsingState.StopServing) return false;

            string query = @$"UPDATE [Food]
                                 SET [Name] = @name ,
                                     [CategoryId] = @categoryId ,
                                     [Price] = @price ,
                                     [FoodStatus] = @foodStatus 
                               WHERE [Food].[Id] = @id ;";

            int result = DataProvider.Instance.ExecuteNonQuery(query, new object[] { name, categoryId, price, (int)foodStatus, id });
            
            if (result == 1) return true;

            return false;
        }

        /// <summary>
        /// Tìm Food.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="option">True để bỏ qua phân biệt Unicode, ngược lại, False.</param>
        /// <returns></returns>
        public List<Food> SearchFood(string input, bool option = true)
        {
            List<Food> listFood = GetListFood();

            //Tìm kiếm không phân biệt unicode
            if (option)
            {
                listFood.RemoveAll(food => ConvertToUnsigned(food.Name.ToLower()).Contains(ConvertToUnsigned(input.ToLower().Trim())) == false);

                return listFood;

            }

            //Phân biệt unicode
            listFood.RemoveAll(food => food.Name.ToLower().Contains(input.ToLower().Trim()) == false);

            return listFood;

        }

        /// <summary>
        /// Lấy tổng doanh thu (chưa tính giảm giá) của từng món ăn dựa theo ngày truyền vào.
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public object GetRevenueByFoodAndDate(DateTime fromDate, DateTime toDate)
        {
            string query = @$"WITH [temp] AS (
                            SELECT [BillInfo].[Id],
                                   [BillId],
                                   [FoodId],
                                   [Name],
                                   [FoodCount],
                                   [Price],
                                   [FoodCount] * [Price] AS [TotalPrice],
                                   [DateCheckIn],
                                   [DateCheckOut]
                              FROM [BillInfo]
                              JOIN [Food] 
                                ON [Food].[Id] = [BillInfo].[FoodId]
                              JOIN [Bill] 
                                ON [Bill].[Id] = [BillInfo].[BillId] AND 
                                   [BillStatus] = 1 AND 
                                   [DateCheckIn] >= '{fromDate.ToString("o")}' AND 
                                   [DateCheckOut] <= '{toDate.ToString("o")}'
                                    )
                            SELECT [temp].[Name],
                                   SUM([temp].[FoodCount]) AS [TotalFoodCount],
                                   SUM([temp].[TotalPrice]) AS [Revenue]
                              FROM [temp]
                          GROUP BY [temp].[Name];";

            return DataProvider.Instance.ExecuteQuery(query);
        }

        /// <summary>
        /// Chuyển chuỗi sang dạng không dấu.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string ConvertToUnsigned(string input)
        {
            string   signed = "ăâđêôơưàảãạáằẳẵặắầẩẫậấèẻẽẹéềểễệế ìỉĩịíòỏõọóồổỗộốờởỡợớùủũụúừửữựứỳỷỹỵý ĂÂĐÊÔƠƯÀẢÃẠÁẰẲẴẶẮẦẨẪẬẤÈẺẼẸÉỀỂỄỆẾÌỈĨỊÍ ÒỎÕỌÓỒỔỖỘỐỜỞỠỢỚÙỦŨỤÚỪỬỮỰỨỲỶỸỴÝ";
            string unsigned = "aadeoouaaaaaaaaaaaaaaaeeeeeeeeee iiiiiooooooooooooooouuuuuuuuuuyyyyy AADEOOUAAAAAAAAAAAAAAAEEEEEEEEEEIIIII OOOOOOOOOOOOOOOUUUUUUUUUUYYYYY";

            for (int i = 0; i < input.Length; i++)
            {
                if (signed.Contains(input[i]))
                    input = input.Replace(input[i], unsigned[signed.IndexOf(input[i])]);
            }

            return input;
        }

        public Food GetFoodById(int Id)
        { 
            DataTable data = DataProvider.Instance.ExecuteQuery("SELECT * FROM Food WHERE Id = " + Id);

            if(data.Rows.Count > 0)
            {
                Food food = new Food(data.Rows[0]);
                DataTable data1 = DataProvider.Instance.ExecuteQuery("SELECT * FROM FoodCategory WHERE ID = " + food.CategoryId);
                Category category = new Category(data1.Rows[0]);
                food.Category = category;

                return food;
            }
            return null;
        }

        public bool DeleteFood(int id)
        {
            string query = @$"UPDATE [Food]
                                 SET [FoodStatus] = 0 
                               WHERE [Food].[Id] = @id ;";

            int result = DataProvider.Instance.ExecuteNonQuery(query, new object[] { id });

            if (result == 1) return true;

            return false;
        }
    }
}
