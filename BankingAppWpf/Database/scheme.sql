CREATE TABLE Customers (
    CustomerId INT PRIMARY KEY AUTO_INCREMENT,
    FirstName VARCHAR(50),
    LastName VARCHAR(50),
    Street VARCHAR(100),
    HouseNumber VARCHAR(10),
    PostalCode VARCHAR(10),
    City VARCHAR(50),
    Phone VARCHAR(20),
    Email VARCHAR(100)
);

CREATE TABLE Accounts (
    AccountId INT PRIMARY KEY AUTO_INCREMENT,
    AccountNumber VARCHAR(20) UNIQUE,
    CustomerId INT,
    StartBalance DECIMAL(18,2),
    CurrentBalance DECIMAL(18,2),
    FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId)
);

CREATE TABLE Transactions (
    TransactionId INT PRIMARY KEY AUTO_INCREMENT,
    AccountId INT,
    Date DATETIME,
    Amount DECIMAL(18,2),
    Purpose VARCHAR(200),
    IBAN VARCHAR(34),
    TransactionNumber VARCHAR(50),
    Type ENUM('Withdrawal','Deposit','Transfer','Incoming'),
    IsActive BOOLEAN DEFAULT TRUE,
    FOREIGN KEY (AccountId) REFERENCES Accounts(AccountId)
);