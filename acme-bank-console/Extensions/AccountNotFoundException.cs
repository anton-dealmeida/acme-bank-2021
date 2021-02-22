using acme_bank.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace acme_bank
{
    [Serializable]
    public class AccountNotFoundException : Exception
    {
        public AccountNotFoundException()
        {

        }

        public AccountNotFoundException(string account) :
            base(String.Format($"Account not found: {account}"))
        {

        }

        public AccountNotFoundException(IEnumerable<IAccount> enumerable) :
            base(message: $"No accounts found!\nAccounts in DB: {enumerable.Count()}")
        {

        }
    }
}