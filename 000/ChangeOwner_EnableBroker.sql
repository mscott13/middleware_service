select * from sys.databases

select * from sys.server_principals

SELECT name AS Login_Name, type_desc AS Account_Type
FROM sys.server_principals 
WHERE TYPE IN ('U', 'S', 'G')
and name not like '%##%'
ORDER BY name, type_desc

EXEC sp_changedbowner 'SMA\cgriffith'

alter database AsmsGenericMaster set enable_broker with rollback immediate

select * from sys.databases

select * from InvoiceBatch
update InvoiceBatch set ExpiryDate='2012-03-18'