using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace BankingAppWpf.Views
{
    /// <summary>
    /// Interaction logic for AbboutDialog.xaml
    /// </summary>
    public partial class AboutDialog : Window
    {
        public AboutDialog()
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void EmailTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SendEmail();
        }

        private void SendEmail()
        {
            string email = "20quentin05@gmail.com";
            string subject = "Inquiry regarding BankingApp WPF";
            string body = "Dear Quentin,\n\n";

            string mailtoUri = $"mailto:{email}?subject={System.Uri.EscapeDataString(subject)}&body={System.Uri.EscapeDataString(body)}";

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = mailtoUri,
                    UseShellExecute = true
                });
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(
                    $"Email client could not be opened:\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
    }
}