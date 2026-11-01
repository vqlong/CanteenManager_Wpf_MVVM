using Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace Bus
{
    public class BillInfoBus
    {
        private BillInfoBus()
        {

        }
        private static readonly BillInfoBus instance = new BillInfoBus();
        public static BillInfoBus Instance => instance;
        IBillInfoData BillInfo => Config.Container.Resolve<IBillInfoData>();
        public int InsertBillInfo(int billId, int foodId, int foodCount) => BillInfo.InsertBillInfo(billId, foodId, foodCount);
    }
}
