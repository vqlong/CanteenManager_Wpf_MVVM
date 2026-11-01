using Azure.Messaging;
using Bus;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfLibrary.UserControls;

namespace ViewModel
{
    public class InsertAccountViewModel : ViewModelBase, IDataErrorInfo
    {
        public string Username { get => GetProperty<string>(); set => SetProperty(value); }
        public string this[string columnName]
        {
            get
            {
                string result = string.Empty;

                if (columnName == nameof(Username))
                {
                    if (string.IsNullOrEmpty(Username)) return result;

                    var match = Regex.Match(Username, @"^0");
                    if (match.Success) return "Tên đăng nhập không được bắt đầu bằng số 0.";

                    match = Regex.Match(Username, @"^.{4,20}$");
                    if (match.Value.Equals(Username) == false) return "Tên đăng nhập phải bao gồm 4 - 20 ký tự.";

                    match = Regex.Match(Username, @"^[a-zA-Z0-9]*$");
                    if (match.Value.Equals(Username) == false) return "Tên đăng nhập phải là các ký tự 0 - 9, a - z, A - Z.";
                }

                return result;
            }
        }

        public string Error => throw new NotImplementedException();
        public ICommand InsertAccount { get; }
        public InsertAccountViewModel()
        {
            InsertAccount = new RelayCommand<object>(InsertAccount_CanExecute, InsertAccount_Execute);
        }
        private bool InsertAccount_CanExecute(object obj)
        {
            if (!string.IsNullOrEmpty(Username) && string.IsNullOrEmpty(this[nameof(Username)])) return true;
            else return false;
        }

        private void InsertAccount_Execute(object obj)
        {
            if (DialogBox.Show($"Bạn có thực sự muốn thêm 1 tài khoản mới với tên đăng nhập {Username}?",
                "Thông báo",
                DialogBoxButton.YesNo,
                DialogBoxIcon.Question)
                == DialogBoxResult.Yes)
            {
                var account = AccountBus.Instance.InsertAccount(Username);
                if (account is not null)
                {
                    DialogBox.Show($"Thêm tài khoản mới thành công.", "Thông báo", DialogBoxButton.OK, DialogBoxIcon.Information);
                    MainViewModel.Instance.Accounts.Add(account);
                    Log.Info($"Insert Account - Username: {account.Username}, Displayname: {account.DisplayName}.");                   
                }
                else
                {
                    DialogBox.Show($"Thêm tài khoản mới thất bại.");
                }
                if (obj is Window window) Close.Execute(window);
            }
        }
    }
}
