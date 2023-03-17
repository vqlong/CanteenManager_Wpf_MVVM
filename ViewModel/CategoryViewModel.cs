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
    public class CategoryViewModel : ViewModelBase
    {
        public Category SelectedItem { get => GetProperty<Category>(); set => SetProperty(value == null ? new Category() : value.GetClone<Category>()); }
        public ListCollectionView CategoriesView { get; } = new ListCollectionView(MainViewModel.Instance.Categories);
        public ListCollectionView CategoryStatusView { get; } = new ListCollectionView(MainViewModel.Instance.UsingStates);
        public ICommand Insert { get; }
        public ICommand Update { get; }
        public ICommand Delete { get; }
        public ICommand RefreshView { get; }
        string _text = "";
        public CategoryViewModel() 
        { 
            CategoriesView.Filter = (obj) => (obj as Category).Name.IndexOf(_text, StringComparison.CurrentCultureIgnoreCase) >= 0;
            RefreshView = new RelayCommand<object>(obj => true, obj =>
            {
                if (obj is string text)
                {
                    _text = text;
                    CategoriesView.Refresh();
                }
            });
            Update = new RelayCommand<object>(UpdateCategory_CanExecute, UpdateCategory_Execute);
            Delete = new RelayCommand<object>(UpdateCategory_CanExecute, DeleteCategory_Execute);
            Insert = new RelayCommand<object>(InsertCategory_CanExecute, InsertCategory_Execute);
        }
        
        private bool UpdateCategory_CanExecute(object obj)
        {
            if (SelectedItem != null && SelectedItem.Id != 0) return true;
            else return false;
        }

        private void UpdateCategory_Execute(object obj)
        { 
            if (DialogBox.Show("Bạn có thực sự muốn cập nhật danh mục này?\n" +
                $"Id: {SelectedItem.Id}\n" +
                $"Tên: {SelectedItem.Name}\n",
                "Thông báo",
                DialogBoxButton.YesNo,
                DialogBoxIcon.Question)
                == DialogBoxResult.Yes)
            {
                if (CategoryBus.Instance.UpdateCategory(SelectedItem.Id, SelectedItem.Name, SelectedItem.CategoryStatus))
                {
                    DialogBox.Show($"Cập nhật danh mục {SelectedItem.Name} thành công.", "Thông báo", DialogBoxButton.OK, DialogBoxIcon.Information);

                    Category currentCategory = (Category)CategoriesView.CurrentItem;

                    //Nếu thay đổi status
                    if (currentCategory.CategoryStatus != SelectedItem.CategoryStatus)
                    {
                        //các food của nó cũng thay đổi theo
                        foreach (var food in currentCategory.Foods)
                        { 
                            food.FoodStatus = SelectedItem.CategoryStatus;
                        }

                        MainViewModel.Instance.FoodsView.Refresh();
                    }

                    Update<Category>(currentCategory, SelectedItem);
                    //refresh listview
                    CategoriesView.Refresh();
                    CategoriesView.MoveCurrentToFirst();
                    CategoriesView.MoveCurrentToLast();
                    CategoriesView.MoveCurrentTo(currentCategory);

                    //refresh cái view của combobox hiện Category bên table manager
                    var view = MainViewModel.Instance.UsingCategoriesView;
                    if (currentCategory.Equals(view.CurrentItem)) view.MoveCurrentToNext();
                    view.Refresh();
                    if (currentCategory.CategoryStatus == UsingState.Serving) view.MoveCurrentTo(currentCategory);
                    else view.MoveCurrentToFirst();

                    Log.Info($"Update Category - Id: {currentCategory.Id}.");
                }
                else
                {
                    DialogBox.Show($"Cập nhật danh mục {SelectedItem.Name} thất bại.");
                }

            }
        }
         
        private bool InsertCategory_CanExecute(object obj)
        {
            if (SelectedItem != null && SelectedItem.Name != "" && SelectedItem.CategoryStatus != UsingState.StopServing) return true;
            else return false;
        }

        private void InsertCategory_Execute(object obj)
        {
            if (DialogBox.Show("Bạn có thực sự muốn thêm danh mục này?\n" +
                $"Tên: {SelectedItem.Name}\n",
                "Thông báo",
                DialogBoxButton.YesNo,
                DialogBoxIcon.Question)
                == DialogBoxResult.Yes)
            {
                var category = CategoryBus.Instance.InsertCategory(SelectedItem.Name);
                if (category is not null)
                {
                    DialogBox.Show($"Thêm danh mục {SelectedItem.Name} thành công.", "Thông báo", DialogBoxButton.OK, DialogBoxIcon.Information);

                    MainViewModel.Instance.Categories.Add(category);
                    CategoriesView.MoveCurrentTo(category);
                    MainViewModel.Instance.UsingCategoriesView.MoveCurrentTo(category);

                    Log.Info($"Insert Category - Id: {category.Id}.");
                }
                else
                {
                    DialogBox.Show($"Thêm danh mục {SelectedItem.Name} thất bại.");
                }

            }
        }
         
        private void DeleteCategory_Execute(object obj)
        { 
            if (DialogBox.Show($"Bạn có thực sự muốn xoá danh mục {SelectedItem.Name}?\ndanh mục bị xoá sẽ chuyển sang trạng thái [Dừng bán].",
                "Thông báo",
                DialogBoxButton.YesNo,
                DialogBoxIcon.Question)
                == DialogBoxResult.Yes)
            {
                if (CategoryBus.Instance.DeleteCategory(SelectedItem.Id))
                {
                    DialogBox.Show($"Xoá danh mục {SelectedItem.Name} thành công.", "Thông báo", DialogBoxButton.OK, DialogBoxIcon.Information);

                    Category currentCategory = (Category)CategoriesView.CurrentItem;
                    currentCategory.CategoryStatus = UsingState.StopServing;
                    foreach (var food in currentCategory.Foods)
                    {
                        food.FoodStatus = UsingState.StopServing;
                    }

                    CategoriesView.Refresh();
                    CategoriesView.MoveCurrentToFirst();
                    CategoriesView.MoveCurrentToLast();
                    CategoriesView.MoveCurrentTo(currentCategory);

                    var view = MainViewModel.Instance.UsingCategoriesView; 
                    view.Refresh(); 
                    view.MoveCurrentToFirst();

                    MainViewModel.Instance.FoodsView.Refresh();

                    Log.Info($"Delete Category - Id: {currentCategory.Id}.");
                }
                else
                {
                    DialogBox.Show($"Xoá danh mục {SelectedItem.Name} thất bại.");
                }

            }
        }
        
    }
}
