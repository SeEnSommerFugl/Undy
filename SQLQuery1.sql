USE Undy;
GO

-- Opret tabelstruktur for lagerstyringssystem
CREATE TABLE Stock(
	StockID UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
	StockLocation NVARCHAR(255) NOT NULL,
	NumberInStock INT NOT NULL,
	StockStatus NvarChar(255) NOT NULL,
);

CREATE TABLE [Product](
	ProductID UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
	ProductNumber INT NOT NULL,
	ProductName NVARCHAR(255) NOT NULL,
	Price DECIMAL(10,2) NOT NULL,
	Size NVARCHAR(255) NOT NULL,
	Colour NVARCHAR(255) NOT NULL,
	StockID UNIQUEIDENTIFIER NOT NULL
	CONSTRAINT FK_Stock_Product
		FOREIGN KEY(StockID)
		REFERENCES Stock(StockID)
);

CREATE TABLE PurchaseOrder(
	PurchaseOrderID UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
	PurchaseOrderDate DATE NOT NULL,
	ExpectedDeliveryDate DATE NOT NULL,
	DeliveryDate DATE NULL,
	OrderStatus NVARCHAR(255) NOT NULL

);

CREATE TABLE ProductPurchaseOrder(
	PurchaseOrderID UNIQUEIDENTIFIER NOT NULL,
	ProductID UNIQUEIDENTIFIER NOT NULL,
	Quantity INT NOT NULL,
	UnitPrice DECIMAL(10,2) NOT NULL,
	QuantityReceived INT NOT NULL
	CONSTRAINT PK_ProductPurchaseOrder PRIMARY KEY(PurchaseOrderID, ProductID),
	CONSTRAINT FK_PurchaseOrder_ProductPurchaseOrder
		FOREIGN KEY(PurchaseOrderID)
		REFERENCES PurchaseOrder(PurchaseOrderID),
	CONSTRAINT FK_Product_ProductPurchaseOrder
		FOREIGN KEY(ProductID)
		REFERENCES [Product](ProductID)
);

CREATE TABLE SalesOrder (
	SalesOrderID UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
	OrderNumber INT IDENTITY(0000000001,1) NOT NULL,
	OrderStatus NvarChar(255) NOT NULL,
	PaymentStatus NvarChar(255) NOT NULL,
	SalesDate DATE NOT NULL,
	TotalPrice DECIMAL(10,2) NOT NULL
);

CREATE TABLE ProductSalesOrder (
	SalesOrderID UNIQUEIDENTIFIER NOT NULL,
	ProductID UNIQUEIDENTIFIER NOT NULL,
	Quantity INT NOT NULL,
	UnitPrice DECIMAL NOT NULL,
	CONSTRAINT PK_ProductSalesOrder PRIMARY KEY(SalesOrderID, ProductID),
	CONSTRAINT FK_SalesOrder_ProductSalesOrder
		FOREIGN KEY(SalesOrderID)
		REFERENCES SalesOrder(SalesOrderID),
	CONSTRAINT FK_Product_ProductSalesOrder
		FOREIGN KEY(ProductID)
		REFERENCES [Product](ProductID)
);

CREATE TABLE ReturnOrder(
	ReturnOrderID INT IDENTITY(0000000001,1) NOT NULL PRIMARY KEY,
	ReturnOrderDate DATE NOT NULL,
	SalesOrderID UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT FK_SalesOrder_ReturnOrder
		FOREIGN KEY(SalesOrderID)
		REFERENCES SalesOrder(SalesOrderID)
);

CREATE TABLE Customers(
	CustomerID int IDENTITY(00000001,1) NOT NULL PRIMARY KEY,
	FirstName NVARCHAR(255) NOT NULL,
	LastName NVARCHAR(255) NOT NULL,
	Email NVARCHAR(255) NOT NULL,
	PhoneNumber NVARCHAR(50) NOT NULL,
	Address NVARCHAR(255) NOT NULL,
	City NVARCHAR(100) NOT NULL,
	PostalCode NVARCHAR(20) NOT NULL,
	Country NVARCHAR(100) NOT NULL
);
GO


-- Inserts til Stock

CREATE PROCEDURE usp_Insert_Stock
	@NumberInStock INT,
	@StockStatus NVARCHAR(255)
AS
BEGIN
SET NOCOUNT ON;
	INSERT INTO Stock(NumberInStock, StockStatus)
	VALUES(@NumberInStock, @StockStatus);
END;
GO

-- Inserts til Products

CREATE PROCEDURE usp_Insert_Product
	@ProductNumber INT,
	@ProductName NVARCHAR(255),
	@Price DECIMAL(10,2),
	@Size NVARCHAR(255),
	@Colour NVARCHAR(255),
	@StockID UNIQUEIDENTIFIER
AS
BEGIN
SET NOCOUNT ON;
	INSERT INTO [Product](ProductNumber, ProductName, Price, Size, Colour, StockID)
	VALUES(@ProductNumber, @ProductName, @Price, @Size, @Colour, @StockID);
END;
GO

-- Inserts til PurchaseOrder

CREATE PROCEDURE usp_Insert_PurchaseOrder
	@PurchaseOrderDate DATE,
	@ExpectedDeliveryDate DATE,
	@OrderStatus NVARCHAR(255)
AS
BEGIN
SET NOCOUNT ON;
	INSERT INTO PurchaseOrder(PurchaseOrderDate, ExpectedDeliveryDate, OrderStatus)
	VALUES(@PurchaseOrderDate, @ExpectedDeliveryDate, @OrderStatus);
END;
GO

-- Inserts til ProductPurchaseOrder

CREATE PROCEDURE usp_Insert_ProductPurchaseOrder
	@PurchaseOrderID UNIQUEIDENTIFIER,
	@ProductID UNIQUEIDENTIFIER,
	@Quantity INT,
	@UnitPrice DECIMAL(10,2)
AS
BEGIN
SET NOCOUNT ON;
	INSERT INTO ProductPurchaseOrder(PurchaseOrderID, ProductID, Quantity, UnitPrice)
	VALUES(@PurchaseOrderID, @ProductID, @Quantity, @UnitPrice);
END;
GO

-- Inserts til SalesOrder

CREATE PROCEDURE usp_Insert_SalesOrder
	@OrderStatus NVARCHAR(255),
	@PaymentStatus NVARCHAR(255),
	@SalesDate DATE,
	@TotalPrice DECIMAL(10,2)
AS
BEGIN
SET NOCOUNT ON;
	INSERT INTO SalesOrder(OrderStatus, PaymentStatus, SalesDate, TotalPrice)
	VALUES(@OrderStatus, @PaymentStatus, @SalesDate, @TotalPrice);
END;
GO

-- Inserts til ProductSalesOrder

CREATE PROCEDURE usp_Insert_ProductSalesOrder
	@SalesOrderID UNIQUEIDENTIFIER,
	@ProductID UNIQUEIDENTIFIER,
	@Quantity INT,
	@UnitPrice DECIMAL(10,2)
AS
BEGIN
SET NOCOUNT ON;
	INSERT INTO ProductSalesOrder(SalesOrderID, ProductID, Quantity, UnitPrice)
	VALUES(@SalesOrderID, @ProductID, @Quantity, @UnitPrice);
END;
GO

-- Inserts til ReturnOrder

CREATE PROCEDURE usp_Insert_ReturnOrder
	@ReturnOrderDate DATE,
	@SalesOrderID UNIQUEIDENTIFIER
AS
BEGIN
SET NOCOUNT ON;
	INSERT INTO ReturnOrder(ReturnOrderDate, SalesOrderID)
	VALUES(@ReturnOrderDate, @SalesOrderID);
END;
GO

-- Inserts til Customers

CREATE PROCEDURE usp_Insert_Customer
	@FirstName NVARCHAR(255),
	@LastName NVARCHAR(255),
	@Email NVARCHAR(255),
	@PhoneNumber NVARCHAR(50),
	@Address NVARCHAR(255),
	@City NVARCHAR(100),
	@PostalCode NVARCHAR(20),
	@Country NVARCHAR(100)
AS
BEGIN
SET NOCOUNT ON;
	INSERT INTO Customers(FirstName, LastName, Email, PhoneNumber, [Address], City, PostalCode, Country)
	VALUES(@FirstName, @LastName, @Email, @PhoneNumber, @Address, @City, @PostalCode, @Country);
END;
GO

-- View til at vise den aktuelle lagerbeholdning for produktkatalog
CREATE VIEW vw_Stock AS
SELECT p.ProductName, p.ProductNumber, p.Price, p.Size, p.Colour, s.NumberInStock, s.StockStatus
FROM Stock s
JOIN [Product] p ON s.StockID = p.StockID
GO

-- View til at vise indkøbsordre
CREATE VIEW vw_PurchaseOrders AS
SELECT po.PurchaseOrderID, po.PurchaseOrderDate, po.ExpectedDeliveryDate, po.DeliveryDate, po.OrderStatus, p.ProductName, p.Size, p.Colour, ppo.Quantity
FROM PurchaseOrder po
JOIN ProductPurchaseOrder ppo ON po.PurchaseOrderID = ppo.PurchaseOrderID
JOIN [Product] p ON ppo.ProductID = p.ProductID;
GO

-- View til at vise salgsordre
CREATE VIEW vw_SalesOrders AS
SELECT so.SalesOrderID, so.OrderNumber, so.OrderStatus, so.PaymentStatus, so.SalesDate, so.TotalPrice, p.ProductName, pso.Quantity
FROM SalesOrder so
JOIN ProductSalesOrder pso ON so.SalesOrderID = pso.SalesOrderID
JOIN [Product] p ON pso.ProductID = p.ProductID;
GO

CREATE VIEW vw_ReturnOrders AS
SELECT ro.ReturnOrderID, ro.ReturnOrderDate, so.OrderNumber, so.TotalPrice
FROM ReturnOrder ro
JOIN SalesOrder so ON ro.SalesOrderID = so.SalesOrderID;
GO

-- Stored Procedure til varemodtagelse af inkøbsordre og opdatering af lagerbeholdning
CREATE PROCEDURE usp_Update_PurchaseOrder @PurchaseOrderID UNIQUEIDENTIFIER AS
BEGIN
	-- Errorhandling hvis ordre ikke findes
	IF NOT EXISTS(SELECT 1 FROM PurchaseOrder WHERE PurchaseOrderID = @PurchaseOrderID)
	BEGIN
		RAISERROR('Indkøbsordre findes ikke', 16, 1);
		RETURN;
	END

	IF NOT EXISTS(SELECT 1 FROM PurchaseOrder WHERE PurchaseOrderID = @PurchaseOrderID AND OrderStatus = 'Modtaget')
	BEGIN
		RAISERROR('Varer for denne ordre er allerede modtaget', 16, 1);
		RETURN;
	END

	-- Opdaterer lagerbeholdning for hver vare i ordren
	UPDATE s
	SET s.NumberInStock = s.NumberInStock + ppo.Quantity
	FROM Stock s
	JOIN [Product] p ON s.StockID = p.StockID
	JOIN ProductPurchaseOrder ppo ON p.ProductID = ppo.ProductID
	WHERE ppo.PurchaseOrderID = @PurchaseOrderID

	-- Opdaterer ordrestatus og modtagelsesdato
	UPDATE PurchaseOrder
	SET
		DeliveryDate = GETDATE(),
		OrderStatus = 'Modtaget'
	WHERE PurchaseOrderID = @PurchaseOrderID
END;
GO

-- TODO Tilføj en QuantityReceived kolonne til ProductPurchaseOrder
-- Stored Procedure til delvis modtagelse af indkøbsordre
CREATE PROCEDURE usp_UpdatePartial_PurchaseOrder @PurchaseOrderID UNIQUEIDENTIFIER, @ProductID UNIQUEIDENTIFIER, @Quantity INT AS
BEGIN
	-- Errorhandling hvis indkøbsordre ikke findes
	IF NOT EXISTS(SELECT 1 FROM PurchaseOrder WHERE PurchaseOrderID = @PurchaseOrderID)
	BEGIN
		RAISERROR('Indkøbsordre findes ikke', 16, 1);
		RETURN;
	END

	-- Errorhandling hvis produkt ikke findes i indkøbsordren
	IF NOT EXISTS(SELECT 1 FROM ProductPurchaseOrder WHERE PurchaseOrderID = @PurchaseOrderID AND ProductID = @ProductID)
	BEGIN
		RAISERROR('Produkt findes ikke i ordren', 16, 1);
		RETURN;
	END

	-- Tjek om levering allerede findes for produkt i ordren
	DECLARE @ReceivedQuantity INT;
	SELECT @ReceivedQuantity = ISNULL(QuantityReceived, 0)
	FROM ProductPurchaseOrder
	WHERE PurchaseOrderID = @PurchaseOrderID AND ProductID = @ProductID AND QuantityReceived IS NOT NULL;

	-- Hvis ingen produkter er modtaget, tillad fuld modtagelse
	IF @ReceivedQuantity IS NULL
	BEGIN
		SET @ReceivedQuantity = 0;
	END

	-- Henter den bestilte mængde
	DECLARE @OrderedQuantity INT;
	SELECT @OrderedQuantity = Quantity
	FROM ProductPurchaseOrder
	WHERE PurchaseOrderID = @PurchaseOrderID AND ProductID = @ProductID;

	-- Errorhandling hvis mængde der modtages er større end mængden i indkøbsordren
	IF(@ReceivedQuantity + @Quantity) > @OrderedQuantity
	BEGIN
		RAISERROR('Modtaget mængde overskrider ordremængde', 16, 1);
		RETURN;
	END

	-- Opdaterer lagerbeholdning baseret på modtaget mængde
	UPDATE s
	SET s.NumberInStock = s.NumberInStock + @Quantity
	FROM Stock s
	JOIN Product p ON s.StockID = p.StockID
	WHERE p.ProductID = @ProductID;

	-- Opdaterer mængden modtaget i indkøbsordre
	UPDATE ProductPurchaseOrder
	SET QuantityReceived = ISNULL(QuantityReceived, 0) + @Quantity
	WHERE PurchaseOrderID = @PurchaseOrderID AND ProductID = @ProductID;

	-- Opdaterer ordrerstatus hvis alle produkter er modtaget
	DECLARE @AllReceived INT;
	SELECT @AllReceived = COUNT(*)
	FROM ProductPurchaseOrder
	WHERE PurchaseOrderID = @PurchaseOrderID
	AND(QuantityReceived IS NULL OR QuantityReceived < Quantity)

	-- Hvis alle produkter er modtaget, opdater ordrestatus
	IF @AllReceived = 0
	BEGIN
		UPDATE PurchaseOrder
		SET OrderStatus = 'Modtaget', DeliveryDate = GETDATE()
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

-- Stored Procedure SalgsOrdre
CREATE PROCEDURE usp_Update_SalesOrder @SalesOrderID UNIQUEIDENTIFIER AS
BEGIN

		--Errorhandling hvis ordre ikke findes
	IF NOT EXISTS(SELECT 1 FROM SalesOrder WHERE SalesOrderID = (@SalesOrderID))
	BEGIN
		RAISERROR('Salgsordre findes ikke', 16, 1);
		RETURN;
	END
		--Tjek om ordre allerede er modtaget
	IF EXISTS (SELECT 1 FROM SalesOrder WHERE SalesOrderID = @SalesOrderID AND OrderStatus = 'Modtaget')
	BEGIN
		RAISERROR('Denne salgsordre er allerede modtaget', 16, 1);
		RETURN;
	END
	--Tjek om der er varer nok på lager
	IF EXISTS (SELECT 1 FROM ProductSalesOrder pso
	JOIN [Product] p ON pso.ProductID = p.ProductID
	JOIN Stock s ON p.StockID = s.StockID
	WHERE pso.SalesOrderID = @SalesOrderID
		AND s.NumberInStock < pso.Quantity)

	BEGIN
		RAISERROR('Der er ikke nok varer på lager', 16, 1);
		RETURN;
	END;

	-- Opdater lager med solgte varer
	UPDATE s
	SET s.NumberInStock = s.NumberInStock - pso.Quantity
	FROM Stock s
	JOIN [Product] p ON s.StockID = p.StockID
	JOIN ProductSalesOrder pso ON p.ProductID = pso.ProductID
	WHERE pso.SalesOrderID = @SalesOrderID;

	--Beregn totalpris og opdater salgsordre
	DECLARE @TotalPrice DECIMAL(10,2);

	SELECT @TotalPrice = SUM(pso.Quantity * pso.UnitPrice)
	FROM ProductSalesOrder pso
	WHERE pso.SalesOrderID = @SalesOrderID

	UPDATE SalesOrder
	SET TotalPrice		= @TotalPrice,
		OrderStatus		= 'Modtaget',
		PaymentStatus	= 'Betalt'
	WHERE SalesOrderID	= @SalesOrderID;
END;
GO

-- Stored Procedure ReturOrdre

CREATE PROCEDURE usp_Update_ReturnOrder @ReturnOrderID UNIQUEIDENTIFIER AS
BEGIN

	--Tjek om returordre findes
	IF NOT EXISTS (SELECT 1 FROM ReturnOrder WHERE ReturnOrderID = @ReturnOrderID)
	BEGIN
		RAISERROR('Returordre findes ikke', 16, 1);
		RETURN;
	END;

	--Find salgsordre der passer til
	DECLARE @SalesOrdreID UNIQUEIDENTIFIER;
	SELECT @SalesOrdreID = SalesOrderID
	FROM ReturnOrder
	WHERE ReturnOrderID = @ReturnOrderID;

	--Tjek om salgsordre findes
	IF NOT EXISTS (SELECT 1 FROM SalesOrder WHERE SalesOrderID = @SalesOrdreID)
	BEGIN
		RAISERROR(' Passende salgsordre findes ikke', 16, 1);
		RETURN;
	END;

	--Tjek om salgsordre allerede er returneret
	IF EXISTS( 
	SELECT 1 
	FROM SalesOrder 
	WHERE SalesOrderID = @SalesOrdreID
		AND OrderStatus = 'Returneret'
	)
	BEGIN 
		RAISERROR('Denne ordre er allerede returneret', 16, 1);
		RETURN;
	END;

	--Returner varer på lager fra salgsordren
	UPDATE s
	SET s.NumberInStock = s. NumberInStock + pso.Quantity
	FROM Stock s
	JOIN [Product] p ON s.StockID = p.StockID
	JOIN  ProductSalesOrder pso ON p.ProductID = pso.ProductID
	WHERE pso.SalesOrderID = @SalesOrdreID;

	--Opdater salgsordre status
	UPDATE SalesOrder
	SET OrderStatus		= 'Returneret',
		PaymentStatus	= 'Returneret'
	WHERE SalesOrderID	= @SalesOrdreID;

END;
GO

-- Stored Procedure til updates

CREATE PROCEDURE usp_Update_Product
	@ProductID UNIQUEIDENTIFIER,
	@ProductNumber INT = NULL,
	@ProductName NVARCHAR(255) = NULL,
	@Price DECIMAL(10,2) = NULL,
	@Size NVARCHAR(255) = NULL,
	@Colour NVARCHAR(255) = NULL
AS
BEGIN
	--Tjek om produkt findes
	IF NOT EXISTS (
	SELECT 1 
	FROM [Product] 
	WHERE ProductID = @ProductID
	)
	BEGIN
		RAISERROR('Produkt findes ikke', 16, 1);
		RETURN
	END;
	--Updaterer produktet
	UPDATE [Product]
	SET ProductNumber	= ISNULL(@ProductNumber,	ProductNumber),
		ProductName		= ISNULL(@ProductName,		ProductName),
		Price			= ISNULL(@Price,			Price),
		Size			= ISNULL(@Size,				Size),
		Colour			= ISNULL(@Colour,			Colour)
	WHERE ProductID = @ProductID;
END;
GO

-- Stored Procedure til updates af lager
CREATE PROCEDURE usp_Update_Stock
	@StockID UNIQUEIDENTIFIER,
	@NumberInStock INT = NULL,
	@StockStatus NVARCHAR(255) = NULL
AS
BEGIN
	
	IF NOT EXISTS (SELECT 1 FROM Stock WHERE StockID = @StockID)
    BEGIN
        RAISERROR('Lagerpost findes ikke', 16, 1);
        RETURN;
    END;

    UPDATE Stock
    SET NumberInStock = ISNULL(@NumberInStock, NumberInStock),
        StockStatus   = ISNULL(@StockStatus,   StockStatus)
    WHERE StockID = @StockID;
END;