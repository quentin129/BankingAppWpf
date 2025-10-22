using BankingAppWpf.Models;
using System.Globalization;
using System.Windows.Data;

namespace BankingAppWpf.Helper.Converters
{
    public class TransactionTypeConverter : IValueConverter
    {
        public static TransactionTypeConverter Instance { get; } = new TransactionTypeConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TransactionType type)
            {
                return type switch
                {
                    TransactionType.Withdrawal => "Withdrawal",
                    TransactionType.Deposit => "Deposit",
                    TransactionType.Transfer => "Transfer",
                    TransactionType.Incoming => "Incoming",
                    _ => value.ToString()
                };
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                return str switch
                {
                    "Withdrawal" => TransactionType.Withdrawal,
                    "Deposit" => TransactionType.Deposit,
                    "Transfer" => TransactionType.Transfer,
                    "Incoming" => TransactionType.Incoming,
                    _ => TransactionType.Withdrawal
                };
            }
            return TransactionType.Withdrawal;
        }
    }
}