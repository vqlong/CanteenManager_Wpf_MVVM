using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows;
using System.Linq;
using Model;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using WpfLibrary;
using System.Reflection;

namespace ViewModel
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public ViewModelBase()
        {
            Sort = new RelayCommand<GridViewColumnHeader>(header => true, Sort_Execute);
        }
        public ICommand Sort { get; }
        bool _isAscending;
        SortingAdorner? _sortingAdorner;
        public ICommand OpenWindow { get; } = new RelayCommand<Type>(type => type.IsSubclassOf(typeof(Window)), type => { if (Activator.CreateInstance(type) is Window window) window.ShowDialog(); });
        public ICommand ShowDialog { get; } = new RelayCommand<Window>(w => w != null, w => w.ShowDialog());
        public ICommand Close { get; } = new RelayCommand<Window>(w => w != null, w => w.Close());
        public event PropertyChangedEventHandler? PropertyChanged;
        Dictionary<string, object> _values = new Dictionary<string, object>();
        protected virtual void SetProperty(object value, [CallerMemberName] string propertyName = "")
        {
            _values[propertyName] = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected virtual T GetProperty<T>([CallerMemberName] string propertyName = "")
        {
            if (_values.ContainsKey(propertyName))
                return (T)_values[propertyName];
            else
                return default;

        }
        protected virtual void OnPropertyChanged<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected virtual void Reload<T>(IList<T> collection, T model) where T : Model.Model
        {
            var old = collection.FirstOrDefault(m => m.Id == model.Id);
            if (old == null) return;
            var index = collection.IndexOf(old);
            collection.RemoveAt(index);
            collection.Insert(index, model);
        }
        protected virtual void Update<T>(T oldModel, T newModel) where T : Model.Model
        {
            if (oldModel is Account oldAccount && newModel is Account newAccount && oldAccount.Username != newAccount.Username) throw new Exception("Username phải giống nhau.");
            if (oldModel.Id != newModel.Id) throw new Exception("Id phải giống nhau.");
            PropertyInfo[] propertyInfos = oldModel.GetType().GetProperties();
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                if (propertyInfos[i].Name.Equals("Id") || propertyInfos[i].Name.Equals("Username")) continue;
                var newValue = propertyInfos[i].GetValue(newModel);
                propertyInfos[i].SetValue(oldModel, newValue);
            }
        }
        protected virtual void Remove<T>(ICollection<T> collection, int id) where T : Model.Model
        {
            var model = collection.FirstOrDefault(t => t.Id == id);
            if (model == null) return;
            collection.Remove(model);
        }
        protected virtual void SetCurrent<T>(ICollectionView view, int id) where T : Model.Model
        {
            foreach (T item in view.SourceCollection)
            {
                if(item.Id == id)
                {
                    view.MoveCurrentTo(item);
                    break;
                }
            }
        }
        public override string ToString()
        {
            return base.ToString() + $" {GetHashCode()}";
        }
        private void Sort_Execute(GridViewColumnHeader header)
        {
            var column = header.Column;
            //Mỗi GridViewColumn đã được attach 1 string để xác định nó hiển thị property nào của model nào
            var propertyName = AttachedManager.GetString(column);

            if (string.IsNullOrWhiteSpace(propertyName)) throw new Exception("Tên cột không phù hợp");

            var ListView = (ItemsControl)AttachedManager.GetTag(column);
            var view = CollectionViewSource.GetDefaultView(ListView.ItemsSource);
            if (_isAscending)
            {
                view.SortDescriptions.Clear();
                view.SortDescriptions.Add(new SortDescription(propertyName, ListSortDirection.Descending));
                _isAscending = false;
            }
            else
            {
                view.SortDescriptions.Clear();
                view.SortDescriptions.Add(new SortDescription(propertyName, ListSortDirection.Ascending));
                _isAscending = true;
            }

            if (_sortingAdorner != null) ((AdornerLayer)_sortingAdorner.Parent).Remove(_sortingAdorner);
            _sortingAdorner = new SortingAdorner(header, _isAscending);
            AdornerLayer.GetAdornerLayer(header).Add(_sortingAdorner);
        }

    }
}
