using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Bus;
using Microsoft.Reporting.WinForms;
using Model;

namespace ViewModel
{
    public class PrintBillViewModel : ViewModelBase
    {
        public ICommand LoadReport { get; }
        int _billId { get => MainViewModel.Instance.PrintBillId; set => MainViewModel.Instance.PrintBillId = value; }
        public PrintBillViewModel() 
        {
            LoadReport = new RelayCommand<ReportViewer>(rpvPrintBill => true, LoadReport_Execute);
        }

        private void LoadReport_Execute(ReportViewer rpvPrintBill)
        {
            if (_billId <= 0) throw new Exception("BillId phải lớn hơn 0.");

            BindingSource bindingSource = new BindingSource() { DataSource = BillDetailBus.Instance.GetListBillDetailByBillId(_billId) };
            ReportDataSource rds = new ReportDataSource("dsBillDetail", bindingSource);

            //var data = DataProvider.Instance.ExecuteQuery($"SELECT * FROM Bill WHERE Bill.Id = {billId}");
            var bill = BillBus.Instance.GetBillById(_billId);
            var data = new List<Bill> { bill };
            ReportDataSource rds2 = new ReportDataSource("dsBill", data);
             
            rpvPrintBill.LocalReport.ReportEmbeddedResource = "ViewModel.Images.rpPrintBill.rdlc";
            rpvPrintBill.LocalReport.DataSources.Clear();
            rpvPrintBill.LocalReport.DataSources.Add(rds);
            rpvPrintBill.LocalReport.DataSources.Add(rds2);

            rpvPrintBill.RefreshReport();

            var dateCheckOut = Convert.ToDateTime(bill.DateCheckOut);
            rpvPrintBill.LocalReport.DisplayName = $"BillId_{_billId}_" + dateCheckOut.ToString("ddMMyyy_hhmmss_tt");

            _billId = -1;
        }
    }
}
