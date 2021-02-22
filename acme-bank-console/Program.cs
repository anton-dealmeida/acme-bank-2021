using acme_bank.Interfaces;
using acme_bank.Models;
using acme_bank.Services;
using NLog;
using System;
using System.Linq;

namespace acme_bank
{
    class Program
    {
        // static reusable implementation of the Logging tool.
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        // This is just a live play ground. The Unit Tests were used to validate functionality.
        static void Main(string[] args)
        {
            AccountService accountService = new();
            var accounts = SystemDB.GetAccounts;

            // If no accounts found, throw AccountNotFound Exception.
            if (!accounts.Any())
                throw new AccountNotFoundException(accounts);

            Logger.Info("List current accounts:");
            foreach (CurrentAccount item in accounts.OfType<CurrentAccount>().ToList())
            {
                Logger.Info($"{item.Id}\t{item.CustomerId}\t{item.Balance}\t{item.Overdraft}");
            }

            Logger.Info("List savings accounts:");
            foreach (SavingsAccount item in accounts.OfType<SavingsAccount>().ToList())
            {
                Logger.Info($"{item.Id}\t{item.CustomerId}\t{item.Balance}");
            }

            Logger.Info("List current accounts:");
            foreach (CurrentAccount item in accounts.OfType<CurrentAccount>().ToList())
            {
                Logger.Info($"{item.Id}\t{item.CustomerId}\t{item.Balance}\t{item.Overdraft}");
            }

            IAccount account = SystemDB.GetAccounts.ToList()[0];
            //account = SystemDB.GetAccounts.ToList()[0];
            Logger.Info($"Deposit into account {account.Id}");
            account.GetType();
            Logger.Info($"Deposit 10,000 to {account.Id}");
            accountService.Deposit(account.Id, 10000);
            Logger.Info($"Account: {account.Id} has balance of {account.Balance}");
            Logger.Info($"Withdraw 5,000 from account {account.Id}");
            accountService.Withdraw(account.Id, 5000);
            Logger.Info($"Account: {account.Id} has balance of {account.Balance}");

            Console.ReadKey(false);
        }
    }
}