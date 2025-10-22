using BankingAppWpf.Helper;
using BankingAppWpf.Models;
using BankingAppWpf.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace BankingAppWpf.ViewModels
{
    public class TransactionDialogViewModel : ViewModelBase
    {
        private readonly DatabaseService _dbService;
        private Transaction _transaction;
        private bool _isEditMode;
        private int _accountId;
        private TransactionType _selectedTransactionType;

        public TransactionDialogViewModel(int accountId, Transaction transaction = null)
        {
            _dbService = new DatabaseService();
            _accountId = accountId;

            Transaction = transaction ?? new Transaction
            {
                AccountId = accountId,
                Date = DateTime.Now,
                IsActive = true,
                Type = TransactionType.Deposit
            };

            // Separate Property für die ComboBox
            _selectedTransactionType = Transaction.Type;

            IsEditMode = transaction != null;

            TransactionTypes = new ObservableCollection<TransactionType>
            {
                TransactionType.Withdrawal,
                TransactionType.Deposit,
                TransactionType.Transfer,
                TransactionType.Incoming
            };

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel);
            GenerateTransactionNumberCommand = new RelayCommand(GenerateTransactionNumber);

            if (!IsEditMode)
            {
                GenerateTransactionNumber();
            }
        }

        public Transaction Transaction
        {
            get => _transaction;
            set => SetProperty(ref _transaction, value);
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        public TransactionType SelectedTransactionType
        {
            get => _selectedTransactionType;
            set
            {
                if (SetProperty(ref _selectedTransactionType, value))
                {
                    // Transaction.Type synchronisieren
                    Transaction.Type = value;
                    // ShowIbanField aktualisieren
                    OnPropertyChanged(nameof(ShowIbanField));
                }
            }
        }

        public bool ShowIbanField => SelectedTransactionType == TransactionType.Transfer;

        public ObservableCollection<TransactionType> TransactionTypes { get; }

        public string Title => IsEditMode ? "Edit Transaction" : "New Transaction";
        public string SaveButtonText => IsEditMode ? "Save" : "Add";

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand GenerateTransactionNumberCommand { get; }

        public event Action<bool?> RequestClose;

        private bool CanSave()
        {
            bool basicValidation = Transaction.Amount != 0 &&
                   !string.IsNullOrWhiteSpace(Transaction.Purpose) &&
                   Transaction.Date != default;

            // IBAN is only required for Transfer transactions
            bool ibanValidation = SelectedTransactionType != TransactionType.Transfer ||
                                 !string.IsNullOrWhiteSpace(Transaction.IBAN);

            return basicValidation && ibanValidation;
        }

        private void Save()
        {
            if (!CanSave())
            {
                if (SelectedTransactionType == TransactionType.Transfer && string.IsNullOrWhiteSpace(Transaction.IBAN))
                {
                    MessageBox.Show("IBAN is required for transfer transactions.",
                        "IBAN Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                MessageBox.Show("Please fill in all required fields (*).",
                    "Incomplete Data", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!string.IsNullOrWhiteSpace(Transaction.TransactionNumber) &&
                _dbService.TransactionNumberExists(Transaction.TransactionNumber, Transaction.TransactionId))
            {
                MessageBox.Show("This transaction number already exists. Please choose a different transaction number.",
                    "Transaction Number Exists", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!ValidateAmountForType())
            {
                return;
            }

            RequestClose?.Invoke(true);
        }

        private bool ValidateAmountForType()
        {
            if (SelectedTransactionType == TransactionType.Withdrawal && Transaction.Amount > 0)
            {
                MessageBox.Show("For withdrawals, the amount must be negative.",
                    "Invalid Amount", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if ((SelectedTransactionType == TransactionType.Deposit ||
                 SelectedTransactionType == TransactionType.Incoming) && Transaction.Amount < 0)
            {
                MessageBox.Show("For deposits and incoming transactions, the amount must be positive.",
                    "Invalid Amount", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void Cancel()
        {
            RequestClose?.Invoke(false);
        }

        private void GenerateTransactionNumber()
        {
            Random random = new Random();
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string randomPart = random.Next(100, 999).ToString();
            string newTransactionNumber = $"{timestamp}-{randomPart}";

            Transaction.TransactionNumber = newTransactionNumber;

            Transaction temp = Transaction;
            Transaction = null;
            Transaction = temp;
        }
    }
}