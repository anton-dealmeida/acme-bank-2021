using acme_bank.Interfaces;
using System;

namespace acme_bank
{
    [Serializable]
    public class WithdrawalAmountTooLargeException : Exception
    {
        public WithdrawalAmountTooLargeException(): 
            base(message: "Insufficientt funds.")
        {

        }

        public WithdrawalAmountTooLargeException(IAccount account)
            : base(message: String.Format($"Insufficient funds. Balance: {account.Balance}\""))
        {

        }
    }
}