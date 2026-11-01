using System;
using System.Collections.Generic;
using System.Data;

namespace Model
{
    /// <summary>
    /// Bảng FoodCategory của database.
    /// </summary>
    public class Category : Model
    {
        public Category()
        {

        }

        public Category(int id, string name, UsingState categoryStatus)
        {
            Id = id;
            Name = name;
            CategoryStatus = categoryStatus;
        }

        /// <summary>
        /// Khởi tạo đối tượng từ 1 hàng của bảng FoodCategory.
        /// </summary>
        /// <param name="row"></param>
        public Category(DataRow row)
        {
            Id = Convert.ToInt32(row["Id"]);
            Name = row["Name"].ToString();
            CategoryStatus = (UsingState)row["CategoryStatus"];
        }

        public string Name { get; set; }
        public UsingState CategoryStatus { get; set; }
        public virtual ICollection<Food> Foods { get; set; } = new HashSet<Food>();

        public override string ToString()
        {
            return base.ToString() + $" ~{GetHashCode()} {Name}";
        }
    }
}
