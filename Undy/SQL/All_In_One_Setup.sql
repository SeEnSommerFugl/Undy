/* =========================================================
   00_Rebuild_Undy_Objects_And_Data.sql

   Does NOT drop the database.
   Drops tables/views/procs (inside Undy), recreates tables,
   inserts dummy data, recreates views + procedures, creates users.

   Run in SSMS (normal mode), as a single script.
   ========================================================= */

SET NOCOUNT ON;
GO

/* ============ 0) Ensure DB exists ============ */
IF DB_ID(N'Undy') IS NULL
BEGIN
    EXEC(N'CREATE DATABASE Undy;');
END
GO

USE Undy;
GO

/* ============ 1) Drop views/procs/fks/tables (but keep DB) ============ */
PRINT 'Dropping views...';
DECLARE @sql nvarchar(max) = N'';
SELECT @sql += N'DROP VIEW ' + QUOTENAME(SCHEMA_NAME(schema_id)) + N'.' + QUOTENAME(name) + N';' + CHAR(10)
FROM sys.views;
EXEC sp_executesql @sql;
GO

PRINT 'Dropping procedures...';
DECLARE @sql2 nvarchar(max) = N'';
SELECT @sql2 += N'DROP PROCEDURE ' + QUOTENAME(SCHEMA_NAME(schema_id)) + N'.' + QUOTENAME(name) + N';' + CHAR(10)
FROM sys.procedures;
EXEC sp_executesql @sql2;
GO

PRINT 'Dropping foreign keys...';
DECLARE @sql3 nvarchar(max) = N'';
SELECT @sql3 += N'ALTER TABLE ' + QUOTENAME(SCHEMA_NAME(t.schema_id)) + N'.' + QUOTENAME(t.name)
            + N' DROP CONSTRAINT ' + QUOTENAME(fk.name) + N';' + CHAR(10)
FROM sys.foreign_keys fk
JOIN sys.tables t ON t.object_id = fk.parent_object_id;
EXEC sp_executesql @sql3;
GO

PRINT 'Dropping tables...';
DECLARE @sql4 nvarchar(max) = N'';
SELECT @sql4 += N'DROP TABLE ' + QUOTENAME(SCHEMA_NAME(schema_id)) + N'.' + QUOTENAME(name) + N';' + CHAR(10)
FROM sys.tables;
EXEC sp_executesql @sql4;
GO

/* ============ 2) CREATE TABLES ============ */
/* Paste your current 01_CreateTables.sql content here */
-- >>> BEGIN 01_CreateTables.sql
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

-- <<< END 01_CreateTables.sql
GO

/* ============ 3) INSERT DUMMY DATA ============ */
/* Paste your current DummyDATA.sql content here */
-- >>> BEGIN DummyDATA.sql
/* =========================================================
   DummyDATA.sql (FIXED: single-batch variables)
   - Removes batch breaks that killed @Today / table variables.
   - Wholesale statuses aligned to rules:
     Afventer / Klar til udpakning / Delvist modtaget / Modtaget
   ========================================================= */

SET NOCOUNT ON;

DECLARE @Today date = CAST(GETDATE() AS date);

/* ---------- Clean existing rows (FK-safe) ---------- */
DELETE FROM dbo.ProductSalesOrder;
DELETE FROM dbo.ProductWholesaleOrder;
DELETE FROM dbo.ReturnOrder;
DELETE FROM dbo.SalesOrder;
DELETE FROM dbo.WholesaleOrder;
DELETE FROM dbo.Customers;
DELETE FROM dbo.Product;

/* ---------- Products (8) ---------- */
INSERT INTO dbo.Product(ProductID, ProductNumber, ProductName, Price, Size, Colour, NumberInStock)
VALUES
(NEWID(), N'UBBABL1-S',     N'Bambus Boxerbriefs', 139.00, N'Small',  N'Sort', 100),
(NEWID(), N'UBBARE1-M',     N'Bambus Boxerbriefs', 139.00, N'Medium', N'Rød',  33),
(NEWID(), N'UBBABL1-L',     N'Bambus Boxerbriefs', 139.00, N'Large',  N'Sort', 751),
(NEWID(), N'UBBARE1-XXL',   N'Bambus Boxerbriefs', 139.00, N'XXL',    N'Rød',  5),
(NEWID(), N'TSPLSS1-S',     N'T-Shirt',            199.00, N'Small',  N'Sort', 100),
(NEWID(), N'TSPLSM1-M',     N'T-Shirt',            199.00, N'Medium', N'Rød',  33),
(NEWID(), N'TSPLSL1-L',     N'T-Shirt',            199.00, N'Large',  N'Sort', 751),
(NEWID(), N'TSPLSXXL1-XXL', N'T-Shirt',            199.00, N'XXL',    N'Rød',  5);

/* ---------- Customers (20) ---------- */
INSERT INTO dbo.Customers(CustomerID, FirstName, LastName, Email, PhoneNumber, [Address], City, PostalCode)
VALUES
(NEWID(), N'John',   N'Doe',        N'john.doe@email.com',         12345678, N'Spurvevej 3',      N'Spurveby',   2000),
(NEWID(), N'Emma',   N'Nielsen',    N'emma.nielsen@email.com',    22334455, N'Birkevej 12',      N'København',  2100),
(NEWID(), N'Lucas',  N'Jensen',     N'lucas.jensen@email.com',    33445566, N'Møllevej 7',       N'Roskilde',   4000),
(NEWID(), N'Sofia',  N'Hansen',     N'sofia.hansen@email.com',    44556677, N'Skovvej 19',       N'Odense',     5000),
(NEWID(), N'Noah',   N'Pedersen',   N'noah.pedersen@email.com',   55667788, N'Fjordvej 2',       N'Aalborg',    9000),
(NEWID(), N'Freja',  N'Kristensen', N'freja.kristensen@email.com',66778899, N'Engvej 44',        N'Aarhus',     8000),
(NEWID(), N'Oliver', N'Madsen',     N'oliver.madsen@email.com',   77889900, N'Bakkevej 5',       N'Esbjerg',    6700),
(NEWID(), N'Alma',   N'Larsen',     N'alma.larsen@email.com',     88990011, N'Strandvej 101',    N'Helsingør',  3000),
(NEWID(), N'William',N'Sørensen',   N'william.sorensen@email.com',99001122, N'Parkvej 8',        N'Kolding',    6000),
(NEWID(), N'Clara',  N'Rasmussen',  N'clara.rasmussen@email.com', 11223344, N'Torvegade 3',      N'Vejle',      7100),
(NEWID(), N'Elias',  N'Johansen',   N'elias.johansen@email.com',  12121212, N'Nørregade 22',     N'Randers',    8900),
(NEWID(), N'Ida',    N'Andersen',   N'ida.andersen@email.com',    13131313, N'Søndergade 14',    N'Silkeborg',  8600),
(NEWID(), N'Arthur', N'Berg',       N'arthur.berg@email.com',     14141414, N'Vestervej 9',      N'Herning',    7400),
(NEWID(), N'Agnes',  N'Holm',       N'agnes.holm@email.com',      15151515, N'Lyngvej 6',        N'Hillerød',   3400),
(NEWID(), N'Oskar',  N'Dahl',       N'oskar.dahl@email.com',      16161616, N'Granvej 11',       N'Holbæk',     4300),
(NEWID(), N'Maja',   N'Krogh',      N'maja.krogh@email.com',      17171717, N'Elmevej 1',        N'Fredericia', 7000),
(NEWID(), N'Viggo',  N'Lund',       N'viggo.lund@email.com',      18181818, N'Bredgade 40',      N'Svendborg',  5700),
(NEWID(), N'Sara',   N'Mortensen',  N'sara.mortensen@email.com',  19191919, N'Åboulevard 5',     N'København',  1635),
(NEWID(), N'Malthe', N'Thomsen',    N'malthe.thomsen@email.com',  20202020, N'Østergade 77',     N'Ribe',       6760),
(NEWID(), N'Julie',  N'Poulsen',    N'julie.poulsen@email.com',   21212121, N'Havnevej 9',       N'Greve',      2670);

/* ---------- SalesOrders (20) ---------- */
DECLARE @CustomerIds TABLE (CustomerID UNIQUEIDENTIFIER NOT NULL);
INSERT INTO @CustomerIds(CustomerID) SELECT CustomerID FROM dbo.Customers;

DECLARE @SalesOrders TABLE (SalesOrderID UNIQUEIDENTIFIER NOT NULL);

DECLARE @s INT = 0;
WHILE (@s < 20)
BEGIN
    DECLARE @sid UNIQUEIDENTIFIER = NEWID();
    DECLARE @cid UNIQUEIDENTIFIER = (SELECT TOP 1 CustomerID FROM @CustomerIds ORDER BY NEWID());
    DECLARE @status NVARCHAR(255) =
        CASE (ABS(CHECKSUM(NEWID())) % 3)
            WHEN 0 THEN N'Afventer'
            WHEN 1 THEN N'Afsendt'
            ELSE N'Færdig'
        END;
    DECLARE @pay NVARCHAR(255) =
        CASE (ABS(CHECKSUM(NEWID())) % 3)
            WHEN 0 THEN N'Afventer'
            WHEN 1 THEN N'Betalt'
            ELSE N'Afventer Betaling'
        END;
    DECLARE @salesDate date = DATEADD(DAY, -(ABS(CHECKSUM(NEWID())) % 30), @Today);

    INSERT INTO dbo.SalesOrder(SalesOrderID, CustomerID, OrderStatus, PaymentStatus, SalesDate, TotalPrice, ShippedDate)
    VALUES(@sid, @cid, @status, @pay, @salesDate, 0.00,
           CASE WHEN @status = N'Afsendt' THEN @Today ELSE NULL END);

    INSERT INTO @SalesOrders(SalesOrderID) VALUES(@sid);

    SET @s += 1;
END

/* ---------- ProductSalesOrder lines (20) ---------- */
DECLARE @Products TABLE (ProductID UNIQUEIDENTIFIER NOT NULL, Price DECIMAL(10,2) NOT NULL);
INSERT INTO @Products(ProductID, Price) SELECT ProductID, Price FROM dbo.Product;

DECLARE @Sales TABLE (SalesOrderID UNIQUEIDENTIFIER NOT NULL);
INSERT INTO @Sales SELECT SalesOrderID FROM dbo.SalesOrder;

DECLARE @l INT = 0;
WHILE (@l < 20)
BEGIN
    DECLARE @sid2 UNIQUEIDENTIFIER = (SELECT TOP 1 SalesOrderID FROM @Sales ORDER BY NEWID());
    DECLARE @pid UNIQUEIDENTIFIER = (SELECT TOP 1 ProductID FROM @Products ORDER BY NEWID());
    DECLARE @unit DECIMAL(10,2) = (SELECT Price FROM @Products WHERE ProductID = @pid);
    DECLARE @qty INT = 1 + (ABS(CHECKSUM(NEWID())) % 120);

    IF NOT EXISTS (SELECT 1 FROM dbo.ProductSalesOrder WHERE SalesOrderID=@sid2 AND ProductID=@pid)
    BEGIN
        INSERT INTO dbo.ProductSalesOrder(SalesOrderID, ProductID, Quantity, UnitPrice)
        VALUES(@sid2, @pid, @qty, @unit);

        SET @l += 1;
    END
END

/* Recalculate SalesOrder totals from lines */
UPDATE so
SET so.TotalPrice = x.SumLineTotal
FROM dbo.SalesOrder so
JOIN (
    SELECT SalesOrderID, SUM(Quantity * UnitPrice) AS SumLineTotal
    FROM dbo.ProductSalesOrder
    GROUP BY SalesOrderID
) x ON x.SalesOrderID = so.SalesOrderID;

/* ---------- WholesaleOrders (20) ---------- */
DECLARE @WO TABLE
(
    WholesaleOrderID UNIQUEIDENTIFIER NOT NULL,
    WholesaleOrderDate DATE NOT NULL,
    ExpectedDeliveryDate DATE NOT NULL,
    DeliveryDate DATE NULL,
    OrderStatus NVARCHAR(255) NOT NULL
);

-- 8 future = Afventer
;WITH n AS (SELECT TOP (8) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS i FROM sys.all_objects)
INSERT INTO @WO
SELECT
    NEWID(),
    DATEADD(DAY, - (ABS(CHECKSUM(NEWID())) % 10), @Today),
    DATEADD(DAY, 1 + (ABS(CHECKSUM(NEWID())) % 14), @Today),
    NULL,
    N'Afventer'
FROM n;

-- 4 past = Klar til udpakning (expected <= today, all received = 0)
;WITH n AS (SELECT TOP (4) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS i FROM sys.all_objects)
INSERT INTO @WO
SELECT
    NEWID(),
    DATEADD(DAY, - (10 + (ABS(CHECKSUM(NEWID())) % 20)), @Today),
    DATEADD(DAY, - (ABS(CHECKSUM(NEWID())) % 5), @Today),
    NULL,
    N'Klar til udpakning'
FROM n;

-- 4 past = Delvist modtaget
;WITH n AS (SELECT TOP (4) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS i FROM sys.all_objects)
INSERT INTO @WO
SELECT
    NEWID(),
    DATEADD(DAY, - (10 + (ABS(CHECKSUM(NEWID())) % 20)), @Today),
    DATEADD(DAY, - (ABS(CHECKSUM(NEWID())) % 5), @Today),
    NULL,
    N'Delvist modtaget'
FROM n;

-- 4 past = Modtaget
;WITH n AS (SELECT TOP (4) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS i FROM sys.all_objects)
INSERT INTO @WO
SELECT
    NEWID(),
    DATEADD(DAY, - (10 + (ABS(CHECKSUM(NEWID())) % 20)), @Today),
    DATEADD(DAY, - (ABS(CHECKSUM(NEWID())) % 5), @Today),
    DATEADD(DAY, - (ABS(CHECKSUM(NEWID())) % 3), @Today),
    N'Modtaget'
FROM n;

INSERT INTO dbo.WholesaleOrder(WholesaleOrderID, WholesaleOrderDate, ExpectedDeliveryDate, DeliveryDate, OrderStatus)
SELECT WholesaleOrderID, WholesaleOrderDate, ExpectedDeliveryDate, DeliveryDate, OrderStatus
FROM @WO;

/* ---------- ProductWholesaleOrder lines (status rules hold) ---------- */
DECLARE @WIds TABLE (WholesaleOrderID UNIQUEIDENTIFIER NOT NULL, Status NVARCHAR(255) NOT NULL);
INSERT INTO @WIds
SELECT WholesaleOrderID, OrderStatus
FROM dbo.WholesaleOrder;

DECLARE @PIds TABLE (ProductID UNIQUEIDENTIFIER NOT NULL, Price DECIMAL(10,2) NOT NULL);
INSERT INTO @PIds SELECT ProductID, Price FROM dbo.Product;

DECLARE wcur CURSOR FAST_FORWARD FOR
SELECT WholesaleOrderID, Status FROM @WIds;

OPEN wcur;

DECLARE @wid UNIQUEIDENTIFIER;
DECLARE @wstatus NVARCHAR(255);

FETCH NEXT FROM wcur INTO @wid, @wstatus;

WHILE @@FETCH_STATUS = 0
BEGIN
    DECLARE @lines INT = 1 + (ABS(CHECKSUM(NEWID())) % 3);
    DECLARE @added INT = 0;

    WHILE (@added < @lines)
    BEGIN
        DECLARE @pid2 UNIQUEIDENTIFIER = (SELECT TOP 1 ProductID FROM @PIds ORDER BY NEWID());
        DECLARE @unit2 DECIMAL(10,2) = (SELECT Price FROM @PIds WHERE ProductID = @pid2);
        DECLARE @qty2 INT = 1 + (ABS(CHECKSUM(NEWID())) % 200);

        IF NOT EXISTS (SELECT 1 FROM dbo.ProductWholesaleOrder WHERE WholesaleOrderID=@wid AND ProductID=@pid2)
        BEGIN
            DECLARE @rec2 INT;

            IF (@wstatus = N'Afventer')
                SET @rec2 = 0;
            ELSE IF (@wstatus = N'Klar til udpakning')
                SET @rec2 = 0;
            ELSE IF (@wstatus = N'Modtaget')
                SET @rec2 = @qty2;
            ELSE IF (@wstatus = N'Delvist modtaget')
                SET @rec2 = CASE WHEN @qty2 = 1 THEN 0 ELSE 1 + (ABS(CHECKSUM(NEWID())) % (@qty2 - 1)) END;
            ELSE
                SET @rec2 = 0;

            INSERT INTO dbo.ProductWholesaleOrder(WholesaleOrderID, ProductID, Quantity, UnitPrice, QuantityReceived)
            VALUES(@wid, @pid2, @qty2, @unit2, @rec2);

            SET @added += 1;
        END
    END

    -- Enforce Delvist modtaget invariant
    IF (@wstatus = N'Delvist modtaget')
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM dbo.ProductWholesaleOrder WHERE WholesaleOrderID=@wid AND QuantityReceived > 0)
        BEGIN
            UPDATE TOP (1) dbo.ProductWholesaleOrder
            SET QuantityReceived = 1
            WHERE WholesaleOrderID=@wid AND Quantity > 1;
        END

        IF NOT EXISTS (SELECT 1 FROM dbo.ProductWholesaleOrder WHERE WholesaleOrderID=@wid AND QuantityReceived < Quantity)
        BEGIN
            UPDATE TOP (1) dbo.ProductWholesaleOrder
            SET QuantityReceived = CASE WHEN Quantity > 1 THEN Quantity - 1 ELSE 0 END
            WHERE WholesaleOrderID=@wid;
        END
    END

    -- Enforce Modtaget invariant + DeliveryDate
    IF (@wstatus = N'Modtaget')
    BEGIN
        UPDATE dbo.ProductWholesaleOrder
        SET QuantityReceived = Quantity
        WHERE WholesaleOrderID=@wid;

        UPDATE dbo.WholesaleOrder
        SET DeliveryDate = ISNULL(DeliveryDate, @Today)
        WHERE WholesaleOrderID=@wid;
    END

    -- Enforce Klar til udpakning & Afventer invariants
    IF (@wstatus IN (N'Klar til udpakning', N'Afventer'))
    BEGIN
        UPDATE dbo.ProductWholesaleOrder
        SET QuantityReceived = 0
        WHERE WholesaleOrderID=@wid;

        UPDATE dbo.WholesaleOrder
        SET DeliveryDate = NULL
        WHERE WholesaleOrderID=@wid;
    END

    FETCH NEXT FROM wcur INTO @wid, @wstatus;
END

CLOSE wcur;
DEALLOCATE wcur;

/* ---------- ReturnOrders (3) ---------- */
DECLARE @SalesIds TABLE (SalesOrderID UNIQUEIDENTIFIER NOT NULL);
INSERT INTO @SalesIds SELECT SalesOrderID FROM dbo.SalesOrder;

DECLARE @r INT = 0;
WHILE (@r < 3)
BEGIN
    DECLARE @sid3 UNIQUEIDENTIFIER = (SELECT TOP 1 SalesOrderID FROM @SalesIds ORDER BY NEWID());

    IF NOT EXISTS (SELECT 1 FROM dbo.ReturnOrder WHERE SalesOrderID = @sid3)
    BEGIN
        INSERT INTO dbo.ReturnOrder(ReturnOrderID, ReturnOrderDate, SalesOrderID, ReturnTotalPrice)
        VALUES(NEWID(), DATEADD(DAY, -(ABS(CHECKSUM(NEWID())) % 30), @Today), @sid3, 0.00);

        SET @r += 1;
    END
END

-- <<< END DummyDATA.sql
GO

/* ============ 4) VIEWS + PROCEDURES ============ */
/* Paste your current 02_ViewsAndProcedures.sql content here */
-- >>> BEGIN 02_ViewsAndProcedures.sql
/* =========================================================
   AREA: SALES HEAER VIEWS (Sorts based on SalesID)
   ========================================================= */

CREATE OR ALTER VIEW dbo.vw_SalesOrderHeaders
AS
SELECT
    so.SalesOrderID,
    so.CustomerID,
    c.CustomerNumber,              -- NEW: business-facing ID
    so.SalesOrderNumber,
    so.OrderStatus,
    so.PaymentStatus,
    so.SalesDate,
    so.ShippedDate,
    so.TotalPrice,
    (c.FirstName + ' ' + c.LastName) AS CustomerName,
    c.Email AS CustomerEmail,      -- NEW: for Selected Order Details
    c.City
FROM dbo.SalesOrder so
JOIN dbo.Customers c ON c.CustomerID = so.CustomerID;
GO



CREATE OR ALTER VIEW dbo.vw_SalesOrderLines
AS
SELECT
    pso.SalesOrderID,
    so.SalesOrderNumber,
    pso.ProductID,
    p.ProductName,
    pso.Quantity,
    pso.UnitPrice,
    (pso.Quantity * pso.UnitPrice) AS LineTotal
FROM dbo.ProductSalesOrder pso
JOIN dbo.SalesOrder so ON so.SalesOrderID = pso.SalesOrderID
JOIN dbo.Product p ON p.ProductID = pso.ProductID;
GO

/* =========================================================
   AREA: WHOLESALE – HEADER VIEWS (Sorts based on WholesaleID)
   ========================================================= */

CREATE OR ALTER VIEW dbo.vw_WholesaleOrdersHeader
AS
SELECT
    wo.WholesaleOrderID,
    wo.WholesaleOrderNumber,
    CONCAT(N'KØB-', CAST(wo.WholesaleOrderNumber AS varchar(20))) AS DisplayWholesaleOrderNumber,
    wo.WholesaleOrderDate,
    wo.ExpectedDeliveryDate,
    wo.DeliveryDate,
    wo.OrderStatus
FROM dbo.WholesaleOrder wo;
GO


/* =========================================================
   AREA: RETURN ORDER VIEW
   ========================================================= */

CREATE OR ALTER VIEW dbo.vw_ReturnOrders
AS
SELECT
    ro.ReturnOrderID,
    ro.ReturnOrderDate,
    ro.SalesOrderID,
    ro.ReturnTotalPrice,
    so.SalesOrderNumber
FROM dbo.ReturnOrder ro
JOIN dbo.SalesOrder so ON so.SalesOrderID = ro.SalesOrderID;
GO


/* =========================================================
   AREA: SALES ORDER LOOKUP (Human Order number to GUID)
   ========================================================= */

CREATE OR ALTER PROCEDURE dbo.usp_SelectSalesOrderId_BySalesOrderNumber
    @SalesOrderNumber INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        so.SalesOrderID
    FROM dbo.SalesOrder so
    WHERE so.SalesOrderNumber = @SalesOrderNumber;
END
GO


CREATE OR ALTER PROCEDURE dbo.usp_SelectSalesOrderValidationInfo_BySalesOrderNumber
    @SalesOrderNumber INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        so.SalesOrderID,
        c.Email
    FROM dbo.SalesOrder so
    INNER JOIN dbo.Customers c ON c.CustomerID = so.CustomerID
    WHERE so.SalesOrderNumber = @SalesOrderNumber;
END
GO


/* =========================================================
   AREA: SALES ORDER CRUD
   ========================================================= */

CREATE OR ALTER PROCEDURE dbo.usp_SelectById_SalesOrder
    @SalesOrderID UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        SalesOrderID,
        CustomerID,
        CustomerNumber,
        SalesOrderNumber,
        OrderStatus,
        PaymentStatus,
        SalesDate,
        ShippedDate,
        TotalPrice,
        CustomerName,
        CustomerEmail,
        City
    FROM dbo.vw_SalesOrderHeaders
    WHERE SalesOrderID = @SalesOrderID;
END
GO



CREATE OR ALTER PROCEDURE dbo.usp_Update_SalesOrder
    @SalesOrderID UNIQUEIDENTIFIER,
    @OrderStatus NVARCHAR(255),
    @PaymentStatus NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.SalesOrder
    SET
        OrderStatus = @OrderStatus,
        PaymentStatus = @PaymentStatus,
        ShippedDate =
            CASE
                WHEN @OrderStatus = 'Afsendt' THEN ISNULL(ShippedDate, CAST(GETDATE() AS date))
                ELSE NULL
            END
    WHERE SalesOrderID = @SalesOrderID;
END
GO


/* =========================================================
   AREA: SALES ORDER LINES (Filtered retrieval for UI / preview)
   ========================================================= */

CREATE OR ALTER PROCEDURE dbo.usp_Select_ProductSalesOrder_BySalesOrderID
    @SalesOrderID UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        SalesOrderID,
        SalesOrderNumber,
        ProductID,
        ProductName,
        Quantity,
        UnitPrice,
        LineTotal
    FROM dbo.vw_SalesOrderLines
    WHERE SalesOrderID = @SalesOrderID;
END
GO


/* =========================================================
   AREA: RETURN ORDER CRUD
   ========================================================= */

CREATE OR ALTER PROCEDURE dbo.usp_Insert_ReturnOrder
    @ReturnOrderID UNIQUEIDENTIFIER,
    @ReturnOrderDate DATE,
    @SalesOrderID UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TotalPrice DECIMAL(10,2);

    SELECT @TotalPrice = so.TotalPrice
    FROM dbo.SalesOrder so
    WHERE so.SalesOrderID = @SalesOrderID;

    INSERT INTO dbo.ReturnOrder (
        ReturnOrderID,
        ReturnOrderDate,
        SalesOrderID,
        ReturnTotalPrice
    )
    VALUES (
        @ReturnOrderID,
        @ReturnOrderDate,
        @SalesOrderID,
        ISNULL(@TotalPrice, 0)
    );
END
GO


CREATE OR ALTER PROCEDURE dbo.usp_SelectById_ReturnOrder
    @ReturnOrderID UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ro.ReturnOrderID,
        ro.ReturnOrderDate,
        ro.SalesOrderID,
        ro.ReturnTotalPrice,
        so.SalesOrderNumber
    FROM dbo.ReturnOrder ro
    JOIN dbo.SalesOrder so ON so.SalesOrderID = ro.SalesOrderID
    WHERE ro.ReturnOrderID = @ReturnOrderID;
END
GO


/* =========================================================
   AREA: RETURN UPDATE = entity only (NO side effects)
   ========================================================= */

CREATE OR ALTER PROCEDURE dbo.usp_Update_ReturnOrder
    @ReturnOrderID UNIQUEIDENTIFIER,
    @ReturnOrderDate DATE,
    @SalesOrderID UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.ReturnOrder
    SET
        ReturnOrderDate = @ReturnOrderDate,
        SalesOrderID = @SalesOrderID
    WHERE ReturnOrderID = @ReturnOrderID;
END
GO


CREATE OR ALTER PROCEDURE dbo.usp_DeleteById_ReturnOrder
    @ReturnOrderID UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.ReturnOrder
    WHERE ReturnOrderID = @ReturnOrderID;
END
GO


/* =========================================================
   AREA: RETURN PROCESS (Business Logic, explicit)
   ========================================================= */

CREATE OR ALTER PROCEDURE dbo.usp_ProcessReturnOrder
    @SalesOrderID UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    /*
        Business / domain logic ONLY:
        - Restock products
        - This return is going off on the assumption, that the product is NOT sell-able - With more time, we'd have implemented a feature to add "sellable" products back into the database.
        - Update order status
        - Audit / logging
    */

    UPDATE dbo.SalesOrder
    SET OrderStatus = 'Returned'
    WHERE SalesOrderID = @SalesOrderID;
END
GO


/* =========================================================
   AREA: StartPage graphics Metrics (Business Logic, explicit)
   ========================================================= */

CREATE OR ALTER PROCEDURE dbo.usp_StartPage_PackedToday
    @Today DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(*) AS PackedToday
    FROM dbo.SalesOrder
    WHERE OrderStatus = 'Afsendt'
      AND ShippedDate = @Today;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_StartPage_AverageOrderValue
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        CAST(SUM(so.TotalPrice) AS DECIMAL(18,2))
        / NULLIF(COUNT(so.SalesOrderID), 0) AS AverageOrderValue
    FROM dbo.SalesOrder so;
END
GO



CREATE OR ALTER PROCEDURE dbo.usp_StartPage_ReadyToPick
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(*) AS ReadyToPick
    FROM dbo.SalesOrder
    WHERE OrderStatus = 'Afventer';
END
GO


CREATE OR ALTER PROCEDURE dbo.usp_StartPage_PackedTotal
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(*) AS PackedTotal
    FROM dbo.SalesOrder
    WHERE OrderStatus = 'Afsendt';
END
GO


CREATE OR ALTER PROCEDURE dbo.usp_StartPage_AverageCustomerValue
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        CAST(SUM(so.TotalPrice) AS DECIMAL(18,2))
        / NULLIF(COUNT(DISTINCT so.CustomerID), 0) AS AverageCustomerValue
    FROM dbo.SalesOrder so;
END
GO

/* =========================================================
   AREA: PRODUCTS + AREA: CUSTOMERS VIEWS (Used by DBRepositories)
   ========================================================= */

CREATE OR ALTER VIEW dbo.vw_Products
AS
SELECT
    p.ProductID,
    p.ProductNumber,
    p.ProductName,
    p.Price,
    p.Size,
    p.Colour,
    p.NumberInStock
FROM dbo.Product p;
GO

CREATE OR ALTER VIEW dbo.vw_Customers
AS
SELECT
    c.CustomerID,
    c.CustomerNumber,
    c.DisplayCustomerNumber,
    c.FirstName,
    c.LastName,
    (c.FirstName + N' ' + c.LastName) AS CustomerName,
    c.Email,
    c.PhoneNumber,
    c.[Address],
    c.City,
    c.PostalCode
FROM dbo.Customers c;
GO



/* =========================================================
   AREA: PRODUCTS - When creating a product, this proc will create a @ProductID
   ========================================================= */
CREATE OR ALTER PROCEDURE dbo.usp_Insert_Product
    @ProductID UNIQUEIDENTIFIER,
    @ProductNumber NVARCHAR(255),
    @ProductName NVARCHAR(255),
    @Price DECIMAL(10,2),
    @Size NVARCHAR(255),
    @Colour NVARCHAR(255),
    @NumberInStock INT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.Product WHERE ProductNumber = @ProductNumber)
    BEGIN
        RAISERROR('ProductNumber %s er allerede i brug', 16, 1, @ProductNumber);
        RETURN;
    END

    IF @Price < 0
    BEGIN
        RAISERROR('Prisen kan ikke være negativ', 16, 1);
        RETURN;
    END

    INSERT INTO dbo.Product
    (
        ProductID, ProductNumber, ProductName, Price, Size, Colour, NumberInStock
    )
    VALUES
    (
        @ProductID, @ProductNumber, @ProductName, @Price, @Size, @Colour, @NumberInStock
    );
END
GO

    /* =========================================================
   AREA: PRODUCTS - Updating/Editting a Product - Same proc as above, however it LookUps for @ProductID, rather than making it.
   ========================================================= */

CREATE OR ALTER PROCEDURE dbo.usp_Update_Product
    @ProductID UNIQUEIDENTIFIER,
    @ProductNumber NVARCHAR(255),
    @ProductName NVARCHAR(255),
    @Price DECIMAL(10,2),
    @Size NVARCHAR(255),
    @Colour NVARCHAR(255),
    @NumberInStock INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Product WHERE ProductID = @ProductID)
    BEGIN
        RAISERROR('Product does not exist', 16, 1);
        RETURN;
    END

    IF EXISTS (
        SELECT 1
        FROM dbo.Product
        WHERE ProductNumber = @ProductNumber
          AND ProductID <> @ProductID
    )
    BEGIN
        RAISERROR('ProductNumber %s is already in use', 16, 1, @ProductNumber);
        RETURN;
    END

    IF @Price < 0
    BEGIN
        RAISERROR('Price cannot be negative', 16, 1);
        RETURN;
    END

    IF @NumberInStock < 0
    BEGIN
        RAISERROR('NumberInStock cannot be negative', 16, 1);
        RETURN;
    END

    UPDATE dbo.Product
    SET
        ProductNumber   = @ProductNumber,
        ProductName     = @ProductName,
        Price           = @Price,
        Size            = @Size,
        Colour          = @Colour,
        NumberInStock   = @NumberInStock
    WHERE ProductID = @ProductID;
END
GO



/* =========================================================
   AREA: CUSTOMER VIEW (Used by CustomerDBRepository)
   ========================================================= */

CREATE OR ALTER VIEW dbo.vw_Customers
AS
SELECT
    c.CustomerID,
    c.CustomerNumber,
    c.DisplayCustomerNumber,
    c.FirstName,
    c.LastName,
    (c.FirstName + ' ' + c.LastName) AS FullName,  -- IMPORTANT: matches CustomerDBRepository.Map(...)
    c.Email,
    c.PhoneNumber,
    c.[Address],
    c.City,
    c.PostalCode
FROM dbo.Customers c;
GO




/* =========================================================
   AREA: WHOLESALE ORDER VIEWS (Header + Lines)
   ========================================================= */

CREATE OR ALTER VIEW dbo.vw_WholesaleOrderHeaders
AS
SELECT
    wo.WholesaleOrderID,
    wo.WholesaleOrderNumber,
    CONCAT(N'KØB-', CAST(wo.WholesaleOrderNumber AS varchar(20))) AS DisplayWholesaleOrderNumber,
    wo.WholesaleOrderDate,
    wo.ExpectedDeliveryDate,
    wo.DeliveryDate,
    wo.OrderStatus
FROM dbo.WholesaleOrder wo;
GO


CREATE OR ALTER VIEW dbo.vw_WholesaleOrderLines
AS
SELECT
    pwo.WholesaleOrderID,
    wo.WholesaleOrderNumber,
    CONCAT(N'KØB-', CAST(wo.WholesaleOrderNumber AS varchar(20))) AS DisplayWholesaleOrderNumber,
    pwo.ProductID,
    p.ProductNumber,
    p.ProductName,
    pwo.Quantity,
    pwo.UnitPrice,
    pwo.QuantityReceived,
    (pwo.Quantity - pwo.QuantityReceived) AS QuantityPending,
    (pwo.Quantity * pwo.UnitPrice) AS LineTotal
FROM dbo.ProductWholesaleOrder pwo
JOIN dbo.WholesaleOrder wo ON wo.WholesaleOrderID = pwo.WholesaleOrderID
JOIN dbo.Product p ON p.ProductID = pwo.ProductID;
GO



/* =========================================================
   AREA: WHOLESALE ORDER LOOKUP (Human number -> GUID)
   ========================================================= */

CREATE OR ALTER PROCEDURE dbo.usp_SelectWholesaleOrderId_ByWholesaleOrderNumber
    @WholesaleOrderNumber INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        wo.WholesaleOrderID
    FROM dbo.WholesaleOrder wo
    WHERE wo.WholesaleOrderNumber = @WholesaleOrderNumber;
END
GO


/* =========================================================
   AREA: WHOLESALE ORDER CRUD (Entity only, NO side effects)
   ========================================================= */

CREATE OR ALTER PROCEDURE dbo.usp_SelectById_WholesaleOrder
    @WholesaleOrderID UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        WholesaleOrderID,
        WholesaleOrderNumber,
        DisplayWholesaleOrderNumber,
        WholesaleOrderDate,
        ExpectedDeliveryDate,
        DeliveryDate,
        OrderStatus
    FROM dbo.vw_WholesaleOrderHeaders
    WHERE WholesaleOrderID = @WholesaleOrderID;
END
GO


CREATE OR ALTER PROCEDURE dbo.usp_Select_ProductWholesaleOrder_ByWholesaleOrderID
    @WholesaleOrderID UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        WholesaleOrderID,
        WholesaleOrderNumber,
        DisplayWholesaleOrderNumber,
        ProductID,
        ProductNumber,
        ProductName,
        Quantity,
        UnitPrice,
        QuantityReceived,
        QuantityPending,
        LineTotal
    FROM dbo.vw_WholesaleOrderLines
    WHERE WholesaleOrderID = @WholesaleOrderID;
END
GO


CREATE OR ALTER PROCEDURE dbo.usp_Insert_WholesaleOrder
    @WholesaleOrderID UNIQUEIDENTIFIER,
    @WholesaleOrderDate DATE,
    @ExpectedDeliveryDate DATE,
    @OrderStatus NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    IF @ExpectedDeliveryDate < @WholesaleOrderDate
    BEGIN
        RAISERROR('Expected delivery date cannot be before wholesale order date', 16, 1);
        RETURN;
    END

    INSERT INTO dbo.WholesaleOrder
    (
        WholesaleOrderID,
        WholesaleOrderDate,
        ExpectedDeliveryDate,
        DeliveryDate,
        OrderStatus
    )
    VALUES
    (
        @WholesaleOrderID,
        @WholesaleOrderDate,
        @ExpectedDeliveryDate,
        NULL,
        @OrderStatus
    );
END
GO


CREATE OR ALTER PROCEDURE dbo.usp_Insert_ProductWholesaleOrder
    @WholesaleOrderID UNIQUEIDENTIFIER,
    @ProductID UNIQUEIDENTIFIER,
    @Quantity INT,
    @UnitPrice DECIMAL(10,2)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.WholesaleOrder WHERE WholesaleOrderID = @WholesaleOrderID)
    BEGIN
        RAISERROR('Wholesale order does not exist', 16, 1);
        RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM dbo.Product WHERE ProductID = @ProductID)
    BEGIN
        RAISERROR('Product does not exist', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1
               FROM dbo.ProductWholesaleOrder
               WHERE WholesaleOrderID = @WholesaleOrderID AND ProductID = @ProductID)
    BEGIN
        RAISERROR('Product already exists in this wholesale order', 16, 1);
        RETURN;
    END

    IF @Quantity <= 0
    BEGIN
        RAISERROR('Quantity must be > 0', 16, 1);
        RETURN;
    END

    IF @UnitPrice < 0
    BEGIN
        RAISERROR('Unit price cannot be negative', 16, 1);
        RETURN;
    END

    INSERT INTO dbo.ProductWholesaleOrder
    (
        WholesaleOrderID,
        ProductID,
        Quantity,
        UnitPrice,
        QuantityReceived
    )
    VALUES
    (
        @WholesaleOrderID,
        @ProductID,
        @Quantity,
        @UnitPrice,
        0
    );
END
GO


CREATE OR ALTER PROCEDURE dbo.usp_Update_WholesaleOrder
    @WholesaleOrderID UNIQUEIDENTIFIER,
    @WholesaleOrderDate DATE,
    @ExpectedDeliveryDate DATE,
    @DeliveryDate DATE = NULL,
    @OrderStatus NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.WholesaleOrder WHERE WholesaleOrderID = @WholesaleOrderID)
    BEGIN
        RAISERROR('Wholesale order does not exist', 16, 1);
        RETURN;
    END

    IF @ExpectedDeliveryDate < @WholesaleOrderDate
    BEGIN
        RAISERROR('Expected delivery date cannot be before wholesale order date', 16, 1);
        RETURN;
    END

    UPDATE dbo.WholesaleOrder
    SET
        WholesaleOrderDate = @WholesaleOrderDate,
        ExpectedDeliveryDate = @ExpectedDeliveryDate,
        DeliveryDate = @DeliveryDate,
        OrderStatus = @OrderStatus
    WHERE WholesaleOrderID = @WholesaleOrderID;
END
GO


/* =========================================================
   AREA: WHOLESALE RECEIVING PROCESS (Business Logic, explicit)
   - Updates QuantityReceived
   - Updates Product stock (NumberInStock)
   - Auto-updates Wholesale order status + DeliveryDate when fully received
   ========================================================= */

CREATE OR ALTER PROCEDURE dbo.usp_ProcessWholesaleReceipt_Line
    @WholesaleOrderID UNIQUEIDENTIFIER,
    @ProductID UNIQUEIDENTIFIER,
    @ReceiveQuantity INT
AS
BEGIN
    SET NOCOUNT ON;

    IF @ReceiveQuantity <= 0
    BEGIN
        RAISERROR('Modtaget mængde skal være mere end 0', 16, 1);
        RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM dbo.WholesaleOrder WHERE WholesaleOrderID = @WholesaleOrderID)
    BEGIN
        RAISERROR('Indkøbsordren findes ikke.', 16, 1);
        RETURN;
    END

    IF NOT EXISTS (SELECT 1
                   FROM dbo.ProductWholesaleOrder
                   WHERE WholesaleOrderID = @WholesaleOrderID AND ProductID = @ProductID)
    BEGIN
        RAISERROR('Produktet er ikke en del af denne indkøbsordre.', 16, 1);
        RETURN;
    END

    DECLARE @Qty INT;
    DECLARE @QtyReceived INT;

    SELECT
        @Qty = Quantity,
        @QtyReceived = QuantityReceived
    FROM dbo.ProductWholesaleOrder
    WHERE WholesaleOrderID = @WholesaleOrderID AND ProductID = @ProductID;

    IF (@QtyReceived + @ReceiveQuantity) > @Qty
    BEGIN
        RAISERROR('Kan ikke modtage mere end bestilte mængde.', 16, 1);
        RETURN;
    END

    -- Update received quantity
    UPDATE dbo.ProductWholesaleOrder
    SET QuantityReceived = QuantityReceived + @ReceiveQuantity
    WHERE WholesaleOrderID = @WholesaleOrderID AND ProductID = @ProductID;

    -- Update stock
    UPDATE dbo.Product
    SET NumberInStock = NumberInStock + @ReceiveQuantity
    WHERE ProductID = @ProductID;



    /* -----------------------------
       AREA: WholesaleOrder Update status
       - Fully received => Modtaget + DeliveryDate
       - Partially received => Delvist modtaget
       - None received => keep current status (e.g., Afventer)
       ----------------------------- */

    DECLARE @HasAnyReceived BIT =
        CASE WHEN EXISTS (
            SELECT 1
            FROM dbo.ProductWholesaleOrder pwo
            WHERE pwo.WholesaleOrderID = @WholesaleOrderID
              AND pwo.QuantityReceived > 0
        ) THEN 1 ELSE 0 END;

    DECLARE @AllFullyReceived BIT =
        CASE WHEN NOT EXISTS (
            SELECT 1
            FROM dbo.ProductWholesaleOrder pwo
            WHERE pwo.WholesaleOrderID = @WholesaleOrderID
              AND pwo.QuantityReceived < pwo.Quantity
        ) THEN 1 ELSE 0 END;

    IF (@AllFullyReceived = 1)
    BEGIN
        UPDATE dbo.WholesaleOrder
        SET
            OrderStatus = N'Modtaget',
            DeliveryDate = ISNULL(DeliveryDate, CAST(GETDATE() AS date))
        WHERE WholesaleOrderID = @WholesaleOrderID;
    END
    ELSE IF (@HasAnyReceived = 1)
    BEGIN
        UPDATE dbo.WholesaleOrder
        SET
            OrderStatus = N'Delvist modtaget'
        WHERE WholesaleOrderID = @WholesaleOrderID;
    END

END
GO


CREATE OR ALTER PROCEDURE dbo.usp_StartPage_WholesaleOnTheWay
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(*) AS WholesaleOnTheWay
    FROM dbo.WholesaleOrder
    WHERE OrderStatus IN (N'Afventer', N'Modtaget', N'Klar til udpakning');
END
GO



CREATE OR ALTER PROCEDURE dbo.usp_StartPage_TotalReturnRate
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        CAST((SELECT COUNT(*) FROM dbo.ReturnOrder) AS DECIMAL(18,4))
        / NULLIF((SELECT COUNT(*) FROM dbo.SalesOrder), 0) AS TotalReturnRate;
END
GO


CREATE OR ALTER PROCEDURE dbo.usp_StartPage_OutstandingPayments
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(*) AS OutstandingPayments
    FROM dbo.SalesOrder
    WHERE OrderStatus = N'Afsendt'
      AND PaymentStatus IN (N'Afventer', N'Afventer Betaling');
END
GO



CREATE OR ALTER PROCEDURE dbo.usp_StartPage_UniqueCustomers
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(DISTINCT so.CustomerID) AS UniqueCustomers
    FROM dbo.SalesOrder so;
END
GO


/* =========================================================
   FIX: Missing view used by CustomerSalesOrderDisplayDBRepository
   ========================================================= */
CREATE OR ALTER VIEW dbo.vw_CustomerSalesOrders
AS
SELECT 
    c.CustomerID,
    c.CustomerNumber,
    c.DisplayCustomerNumber,
    c.FirstName,
    c.LastName,
    (c.FirstName + N' ' + c.LastName) AS FullName,
    so.SalesOrderID,
    so.SalesOrderNumber,
    so.DisplaySalesOrderNumber,
    so.OrderStatus,
    so.PaymentStatus,
    so.SalesDate,
    so.TotalPrice
FROM dbo.Customers c
JOIN dbo.SalesOrder so ON c.CustomerID = so.CustomerID;
GO


/* =========================================================
   FIX: Missing legacy view (some parts of the app expect it)
   ========================================================= */
CREATE OR ALTER VIEW dbo.vw_SalesOrders
AS
SELECT 
    so.SalesOrderID,
    so.CustomerID,
    so.SalesOrderNumber, 
    so.DisplaySalesOrderNumber,
    so.OrderStatus, 
    so.PaymentStatus, 
    so.SalesDate, 
    so.TotalPrice, 
    p.ProductID,
    p.ProductNumber,
    p.ProductName, 
    pso.Quantity,
    pso.UnitPrice,
    (pso.Quantity * pso.UnitPrice) AS LineTotal
FROM dbo.SalesOrder so
JOIN dbo.ProductSalesOrder pso ON so.SalesOrderID = pso.SalesOrderID
JOIN dbo.Product p ON pso.ProductID = p.ProductID;
GO


/* =========================================================
   FIX: Missing legacy view used by WholesaleOrderDisplayDBRepository
   ========================================================= */
CREATE OR ALTER VIEW dbo.vw_WholesaleOrders
AS
SELECT 
    wo.WholesaleOrderID,
    wo.WholesaleOrderNumber,
    CONCAT(N'KØB-', CAST(wo.WholesaleOrderNumber AS varchar(20))) AS DisplayWholesaleOrderNumber,
    wo.WholesaleOrderDate, 
    wo.ExpectedDeliveryDate, 
    wo.DeliveryDate, 
    wo.OrderStatus, 
    p.ProductID,
    p.ProductNumber,
    p.ProductName, 
    p.Size, 
    p.Colour, 
    pwo.Quantity,
    pwo.UnitPrice,
    pwo.QuantityReceived,
    (pwo.Quantity * pwo.UnitPrice) AS LineTotal
FROM dbo.WholesaleOrder wo
JOIN dbo.ProductWholesaleOrder pwo ON wo.WholesaleOrderID = pwo.WholesaleOrderID
JOIN dbo.Product p ON pwo.ProductID = p.ProductID;
GO


-- <<< END 02_ViewsAndProcedures.sql
GO

/* ============ 5) Users (Alex/David/Patrick) ============ */
USE master;
GO

DECLARE @pwd nvarchar(128) = N'Sommerfugl123!';

IF SUSER_ID(N'Alex') IS NULL
    EXEC(N'CREATE LOGIN [Alex] WITH PASSWORD = ''' + @pwd + N''', CHECK_POLICY = OFF, CHECK_EXPIRATION = OFF;');
IF SUSER_ID(N'David') IS NULL
    EXEC(N'CREATE LOGIN [David] WITH PASSWORD = ''' + @pwd + N''', CHECK_POLICY = OFF, CHECK_EXPIRATION = OFF;');
IF SUSER_ID(N'Patrick') IS NULL
    EXEC(N'CREATE LOGIN [Patrick] WITH PASSWORD = ''' + @pwd + N''', CHECK_POLICY = OFF, CHECK_EXPIRATION = OFF;');
GO

USE Undy;
GO

IF USER_ID(N'Alex') IS NULL    CREATE USER [Alex] FOR LOGIN [Alex] WITH DEFAULT_SCHEMA = [dbo];
IF USER_ID(N'David') IS NULL   CREATE USER [David] FOR LOGIN [David] WITH DEFAULT_SCHEMA = [dbo];
IF USER_ID(N'Patrick') IS NULL CREATE USER [Patrick] FOR LOGIN [Patrick] WITH DEFAULT_SCHEMA = [dbo];
GO

ALTER ROLE [db_owner] ADD MEMBER [Alex];
ALTER ROLE [db_owner] ADD MEMBER [David];
ALTER ROLE [db_owner] ADD MEMBER [Patrick];
GO

/* ============ 6) Verify ============ */
USE Undy;
GO

SELECT
  (SELECT COUNT(*) FROM sys.tables)      AS Tables,
  (SELECT COUNT(*) FROM sys.views)       AS Views,
  (SELECT COUNT(*) FROM sys.procedures)  AS Procs;

SELECT TOP (5) * FROM dbo.Customers;
SELECT TOP (5) * FROM dbo.Product;
SELECT TOP (5) * FROM dbo.SalesOrder;
SELECT TOP (5) * FROM dbo.WholesaleOrder;
GO
