CREATE TABLE ProductCatalogue(
	ProductCatalogueID UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	InStock INT NOT NULL,
	MinimumInStock INT NOT NULL,
	StockStatus NVARCHAR(255) NOT NULL,
);

CREATE TABLE [Product](
	ProductID UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	ProductNumber INT NOT NULL,
	ProductName NVARCHAR(255) NOT NULL,
	Price DECIMAL(10,2) NOT NULL,
	Size NVARCHAR(255) NOT NULL,
	Colour NVARCHAR(255) NOT NULL,
	ProductCatalogueID UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT FK_ProductCatalogue_Product
		FOREIGN KEY(ProductCatalogueID)
		REFERENCES ProductCatalogue(ProductCatalogueID)
);

CREATE TABLE PurchaseOrder(
	PurchaseOrderID UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	PurchaseOrderDate DATE NOT NULL,
	ExpectedDeliveryDate DATE NOT NULL,
	DeliveryDate DATE NULL,
	OrderStatus NVARCHAR(255) NOT NULL,
);

CREATE TABLE ProductPurchaseOrder(
	PurchaseOrderID UNIQUEIDENTIFIER NOT NULL,
	ProductID UNIQUEIDENTIFIER NOT NULL,
	Quantity INT NOT NULL,
	UnitPrice DECIMAL(10,2) NOT NULL,
	CONSTRAINT PK_ProductPurchaseOrder PRIMARY KEY(PurchaseOrderID, ProductID),
	CONSTRAINT FK_PurchaseOrder_ProductPurchaseOrder
		FOREIGN KEY(PurchaseOrderID)
		REFERENCES PurchaseOrder(PurchaseOrderID),
	CONSTRAINT FK_Product_ProductPurchaseOrder
		FOREIGN KEY(ProductID)
		REFERENCES [Product](ProductID)
);

--CREATE VIEW vwProductCatalogue AS
--SELECT
--FROM

-- View til at vise den aktuelle lagerbeholdning
CREATE VIEW vwInStock AS
SELECT p.ProductName, p.ProductNumber, p.Price, p.Size, p.Colour, pc.InStock, pc.StockStatus
FROM ProductCatalogue pc
JOIN [Product] p ON pc.ProductCatalogueID = p.ProductCatalogueID
ORDER BY p.ProductName;

-- Stored Procedure til varemodtagelse og opdatering af lagerbeholdning
CREATE PROCEDURE uspRegisterPurchaseOrder @PurchaseOrderID UNIQUEIDENTIFIER AS
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
	UPDATE pc
	SET pc.InStock = pc.InStock + ppo.Quantity
	FROM ProductCatalogue pc
	JOIN [Product] p ON pc.ProductCatalogueID = p.ProductCatalogueID
	JOIN ProductPurchaseOrder ppo ON p.ProductID = ppo.ProductID
	WHERE ppo.PurchaseOrderID = @PurchaseOrderID

	-- Opdaterer ordrestatus og modtagelsesdato
	UPDATE PurchaseOrder
	SET
		DeliveryDate = GETDATE(),
		OrderStatus = 'Modtaget'
	WHERE PurchaseOrderID = @PurchaseOrderID
END;

-- TODO Tilføj en QuantityReceived kolonne til ProductPurchaseOrder
CREATE PROCEDURE uspRegisterPartialPurchaseOrder @PurchaseOrderID UNIQUEIDENTIFIER, @ProductID UNIQUEIDENTIFIER, @Quantity INT AS
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
	UPDATE pc
	SET pc.InStock = pc.InStock + @Quantity
	FROM ProductCatalogue pc
	JOIN Product p ON pc.ProductCatalogueID = p.ProductCatalogueID
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