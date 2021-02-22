using acme_bank.Extensions;
using acme_bank.Interfaces;
using acme_bank.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace acme_bank.Services
{
    /// <summary>
    /// Initializes an in-memory database service.
    /// </summary>
    /// Note: This used the Singleton pattern. It ensures that there is only 1 instantiated version of this list and using Lazy loading of the Singleton
    /// allows for preventing unsafe threading practices.
    public sealed class SystemDB
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static IList<Account> accounts;
        private SystemDB()
        {
            accounts = new List<Account>() { };
            Add(new SavingsAccount() { Id = GetAccountId, CustomerId = 1, Balance = 2000 });
            Add(new SavingsAccount() { Id = GetAccountId, CustomerId = 2, Balance = 5000 });
            Add(new CurrentAccount() { Id = GetAccountId, CustomerId = 3, Balance = 1000, Overdraft = 10000 });
            Add(new CurrentAccount() { Id = GetAccountId, CustomerId = 4, Balance = -5000, Overdraft = 20000 });
        }

        private static readonly SystemDB instance = new SystemDB();
        public static SystemDB Instance
        {
            get
            {
                return instance;
            }
        }

        // This is a very simplistic implementation of a user id, there is a limitation for this. I would have personally used
        // this field to distinguish between bank accounts for a client. A client would normally be able to have multiple types of accounts
        // and multiples of the same types of accounts. I would've used this identifier to distinguish between them as well and attempted to
        // do so as simplistically as possible.
        private static int GetAccountId => accounts.Any() ? accounts.Count : 0;

        // In order to ensure that there is only 1 static and consistent list being used all around, I expose the original list via this property.
        // note: this exposes a reference value - thus when this list's existing values are updated directly so are the original lists values.
        // note: updating this list itself to add new values or remove values will not reflect on the original list. Only updates to the existing objects.
        public static IEnumerable<IAccount> GetAccounts => accounts;

        /// <summary>
        /// Saves changes to a specific account to the original data source.
        /// </summary>
        /// <param name="account"></param>
        // Passing in a full account object was a simple approach to updating the account. Ideally I would have implemented this a little differently.
        // I would also not have placed this on the data layer but in this case with this singleton approach, this is one of the safer ways to do so to
        // add new values to the existing list. It is recommended to use this to update the orignal values. There could be a security flaw here but that
        // was out of scope.
        public static void Save(IAccount account)
        {
            try
            {
                accounts.Where(acc => acc.Id == account.Id).ToList().SetValue(acc => { acc = (Account)account; });
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occured: {e.Message}");
                Logger.Info(e, $"Failed to save changes to account {account.Id}.\n{account.ToString()}");
            }
        }

        // I felt that any types of open of accounts should slap the account to the db (even if it's in memory.)
        // I hate it when my details get lost, so my totally not real fake user would also feel disrespected..
        /// <summary>
        /// Add a new record to the accounts list. It will not be persisted between sessions.
        /// </summary>
        /// <param name="account">Pass in an object that inherits from type Account.</param>
        // This is a very hard coded "factory pattern" like implementation of a .Add() method. I take in the new account that should be added and then 
        // append it to the existing IEnumerable list and convert that back to a concrete list as that is what I was originally using.
        // This is a semi-generic and clean way to do this. However, in a real life situation there would be an actual data layer so this is improvised.
        public static IEnumerable<IAccount> Add(IAccount account)
        {
            try
            {
                account.Id = GetAccountId;
                if (account.GetType() == typeof(CurrentAccount))
                    accounts = accounts.Append((CurrentAccount)account).ToList();
                else if (account.GetType() == typeof(SavingsAccount))
                    accounts = accounts.Append((SavingsAccount)account).ToList();
                else throw new AccountNotFoundException();

                return accounts;
            }
            catch (Exception e)
            {
                Logger.Info(e, $"An error occured adding {account.Id} for {account.CustomerId}\n{e.Message}");
            }
            return null;
        }
    }
}