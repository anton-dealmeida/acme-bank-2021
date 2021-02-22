namespace acme_bank.Models
{
    public class SavingsAccount : Account
    {
        public override string ToString() { return $"{{ SavingsAccount: {{ {Id}, {CustomerId}, {Balance} }} }}"; }

    }
}