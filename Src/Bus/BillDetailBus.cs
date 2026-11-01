using Interface;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace Bus
{
    public class BillDetailBus
    {
        private BillDetailBus()
        {

        }
        private static readonly BillDetailBus instance = new BillDetailBus();
        public static BillDetailBus Instance => instance;
        IBillDetailData BillDetail => Config.Container.Resolve<IBillDetailData>();
        public List<BillDetail> GetListBillDetailByBillId(int billId) => BillDetail.GetListBillDetailByBillId(billId);
        public List<BillDetail> GetListBillDetailByTableId(int tableId) => BillDetail.GetListBillDetailByTableId(tableId);
    }
}
