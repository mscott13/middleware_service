select * from tblGLDocuments
select * from tblARPayments
select * from paymentList order by sequence desc

alter table paymentList add referenceNumber integer
alter table paymentlist add prepaymentRemainder decimal (19,2) default 0

update PaymentList set referenceNumber = 0
update PaymentList set prepaymentRemainder = 0
