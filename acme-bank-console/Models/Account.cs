using acme_bank.Interfaces;
using acme_bank.Services;
using System;
using System.Linq;

namespace acme_bank.Models
{
    public abstract class Account : IAccount
    {
        public long Id { get; set; }
        public int CustomerId { get; set; }
        public double Balance { get; set; }
        public override string ToString() { return $"{{ Account: {{ {Id}, {CustomerId}, {Balance} }} }}"; }
    }
}