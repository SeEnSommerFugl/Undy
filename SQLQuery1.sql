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

-- PurchaseOrder Table
CREATE TABLE PurchaseOrder(
	PurchaseOrderID UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	PurchaseOrderDate DATE NOT NULL,
	ExpectedDeliveryDate DATE NOT NULL,
	DeliveryDate DATE NULL,
	OrderStatus NVARCHAR(255) NOT NULL
);

-- ProductPurchaseOrder
CREATE TABLE ProductPurchaseOrder(
	PurchaseOrderID UNIQUEIDENTIFIER NOT NULL,
	ProductID UNIQUEIDENTIFIER NOT NULL,
	Quantity INT NOT NULL,
	UnitPrice DECIMAL(10,2) NOT NULL,
	QuantityReceived INT NOT NULL DEFAULT 0,
	CONSTRAINT PK_ProductPurchaseOrder PRIMARY KEY(PurchaseOrderID, ProductID),
	CONSTRAINT FK_PurchaseOrder_ProductPurchaseOrder
		FOREIGN KEY(PurchaseOrderID)
		REFERENCES PurchaseOrder(PurchaseOrderID),
	CONSTRAINT FK_Product_ProductPurchaseOrder
		FOREIGN KEY(ProductID)
		REFERENCES [Product](ProductID)
);

-- SalesOrder Table
CREATE TABLE SalesOrder (
	SalesOrderID UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	OrderNumber INT IDENTITY(0000000001,1) NOT NULL,
	OrderStatus NVARCHAR(255) NOT NULL,
	PaymentStatus NVARCHAR(255) NOT NULL,
	SalesDate DATE NOT NULL,
	TotalPrice DECIMAL(10,2) NOT NULL
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
	ReturnOrderNumber INT IDENTITY(0000000001,1) NOT NULL UNIQUE,
	ReturnOrderDate DATE NOT NULL,
	SalesOrderID UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT FK_SalesOrder_ReturnOrder
		FOREIGN KEY(SalesOrderID)
		REFERENCES SalesOrder(SalesOrderID)
);

-- Customers Table
CREATE TABLE Customers(
	CustomerID UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	CustomerNumber INT IDENTITY(0000000001,1) NOT NULL UNIQUE,
	FirstName NVARCHAR(255) NOT NULL,
	LastName NVARCHAR(255) NOT NULL,
	Email NVARCHAR(255) NOT NULL UNIQUE,
	PhoneNumber NVARCHAR(50) NOT NULL,
	[Address] NVARCHAR(255) NOT NULL,
	City NVARCHAR(100) NOT NULL,
	PostalCode NVARCHAR(20) NOT NULL,
	Country NVARCHAR(100) NOT NULL
);
GO

-- ============================================================================
-- INSERT STORED PROCEDURES
-- ============================================================================

-- Insert Product
CREATE PROCEDURE usp_Insert_Product
	@ProductNumber NVARCHAR(255),
	@ProductName NVARCHAR(255),
	@Price DECIMAL(10,2),
	@Size NVARCHAR(255),
	@Colour NVARCHAR(255),
	@NumberInStock INT,
	@ProductID UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON;
	
	-- Generate new GUID
	SET @ProductID = NEWID();

	-- Validate ProductNumber doesn't already exist
	IF EXISTS(SELECT 1 FROM [Product] WHERE ProductNumber = @ProductNumber)
	BEGIN
		RAISERROR('Product with ProductNumber %s already exists', 16, 1, @ProductNumber);
		RETURN;
	END
	
	-- Validate price is positive
	IF @Price < 0
	BEGIN
		RAISERROR('Price cannot be negative', 16, 1);
		RETURN;
	END
	
	INSERT INTO [Product](ProductID, ProductNumber, ProductName, Price, Size, Colour, NumberInStock)
	VALUES(@ProductID, @ProductNumber, @ProductName, @Price, @Size, @Colour, @NumberInStock);
END;
GO

-- Insert PurchaseOrder
CREATE PROCEDURE usp_Insert_PurchaseOrder
	@PurchaseOrderDate DATE,
	@ExpectedDeliveryDate DATE,
	@OrderStatus NVARCHAR(255),
	@PurchaseOrderID UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON;
	
	-- Validate expected delivery is after order date
	IF @ExpectedDeliveryDate < @PurchaseOrderDate
	BEGIN
		RAISERROR('Expected delivery date cannot be before purchase order date', 16, 1);
		RETURN;
	END
	
	-- Generate new GUID
	SET @PurchaseOrderID = NEWID();
	
	INSERT INTO PurchaseOrder(PurchaseOrderID, PurchaseOrderDate, ExpectedDeliveryDate, OrderStatus)
	VALUES(@PurchaseOrderID, @PurchaseOrderDate, @ExpectedDeliveryDate, @OrderStatus);
END;
GO

-- Insert ProductPurchaseOrder
CREATE PROCEDURE usp_Insert_ProductPurchaseOrder
	@PurchaseOrderID UNIQUEIDENTIFIER,
	@ProductNumber NVARCHAR(255),
	@Quantity INT,
	@UnitPrice DECIMAL(10,2)
AS
BEGIN
	SET NOCOUNT ON;
	
	-- Validate PurchaseOrder exists
	IF NOT EXISTS(SELECT 1 FROM PurchaseOrder WHERE PurchaseOrderID = @PurchaseOrderID)
	BEGIN
		RAISERROR('Purchase Order does not exist', 16, 1);
		RETURN;
	END
	
	-- Look up ProductID from the user-friendly ProductNumber
	DECLARE @ProductID UNIQUEIDENTIFIER;
	
	SELECT @ProductID = ProductID
	FROM [Product]
	WHERE ProductNumber = @ProductNumber;
	
	-- Validate Product exists
	IF @ProductID IS NULL
	BEGIN
		RAISERROR('Product with ProductNumber %s not found', 16, 1, @ProductNumber);
		RETURN;
	END
	
	-- Check if this product is already in the purchase order
	IF EXISTS(SELECT 1 FROM ProductPurchaseOrder 
	          WHERE PurchaseOrderID = @PurchaseOrderID AND ProductID = @ProductID)
	BEGIN
		RAISERROR('Product %s is already in this purchase order', 16, 1, @ProductNumber);
		RETURN;
	END
	
	-- Validate quantity and price
	IF @Quantity < 0
	BEGIN
		RAISERROR('Quantity must be greater than 0', 16, 1);
		RETURN;
	END
	
	IF @UnitPrice < 0
	BEGIN
		RAISERROR('Unit price cannot be negative', 16, 1);
		RETURN;
	END

	INSERT INTO ProductPurchaseOrder(PurchaseOrderID, ProductID, Quantity, UnitPrice, QuantityReceived)
	VALUES(@PurchaseOrderID, @ProductID, @Quantity, @UnitPrice, 0);
END;
GO

-- Insert SalesOrder
CREATE PROCEDURE usp_Insert_SalesOrder
	@OrderStatus NVARCHAR(255),
	@PaymentStatus NVARCHAR(255),
	@SalesDate DATE,
	@TotalPrice DECIMAL(10,2),
	@SalesOrderID UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON;
	
	-- Generate new GUID
	SET @SalesOrderID = NEWID();
	
	INSERT INTO SalesOrder(SalesOrderID, OrderStatus, PaymentStatus, SalesDate, TotalPrice)
	VALUES(@SalesOrderID, @OrderStatus, @PaymentStatus, @SalesDate, @TotalPrice);
END;
GO

-- Insert ProductSalesOrder
CREATE PROCEDURE usp_Insert_ProductSalesOrder
	@SalesOrderID UNIQUEIDENTIFIER,
	@ProductNumber NVARCHAR(255),
	@Quantity INT,
	@UnitPrice DECIMAL(10,2)
AS
BEGIN
	SET NOCOUNT ON;
	
	-- Validate SalesOrder exists
	IF NOT EXISTS(SELECT 1 FROM SalesOrder WHERE SalesOrderID = @SalesOrderID)
	BEGIN
		RAISERROR('Sales Order does not exist', 16, 1);
		RETURN;
	END
	
	-- Look up ProductID from ProductNumber
	DECLARE @ProductID UNIQUEIDENTIFIER;
	
	SELECT @ProductID = ProductID
	FROM [Product]
	WHERE ProductNumber = @ProductNumber;
	
	-- Validate Product exists
	IF @ProductID IS NULL
	BEGIN
		RAISERROR('Product with ProductNumber %s not found', 16, 1, @ProductNumber);
		RETURN;
	END
	
	-- Check if product already in sales order
	IF EXISTS(SELECT 1 FROM ProductSalesOrder 
	          WHERE SalesOrderID = @SalesOrderID AND ProductID = @ProductID)
	BEGIN
		RAISERROR('Product %s is already in this sales order', 16, 1, @ProductNumber);
		RETURN;
	END
	
	-- Validate quantity and price
	IF @Quantity < 0
	BEGIN
		RAISERROR('Quantity must be greater than 0', 16, 1);
		RETURN;
	END
	
	IF @UnitPrice < 0
	BEGIN
		RAISERROR('Unit price cannot be negative', 16, 1);
		RETURN;
	END
	
	INSERT INTO ProductSalesOrder(SalesOrderID, ProductID, Quantity, UnitPrice)
	VALUES(@SalesOrderID, @ProductID, @Quantity, @UnitPrice);
END;
GO

-- Insert ReturnOrder
CREATE PROCEDURE usp_Insert_ReturnOrder
	@ReturnOrderDate DATE,
	@SalesOrderID UNIQUEIDENTIFIER,
	@ReturnOrderID UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON;

	SET @ReturnOrderID = NEWID();
	
	-- Validate SalesOrder exists
	IF NOT EXISTS(SELECT 1 FROM SalesOrder WHERE SalesOrderID = @SalesOrderID)
	BEGIN
		RAISERROR('Sales Order does not exist', 16, 1);
		RETURN;
	END
	
	-- Check if return already exists for this sales order
	IF EXISTS(SELECT 1 FROM ReturnOrder WHERE SalesOrderID = @SalesOrderID)
	BEGIN
		RAISERROR('A return order already exists for this sales order', 16, 1);
		RETURN;
	END
	
	INSERT INTO ReturnOrder(ReturnOrderID, ReturnOrderDate, SalesOrderID)
	VALUES(@ReturnOrderID, @ReturnOrderDate, @SalesOrderID);
END;
GO

-- Insert Customer
CREATE PROCEDURE usp_Insert_Customer
	@FirstName NVARCHAR(255),
	@LastName NVARCHAR(255),
	@Email NVARCHAR(255),
	@PhoneNumber NVARCHAR(50),
	@Address NVARCHAR(255),
	@City NVARCHAR(100),
	@PostalCode NVARCHAR(20),
	@Country NVARCHAR(100),
	@CustomerID UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON;
	
	-- Validate email doesn't already exist
	IF EXISTS(SELECT 1 FROM Customers WHERE Email = @Email)
	BEGIN
		RAISERROR('Customer with email %s already exists', 16, 1, @Email);
		RETURN;
	END
	
	-- Generate new GUID for CustomerID
	SET @CustomerID = NEWID();
	
	-- Insert customer
	INSERT INTO Customers(CustomerID, FirstName, LastName, Email, PhoneNumber, [Address], City, PostalCode, Country)
	VALUES(@CustomerID, @FirstName, @LastName, @Email, @PhoneNumber, @Address, @City, @PostalCode, @Country);
END;
GO

-- ============================================================================
-- VIEWS
-- ============================================================================

-- View Products
CREATE VIEW vw_Products
AS
SELECT 
	ProductID, 
	ProductNumber, 
	ProductName, 
	Price, 
	Size, 
	Colour, 
	NumberInStock
FROM [Product];
GO

-- View PurchaseOrders
CREATE VIEW vw_PurchaseOrders AS
SELECT 
	po.PurchaseOrderID, 
	po.PurchaseOrderDate, 
	po.ExpectedDeliveryDate, 
	po.DeliveryDate, 
	po.OrderStatus, 
	p.ProductID,
	p.ProductNumber,
	p.ProductName, 
	p.Size, 
	p.Colour, 
	ppo.Quantity,
	ppo.UnitPrice,
	ppo.QuantityReceived,
	(ppo.Quantity * ppo.UnitPrice) AS LineTotal
FROM PurchaseOrder po
JOIN ProductPurchaseOrder ppo ON po.PurchaseOrderID = ppo.PurchaseOrderID
JOIN [Product] p ON ppo.ProductID = p.ProductID;
GO

-- View SalesOrders
CREATE VIEW vw_SalesOrders AS
SELECT 
	so.SalesOrderID, 
	so.OrderNumber, 
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
FROM SalesOrder so
JOIN ProductSalesOrder pso ON so.SalesOrderID = pso.SalesOrderID
JOIN [Product] p ON pso.ProductID = p.ProductID;
GO

-- View ReturnOrders
CREATE VIEW vw_ReturnOrders AS
SELECT 
	ro.ReturnOrderID, 
	ro.ReturnOrderDate, 
	so.SalesOrderID,
	so.OrderNumber, 
	so.TotalPrice,
	so.OrderStatus,
	so.PaymentStatus
FROM ReturnOrder ro
JOIN SalesOrder so ON ro.SalesOrderID = so.SalesOrderID;
GO

-- View Customers
CREATE VIEW vw_Customers AS
SELECT 
	CustomerID,
	CustomerNumber,
	FirstName,
	LastName,
	Email,
	PhoneNumber,
	[Address],
	City,
	PostalCode,
	Country,
	FirstName + ' ' + LastName AS FullName
FROM Customers;
GO

-- ============================================================================
-- UPDATE STORED PROCEDURES
-- ============================================================================

-- Update PurchaseOrder
CREATE PROCEDURE usp_Update_PurchaseOrder 
	@PurchaseOrderID UNIQUEIDENTIFIER 
AS
BEGIN
	SET NOCOUNT ON;
	
	-- Validate purchase order exists
	IF NOT EXISTS(SELECT 1 FROM PurchaseOrder WHERE PurchaseOrderID = @PurchaseOrderID)
	BEGIN
		RAISERROR('Purchase order does not exist', 16, 1);
		RETURN;
	END

	-- Check if order is already received
	IF EXISTS(SELECT 1 FROM PurchaseOrder WHERE PurchaseOrderID = @PurchaseOrderID AND OrderStatus = 'Modtaget')
	BEGIN
		RAISERROR('This purchase order has already been fully received', 16, 1);
		RETURN;
	END

	-- Update stock for each product in the order
	UPDATE p
	SET p.NumberInStock = p.NumberInStock + ppo.Quantity
	FROM [Product] p
	JOIN ProductPurchaseOrder ppo ON p.ProductID = ppo.ProductID
	WHERE ppo.PurchaseOrderID = @PurchaseOrderID;

	-- Update order status and delivery date
	UPDATE PurchaseOrder
	SET
		DeliveryDate = GETDATE(),
		OrderStatus = 'Modtaget'
	WHERE PurchaseOrderID = @PurchaseOrderID;
	
	-- Update all products as fully received
	UPDATE ProductPurchaseOrder
	SET QuantityReceived = Quantity
	WHERE PurchaseOrderID = @PurchaseOrderID;
END;
GO

-- Update Partial PurchaseOrder
CREATE PROCEDURE usp_UpdatePartial_PurchaseOrder 
	@PurchaseOrderID UNIQUEIDENTIFIER, 
	@ProductNumber NVARCHAR(255),
	@Quantity INT 
AS
BEGIN
	SET NOCOUNT ON;
	
	-- Validate purchase order exists
	IF NOT EXISTS(SELECT 1 FROM PurchaseOrder WHERE PurchaseOrderID = @PurchaseOrderID)
	BEGIN
		RAISERROR('Purchase order does not exist', 16, 1);
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

	-- Validate product exists in purchase order
	IF NOT EXISTS(SELECT 1 FROM ProductPurchaseOrder 
	              WHERE PurchaseOrderID = @PurchaseOrderID AND ProductID = @ProductID)
	BEGIN
		RAISERROR('Product %s is not in this purchase order', 16, 1, @ProductNumber);
		RETURN;
	END

	-- Get current received quantity
	DECLARE @ReceivedQuantity INT;
	DECLARE @OrderedQuantity INT;
	
	SELECT 
		@ReceivedQuantity = ISNULL(QuantityReceived, 0),
		@OrderedQuantity = Quantity
	FROM ProductPurchaseOrder
	WHERE PurchaseOrderID = @PurchaseOrderID AND ProductID = @ProductID;


	-- Update stock based on received quantity
	UPDATE [Product]
	SET NumberInStock = NumberInStock + @Quantity
	WHERE ProductID = @ProductID;

	-- Update quantity received in purchase order
	UPDATE ProductPurchaseOrder
	SET QuantityReceived = QuantityReceived + @Quantity
	WHERE PurchaseOrderID = @PurchaseOrderID AND ProductID = @ProductID;

	-- Check if all products are fully received
	DECLARE @PendingItems INT;
	
	SELECT @PendingItems = COUNT(*)
	FROM ProductPurchaseOrder
	WHERE PurchaseOrderID = @PurchaseOrderID
	AND QuantityReceived < Quantity;

	-- Update order status based on completion
	IF @PendingItems = 0
	BEGIN
		UPDATE PurchaseOrder
		SET OrderStatus = 'Modtaget', 
		    DeliveryDate = GETDATE()
		WHERE PurchaseOrderID = @PurchaseOrderID;
	END
	ELSE
	BEGIN
		UPDATE PurchaseOrder
		SET OrderStatus = 'Delvist Modtaget'
		WHERE PurchaseOrderID = @PurchaseOrderID;
	END
END;
GO

-- Update SalesOrder
CREATE PROCEDURE usp_Update_SalesOrder 
	@SalesOrderID UNIQUEIDENTIFIER 
AS
BEGIN
	SET NOCOUNT ON;

	-- Validate order exists
	IF NOT EXISTS(SELECT 1 FROM SalesOrder WHERE SalesOrderID = @SalesOrderID)
	BEGIN
		RAISERROR('Sales order does not exist', 16, 1);
		RETURN;
	END
	
	-- Check if order already completed
	IF EXISTS (SELECT 1 FROM SalesOrder 
	           WHERE SalesOrderID = @SalesOrderID AND OrderStatus = 'Afsluttet')
	BEGIN
		RAISERROR('This sales order is already completed', 16, 1);
		RETURN;
	END
	
	-- Check if sufficient stock exists for all items
	IF EXISTS (
		SELECT 1 
		FROM ProductSalesOrder pso
		JOIN [Product] p ON pso.ProductID = p.ProductID
		WHERE pso.SalesOrderID = @SalesOrderID
		AND p.NumberInStock < pso.Quantity
	)
	BEGIN
		RAISERROR('Insufficient stock for one or more products', 16, 1);
		RETURN;
	END

	-- Deduct sold items from stock
	UPDATE p
	SET p.NumberInStock = p.NumberInStock - pso.Quantity
	FROM [Product] p
	JOIN ProductSalesOrder pso ON p.ProductID = pso.ProductID
	WHERE pso.SalesOrderID = @SalesOrderID;

	-- Calculate total price
	DECLARE @TotalPrice DECIMAL(10,2);

	SELECT @TotalPrice = SUM(pso.Quantity * pso.UnitPrice)
	FROM ProductSalesOrder pso
	WHERE pso.SalesOrderID = @SalesOrderID;

	-- Update sales order
	UPDATE SalesOrder
	SET 
		TotalPrice = @TotalPrice,
		OrderStatus = 'Afsluttet',
		PaymentStatus = 'Betalt'
	WHERE SalesOrderID = @SalesOrderID;
END;
GO

-- Update ReturnOrder
CREATE PROCEDURE usp_Update_ReturnOrder 
	@ReturnOrderID UNIQUEIDENTIFIER 
AS
BEGIN
	SET NOCOUNT ON;

	-- Validate return order exists
	IF NOT EXISTS (SELECT 1 FROM ReturnOrder WHERE ReturnOrderID = @ReturnOrderID)
	BEGIN
		RAISERROR('Return order does not exist', 16, 1);
		RETURN;
	END

	-- Get the associated sales order
	DECLARE @SalesOrderID UNIQUEIDENTIFIER;
	
	SELECT @SalesOrderID = SalesOrderID
	FROM ReturnOrder
	WHERE ReturnOrderID = @ReturnOrderID;

	-- Validate sales order exists
	IF NOT EXISTS (SELECT 1 FROM SalesOrder WHERE SalesOrderID = @SalesOrderID)
	BEGIN
		RAISERROR('Associated sales order does not exist', 16, 1);
		RETURN;
	END

	-- Check if already returned
	IF EXISTS(
		SELECT 1 
		FROM SalesOrder 
		WHERE SalesOrderID = @SalesOrderID
		AND OrderStatus = 'Returneret'
	)
	BEGIN 
		RAISERROR('This order has already been returned', 16, 1);
		RETURN;
	END

	-- Return items to stock
	UPDATE p
	SET p.NumberInStock = p.NumberInStock + pso.Quantity
	FROM [Product] p
	JOIN ProductSalesOrder pso ON p.ProductID = pso.ProductID
	WHERE pso.SalesOrderID = @SalesOrderID;

	-- Update sales order status
	UPDATE SalesOrder
	SET 
		OrderStatus = 'Returneret',
		PaymentStatus = 'Refunderet'
	WHERE SalesOrderID = @SalesOrderID;
END;
GO

-- Update Product
CREATE PROCEDURE usp_Update_Product
	@ProductNumber NVARCHAR(255),
	@NewProductNumber NVARCHAR(255) = NULL,
	@ProductName NVARCHAR(255) = NULL,
	@Price DECIMAL(10,2) = NULL,
	@Size NVARCHAR(255) = NULL,
	@Colour NVARCHAR(255) = NULL,
	@NumberInStock INT = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	-- Look up ProductID
	DECLARE @ProductID UNIQUEIDENTIFIER;
	
	SELECT @ProductID = ProductID
	FROM [Product]
	WHERE ProductNumber = @ProductNumber;
	
	-- Validate product exists
	IF @ProductID IS NULL
	BEGIN
		RAISERROR('Product with ProductNumber %s does not exist', 16, 1, @ProductNumber);
		RETURN;
	END
	
	-- If changing ProductNumber, check it's not already taken
	IF @NewProductNumber IS NOT NULL AND @NewProductNumber != @ProductNumber
	BEGIN
		IF EXISTS(SELECT 1 FROM [Product] WHERE ProductNumber = @NewProductNumber)
		BEGIN
			RAISERROR('ProductNumber %s is already in use', 16, 1, @NewProductNumber);
			RETURN;
		END
	END
	
	-- Validate price if provided
	IF @Price IS NOT NULL AND @Price < 0
	BEGIN
		RAISERROR('Price cannot be negative', 16, 1);
		RETURN;
	END
	
	-- Update product
	UPDATE [Product]
	SET 
		ProductNumber = ISNULL(@NewProductNumber, ProductNumber),
		ProductName = ISNULL(@ProductName, ProductName),
		Price = ISNULL(@Price, Price),
		Size = ISNULL(@Size, Size),
		Colour = ISNULL(@Colour, Colour),
		NumberInStock = ISNULL(@NumberInStock, NumberInStock)
	WHERE ProductID = @ProductID;
END;
GO

-- Update Customer - Uses CustomerNumber to identify customer
CREATE PROCEDURE usp_Update_Customer
	@CustomerNumber INT,
	@FirstName NVARCHAR(255) = NULL,
	@LastName NVARCHAR(255) = NULL,
	@Email NVARCHAR(255) = NULL,
	@PhoneNumber NVARCHAR(50) = NULL,
	@Address NVARCHAR(255) = NULL,
	@City NVARCHAR(100) = NULL,
	@PostalCode NVARCHAR(20) = NULL,
	@Country NVARCHAR(100) = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	-- Look up CustomerID
	DECLARE @CustomerID UNIQUEIDENTIFIER;
	
	SELECT @CustomerID = CustomerID
	FROM Customers
	WHERE CustomerNumber = @CustomerNumber;
	
	-- Validate customer exists
	IF @CustomerID IS NULL
	BEGIN
		RAISERROR('Customer with CustomerNumber %s does not exist', 16, 1, @CustomerNumber);
		RETURN;
	END
	
	-- If changing email, check it's not already taken
	IF @Email IS NOT NULL
	BEGIN
		IF EXISTS(SELECT 1 FROM Customers WHERE Email = @Email AND CustomerID != @CustomerID)
		BEGIN
			RAISERROR('Email %s is already in use by another customer', 16, 1, @Email);
			RETURN;
		END
	END
	
	-- Update customer
	UPDATE Customers
	SET 
		FirstName = ISNULL(@FirstName, FirstName),
		LastName = ISNULL(@LastName, LastName),
		Email = ISNULL(@Email, Email),
		PhoneNumber = ISNULL(@PhoneNumber, PhoneNumber),
		[Address] = ISNULL(@Address, [Address]),
		City = ISNULL(@City, City),
		PostalCode = ISNULL(@PostalCode, PostalCode),
		Country = ISNULL(@Country, Country)
	WHERE CustomerID = @CustomerID;
END;
GO

-- Get Customer by CustomerNumber
CREATE PROCEDURE usp_Get_CustomerByNumber
	@CustomerNumber INT
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT 
		CustomerID,
		CustomerNumber,
		FirstName,
		LastName,
		Email,
		PhoneNumber,
		[Address],
		City,
		PostalCode,
		Country
	FROM Customers
	WHERE CustomerNumber = @CustomerNumber;
END;
GO

-- Get Customer by Email
CREATE PROCEDURE usp_Get_CustomerByEmail
	@Email NVARCHAR(255)
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT 
		CustomerID,
		CustomerNumber,
		FirstName,
		LastName,
		Email,
		PhoneNumber,
		[Address],
		City,
		PostalCode,
		Country
	FROM Customers
	WHERE Email = @Email;
END;
GO