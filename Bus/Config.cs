using Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace Bus
{
    public static class Config
    {
        public static UnityContainer Container { get; private set; } //= new UnityContainer();

        public static IDataProvider DataProvider { get; private set; }

        public static string ConnectionString { get; private set; }

        /// <summary>
        /// Dùng ADO.NET với SQLite.
        /// </summary>
        public static void RegisterSQLite()
        {
            Container = new UnityContainer();

            Container.RegisterInstance<IAccountData>(Activator.CreateInstance(typeof(SQLiteDataAccess.AccountData), true) as SQLiteDataAccess.AccountData, InstanceLifetime.Singleton);
            Container.RegisterInstance<IBillData>(SQLiteDataAccess.BillData.Instance, InstanceLifetime.Singleton);
            Container.RegisterInstance<IBillDetailData>(Activator.CreateInstance(typeof(SQLiteDataAccess.BillDetailData), true) as SQLiteDataAccess.BillDetailData, InstanceLifetime.Singleton);
            Container.RegisterInstance<IBillInfoData>(Activator.CreateInstance(typeof(SQLiteDataAccess.BillInfoData), true) as SQLiteDataAccess.BillInfoData, InstanceLifetime.Singleton);
            Container.RegisterInstance<ICategoryData>(SQLiteDataAccess.CategoryData.Instance, InstanceLifetime.Singleton);
            Container.RegisterInstance<IFoodData>(SQLiteDataAccess.FoodData.Instance, InstanceLifetime.Singleton);
            Container.RegisterInstance<ITableData>(Activator.CreateInstance(typeof(SQLiteDataAccess.TableData), true) as SQLiteDataAccess.TableData, InstanceLifetime.Singleton);

            DataProvider = SQLiteDataAccess.DataProvider.Instance;
            ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SQLiteConnection"].ConnectionString;
        }

        /// <summary>
        /// Dùng SQL Server.
        /// </summary>
        //public static void RegisterSQLServer()
        //{
        //    Container = new UnityContainer();

        //    Container.RegisterInstance<IAccountData>(Activator.CreateInstance(typeof(AccountData), true) as AccountData, InstanceLifetime.Singleton);
        //    Container.RegisterInstance<IBillData>(Activator.CreateInstance(typeof(BillData), true) as BillData, InstanceLifetime.Singleton);
        //    Container.RegisterInstance<IBillDetailData>(Activator.CreateInstance(typeof(BillDetailData), true) as BillDetailData, InstanceLifetime.Singleton);
        //    Container.RegisterInstance<IBillInfoData>(Activator.CreateInstance(typeof(BillInfoData), true) as BillInfoData, InstanceLifetime.Singleton);
        //    Container.RegisterInstance<ICategoryData>(Activator.CreateInstance(typeof(CategoryData), true) as CategoryData, InstanceLifetime.Singleton);
        //    Container.RegisterInstance<IFoodData>(Activator.CreateInstance(typeof(FoodData), true) as FoodData, InstanceLifetime.Singleton);
        //    Container.RegisterInstance<ITableData>(Activator.CreateInstance(typeof(TableData), true) as TableData, InstanceLifetime.Singleton);

        //    DataProvider = CanteenManager.DAO.DataProvider.Instance;
        //    //ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SqlServerConnection"].ConnectionString.Replace("{ApplicationFolder}", Application.StartupPath);
        //    ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SqlServerConnection"].ConnectionString;

        //}

        /// <summary>
        /// Dùng EntityFramework Core với SQL Server.
        /// </summary>
        public static void RegisterEntity()
        {
            Container = new UnityContainer();

            Container.RegisterInstance<IAccountData>(Activator.CreateInstance(typeof(EntityDataAccess.AccountData), true) as EntityDataAccess.AccountData, InstanceLifetime.Singleton);
            Container.RegisterInstance<IBillData>(Activator.CreateInstance(typeof(EntityDataAccess.BillData), true) as EntityDataAccess.BillData, InstanceLifetime.Singleton);
            Container.RegisterInstance<IBillDetailData>(Activator.CreateInstance(typeof(EntityDataAccess.BillDetailData), true) as EntityDataAccess.BillDetailData, InstanceLifetime.Singleton);
            Container.RegisterInstance<IBillInfoData>(Activator.CreateInstance(typeof(EntityDataAccess.BillInfoData), true) as EntityDataAccess.BillInfoData, InstanceLifetime.Singleton);
            Container.RegisterInstance<ICategoryData>(Activator.CreateInstance(typeof(EntityDataAccess.CategoryData), true) as EntityDataAccess.CategoryData, InstanceLifetime.Singleton);
            Container.RegisterInstance<IFoodData>(Activator.CreateInstance(typeof(EntityDataAccess.FoodData), true) as EntityDataAccess.FoodData, InstanceLifetime.Singleton);
            Container.RegisterInstance<ITableData>(Activator.CreateInstance(typeof(EntityDataAccess.TableData), true) as EntityDataAccess.TableData, InstanceLifetime.Singleton);

            DataProvider = EntityDataAccess.DataProvider.Instance;
            ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SqlServerConnection"].ConnectionString;
        }
    }
}
