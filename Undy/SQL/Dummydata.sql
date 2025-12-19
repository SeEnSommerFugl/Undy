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
