using Bus;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using WpfLibrary.UserControls;

namespace ViewModel
{
    public class TableViewModel : ViewModelBase
    {
        public Table SelectedItem { get => GetProperty<Table>(); set => SetProperty(value == null ? new Table() : value.GetClone<Table>()); }
        public ListCollectionView TablesView { get; } = new ListCollectionView(MainViewModel.Instance.Tables);
        public ListCollectionView TableStatusView { get; } = new ListCollectionView(MainViewModel.Instance.UsingStates);
        public ICommand Insert { get; }
        public ICommand Update { get; }
        public ICommand Delete { get; }
        public TableViewModel()
        {
            Update = new RelayCommand<object>(UpdateTable_CanExecute, UpdateTable_Execute);
            Delete = new RelayCommand<object>(UpdateTable_CanExecute, DeleteTable_Execute);
            Insert = new RelayCommand<object>(InsertTable_CanExecute, InsertTable_Execute);
        }
        private bool UpdateTable_CanExecute(object obj)
        {
            if (SelectedItem != null && SelectedItem.Id != 0 && SelectedItem.Status.Equals("Trống")) return true;
            else return false;
        }

        private void UpdateTable_Execute(object obj)
        { 
            if (DialogBox.Show("Bạn có thực sự muốn cập nhật bàn ăn này?\n" +
                $"Id: {SelectedItem.Id}\n" +
                $"Tên: {SelectedItem.Name}\n",
                "Thông báo",
                DialogBoxButton.YesNo,
                DialogBoxIcon.Question)
                == DialogBoxResult.Yes)
            {
                if (TableBus.Instance.UpdateTable(SelectedItem.Id, SelectedItem.Name, SelectedItem.UsingState))
                {
                    DialogBox.Show($"Cập nhật {SelectedItem.Name} thành công.", "Thông báo", DialogBoxButton.OK, DialogBoxIcon.Information);

                    Table currentTable = (Table)TablesView.CurrentItem;
                    Update<Table>(currentTable, SelectedItem);

                    TablesView.Refresh();
                    TablesView.MoveCurrentToFirst();
                    TablesView.MoveCurrentToLast();
                    TablesView.MoveCurrentTo(currentTable);

                    MainViewModel.Instance.UsingTablesView.Refresh();
                    MainViewModel.Instance.UsingTablesView2.Refresh();

                    Log.Info($"Update Table - Id: {currentTable.Id}.");
                }
                else
                {
                    DialogBox.Show($"Cập nhật {SelectedItem.Name} thất bại.");
                }

            }
        }

        private bool InsertTable_CanExecute(object obj)
        {
            return true;
        }

        private void InsertTable_Execute(object obj)
        {
            if (DialogBox.Show("Bạn có thực sự muốn thêm 1 bàn ăn mới?",
                "Thông báo",
                DialogBoxButton.YesNo,
                DialogBoxIcon.Question)
                == DialogBoxResult.Yes)
            {
                var table = TableBus.Instance.InsertTable();
                if (table is not null)
                {
                    DialogBox.Show($"Thêm bàn ăn mới thành công.", "Thông báo", DialogBoxButton.OK, DialogBoxIcon.Information);

                    MainViewModel.Instance.Tables.Add(table); 

                    Log.Info($"Insert Table - Id: {table.Id}.");
                }
                else
                {
                    DialogBox.Show($"Thêm bàn ăn mới thất bại.");
                }

            }
        }

        private void DeleteTable_Execute(object obj)
        { 
            if (DialogBox.Show($"Bạn có thực sự muốn xoá {SelectedItem.Name}?\nbàn ăn bị xoá sẽ chuyển sang trạng thái [Dừng bán].",
                "Thông báo",
                DialogBoxButton.YesNo,
                DialogBoxIcon.Question)
                == DialogBoxResult.Yes)
            {
                if (TableBus.Instance.DeleteTable(SelectedItem.Id))
                {
                    DialogBox.Show($"Xoá {SelectedItem.Name} thành công.", "Thông báo", DialogBoxButton.OK, DialogBoxIcon.Information);

                    Table currentTable = (Table)TablesView.CurrentItem;
                    currentTable.UsingState = UsingState.StopServing;

                    TablesView.Refresh();
                    TablesView.MoveCurrentToFirst();
                    TablesView.MoveCurrentToLast();
                    TablesView.MoveCurrentTo(currentTable);

                    MainViewModel.Instance.UsingTablesView.Refresh();
                    MainViewModel.Instance.UsingTablesView2.Refresh();

                    Log.Info($"Delete Table - Id: {currentTable.Id}.");
                }
                else
                {
                    DialogBox.Show($"Xoá {SelectedItem.Name} thất bại.");
                }

            }
        }
    }
}
