/* =========================================================
   01_CreateTables.sql
   Aligned 1:1 with current Undy schema and business rules
   - NO Pending status (handled by data / logic, not constraint)
   - Identity-based numbers (CustomerNumber, SalesOrderNumber, WholesaleOrderNumber)
   ========================================================= */

SET NOCOUNT ON;
GO

/* =========================================================
   DROP TABLES (FK-safe order)
   ========================================================= */
IF OBJECT_ID('dbo.ProductSalesOrder', 'U') IS NOT NULL DROP TABLE dbo.ProductSalesOrder;
IF OBJECT_ID('dbo.ProductWholesaleOrder', 'U') IS NOT NULL DROP TABLE dbo.ProductWholesaleOrder;
IF OBJECT_ID('dbo.ReturnOrder', 'U') IS NOT NULL DROP TABLE dbo.ReturnOrder;
IF OBJECT_ID('dbo.SalesOrder', 'U') IS NOT NULL DROP TABLE dbo.SalesOrder;
IF OBJECT_ID('dbo.WholesaleOrder', 'U') IS NOT NULL DROP TABLE dbo.WholesaleOrder;
IF OBJECT_ID('dbo.Customers', 'U') IS NOT NULL DROP TABLE dbo.Customers;
IF OBJECT_ID('dbo.Product', 'U') IS NOT NULL DROP TABLE dbo.Product;
GO

/* =========================================================
   TABLE: Product
   ========================================================= */
CREATE TABLE dbo.Product
(
    ProductID        UNIQUEIDENTIFIER NOT NULL,
    ProductNumber    NVARCHAR(255) NOT NULL,
    ProductName      NVARCHAR(255) NOT NULL,
    Price            DECIMAL(10,2) NOT NULL,
    Size             NVARCHAR(255) NOT NULL,
    Colour           NVARCHAR(255) NOT NULL,
    NumberInStock    INT NOT NULL
        CONSTRAINT DF_Product_NumberInStock DEFAULT (0),

    CONSTRAINT PK_Product PRIMARY KEY CLUSTERED (ProductID),
    CONSTRAINT UQ_Product_ProductNumber UNIQUE (ProductNumber)
);
GO

/* =========================================================
   TABLE: Customers
   ========================================================= */
CREATE TABLE dbo.Customers
(
    CustomerID            UNIQUEIDENTIFIER NOT NULL,
    CustomerNumber        INT IDENTITY(1,1) NOT NULL,
    DisplayCustomerNumber AS
        ('KUND-' + RIGHT('000000000' + CONVERT(NVARCHAR(10), CustomerNumber), 10)) PERSISTED,
    FirstName             NVARCHAR(255) NOT NULL,
    LastName              NVARCHAR(255) NOT NULL,
    Email                 NVARCHAR(255) NOT NULL,
    PhoneNumber           INT NOT NULL,
    [Address]             NVARCHAR(255) NOT NULL,
    City                  NVARCHAR(100) NOT NULL,
    PostalCode            INT NOT NULL,

    CONSTRAINT PK_Customers PRIMARY KEY CLUSTERED (CustomerID),
    CONSTRAINT UQ_Customers_CustomerNumber UNIQUE (CustomerNumber),
    CONSTRAINT UQ_Customers_Email UNIQUE (Email),
    CONSTRAINT UQ_Customers_PhoneNumber UNIQUE (PhoneNumber)
);
GO

/* =========================================================
   TABLE: SalesOrder
   ========================================================= */
CREATE TABLE dbo.SalesOrder
(
    SalesOrderID            UNIQUEIDENTIFIER NOT NULL,
    CustomerID              UNIQUEIDENTIFIER NOT NULL,
    SalesOrderNumber        INT IDENTITY(1,1) NOT NULL,
    DisplaySalesOrderNumber AS
        ('SALG-' + RIGHT('000000000' + CONVERT(NVARCHAR(10), SalesOrderNumber), 10)) PERSISTED,
    OrderStatus             NVARCHAR(255) NOT NULL,
    PaymentStatus           NVARCHAR(255) NOT NULL,
    SalesDate               DATE NOT NULL,
    TotalPrice              DECIMAL(10,2) NOT NULL
        CONSTRAINT DF_SalesOrder_TotalPrice DEFAULT (0),
    ShippedDate             DATE NULL,

    CONSTRAINT PK_SalesOrder PRIMARY KEY CLUSTERED (SalesOrderID),
    CONSTRAINT UQ_SalesOrder_SalesOrderNumber UNIQUE (SalesOrderNumber),
    CONSTRAINT FK_Customer_SalesOrder
        FOREIGN KEY (CustomerID) REFERENCES dbo.Customers(CustomerID)
);
GO

/* =========================================================
   TABLE: WholesaleOrder
   ========================================================= */
CREATE TABLE dbo.WholesaleOrder
(
    WholesaleOrderID            UNIQUEIDENTIFIER NOT NULL,
    WholesaleOrderNumber        INT IDENTITY(1,1) NOT NULL,
    DisplayWholesaleOrderNumber AS
        ('KØB-' + RIGHT('000000000' + CONVERT(NVARCHAR(10), WholesaleOrderNumber), 10)) PERSISTED,
    WholesaleOrderDate          DATE NOT NULL,
    ExpectedDeliveryDate        DATE NOT NULL,
    DeliveryDate                DATE NULL,
    OrderStatus                 NVARCHAR(255) NOT NULL,

    CONSTRAINT PK_WholesaleOrder PRIMARY KEY CLUSTERED (WholesaleOrderID),
    CONSTRAINT UQ_WholesaleOrder_WholesaleOrderNumber UNIQUE (WholesaleOrderNumber)
);
GO

/* =========================================================
   TABLE: ProductSalesOrder (link)
   ========================================================= */
CREATE TABLE dbo.ProductSalesOrder
(
    SalesOrderID UNIQUEIDENTIFIER NOT NULL,
    ProductID    UNIQUEIDENTIFIER NOT NULL,
    Quantity     INT NOT NULL,
    UnitPrice    DECIMAL(10,2) NOT NULL,

    CONSTRAINT PK_ProductSalesOrder
        PRIMARY KEY CLUSTERED (SalesOrderID, ProductID),
    CONSTRAINT FK_ProductSalesOrder_SalesOrder
        FOREIGN KEY (SalesOrderID) REFERENCES dbo.SalesOrder(SalesOrderID),
    CONSTRAINT FK_ProductSalesOrder_Product
        FOREIGN KEY (ProductID) REFERENCES dbo.Product(ProductID)
);
GO

/* =========================================================
   TABLE: ProductWholesaleOrder (link)
   ========================================================= */
CREATE TABLE dbo.ProductWholesaleOrder
(
    WholesaleOrderID UNIQUEIDENTIFIER NOT NULL,
    ProductID        UNIQUEIDENTIFIER NOT NULL,
    Quantity         INT NOT NULL,
    UnitPrice        DECIMAL(10,2) NOT NULL,
    QuantityReceived INT NOT NULL
        CONSTRAINT DF_ProductWholesaleOrder_QuantityReceived DEFAULT (0),

    CONSTRAINT PK_ProductWholesaleOrder
        PRIMARY KEY CLUSTERED (WholesaleOrderID, ProductID),
    CONSTRAINT FK_ProductWholesaleOrder_WholesaleOrder
        FOREIGN KEY (WholesaleOrderID) REFERENCES dbo.WholesaleOrder(WholesaleOrderID),
    CONSTRAINT FK_ProductWholesaleOrder_Product
        FOREIGN KEY (ProductID) REFERENCES dbo.Product(ProductID)
);
GO

/* =========================================================
   TABLE: ReturnOrder
   ========================================================= */
CREATE TABLE dbo.ReturnOrder
(
    ReturnOrderID    UNIQUEIDENTIFIER NOT NULL,
    ReturnOrderDate  DATE NOT NULL,
    SalesOrderID     UNIQUEIDENTIFIER NOT NULL,
    ReturnTotalPrice DECIMAL(10,2) NOT NULL
        CONSTRAINT DF_ReturnOrder_ReturnTotalPrice DEFAULT (0),

    CONSTRAINT PK_ReturnOrder PRIMARY KEY CLUSTERED (ReturnOrderID),
    CONSTRAINT FK_ReturnOrder_SalesOrder
        FOREIGN KEY (SalesOrderID) REFERENCES dbo.SalesOrder(SalesOrderID)
);
GO
