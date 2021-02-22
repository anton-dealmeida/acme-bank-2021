using acme_bank.Interfaces;
using acme_bank.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace acme_bank.Services
{
    public class AccountService : IAccountService
    {
        // the Logger object is used through out - Dependency Injection would be a better way to do this. This app use case is too small
        // for a realistic implementation of DI so doing so would be a bit overkill.
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Deposit funds into specified acount.
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="amountToDeposit"></param>
        public IAccount Deposit(long accountId, int amountToDeposit)
        {
            if (amountToDeposit > 0)
            {
                try
                {
                    IEnumerable<IAccount> accounts = SystemDB.GetAccounts.Where(a => a.Id == accountId).Select(a => a);
                    if (accounts.Any())
                    {
                        IAccount account = accounts.FirstOrDefault();
                        account.Balance += amountToDeposit;
                        SystemDB.Save(account);
                        return account;
                    }
                    else
                    {
                        throw new AccountNotFoundException();
                    }
                }
                catch (AccountNotFoundException e)
                {
                    Logger.Info(e, $"Account {accountId} not found.");
                }
                catch (Exception e)
                {
                    Logger.Info(e, $"An error occured processing deposit for {accountId}");
                }
            }
            else
            {
                Logger.Info($"Deposit failed. The amount to deposit (R{amountToDeposit}) can not be below or equal to R0.");
            }
            return null;
        }

        /// <summary>
        /// Creates a new Curron account for specified custsomer Id.
        /// Bank accounts are linked to customerIds.
        /// </summary>
        /// <param name="customerId">Client for which the account should be opened</param>
        public CurrentAccount OpenCurrentAccount(int customerId)
        {
            try
            {
                IAccount newAccount = new CurrentAccount() { CustomerId = customerId, Balance = 0, Overdraft = 0 };
                SystemDB.Add(newAccount);
                return (CurrentAccount)newAccount;
            }
            catch (Exception e)
            {
                Logger.Info(e, $"Failed to open a new current account for {customerId}.");
                return null;
            }
        }

        /// <summary>
        /// Opens a new savings account for specified customer.
        /// </summary>
        /// <param name="customerId">Client for which the account should be opened</param>
        /// <param name="amountToDeposit">May be no less than R1000</param>
        // accept the customerID (ideally identity number but in this case a unique string of numbers. ID won't work because the int range stops before their ID is fully inserted.)
        public SavingsAccount OpenSavingsAccount(int customerId, long amountToDeposit)
        {
            if (amountToDeposit >= 1000.00d)
            {
                try
                {
                    IAccount newAccount = new SavingsAccount() { CustomerId = customerId, Balance = amountToDeposit };
                    var accounts = SystemDB.Add(newAccount);
                    return (SavingsAccount)newAccount;
                }
                catch (Exception e)
                {
                    Logger.Info(e, $"Failed to open a new account for {customerId} with deposit of {amountToDeposit}.");
                    return null;
                }
            }
            else if (amountToDeposit <= 1000.00)
            {
                Logger.Info($"Minimum deposit requirement not met. {amountToDeposit} is less than R1000.");
            }
            return null;
        }

        /// <summary>
        /// Withdraw sum of money from specified account.
        /// </summary>
        /// <param name="accountId">Account from which to withdraw funds.</param>
        /// <param name="amountToWithdraw">Amount to be withdrawn from specified account.</param>
        // takes accountId (because the accountId would be unique.. for the most part in this small case) and amountToWithdraw.
        // in a real world use-case I would take the userID + accountId + amountToWithdraw into this method. The userId would be a unique
        // session-based ID attached to their ID number because the ID number is unique and an ideal value to use to identify customers.
        // I then do very basic validation of whether or not this passes the criteria. The criteria test can probably be abstracted but I felt
        // there's no need for that in this case, it would complicate the code too much to DRY it up that much.
        public IAccount Withdraw(long accountId, int amountToWithdraw)
        {
            try
            {
                IAccount account = SystemDB.GetAccounts.Select(a => a).Where(a => a.Id == accountId).FirstOrDefault();

                if (account.GetType() == typeof(SavingsAccount))
                    if (account.Balance - amountToWithdraw < 1000)
                        throw new WithdrawalAmountTooLargeException();

                if (account.GetType() == typeof(CurrentAccount))
                    if ((((account as CurrentAccount).Overdraft + account.Balance) - amountToWithdraw) < 0)
                        throw new WithdrawalAmountTooLargeException();

                account.Balance -= amountToWithdraw;
                SystemDB.Save(account);
                return account;

            }
            catch (WithdrawalAmountTooLargeException e)
            {
                Logger.Info(e, "Insufficient funds.");
            }
            catch (Exception e)
            {
                Logger.Info(e, $"{e.Message}");
            }
            return null;
        }
    }
}
