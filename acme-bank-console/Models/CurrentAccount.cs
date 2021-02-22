namespace acme_bank.Models
{
    public class CurrentAccount : Account
    {
        public double Overdraft { get; set; }
        public override string ToString() { return $"{{ CurrentAccount: {{ {Id}, {CustomerId}, {Balance}, {Overdraft} }} }}"; }

    }
}