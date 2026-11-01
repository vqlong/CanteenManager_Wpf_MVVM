using Bus;
using Model;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using WpfLibrary.UserControls;

namespace ViewModel
{
    public class LoginViewModel : ViewModelBase
    {

        public string Username { get => GetProperty<string>(); set => SetProperty(value); }
        public string Password { get => GetProperty<string>(); set => SetProperty(value); }
        public TestConnectionResult TestResult { get => GetProperty<TestConnectionResult>(); set => SetProperty(value); }
        public ICommand TestConnection { get; }
        public ICommand Login { get; }
        public Account LoginAccount { get => MainViewModel.Instance.LoginAccount; internal set => MainViewModel.Instance.LoginAccount = value; }
        public LoginViewModel()
        {
            LoadConfig();

            Username = "admin";
            Password = "123456";
            TestConnection = new RelayCommand<object>(o => true, TestConnection_Execute);
            Login = new RelayCommand<Type>(type => type.IsSubclassOf(typeof(Window)), Login_Execute);
        }
        void LoadConfig()
        {
            //Bus.Config.RegisterEntity();
            Bus.Config.RegisterSQLite();

            //Chọn đường dẫn cho thư mục chứa file log
            log4net.GlobalContext.Properties["ApplicationPath"] = Environment.CurrentDirectory;
            //Hiện tên DataProvider trong message log
            log4net.GlobalContext.Properties["DataProvider"] = Bus.Config.DataProvider.Name;

            //Cấu hình đặt trong file App.config
            //[assembly: log4net.Config.XmlConfigurator(Watch=true)]
            //log4net.Config.XmlConfigurator.Configure();

            //Cấu hình đặt trong file log4net.config
            //[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("log4net.config"));
        }
        DispatcherFrame? _frame;
        void TestConnection_Execute(object obj)
        {
            string connectionString = Config.ConnectionString;

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += (s, e) =>
            {
                worker.ReportProgress(0);
                e.Result = Config.DataProvider.TestConnection(e.Argument.ToString());
            };
            worker.ProgressChanged += (s, e) =>
            {
                TestResult = TestConnectionResult.Waiting; 
            };
            worker.RunWorkerCompleted += (s, e) =>
            {
                if ((bool)e.Result)
                    TestResult = TestConnectionResult.Success;
                else
                    TestResult = TestConnectionResult.Fail;

                if (_frame != null) _frame.Continue = false;
            };
            worker.RunWorkerAsync(connectionString);
        }
        void Login_Execute(Type type)
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                DialogBox.Show("Không được để trống tên đăng nhập hoặc mật khẩu!", "Thông báo", DialogBoxButton.OK, DialogBoxIcon.Warning);
                return;
            }

            _frame = new DispatcherFrame();
            TestConnection?.Execute(null);
            Dispatcher.PushFrame(_frame);

            if (TestResult != TestConnectionResult.Success) return;

            Account loginAccount = AccountBus.Instance.Login(Username, Password);

            if (loginAccount != null)
            {
                Log.Info($"Login Account - Username: {loginAccount.Username}, Displayname: {loginAccount.DisplayName}.");

                Username = "";
                Password = "";
                var obj = Application.Current.MainWindow.FindName("ckbShowPassword");
                if(obj is CheckBox checkBox)
                {
                    //Đổi qua lại 2 template để reset passwordbox
                    checkBox.IsChecked = true;
                    checkBox.IsChecked = false;
                }
                LoginAccount = MainViewModel.Instance.Accounts.First(a => a.Username == loginAccount.Username);
                Application.Current.MainWindow.Hide();

                var tablemng = Activator.CreateInstance(type);
                if (tablemng is Window tableManager) tableManager.ShowDialog();
                else return;

                Log.Info($"Logout Account - Username: {LoginAccount.Username}, Displayname: {LoginAccount.DisplayName}.");
                LoginAccount = new Account { DisplayName = "none"};
                Application.Current.MainWindow.Show();
            }
            else
            {
                DialogBox.Show("Sai tên đăng nhập hoặc mật khẩu!", "Thông báo", DialogBoxButton.OK, DialogBoxIcon.Error);
            }
        }
    }
}
