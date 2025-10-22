using BankingAppWpf.Helper;
using BankingAppWpf.Models;
using BankingAppWpf.Services;
using System.Windows;
using System.Windows.Input;

namespace BankingAppWpf.ViewModels
{
    public class CustomerDialogViewModel : ViewModelBase
    {
        private Customer _customer;
        private bool _isEditMode;

        public CustomerDialogViewModel(Customer customer = null)
        {
            Customer = customer ?? new Customer();
            IsEditMode = customer != null;

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel);
        }

        public Customer Customer
        {
            get => _customer;
            set => SetProperty(ref _customer, value);
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        public string Title => IsEditMode ? "Edit Customer" : "New Customer";
        public string SaveButtonText => IsEditMode ? "Save" : "Add";

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event Action<bool?> RequestClose;

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Customer.FirstName) &&
                   !string.IsNullOrWhiteSpace(Customer.LastName) &&
                   !string.IsNullOrWhiteSpace(Customer.Street) &&
                   !string.IsNullOrWhiteSpace(Customer.City) &&
                   !string.IsNullOrWhiteSpace(Customer.PostalCode);
        }

        private void Save()
        {
            if (!CanSave())
            {
                MessageBox.Show("Please fill in all required fields (*).",
                    "Incomplete Data", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!string.IsNullOrWhiteSpace(Customer.Email) && !IsValidEmail(Customer.Email))
            {
                MessageBox.Show("Please enter a valid email address or leave the field empty.",
                    "Invalid Email", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            RequestClose?.Invoke(true);
        }

        private void Cancel()
        {
            RequestClose?.Invoke(false);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                System.Net.Mail.MailAddress addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}