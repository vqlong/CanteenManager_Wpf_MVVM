using Bus;
using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using WpfLibrary.UserControls;

namespace ViewModel
{
    public class AccountViewModel : ViewModelBase
    {
        public Account SelectedItem { get => GetProperty<Account>(); set => SetProperty(value); }
        public Account LoginAccount { get => MainViewModel.Instance.LoginAccount; internal set => MainViewModel.Instance.LoginAccount = value; }
        public ListCollectionView AccountsView { get; } = new ListCollectionView(MainViewModel.Instance.Accounts); 
        public ICommand Update { get; }
        public ICommand Delete { get; }
        public ICommand ResetPassword { get; }
        public AccountViewModel()
        {
            Update = new RelayCommand<object>(UpdateAccount_CanExecute, UpdateAccount_Execute);
            Delete = new RelayCommand<object>(DeleteAccount_CanExecute, DeleteAccount_Execute);
            ResetPassword = new RelayCommand<object>(UpdateAccount_CanExecute, ResetPassword_Execute);
        }

        private bool UpdateAccount_CanExecute(object obj)
        {
            if (SelectedItem != null && !string.IsNullOrEmpty(SelectedItem.Username))
            {
                if (SelectedItem.Type == AccountType.Staff || (SelectedItem.Type == AccountType.Admin && SelectedItem.Username == LoginAccount.Username)) return true;
            }
            return false;
        }

        private void UpdateAccount_Execute(object obj)
        { 
            if (DialogBox.Show("Bạn có thực sự muốn cập nhật tài khoản này?\n" +
                $"Tên đăng nhập: {SelectedItem.Username}\n" +
                $"Tên hiển thị: {SelectedItem.DisplayName}\n",
                "Thông báo",
                DialogBoxButton.YesNo,
                DialogBoxIcon.Question)
                == DialogBoxResult.Yes)
            {
                if (AccountBus.Instance.Update(SelectedItem.Username, SelectedItem.DisplayName).Item1)
                {
                    DialogBox.Show($"Cập nhật tài khoản {SelectedItem.Username} thành công.", "Thông báo", DialogBoxButton.OK, DialogBoxIcon.Information); 

                    Log.Info($"Update Account - Username: {SelectedItem.Username}, Displayname: {SelectedItem.DisplayName}.");
                }
                else
                {
                    DialogBox.Show($"Cập nhật tài khoản {SelectedItem.Username} thất bại.");
                }

            }
        }
        private bool DeleteAccount_CanExecute(object obj)
        {
            if (SelectedItem != null && !string.IsNullOrEmpty(SelectedItem.Username) && SelectedItem.Type == AccountType.Staff) return true;
            return false;
        }
        private void DeleteAccount_Execute(object obj)
        { 
            if (DialogBox.Show($"Bạn có thực sự muốn xoá tài khoản {SelectedItem.Username}?",
                "Thông báo",
                DialogBoxButton.YesNo,
                DialogBoxIcon.Question)
                == DialogBoxResult.Yes)
            {
                if (AccountBus.Instance.DeleteAccount(SelectedItem.Username))
                {
                    DialogBox.Show($"Xoá tài khoản {SelectedItem.Username} thành công.", "Thông báo", DialogBoxButton.OK, DialogBoxIcon.Information);

                    MainViewModel.Instance.Accounts.Remove((Account)AccountsView.CurrentItem); 

                    Log.Info($"Delete Account - Username: {SelectedItem.Username}, Displayname: {SelectedItem.DisplayName}.");
                }
                else
                {
                    DialogBox.Show($"Xoá tài khoản {SelectedItem.Username} thất bại.");
                }

            }
        }

        private void ResetPassword_Execute(object obj)
        { 
            if (DialogBox.Show("Bạn có thực sự muốn đặt lại mật khẩu cho tài khoản này?\n" +
                $"Tên đăng nhập: {SelectedItem.Username}\n" +
                $"Tên hiển thị: {SelectedItem.DisplayName}\n",
                "Thông báo",
                DialogBoxButton.YesNo,
                DialogBoxIcon.Question)
                == DialogBoxResult.Yes)
            {
                if (AccountBus.Instance.Update(SelectedItem.Username, null, "123456").Item2)
                {
                    DialogBox.Show($"Đặt lại mật khẩu cho tài khoản {SelectedItem.Username} thành công.", "Thông báo", DialogBoxButton.OK, DialogBoxIcon.Information);

                    Log.Info($"Reset Password - Username: {SelectedItem.Username}, Displayname: {SelectedItem.DisplayName}.");
                }
                else
                {
                    DialogBox.Show($"Đặt lại mật khẩu cho tài khoản {SelectedItem.DisplayName} thất bại.");
                }

            }
        }
    }
}
