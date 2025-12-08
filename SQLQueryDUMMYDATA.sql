USE Undy
GO
-- Product insert

--INSERT INTO Product(ProductNumber, ProductName, Price, Size, Colour, NumberInStock)
--VALUES('UBBABLS1', 'Bambus Boxerbriefs', 139, 'S', 'Sort', 40);

--INSERT INTO Product(ProductNumber, ProductName, Price, Size, Colour, NumberInStock)
--VALUES('UBBABLM1', 'Bambus Boxerbriefs', 139, 'M', 'Sort', 50);

--INSERT INTO Product(ProductNumber, ProductName, Price, Size, Colour, NumberInStock)
--VALUES('UBBABLL1', 'Bambus Boxerbriefs', 139, 'L', 'Sort', 40);

--INSERT INTO Product(ProductNumber, ProductName, Price, Size, Colour, NumberInStock)
--VALUES('UBBABLXL1', 'Bambus Boxerbriefs', 139, 'XL', 'Sort', 30);

--INSERT INTO Product(ProductNumber, ProductName, Price, Size, Colour, NumberInStock)
--VALUES('UBBABLXXL1', 'Bambus Boxerbriefs', 139, '2XL', 'Sort', 20);

--INSERT INTO Product(ProductNumber, ProductName, Price, Size, Colour, NumberInStock)
--VALUES('UBBABLXXXL1', 'Bambus Boxerbriefs', 139, '3XL', 'Sort', 20);

EXEC usp_Insert_Product
	@ProductNumber = 'UBBABLS1',
	@ProductName = 'Bambus Boxerbriefs',
	@Price = 139,
	@Size = 'S',
	@Colour = 'Sort',
	@NumberInStock = 40
GO

EXEC usp_Insert_Product
	@ProductNumber = 'UBBABLM1',
	@ProductName = 'Bambus Boxerbriefs',
	@Price = 139,
	@Size = 'M',
	@Colour = 'Sort',
	@NumberInStock = 50
GO

EXEC usp_Insert_Product
	@ProductNumber = 'UBBABLL1',
	@ProductName = 'Bambus Boxerbriefs',
	@Price = 139,
	@Size = 'L',
	@Colour = 'Sort',
	@NumberInStock = 40
GO

EXEC usp_Insert_Product
	@ProductNumber = 'UBBABLXL1',
	@ProductName = 'Bambus Boxerbriefs',
	@Price = 139,
	@Size = 'XL',
	@Colour = 'Sort',
	@NumberInStock = 30
GO

EXEC usp_Insert_Product
	@ProductNumber = 'UBBABLXXL1',
	@ProductName = 'Bambus Boxerbriefs',
	@Price = 139,
	@Size = '2XL',
	@Colour = 'Sort',
	@NumberInStock = 20
GO

EXEC usp_Insert_Product
	@ProductNumber = 'UBBABLXXXL1',
	@ProductName = 'Bambus Boxerbriefs',
	@Price = 139,
	@Size = '3XL',
	@Colour = 'Sort',
	@NumberInStock = 20
GO


---- PurchaseOrder insert

--INSERT INTO PurchaseOrder(PurchaseOrderDate, ExpectedDeliveryDate, DeliveryDate, OrderStatus)
--VALUES('2025-12-01', '2025-12-24', NULL, 'Pending');

--INSERT INTO PurchaseOrder(PurchaseOrderDate, ExpectedDeliveryDate, DeliveryDate, OrderStatus)
--VALUES('2026-01-01', '2026-01-24', NULL, 'Pending');

--INSERT INTO PurchaseOrder(PurchaseOrderDate, ExpectedDeliveryDate, DeliveryDate, OrderStatus)
--VALUES('2026-02-01', '2026-02-24', NULL, 'Pending');

--INSERT INTO PurchaseOrder(PurchaseOrderDate, ExpectedDeliveryDate, DeliveryDate, OrderStatus)
--VALUES('2026-03-01', '2026-03-24', NULL, 'Pending');


-- Customer Insert
INSERT INTO CUSTOMER(FirstName, LastName, Email, PhoneNumber, [Address], City, PostalCode)
VALUES('John', 'Doe', 'JonhD@email.com', 12345678,  'Spurvevej 3', 'Spurveby', 2000);

INSERT INTO CUSTOMER(FirstName, LastName, Email, PhoneNumber, [Address], City, PostalCode)
VALUES('Per', 'Pedersen', 'PerP@email.com', 23456789,  'Fasanvej 25', 'Fasanby', 2200);

INSERT INTO CUSTOMER(FirstName, LastName, Email, PhoneNumber, [Address], City, PostalCode)
VALUES('Lars', 'Larsen', 'LarsL@email.com', 34567891,  'Ørnevej 38', 'Ørneby', 2400);

INSERT INTO CUSTOMER(FirstName, LastName, Email, PhoneNumber, [Address], City, PostalCode)
VALUES('Jens', 'Jensen', 'JensJ@email.com', 45678912,  'Hajrevej 3', 'Hajreby', 2600);