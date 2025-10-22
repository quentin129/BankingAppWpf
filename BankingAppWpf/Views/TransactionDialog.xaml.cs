using BankingAppWpf.Models;
using BankingAppWpf.ViewModels;
using System.Windows;

namespace BankingAppWpf.Views
{
    public partial class TransactionDialog : Window
    {
        public Transaction Transaction => ViewModel.Transaction;

        private TransactionDialogViewModel ViewModel => (TransactionDialogViewModel)DataContext;

        public TransactionDialog(int accountId, Transaction transaction = null)
        {
            InitializeComponent();

            TransactionDialogViewModel viewModel = new TransactionDialogViewModel(accountId, transaction);
            viewModel.RequestClose += (result) =>
            {
                this.DialogResult = result;
                this.Close();
            };

            DataContext = viewModel;
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            DatePicker.Focus();
        }
    }
}