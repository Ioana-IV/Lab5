CREATE TABLE [dbo].[Order] (
    [OrderId] INT IDENTITY (1, 1) NOT NULL,
    [CustId]  INT NULL,
    [CarId]   INT NULL,
    PRIMARY KEY CLUSTERED ([OrderId] ASC),
	CONSTRAINT fk_order_cust_id
		FOREIGN KEY (CustId)
		REFERENCES Customer (CustId)
		ON DELETE CASCADE
			ON UPDATE CASCADE,
			CONSTRAINT fk_order_inv_id
		FOREIGN KEY (CarId)
		REFERENCES Inventory (CarId)
		ON DELETE CASCADE
			ON UPDATE CASCADE
);

