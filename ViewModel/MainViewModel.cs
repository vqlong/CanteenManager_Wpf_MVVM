using Bus;
using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private static readonly MainViewModel _instance = new MainViewModel();
        public static MainViewModel Instance => _instance; 
        public Account LoginAccount { get => GetProperty<Account>(); internal set => SetProperty(value); }
        internal ObservableCollection<Category> Categories { get; } 
        internal ObservableCollection<Food> Foods { get; } 
        internal ObservableCollection<Table> Tables { get; } = new ObservableCollection<Table>(TableBus.Instance.GetListTable());
        internal ObservableCollection<Account> Accounts { get; } = new ObservableCollection<Account>(AccountBus.Instance.GetListAccount());
        internal ObservableCollection<UsingState> UsingStates { get; } = new ObservableCollection<UsingState>(Enum.GetValues<UsingState>());
        internal ListCollectionView FoodsView { get; }
        internal ListCollectionView UsingCategoriesView { get; }
        internal ListCollectionView UsingTablesView { get; }
        internal ListCollectionView UsingTablesView2 { get; } 
        internal int PrintBillId { get => GetProperty<int>(); set => SetProperty(value); }
        internal List<ICollectionView> Views { get; } = new List<ICollectionView>();
        private MainViewModel()
        {
            //Với mỗi record của food và category chỉ có duy nhất 1 đối tượng được tạo
            Foods = new ObservableCollection<Food>(FoodBus.Instance.GetListFood());
            Categories  = new ObservableCollection<Category>(CategoryBus.Instance.GetListCategory(Foods));

            //view cho table manager window
            UsingCategoriesView = new ListCollectionView(Categories) { Filter = obj => (obj as Category).CategoryStatus == UsingState.Serving };
            foreach (var category in Categories)
            {
                var view = CollectionViewSource.GetDefaultView(category.Foods);
                //add view vào list để khỏi bị GC dọn đi mất
                if (Views.Contains(view) == false) Views.Add(view);
                view.Filter = obj => (obj as Food).FoodStatus == UsingState.Serving; 
            }
            UsingTablesView = new ListCollectionView(Tables) { Filter = obj => (obj as Table).UsingState == UsingState.Serving };
            UsingTablesView2 = new ListCollectionView(Tables) { Filter = obj => (obj as Table).UsingState == UsingState.Serving };

            //view cho admin food
            FoodsView = new ListCollectionView(Foods);
        }
    }
}
