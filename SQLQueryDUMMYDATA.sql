USE Undy
GO
-- Product insert

DECLARE @ProductID UNIQUEIDENTIFIER;

SET @ProductID = NEWID();
EXEC usp_Insert_Product
	@ProductID = @ProductID,
	@ProductNumber = 'UBBABLS1',
	@ProductName = 'Bambus Boxerbriefs',
	@Price = 139,
	@Size = 'S',
	@Colour = 'Sort',
	@NumberInStock = 40


SET @ProductID = NEWID();
EXEC usp_Insert_Product
	@ProductID = @ProductID,
	@ProductNumber = 'UBBABLM1',
	@ProductName = 'Bambus Boxerbriefs',
	@Price = 139,
	@Size = 'M',
	@Colour = 'Sort',
	@NumberInStock = 50


SET @ProductID = NEWID();
EXEC usp_Insert_Product
	@ProductID = @ProductID,
	@ProductNumber = 'UBBABLL1',
	@ProductName = 'Bambus Boxerbriefs',
	@Price = 139,
	@Size = 'L',
	@Colour = 'Sort',
	@NumberInStock = 40


SET @ProductID = NEWID();
EXEC usp_Insert_Product
	@ProductID = @ProductID,
	@ProductNumber = 'UBBABLXL1',
	@ProductName = 'Bambus Boxerbriefs',
	@Price = 139,
	@Size = 'XL',
	@Colour = 'Sort',
	@NumberInStock = 30


SET @ProductID = NEWID();
EXEC usp_Insert_Product
	@ProductID = @ProductID,
	@ProductNumber = 'UBBABLXXL1',
	@ProductName = 'Bambus Boxerbriefs',
	@Price = 139,
	@Size = '2XL',
	@Colour = 'Sort',
	@NumberInStock = 20


SET @ProductID = NEWID();
EXEC usp_Insert_Product
	@ProductID = @ProductID,
	@ProductNumber = 'UBBABLXXXL1',
	@ProductName = 'Bambus Boxerbriefs',
	@Price = 139,
	@Size = '3XL',
	@Colour = 'Sort',
	@NumberInStock = 20
GO


-- WholesaleOrder insert

DECLARE @WholesaleOrderID UNIQUEIDENTIFIER;


SET @WholesaleOrderID = NEWID();
EXEC usp_Insert_WholesaleOrder
	@WholesaleOrderID = @WholesaleOrderID,
	@WholesaleOrderDate = '2025-12-01',
	@ExpectedDeliveryDate = '2025-12-24',
	@OrderStatus = 'Afventer'


SET @WholesaleOrderID = NEWID();
EXEC usp_Insert_WholesaleOrder
	@WholesaleOrderID = @WholesaleOrderID,
	@WholesaleOrderDate = '2026-01-01',
	@ExpectedDeliveryDate = '2026-01-24',
	@OrderStatus = 'Afventer'


SET @WholesaleOrderID = NEWID();
EXEC usp_Insert_WholesaleOrder
	@WholesaleOrderID = @WholesaleOrderID,
	@WholesaleOrderDate = '2026-02-01',
	@ExpectedDeliveryDate = '2026-02-24',
	@OrderStatus = 'Afventer'


SET @WholesaleOrderID = NEWID();
EXEC usp_Insert_WholesaleOrder
	@WholesaleOrderID = @WholesaleOrderID,
	@WholesaleOrderDate = '2026-03-01',
	@ExpectedDeliveryDate = '2026-03-24',
	@OrderStatus = 'Afventer'
GO

-- Insert ProductWholesaleOrder

EXEC usp_Insert_ProductWholesaleOrder
	@WholesaleOrderNumber = 1,
	@ProductNumber = 'UBBABLS1',
	@Quantity = 50,
	@UnitPrice = 139


EXEC usp_Insert_ProductWholesaleOrder
	@WholesaleOrderNumber = 2,
	@ProductNumber = 'UBBABLM1',
	@Quantity = 60,
	@UnitPrice = 139


EXEC usp_Insert_ProductWholesaleOrder
	@WholesaleOrderNumber = 3,
	@ProductNumber = 'UBBABLL1',
	@Quantity = 50,
	@UnitPrice = 139


EXEC usp_Insert_ProductWholesaleOrder
	@WholesaleOrderNumber = 4,
	@ProductNumber = 'UBBABLXL1',
	@Quantity = 40,
	@UnitPrice = 139
GO

-- Insert SalesOrder

DECLARE @SalesOrderID UNIQUEIDENTIFIER;

SET @SalesOrderID = NEWID();
EXEC usp_Insert_SalesOrder
	@SalesOrderID = @SalesOrderID,
	@OrderStatus = 'Afventer',
	@PaymentStatus = 'Afventer Betaling',
	@SalesDate = '2025-12-05',
	@CustomerNumber = 11


SET @SalesOrderID = NEWID();
EXEC usp_Insert_SalesOrder
	@SalesOrderID = @SalesOrderID,
	@OrderStatus = 'Afsendt',
	@PaymentStatus = 'Betalt',
	@SalesDate = '2025-12-05',
	@CustomerNumber = 12


SET @SalesOrderID = NEWID();
EXEC usp_Insert_SalesOrder
	@SalesOrderID = @SalesOrderID,
	@OrderStatus = 'Afventer',
	@PaymentStatus = 'Afventer Betaling',
	@SalesDate = '2025-12-05',
	@CustomerNumber = 13
GO

-- Insert ProductSalesOrder
EXEC usp_Insert_ProductSalesOrder
	@SalesOrderNumber = 14,
	@ProductNumber = 'UBBABLS1',
	@Quantity = 2,
	@UnitPrice = 139


EXEC usp_Insert_ProductSalesOrder
	@SalesOrderNumber = 15,
	@ProductNumber = 'UBBABLS1',
	@Quantity = 1,
	@UnitPrice = 139


EXEC usp_Insert_ProductSalesOrder
	@SalesOrderNumber = 16,
	@ProductNumber = 'UBBABLM1',
	@Quantity = 1,
	@UnitPrice = 139


EXEC usp_Insert_ProductSalesOrder
	@SalesOrderNumber = 16,
	@ProductNumber = 'UBBABLXL1',
	@Quantity = 4,
	@UnitPrice = 139
GO

-- Customer Insert

DECLARE @CustomerID UNIQUEIDENTIFIER;

SET @CustomerID = NEWID();
EXEC usp_Insert_Customer
	@CustomerID = @CustomerID,
	@Firstname = 'John',
	@LastName = 'Doe',
	@Email = 'JohnD@email.com',
	@PhoneNumber = 12345678,
	@Address = 'Spurvevej 3',
	@City = 'Spurveby',
	@PostalCode = 2000


SET @CustomerID = NEWID();
EXEC usp_Insert_Customer
	@CustomerID = @CustomerID,
	@Firstname = 'Per',
	@LastName = 'Pedersen',
	@Email = 'PerP@email.com',
	@PhoneNumber = 23456789,
	@Address = 'Fasanvej 25',
	@City = 'Fasanby',
	@PostalCode = 2200


SET @CustomerID = NEWID();
EXEC usp_Insert_Customer
	@CustomerID = @CustomerID,
	@Firstname = 'Lars',
	@LastName = 'Larsen',
	@Email = 'LarsL@email.com',
	@PhoneNumber = 34567891,
	@Address = 'Ørnevej 38',
	@City = 'Ørneby',
	@PostalCode = 2400


SET @CustomerID = NEWID();
EXEC usp_Insert_Customer
	@CustomerID = @CustomerID,
	@Firstname = 'Jens',
	@LastName = 'Jensen',
	@Email = 'JensJ@email.com',
	@PhoneNumber = 45678912,
	@Address = 'Hajrevej 3',
	@City = 'Hajreby',
	@PostalCode = 2600


SET @CustomerID = NEWID();
EXEC usp_Insert_Customer
	@CustomerID = @CustomerID,
	@Firstname = 'Martin',
    @LastName = 'Hansen',
    @Email = 'MartinH@email.com',
    @PhoneNumber = 56789123,
    @Address = 'Skovvej 12',
    @City = 'Skovby',
    @PostalCode = 2700


SET @CustomerID = NEWID();
EXEC usp_insert_Customer
	@CustomerID = @CustomerID,
	@FirstName = 'Mikkel',
	@LastName = 'Nielsen',
	@Email = 'MikkelN@email.com',
	@PhoneNumber = 67891234,
    @Address = 'Bakkevej 44',
    @City = 'Bakkeby',
    @PostalCode = 2800


SET @CustomerID = NEWID();
EXEC usp_Insert_Customer
	@CustomerID = @CustomerID,
    @Firstname = 'Søren',
    @LastName = 'Madsen',
    @Email = 'SørenM@email.com',
    @PhoneNumber = 78912345,
    @Address = 'Engvej 7',
    @City = 'Engby',
    @PostalCode = 2900


SET @CustomerID = NEWID();
EXEC usp_Insert_Customer
	@CustomerID = @CustomerID,
    @Firstname = 'Anders',
    @LastName = 'Kristensen',
    @Email = 'AndersK@email.com',
    @PhoneNumber = 89123456,
    @Address = 'Strandvej 55',
    @City = 'Strandby',
    @PostalCode = 2100


SET @CustomerID = NEWID();
EXEC usp_Insert_Customer
	@CustomerID = @CustomerID,
    @Firstname = 'Karsten',
    @LastName = 'Olsen',
    @Email = 'KarstenO@email.com',
    @PhoneNumber = 91234567,
    @Address = 'Mosevej 9',
    @City = 'Moseby',
    @PostalCode = 2300


EXEC usp_Insert_Customer
	@CustomerID = @CustomerID,
    @Firstname = 'Thomas',
    @LastName = 'Mortensen',
    @Email = 'ThomasM@email.com',
    @PhoneNumber = 12349876,
    @Address = 'Klitvej 21',
    @City = 'Klitby',
    @PostalCode = 2500
GO



CREATE OR ALTER PROCEDURE usp_UpdatePartial_WholesaleOrder 
	@WholesaleOrderNumber INT, 
	@ProductNumber NVARCHAR(255),
	@Quantity INT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @WholesaleOrderID UNIQUEIDENTIFIER;
	
	SELECT @WholesaleOrderID = WholesaleOrderID
	FROM WholesaleOrder
	WHERE WholesaleOrderNumber = @WholesaleOrderNumber;
	
	-- Validate Wholesale order exists
	IF NOT EXISTS(SELECT 1 FROM WholesaleOrder WHERE WholesaleOrderID = @WholesaleOrderID)
	BEGIN
		RAISERROR('Wholesale order does not exist', 16, 1);
		RETURN;
	END

	-- Look up ProductID from ProductNumber
	DECLARE @ProductID UNIQUEIDENTIFIER;
	
	SELECT @ProductID = ProductID
	FROM [Product]
	WHERE ProductNumber = @ProductNumber;
	
	IF @ProductID IS NULL
	BEGIN
		RAISERROR('Product with ProductNumber %s not found', 16, 1, @ProductNumber);
		RETURN;
	END

	-- Validate product exists in Wholesale order
	IF NOT EXISTS(SELECT 1 FROM ProductWholesaleOrder 
	              WHERE WholesaleOrderID = @WholesaleOrderID AND ProductID = @ProductID)
	BEGIN
		RAISERROR('Product %s is not in this Wholesale order', 16, 1, @ProductNumber);
		RETURN;
	END

	-- Get current received quantity
	DECLARE @ReceivedQuantity INT;
	DECLARE @OrderedQuantity INT;
	
	SELECT 
		@ReceivedQuantity = ISNULL(QuantityReceived, 0),
		@OrderedQuantity = Quantity
	FROM ProductWholesaleOrder
	WHERE WholesaleOrderID = @WholesaleOrderID AND ProductID = @ProductID;


	-- Update stock based on received quantity
	UPDATE [Product]
	SET NumberInStock = NumberInStock + @Quantity
	WHERE ProductID = @ProductID;

	-- Update quantity received in Wholesale order
	UPDATE ProductWholesaleOrder
	SET QuantityReceived = QuantityReceived + @Quantity
	WHERE WholesaleOrderID = @WholesaleOrderID AND ProductID = @ProductID;

	-- Check if all products are fully received
	DECLARE @PendingItems INT;
	
	SELECT @PendingItems = COUNT(*)
	FROM ProductWholesaleOrder
	WHERE WholesaleOrderID = @WholesaleOrderID
	AND QuantityReceived < Quantity;

	-- Update order status based on completion
	IF @PendingItems = 0
	BEGIN
		UPDATE WholesaleOrder
		SET OrderStatus = 'Modtaget', 
		    DeliveryDate = GETDATE()
		WHERE WholesaleOrderID = @WholesaleOrderID;
	END
	ELSE
	BEGIN
		UPDATE WholesaleOrder
		SET OrderStatus = 'Delvist Modtaget'
		WHERE WholesaleOrderID = @WholesaleOrderID;
	END
END;
GO

CREATE OR ALTER TRIGGER trg_AfterInsert_ProductWholesaleOrder
ON ProductWholesaleOrder
AFTER UPDATE
AS
BEGIN
	SET NOCOUNT ON;


END;