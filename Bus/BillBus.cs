using Interface;
using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace Bus
{
    public class BillBus
    {
        private BillBus()
        {

        }
        private static readonly BillBus instance = new BillBus();
        public static BillBus Instance => instance;
        IBillData Bill => Config.Container.Resolve<IBillData>();
        public bool CheckOut(int billId, double totalPrice, int discount = 0) => Bill.CheckOut(billId, totalPrice, discount);
        public Bill GetBillById(int id) => Bill.GetBillById(id);
        public List<Bill> GetListBillByDate(DateTime fromDate, DateTime toDate) => Bill.GetListBillByDate(fromDate, toDate);
        public List<Bill> GetListBillByDateAndPage(DateTime fromDate, DateTime toDate, int pageNumber = 1, int pageSize = 10) => Bill.GetListBillByDateAndPage(fromDate, toDate, pageNumber, pageSize);
        public int GetNumberBillByDate(DateTime fromDate, DateTime toDate) => Bill.GetNumberBillByDate(fromDate, toDate);
        public object GetRevenueByMonth(DateTime fromDate, DateTime toDate) =>Bill.GetRevenueByMonth(fromDate, toDate);
        public int GetUnCheckBillIdByTableId(int tableId) => Bill.GetUnCheckBillIdByTableId(tableId);
        public int InsertBill(int tableId) => Bill.InsertBill(tableId);
    }
}
