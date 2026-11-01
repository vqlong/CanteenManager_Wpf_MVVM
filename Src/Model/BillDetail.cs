using System.Data;

namespace Model
{
    /// <summary>
    /// Lớp trung gian, dùng để hiển thị thông tin của Bill lên ListView.
    /// <br>Join từ Bill, BillInfo, Food, FoodCategory.</br>
    /// </summary>
    public class BillDetail : Model
    {
        public BillDetail(string foodName, string categoryName, int foodCount, double price, double totalPrice = 0)
        {
            FoodName = foodName;
            CategoryName = categoryName;
            FoodCount = foodCount;
            Price = price;
            TotalPrice = totalPrice;
        }

        /// <summary>
        /// Khởi tạo đối tượng từ 1 hàng của các bảng Bill, BillInfo, Food, FoodCategory.
        /// </summary>
        /// <param name="row"></param>
        public BillDetail(DataRow row)
        {
            FoodName = row["FoodName"].ToString();
            CategoryName = row["CategoryName"].ToString();
            FoodCount = (int)row["FoodCount"];
            //Kiểu float trong C# phải có hậu tố "f" mà trong SQL thì không => ép về double luôn cho đỡ lằng nhằng
            Price = (double)row["Price"];
            TotalPrice = (double)row["TotalPrice"];
        }

        public string FoodName { get; set; }
        public string CategoryName { get; set; }
        public int FoodCount { get; set; }
        public double Price { get; set; }
        public double TotalPrice { get; set; }
    }
}
