namespace BankingAppWpf.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public int AccountId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Purpose { get; set; }
        public string IBAN { get; set; }
        public string TransactionNumber { get; set; }
        public TransactionType Type { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation property
        public Account Account { get; set; }

        public string DisplayType
        {
            get
            {
                return Type switch
                {
                    TransactionType.Withdrawal => "Withdrawal",
                    TransactionType.Deposit => "Deposit",
                    TransactionType.Transfer => "Transfer",
                    TransactionType.Incoming => "Incoming",
                    _ => "Unknown"
                };
            }
        }
    }
}