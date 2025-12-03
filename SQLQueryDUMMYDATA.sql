USE Undy
GO

DROP PROCEDURE usp_Insert_Stock

-- Product insert

INSERT INTO Product(ProductNumber, ProductName, Price, Size, Colour, StockID)
VALUES('UBBABLS1', 'Bambus Boxerbriefs', 139, 'S', 'Sort', 1);

INSERT INTO Product(ProductNumber, ProductName, Price, Size, Colour, StockID)
VALUES('UBBABLM1', 'Bambus Boxerbriefs', 139, 'M', 'Sort', 1);

INSERT INTO Product(ProductNumber, ProductName, Price, Size, Colour, StockID)
VALUES('UBBABLL1', 'Bambus Boxerbriefs', 139, 'L', 'Sort', 1);

INSERT INTO Product(ProductNumber, ProductName, Price, Size, Colour, StockID)
VALUES('UBBABLXL1', 'Bambus Boxerbriefs', 139, 'XL', 'Sort', 1);

INSERT INTO Product(ProductNumber, ProductName, Price, Size, Colour, StockID)
VALUES('UBBABLXXL1', 'Bambus Boxerbriefs', 139, '2XL', 'Sort', 1);

INSERT INTO Product(ProductNumber, ProductName, Price, Size, Colour, StockID)
VALUES('UBBABLXXXL1', 'Bambus Boxerbriefs', 139, '3XL', 'Sort', 1);