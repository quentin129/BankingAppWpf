using System.Globalization;
using System.Windows.Data;

namespace BankingAppWpf.Helper.Converters
{
    public class NullToBoolConverter : IValueConverter
    {
        public static NullToBoolConverter Instance { get; } = new NullToBoolConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
