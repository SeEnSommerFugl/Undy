CREATE TABLE ProductCatalogue(
	ProductCatalogueID UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	ProductNumber INT NOT NULL,
	ProductName NVARCHAR(255) NOT NULL
);

CREATE TABLE [Product](
	ProductID UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	ProductNumber INT NOT NULL,
	ProductName NVARCHAR(255) NOT NULL,
	Price DECIMAL(10,2) NOT NULL,
	InStock INT NOT NULL,
	[Status] NVARCHAR(255) NOT NULL,
	MinimumStockLimit INT NOT NULL,
	ProductCatalogueID UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT FK_ProductCatalogue_Product
		FOREIGN KEY(ProductCatalogueID)
		REFERENCES ProductCatalogue(ProductCatalogueID)
);

CREATE TABLE PurchaseOrder(
	PurchaseOrderID UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	ProductNumber INT NOT NULL,
	ExpectedDeliveryDate DATE NOT NULL,
	PurchaseOrderDate DATE NOT NULL,
	DeliveryDate DATE NULL,
	OrderStatus NVARCHAR(255) NOT NULL,
	ProductID UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT FK_Product_PurchaseOrder
		FOREIGN KEY(ProductID)
		REFERENCES [Product](ProductID)
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