

USE AsmsGenericMaster
GO
UPDATE tblGLAccounts
SET ACTIVE=0
WHERE PROJ='JMC' AND BalanceSheetType=1



-- PAYMENT ACCOUNTS
INSERT INTO tblGLAccounts
           ([GLAccountNumber]           ,[GLAccountName]           ,[GLAccountType]           ,[CreditBalance]           ,[DebitBalance]
           ,[BeginningBalance]           ,[BalanceSheetType]           ,[ContraID]           ,[BegBalType]           ,[Active]
           ,[DeleteFlag]           ,[Bank#]           ,[BankOpeningBalance]           ,[BudgetType]           ,[BudgetAmount]
           ,[Period1]           ,[Period2]           ,[Period3]           ,[Period4]           ,[Period5]
           ,[Period6]           ,[Period7]           ,[Period8]           ,[Period9]           ,[Period10]
            ,[Period11]           ,[Period12]           ,[SubAcctID]           ,[MainAccountID]         ,[Seperator]
           ,[GeoCode]           ,[Proj])
SELECT '10010-100','FGB JA$ CURRENT A/C',[GLAccountType]           ,[CreditBalance]           ,[DebitBalance]
           ,[BeginningBalance]           ,[BalanceSheetType]           ,[ContraID]           ,[BegBalType]           ,1
           ,[DeleteFlag]           ,[Bank#]           ,[BankOpeningBalance]           ,[BudgetType]           ,[BudgetAmount]
           ,[Period1]           ,[Period2]           ,[Period3]           ,[Period4]           ,[Period5]
           ,[Period6]           ,[Period7]           ,[Period8]           ,[Period9]           ,[Period10]
            ,[Period11]           ,[Period12]           ,[SubAcctID]           ,[MainAccountID]         ,[Seperator]
           ,[GeoCode]           ,'JMC' 
FROM    tblGLAccounts        
           WHERE GLAccountID=137

UPDATE dbo.tblCompanyProfile
SET DefaultARGLBank=SCOPE_IDENTITY()
WHERE proj='JMC'
           
INSERT INTO tblGLAccounts
           ([GLAccountNumber]           ,[GLAccountName]           ,[GLAccountType]           ,[CreditBalance]           ,[DebitBalance]
           ,[BeginningBalance]           ,[BalanceSheetType]           ,[ContraID]           ,[BegBalType]           ,[Active]
           ,[DeleteFlag]           ,[Bank#]           ,[BankOpeningBalance]           ,[BudgetType]           ,[BudgetAmount]
           ,[Period1]           ,[Period2]           ,[Period3]           ,[Period4]           ,[Period5]
           ,[Period6]           ,[Period7]           ,[Period8]           ,[Period9]           ,[Period10]
            ,[Period11]           ,[Period12]           ,[SubAcctID]           ,[MainAccountID]         ,[Seperator]
           ,[GeoCode]           ,[Proj])
        
           
SELECT '10012-100','FGB US$ SAVINGS A/C',[GLAccountType]           ,[CreditBalance]           ,[DebitBalance]
           ,[BeginningBalance]           ,[BalanceSheetType]           ,[ContraID]           ,[BegBalType]           ,1
           ,[DeleteFlag]           ,[Bank#]           ,[BankOpeningBalance]           ,[BudgetType]           ,[BudgetAmount]
           ,[Period1]           ,[Period2]           ,[Period3]           ,[Period4]           ,[Period5]
           ,[Period6]           ,[Period7]           ,[Period8]           ,[Period9]           ,[Period10]
            ,[Period11]           ,[Period12]           ,[SubAcctID]           ,[MainAccountID]         ,[Seperator]
           ,[GeoCode]           ,'JMC'
FROM    tblGLAccounts        
           WHERE GLAccountID=137           
INSERT INTO tblGLAccounts
           ([GLAccountNumber]           ,[GLAccountName]           ,[GLAccountType]           ,[CreditBalance]           ,[DebitBalance]
           ,[BeginningBalance]           ,[BalanceSheetType]           ,[ContraID]           ,[BegBalType]           ,[Active]
           ,[DeleteFlag]           ,[Bank#]           ,[BankOpeningBalance]           ,[BudgetType]           ,[BudgetAmount]
           ,[Period1]           ,[Period2]           ,[Period3]           ,[Period4]           ,[Period5]
           ,[Period6]           ,[Period7]           ,[Period8]           ,[Period9]           ,[Period10]
            ,[Period11]           ,[Period12]           ,[SubAcctID]           ,[MainAccountID]         ,[Seperator]
           ,[GeoCode]           ,[Proj])
SELECT '10020-100','NCB JA$ SAVINGS A/C',[GLAccountType]           ,[CreditBalance]           ,[DebitBalance]
           ,[BeginningBalance]           ,[BalanceSheetType]           ,[ContraID]           ,[BegBalType]           ,1
           ,[DeleteFlag]           ,[Bank#]           ,[BankOpeningBalance]           ,[BudgetType]           ,[BudgetAmount]
           ,[Period1]           ,[Period2]           ,[Period3]           ,[Period4]           ,[Period5]
           ,[Period6]           ,[Period7]           ,[Period8]           ,[Period9]           ,[Period10]
            ,[Period11]           ,[Period12]           ,[SubAcctID]           ,[MainAccountID]         ,[Seperator]
           ,[GeoCode]           ,'JMC'                      
FROM    tblGLAccounts        
           WHERE GLAccountID=137           


-- A/R ACCOUNTS

UPDATE tblGLAccounts
SET ACTIVE=0
WHERE PROJ='JMC' AND BalanceSheetType=2 --143

INSERT INTO tblGLAccounts
           ([GLAccountNumber]           ,[GLAccountName]           ,[GLAccountType]           ,[CreditBalance]           ,[DebitBalance]
           ,[BeginningBalance]           ,[BalanceSheetType]           ,[ContraID]           ,[BegBalType]           ,[Active]
           ,[DeleteFlag]           ,[Bank#]           ,[BankOpeningBalance]           ,[BudgetType]           ,[BudgetAmount]
           ,[Period1]           ,[Period2]           ,[Period3]           ,[Period4]           ,[Period5]
           ,[Period6]           ,[Period7]           ,[Period8]           ,[Period9]           ,[Period10]
            ,[Period11]           ,[Period12]           ,[SubAcctID]           ,[MainAccountID]         ,[Seperator]
           ,[GeoCode]           ,[Proj])
SELECT '10100-100','A/R Control – Regulatory Fees',[GLAccountType]           ,[CreditBalance]           ,[DebitBalance]
           ,[BeginningBalance]           ,[BalanceSheetType]           ,[ContraID]           ,[BegBalType]           ,1
           ,[DeleteFlag]           ,[Bank#]           ,[BankOpeningBalance]           ,[BudgetType]           ,[BudgetAmount]
           ,[Period1]           ,[Period2]           ,[Period3]           ,[Period4]           ,[Period5]
           ,[Period6]           ,[Period7]           ,[Period8]           ,[Period9]           ,[Period10]
            ,[Period11]           ,[Period12]           ,[SubAcctID]           ,[MainAccountID]         ,[Seperator]
           ,[GeoCode]           ,'JMC'
FROM    tblGLAccounts        
           WHERE GLAccountID=143           
INSERT INTO tblGLAccounts
           ([GLAccountNumber]           ,[GLAccountName]           ,[GLAccountType]           ,[CreditBalance]           ,[DebitBalance]
           ,[BeginningBalance]           ,[BalanceSheetType]           ,[ContraID]           ,[BegBalType]           ,[Active]
           ,[DeleteFlag]           ,[Bank#]           ,[BankOpeningBalance]           ,[BudgetType]           ,[BudgetAmount]
           ,[Period1]           ,[Period2]           ,[Period3]           ,[Period4]           ,[Period5]
           ,[Period6]           ,[Period7]           ,[Period8]           ,[Period9]           ,[Period10]
            ,[Period11]           ,[Period12]           ,[SubAcctID]           ,[MainAccountID]         ,[Seperator]
           ,[GeoCode]           ,[Proj])
SELECT '10102-100','Spectrum Fees Control',[GLAccountType]           ,[CreditBalance]           ,[DebitBalance]
           ,[BeginningBalance]           ,[BalanceSheetType]           ,[ContraID]           ,[BegBalType]           ,1
           ,[DeleteFlag]           ,[Bank#]           ,[BankOpeningBalance]           ,[BudgetType]           ,[BudgetAmount]
           ,[Period1]           ,[Period2]           ,[Period3]           ,[Period4]           ,[Period5]
           ,[Period6]           ,[Period7]           ,[Period8]           ,[Period9]           ,[Period10]
            ,[Period11]           ,[Period12]           ,[SubAcctID]           ,[MainAccountID]         ,[Seperator]
           ,[GeoCode]           ,'JMC'                      
FROM    tblGLAccounts        
WHERE GLAccountID=143     

--265

UPDATE dbo.tblGLAccounts
SET ACTIVE=0
WHERE PROJ='JMC' AND (BalanceSheetType=11 OR GLAccountType=2)


CREATE TABLE #arAccounts 
    (accountNumber VARCHAR(50),
     accountName  VARCHAR(250))

INSERT     #arAccounts  VALUES ('10102-100','Spectrum Fees Control')
INSERT     #arAccounts VALUES ('50001-100-PFCC','Processing Fees – Customs Clearance')
INSERT     #arAccounts VALUES ('50001-100-PFDN','Processing Fees – Detention Notice')
INSERT     #arAccounts VALUES ('50001-100-PFNA','Processing Fees – New Application')
INSERT     #arAccounts VALUES ('50001-100-PFTA','Processing Fees – Type Approval')
INSERT     #arAccounts VALUES ('50010-100-RFCC','Reg Fee – Cell/Mobile')
INSERT     #arAccounts VALUES ('50011-100-RFCC','Reg Fee – P/R Commercial - Broadband')
INSERT     #arAccounts VALUES ('50012-100-RFCC','Reg Fee – P/R Commercial - Microwave')
INSERT     #arAccounts VALUES ('50013-100-RFCC','Reg Fee – P/R Commercial – Data & Service Provider')
INSERT     #arAccounts VALUES ('50014-100-RFCC','Reg Fee – P/R Commercial - VSAT')
INSERT     #arAccounts VALUES ('50020-100-RFNC','Reg Fee – P/R Non-Commercial - Aeronautical')
INSERT     #arAccounts VALUES ('50021-100-RFNC','Reg Fee – P/R Non-Commercial - Marine')
INSERT     #arAccounts VALUES ('50022-100-RFNC','Reg Fee – P/R Non-Commercial - Trunking')
INSERT     #arAccounts VALUES ('50023-100-RFNC','Reg Fee – P/R Non-Commercial - Other')

INSERT INTO tblGLAccounts
           ([GLAccountNumber]           ,[GLAccountName]           ,[GLAccountType]           ,[CreditBalance]           ,[DebitBalance]
           ,[BeginningBalance]           ,[BalanceSheetType]           ,[ContraID]           ,[BegBalType]           ,[Active]
           ,[DeleteFlag]           ,[Bank#]           ,[BankOpeningBalance]           ,[BudgetType]           ,[BudgetAmount]
           ,[Period1]           ,[Period2]           ,[Period3]           ,[Period4]           ,[Period5]
           ,[Period6]           ,[Period7]           ,[Period8]           ,[Period9]           ,[Period10]
            ,[Period11]           ,[Period12]           ,[SubAcctID]           ,[MainAccountID]         ,[Seperator]
           ,[GeoCode]           ,[Proj])
SELECT a.accountnumber,a.accountname, [GLAccountType]           ,[CreditBalance]           ,[DebitBalance]
           ,[BeginningBalance]           ,[BalanceSheetType]           ,[ContraID]           ,[BegBalType]           ,1
           ,[DeleteFlag]           ,[Bank#]           ,[BankOpeningBalance]           ,[BudgetType]           ,[BudgetAmount]
           ,[Period1]           ,[Period2]           ,[Period3]           ,[Period4]           ,[Period5]
           ,[Period6]           ,[Period7]           ,[Period8]           ,[Period9]           ,[Period10]
            ,[Period11]           ,[Period12]           ,[SubAcctID]           ,[MainAccountID]         ,[Seperator]
           ,[GeoCode]           ,[Proj]
FROM        tblGLAccounts ,#araccounts   a          
WHERE  GLAccountID=265
