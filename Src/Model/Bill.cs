using System;
using System.Collections.Generic;
using System.Data;

namespace Model
{
    public class Bill : Model
    {
        public Bill()
        {

        }

        public Bill(int id, DateTime dateCheckIn, DateTime dateCheckOut, int tableId, int status, int discount = 0, double totalPrice = 0)
        {
            Id = id;
            DateCheckIn = dateCheckIn;
            DateCheckOut = dateCheckOut;
            TableId = tableId;
            Status = status;
            Discount = discount;
            TotalPrice = totalPrice;
        }

        public Bill(DataRow row)
        {
            Id = Convert.ToInt32(row["Id"]);
            DateCheckIn = Convert.ToDateTime(row["DateCheckIn"]);
            //Dưới database, mặc định khi chạy proc thêm mới 1 Bill, DateCheckOut sẽ được gán là NULL => cần kiểm tra giá trị này khi xử lý trên C#
            DateCheckOut = row["DateCheckOut"] == DBNull.Value ? null : Convert.ToDateTime(row["DateCheckOut"]);
            TableId = (int)row["TableId"];
            Status = (int)row["BillStatus"];
            Discount = (int)row["Discount"];
            TotalPrice = (double)row["TotalPrice"];
        }

        public DateTime DateCheckIn { get; set; }
        public DateTime? DateCheckOut { get; set; }
        public int TableId { get; set; }
        public int Status { get; set; }
        public int Discount { get; set; }
        public double TotalPrice { get; set; }
        public virtual Table Table { get; set; }
        public virtual ICollection<BillInfo> BillInfos { get; set; } = new HashSet<BillInfo>();
    }
}
