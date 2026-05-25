-- Переключаемся на базу (создайте её заранее или раскомментируйте CREATE DATABASE ниже)
-- CREATE DATABASE FootwearShopDB;
USE FootwearShopDB;
GO

-- ==================== Справочники (без внешних ключей) ====================

CREATE TABLE Addresses (
    AddressID INT IDENTITY(1,1) PRIMARY KEY,
    AddressName NVARCHAR(MAX) NOT NULL
);
GO

CREATE TABLE Categories (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(150) NOT NULL
);
GO

CREATE TABLE Manufacturers (
    ManufacturerID INT IDENTITY(1,1) PRIMARY KEY,
    ManufacturerName NVARCHAR(300) NOT NULL
);
GO

CREATE TABLE OrderStatuses (
    StatusID INT IDENTITY(1,1) PRIMARY KEY,
    StatusName NVARCHAR(150) NOT NULL
);
GO

CREATE TABLE ProductNames (
    ProductNameID INT IDENTITY(1,1) PRIMARY KEY,
    ProductName NVARCHAR(150) NOT NULL
);
GO

CREATE TABLE Providers (
    ProviderID INT IDENTITY(1,1) PRIMARY KEY,
    ProviderName NVARCHAR(200) NOT NULL
);
GO

CREATE TABLE Roles (
    RoleID INT IDENTITY(1,1) PRIMARY KEY,
    RoleName NVARCHAR(150) NOT NULL
);
GO

-- ==================== Таблицы с зависимостями ====================

CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(MAX) NOT NULL,
    [Login] NVARCHAR(150) NOT NULL UNIQUE,
    [Password] NVARCHAR(MAX) NOT NULL,
    RoleID INT NOT NULL,
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleID) REFERENCES Roles(RoleID)
);
GO

CREATE TABLE Products (
    ProductID INT IDENTITY(1,1) PRIMARY KEY,
    Article NVARCHAR(250) NOT NULL,
    ProductNameID INT NOT NULL,
    [Unit] NVARCHAR(50) NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    ProviderID INT NOT NULL,
    ManufacturerID INT NOT NULL,
    CategoryID INT NOT NULL,
    Discount INT NULL,
    QuantityInStock INT NOT NULL DEFAULT 0,  -- Исправлено: было QuantityInStok
    [Description] NVARCHAR(MAX) NULL,
    PhotoFileName NVARCHAR(300) NULL,
    CONSTRAINT FK_Products_ProductNames FOREIGN KEY (ProductNameID) REFERENCES ProductNames(ProductNameID),
    CONSTRAINT FK_Products_Providers FOREIGN KEY (ProviderID) REFERENCES Providers(ProviderID),
    CONSTRAINT FK_Products_Manufacturers FOREIGN KEY (ManufacturerID) REFERENCES Manufacturers(ManufacturerID),
    CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID)
);
GO

CREATE TABLE Orders (
    OrderID INT IDENTITY(1,1) PRIMARY KEY,
    OrderDate DATE NOT NULL DEFAULT GETDATE(),
    DeliveryDate DATE NOT NULL,
    AddressID INT NOT NULL,
    UserID INT NOT NULL,
    ReceiptCode INT NULL,
    StatusID INT NOT NULL,
    CONSTRAINT FK_Orders_Addresses FOREIGN KEY (AddressID) REFERENCES Addresses(AddressID),
    CONSTRAINT FK_Orders_Users FOREIGN KEY (UserID) REFERENCES Users(UserID),
    CONSTRAINT FK_Orders_OrderStatuses FOREIGN KEY (StatusID) REFERENCES OrderStatuses(StatusID)
);
GO

CREATE TABLE OrderItems (
    OrderItemID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID INT NOT NULL,
    ProductID INT NOT NULL,
    Quantity INT NOT NULL,
    CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderID) REFERENCES Orders(OrderID),
    CONSTRAINT FK_OrderItems_Products FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);
GO