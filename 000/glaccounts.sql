select * from tblGLAccounts order by Active desc

select GLAccountID, GlAccountNumber, GlAccountName from tblGLAccounts where GlAccountNumber like '%10012-100%'

select GLAccountID, GlAccountNumber, GlAccountName from tblGLAccounts where GlAccountNumber like '%10020-100%'

select GLAccountID, GlAccountNumber, GlAccountName from tblGLAccounts where GlAccountNumber like '%10010-100%'

select * from tblARPayments

select * from tblGLAccounts where GLAccountID=127