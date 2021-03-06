CREATE DATABASE QuanLyQuanCafe2
GO

USE QuanLyQuanCafe2
GO

-- Food
-- Table
-- FoodCategory
-- Account
-- Bill
-- BillInfo

CREATE TABLE TableFood
(
	id INT IDENTITY PRIMARY KEY,
	name NVARCHAR(100) NOT NULL DEFAULT N'Chua co ten',
	status NVARCHAR(100) NOT NULL DEFAULT N'Trống',	-- Trống || Có người
	IsUsed bit NOT NULL DEFAULT 'true' 
)
GO

CREATE TABLE Account
(
	Id INT IDENTITY PRIMARY KEY,
	UserName NVARCHAR(100),
	PassWord NVARCHAR(1000),
	DisplayName NVARCHAR(100),
	CMND VARCHAR(10),
	Email NVARCHAR(100),
	Phone VARCHAR(10),
	GioiTinh BIT,
	Address NVARCHAR(MAX),
	Birthday DATETIME,
	ImageID NVARCHAR(MAX),
	Type NVARCHAR(50),
    IsUsed bit NOT NULL DEFAULT 'true' 
)
GO

CREATE TABLE FoodCategory
(
	id INT IDENTITY PRIMARY KEY,
	name NVARCHAR(100) NOT NULL DEFAULT N'Chua dat ten',
	IsUsed bit NOT NULL DEFAULT 'true' 
)
GO

CREATE TABLE Food
(
	id INT IDENTITY PRIMARY KEY,
	name NVARCHAR(100),
	idCategory INT,
	price FLOAT NOT NULL DEFAULT 0,
	IsUsed bit NOT NULL DEFAULT 'true' 
	FOREIGN KEY (idCategory) REFERENCES FoodCategory(id),
	
)
GO

CREATE TABLE Bill
(
	id INT IDENTITY PRIMARY KEY,
	DateCheckIn DATE NOT NULL DEFAULT GETDATE(),
	DateCheckOut DATE,
	idTable INT NOT NULL,
	status INT NOT NULL, -- 1 Thanh toan || 0 chua thanh toan
	discount INT NOT NULL DEFAULT 0,
	totalPrice FLOAT
	FOREIGN KEY (idTable) REFERENCES TableFood(id)
)
GO

CREATE TABLE BillInfo
(
	id INT IDENTITY PRIMARY KEY,
	idBill INT NOT NULL,
	idFood INT NOT NULL,
	count INT NOT NULL DEFAULT 0
	FOREIGN KEY (idBill) REFERENCES Bill(id),
	FOREIGN KEY (idFood) REFERENCES Food(id)
)
GO

-- Tạo 1 tì khoản mặc định
INSERT dbo.Account
        ( DisplayName ,
          UserName ,
          Password ,
          Type ,
          isUsed
        )
VALUES  ( N'HiepVo' , -- DisplayName - nvarchar(100)
          N'k9' , -- UserName - nvarchar(100)
          N'1962026656160185351301320480154111117132155' , -- Password - nvarchar(100)
          N'Admin' , -- Type - nvarchar(50)
          'true'  -- isUsed - bit
        )
GO

CREATE PROC USP_GetAccountByUserName
@userName nvarchar(100)
AS
BEGIN
	SELECT * FROM Account WHERE UserName = @userName AND IsUsed = 'true'
END
GO

-- Câu lệnh thực thi proceduce
-- EXEC USP_GetAccountByUserName @username = N'K9' -- nvarchar(100)

alter PROC USP_Login
@username nvarchar(100), @passWord nvarchar(100)
AS
BEGIN
PRINT(@username)
PRINT(@passWord)
	SELECT * FROM Account WHERE UserName = @username and PassWord = @passWord 
END
GO

EXEC USP_Login @userName = N'k9' , @passWord = N'1962026656160185351301320480154111117132155'
												
-- Thêm bàn
DECLARE @i INT = 0		-- khai báo biến i với default value = 0
WHILE @i <= 10
BEGIN
	INSERT TableFood(name)
	VALUES (N'Bàn ' + CAST(@i AS nvarchar(100)))
	SET @i = @i + 1
END
GO

-- tạo proceduce lấy danh sách bàn
CREATE PROC USP_GetTableList
AS SELECT * FROM TableFood
GO

-- them category
INSERT FoodCategory (name)
VALUES (N'Hải sản')

INSERT FoodCategory (name)
VALUES (N'Nông sản')

INSERT FoodCategory (name)
VALUES (N'Lẩu')

INSERT FoodCategory (name)
VALUES (N'Giải khát')

-- Them mon an
INSERT Food (name, idCategory, price)
VALUES (N'Mực một nắng nướng sa tế', 1, 150000)

INSERT Food (name, idCategory, price)
VALUES (N'Ngêu hấp xả', 1, 100000)

INSERT Food (name, idCategory, price)
VALUES (N'Vú dê nướng', 2, 80000)

INSERT Food (name, idCategory, price)
VALUES (N'Bò né', 2, 50000)

INSERT Food (name, idCategory, price)
VALUES (N'Lẩu Thái', 3, 120000)

INSERT Food (name, idCategory, price)
VALUES (N'Sting', 4, 120000)

INSERT Food (name, idCategory, price)
VALUES (N'Coca', 4, 120000)

-- them bill
INSERT Bill (DateCheckIn, DateCheckOut, idTable, status)  -- status: 0 - chưa check out   1 - đã check out
VALUES (GETDATE(), NULL, 3, 0)

INSERT Bill (DateCheckIn, DateCheckOut, idTable, status)
VALUES (GETDATE(), NULL, 4, 0)

INSERT Bill (DateCheckIn, DateCheckOut, idTable, status)
VALUES (GETDATE(), GETDATE(), 5, 1)

-- them bill info 
INSERT BillInfo (idBill, idFood, count)
VALUES (1, 1, 2)

INSERT BillInfo (idBill, idFood, count)
VALUES (2, 3, 4)

INSERT BillInfo (idBill, idFood, count)
VALUES (3, 5, 2)

INSERT BillInfo (idBill, idFood, count)
VALUES (1, 1, 2)

INSERT BillInfo (idBill, idFood, count)
VALUES (2, 1, 6)

INSERT BillInfo (idBill, idFood, count)
VALUES (1, 2, 2)
GO

CREATE PROC USP_InsertBill
@idTable INT
AS
BEGIN
	INSERT dbo.Bill 
	        ( DateCheckIn ,
	          DateCheckOut ,
	          idTable ,
	          status,
	          discount
	        )
	VALUES  ( GETDATE() , -- DateCheckIn - date
	          NULL , -- DateCheckOut - date
	          @idTable , -- idTable - int
	          0,  -- status - int
	          0
	        )
END
GO


GO
CREATE PROC USP_InsertBillInfo
@idBill INT, @idFood INT, @count INT
AS
BEGIN

	DECLARE @isExitsBillInfo INT
	DECLARE @foodCount INT = 1
	
	SELECT @isExitsBillInfo = id, @foodCount = b.count 
	FROM dbo.BillInfo AS b 
	WHERE idBill = @idBill AND idFood = @idFood

	IF (@isExitsBillInfo > 0)
	BEGIN
		DECLARE @newCount INT = @foodCount + @count
		IF (@newCount > 0)
			UPDATE dbo.BillInfo	SET count = @foodCount + @count WHERE idFood = @idFood
		ELSE
			DELETE dbo.BillInfo WHERE idBill = @idBill AND idFood = @idFood
	END
	ELSE
	BEGIN
		INSERT	dbo.BillInfo
        ( idBill, idFood, count )
		VALUES  ( @idBill, -- idBill - int
          @idFood, -- idFood - int
          @count  -- count - int
          )
	END
END
GO

CREATE TRIGGER UTG_UpdateBillInfo
ON dbo.BillInfo FOR INSERT, UPDATE
AS
BEGIN
	DECLARE @idBill INT
	
	SELECT @idBill = idBill FROM Inserted
	
	DECLARE @idTable INT
	
	SELECT @idTable = idTable FROM dbo.Bill WHERE id = @idBill AND status = 0	
	
	DECLARE @count INT
	SELECT @count = COUNT(*) FROM dbo.BillInfo WHERE idBill = @idBill
	
	IF (@count > 0)
		BEGIN
	
			PRINT @idTable
			PRINT @idBill
			PRINT @count
		
			UPDATE dbo.TableFood SET status = N'Có người' WHERE id = @idTable		
		
		END		
	ELSE
		BEGIN
		PRINT @idTable
			PRINT @idBill
			PRINT @count
		UPDATE dbo.TableFood SET status = N'Trống' WHERE id = @idTable	
		end
	
END
GO
 --CREATE TRIGGER UTG_UpdateTable
 --ON dbo.TableFood FOR UPDATE
 --AS
 --BEGIN
	--DECLARE @idTable INT
	--DECLARE	@status NVARCHAR(100)

	--SELECT @idTable = id, @status = Inserted.status FROM Inserted
	
	--DECLARE @idBill INT
	--SELECT @idBill = id FROM dbo.Bill WHERE idTable = @idTable AND status = 0

	--DECLARE @countBillInfo INT
	--SELECT @countBillInfo = COUNT(*) FROM dbo.BillInfo WHERE idBill = @idBill

	--IF(@countBillInfo > 0 and @status <> N'Có người')
	--	UPDATE dbo.TableFood SET status = N'Có người' WHERE id = @idTable
	--ELSE IF(@countBillInfo <= 0 AND @status <>N'Trống')
	--	UPDATE dbo.TableFood SET status = N'Trống' WHERE id = @idTable
 --END
 
CREATE TRIGGER UTG_UpdateBill
ON dbo.Bill FOR UPDATE
AS
BEGIN
	DECLARE @idBill INT
	
	SELECT @idBill = id FROM Inserted	
	
	DECLARE @idTable INT
	
	SELECT @idTable = idTable FROM dbo.Bill WHERE id = @idBill
	
	DECLARE @count int = 0
	
	SELECT @count = COUNT(*) FROM dbo.Bill WHERE idTable = @idTable AND status = 0
	
	IF (@count = 0)
		UPDATE dbo.TableFood SET status = N'Trống' WHERE id = @idTable
END
GO
 
CREATE PROC USP_SwitchTable
@idTable1 INT, @idTable2 INT
AS BEGIN

	DECLARE @idFirstBill INT
    DECLARE @idSecondBill INT

	DECLARE @isFirstTableEmpty INT = 1
	DECLARE @isSecondTableEmpty INT = 1

	--SELECT @idSecondBill = id FROM Bill WHERE idTable = @idTable2 AND status = 0

	SELECT @idFirstBill = id FROM Bill WHERE idTable = @idTable1 AND status = 0
	SELECT @idSecondBill = id FROM Bill WHERE idTable = @idTable2 AND status = 0

	IF(@idFirstBill IS NULL)
	BEGIN
		 INSERT INTO dbo.Bill
		         ( DateCheckIn ,
		           DateCheckOut ,
		           idTable ,
		           status
		         )
		 VALUES  ( GETDATE() , -- DateCheckIn - date
		           NULL , -- DateCheckOut - date
		           @idTable1 , -- idTable - int
		           0  -- status - int
		         )
		 SELECT @idFirstBill = MAX(id) FROM dbo.Bill WHERE idTable = @idTable1 AND status = 0
		
	END

	SELECT @isFirstTableEmpty = COUNT(*) FROM dbo.BillInfo WHERE idBill = @idFirstBill

	IF(@idSecondBill IS NULL)
	BEGIN
		 INSERT INTO dbo.Bill
		         ( DateCheckIn ,
		           DateCheckOut ,
		           idTable ,
		           status
		         )
		 VALUES  ( GETDATE() , -- DateCheckIn - date
		           NULL , -- DateCheckOut - date
		           @idTable2 , -- idTable - int
		           0  -- status - int
		         )
		 SELECT @idSecondBill = MAX(id) FROM dbo.Bill WHERE idTable = @idTable2 AND status = 0
	END
    SELECT @isSecondTableEmpty = COUNT(*) FROM dbo.BillInfo WHERE idBill = @idSecondBill


	SELECT id INTO IDBillInfoTable FROM dbo.BillInfo WHERE idBill = @idSecondBill
	
	UPDATE dbo.BillInfo SET idBill = @idSecondBill WHERE idBill = @idFirstBill

	UPDATE dbo.BillInfo SET idBill = @idFirstBill WHERE id  IN (SELECT * FROM IDBillInfoTable)

	DROP TABLE IDBillInfoTable

	IF(@isFirstTableEmpty = 0)
		UPDATE dbo.TableFood SET status = N'Trống' WHERE id = @idTable2
	IF(@isSecondTableEmpty = 0)
		UPDATE dbo.TableFood SET status = N'Trống' WHERE id = @idTable1
END


GO
CREATE PROC USP_GetListBillByDate
@checkIn date,@checkOut DATE
AS
BEGIN
	SELECT t.name AS [Tên bàn], b.totalPrice AS [Tổng tiền], b.DateCheckIn AS[Thời gian vào], b.DateCheckOut AS [Thời gian ra], b.discount AS [Giảm giá]
FROM dbo.Bill AS b, dbo.TableFood AS t
WHERE b.DateCheckIn >= @checkIn AND b.DateCheckOut <= @checkOut AND b.status = 1
AND t.id = b.idTable

END


GO
Create PROC USP_UpdateAccount
 @idAccount INT ,@userName NVARCHAR(100), @displayName NVARCHAR(100), @password NVARCHAR(100), @newPassword NVARCHAR(100), @type NVARCHAR(50), @phone VARCHAR(10), @GioiTinh BIT, @Address NVARCHAR(MAX), @CMND VARCHAR(10), @ImageId NVARCHAR(MAX), @Birthday DATETIME, @Email NVARCHAR(100), @IsUsed BIT
AS
BEGIN
	DECLARE @exists INT
	SELECT @exists = COUNT(*) FROM dbo.Account WHERE ID = @idAccount 
	
	IF(@exists = 1)
	BEGIN
		IF(@newPassword = NULL OR @newPassword = '')
		BEGIN
			UPDATE dbo.Account SET DisplayName = @displayName, Phone = @phone, GioiTinh = @GioiTinh, Address = @Address, CMND = @CMND, ImageID = @ImageID, Birthday = @Birthday, Email = @Email, IsUsed = @IsUsed
			WHERE UserName = @userName AND ID = @idAccount
			PRINT('---------111')
			--SELECT * FROM dbo.Account
		END
		ELSE
		BEGIN
			
			UPDATE dbo.Account SET DisplayName = @displayName, PassWord = @newPassword, Phone = @phone, GioiTinh = @GioiTinh, Address = @Address, CMND = @CMND, ImageID = @ImageID, Birthday = @Birthday, Email = @Email, IsUsed = @IsUsed
			WHERE UserName = @userName AND ID = @idAccount
			PRINT('---------2222')
			--SELECT * FROM dbo.Account
		END
	END
    ELSE
	BEGIN
		INSERT dbo.Account ( UserName ,PassWord ,DisplayName ,Type ,IsUsed ,Phone ,GioiTinh ,Address ,CMND ,ImageID ,Birthday ,Email)
		VALUES  ( @userName, @password, @displayName, @type, 'true', @phone , @GioiTinh , @Address , @CMND , @ImageId , @Birthday , @Email)
	END
    
END
GO


CREATE TRIGGER UTG_DeleteBillInfo
ON dbo.BillInfo FOR DELETE
AS
BEGIN
	DECLARE @idBillInfo INT
	DECLARE @idBill INT
	SELECT @idBillInfo = id, @idBill =  Deleted.idBill FROM Deleted

	DECLARE @idTable INT
	SELECT @idTable = idTable FROM Bill WHERE id = @idBill

	DECLARE @count INT = 0

	SELECT @count = COUNT(*) FROM dbo.BillInfo AS bi, dbo.Bill AS b WHERE b.id = bi.idBill AND b.id = @idBill AND b.status = 0

	IF(@count = 0)
		UPDATE dbo.TableFood SET status = N'Trống' WHERE id = @idTable
END
GO

 
 GO
 -- Hàm chuyển đối có dấu thành ko dấu
CREATE FUNCTION [dbo].[fuConvertToUnsign1] ( @strInput NVARCHAR(4000) ) RETURNS NVARCHAR(4000) AS BEGIN IF @strInput IS NULL RETURN @strInput IF @strInput = '' RETURN @strInput DECLARE @RT NVARCHAR(4000) DECLARE @SIGN_CHARS NCHAR(136) DECLARE @UNSIGN_CHARS NCHAR (136) SET @SIGN_CHARS = N'ăâđêôơưàảãạáằẳẵặắầẩẫậấèẻẽẹéềểễệế ìỉĩịíòỏõọóồổỗộốờởỡợớùủũụúừửữựứỳỷỹỵý ĂÂĐÊÔƠƯÀẢÃẠÁẰẲẴẶẮẦẨẪẬẤÈẺẼẸÉỀỂỄỆẾÌỈĨỊÍ ÒỎÕỌÓỒỔỖỘỐỜỞỠỢỚÙỦŨỤÚỪỬỮỰỨỲỶỸỴÝ' +NCHAR(272)+ NCHAR(208) SET @UNSIGN_CHARS = N'aadeoouaaaaaaaaaaaaaaaeeeeeeeeee iiiiiooooooooooooooouuuuuuuuuuyyyyy AADEOOUAAAAAAAAAAAAAAAEEEEEEEEEEIIIII OOOOOOOOOOOOOOOUUUUUUUUUUYYYYYDD' DECLARE @COUNTER int DECLARE @COUNTER1 int SET @COUNTER = 1 WHILE (@COUNTER <=LEN(@strInput)) BEGIN SET @COUNTER1 = 1 WHILE (@COUNTER1 <=LEN(@SIGN_CHARS)+1) BEGIN IF UNICODE(SUBSTRING(@SIGN_CHARS, @COUNTER1,1)) = UNICODE(SUBSTRING(@strInput,@COUNTER ,1) ) BEGIN IF @COUNTER=1 SET @strInput = SUBSTRING(@UNSIGN_CHARS, @COUNTER1,1) + SUBSTRING(@strInput, @COUNTER+1,LEN(@strInput)-1) ELSE SET @strInput = SUBSTRING(@strInput, 1, @COUNTER-1) +SUBSTRING(@UNSIGN_CHARS, @COUNTER1,1) + SUBSTRING(@strInput, @COUNTER+1,LEN(@strInput)- @COUNTER) BREAK END SET @COUNTER1 = @COUNTER1 +1 END SET @COUNTER = @COUNTER +1 END SET @strInput = replace(@strInput,' ','-') RETURN @strInput END

GO
--EXEC dbo.USP_UpdateAccount @userName = N'K9', -- nvarchar(100)
--    @displayName = N'Hiep', -- nvarchar(100)
--    @password = N'', -- nvarchar(100)
--    @newPassword = N'' -- nvarchar(100)


CREATE PROC USP_GetListBillByDateAndPage
@checkIn date,@checkOut DATE, @page int
AS
BEGIN
	DECLARE @pageRows INT = 10
	DECLARE @selectRows INT = @pageRows
	DECLARE @exceptRows INT = (@page - 1) * @pageRows


	;WITH BillShow AS (SELECT b.id, t.name AS [Tên bàn], b.totalPrice AS [Tổng tiền], b.DateCheckIn AS[Thời gian vào], b.DateCheckOut AS [Thời gian ra], b.discount AS [Giảm giá]
	FROM dbo.Bill AS b, dbo.TableFood AS t
	WHERE b.DateCheckIn >= @checkIn AND b.DateCheckOut <= @checkOut AND b.status = 1
		AND t.id = b.idTable)
	
	SELECT TOP (@selectRows) * FROM BillShow WHERE id NOT IN (SELECT TOP (@exceptRows) id FROM BillShow)
	EXCEPT
    SELECT TOP (@exceptRows) * FROM BillShow

END
GO

CREATE PROC USP_GetNumBillByDate
@checkIn date,@checkOut DATE
AS
BEGIN
	SELECT COUNT(*)
FROM dbo.Bill AS b, dbo.TableFood AS t
WHERE b.DateCheckIn >= @checkIn AND b.DateCheckOut <= @checkOut AND b.status = 1
AND t.id = b.idTable

END
GO



CREATE PROC USP_GetListBillByDateForReport
@checkIn date,@checkOut DATE
AS
BEGIN
	SELECT t.name , b.totalPrice , b.DateCheckIn, b.DateCheckOut , b.discount
FROM dbo.Bill AS b, dbo.TableFood AS t
WHERE b.DateCheckIn >= @checkIn AND b.DateCheckOut <= @checkOut AND b.status = 1
AND t.id = b.idTable

END
GO

CREATE PROC USP_GetNumFood
AS
BEGIN
	SELECT COUNT(*)
FROM dbo.Food

END
GO

--INSERT dbo.Account
--        ( UserName ,
--          PassWord ,
--          DisplayName ,
--		  Phone,
--		  ImageID,
--		  Address,
--		  GioiTinh,
--		  CMND,
--		  Birthday,
--		  Email,
--          Type ,
--          IsUsed
--        )
--VALUES  ( N'' , 
--          N'' , 
--          N'' , 
--          N'' , 
--          NULL 
--        )

		
--DROP DATABASE QuanLyQuanCafe
--ALTER TABLE	dbo.Bill
--ADD discount INT

--UPDATE dbo.Bill SET discount = 0


--ALTER TABLE dbo.Account
--ADD ID NVARCHAR(10)
--UPDATE dbo.Account SET IsUsed = 'true'

--ALTER TABLE dbo.Food
--ADD isUsed BIT
--UPDATE dbo.Food SET IsUsed = 'true'

--ALTER TABLE dbo.FoodCategory
--ADD isUsed BIT
--UPDATE dbo.FoodCategory SET IsUsed = 'true'

--ALTER TABLE dbo.TableFood
--ADD isUsed BIT
--UPDATE dbo.TableFood SET IsUsed = 'true'


--INSERT dbo.FoodCategory( name, IsUsed )VALUES  ( N'', NULL)

--DELETE dbo.Bill

--DELETE dbo.BillInfo

--INSERT dbo.Account( UserName, DisplayName, Type, Password ) VALUES  ( N'', N'', N'', N'')
--DELETE dbo.Account
--EXEC dbo.USP_UpdateAccount @idAccount = 14, @userName = N'sdadas', @displayName = N'hiepvodasdasd', @password = N'1', @newPassword = N'1', @type = N'Staff', @phone = '234324324', @GioiTinh = 'false', @Address = N'sakjdksdnasjdjask', @CMND = '251515425', @ImageId = N'', @Birthday = '2018-11-19 19:48:06', @Email = N'dsfsflksjdf@yadskadjkal' 
--select COUNT(*) from FoodCategory where id = 5
Exec USP_Login @userName = N'k9' , @passWord = N'1962026656160185351301320480154111117132155'
--SELECT * FROM dbo.Bill WHERE idTable = 1 AND status = 0