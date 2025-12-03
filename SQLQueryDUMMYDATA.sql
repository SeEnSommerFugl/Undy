USE Undy
GO
-- Product insert

INSERT INTO Product(ProductID, ProductName, Price, Size, Colour, NumberInStock)
VALUES('UBBABLS1', 'Bambus Boxerbriefs', 139, 'S', 'Sort', 40);

INSERT INTO Product(ProductID, ProductName, Price, Size, Colour, NumberInStock)
VALUES('UBBABLM1', 'Bambus Boxerbriefs', 139, 'M', 'Sort', 50);

INSERT INTO Product(ProductID, ProductName, Price, Size, Colour, NumberInStock)
VALUES('UBBABLL1', 'Bambus Boxerbriefs', 139, 'L', 'Sort', 40);

INSERT INTO Product(ProductID, ProductName, Price, Size, Colour, NumberInStock)
VALUES('UBBABLXL1', 'Bambus Boxerbriefs', 139, 'XL', 'Sort', 30);

INSERT INTO Product(ProductID, ProductName, Price, Size, Colour, NumberInStock)
VALUES('UBBABLXXL1', 'Bambus Boxerbriefs', 139, '2XL', 'Sort', 20);

INSERT INTO Product(ProductID, ProductName, Price, Size, Colour, NumberInStock)
VALUES('UBBABLXXXL1', 'Bambus Boxerbriefs', 139, '3XL', 'Sort', 20);

-- PurchaseOrder insert

INSERT INTO PurchaseOrder(PurchaseOrderDate, ExpectedDeliveryDate, DeliveryDate, OrderStatus)
VALUES('2025-12-01', '2025-12-24', NULL, 'Pending');

INSERT INTO PurchaseOrder(PurchaseOrderDate, ExpectedDeliveryDate, DeliveryDate, OrderStatus)
VALUES('2026-01-01', '2026-01-24', NULL, 'Pending');

INSERT INTO PurchaseOrder(PurchaseOrderDate, ExpectedDeliveryDate, DeliveryDate, OrderStatus)
VALUES('2026-02-01', '2026-02-24', NULL, 'Pending');

INSERT INTO PurchaseOrder(PurchaseOrderDate, ExpectedDeliveryDate, DeliveryDate, OrderStatus)
VALUES('2026-03-01', '2026-03-24', NULL, 'Pending');
