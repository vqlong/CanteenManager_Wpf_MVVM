using Bus;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ViewModel
{
    public class BillViewModel : ViewModelBase
    {
        public DateTime FirstDayInMonth { get; } = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        public DateTime LastDayInMonth { get; } = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1).AddSeconds(-1);
        public DateTime FromDate 
        { 
            get => GetProperty<DateTime>(); 
            set  
            { 
                SetProperty(value); 
                SetBillsByDateAndPage(value, ToDate, (int)PageNumber, (int)PageSize, IsDateAndPage);
                SetMaxPage(value, ToDate, (int)PageSize);
            } 
        }
        public DateTime ToDate 
        { 
            get => GetProperty<DateTime>(); 
            set 
            { 
                SetProperty(value); 
                SetBillsByDateAndPage(FromDate, value, (int)PageNumber, (int)PageSize, IsDateAndPage);
                SetMaxPage(FromDate, value, (int)PageSize);
            } 
        }
        public double PageNumber { get => GetProperty<double>(); set { SetProperty(value); SetBillsByDateAndPage(FromDate, ToDate, (int)value, (int)PageSize, IsDateAndPage); } }
        public double PageSize 
        { 
            get => GetProperty<double>(); 
            set 
            { 
                SetProperty(value); 
                SetBillsByDateAndPage(FromDate, ToDate, (int)PageNumber, (int)value, IsDateAndPage);
                SetMaxPage(FromDate, ToDate, (int)value);
            } 
        }
        public bool IsDateAndPage { get => GetProperty<bool>(); set { SetProperty(value); SetBillsByDateAndPage(FromDate, ToDate, (int)PageNumber, (int)PageSize, value); } }
        public List<Bill> Bills { get => GetProperty<List<Bill>>(); set { SetProperty(value); if (value != null) BillsTotalPrice = value.Sum(b => b.TotalPrice); } }
        public double BillsTotalPrice { get => GetProperty<double>(); set => SetProperty(value); }
        public int MaxPage { get => GetProperty<int>(); set => SetProperty(value); }
        public object SelectedItem { get => GetProperty<object>(); set => SetProperty(value); }
        public ICommand PrintBill { get; }
        public int BillId { get => GetProperty<int>(); set => SetProperty(value); }

        public BillViewModel()
        {
            FromDate = FirstDayInMonth;
            ToDate = LastDayInMonth;
            PageNumber = 1;
            PageSize = 20;
            IsDateAndPage = true;
            PrintBill = new RelayCommand<Type>(type => type.IsSubclassOf(typeof(Window)), PrintBill_Execute);
        }
        private void SetBillsByDateAndPage(DateTime fromDate, DateTime toDate, int pageNumber = 1, int pageSize = 10, bool isDateAndPage = true)
        {
            if (pageNumber <= 0 || pageSize <= 0) return;
            if (isDateAndPage) Bills = BillBus.Instance.GetListBillByDateAndPage(fromDate, toDate, pageNumber, pageSize);
            else Bills = BillBus.Instance.GetListBillByDate(fromDate, toDate);
        }
        private void SetMaxPage(DateTime fromDate, DateTime toDate, int pageSize)
        {
            if (pageSize == 0) return;

            var totalRow = BillBus.Instance.GetNumberBillByDate(fromDate, toDate);

            //Trong trường hợp người dùng chọn ngày lung tung, totalRow trả về kết quả 0 => thoát
            if (totalRow <= 0) return;

            var lastPage = totalRow / pageSize;

            if (totalRow % pageSize != 0) lastPage++;

            MaxPage = lastPage;
        }
        private void PrintBill_Execute(Type type)
        {
            var billId = 0;
            if (SelectedItem != null && int.TryParse(SelectedItem.GetType().GetProperty("Id")?.GetValue(SelectedItem)?.ToString(), out billId)) { }
            MainViewModel.Instance.PrintBillId = billId;
            if (Activator.CreateInstance(type) is Window window) window.ShowDialog();
            MainViewModel.Instance.PrintBillId = -1;
        }
    }
}
