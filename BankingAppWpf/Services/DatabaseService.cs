using BankingAppWpf.Models;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;

namespace BankingAppWpf.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;
        }

        #region Customer Operations
        public DataTable GetCustomersDataTable()
        {
            using MySqlConnection conn = new MySqlConnection(_connectionString);
            conn.Open();
            MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Customers ORDER BY LastName, FirstName", conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public List<Customer> GetCustomers()
        {
            List<Customer> customers = new List<Customer>();

            using MySqlConnection conn = new MySqlConnection(_connectionString);
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand("SELECT * FROM Customers ORDER BY LastName, FirstName", conn);
            using MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                customers.Add(new Customer
                {
                    CustomerId = reader.GetInt32("CustomerId"),
                    FirstName = reader.GetString("FirstName"),
                    LastName = reader.GetString("LastName"),
                    Street = reader.IsDBNull("Street") ? null : reader.GetString("Street"),
                    HouseNumber = reader.IsDBNull("HouseNumber") ? null : reader.GetString("HouseNumber"),
                    PostalCode = reader.IsDBNull("PostalCode") ? null : reader.GetString("PostalCode"),
                    City = reader.IsDBNull("City") ? null : reader.GetString("City"),
                    Phone = reader.IsDBNull("Phone") ? null : reader.GetString("Phone"),
                    Email = reader.IsDBNull("Email") ? null : reader.GetString("Email")
                });
            }

            return customers;
        }

        public int SaveCustomer(Customer customer)
        {
            using MySqlConnection conn = new MySqlConnection(_connectionString);
            conn.Open();

            string query = customer.CustomerId == 0 ?
                @"INSERT INTO Customers (FirstName, LastName, Street, HouseNumber, PostalCode, City, Phone, Email) 
                  VALUES (@FirstName, @LastName, @Street, @HouseNumber, @PostalCode, @City, @Phone, @Email);
                  SELECT LAST_INSERT_ID();" :
                @"UPDATE Customers SET FirstName=@FirstName, LastName=@LastName, Street=@Street, 
                  HouseNumber=@HouseNumber, PostalCode=@PostalCode, City=@City, Phone=@Phone, Email=@Email 
                  WHERE CustomerId=@CustomerId";

            using MySqlCommand cmd = new MySqlCommand(query, conn);
            AddCustomerParameters(cmd, customer);

            if (customer.CustomerId == 0)
            {
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                cmd.ExecuteNonQuery();
                return customer.CustomerId;
            }
        }

        public void DeleteCustomer(int customerId)
        {
            using MySqlConnection conn = new MySqlConnection(_connectionString);
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand("DELETE FROM Customers WHERE CustomerId = @CustomerId", conn);
            cmd.Parameters.AddWithValue("@CustomerId", customerId);
            cmd.ExecuteNonQuery();
        }

        private void AddCustomerParameters(MySqlCommand cmd, Customer customer)
        {
            cmd.Parameters.AddWithValue("@FirstName", customer.FirstName);
            cmd.Parameters.AddWithValue("@LastName", customer.LastName);
            cmd.Parameters.AddWithValue("@Street", customer.Street ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@HouseNumber", customer.HouseNumber ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@PostalCode", customer.PostalCode ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@City", customer.City ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Phone", customer.Phone ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Email", customer.Email ?? (object)DBNull.Value);

            if (customer.CustomerId > 0)
            {
                cmd.Parameters.AddWithValue("@CustomerId", customer.CustomerId);
            }
        }
        #endregion

        #region Account Operations
        public DataTable GetAccountsDataTable()
        {
            using MySqlConnection conn = new MySqlConnection(_connectionString);
            conn.Open();
            MySqlDataAdapter da = new MySqlDataAdapter(@"
                SELECT a.*, c.FirstName, c.LastName 
                FROM Accounts a 
                INNER JOIN Customers c ON a.CustomerId = c.CustomerId 
                ORDER BY a.AccountNumber", conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public List<Account> GetAccounts()
        {
            List<Account> accounts = new List<Account>();

            using MySqlConnection conn = new MySqlConnection(_connectionString);
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand(@"
                SELECT a.*, c.FirstName, c.LastName 
                FROM Accounts a 
                INNER JOIN Customers c ON a.CustomerId = c.CustomerId 
                ORDER BY a.AccountNumber", conn);
            using MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                accounts.Add(new Account
                {
                    AccountId = reader.GetInt32("AccountId"),
                    AccountNumber = reader.GetString("AccountNumber"),
                    CustomerId = reader.GetInt32("CustomerId"),
                    StartBalance = reader.GetDecimal("StartBalance"),
                    CurrentBalance = reader.GetDecimal("CurrentBalance"),
                    Customer = new Customer
                    {
                        CustomerId = reader.GetInt32("CustomerId"),
                        FirstName = reader.GetString("FirstName"),
                        LastName = reader.GetString("LastName")
                    }
                });
            }

            return accounts;
        }

        public int SaveAccount(Account account)
        {
            using MySqlConnection conn = new MySqlConnection(_connectionString);
            conn.Open();

            string query = account.AccountId == 0 ?
                @"INSERT INTO Accounts (AccountNumber, CustomerId, StartBalance, CurrentBalance) 
          VALUES (@AccountNumber, @CustomerId, @StartBalance, @StartBalance);  -- Hier geändert
          SELECT LAST_INSERT_ID();" :
                @"UPDATE Accounts SET AccountNumber=@AccountNumber, CustomerId=@CustomerId, 
          StartBalance=@StartBalance, CurrentBalance=@CurrentBalance 
          WHERE AccountId=@AccountId";

            using MySqlCommand cmd = new MySqlCommand(query, conn);
            AddAccountParameters(cmd, account);

            if (account.AccountId == 0)
            {
                int newId = Convert.ToInt32(cmd.ExecuteScalar());

                account.CurrentBalance = account.StartBalance;
                account.AccountId = newId;

                return newId;
            }
            else
            {
                cmd.ExecuteNonQuery();
                return account.AccountId;
            }
        }

        public void DeleteAccount(int accountId)
        {
            using MySqlConnection conn = new MySqlConnection(_connectionString);
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand("DELETE FROM Accounts WHERE AccountId = @AccountId", conn);
            cmd.Parameters.AddWithValue("@AccountId", accountId);
            cmd.ExecuteNonQuery();
        }

        private void AddAccountParameters(MySqlCommand cmd, Account account)
        {
            cmd.Parameters.AddWithValue("@AccountNumber", account.AccountNumber);
            cmd.Parameters.AddWithValue("@CustomerId", account.CustomerId);
            cmd.Parameters.AddWithValue("@StartBalance", account.StartBalance);
            cmd.Parameters.AddWithValue("@CurrentBalance", account.CurrentBalance);

            if (account.AccountId > 0)
            {
                cmd.Parameters.AddWithValue("@AccountId", account.AccountId);
            }
        }

        public bool AccountNumberExists(string accountNumber, int excludeAccountId = 0)
        {
            using MySqlConnection conn = new MySqlConnection(_connectionString);
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand(
                "SELECT COUNT(*) FROM Accounts WHERE AccountNumber = @AccountNumber AND AccountId != @ExcludeAccountId",
                conn);
            cmd.Parameters.AddWithValue("@AccountNumber", accountNumber);
            cmd.Parameters.AddWithValue("@ExcludeAccountId", excludeAccountId);

            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }
        #endregion

        #region Transaction Operations
        public DataTable GetTransactionsDataTable(int accountId)
        {
            using MySqlConnection conn = new MySqlConnection(_connectionString);
            conn.Open();
            MySqlDataAdapter da = new MySqlDataAdapter(@"
                SELECT * FROM Transactions 
                WHERE AccountId = @AccountId AND IsActive = TRUE 
                ORDER BY Date DESC, TransactionId DESC", conn);
            da.SelectCommand.Parameters.AddWithValue("@AccountId", accountId);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public List<Transaction> GetTransactions(int accountId)
        {
            List<Transaction> transactions = new List<Transaction>();

            using MySqlConnection conn = new MySqlConnection(_connectionString);
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand(@"
                SELECT t.*, a.AccountNumber, c.FirstName, c.LastName
                FROM Transactions t
                INNER JOIN Accounts a ON t.AccountId = a.AccountId
                INNER JOIN Customers c ON a.CustomerId = c.CustomerId
                WHERE t.AccountId = @AccountId AND t.IsActive = TRUE 
                ORDER BY t.Date DESC, t.TransactionId DESC", conn);
            cmd.Parameters.AddWithValue("@AccountId", accountId);

            using MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                transactions.Add(new Transaction
                {
                    TransactionId = reader.GetInt32("TransactionId"),
                    AccountId = reader.GetInt32("AccountId"),
                    Date = reader.GetDateTime("Date"),
                    Amount = reader.GetDecimal("Amount"),
                    Purpose = reader.IsDBNull("Purpose") ? null : reader.GetString("Purpose"),
                    IBAN = reader.IsDBNull("IBAN") ? null : reader.GetString("IBAN"),
                    TransactionNumber = reader.IsDBNull("TransactionNumber") ? null : reader.GetString("TransactionNumber"),
                    Type = (TransactionType)Enum.Parse(typeof(TransactionType), reader.GetString("Type")),
                    IsActive = reader.GetBoolean("IsActive"),
                    Account = new Account
                    {
                        AccountNumber = reader.GetString("AccountNumber"),
                        Customer = new Customer
                        {
                            FirstName = reader.GetString("FirstName"),
                            LastName = reader.GetString("LastName")
                        }
                    }
                });
            }

            return transactions;
        }

        public int SaveTransaction(Transaction transaction)
        {
            using MySqlConnection conn = new MySqlConnection(_connectionString);
            conn.Open();

            using MySqlTransaction trans = conn.BeginTransaction();
            try
            {
                string query = transaction.TransactionId == 0 ?
                    @"INSERT INTO Transactions (AccountId, Date, Amount, Purpose, IBAN, TransactionNumber, Type, IsActive) 
                      VALUES (@AccountId, @Date, @Amount, @Purpose, @IBAN, @TransactionNumber, @Type, @IsActive);
                      SELECT LAST_INSERT_ID();" :
                    @"UPDATE Transactions SET AccountId=@AccountId, Date=@Date, Amount=@Amount, Purpose=@Purpose, 
                      IBAN=@IBAN, TransactionNumber=@TransactionNumber, Type=@Type, IsActive=@IsActive 
                      WHERE TransactionId=@TransactionId";

                using MySqlCommand cmd = new MySqlCommand(query, conn, trans);
                AddTransactionParameters(cmd, transaction);

                int transactionId;
                if (transaction.TransactionId == 0)
                {
                    transactionId = Convert.ToInt32(cmd.ExecuteScalar());

                    UpdateAccountBalance(conn, trans, transaction.AccountId);
                }
                else
                {
                    cmd.ExecuteNonQuery();
                    transactionId = transaction.TransactionId;

                    UpdateAccountBalance(conn, trans, transaction.AccountId);
                }

                trans.Commit();
                return transactionId;
            }
            catch
            {
                trans.Rollback();
                throw;
            }
        }

        public void DeleteTransaction(int transactionId)
        {
            using MySqlConnection conn = new MySqlConnection(_connectionString);
            conn.Open();

            using MySqlTransaction trans = conn.BeginTransaction();
            try
            {
                using MySqlCommand cmd = new MySqlCommand(
                    "UPDATE Transactions SET IsActive = FALSE WHERE TransactionId = @TransactionId",
                    conn, trans);
                cmd.Parameters.AddWithValue("@TransactionId", transactionId);
                cmd.ExecuteNonQuery();

                trans.Commit();
            }
            catch
            {
                trans.Rollback();
                throw;
            }
        }

        private void AddTransactionParameters(MySqlCommand cmd, Transaction transaction)
        {
            cmd.Parameters.AddWithValue("@AccountId", transaction.AccountId);
            cmd.Parameters.AddWithValue("@Date", transaction.Date);
            cmd.Parameters.AddWithValue("@Amount", transaction.Amount);
            cmd.Parameters.AddWithValue("@Purpose", transaction.Purpose ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@IBAN", transaction.IBAN ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@TransactionNumber", transaction.TransactionNumber ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Type", transaction.Type.ToString());
            cmd.Parameters.AddWithValue("@IsActive", transaction.IsActive);

            if (transaction.TransactionId > 0)
            {
                cmd.Parameters.AddWithValue("@TransactionId", transaction.TransactionId);
            }
        }

        private void UpdateAccountBalance(MySqlConnection conn, MySqlTransaction trans, int accountId)
        {
            using MySqlCommand cmd = new MySqlCommand(@"
                UPDATE Accounts 
                SET CurrentBalance = StartBalance + COALESCE(
                    (SELECT SUM(Amount) FROM Transactions WHERE AccountId = @AccountId AND IsActive = TRUE), 0)
                WHERE AccountId = @AccountId", conn, trans);
            cmd.Parameters.AddWithValue("@AccountId", accountId);
            cmd.ExecuteNonQuery();
        }

        public decimal GetAccountBalance(int accountId)
        {
            using MySqlConnection conn = new MySqlConnection(_connectionString);
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand(
                "SELECT CurrentBalance FROM Accounts WHERE AccountId = @AccountId", conn);
            cmd.Parameters.AddWithValue("@AccountId", accountId);

            object result = cmd.ExecuteScalar();
            return result == DBNull.Value ? 0 : Convert.ToDecimal(result);
        }

        public bool TransactionNumberExists(string transactionNumber, int excludeTransactionId = 0)
        {
            using MySqlConnection conn = new MySqlConnection(_connectionString);
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand(
                "SELECT COUNT(*) FROM Transactions WHERE TransactionNumber = @TransactionNumber AND TransactionId != @ExcludeTransactionId",
                conn);
            cmd.Parameters.AddWithValue("@TransactionNumber", transactionNumber);
            cmd.Parameters.AddWithValue("@ExcludeTransactionId", excludeTransactionId);

            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }
        #endregion

        #region Utility Methods
        public bool TestConnection()
        {
            try
            {
                using MySqlConnection conn = new MySqlConnection(_connectionString);
                conn.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}