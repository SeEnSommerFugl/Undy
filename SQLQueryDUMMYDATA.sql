USE Undy
GO
-- Product insert
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


-- PurchaseOrder insert

--INSERT INTO PurchaseOrder(PurchaseOrderDate, ExpectedDeliveryDate, DeliveryDate, OrderStatus)
--VALUES('2025-12-01', '2025-12-24', NULL, 'Pending');

--INSERT INTO PurchaseOrder(PurchaseOrderDate, ExpectedDeliveryDate, DeliveryDate, OrderStatus)
--VALUES('2026-01-01', '2026-01-24', NULL, 'Pending');

--INSERT INTO PurchaseOrder(PurchaseOrderDate, ExpectedDeliveryDate, DeliveryDate, OrderStatus)
--VALUES('2026-02-01', '2026-02-24', NULL, 'Pending');

--INSERT INTO PurchaseOrder(PurchaseOrderDate, ExpectedDeliveryDate, DeliveryDate, OrderStatus)
--VALUES('2026-03-01', '2026-03-24', NULL, 'Pending');


-- Customer Insert
EXEC usp_Insert_Customer
	@Firstname = 'John',
	@LastName = 'Doe',
	@Email = 'JohnD@email.com',
	@PhoneNumber = 12345678,
	@Address = 'Spurvevej 3',
	@City = 'Spurveby',
	@PostalCode = 2000
GO

EXEC usp_Insert_Customer
	@Firstname = 'Per',
	@LastName = 'Pedersen',
	@Email = 'PerP@email.com',
	@PhoneNumber = 23456789,
	@Address = 'Fasanvej 25',
	@City = 'Fasanby',
	@PostalCode = 2200
GO

EXEC usp_Insert_Customer
	@Firstname = 'Lars',
	@LastName = 'Larsen',
	@Email = 'LarsL@email.com',
	@PhoneNumber = 34567891,
	@Address = 'Ørnevej 38',
	@City = 'Ørneby',
	@PostalCode = 2400
GO

EXEC usp_Insert_Customer
	@Firstname = 'Jens',
	@LastName = 'Jensen',
	@Email = 'JensJ@email.com',
	@PhoneNumber = 45678912,
	@Address = 'Hajrevej 3',
	@City = 'Hajreby',
	@PostalCode = 2600
GO

