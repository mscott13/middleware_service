create table PaymentBatch
(
BatchId integer primary key not null,
CreatedDate date not null,
ExpiryDate date not null,
Status varchar(30) not null,
BankCodeId varchar(20) not null,
Count integer not null
foreign key (BankCodeId) references BankCode(BankCodeId)
);
GO

---------------------------------------------------------------

alter PROCEDURE sp_NewBatchSet_Payment
(
@FirstBatch integer,
@CreatedDate date ,
@ExpiryDate date
)
AS
BEGIN
	
	DECLARE @SecondBatch INTEGER
	SET @SecondBatch = @FirstBatch +1

	DECLARE @ThirdBatch INTEGER
	SET @ThirdBatch = @SecondBatch +1

	INSERT INTO PaymentBatch
	VALUES(@FirstBatch, @CreatedDate, @ExpiryDate, 'Open', '10010-100', 0),
		  (@SecondBatch, @CreatedDate, @ExpiryDate,'Open', '10012-100', 0),
		  (@ThirdBatch, @CreatedDate, @ExpiryDate,'Open', '10020-100', 0)
END

------------------------------------------------------------

create table BankCode
(
BankCodeId varchar(20) primary key,
BankCode varchar(30) not null,
CurrentRefNumber integer not null
);

CREATE PROCEDURE sp_GetLastBatchId_Payment
AS
BEGIN
	SELECT TOP 1 BatchId FROM PaymentBatch ORDER BY BatchId DESC
END
GO

-------------------------------------------------------------

CREATE PROCEDURE sp_CloseBatch_Payment
AS
BEGIN
	Update PaymentBatch set Status='Closed' where Status='Open'
END

CREATE PROCEDURE sp_GetOpenBatch_Payment
AS
BEGIN
	select * from PaymentBatch where Status='Open'
END

--------------------------------------------------------------

create procedure sp_GetCount_payment
AS
BEGIN
SELECT COUNT(*) from PaymentBatch
END

create procedure sp_GetCurrentRefNumber
(
@BankCodeId varchar(20)
)
AS
BEGIN
	select CurrentRefNumber from BankCode where BankCodeId=@BankCodeId
END

--------------------------------------------------------------

create procedure sp_IncrementRefNumber
(
@BankCodeId varchar(20)
)
AS
BEGIN
Declare @var1 integer
	SELECT @var1 = CurrentRefNumber from BankCode where BankCode.BankCodeId=@BankCodeId
	set @var1 = @var1 +1
	update BankCode set CurrentRefNumber=@var1 where BankCode.BankCodeId=@BankCodeId
END

-------------------------------------------------------------

alter procedure sp_IncrementEntryCount_payment
(
@BatchId integer
)
AS
BEGIN
Declare @count integer
	select @count = Count from PaymentBatch where BatchId = @BatchId
	set @count = @count + 1
	update PaymentBatch set Count=@count where BatchId=@BatchId
END

create procedure sp_ResetReferenceNumbers
(
@fgbjmrec integer,
@fgbusmrc integer,
@ncbjmrec integer
)
as
begin
update BankCode set CurrentRefNumber=@fgbjmrec where BankCodeId='10010-100'
update BankCode set CurrentRefNumber=@fgbusmrc where BankCodeId='10012-100'
update BankCode set CurrentRefNumber=@ncbjmrec where BankCodeId='10020-100'
end

exec sp_ResetReferenceNumbers 40000353, 30000227, 10000024


exec sp_IncrementEntryCount_payment 3295
exec sp_IncrementRefNumber '10010-100'


--Statements--
select * from PaymentBatch
select * from BankCode
delete from PaymentBatch
select * from InvoiceBatch
delete from InvoiceBatch

