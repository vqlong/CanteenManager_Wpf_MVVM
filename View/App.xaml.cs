using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using WpfLibrary.UserControls;

namespace View
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static Mutex? mutex;
        protected override void OnStartup(StartupEventArgs e)
        {
            mutex = new Mutex(true, "thisapplicationonlyrunswithonlyoneinstance", out bool createdNew);
            if(createdNew == false)
            {
                DialogBox.Show("Một phiên bản của chương trình vẫn đang chạy.", "Thông báo", DialogBoxButton.OK, DialogBoxIcon.Information);
                Shutdown();
            }

            ////Bus.Config.RegisterEntity();
            //Bus.Config.RegisterSQLite();

            ////Chọn đường dẫn cho thư mục chứa file log
            //log4net.GlobalContext.Properties["ApplicationPath"] = Environment.CurrentDirectory;
            ////Hiện tên DataProvider trong message log
            //log4net.GlobalContext.Properties["DataProvider"] = Bus.Config.DataProvider.Name;

            ////Cấu hình đặt trong file App.config
            ////[assembly: log4net.Config.XmlConfigurator(Watch=true)]
            ////log4net.Config.XmlConfigurator.Configure();

            ////Cấu hình đặt trong file log4net.config
            ////[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]
            //log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("log4net.config"));

            //var resource = (ResourceDictionary)Application.LoadComponent(new Uri("ViewModel.xaml", UriKind.Relative));
            //Resources.MergedDictionaries.Add(resource);

            base.OnStartup(e);
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            DialogBox.Show(e.Exception.Message);
            e.Handled = true;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var uri = new Uri("pack://application:,,,/PresentationFramework.Royale;component/themes/Royale.NormalColor.xaml");
            var dictionary = new ResourceDictionary { Source = uri };
            foreach (var key in dictionary.Keys)
            {
                //Lấy ra style cho scrollbar và thêm vào resource của app
                if (dictionary[key] is Style style && style.TargetType == typeof(ScrollBar))
                {
                    Resources.Add(key, style);
                    break;
                }
            }
        }
    }
}
