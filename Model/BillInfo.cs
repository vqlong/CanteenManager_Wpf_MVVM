using System.Data;

namespace Model
{
    public class BillInfo : Model
    {
        public BillInfo()
        {

        }

        public BillInfo(int id, int billId, int foodId, int foodCount)
        {
            Id = id;
            BillId = billId;
            FoodId = foodId;
            FoodCount = foodCount;
        }

        public BillInfo(DataRow row)
        {
            Id = (int)row["Id"];
            BillId = (int)row["BillId"];
            FoodId = (int)row["FoodId"];
            FoodCount = (int)row["FoodCount"];
        }

        public int BillId { get; set; }
        public int FoodId { get; set; }
        public int FoodCount { get; set; }
        public virtual Bill Bill { get; set; }  
        public virtual Food Food { get; set; }
    }
}
