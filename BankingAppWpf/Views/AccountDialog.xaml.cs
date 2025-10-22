using BankingAppWpf.Models;
using BankingAppWpf.ViewModels;
using System.Windows;

namespace BankingAppWpf.Views
{
    /// <summary>
    /// Interaction logic for AccountDialog.xaml
    /// </summary>
    public partial class AccountDialog : Window
    {
        public Account Account => ViewModel.Account;

        private AccountDialogViewModel ViewModel => (AccountDialogViewModel)DataContext;

        public AccountDialog(List<Customer> customers, Account account = null)
        {
            InitializeComponent();

            // Nur EINE ViewModel-Instanz erstellen und für DataContext verwenden
            AccountDialogViewModel viewModel = new AccountDialogViewModel(customers, account);
            viewModel.RequestClose += (result) =>
            {
                this.DialogResult = result;
                this.Close();
            };

            DataContext = viewModel; // Hier die gleiche Instanz verwenden!
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            AccountNumberTextBox.Focus();
        }
    }
}