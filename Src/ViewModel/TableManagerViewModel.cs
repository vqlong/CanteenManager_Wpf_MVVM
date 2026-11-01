using Bus;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WpfLibrary;
using WpfLibrary.UserControls;
using MediaPlayer = WpfLibrary.UserControls.MediaPlayer;
using Table = Model.Table;

namespace ViewModel
{
    public class TableManagerViewModel : ViewModelBase
    {
        public Account LoginAccount { get => MainViewModel.Instance.LoginAccount; internal set => MainViewModel.Instance.LoginAccount = value; }
        public MediaPlayer MediaPlayer { get => GetProperty<MediaPlayer>(); set => SetProperty(value); }
        public ICommand PlayMedia { get; }
        public ICommand CloseMedia { get; }
        public ListCollectionView UsingCategoriesView => MainViewModel.Instance.UsingCategoriesView;
        public ListCollectionView UsingTablesView => MainViewModel.Instance.UsingTablesView;
        public ListCollectionView UsingTablesView2 => MainViewModel.Instance.UsingTablesView2; 
        public ICommand AddFood { get; }
        public double FoodCount { get => GetProperty<double>(); set => SetProperty(value); }
        public Food SelectedFood { get => GetProperty<Food>(); set => SetProperty(value); } 
        public double TotalPrice { get => GetProperty<double>(); set => SetProperty(value); }
        public List<BillDetail> BillDetails { get => GetProperty<List<BillDetail>>(); set { SetProperty(value); if(value != null) TotalPrice = value.Sum(bd => bd.TotalPrice); } }
        public Table SelectedTable { get => GetProperty<Table>(); set { SetProperty(value); if(value != null) BillDetails = BillDetailBus.Instance.GetListBillDetailByTableId(value.Id); } }
        public ICommand Checkout { get; }
        public double Discount { get => GetProperty<double>(); set => SetProperty(value); }
        public ICommand PrintBill { get; }
        bool _canPrintBill;
        int _billId;
        public ICommand SwapTable { get; }
        public ICommand CombineTable { get; }
        public ICommand SetIndex { get; } = new RelayCommand<Selector>(selector => true, selector => selector.SelectedIndex = 0);
        public TableManagerViewModel()
        {
            //UsingCategoriesView.CurrentChanged += (s, e) =>
            //{ 
            //    if (UsingCategoriesView.CurrentItem is Category category)
            //    {
            //        var view = CollectionViewSource.GetDefaultView(category.Foods);
            //        //Vấn đề thằng cbFoods luôn null SelectedItem lúc mới load
            //        if(view.CurrentItem == null) SelectedFoodIndex = 0;
            //    }
            //};

            MediaPlayer = new MediaPlayer();

            PlayMedia = new RelayCommand<Storyboard>(storyboard => true, PlayMedia_Execute);
            CloseMedia = new RelayCommand<object>(obj => true, CloseMedia_Execute);

            AddFood = new RelayCommand<object>(obj => true, AddFood_Execute);
            Checkout = new RelayCommand<object>(obj => TotalPrice > 0, Checkout_Execute);
            PrintBill = new RelayCommand<Type>(type => type.IsSubclassOf(typeof(Window)), PrintBill_Execute);
            SwapTable = new RelayCommand<object>(SwapTable_CanExecute, SwapTable_Execute);
            CombineTable = new RelayCommand<object>(CombineTable_CanExecute, CombineTable_Execute);
        }

        private bool CombineTable_CanExecute(object obj)
        {
            if (UsingTablesView.CurrentItem is Table table1 && UsingTablesView2.CurrentItem is Table table2)
            {
                if (table1.Status == "Có người" && table2.Status == "Có người" && table1.Id != table2.Id) return true;
            }
            return false;
        }
        private void CombineTable_Execute(object obj)
        {
            if(UsingTablesView.CurrentItem is Table table1 && UsingTablesView2.CurrentItem is Table table2)
            {
                if (DialogBox.Show($"Bạn có thực sự muốn gộp {table1.Name} vào {table2.Name}?", "Thông báo", DialogBoxButton.YesNo, DialogBoxIcon.Question) == DialogBoxResult.Yes)
                {
                    TableBus.Instance.CombineTable(table1.Id, table2.Id);

                    var newTable1 = TableBus.Instance.GetTableById(table1.Id);
                    Reload<Table>(MainViewModel.Instance.Tables, newTable1);

                    var newTable2 = TableBus.Instance.GetTableById(table2.Id);
                    Reload<Table>(MainViewModel.Instance.Tables, newTable2);

                    UsingTablesView.MoveCurrentTo(newTable2);
                    UsingTablesView2.MoveCurrentTo(newTable2);
                }
            }
        }
        private bool SwapTable_CanExecute(object obj)
        {
            if (UsingTablesView.CurrentItem is Table table1 && UsingTablesView2.CurrentItem is Table table2)
            {
                if (table1.Status == "Có người" && table1.Id != table2.Id) return true;
            }
            return false;
        }
        private void SwapTable_Execute(object obj)
        {
            if (UsingTablesView.CurrentItem is Table table1 && UsingTablesView2.CurrentItem is Table table2)
            {
                if (DialogBox.Show($"Bạn có thực sự muốn chuyển {table1.Name} tới {table2.Name}?", "Thông báo", DialogBoxButton.YesNo, DialogBoxIcon.Question) == DialogBoxResult.Yes)
                {
                    TableBus.Instance.SwapTable(table1.Id, table2.Id);

                    var newTable1 = TableBus.Instance.GetTableById(table1.Id);
                    Reload<Table>(MainViewModel.Instance.Tables, newTable1); 

                    var newTable2 = TableBus.Instance.GetTableById(table2.Id);
                    Reload<Table>(MainViewModel.Instance.Tables, newTable2);

                    UsingTablesView.MoveCurrentTo(newTable2);
                    UsingTablesView2.MoveCurrentTo(newTable2);
                }
            }
        }
        private void PrintBill_Execute(Type type)
        {
            if (_canPrintBill is false) return;

            //Command OpenWindow cũng tạo mới 1 window PrintBillView nhưng nó không gán giá trị cho PrintBillId nên sẽ có exception
            MainViewModel.Instance.PrintBillId = _billId;
            if (Activator.CreateInstance(type) is Window window) window.ShowDialog();

            MainViewModel.Instance.PrintBillId = -1;
            _canPrintBill = false;
        }
        private void Checkout_Execute(object obj)
        {
            if (TotalPrice == 0) return;

            if (UsingTablesView.CurrentItem is not Table table) return;

            _billId = BillBus.Instance.GetUnCheckBillIdByTableId(table.Id);

            if (_billId != -1)
            {
                int discount = (int)Discount;

                double billFinalPrice = TotalPrice - TotalPrice * discount / 100;

                System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("vi-vn");

                if (DialogBox.Show($"Bạn có muốn thanh toán hoá đơn cho\n{table.Name}?\nTổng tiền:\t {TotalPrice.ToString("c1", culture)}\nGiảm giá:\t {discount}%\nPhải trả:\t\t {billFinalPrice.ToString("c1", culture)}", "Thông báo", DialogBoxButton.YesNo, DialogBoxIcon.Question) == DialogBoxResult.Yes)
                {
                    var result = BillBus.Instance.CheckOut(_billId, billFinalPrice, discount);

                    Log.Info($"Checkout Bill - Id: {_billId}, Result: {result}.");

                    if (result == false)
                    {
                        DialogBox.Show("Có lỗi trong quá trình thanh toán.\nThanh toán thất bại.");
                        return;
                    }

                    if (DialogBox.Show("Bạn có muốn in hoá đơn?", "Thông báo", DialogBoxButton.YesNo, DialogBoxIcon.Question) == DialogBoxResult.Yes)
                    {                        
                        _canPrintBill = true;
                    }
                    else
                    { 
                        _canPrintBill = false;
                    }

                    var newTable = TableBus.Instance.GetTableById(table.Id);
                    Reload<Table>(MainViewModel.Instance.Tables, newTable);
                    UsingTablesView.MoveCurrentTo(newTable);
                }
            }
        }
        private void AddFood_Execute(object obj)
        {
            if (UsingTablesView.CurrentItem == null)
            {
                DialogBox.Show("Chưa có bàn ăn nào được chọn.");
                return;
            } 

            if (FoodCount == 0) return;

            int foodId = SelectedFood is null ? 0 : SelectedFood.Id;
            if (foodId == 0) return;

            var tableId = UsingTablesView.CurrentItem is Table table ? table.Id : 0;
            if (tableId == 0) return;

            var billId = BillBus.Instance.GetUnCheckBillIdByTableId(tableId);

            if (billId == -1)
            {
                if (FoodCount < 0) return;

                var newBillId = BillBus.Instance.InsertBill(tableId);

                Log.Info($"Insert Bill - Id: {newBillId}.");

                BillInfoBus.Instance.InsertBillInfo(newBillId, foodId, (int)FoodCount);
            }
            else
            {
                BillInfoBus.Instance.InsertBillInfo(billId, foodId, (int)FoodCount);
            }

            var newTable = TableBus.Instance.GetTableById(tableId);
            Reload<Table>(MainViewModel.Instance.Tables, newTable);
            UsingTablesView.MoveCurrentTo(newTable);
        }
        private void PlayMedia_Execute(Storyboard storyboard)
        {
            if (MediaPlayer is null || MediaPlayer.IsClosed) MediaPlayer = new MediaPlayer();
            MediaPlayer.StoryboardEqualizer = storyboard;
            MediaPlayer.Show();
            MediaPlayer.Activate();
        }
        private void CloseMedia_Execute(object obj)
        {
            //Tắt hẳn media
            if (MediaPlayer is null || MediaPlayer.IsClosed) return;
            MediaPlayer.ShouldClose = true;
            MediaPlayer.Close();
        }

    }
}
