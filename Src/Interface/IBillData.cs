using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Windows.Documents;

namespace Interface
{
    public interface IBillData
    {
        bool CheckOut(int billId, double totalPrice, int discount = 0);
        Bill GetBillById(int id);
        List<Bill> GetListBillByDate(DateTime fromDate, DateTime toDate);
        List<Bill> GetListBillByDateAndPage(DateTime fromDate, DateTime toDate, int pageNumber = 1, int pageSize = 10);
        int GetNumberBillByDate(DateTime fromDate, DateTime toDate);
        object GetRevenueByMonth(DateTime fromDate, DateTime toDate);
        int GetUnCheckBillIdByTableId(int tableId);
        int InsertBill(int tableId);
    }
}