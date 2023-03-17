using Interface;
using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityDataAccess
{
    public class BillData : IBillData
    {
        private BillData() { }

        public bool CheckOut(int billId, double totalPrice, int discount = 0)
        {
            using var context = new CanteenContext();

            var bill = context.Bills.Find(billId);
            if (bill == null) return false;

            bill.TotalPrice = totalPrice;
            bill.Discount = discount;
            bill.DateCheckOut = DateTime.Now;
            bill.Status = 1;

            if (context.SaveChanges() == 1) return true;
            return false;
        }

        public Bill GetBillById(int id)
        {
            using var context = new CanteenContext();
            return context.Bills.Find(id);
        }

        public List<Bill> GetListBillByDate(DateTime fromDate, DateTime toDate)
        {
            using var context = new CanteenContext();

            var bills = context.Bills.Where(b => b.DateCheckIn >= fromDate && b.DateCheckOut <= toDate && b.Status == 1)
                                    .OrderBy(b => b.Id)
                                    .Include(b => b.Table)
                                    .ToList();
            
            return bills;
        }

        public List<Bill> GetListBillByDateAndPage(DateTime fromDate, DateTime toDate, int pageNumber = 1, int pageSize = 10)
        {
            using var context = new CanteenContext();

            var bills = context.Bills.Where(b => b.DateCheckIn >= fromDate && b.DateCheckOut <= toDate && b.Status == 1)
                                    .OrderBy(b => b.Id)
                                    .Include(b => b.Table)
                                    .Skip((pageNumber - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToList();

            return bills;
        }

        public int GetNumberBillByDate(DateTime fromDate, DateTime toDate)
        {
            using var context = new CanteenContext();

            return context.Bills.Count(b => b.DateCheckIn >= fromDate && b.DateCheckOut <= toDate && b.Status == 1);
        }

        public object GetRevenueByMonth(DateTime fromDate, DateTime toDate)
        {
            using var context = new CanteenContext();

            var data = context.Bills.Where(b => b.DateCheckIn >= fromDate && b.DateCheckOut <= toDate && b.Status == 1)
                                    .Select(b => new {
                                        TotalPrice = b.TotalPrice,
                                        Month = b.DateCheckOut.Value.Month.ToString() + "-" + b.DateCheckOut.Value.Year.ToString(),
                                        FirstDayInMonth = EF.Functions.DateFromParts(b.DateCheckOut.Value.Year, b.DateCheckOut.Value.Month, 1) })
                                    .GroupBy(e => new { e.FirstDayInMonth, e.Month })
                                    .Select(g => new {
                                        FirstDayInMonth = g.Key.FirstDayInMonth,
                                        Month = g.Key.Month,
                                        Revenue = g.Sum(e => e.TotalPrice) })
                                    .OrderBy(g => g.FirstDayInMonth)
                                    .ToList();

            return data;
        }

        public int GetUnCheckBillIdByTableId(int tableId)
        {
            using var context = new CanteenContext();

            var bill = context.Bills.Where(b => b.Status == 0 && b.TableId == tableId).FirstOrDefault();

            if (bill != null) return bill.Id;
            return -1;
        }

        public int InsertBill(int tableId)
        {
            using var context = new CanteenContext();
            var bill = new Bill { TableId = tableId };
            context.Bills.Add(bill);
            if (context.SaveChanges() == 1) return bill.Id;
            return -1;
        }
    }
}
