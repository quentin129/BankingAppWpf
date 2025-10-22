using BankingAppWpf.Helper;
using BankingAppWpf.Models;
using BankingAppWpf.Services;
using BankingAppWpf.Views;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace BankingAppWpf.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly DatabaseService _dbService;

        public MainViewModel()
        {
            _dbService = new DatabaseService();

            LoadCustomersCommand = new RelayCommand(LoadCustomers);
            LoadAccountsCommand = new RelayCommand(LoadAccounts);
            AddCustomerCommand = new RelayCommand(AddCustomer);
            EditCustomerCommand = new RelayCommand<Customer>(EditCustomer, CanEditDeleteCustomer);
            DeleteCustomerCommand = new RelayCommand<Customer>(DeleteCustomer, CanEditDeleteCustomer);
            AddAccountCommand = new RelayCommand(AddAccount);
            EditAccountCommand = new RelayCommand<Account>(EditAccount, CanEditDeleteAccount);
            DeleteAccountCommand = new RelayCommand<Account>(DeleteAccount, CanEditDeleteAccount);
            LoadTransactionsCommand = new RelayCommand<Account>(LoadTransactions);
            AddTransactionCommand = new RelayCommand(AddTransaction);
            EditTransactionCommand = new RelayCommand<Transaction>(EditTransaction, CanEditDeleteTransaction);
            DeleteTransactionCommand = new RelayCommand<Transaction>(DeleteTransaction, CanEditDeleteTransaction);
            RefreshDataCommand = new RelayCommand(RefreshData);

            Customers = new ObservableCollection<Customer>();
            Accounts = new ObservableCollection<Account>();
            Transactions = new ObservableCollection<Transaction>();
            CustomersDataTable = new DataTable();
            AccountsDataTable = new DataTable();
            TransactionsDataTable = new DataTable();

            LoadCustomers();
            LoadAccounts();
        }

        #region Properties
        private ObservableCollection<Customer> _customers;
        public ObservableCollection<Customer> Customers
        {
            get => _customers;
            set => SetProperty(ref _customers, value);
        }

        private ObservableCollection<Account> _accounts;
        public ObservableCollection<Account> Accounts
        {
            get => _accounts;
            set => SetProperty(ref _accounts, value);
        }

        private ObservableCollection<Transaction> _transactions;
        public ObservableCollection<Transaction> Transactions
        {
            get => _transactions;
            set => SetProperty(ref _transactions, value);
        }

        private DataTable _customersDataTable;
        public DataTable CustomersDataTable
        {
            get => _customersDataTable;
            set => SetProperty(ref _customersDataTable, value);
        }

        private DataTable _accountsDataTable;
        public DataTable AccountsDataTable
        {
            get => _accountsDataTable;
            set => SetProperty(ref _accountsDataTable, value);
        }

        private DataTable _transactionsDataTable;
        public DataTable TransactionsDataTable
        {
            get => _transactionsDataTable;
            set => SetProperty(ref _transactionsDataTable, value);
        }

        private Customer _selectedCustomer;
        public Customer SelectedCustomer
        {
            get => _selectedCustomer;
            set => SetProperty(ref _selectedCustomer, value);
        }

        private Account _selectedAccount;
        public Account SelectedAccount
        {
            get => _selectedAccount;
            set
            {
                if (SetProperty(ref _selectedAccount, value) && value != null)
                {
                    LoadTransactions(value);
                }
            }
        }

        private Transaction _selectedTransaction;
        public Transaction SelectedTransaction
        {
            get => _selectedTransaction;
            set => SetProperty(ref _selectedTransaction, value);
        }

        private decimal _currentBalance;
        public decimal CurrentBalance
        {
            get => _currentBalance;
            set => SetProperty(ref _currentBalance, value);
        }

        private string _statusMessage = "Ready";
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }
        #endregion

        #region Commands
        public ICommand LoadCustomersCommand { get; }
        public ICommand LoadAccountsCommand { get; }
        public ICommand AddCustomerCommand { get; }
        public ICommand EditCustomerCommand { get; }
        public ICommand DeleteCustomerCommand { get; }
        public ICommand AddAccountCommand { get; }
        public ICommand EditAccountCommand { get; }
        public ICommand DeleteAccountCommand { get; }
        public ICommand LoadTransactionsCommand { get; }
        public ICommand AddTransactionCommand { get; }
        public ICommand EditTransactionCommand { get; }
        public ICommand DeleteTransactionCommand { get; }
        public ICommand RefreshDataCommand { get; }
        #endregion

        #region Command CanExecute Methods
        private bool CanEditDeleteCustomer(Customer customer) => customer != null;
        private bool CanEditDeleteAccount(Account account) => account != null;
        private bool CanEditDeleteTransaction(Transaction transaction) => transaction != null;
        #endregion

        #region Data Loading Methods
        private void LoadCustomers()
        {
            try
            {
                List<Customer> customers = _dbService.GetCustomers();
                Customers = new ObservableCollection<Customer>(customers);
                CustomersDataTable = _dbService.GetCustomersDataTable();
                StatusMessage = $"{Customers.Count} customers loaded";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customers: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                StatusMessage = "Error loading customers";
            }
        }

        private void LoadAccounts()
        {
            try
            {
                List<Account> accounts = _dbService.GetAccounts();
                Accounts = new ObservableCollection<Account>(accounts);
                AccountsDataTable = _dbService.GetAccountsDataTable();
                StatusMessage = $"{Accounts.Count} accounts loaded";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading accounts: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                StatusMessage = "Error loading accounts";
            }
        }

        private void LoadTransactions(Account account)
        {
            if (account == null) return;

            try
            {
                List<Transaction> transactions = _dbService.GetTransactions(account.AccountId);
                Transactions = new ObservableCollection<Transaction>(transactions);
                TransactionsDataTable = _dbService.GetTransactionsDataTable(account.AccountId);
                CurrentBalance = _dbService.GetAccountBalance(account.AccountId);
                StatusMessage = $"{Transactions.Count} transactions for account {account.AccountNumber} loaded";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading transactions: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                StatusMessage = "Error loading transactions";
            }
        }
        #endregion

        #region CRUD Methods
        private void AddCustomer()
        {
            CustomerDialog dialog = new CustomerDialog();
            if (dialog.ShowDialog() == true && dialog.Customer != null)
            {
                try
                {
                    int customerId = _dbService.SaveCustomer(dialog.Customer);
                    dialog.Customer.CustomerId = customerId;
                    Customers.Add(dialog.Customer);
                    LoadCustomers();
                    StatusMessage = "Customer successfully added";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding customer: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void EditCustomer(Customer customer)
        {
            if (customer == null) return;

            CustomerDialog dialog = new CustomerDialog(customer);
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    _dbService.SaveCustomer(dialog.Customer);
                    LoadCustomers();
                    LoadAccounts();
                    StatusMessage = "Customer successfully updated";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating customer: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DeleteCustomer(Customer customer)
        {
            if (customer == null) return;

            MessageBoxResult result = MessageBox.Show(
                $"Do you really want to delete the customer '{customer.FullName}'?\n\n" +
                $"This action cannot be undone!",
                "Delete Customer",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _dbService.DeleteCustomer(customer.CustomerId);
                    Customers.Remove(customer);
                    LoadCustomers();
                    LoadAccounts();
                    StatusMessage = "Customer successfully deleted";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting customer: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void AddAccount()
        {
            AccountDialog dialog = new AccountDialog(_dbService.GetCustomers().ToList());
            if (dialog.ShowDialog() == true && dialog.Account != null)
            {
                try
                {
                    if (_dbService.AccountNumberExists(dialog.Account.AccountNumber))
                    {
                        MessageBox.Show("This account number already exists.", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    int accountId = _dbService.SaveAccount(dialog.Account);
                    dialog.Account.AccountId = accountId;

                    dialog.Account.Customer = _dbService.GetCustomers()
                        .FirstOrDefault(c => c.CustomerId == dialog.Account.CustomerId);

                    Accounts.Add(dialog.Account);
                    LoadAccounts();
                    StatusMessage = "Account successfully added";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding account: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void EditAccount(Account account)
        {
            if (account == null) return;

            AccountDialog dialog = new AccountDialog(_dbService.GetCustomers().ToList(), account);
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    if (_dbService.AccountNumberExists(dialog.Account.AccountNumber, dialog.Account.AccountId))
                    {
                        MessageBox.Show("This account number already exists.", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    _dbService.SaveAccount(dialog.Account);
                    LoadAccounts();
                    StatusMessage = "Account successfully updated";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating account: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DeleteAccount(Account account)
        {
            if (account == null) return;

            MessageBoxResult result = MessageBox.Show(
                $"Do you really want to delete the account '{account.AccountNumber}'?\n\n" +
                $"Customer: {account.Customer?.FullName}\n" +
                $"Current Balance: {account.CurrentBalance:C}\n\n" +
                $"This action cannot be undone!",
                "Delete Account",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _dbService.DeleteAccount(account.AccountId);
                    Accounts.Remove(account);
                    LoadAccounts();
                    if (SelectedAccount == account)
                    {
                        SelectedAccount = null;
                        Transactions.Clear();
                    }
                    StatusMessage = "Account successfully deleted";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting account: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void AddTransaction()
        {
            if (SelectedAccount == null)
            {
                MessageBox.Show("Please select an account first.", "Information",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            TransactionDialog dialog = new TransactionDialog(SelectedAccount.AccountId);
            if (dialog.ShowDialog() == true && dialog.Transaction != null)
            {
                try
                {
                    if (!string.IsNullOrEmpty(dialog.Transaction.TransactionNumber) &&
                        _dbService.TransactionNumberExists(dialog.Transaction.TransactionNumber))
                    {
                        MessageBox.Show("This transaction number already exists.", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    int transactionId = _dbService.SaveTransaction(dialog.Transaction);
                    dialog.Transaction.TransactionId = transactionId;

                    LoadTransactions(SelectedAccount);
                    StatusMessage = "Transaction successfully added";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding transaction: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void EditTransaction(Transaction transaction)
        {
            if (transaction == null) return;

            TransactionDialog dialog = new TransactionDialog(transaction.AccountId, transaction);
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    if (!string.IsNullOrEmpty(dialog.Transaction.TransactionNumber) &&
                        _dbService.TransactionNumberExists(dialog.Transaction.TransactionNumber, dialog.Transaction.TransactionId))
                    {
                        MessageBox.Show("This transaction number already exists.", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    _dbService.SaveTransaction(dialog.Transaction);
                    LoadTransactions(SelectedAccount);
                    StatusMessage = "Transaction successfully updated";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating transaction: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DeleteTransaction(Transaction transaction)
        {
            if (transaction == null) return;

            MessageBoxResult result = MessageBox.Show(
                $"Do you really want to delete this transaction?\n\n" +
                $"Date: {transaction.Date:dd.MM.yyyy}\n" +
                $"Amount: {transaction.Amount:C}\n" +
                $"Type: {transaction.DisplayType}\n" +
                $"Purpose: {transaction.Purpose}\n\n" +
                $"Note: The balance remains unchanged (Soft-Delete).",
                "Delete Transaction",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _dbService.DeleteTransaction(transaction.TransactionId);
                    Transactions.Remove(transaction);
                    // Balance remains unchanged due to soft delete
                    StatusMessage = "Transaction successfully deleted (balance unchanged)";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting transaction: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RefreshData()
        {
            LoadCustomers();
            LoadAccounts();
            if (SelectedAccount != null)
            {
                LoadTransactions(SelectedAccount);
            }
            StatusMessage = "Data updated";
        }
        #endregion
    }
}