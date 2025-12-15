/* =========================================================
   SALES ORDER VIEWS
   ========================================================= */

CREATE OR ALTER VIEW vw_SalesOrderHeaders
AS
SELECT
    so.SalesOrderID,
    so.SalesOrderNumber,
    so.OrderDate,
    so.OrderStatus,
    so.CustomerID,
    c.FirstName + ' ' + c.LastName AS CustomerName
FROM SalesOrder so
JOIN Customers c ON c.CustomerID = so.CustomerID;
GO


CREATE OR ALTER VIEW vw_SalesOrderLines
AS
SELECT
    pso.ProductSalesOrderID,
    pso.SalesOrderID,
    pso.ProductID,
    p.ProductName,
    pso.Quantity,
    pso.UnitPrice
FROM ProductSalesOrder pso
JOIN Product p ON p.ProductID = pso.ProductID;
GO


/* =========================================================
   RETURN ORDER VIEW
   ========================================================= */

CREATE OR ALTER VIEW vw_ReturnOrders
AS
SELECT
    ro.ReturnOrderID,
    ro.ReturnOrderDate,
    ro.SalesOrderID,
    so.SalesOrderNumber
FROM ReturnOrder ro
JOIN SalesOrder so ON so.SalesOrderID = ro.SalesOrderID;
GO


/* =========================================================
   SALES ORDER LOOKUP (Human Order number to  GUID)
   ========================================================= */

CREATE OR ALTER PROCEDURE usp_SelectSalesOrderId_BySalesOrderNumber
    @SalesOrderNumber INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        SalesOrderID
    FROM SalesOrder
    WHERE SalesOrderNumber = @SalesOrderNumber;
END
GO


/* =========================================================
   RETURN ORDER CRUD
   ========================================================= */

CREATE OR ALTER PROCEDURE usp_Insert_ReturnOrder
    @ReturnOrderID UNIQUEIDENTIFIER,
    @ReturnOrderDate DATE,
    @SalesOrderID UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO ReturnOrder (
        ReturnOrderID,
        ReturnOrderDate,
        SalesOrderID
    )
    VALUES (
        @ReturnOrderID,
        @ReturnOrderDate,
        @SalesOrderID
    );
END
GO


CREATE OR ALTER PROCEDURE usp_SelectById_ReturnOrder
    @ReturnOrderID UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ReturnOrderID,
        ReturnOrderDate,
        SalesOrderID,
        SalesOrderNumber
    FROM vw_ReturnOrders
    WHERE ReturnOrderID = @ReturnOrderID;
END
GO


/* =========================================================
   UPDATE = entity only (NO side effects)
   ========================================================= */

CREATE OR ALTER PROCEDURE usp_Update_ReturnOrder
    @ReturnOrderID UNIQUEIDENTIFIER,
    @ReturnOrderDate DATE,
    @SalesOrderID UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE ReturnOrder
    SET
        ReturnOrderDate = @ReturnOrderDate,
        SalesOrderID = @SalesOrderID
    WHERE ReturnOrderID = @ReturnOrderID;
END
GO


CREATE OR ALTER PROCEDURE usp_DeleteById_ReturnOrder
    @ReturnOrderID UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM ReturnOrder
    WHERE ReturnOrderID = @ReturnOrderID;
END
GO


/* =========================================================
   RETURN PROCESS (Business Logic, explicit)
   ========================================================= */

CREATE OR ALTER PROCEDURE usp_ProcessReturnOrder
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

    UPDATE SalesOrder
    SET OrderStatus = 'Returned'
    WHERE SalesOrderID = @SalesOrderID;
END
GO
