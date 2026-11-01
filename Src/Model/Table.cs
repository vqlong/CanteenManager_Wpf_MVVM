using System;
using System.Collections.Generic;
using System.Data;

namespace Model
{
    /// <summary>
    /// Bảng TableFood của database.
    /// </summary>
    public class Table : Model
    {
        public Table() { }

        public Table(int id, string name, string status, UsingState usingState)
        {
            Id = id;
            Name = name;
            Status = status;
            UsingState = usingState;
        }

        /// <summary>
        /// Khởi tạo đối tượng từ 1 hàng của bảng TableFood.
        /// </summary>
        /// <param name="row"></param>
        public Table(DataRow row)
        {
            Id = Convert.ToInt32(row["Id"]);
            Name = row["Name"].ToString();
            Status = row["TableStatus"].ToString();
            UsingState = (UsingState)row["UsingState"];
        }

        public string Name { get; set; } = "Bàn ăn";
        public string Status { get; set; } = "Trống";
        public UsingState UsingState { get; set; }
        public virtual ICollection<Bill> Bills { get; set; } = new HashSet<Bill>();

        public override string ToString()
        {
            return base.ToString() + $" ~{GetHashCode()} {Name}";
        }
    }
}
