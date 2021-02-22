namespace acme_bank.Interfaces
{
    public interface IAccount
    {
        public long Id { get; set; }
        public int CustomerId { get; set; }
        public double Balance { get; set; }

        public string ToString();
    }
}