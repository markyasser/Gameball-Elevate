namespace Gameball_Elevate.Models
{
    public struct TransactionType
    {
        public const string Added = "Added";
        public const string Redeemed = "Redeemed";
    }
    public class Transaction
    {
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public int Points { get; set; }

        public int CurrentBalance { get; set; }

        public string type { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
