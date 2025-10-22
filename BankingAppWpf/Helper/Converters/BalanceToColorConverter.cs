using System.Globalization;
using System.Windows.Data;

namespace BankingAppWpf.Helper.Converters
{
    public class BalanceToColorConverter : IValueConverter
    {
        public static BalanceToColorConverter Instance { get; } = new BalanceToColorConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal balance)
            {
                return balance >= 0 ? "Green" : "Red";
            }
            return "Black";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
