select * from log
delete from log
delete from log
CREATE proc sp_ReadQueue
as
begin
	select * from MessageQueue order by date asc
	delete from MessageQueue
end

create proc sp_sendMessageToQueue
(
	@Message varchar(300)
)
as
begin
	insert into MessageQueue
	VALUES(GETDATE(), @Message)
end

sp_GetInvoiceDetail 14380