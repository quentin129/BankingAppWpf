using BankingAppWpf.Helper;
using BankingAppWpf.Models;
using BankingAppWpf.Services;
using System.Windows;
using System.Windows.Input;

namespace BankingAppWpf.ViewModels
{
    public class AccountDialogViewModel : ViewModelBase
    {
        private readonly DatabaseService _dbService;
        private Account _account;
        private bool _isEditMode;
        private List<Customer> _customers;
        private Customer _selectedCustomer;

        public AccountDialogViewModel(List<Customer> customers, Account account = null)
        {
            _dbService = new DatabaseService();
            Customers = customers;
            Account = account ?? new Account();
            IsEditMode = account != null;

            if (IsEditMode && account != null)
            {
                SelectedCustomer = Customers.FirstOrDefault(c => c.CustomerId == account.CustomerId);
            }

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel);
            GenerateAccountNumberCommand = new RelayCommand(GenerateAccountNumber);
        }

        public Account Account
        {
            get => _account;
            set => SetProperty(ref _account, value);
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        public List<Customer> Customers
        {
            get => _customers;
            set => SetProperty(ref _customers, value);
        }

        public Customer SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                if (SetProperty(ref _selectedCustomer, value) && value != null)
                {
                    Account.CustomerId = value.CustomerId;
                }
            }
        }

        public string Title => IsEditMode ? "Edit Account" : "New Account";
        public string SaveButtonText => IsEditMode ? "Save" : "Add";

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand GenerateAccountNumberCommand { get; }

        public bool DialogResult { get; set; }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Account.AccountNumber) &&
                   SelectedCustomer != null;
        }

        public event Action<bool?> RequestClose;

        private void Save()
        {
            if (!CanSave())
            {
                MessageBox.Show("Please fill in all required fields.",
                    "Incomplete Data", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_dbService.AccountNumberExists(Account.AccountNumber, Account.AccountId))
            {
                MessageBox.Show("This account number already exists.",
                    "Account Number Exists", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            RequestClose?.Invoke(true);
        }

        private void Cancel()
        {
            RequestClose?.Invoke(false);
        }

        private void GenerateAccountNumber()
        {
            Random random = new Random();
            string accountNumber = "DE" + random.Next(100000000, 999999999).ToString();

            Account newAccount = new Account
            {
                AccountId = Account.AccountId,
                AccountNumber = accountNumber,
                CustomerId = Account.CustomerId,
                StartBalance = Account.StartBalance,
                CurrentBalance = Account.CurrentBalance
            };

            Account = newAccount;
        }
    }
}