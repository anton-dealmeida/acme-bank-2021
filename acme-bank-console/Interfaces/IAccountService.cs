using acme_bank.Interfaces;
using acme_bank.Models;

namespace acme_bank
{
    public interface IAccountService
    {
        public SavingsAccount OpenSavingsAccount(int customerId, long amountToDeposit);
        public CurrentAccount OpenCurrentAccount(int customerId);
        public IAccount Withdraw(long accountId, int amountToWithdraw);
        public IAccount Deposit(long accountId, int amountToDeposit);
    }
}