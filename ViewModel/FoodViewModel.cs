using Bus;
using Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using WpfLibrary.UserControls;

namespace ViewModel
{
    public class FoodViewModel : ViewModelBase
    {
        public Food SelectedItem { get => GetProperty<Food>(); set => SetProperty(value == null ? new Food() : value.GetClone<Food>()); }
        public ListCollectionView FoodsView => MainViewModel.Instance.FoodsView;
        public ListCollectionView FoodCategoriesView { get; } = new ListCollectionView(MainViewModel.Instance.Categories);
        public ListCollectionView FoodStatusView { get; } = new ListCollectionView(MainViewModel.Instance.UsingStates);
        public ICommand Insert { get; }
        public ICommand Update { get; }
        public ICommand Delete { get; }
        public ICommand RefreshView { get; }
        string _text = "";
        Predicate<object> _usingFoodFilter = obj => (obj as Food).FoodStatus == UsingState.Serving;
        public FoodViewModel() 
        { 
            FoodsView.Filter = (obj) => (obj as Food).Name.IndexOf(_text, StringComparison.CurrentCultureIgnoreCase) >= 0;
            Update = new RelayCommand<object>(UpdateFood_CanExecute, UpdateFood_Execute);
            Delete = new RelayCommand<object>(UpdateFood_CanExecute, DeleteFood_Execute);
            Insert = new RelayCommand<object>(InsertFood_CanExecute, InsertFood_Execute);
            RefreshView = new RelayCommand<object>(obj => true, obj => 
            { 
                if (obj is string text)
                {
                    _text = text;
                    FoodsView.Refresh();
                }
            });
        }
        private bool UpdateFood_CanExecute(object obj)
        {
            if (SelectedItem != null && SelectedItem.Id != 0) return true;
            else return false;
        }       
        private void UpdateFood_Execute(object obj)
        { 
            if (DialogBox.Show("Bạn có thực sự muốn cập nhật món ăn này?\n" +
                $"Id: {SelectedItem.Id}\n" +
                $"Tên: {SelectedItem.Name}\n" +
                $"Mục: {MainViewModel.Instance.Categories.First(c => c.Id == SelectedItem.CategoryId).Name}\n" +
                $"Giá: {SelectedItem.Price.ToString("C1", CultureInfo.GetCultureInfo("vi-vn"))}\n",
                "Thông báo",
                DialogBoxButton.YesNo,
                DialogBoxIcon.Question)
                == DialogBoxResult.Yes)
            {
                if (FoodBus.Instance.UpdateFood(SelectedItem.Id, SelectedItem.Name, SelectedItem.CategoryId, SelectedItem.Price, SelectedItem.FoodStatus))
                {
                    DialogBox.Show($"Cập nhật món ăn {SelectedItem.Name} thành công.", "Thông báo", DialogBoxButton.OK, DialogBoxIcon.Information);

                    Food currentFood = (Food)FoodsView.CurrentItem;
                    Update<Food>(currentFood, SelectedItem);

                    var currentCategory = currentFood.Category;
                    //refresh cái view của combobox hiện food bên table manager
                    var view1 = CollectionViewSource.GetDefaultView(currentCategory.Foods); 
                    //Nếu thay đổi category
                    if (currentFood.CategoryId != currentFood.Category.Id)
                    {
                        //xoá food khỏi category cũ của nó
                        currentCategory.Foods.Remove(currentFood);
                        view1.Refresh();
                        view1.MoveCurrentToFirst();

                        //add lại category mới cho food
                        var newCategory = MainViewModel.Instance.Categories.First(c => c.Id == currentFood.CategoryId);
                        currentFood.Category = newCategory;
                        newCategory.Foods.Add(currentFood);
                        //refresh cái view của combobox hiện food bên table manager
                        var view2 = CollectionViewSource.GetDefaultView(newCategory.Foods); 
                        view2.Refresh();
                        view2.MoveCurrentTo(currentFood); 
                    }
                    else
                    {
                        //nếu food đang được lựa chọn => move qua vị trí khác để refresh xong quay lại
                        if (currentFood.Equals(view1.CurrentItem)) view1.MoveCurrentToNext();
                        view1.Refresh();
                        if (currentFood.FoodStatus == UsingState.Serving) view1.MoveCurrentTo(currentFood);
                        else view1.MoveCurrentToFirst();
                    }

                    FoodsView.Refresh();
                    //Move qua move lại để load lại SelectedItem
                    FoodsView.MoveCurrentToFirst();
                    FoodsView.MoveCurrentToLast();
                    FoodsView.MoveCurrentTo(currentFood);

                    //var id = SelectedItem.Id;
                    ////reload lại food trên listview
                    //Reload<Food>(MainViewModel.Instance.Foods, SelectedItem);
                    ////Sau khi reload SelectedItem sẽ tự nhảy sang thằng bên dưới
                    ////set lại CurrentItem
                    //SetCurrent<Food>(FoodsView, id); 

                    Log.Info($"Update Food - Id: {currentFood.Id}.");
                }
                else
                {
                    DialogBox.Show($"Cập nhật món ăn {SelectedItem.Name} thất bại.");
                }

            }
        }       
        private void DeleteFood_Execute(object obj)
        { 
            if (DialogBox.Show($"Bạn có thực sự muốn xoá món ăn {SelectedItem.Name}?\nMón ăn bị xoá sẽ chuyển sang trạng thái [Dừng bán].",
                "Thông báo",
                DialogBoxButton.YesNo,
                DialogBoxIcon.Question)
                == DialogBoxResult.Yes)
            {
                if (FoodBus.Instance.DeleteFood(SelectedItem.Id))
                {
                    DialogBox.Show($"Xoá món ăn {SelectedItem.Name} thành công.", "Thông báo", DialogBoxButton.OK, DialogBoxIcon.Information);

                    Food currentFood = (Food)FoodsView.CurrentItem;
                    currentFood.FoodStatus = UsingState.StopServing; 

                    var view = CollectionViewSource.GetDefaultView(currentFood.Category.Foods); 
                    view.Refresh();
                    view.MoveCurrentToFirst();

                    FoodsView.Refresh();
                    FoodsView.MoveCurrentToFirst();
                    FoodsView.MoveCurrentToLast();
                    FoodsView.MoveCurrentTo(currentFood); 

                    Log.Info($"Delete Food - Id: {currentFood.Id}.");
                }
                else
                {
                    DialogBox.Show($"Xoá món ăn {SelectedItem.Name} thất bại.");
                }
            }
        } 
        private bool InsertFood_CanExecute(object obj)
        {
            if (SelectedItem != null && SelectedItem.Name != "" && SelectedItem.CategoryId != 0 && SelectedItem.Price != 0 && SelectedItem.FoodStatus != UsingState.StopServing) return true;
            else return false;
        }
        private void InsertFood_Execute(object obj)
        {
            if (DialogBox.Show("Bạn có thực sự muốn thêm món ăn này?\n" +
                $"Tên: {SelectedItem.Name}\n" +
                $"Mục: {MainViewModel.Instance.Categories.First(c => c.Id == SelectedItem.CategoryId).Name}\n" +
                $"Giá: {SelectedItem.Price.ToString("C1", CultureInfo.GetCultureInfo("vi-vn"))}\n",
                "Thông báo",
                DialogBoxButton.YesNo,
                DialogBoxIcon.Question)
                == DialogBoxResult.Yes)
            {
                var food = FoodBus.Instance.InsertFood(SelectedItem.Name, SelectedItem.CategoryId, SelectedItem.Price);
                if (food != null)
                {
                    DialogBox.Show($"Thêm món ăn {food.Name} thành công.", "Thông báo", DialogBoxButton.OK, DialogBoxIcon.Information);

                    MainViewModel.Instance.Foods.Add(food); 
                    FoodsView.MoveCurrentTo(food);
                    var category = MainViewModel.Instance.Categories.First(c => c.Id == food.CategoryId);
                    category.Foods.Add(food);
                    food.Category = category;
                    var view = CollectionViewSource.GetDefaultView(category.Foods);
                    view.Refresh();
                    view.MoveCurrentTo(food);

                    Log.Info($"Insert Food - Id: {food.Id}.");
                }
                else
                {
                    DialogBox.Show($"Thêm món ăn {SelectedItem.Name} thất bại.");
                }

            }
        }
 
    }
}

        