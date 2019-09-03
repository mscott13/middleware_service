ALTER proc [dbo].[sp_StoreInvoice]
(
	@invoiceId int,
	@targetBatch int,
	@CreditGL int,
	@clientName varchar(120),
	@clientId varchar(20),
	@dateCreated datetime,
	@author varchar(50),
	@amount decimal(19,2),
	@state varchar(30)
)
as
begin

	declare @var1 integer
	declare @var2 integer
	set @var1 = (select [Count] FROM InvoiceBatch where BatchId=@targetBatch)
	set @var2 = (select COUNT(@invoiceId) from InvoiceList)
	set @var2 = @var2 +1;
	
	
	if(@state = 'updated')
	begin
			insert into InvoiceList
	values(@invoiceId, 'T', @targetBatch, @var1, @CreditGl, @var2, @clientName, @clientId, @dateCreated, @author, @amount, GETDATE(), @state);
	end
	else
	begin
			insert into InvoiceList
	values(@invoiceId, 'NT', @targetBatch, @var1, @CreditGl, @var2, @clientName, @clientId, @dateCreated, @author, @amount, GETDATE(), @state);
	end

	update InvoiceBatch set [Count]=@var1 where BatchId=@targetBatch

end

select * from InvoiceList