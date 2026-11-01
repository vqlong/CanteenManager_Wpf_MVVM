using Interface;
using Model;
using System.Collections.Generic;
using Unity;

namespace Bus
{
    public class AccountBus
    {
        private AccountBus()
        {

        }

        private static readonly AccountBus instance = new AccountBus();
        public static AccountBus Instance => instance;
        IAccountData Account => Config.Container.Resolve<IAccountData>();

        public bool DeleteAccount(string username) => Account.DeleteAccount(username);
        public List<Account> GetListAccount() => Account.GetListAccount();
        public Account InsertAccount(string username) => Account.InsertAccount(username);
        public Account Login(string username, string password) => Account.Login(username, password);
        /// <summary>
        /// Update Account.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="displayname"></param>
        /// <param name="password"></param>
        /// <returns>
        /// Item1 - Kết quả cập nhật DisplayName.
        /// <br>Item2 - Kết quả cập nhật Password.</br>
        /// <br>Item3 - Kết quả cập nhật Type.</br>
        /// </returns>
        public (bool, bool, bool) Update(string username, string? displayname = null, string? password = null, AccountType? type = null) => Account.Update(username, displayname, password, type);
    }
}
