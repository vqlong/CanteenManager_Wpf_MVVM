using Model;
using System.Collections.Generic;

namespace Interface
{
    public interface IBillDetailData
    {
        List<BillDetail> GetListBillDetailByBillId(int billId);
        List<BillDetail> GetListBillDetailByTableId(int tableId);
    }
}