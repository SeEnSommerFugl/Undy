/* =========================================================
   SALES ORDER VIEWS
   ========================================================= */

CREATE OR ALTER VIEW dbo.vw_SalesOrderHeaders
AS
SELECT
    so.SalesOrderID,
    so.CustomerID,
    so.SalesOrderNumber,
    so.OrderStatus,
    so.PaymentStatus,
    so.SalesDate,
    so.ShippedDate,
    so.TotalPrice,
    (c.FirstName + ' ' + c.LastName) AS CustomerName,
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
   RETURN ORDER VIEW
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
   SALES ORDER LOOKUP (Human Order number to GUID)
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
   SALES ORDER CRUD
   ========================================================= */

CREATE OR ALTER PROCEDURE dbo.usp_SelectById_SalesOrder
    @SalesOrderID UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        SalesOrderID,
        CustomerID,
        SalesOrderNumber,
        OrderStatus,
        PaymentStatus,
        SalesDate,
        ShippedDate,
        TotalPrice,
        CustomerName,
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
   SALES ORDER LINES (Filtered retrieval for UI / preview)
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
   RETURN ORDER CRUD
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
   UPDATE = entity only (NO side effects)
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
   RETURN PROCESS (Business Logic, explicit)
   ========================================================= */

CREATE OR ALTER PROCEDURE dbo.usp_ProcessReturnOrder
    @SalesOrderID UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    /*
        Business / domain logic ONLY:
        - Restock products
        - Update order status
        - Audit / logging
    */

    UPDATE dbo.SalesOrder
    SET OrderStatus = 'Returned'
    WHERE SalesOrderID = @SalesOrderID;
END
GO


/* =========================================================
   StartPage graphics Metrics (Business Logic, explicit)
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
        CAST((SELECT COUNT(*) FROM dbo.Customers) AS DECIMAL(18,4))
        / NULLIF((SELECT SUM(TotalPrice) FROM dbo.SalesOrder), 0) AS AverageCustomerValue;
END
GO


CREATE OR ALTER PROCEDURE dbo.usp_StartPage_WholesaleOnTheWay
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(*) AS WholesaleOnTheWay
    FROM dbo.WholesaleOrder
    WHERE OrderStatus = 'Pending';
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
    WHERE OrderStatus = 'Afsendt'
      AND PaymentStatus = 'Afventer';
END
GO


CREATE OR ALTER PROCEDURE dbo.usp_StartPage_UniqueCustomers
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(*) AS UniqueCustomers
    FROM dbo.Customers;
END
GO
