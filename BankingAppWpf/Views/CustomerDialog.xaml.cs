using BankingAppWpf.Models;
using BankingAppWpf.ViewModels;
using System.Windows;

namespace BankingAppWpf.Views
{
    /// <summary>
    /// Interaction logic for CustomerDialog.xaml
    /// </summary>
    public partial class CustomerDialog : Window
    {
        public Customer Customer => ViewModel.Customer;

        private CustomerDialogViewModel ViewModel => (CustomerDialogViewModel)DataContext;

        public CustomerDialog(Customer customer = null)
        {
            InitializeComponent();

            CustomerDialogViewModel viewModel = new CustomerDialogViewModel(customer);
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
            FirstNameTextBox.Focus();
        }
    }
}
