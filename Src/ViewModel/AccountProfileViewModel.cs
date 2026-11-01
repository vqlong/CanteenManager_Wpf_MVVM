using Bus;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using WpfLibrary.UserControls;

namespace ViewModel
{
    public class AccountProfileViewModel : ViewModelBase, IDataErrorInfo
    {
        public Account LoginAccount { get => MainViewModel.Instance.LoginAccount; internal set => MainViewModel.Instance.LoginAccount = value; }
        public string Password { get => GetProperty<string>(); set => SetProperty(value); } 
        public string NewPassword { get => GetProperty<string>(); set => SetProperty(value); }
        public string ConfirmPassword { get => GetProperty<string>(); set => SetProperty(value); } 
        public string Error => throw new NotImplementedException();
        public string this[string columnName]
        {
            get
            {
                string result = string.Empty;

                if (string.IsNullOrEmpty(NewPassword) && string.IsNullOrEmpty(ConfirmPassword)) return result;

                if (columnName == nameof(NewPassword))
                {
                    if (!string.IsNullOrEmpty(ConfirmPassword))
                    {
                        //invoke PropertyChanged để validate lại ConfirmPassword
                        SetProperty(ConfirmPassword, nameof(ConfirmPassword));
                    }

                    var match = Regex.Match(NewPassword, @"^0");
                    if (match.Success) return "Mật khẩu không được bắt đầu bằng số 0.";

                    match = Regex.Match(NewPassword, @"^.{6,20}$");
                    if (match.Value.Equals(NewPassword) == false) return "Mật khẩu phải bao gồm 6 - 20 ký tự.";

                    match = Regex.Match(NewPassword, @"^[a-zA-Z0-9]*$");
                    if (match.Value.Equals(NewPassword) == false) return "Mật khẩu phải là các ký tự 0 - 9, a - z, A - Z.";
                }

                if (columnName == nameof(ConfirmPassword))
                {
                    if (!string.IsNullOrEmpty(ConfirmPassword) && ConfirmPassword != NewPassword) return "Mật khẩu nhập lại không trùng khớp.";
                }

                return result;
            }
        }
        public ICommand UpdateAccount { get; }
        public AccountProfileViewModel()
        {
            UpdateAccount = new RelayCommand<object>(UpdateAccount_CanExecute, UpdateAccount_Execute); 
        }

        private bool UpdateAccount_CanExecute(object obj)
        {
            if (!string.IsNullOrWhiteSpace(Password) 
                && string.IsNullOrEmpty(this[nameof(NewPassword)]) 
                && string.IsNullOrEmpty(this[nameof(ConfirmPassword)])) return true;
            else return false;
        }

        private void UpdateAccount_Execute(object obj)
        {
            var loginAccount = AccountBus.Instance.Login(LoginAccount.Username, Password);

            if (loginAccount != null)
            {
                var newPassword = string.IsNullOrEmpty(NewPassword) ? null : NewPassword;
                var result = AccountBus.Instance.Update(LoginAccount.Username, LoginAccount.DisplayName, newPassword);
                if (result.Item1 || result.Item2)
                {
                    Log.Info($"Update Account - Username: {LoginAccount.Username}, Displayname: {LoginAccount.DisplayName}.");

                    string info = "";
                    if (result.Item1) info = "tên hiển thị";
                    if (result.Item1 && result.Item2) info += " và mật khẩu";
                    DialogBox.Show($"Cập nhật {info} thành công!", "Thông báo", DialogBoxButton.OK, DialogBoxIcon.Information);

                    Password = "";
                    NewPassword = "";
                    ConfirmPassword = "";
                    ////Đổi qua lại 2 template để reset passwordbox
                    if (obj is Window window && window.FindName("ckbShowPassword") is CheckBox checkBox)
                    {
                        checkBox.IsChecked = true;
                        checkBox.IsChecked = false;
                    }
                }
                else
                {
                    DialogBox.Show("Cập nhật thất bại!", "Thông báo", DialogBoxButton.OK, DialogBoxIcon.Error);
                }
            }
            else
            {
                DialogBox.Show("Sai mật khẩu!", "Thông báo", DialogBoxButton.OK, DialogBoxIcon.Error);

                Password = "";
                NewPassword = "";
                ConfirmPassword = "";
                if (obj is Window window && window.FindName("ckbShowPassword") is CheckBox checkBox)
                {
                    checkBox.IsChecked = true;
                    checkBox.IsChecked = false;
                }
            }
        }
    }

}
