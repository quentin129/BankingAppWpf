namespace BankingAppWpf.Models
{
    public class Account
    {
        public int AccountId { get; set; }
        public string AccountNumber { get; set; }
        public int CustomerId { get; set; }
        public decimal StartBalance { get; set; }
        public decimal CurrentBalance { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation property
        public Customer Customer { get; set; }

        public string DisplayInfo => $"{AccountNumber} - {Customer?.FullName}";
    }
}
