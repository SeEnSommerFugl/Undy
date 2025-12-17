USE Undy;
GO

-- ============================================================================
-- TABLES
-- ============================================================================

-- Product Table
CREATE TABLE [Product](
	ProductID UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	ProductNumber NVARCHAR(255) NOT NULL UNIQUE,
	ProductName NVARCHAR(255) NOT NULL,
	Price DECIMAL(10,2) NOT NULL,
	Size NVARCHAR(255) NOT NULL,
	Colour NVARCHAR(255) NOT NULL,
	NumberInStock INT DEFAULT 0 NOT NULL
);

-- WholesaleOrder Table
CREATE TABLE WholesaleOrder(
	WholesaleOrderID UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	WholesaleOrderNumber INT IDENTITY(1,1) NOT NULL UNIQUE,
	DisplayWholesaleOrderNumber AS 'KØB-' + RIGHT(CAST(WholesaleOrderNumber AS NVARCHAR(10)), 10) PERSISTED,
	WholesaleOrderDate DATE NOT NULL,
	ExpectedDeliveryDate DATE NOT NULL,
	DeliveryDate DATE NULL,
	OrderStatus NVARCHAR(255) NOT NULL
);

-- ProductWholesaleOrder
CREATE TABLE ProductWholesaleOrder(
	WholesaleOrderID UNIQUEIDENTIFIER NOT NULL,
	ProductID UNIQUEIDENTIFIER NOT NULL,
	Quantity INT NOT NULL,
	UnitPrice DECIMAL(10,2) NOT NULL,
	QuantityReceived INT NOT NULL DEFAULT 0,
	CONSTRAINT PK_ProductWholesaleOrder PRIMARY KEY(WholesaleOrderID, ProductID),
	CONSTRAINT FK_WholesaleOrder_ProductWholesaleOrder
		FOREIGN KEY(WholesaleOrderID)
		REFERENCES WholesaleOrder(WholesaleOrderID),
	CONSTRAINT FK_Product_ProductWholesaleOrder
		FOREIGN KEY(ProductID)
		REFERENCES [Product](ProductID)
);

-- Customers Table
CREATE TABLE Customers(
	CustomerID UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	CustomerNumber INT IDENTITY(1,1) NOT NULL UNIQUE,
	DisplayCustomerNumber AS 'KUND-' + RIGHT(CAST(CustomerNumber AS NVARCHAR(10)), 10) PERSISTED,
	FirstName NVARCHAR(255) NOT NULL,
	LastName NVARCHAR(255) NOT NULL,
	Email NVARCHAR(255) NOT NULL UNIQUE,
	PhoneNumber INT NOT NULL UNIQUE,
	[Address] NVARCHAR(255) NOT NULL,
	City NVARCHAR(100) NOT NULL,
	PostalCode INT NOT NULL
);

-- SalesOrder Table
CREATE TABLE SalesOrder (
	SalesOrderID UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	CustomerID UNIQUEIDENTIFIER NOT NULL,
	SalesOrderNumber INT IDENTITY(1,1) NOT NULL UNIQUE,
	DisplaySalesOrderNumber AS 'SALG-' + RIGHT(CAST(SalesOrderNumber AS NVARCHAR(10)), 10) PERSISTED,
	OrderStatus NVARCHAR(255) NOT NULL,
	PaymentStatus NVARCHAR(255) NOT NULL,
	SalesDate DATE NOT NULL,
	ShippedDate DATE NULL,
	TotalPrice DECIMAL(10,2) NOT NULL DEFAULT 0,
	CONSTRAINT FK_Customer_SalesOrder
		FOREIGN KEY(CustomerID)
		REFERENCES Customers(CustomerID)
);

-- ProductSalesOrder
CREATE TABLE ProductSalesOrder (
	SalesOrderID UNIQUEIDENTIFIER NOT NULL,
	ProductID UNIQUEIDENTIFIER NOT NULL,
	Quantity INT NOT NULL,
	UnitPrice DECIMAL(10,2) NOT NULL,
	CONSTRAINT PK_ProductSalesOrder PRIMARY KEY(SalesOrderID, ProductID),
	CONSTRAINT FK_SalesOrder_ProductSalesOrder
		FOREIGN KEY(SalesOrderID)
		REFERENCES SalesOrder(SalesOrderID),
	CONSTRAINT FK_Product_ProductSalesOrder
		FOREIGN KEY(ProductID)
		REFERENCES [Product](ProductID)
);

-- ReturnOrder Table
CREATE TABLE ReturnOrder(
	ReturnOrderID UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	ReturnOrderDate DATE NOT NULL,
	SalesOrderID UNIQUEIDENTIFIER NOT NULL,
    ReturnTotalPrice DECIMAL(10,2) NOT NULL DEFAULT 0,
	CONSTRAINT FK_SalesOrder_ReturnOrder
		FOREIGN KEY(SalesOrderID)
		REFERENCES SalesOrder(SalesOrderID)
);

GO
