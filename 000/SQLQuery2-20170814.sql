alter proc sp_getCustomerPrepayment
(
	@customerId varchar(10)
)
as
begin
	select top 1  amount, referenceNumber, prepaymentRemainder, sequence from PaymentList where prepstat = 'Yes' and clientId = @customerId order by createdDate desc
end

create proc sp_adjustPrepaymentRemainder
(
	@amount decimal (19,2),
	@sequenceNumber integer
)
as
begin
	update PaymentList set prepaymentRemainder=prepaymentRemainder-@amount where sequence=@sequenceNumber
end

select * from PaymentList where prepstat='Yes'