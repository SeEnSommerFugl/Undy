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
   PRODUCTS + CUSTOMERS VIEWS (Used by DBRepositories)
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
   WHOLESALE ORDER VIEWS (Header + Lines)
   ========================================================= */

CREATE OR ALTER VIEW dbo.vw_WholesaleOrderHeaders
AS
SELECT
    wo.WholesaleOrderID,
    wo.WholesaleOrderNumber,
    wo.DisplayWholesaleOrderNumber,
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
    wo.DisplayWholesaleOrderNumber,
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
   WHOLESALE ORDER LOOKUP (Human number -> GUID)
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
   WHOLESALE ORDER CRUD (Entity only, NO side effects)
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
   WHOLESALE RECEIVING PROCESS (Business Logic, explicit)
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
        RAISERROR('ReceiveQuantity must be > 0', 16, 1);
        RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM dbo.WholesaleOrder WHERE WholesaleOrderID = @WholesaleOrderID)
    BEGIN
        RAISERROR('Wholesale order does not exist', 16, 1);
        RETURN;
    END

    IF NOT EXISTS (SELECT 1
                   FROM dbo.ProductWholesaleOrder
                   WHERE WholesaleOrderID = @WholesaleOrderID AND ProductID = @ProductID)
    BEGIN
        RAISERROR('Product is not part of this wholesale order', 16, 1);
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
        RAISERROR('Cannot receive more than ordered quantity', 16, 1);
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
       Update WholesaleOrder status
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
    WHERE OrderStatus IN (N'Afventer', N'Pending', N'Klar til udpakning');
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

