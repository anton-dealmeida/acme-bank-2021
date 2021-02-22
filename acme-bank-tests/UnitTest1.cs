using acme_bank.Interfaces;
using acme_bank.Models;
using acme_bank.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace acme_bank_tests
{
    [TestClass]
    public class UnitTest1
    {
        //AccountService accountService = new AccountService();

        [TestMethod]
        public void SuccessfulGetAccounts()
        {
            AccountService accountService = new();
            var accounts = SystemDB.GetAccounts;
            Assert.IsNotNull(accounts);
        }

        [TestMethod]
        public void SuccessfulOpenNewSavingsAccount()
        {
            AccountService accountService = new();
            IAccount account = accountService.OpenSavingsAccount(94061551, 1000);
            Assert.IsTrue(account.GetType() == typeof(SavingsAccount));
            Assert.IsTrue(SystemDB.GetAccounts.Contains(account));
        }

        [TestMethod]
        public void DepositToExistingAccount()
        {
            int amountToDeposit = 5000;
            AccountService accountService = new();
            IAccount account = SystemDB.GetAccounts.First();
            double original_value = account.Balance;
            account = accountService.Deposit(account.Id, amountToDeposit);

            Assert.IsNotNull(account);
            Assert.AreNotEqual(original_value, account.Balance);
            Assert.IsTrue(original_value + amountToDeposit == account.Balance);
        }

        [TestMethod]
        public void FailedDepositToAccountNotExist()
        {
            AccountService accountService = new();
            IAccount account = accountService.Deposit(-2345678, 5000);

            Assert.IsNull(account);
        }

        [TestMethod]
        public void SuccessfullWithdrawFromCurrent()
        {
            AccountService accountService = new();

            int withdrawAmount = 1000;
            IAccount _account = SystemDB.GetAccounts.First();
            double original_value = _account.Balance;
            IAccount account = accountService.Withdraw(_account.Id, withdrawAmount);

            Assert.AreNotEqual(original_value, account.Balance);
            Assert.AreEqual(original_value - withdrawAmount, account.Balance);
        }

        [TestMethod]
        public void WithdrawFromCurrentExceedOverdraft()
        {
            AccountService accountService = new();
            IAccount _account = SystemDB.GetAccounts.OfType<CurrentAccount>().First();
            int withdrawAmount = (int)(_account.Balance * _account.Balance);

            // Makes sure that withdrawal amount will exceed Overdraft.
            if (_account.GetType() == typeof(CurrentAccount))
                withdrawAmount += (int)((CurrentAccount)_account).Overdraft;

            IAccount account = accountService.Withdraw(_account.Id, withdrawAmount);

            Assert.IsNull(account);
        }

        public void WithdrawFromAccountsExceedMinimumBlana()
        {
            AccountService accountService = new();
            foreach (var _account in SystemDB.GetAccounts)
            {
                int withdrawAmount = (int)(_account.Balance * _account.Balance);

                // Makes sure that withdrawal amount will exceed Overdraft (if any).
                if (_account.GetType() == typeof(CurrentAccount))
                    withdrawAmount += (int)((CurrentAccount)_account).Overdraft;

                IAccount account = accountService.Withdraw(_account.Id, withdrawAmount);

                Assert.IsNull(account);
            }
        }

        [TestMethod]
        public void WithdrawFromSavingsExceedMinimumBalance()
        {
            int minimumSavingsBalance = 1000;
            AccountService accountService = new();
            IAccount account = SystemDB.GetAccounts.OfType<SavingsAccount>().First();
            double initialBalance = account.Balance;
            int withdrawAmount = (int)(account.Balance + account.Balance);
            account = accountService.Withdraw(account.Id, withdrawAmount);

            double balanceAfterWithdrawal = (initialBalance - minimumSavingsBalance) - withdrawAmount;

            Assert.IsTrue(balanceAfterWithdrawal <= 0);
            Assert.IsNull(account);
        }

        [TestMethod]
        public void FailOpenSavingsAccountLessThanMinimumBalance()
        {
            AccountService accountService = new();
            IAccount account = accountService.OpenSavingsAccount(94061552, 0); // if creation fails, returns a null object for account.
            Assert.IsNull(account);
            Assert.IsFalse(SystemDB.GetAccounts.Contains(account));
        }
    }
}
