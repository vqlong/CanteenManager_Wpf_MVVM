using Bus;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ViewModel
{
    public class ReportViewModel : ViewModelBase
    {
        public DateTime FirstDayInMonth { get; } = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        public DateTime LastDayInMonth { get; } = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1).AddSeconds(-1);
        public ICommand LoadReport { get; }
        public DateTime? FromDate { get => GetProperty<DateTime?>(); set => SetProperty(value); }
        public DateTime? ToDate { get => GetProperty<DateTime?>(); set => SetProperty(value); }
        public ReportViewModel()
        {
            FromDate = FirstDayInMonth;
            ToDate = LastDayInMonth;
            LoadReport = new RelayCommand<ReportViewer>(rpvPrintBill => FromDate != null && ToDate != null, LoadReport_Execute);
        }

        private void LoadReport_Execute(ReportViewer rpvRevenue)
        {
            var data = FoodBus.Instance.GetRevenueByFoodAndDate((DateTime)FromDate, (DateTime)ToDate);
            ReportDataSource rds = new ReportDataSource("dsRevenueByFoodAndDate", data);

            var data2 = BillBus.Instance.GetRevenueByMonth((DateTime)FromDate, (DateTime)ToDate);
            ReportDataSource rds2 = new ReportDataSource("dsRevenueByMonth", data2);

            rpvRevenue.LocalReport.ReportEmbeddedResource = "ViewModel.Images.rpRevenue.rdlc";
            rpvRevenue.LocalReport.DataSources.Clear();
            rpvRevenue.LocalReport.DataSources.Add(rds);
            rpvRevenue.LocalReport.DataSources.Add(rds2);
            rpvRevenue.RefreshReport();
        }
    }
}
