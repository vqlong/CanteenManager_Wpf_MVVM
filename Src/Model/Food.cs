using System;
using System.Collections.Generic;
using System.Data;

namespace Model
{
    public class Food : Model
    {
        public Food()
        {

        }

        public Food(int id, string name, int categoryId, double price, UsingState foodStatus)
        {
            Id = id;
            Name = name;
            CategoryId = categoryId;
            Price = price;
            FoodStatus = foodStatus;
        }

        /// <summary>
        /// Khởi tạo đối tượng từ 1 hàng của bảng Food.
        /// </summary>
        /// <param name="row"></param>
        public Food(DataRow row)
        {
            Id = Convert.ToInt32(row["Id"]);
            Name = row["Name"].ToString();
            CategoryId = (int)row["CategoryId"];
            Price = (double)row["Price"];
            FoodStatus = (UsingState)row["FoodStatus"];
        }

        public string Name { get; set; }
        public int CategoryId { get; set; }
        public double Price { get; set; }
        public UsingState FoodStatus { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<BillInfo> BillInfos { get; set; } = new HashSet<BillInfo>();

        public override string ToString()
        {
            return base.ToString() + $" ~{GetHashCode()} {Name}";
        }
    }
}
