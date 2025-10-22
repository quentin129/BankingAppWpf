using BankingAppWpf.Views;
using System.Windows;

namespace BankingAppWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                // Test database connection
                Services.DatabaseService dbService = new Services.DatabaseService();
                if (!dbService.TestConnection())
                {
                    MessageBox.Show(
                        "No connection to database possible.\nPlease check the connection string settings.",
                        "Database Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    Shutdown();
                    return;
                }

                // Start MainView
                MainView mainView = new MainView();
                mainView.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error starting the application: {ex.Message}",
                    "Startup Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Shutdown();
            }
        }
    }
}