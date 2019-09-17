using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using middleware_service.Database_Classes;

namespace middleware_service.Database_Operations
{
    public static class Integration
    {
        private const string CURRENT_GENERIC_CONNECTION = Constants.DB_GENERIC;
        private const string CURRENT_INTEGRATION_CONNECTION = Constants.DB_INTEGRATION;
      
        public static bool IsBrokerEnabled(string databaseName, string databaseConnection = CURRENT_GENERIC_CONNECTION)
        {
            bool result;
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "select name, database_id, IS_BROKER_ENABLED from sys.databases where name=@dbname";
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@dbname", databaseName);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            if (Convert.ToInt32(reader["IS_BROKER_ENABLED"]) == 1)
                            {
                                result = true;
                            }
                            else
                            {
                                result = false;
                            }
                        }
                        else
                        {
                            result = false;
                        }
                    }
                }
            }
            return result;
        }

        public static void SetBrokerEnabled(string databaseName, string databaseConnection = CURRENT_GENERIC_CONNECTION)
        {

            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "alter database " + databaseName + " set ENABLE_BROKER WITH ROLLBACK IMMEDIATE";
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public static List<ReportRawData> GetDIRInformation(string ReportType, DateTime searchStartDate, DateTime searchEndDate, string databaseConnection = CURRENT_INTEGRATION_CONNECTION)
        {
            List<ReportRawData> reportInfo = new List<ReportRawData>();
            ReportRawData record = new ReportRawData();

            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                #region lengthQuery
                string query = "CREATE TABLE #AllRecordsTable" +
                            "(" +
                            "clientID int," +
                            "ccNum varchar(25)," +
                            "clientCompany varchar(250)," +
                            "clientFname varchar(30)," +
                            "clientLname varchar(30)," +
                            "budget varchar(50)," +
                            "InvAmount money," +
                            "ExistedBefore bit," +
                            "LastRptsClosingBal varchar(50)," +
                            "LastRptsStartValPeriod varchar(100)," +
                            "LastRptsEndValPeriod varchar(100)," +
                            "CurrentStartValPeriod datetime," +
                            "CurrentEndValPeriod datetime," +
                            "currCreditGLID int," +
                            "notes varchar(255)," +
                            "ARInvoiceID varchar(50)," +
                            "InvoiceCreationDate datetime," +
                            "isCancelled bit," +
                            "isCreditMemo bit," +
                            "CreditMemoNum varchar(50)," +
                            "CreditMemoAmt money" +
                            ");";

                query += "CREATE TABLE #ModificationRecordsTable" +
                        "(" +
                        "clientID int," +
                        "ccNum varchar(25)," +
                        "clientCompany varchar(250)," +
                        "clientFname varchar(30)," +
                        "clientLname varchar(30)," +
                        "budget varchar(50)," +
                        "InvAmount money," +
                        "ExistedBefore int," +
                        "LastRptsClosingBal varchar(50)," +
                        "LastRptsStartValPeriod varchar(100)," +
                        "LastRptsEndValPeriod varchar(100)," +
                        "CurrentStartValPeriod datetime," +
                        "CurrentEndValPeriod datetime," +
                        "currCreditGLID int," +
                        "notes varchar(255)," +
                        "ARInvoiceID varchar(50)," +
                        "InvoiceCreationDate datetime," +
                        "DebitTotal money," +
                        "isCancelled bit," +
                        "isCreditMemo bit," +
                        "CreditMemoNum varchar(50)," +
                        "CreditMemoAmt money" +
                        ");";

                query += "CREATE TABLE #CancellationRecordsTable" +
                        "(" +
                        "clientID int," +
                        "ccNum varchar(25)," +
                        "clientCompany varchar(250)," +
                        "clientFname varchar(30)," +
                        "clientLname varchar(30)," +
                        "budget varchar(50)," +
                        "InvAmount money," +
                        "ExistedBefore int," +
                        "LastRptsClosingBal varchar(50)," +
                        "LastRptsStartValPeriod varchar(100)," +
                        "LastRptsEndValPeriod varchar(100)," +
                        "CurrentStartValPeriod datetime," +
                        "CurrentEndValPeriod datetime," +
                        "currCreditGLID int," +
                        "notes varchar(255)," +
                        "ARInvoiceID varchar(50)," +
                        "InvoiceCreationDate datetime," +
                        "isCancelled bit," +
                        "isCreditMemo bit," +
                        "CreditMemoNum varchar(50)," +
                        "CreditMemoAmt money" +
                        ");";

                query += "CREATE TABLE #AllCreditMemosTable" +
                        "(" +
                        "ARInvoiceID varchar(50)," +
                        "CreditMemoNum varchar(50)," +
                        "CreditMemoAmt money" +
                        ");";

                query += "CREATE TABLE #CreditMemoInvoicesTobeDeletedTable" +
                        "(" +
                        "ARInvoiceID varchar(50)" +
                        ");";

                query += "CREATE TABLE #LastRptsInfoTable" +
                        "(" +
                        "ARInvoiceID varchar(50)," +
                        "Budget varchar(50)," +
                        "ClosingBal varchar(50)," +
                        "StartValPeriod varchar(50)," +
                        "EndValPeriod varchar(50)," +
                        "CreditGLID int" +
                        ");";

                query += "CREATE TABLE #CreditGLIDInvoicesTobeDeletedTable" +
                        "(" +
                        "ARInvoiceID varchar(50)" +
                        ");";

                query += "INSERT INTO #AllRecordsTable (clientID, ccNum, clientCompany, clientFname, clientLname, budget, InvAmount, ExistedBefore, LastRptsClosingBal, LastRptsStartValPeriod, " +
                        "LastRptsEndValPeriod, CurrentStartValPeriod, CurrentEndValPeriod, currCreditGLID, notes, ARInvoiceID, InvoiceCreationDate, isCancelled, isCreditMemo, CreditMemoNum, CreditMemoAmt)" +
                        "SELECT distinct cus.clientID, cus.ccNum, cus.clientCompany, cus.clientFname, cus.clientLname, 0, inv.Amount, 0, '', '', ''," +
                        "val.ValidFrom, val.ValidTo, GL.CreditGLID, inv.notes, inv.ARInvoiceID, inv.InvoiceCreationDate, 0, 0, '', '' " +
                        "FROM[ASMSGenericMaster].[dbo].[tblARInvoices] inv, [ASMSGenericMaster].[dbo].[tbl_LicenseValidityHistory] val," +
                        "[ASMSGenericMaster].[dbo].[Client] cus, [ASMSGenericMaster].[dbo].[tblARInvoiceDetail] GL " +
                        "where inv.LicensevalidityHistoryID = val.LicenseValidityHistoryID " +
                        "AND inv.isvoided!=1 " +
                        "AND inv.notes!= 'Type Approval' " +
                        "AND val.ClientID = cus.clientID " +
                        "AND (cus.ccNum is not null) " +
                        "AND inv.FeeType!='SLF' " +
                        "AND inv.notes!='Radio Operator' " +
                        "AND inv.notes != 'Modification' " +
                        "AND val.ValidTo >= @searchStartDate " +
                        "AND val.ValidFrom<@searchEndDate + 1 " +
                        "AND inv.ARInvoiceID = GL.ARInvoiceID " +
                        "AND GL.CreditGLID >=5156 " +
                        "AND val.LicenseID != 0 " +
                        "Group by cus.clientID, cus.ccNum, cus.clientCompany, cus.clientFname, cus.clientLname, inv.notes, " +
                        "inv.FeeType, inv.ARInvoiceID, inv.Amount, val.ValidFrom, val.ValidTo, GL.CreditGLID, GL.Description, " +
                        "inv.isvoided, inv.InvoiceCreationDate order by cus.ccNum desc; ";

                query += "INSERT INTO #AllRecordsTable (clientID, ccNum, clientCompany, clientFname, clientLname, budget, InvAmount, ExistedBefore, LastRptsClosingBal, LastRptsStartValPeriod, " +
                        "LastRptsEndValPeriod, CurrentStartValPeriod, CurrentEndValPeriod, currCreditGLID, notes, ARInvoiceID, InvoiceCreationDate, isCancelled, isCreditMemo, CreditMemoNum, CreditMemoAmt) " +
                        "SELECT distinct cus.clientID, cus.ccNum, cus.clientCompany, cus.clientFname, cus.clientLname, 0, inv.Amount, 0, '', '', '', " +
                        "val.ValidFrom, val.ValidTo, GL.CreditGLID, inv.notes, inv.ARInvoiceID, inv.InvoiceCreationDate, 0, 0, '', '' " +
                        "FROM[ASMSGenericMaster].[dbo].[tblARInvoices] inv, [ASMSGenericMaster].[dbo].[tbl_LicenseValidityHistory] val, " +
                        "[ASMSGenericMaster].[dbo].[client] cus, [ASMSGenericMaster].[dbo].[tblARInvoiceDetail] GL " +
                        "WHERE inv.LicensevalidityHistoryID = val.LicenseValidityHistoryID " +
                        "AND inv.isvoided!=1 " +
                        "AND inv.notes!= 'Type Approval' " +
                        "AND val.ClientID = cus.clientID " +
                        "AND (cus.ccNum is not null) " +
                        "AND inv.FeeType!='SLF' " +
                        "AND inv.notes!='Radio Operator' " +
                        "AND inv.notes != 'Modification' " +
                        "AND val.ValidTo >= @searchStartDate " +
                        "AND val.ValidFrom<@searchEndDate + 1 " +
                        "AND inv.ARInvoiceID = GL.ARInvoiceID " +
                        "AND GL.CreditGLID >=5156 " +
                        "AND inv.CustomerID = 9569 " +
                        "Group by cus.clientID, cus.ccNum, cus.clientCompany, cus.clientFname, cus.clientLname, inv.notes, " +
                        "inv.FeeType, inv.ARInvoiceID, inv.Amount, val.ValidFrom, val.ValidTo, GL.CreditGLID, GL.Description, " +
                        "inv.isvoided, inv.InvoiceCreationDate order by cus.ccNum desc ";

                query += "INSERT INTO #AllRecordsTable (clientID, ccNum, clientCompany, clientFname, clientLname, budget, InvAmount, ExistedBefore, LastRptsClosingBal, LastRptsStartValPeriod, " +
                        "LastRptsEndValPeriod, CurrentStartValPeriod, CurrentEndValPeriod, currCreditGLID, notes, ARInvoiceID, InvoiceCreationDate, isCancelled, isCreditMemo, CreditMemoNum, CreditMemoAmt) " +
                        "SELECT distinct cus.clientID, cus.ccNum, cus.clientCompany, cus.clientFname, cus.clientLname, 0, inv.Amount, 0, '', '', '', " +
                        "val.ValidFrom, val.ValidTo, GL.CreditGLID, inv.notes, inv.ARInvoiceID, inv.InvoiceCreationDate, 0, 0, '', '' " +
                        "FROM[ASMSGenericMaster].[dbo].[tblARInvoices] inv, [ASMSGenericMaster].[dbo].[tbl_LicenseValidityHistory] val, " +
                        "[ASMSGenericMaster].[dbo].[client] cus, [ASMSGenericMaster].[dbo].[tblARInvoiceDetail] GL " +
                        "WHERE inv.LicensevalidityHistoryID = val.LicenseValidityHistoryID " +
                        "AND inv.isvoided!=1 " +
                        "AND inv.notes!= 'Type Approval' " +
                        "AND val.ClientID = cus.clientID " +
                        "AND (cus.ccNum is not null) " +
                        "AND inv.FeeType!='SLF' " +
                        "AND inv.notes!='Radio Operator' " +
                        "AND inv.notes != 'Modification' " +
                        "AND val.ValidTo >= @searchStartDate " +
                        "AND val.ValidFrom<@searchEndDate + 1 " +
                        "AND inv.ARInvoiceID = GL.ARInvoiceID " +
                        "AND GL.CreditGLID >=5156 " +
                        "AND inv.CustomerID = 9882 " +
                        "Group by cus.clientID, cus.ccNum, cus.clientCompany, cus.clientFname, cus.clientLname, inv.notes, " +
                        "inv.FeeType, inv.ARInvoiceID, inv.Amount, val.ValidFrom, val.ValidTo, GL.CreditGLID, GL.Description, " +
                        "inv.isvoided, inv.InvoiceCreationDate order by cus.ccNum desc ";

                query += "INSERT INTO #ModificationRecordsTable (clientID, ccNum, clientCompany, clientFname, clientLname, budget, InvAmount, ExistedBefore, LastRptsClosingBal, LastRptsStartValPeriod, " +
                        "LastRptsEndValPeriod, CurrentStartValPeriod, CurrentEndValPeriod, currCreditGLID, notes, ARInvoiceID, InvoiceCreationDate, DebitTotal, isCancelled, isCreditMemo, CreditMemoNum, CreditMemoAmt) " +
                        "SELECT distinct  cus.clientID, cus.ccNum, cus.clientCompany, cus.clientFname, cus.clientLname, 0, inv.Amount, 0, '', '', '', val.ValidFrom, val.ValidTo, GL.CreditGLID, " +
                        "inv.notes, inv.ARInvoiceID, inv.InvoiceCreationDate, SUM(receipt.debit) as DebitTotal, 0, 0, '', '' FROM[ASMSGenericMaster].[dbo].[tblARInvoices] inv, " +
                        "[ASMSGenericMaster].[dbo].[tbl_LicenseValidityHistory] val, [ASMSGenericMaster].[dbo].[client] cus, " +
                        "[ASMSGenericMaster].[dbo].[tblARInvoiceDetail] GL, [ASMSGenericMaster].[dbo].[tblARPayments] receipt " +
                        "where inv.LicensevalidityHistoryID = val.LicenseValidityHistoryID " +
                        "AND inv.isvoided!=1 " +
                        "AND inv.notes!= 'Type Approval' " +
                        "AND val.ClientID = cus.clientID " +
                        "AND (cus.ccNum is not null) " +
                        "AND inv.FeeType!='SLF' " +
                        "AND inv.notes!='Radio Operator' " +
                        "AND inv.notes = 'Modification' " +
                        "AND val.ValidTo >= @searchStartDate " +
                        "AND val.ValidFrom<@searchEndDate + 1 " +
                        "AND inv.ARInvoiceID = GL.ARInvoiceID " +
                        "AND GL.CreditGLID >=5156 " +
                        "AND receipt.InvoiceID = inv.ARInvoiceID " +
                        "Group by cus.clientID, cus.ccNum, cus.clientCompany, cus.clientFname, cus.clientLname, inv.notes, " +
                        "inv.FeeType, inv.ARInvoiceID, inv.Amount, val.ValidFrom, val.ValidTo, GL.CreditGLID, GL.Description, " +
                        "inv.isvoided, inv.InvoiceCreationDate order by cus.ccNum desc ";

                query += "INSERT INTO #CancellationRecordsTable (clientID, ccNum, clientCompany, clientFname, clientLname, budget, InvAmount, ExistedBefore, LastRptsClosingBal, " +
                        "LastRptsStartValPeriod, LastRptsEndValPeriod, currCreditGLID, notes, ARInvoiceID, InvoiceCreationDate, isCancelled, isCreditMemo, CreditMemoNum, CreditMemoAmt) " +
                        "SELECT distinct cus.clientID, cus.ccNum, cus.clientCompany, cus.clientFname, cus.clientLname, 0, inv.Amount, " +
                        "0, '', '', '', GL.CreditGLID, inv.notes, inv.ARInvoiceID, inv.InvoiceCreationDate, inv.isvoided, 0, '', '' " +
                        "FROM[ASMSGenericMaster].[dbo].[tblARInvoices] inv,	[ASMSGenericMaster].[dbo].[client] cus, " +
                        "[ASMSGenericMaster].[dbo].[tblARInvoiceDetail] GL " +
                        "WHERE inv.isvoided = 1 " +
                        "AND inv.canceledDate >= @searchStartDate " +
                        "AND inv.notes!= 'Type Approval' " +
                        "AND cus.clientID = inv.CustomerID " +
                        "AND (cus.ccNum is not null) " +
                        "AND inv.FeeType!='SLF' " +
                        "AND inv.notes!='Radio Operator' " +
                        "AND inv.ARInvoiceID = GL.ARInvoiceID " +
                        "AND GL.CreditGLID >=5156 " +
                        "Group by cus.clientID, cus.ccNum, cus.clientCompany, cus.clientFname, cus.clientLname, inv.notes, " +
                        "inv.FeeType, inv.ARInvoiceID, inv.Amount, GL.CreditGLID, GL.Description, " +
                        "inv.isvoided, inv.InvoiceCreationDate order by cus.ccNum desc ";

                query += "INSERT INTO #AllRecordsTable (clientID, ccNum, clientCompany, clientFname, clientLname, budget, InvAmount, ExistedBefore, LastRptsClosingBal, LastRptsStartValPeriod, " +
                        "LastRptsEndValPeriod, CurrentStartValPeriod, CurrentEndValPeriod, currCreditGLID, notes, ARInvoiceID, InvoiceCreationDate, isCancelled, isCreditMemo, CreditMemoNum, CreditMemoAmt) " +
                        "SELECT distinct clientID, ccNum, clientCompany, clientFname, clientLname, budget, InvAmount, ExistedBefore, LastRptsClosingBal, LastRptsStartValPeriod, LastRptsEndValPeriod, " +
                        "CurrentStartValPeriod, CurrentEndValPeriod, currCreditGLID, notes, ARInvoiceID, InvoiceCreationDate, isCancelled, isCreditMemo, CreditMemoNum, CreditMemoAmt " +
                        "FROM #ModificationRecordsTable where DebitTotal >= InvAmount ";

                query += "INSERT INTO #AllRecordsTable (clientID, ccNum, clientCompany, clientFname, clientLname, budget, InvAmount, ExistedBefore, LastRptsClosingBal, LastRptsStartValPeriod, " +
                        "LastRptsEndValPeriod, CurrentStartValPeriod, CurrentEndValPeriod, currCreditGLID, notes, ARInvoiceID, InvoiceCreationDate, isCancelled, isCreditMemo, CreditMemoNum, CreditMemoAmt) " +
                        "SELECT DISTINCT clientID, ccNum, clientCompany, clientFname, clientLname, budget, InvAmount, ExistedBefore, LastRptsClosingBal, LastRptsStartValPeriod, " +
                        "LastRptsEndValPeriod, @searchEndDate, @searchEndDate, currCreditGLID, notes, ARInvoiceID, InvoiceCreationDate, isCancelled, isCreditMemo, CreditMemoNum, CreditMemoAmt " +
                        "FROM #CancellationRecordsTable ";

                query += "INSERT INTO #AllCreditMemosTable (ARInvoiceID, CreditMemoNum, CreditMemoAmt) SELECT distinct CN.RelatedInvoice, DOC.DocumentDisplayNumber, CN.amount " +
                        "FROM[ASMSGenericMaster].[dbo].[tblGLDocuments] DOC, [ASMSGenericMaster].[dbo].[ARCreditMemo] CN " +
                        "WHERE DOC.DocumentType = 5 AND DOC.OriginalDocumentID = CN.CreditMemoId AND CN.date1 <= @searchEndDate ";

                query += "declare @count as integer " +
                        "declare @invoice as varchar (50) " +
                        "declare @creditMemoNum as varchar (50) " +
                        "declare @creditMemoAmt as money " +
                        "declare @i as integer " +
                        "set @i = 0 " +
                        "set @count = (select count(*) from #AllCreditMemosTable) " +
                        "while (@i < @count) " +
                                    "begin " +
                                        "set @invoice = (SELECT TOP 1 ARInvoiceID FROM #AllCreditMemosTable order by ARInvoiceID asc) " +
                                        "set @creditMemoNum = (SELECT TOP 1 CreditMemoNum FROM #AllCreditMemosTable order by ARInvoiceID asc) " +
                                        "set @creditMemoAmt = (SELECT TOP 1 CreditMemoAmt FROM #AllCreditMemosTable order by ARInvoiceID asc) " +
                                        "UPDATE #AllRecordsTable SET isCreditMemo = 1, CreditMemoNum = 'CN' + @creditMemoNum, CreditMemoAmt = @creditMemoAmt WHERE ARInvoiceID = @invoice " +
                                        "DELETE FROM #AllCreditMemosTable WHERE ARInvoiceID =  @invoice " +
                                        "set @i = @i + 1 " +
                        "end ";

                query += "INSERT INTO #CreditMemoInvoicesToBeDeletedTable (ARInvoiceID) SELECT distinct CN.RelatedInvoice " +
                        "FROM[ASMSGenericMaster].[dbo].[tblGLDocuments] DOC, [ASMSGenericMaster].[dbo].[ARCreditMemo] CN " +
                        "WHERE DOC.DocumentType = 5 AND DOC.OriginalDocumentID = CN.CreditMemoId AND CN.date1<@searchStartDate ";

                query += "if (@ReportType = 'Annual' AND YEAR(@searchStartDate) = 2017) " +
                        "begin " +
                            "DELETE FROM #CreditMemoInvoicesToBeDeletedTable WHERE ARInvoiceID = '14613' " +
                            "DELETE FROM #CreditMemoInvoicesToBeDeletedTable WHERE ARInvoiceID = '12607' " +
                            "DELETE FROM #CreditMemoInvoicesToBeDeletedTable WHERE ARInvoiceID = '13695' " +
                            "DELETE FROM #CreditMemoInvoicesToBeDeletedTable WHERE ARInvoiceID = '14370' " +
                            "DELETE FROM #CreditMemoInvoicesToBeDeletedTable WHERE ARInvoiceID = '12099' " +
                            "INSERT INTO #AllRecordsTable values ('','01013-1', 'Leslie West','','','0.00','35,000.00',1,'14,583.33','08/09/2016','07/09/2017','2016-09-08','2017-09-07','5164','','14759','',0,0,'','') " +
                            "INSERT INTO #AllRecordsTable values ('','00205-4', 'Digicel (Jamaica) Limited','','','0.00','2,274,048.00',1,'947,520.00','31/08/2016','30/08/2017','2016-08-31','2017-08-31','5158','','14391','',0,0,'','') " +
                            "INSERT INTO #AllRecordsTable values ('9966','01307-1', 'Posttopost Betting, Limited', '','','0.00','350,000.00',1,'145,833.33','08/09/2016','07/09/2017','2016-09-08','2017-09-07','5159','','14512','',0,0,'','') " +
                            "INSERT INTO #AllRecordsTable values ('8378','00724-2', 'Axel Sonnenberg', '','','0.00','4,375.00',1,'729.17','01/11/2016','31/10/2017','2016-11-01','2017-10-31','5161','','14853','',0,0,'','') " +
                            "INSERT INTO #AllRecordsTable values ('','00372-2', 'Mustard Seed Communities', '','','0.00','',1,'-3,045.00','25/10/2016','24/10/2017','2016-10-25','2017-10-24','5164','','','',0,0,'','') " +
                            "INSERT INTO #AllRecordsTable values ('','00046-3', 'Guardsman Communications Limited', '','','0.00','',1,'29,166.67','01/04/2016','31/03/2017','2016-04-01','2017-03-31','5163','','','',0,0,'','') " +
                        "end ";

                query += "set @count = (select count(*) from #CreditMemoInvoicesToBeDeletedTable) " +
                        "set @i = 0 " +
                        "while (@i < @count) " +
                        "begin " +
                            "set @invoice = (SELECT TOP 1 * FROM #CreditMemoInvoicesToBeDeletedTable) " +
                            "DELETE FROM #AllRecordsTable WHERE ARInvoiceID = @invoice " +
                            "DELETE FROM #CreditMemoInvoicesToBeDeletedTable WHERE ARInvoiceID =  @invoice " +
                            "set @i = @i + 1 " +
                         "end ";

                query += "INSERT INTO #CreditGLIDInvoicesTobeDeletedTable (ARInvoiceID) SELECT distinct tbl1.ARInvoiceID " +
                        "FROM #AllRecordsTable tbl1, #AllRecordsTable tbl2 WHERE tbl1.ARInvoiceID = tbl2.ARInvoiceID " +
                        "AND tbl1.currCreditGLID != tbl2.currCreditGLID " +
                        "declare @creditGLID as int " +
                        "set @count = (select count(*) from #CreditGLIDInvoicesToBeDeletedTable) " +
                        "set @i = 0 " +
                        "while (@i < @count) " +
                         "begin " +
                            "set @invoice = (SELECT TOP 1 * FROM #CreditGLIDInvoicesToBeDeletedTable) " +
                            "set @creditGLID = (SELECT TOP 1 CreditGLID from[ASMSGenericMaster].[dbo].[tblARInvoiceDetail] WHERE ARInvoiceID = @invoice ORDER BY ARInvoiceDetailID) " +
                            "DELETE FROM #AllRecordsTable WHERE ARInvoiceID = @invoice AND currCreditGLID != @creditGLID " +
                            "DELETE FROM #CreditGLIDInvoicesTobeDeletedTable WHERE ARInvoiceID =  @invoice " +
                            "set @i = @i + 1 " +
                        "end ";

                query += "declare @category as varchar (50) " +
                        "declare @sql as nvarchar (max) " +
                        "declare @report_id as varchar (100) " +
                        "set @count = (select count(*) from DIRCategories) " +
                        "set @i = 0 " +
                        "CREATE table #ReportIDTable (ReportID varchar (100)) " +
                        "set @sql = 'select report_id from ' + @ReportType + 'DIR_ReportMain where MONTH(report_date) = ' + " +
                                    "cast(MONTH(@searchStartDate) as varchar(10)) + ' and YEAR(report_date) = ' + cast(YEAR(@searchStartDate) as varchar (10)) + ';' " +
                        "INSERT INTO #ReportIDTable exec (@sql) " +
                        "set @report_id = (select ReportID from #ReportIDTable) " +
                        "while (@i < @count) " +
                        "begin " +
                            "select @category = Description from DIRCategories where Category = @i " +
                            "select @creditGLID = CreditGLID from DIRcategories where Category = @i " +
                            "set @sql = 'SELECT distinct invoiceID, budget, closingBalance, validityStart, validityEnd, ' + cast(@creditGLID as varchar (10)) + ' FROM ' + @ReportType + 'DIR_' + " +
                                        "@category + ' WHERE report_id = ' + @report_id + ';' " +
                            "INSERT INTO #LastRptsInfoTable (ARInvoiceID, Budget, ClosingBal, StartValPeriod, EndValPeriod, CreditGLID) exec (@sql) " +
                            "set @i = @i + 1 " +
                        "end " +
                        "DELETE FROM #LastRptsInfoTable WHERE ClosingBal = '0.00' ";

                query += "declare @budget as varchar (50) " +
                        "declare @LastRptsClosingBal as varchar (50) " +
                        "declare @LastRptsStartValPeriod as varchar (100) " +
                        "declare @LastRptsEndValPeriod as varchar (100) " +
                        "declare @LastRptsCreditGLID as int " +
                        "set @i = 0 " +
                        "set @count = (select count(*) from #LastRptsInfoTable) " +
                        "while (@i < @count) " +
                        "begin " +
                            "set @invoice = (SELECT TOP 1 ARInvoiceID FROM #LastRptsInfoTable order by ARInvoiceID asc) " +
                            "set @budget = (SELECT TOP 1 Budget FROM #LastRptsInfoTable order by ARInvoiceID asc) " +
                            "set @LastRptsClosingBal = (SELECT TOP 1 ClosingBal FROM #LastRptsInfoTable order by ARInvoiceID asc) " +
                            "set @LastRptsStartValPeriod = (SELECT TOP 1 StartValPeriod FROM #LastRptsInfoTable order by ARInvoiceID asc) " +
                            "set @LastRptsEndValPeriod = (SELECT TOP 1 EndValPeriod FROM #LastRptsInfoTable order by ARInvoiceID asc) " +
                            "set @LastRptsCreditGLID = (SELECT TOP 1 CreditGLID FROM #LastRptsInfoTable order by ARInvoiceID asc) " +

                            "UPDATE #AllRecordsTable SET budget = @budget, LastRptsClosingBal = @LastRptsClosingBal, ExistedBefore = 1, LastRptsStartValPeriod = @LastRptsStartValPeriod, " +
                            "LastRptsEndValPeriod = @LastRptsEndValPeriod WHERE ARInvoiceID = @invoice and currCreditGLID = @LastRptsCreditGLID " +

                            "INSERT INTO #AllRecordsTable (clientID, ccNum, clientCompany, clientFname, clientLname, budget, InvAmount, ExistedBefore, LastRptsClosingBal, LastRptsStartValPeriod, " +
                            "LastRptsEndValPeriod, CurrentStartValPeriod, CurrentEndValPeriod, currCreditGLID, notes, ARInvoiceID, InvoiceCreationDate, isCancelled, isCreditMemo, CreditMemoNum, CreditMemoAmt) " +
                            "SELECT distinct clientID, ccNum, clientCompany, clientFname, clientLname, budget, InvAmount, 1, @LastRptsClosingBal, @LastRptsStartValPeriod, " +
                            "@LastRptsEndValPeriod, CurrentStartValPeriod, CurrentEndValPeriod, @LastRptsCreditGLID, notes, ARInvoiceID, InvoiceCreationDate, 1, isCreditMemo, CreditMemoNum, CreditMemoAmt " +
                            "FROM #AllRecordsTable WHERE ARInvoiceID = @invoice AND currCreditGLID != @LastRptsCreditGLID " +

                            "DELETE FROM #LastRptsInfoTable WHERE ARInvoiceID =  @invoice " +
                            "set @i = @i + 1 " +
                        "end ";

                query += "set @i = 0 " +
                        "set @count = (select count(*) from DeferredBudget) " +
                        "while (@i < @count) " +
                        "begin " +
                            "set @invoice = (SELECT TOP 1 ARInvoiceID FROM DeferredBudget order by ARInvoiceID asc) " +
                            "set @budget = (SELECT TOP 1 Budget FROM DeferredBudget order by ARInvoiceID asc) " +
                            "UPDATE #AllRecordsTable SET budget = @budget WHERE ARInvoiceID = @invoice " +
                            "DELETE FROM DeferredBudget WHERE ARInvoiceID = @invoice " +
                            "set @i = @i + 1 " +
                        "end ";

                query += "if (@ReportType = 'Monthly' AND YEAR(@searchStartDate) = 2019 AND MONTH(@searchStartDate) = 1) " +
                        "begin " +
                            "DELETE FROM #AllRecordsTable WHERE ARInvoiceID = 18795 " +
                            "INSERT INTO #AllRecordsTable values ('12779','02158-1', '', 'Paul','Thompson','0.00','2071.44',1,'1014.14','16/11/2018','15/11/2022','2018-11-16','2022-11-15','5162','Annual Fee','18795','2018-11-16',0,1,'CN417','72.60') " +
                            "INSERT INTO #AllRecordsTable values ('12779','02158-1', '', 'Paul','Thompson','0.00','2071.44',1,'1014.14','16/11/2018','15/11/2022','2018-11-16','2022-11-15','5162','Annual Fee','18795','2018-11-16',0,1,'CN418','1998.84') " +
                            "INSERT INTO #AllRecordsTable values ('9484','01106-1', 'Global Community Broadcasting Network (More FM) Limited', '','','0.00','35000.00',0,'','','','2018-02-09','2019-02-08','5159','Renewal','19007','2019-01-14',0,0,'','') " +
                            "INSERT INTO #AllRecordsTable values ('9484','01106-1', 'Global Community Broadcasting Network (More FM) Limited', '','','0.00','35000.00',0,'','','','2018-02-09','2019-02-08','5159','Renewal','19009','2019-01-14',0,0,'','') " +
                            "INSERT INTO #AllRecordsTable values ('9484','01106-1', 'Global Community Broadcasting Network (More FM) Limited', '','','0.00','35000.00',0,'','','','2018-02-09','2019-02-08','5159','Renewal','19011','2019-01-14',0,0,'','') " +
                        "end ";

                query += "if (@ReportType = 'Annual' AND YEAR(@searchStartDate) = 2018) " +
                        "begin " +
                            "INSERT INTO #AllRecordsTable values ('9484','01106-1', 'Global Community Broadcasting Network (More FM) Limited', '','','0.00','35000.00',0,'','','','2015-02-09','2016-02-08','5159','Renewal','19007','2019-01-14',0,0,'','') " +
                            "INSERT INTO #AllRecordsTable values ('9484','01106-1', 'Global Community Broadcasting Network (More FM) Limited', '','','0.00','35000.00',0,'','','','2016-02-09','2017-02-08','5159','Renewal','19009','2019-01-14',0,0,'','') " +
                            "INSERT INTO #AllRecordsTable values ('9484','01106-1', 'Global Community Broadcasting Network (More FM) Limited', '','','0.00','35000.00',0,'','','','2017-02-09','2018-02-08','5159','Renewal','19011','2019-01-14',0,0,'','') " +
                        "end ";

                query += "INSERT INTO DeferredBudget(ARInvoiceID, Budget) SELECT ARInvoiceID, Budget From #AllRecordsTable " +
                        "DELETE FROM #AllRecordsTable WHERE InvoiceCreationDate >= @searchEndDate + 1 " +
                        "SELECT* FROM #AllRecordsTable order by ccNum desc ";

                query += "drop TABLE #AllRecordsTable " +
                        "drop TABLE #ModificationRecordsTable " +
                        "drop TABLE #CancellationRecordsTable " +
                        "drop TABLE #AllCreditMemosTable " +
                        "drop TABLE #CreditMemoInvoicesToBeDeletedTable " +
                        "drop TABLE #CreditGLIDInvoicesTobeDeletedTable " +
                        "drop TABLE #LastRptsInfoTable " +
                        "drop TABLE #ReportIDTable ";
                #endregion

                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ReportType", ReportType);
                    command.Parameters.AddWithValue("@searchStartDate", searchStartDate);
                    command.Parameters.AddWithValue("@searchEndDate", searchEndDate);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                record = new ReportRawData();
                                record.clientID = reader.GetInt32(0);
                                record.ccNum = reader["ccNum"].ToString();
                                record.clientCompany = reader["clientCompany"].ToString();
                                record.clientFname = reader["clientFname"].ToString();
                                record.clientLname = reader["clientLname"].ToString();
                                record.Budget = Convert.ToDecimal(reader["budget"]);
                                record.InvAmount = Convert.ToDecimal(reader["InvAmount"]);
                                record.ExistedBefore = Convert.ToInt32(reader["ExistedBefore"]);
                                record.LastRptsClosingBal = reader["LastRptsClosingBal"].ToString();
                                record.LastRptsStartValPeriod = reader["LastRptsStartValPeriod"].ToString();
                                record.LastRptsEndValPeriod = reader["LastRptsEndValPeriod"].ToString();
                                record.CurrentStartValPeriod = reader.GetDateTime(11);
                                record.CurrentEndValPeriod = reader.GetDateTime(12);
                                record.CreditGLID = reader.GetInt32(13);
                                record.notes = reader["notes"].ToString();
                                record.ARInvoiceID = reader["ARInvoiceID"].ToString();
                                record.InvoiceCreationDate = reader.GetDateTime(16);
                                record.isCancelled = Convert.ToInt32(reader["isCancelled"]);
                                record.isCreditMemo = Convert.ToInt32(reader["isCreditMemo"]);
                                record.CreditMemoNum = reader["CreditMemoNum"].ToString();
                                record.CreditMemoAmt = Convert.ToDecimal(reader["CreditMemoAmt"]);
                                reportInfo.Add(record);
                            }
                        }
                    }
                    return reportInfo;
                }
            }
        }

        public static void CloseInvoiceBatch(string databaseConnection = CURRENT_INTEGRATION_CONNECTION)
        {
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query =
                     "Update InvoiceBatch set Status='Closed' where Status='Open' " +
                     "Update counters set transferredInvoices = 0 where id = 1 " +
                     "Update counters set createdCustomers = 0 where id = 1;";

                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public static Maj GetMajDetail(int referenceNumber, string databaseConnection = Constants.TEST_DB_GENERIC)
        {
            Maj maj = new Maj();
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "select TOP 1 stationType, CertificateType, subStationType, Proj from tbl_site where referenceNum=@referenceNumber";
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@referenceNumber", referenceNumber);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            maj.stationType = reader["stationType"].ToString();
                            if (reader["CertificateType"].ToString() != "") maj.certificateType = Convert.ToInt32(reader["CertificateType"]);
                            if (reader["subStationType"].ToString() != "") maj.substationType = Convert.ToInt32(reader["subStationType"]);
                            maj.proj = reader["Proj"].ToString();
                        }
                    }
                }
            }
            return maj;
        }

        public static DataSet GetRenewalInvoiceValidity(int invoiceid, string databaseConnection = CURRENT_INTEGRATION_CONNECTION) // could not find stored procedure sp_getValidityRenewalInvoice
        {
            DataSet ds = new DataSet();
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "";
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@invoiceid", invoiceid);
                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                    {
                        dataAdapter.Fill(ds);
                    }
                }
            }
            return ds;
        }

        public static int GetInvoiceReference(int invoiceId, string databaseConnection = Constants.TEST_DB_GENERIC)
        {
            int refNumber = -1;
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "select Ref# from tblARInvoices where ARInvoiceID = @invoiceId";
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@invoiceId", invoiceId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            if (reader[0].ToString() != "")
                            {
                                refNumber = Convert.ToInt32(reader[0]);
                            }
                        }
                    }
                }
            }
            return refNumber;
        }

        public static void CreateInvoiceBatch(double daysExpire, int batchId, string batchType, string renstat, string databaseConnection = CURRENT_INTEGRATION_CONNECTION)
        {
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "INSERT INTO InvoiceBatch VALUES(@batchId, @date, @expiryDate, 'Open', 0, @batchType, 0, @renstat)";
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    var expiryDate = DateTime.Now.AddDays(daysExpire);
                    command.Parameters.AddWithValue("@batchId", batchId);
                    command.Parameters.AddWithValue("@expirydate", expiryDate);
                    command.Parameters.AddWithValue("@batchType", batchType);
                    command.Parameters.AddWithValue("@renstat", renstat);
                    command.Parameters.AddWithValue("@date", DateTime.Now);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static bool BatchAvail(string batchType, string databaseConnection = CURRENT_INTEGRATION_CONNECTION)
        {
            bool result = false;
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "select count(*) from InvoiceBatch where BatchType=@batchType and Status = 'Open'";
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@batchType", batchType);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            var val = Convert.ToInt32(reader[0]);

                            if (val > 0)
                            {
                                result = true;
                            }
                        }
                    }
                }
            }
            return result;
        }

        public static bool IsBatchExpired(int batchId, string databaseConnection = CURRENT_INTEGRATION_CONNECTION)
        {
            bool result = false;
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "select ExpiryDate, renstat from InvoiceBatch where BatchId=@batchId";
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@batchId", batchId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            var expiryDate = Convert.ToDateTime(reader[0]);
                            var renstat = Convert.ToString(reader[1]);

                            if (renstat == "Regulatory" || renstat == "Spectrum")
                            {
                                result = false;
                            }
                            else if (DateTime.Now > expiryDate)
                            {
                                result = true;
                            }
                            else
                            {
                                result = false;
                            }
                        }
                    }
                }
            }
            return result;
        }

        public static int GetAvailBatch(string batchType, string databaseConnection = CURRENT_INTEGRATION_CONNECTION)
        {
            int result = -1;
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "select batchId from InvoiceBatch where BatchType=@batchType and status='Open'";
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@batchType", batchType);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            result = Convert.ToInt32(reader[0]);
                        }
                    }
                }
            }
            return result;
        }

        public static void OpenNewReceiptBatch(double DaysTillExpired, int LastBatchId, string bankcode, string databaseConnection = CURRENT_INTEGRATION_CONNECTION)
        {
            using (SqlConnection connection = new SqlConnection(CURRENT_INTEGRATION_CONNECTION))
            {
                string query = "if(@bankcode='FGBJMREC') " +
                                "begin " +
                                    "INSERT INTO PaymentBatch " +
                                    "VALUES(@batchId, @CreatedDate, @ExpiryDate, 'Open', '10010-100', 0, 0) " +
                                "end " +
                                "else if (@bankcode = 'FGBUSMRC') " +
                                            "begin " +
                                                "INSERT INTO PaymentBatch " +
                                                "VALUES(@batchId, @CreatedDate, @ExpiryDate, 'Open', '10012-100', 0, 0) " +
                                "end " +
                                "else if (@bankcode = 'NCBJMREC') " +
                                            "begin " +
                                                "INSERT INTO PaymentBatch " +
                                                "VALUES(@batchId, @CreatedDate, @ExpiryDate, 'Open', '10020-100', 0, 0) " +
                                "end";

                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    var CreatedDate = DateTime.Now;
                    var ExpiryDate = DateTime.Now.AddDays(DaysTillExpired);

                    command.Parameters.AddWithValue("@batchId", LastBatchId);
                    command.Parameters.AddWithValue("@CreatedDate", CreatedDate);
                    command.Parameters.AddWithValue("@ExpiryDate", ExpiryDate);
                    command.Parameters.AddWithValue("@bankcode", bankcode);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void CloseReceiptBatch(int batchId, string databaseConnection = CURRENT_INTEGRATION_CONNECTION)
        {
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "update PaymentBatch set Status='Closed' where BatchId=@batchId";
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@batchId", batchId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateBatchAmount(string batchType, decimal amount, string databaseConnection = CURRENT_INTEGRATION_CONNECTION)
        {
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "if((select amount from InvoiceBatch where Status='Open' and BatchType=@batchType) is NULL) " +
                            "begin " +
                                "update InvoiceBatch set amount = @amount where BatchType = @batchType and Status = 'Open' " +
                            "end " +
                            "else " +
                            "begin " +
                                "update InvoiceBatch set amount = amount + @amount where Status = 'Open' and BatchType = @batchType " +
                            "end " +

                            "if ((select count(*) from invoiceTotal)= 0) " +
                            "begin " +
                                "insert into invoiceTotal values(@amount) " +
                            "end " +
                            "else " +
                            "begin " +
                                "update invoiceTotal set total = total + @amount " +
                            "end";

                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@batchType", batchType);
                    command.Parameters.AddWithValue("@amount", amount);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static string GetBankCodeId(string bankcode, string databaseConnection = CURRENT_INTEGRATION_CONNECTION)
        {
            string bankcodeid = "";
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "if(@bankcode='FGBJMREC') " +
                            "begin " +
                                "select '10010-100' " +
                            "end " +
                            "else if (@bankcode = 'FGBUSMRC') " +
                            "begin " +
                                "select '10012-100' " +
                            "end " +
                            "else if (@bankcode = 'NCBJMREC') " +
                            "begin " +
                                "select '10020-100' " +
                            "end";

                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@bankcode", bankcode);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            bankcodeid = reader[0].ToString();
                        }
                    }
                }
            }
            return bankcodeid;
        }

        public static DateTime GetDocDate(int docNumber, string databaseConnection = Constants.TEST_DB_GENERIC)
        {
            DateTime date = DateTime.Now;
            using (SqlConnection connection = new SqlConnection(Constants.TEST_DB_GENERIC))
            {
                string query = "select top 1 StartPeriod from tblArInvoiceDetail where arinvoiceid=@docNumber ";
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@docNumber", docNumber);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            date = Convert.ToDateTime(reader[0]);
                        }
                    }
                }
            }
            return date;
        }

        public static void UpdateBatchCount(string BatchType, string databaseConnection = CURRENT_INTEGRATION_CONNECTION)
        {
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "Declare @count integer " +
                         "select @count = Count from InvoiceBatch where BatchType = @BatchType_ AND Status = 'Open' " +
                         "set @count = @count + 1 " +
                         "Update InvoiceBatch set Count = @count where BatchType = @BatchType_ AND Status = 'Open'";

                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@BatchType_", BatchType);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void IncrementReferenceNumber(string BankCode, decimal amount, string databaseConnection = CURRENT_INTEGRATION_CONNECTION)
        {
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "Declare @var1 integer " +
                          "Declare @var2 decimal(19, 2) " +
                          "SELECT @var1 = CurrentRefNumber from BankCode where BankCode.BankCodeId = @BankCodeId " +
                          "set @var1 = @var1 + 1 " +
                          "select @var2 = Total from PaymentBatch where BankCodeId = @BankCodeId and Status = 'Open' " +
                          "if (@var2 is null) " +
                          "begin " +
                              "set @var2 = 0 " +
                          "end " +
                          "set @var2 = @amount + @var2 " +
                          "update PaymentBatch set Total = @var2 where BankCodeId = @BankCodeId and Status = 'Open' " +
                          "update BankCode set CurrentRefNumber = @var1 where BankCode.BankCodeId = @BankCodeId ";

                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@BankCodeId", BankCode);
                    command.Parameters.AddWithValue("@amount", amount);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static decimal GetRate(string databaseConnection = Constants.TEST_DB_GENERIC)
        {
            decimal result = 0;
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "select top 1 CurrencyExchangeRate from tbl_CurrencyExchangeRates order by SavedDate desc";
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            result = Convert.ToDecimal(reader[0].ToString());
                        }
                    }
                }
            }
            return result;
        }

        public static void UpdateBatchCountPayment(string BatchId, string databaseConnection = CURRENT_INTEGRATION_CONNECTION)
        {
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "Declare @count integer " +
                          "select @count = [Count] from PaymentBatch where BatchId = @BatchId " +
                          "set @count = @count + 1 " +
                          "update PaymentBatch set[Count] = @count where BatchId = @BatchId";

                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@BatchId", BatchId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static decimal GetUsRateByInvoice(int invoiceid, string databaseConnection = CURRENT_INTEGRATION_CONNECTION)
        {
            decimal rate = 1;
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "select usrate from InvoiceList where invoiceId = @invoiceid";
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@invoiceid", invoiceid);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            rate = Convert.ToDecimal(reader[0].ToString());
                        }
                    }
                }
            }
            return rate;
        }

        public static string GetCurrentRef(string bankCode, string databaseConnection = CURRENT_INTEGRATION_CONNECTION)
        {
            string refNumber = "";
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "select CurrentRefNumber from BankCode where BankCode=@BankCode";
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@BankCode", bankCode);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            int i = Convert.ToInt32(reader[0]);
                            refNumber = i.ToString();
                        }
                    }
                }
            }
            return refNumber;
        }

        public static string GetRecieptBatchId(string bankcode, string databaseConnection = CURRENT_INTEGRATION_CONNECTION)
        {
            string batch = "";
            switch (bankcode)
            {
                case "FGBJMREC":
                    bankcode = "10010-100";
                    break;
                case "FGBUSMRC":
                    bankcode = "10012-100";
                    break;
                case "NCBJMREC":
                    bankcode = "10020-100";
                    break;
            }

            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "select BatchId from PaymentBatch where BankCodeId = @bankcode and Status = 'Open'";
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@bankcode", bankcode);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            batch = reader[0].ToString();
                        }
                    }
                }
            }
            return batch;
        }

        public static ReceiptBatch GetReceiptBatchDetail(string bankcode, string databaseConnection = CURRENT_INTEGRATION_CONNECTION)
        {
            ReceiptBatch batch = new ReceiptBatch();
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "if (@bankcode = 'FGBJMREC') " +
                            "begin " +
                                "select BatchId, CreatedDate, ExpiryDate, Status, BankCodeId, Count, Total from PaymentBatch where Status = 'Open' and BankCodeId = '10010-100' " +
                            "end " +
                            "else if (@bankcode = 'FGBUSMRC') " +
                            "begin " +
                                "select BatchId, CreatedDate, ExpiryDate, Status, BankCodeId, Count, Total from PaymentBatch where Status = 'Open' and BankCodeId = '10012-100' " +
                            "end " +
                            "else if (@bankcode = 'NCBJMREC') " +
                            "begin " +
                                "select BatchId, CreatedDate, ExpiryDate, Status, BankCodeId, Count, Total from PaymentBatch where Status = 'Open' and BankCodeId = '10020-100' " +
                            "end";

                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@bankcode", bankcode);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            batch.batchId = Convert.ToInt32(reader["BatchId"]);
                            batch.createdDate = Convert.ToDateTime(reader["CreatedDate"]);
                            batch.expiryDate = Convert.ToDateTime(reader["ExpiryDate"]);
                            batch.status = reader["Status"].ToString();
                            batch.count = Convert.ToInt32(reader["Count"]);
                            batch.total = Convert.ToDecimal(reader["Total"]);
                        }
                        else
                        {
                            batch = null;
                        }
                    }
                }
            }
            return batch;
        }

        public static List<string> CheckInvoiceAvail(string invoiceId, string databaseConnection = CURRENT_INTEGRATION_CONNECTION)
        {
            List<string> data = new List<string>(3);
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "select * from InvoiceList where invoiceId=@invoiceId";
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@invoiceId", invoiceId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            data.Add(reader[0].ToString());
                            data.Add(reader[1].ToString());
                            data.Add(reader[2].ToString());
                        }
                        else
                        {
                            data = null;
                        }
                    }
                }
            }
            return data;
        }

        public static void StoreInvoice(int invoiceId, int batchTarget, int CreditGL, string clientName, string clientId, DateTime date, string author, decimal amount, string state, decimal usrate, decimal usamount, int isvoid, int isCreditMemo, int creditMemoNumber, string databaseConnection = CURRENT_INTEGRATION_CONNECTION)
        {
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "declare @var1 integer " +
                        "declare @var2 integer " +
                        "declare @tBatch integer " +
                        "declare @originalAmt decimal(19, 2) " +

                        "set @var1 = (select[Count] FROM InvoiceBatch where BatchId = @targetBatch) " +
                        "set @var2 = (select COUNT(@invoiceId) from InvoiceList) " +
                        "set @var2 = @var2 + 1; " +

                        "if (@state = 'updated') " +
                            "begin " +
                                "set @originalAmt = (select top 1 amount from InvoiceList where invoiceId = @invoiceId order by LastModified desc) " +
                                "insert into InvoiceList values(@invoiceId, 'T', @targetBatch, @var1, @CreditGl, @var2, @clientName, @clientId, @dateCreated, @author, @amount, GETDATE(), @state, @usrate, @usamount, @isvoid, @isCreditMemo, @credMemoNum); " +
                                    "set @tBatch = (select top 1 TargetBatch from invoiceList where invoiceId = @invoiceId and state = 'no modification') " +
                                "update InvoiceBatch set amount = amount - @originalAmt + @amount where BatchId = @tBatch " +
                            "end " +
                        "else " +
                            "begin " +
                                "insert into InvoiceList values(@invoiceId, 'NT', @targetBatch, @var1, @CreditGl, @var2, @clientName, @clientId, @dateCreated, @author, @amount, GETDATE(), @state, @usrate, @usamount, @isvoid, @isCreditMemo, @credMemoNum); " +
                                "update InvoiceBatch set[Count] = @var1 where BatchId = @targetBatch " +
                            "end";

                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@invoiceId", invoiceId);
                    command.Parameters.AddWithValue("@targetBatch", batchTarget);
                    command.Parameters.AddWithValue("@CreditGL", CreditGL);
                    command.Parameters.AddWithValue("@clientName", clientName);
                    command.Parameters.AddWithValue("@clientId", clientId);
                    command.Parameters.AddWithValue("@dateCreated", date);
                    command.Parameters.AddWithValue("@author", author);
                    command.Parameters.AddWithValue("@amount", amount);
                    command.Parameters.AddWithValue("@state", state);
                    command.Parameters.AddWithValue("@usrate", usrate);
                    command.Parameters.AddWithValue("@usamount", usamount);
                    command.Parameters.AddWithValue("@isvoid", isvoid);
                    command.Parameters.AddWithValue("@isCreditMemo", isCreditMemo);
                    command.Parameters.AddWithValue("@credMemoNum", creditMemoNumber);
                    command.ExecuteNonQuery();
                }
            }
            MiddlewareService.BroadcastEvent(new EventObjects.Invoice(invoiceId.ToString(), clientName, clientId, batchTarget.ToString(), amount.ToString(), DateTime.Now, author, state));
        }

        public static void StorePayment(string clientId, string clientName, DateTime createdDate, string invoiceId, decimal amount, decimal usamount, string prepstat, int referenceNumber, int destinationBank, string isPayByCredit, decimal prepaymentUsRate, string databaseConnection = CURRENT_INTEGRATION_CONNECTION)
        {
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "DECLARE @var1 integer " +
                       "select top 1 @var1 = [sequence] from PaymentList order by sequence desc " +
                       "if (@var1 is null) " +
                       "begin " +
                           "set @var1 = 1 " +
                       "end " +
                       "else " +
                       "begin " +
                           "set @var1 = @var1 + 1 " +
                       "end " +
                       "if (@prepstat = 'Yes') " +
                       "begin " +
                           "insert into PaymentList values(@clientId, @clientName, @createdDate, @invoiceId, @amount, 'NO', @var1, @usamount, @prepstat, @referenceNumber, @amount, @destinationBank, @isPayByCredit, @prepaymentUsRate) " +
                       "end " +
                       "else " +
                       "begin " +
                            "insert into PaymentList values(@clientId, @clientName, @createdDate, @invoiceId, @amount, 'NO', @var1, @usamount, @prepstat, @referenceNumber, 0, @destinationBank, @isPayByCredit, @prepaymentUsRate) " +
                       "end " +
                       "if (@prepstat = 'Yes' and @clientId like '%-T' and @destinationBank = 5147) " +
                       "begin " +
                           "update PaymentList set prepaymentRemainder = @usamount where referenceNumber = @referenceNumber " +
                       "end " +
                       "if ((Select transferredReceipts from counters) is null) " +
                       "begin " +
                           "update counters set transferredReceipts = 1 where id = 1 " +
                       "end " +
                       "else " +
                       "begin " +
                           "update counters set transferredReceipts = transferredReceipts + 1 where id = 1 " +
                           "select* from counters " +
                       "end";

                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@clientId", clientId);
                    command.Parameters.AddWithValue("@clientName", clientName);
                    command.Parameters.AddWithValue("@createdDate", createdDate);
                    command.Parameters.AddWithValue("@invoiceId", invoiceId);
                    command.Parameters.AddWithValue("@amount", amount);
                    command.Parameters.AddWithValue("@usamount", usamount);
                    command.Parameters.AddWithValue("@prepstat", prepstat);
                    command.Parameters.AddWithValue("@referenceNumber", referenceNumber);
                    command.Parameters.AddWithValue("@destinationBank", destinationBank);
                    command.Parameters.AddWithValue("@isPayByCredit", isPayByCredit);
                    command.Parameters.AddWithValue("@prepaymentUsRate", prepaymentUsRate);
                    command.ExecuteNonQuery();
                    MiddlewareService.BroadcastEvent(new EventObjects.Receipt(createdDate,clientName, clientId, invoiceId, amount, usamount, referenceNumber, prepstat, destinationBank, isPayByCredit));
                }
            }
        }

        public static string GetAccountNumber(int GLID, string databaseConnection = Constants.TEST_DB_GENERIC)
        {
            string accountNumber = "";
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "select GLAccountNumber from tblGLAccounts where GLAccountID=@GLID";
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@GLID", GLID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            accountNumber = reader[0].ToString();
                        }
                    }
                }
            }
            return accountNumber;
        }

        public static List<string> GetInvoiceDetails(int invoiceId, string databaseConnection = CURRENT_INTEGRATION_CONNECTION)
        {
            List<string> data = new List<string>(3);
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "select TargetBatch, EntryNumber, CreditGL, Amount from InvoiceList where invoiceId=@invoiceId ";
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@invoiceId", invoiceId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            data.Add(reader[0].ToString());
                            data.Add(reader[1].ToString());
                            data.Add(reader[2].ToString());
                            data.Add(reader[3].ToString());
                        }
                        else
                        {
                            data = null;
                        }
                    }
                }
            }
            return data;
        }

        public static void MarkTransferred(int invoiceId, string databaseConnection = CURRENT_INTEGRATION_CONNECTION)
        {
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "update InvoiceList set status='T', LastModified=GETDATE() where invoiceId=@invoiceId " +
                            "if((Select transferredInvoices from counters) is null) " +
                            "begin " +
                                "update counters set transferredInvoices = 1 where id = 1 " +
                                "select '2' " +
                            "end " +
                            "else " +
                            "begin " +
                                "update counters set transferredInvoices = transferredInvoices + 1 where id = 1 " +
                                "select* from counters " +
                            "end";

                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@invoiceId", invoiceId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static int GetCreditGl(string invoiceiD, string databaseConnection = Constants.TEST_DB_GENERIC)
        {
            int i = 0;
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "SELECT top 1 CreditGLID FROM tblARInvoiceDetail where ARInvoiceID = @invoiceId ";
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@invoiceId", invoiceiD);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            i = Convert.ToInt32(reader[0].ToString());
                        }
                    }
                }
            }
            return i;
        }

        public static int GetCreditGlID(string GLTransactionID, string databaseConnection = Constants.TEST_DB_GENERIC)
        {
            int i = 0;
            using (SqlConnection connection = new SqlConnection(Constants.TEST_DB_GENERIC))
            {
                string query = "SELECT GLID FROM tblARPayments WHERE GLTransactionID = @GLTransactionID ";
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@GLTransactionID", GLTransactionID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            i = Convert.ToInt32(reader[0].ToString());
                        }
                    }
                }
            }
            return i;
        }

        public static string IsAnnualFee(int invoiceid, string databaseConnection = Constants.TEST_DB_GENERIC)
        {
            string notes = " ";
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "select notes from tblARInvoices where ARInvoiceID = @invoiceId";
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@invoiceid", invoiceid);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            notes = reader[0].ToString();
                        }
                    }
                }
            }
            return notes;
        }

        public static void UpdateCreditGl(int invoiceId, int newCreditGl, string databaseConnection = CURRENT_INTEGRATION_CONNECTION)
        {
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "update InvoiceList set CreditGl=@newCreditGl where invoiceId=@invoiceId";
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@invoiceId", invoiceId);
                    command.Parameters.AddWithValue("@newCreditGl", newCreditGl);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void ModifyInvoiceList(int invoiceId, decimal rate, string customerId, string databaseConnection = CURRENT_INTEGRATION_CONNECTION)
        {
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "update InvoiceList set usrate = @usrate where invoiceId = @invoiceid " +
                               "update InvoiceList set clientId = @customerId where invoiceId = @invoiceid ";

                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@invoiceId", invoiceId);
                    command.Parameters.AddWithValue("@usrate", rate);
                    command.Parameters.AddWithValue("@customerId", customerId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateEntryNumber(int invoiceId, string databaseConnection = CURRENT_INTEGRATION_CONNECTION)
        {
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "declare @var1 int " +
                        "declare @var2 int " +
                        "select @var2 = TargetBatch from InvoiceList where invoiceId = @invoiceId " +
                        "select @var1 =[Count] from InvoiceBatch where BatchId = @var2 " +
                        "update InvoiceList set EntryNumber = @var1 where invoiceId = @invoiceId";

                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@invoiceId", invoiceId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static List<string> GetPaymentInfo(int gl_id, string databaseConnection = Constants.TEST_DB_GENERIC)
        {
            List<string> data = new List<string>(4);
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "SELECT Debit, GLID, InvoiceID, Date1 from tblARPayments where GLTransactionID=@id ";
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", gl_id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            var debit = reader[0].ToString();
                            var glid = reader[1].ToString();
                            var invoiceId = reader[2].ToString();
                            var paymentDate = reader[3].ToString();

                            data.Add(debit);
                            data.Add(glid);
                            data.Add(invoiceId);
                            data.Add(paymentDate);
                        }
                    }
                }
            }
            return data;
        }

        public static List<string> GetClientInfoInv(string id, string databaseConnection = Constants.TEST_DB_GENERIC)
        {
            List<string> data = new List<string>(4);
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "SELECT clientCompany, ccNum, clientFname, clientLname from client where clientId=@clientId ";
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@clientId", id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            var companyName = reader[0].ToString();
                            var ccNum = reader[1].ToString();
                            var clientFname = reader[2].ToString();
                            var clientLname = reader[3].ToString();

                            data.Add(companyName);
                            data.Add(ccNum);
                            data.Add(clientFname);
                            data.Add(clientLname);
                        }
                    }
                }
            }
            return data;
        }

        public static List<string> GetFeeInfo(int invoiceId, string databaseConnection = Constants.TEST_DB_GENERIC)
        {
            List<string> data = new List<string>(2);
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "Select FeeType, notes from tblARInvoices where ARInvoiceID=@id_inv";
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_inv", invoiceId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            var ftype = reader[0].ToString();
                            var notes = reader[1].ToString();

                            data.Add(ftype);
                            data.Add(notes);
                        }
                    }
                }
            }
            return data;
        }

        public static void UpdateCustomerCount(string databaseConnection = CURRENT_INTEGRATION_CONNECTION)
        {
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "declare @var1 integer " +
                          "select @var1 = Count from tblCustomerCreatedCount " +
                          "if (@var1 is null) " +
                          "begin " +
                              "select 'null' " +
                              "set @var1 = 1 " +
                              "insert into tblCustomerCreatedCount " +
                              "values(@var1, GETDATE()) " +
                          "end " +
                          "else " +
                          "begin " +
                          "set @var1 = @var1 + 1 " +
                               "update tblCustomerCreatedCount set Count = @var1, LastUpdate = GETDATE() " +
                          "end";

                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void StoreCustomer(string clientId, string clientName, string databaseConnection = CURRENT_INTEGRATION_CONNECTION)
        {
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "insert into CustomerCreatedDetail " +
                          "values(@clientId, @clientName, GETDATE()) " +
                          "if((Select createdCustomers from counters) is null) " +
                          "begin " +
                              "update counters set createdCustomers = 1 where id = 1 " +
                          "end " +
                          "else " +
                          "begin " +
                              "update counters set createdCustomers = createdCustomers + 1 where id = 1 " +
                              "select* from counters " +
                         "end";

                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@clientId", clientId);
                    command.Parameters.AddWithValue("@clientName", clientName);
                    command.ExecuteNonQuery();
                }
            }
            MiddlewareService.BroadcastEvent(new EventObjects.Customer(clientName, clientId));
        }

        public static void UpdateReceiptNumber(int transactionId, string referenceNumber, string databaseConnection = Constants.TEST_DB_GENERIC)
        {
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "update tblARPayments set ReceiptNumber=@reference where ReceiptNumber=@receiptNum";
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@receiptNum", transactionId.ToString());
                    command.Parameters.AddWithValue("@reference", referenceNumber);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static DateTime GetValidity(int invoiceId, string databaseConnection = Constants.TEST_DB_GENERIC)
        {
            var datetime = DateTime.Now;
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "SELECT cus.clientID, cus.ccNum, cus.clientCompany, cus.clientFname, cus.clientLname, Amount, val.ValidFrom, val.ValidTo, " +
                           "GL.CreditGLID, Gl.Description, inv.ARInvoiceID " +
                           "FROM tblARInvoices inv, tbl_LicenseValidityHistory val, client cus, " +
                           "tblARInvoiceDetail GL where inv.LicensevalidityHistoryID = val.LicenseValidityHistoryID " +
                           "AND val.ClientID = cus.clientID AND (cus.ccNum is not null) AND val.ValidTo >=GETDATE() AND inv.ARInvoiceID = GL.ARInvoiceID " +
                           "AND inv.ARInvoiceID =@invoiceId Group by cus.clientID, cus.ccNum, cus.clientCompany, cus.clientFname, cus.clientLname, " +
                           "inv.notes, inv.FeeType, inv.ARInvoiceID, inv.Amount, val.ValidFrom, val.ValidTo, GL.CreditGLID, GL.Description, inv.ARInvoiceID  order by inv.ARInvoiceID desc";

                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@invoiceId", invoiceId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            datetime = Convert.ToDateTime(reader[6]);
                        }
                    }
                }
            }
            return datetime;
        }

        public static DateTime GetValidityEnd(int invoiceId, string databaseConnection = Constants.TEST_DB_GENERIC)
        {
            var datetime = DateTime.Now;
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "SELECT cus.clientID, cus.ccNum, cus.clientCompany, cus.clientFname, cus.clientLname, Amount, val.ValidFrom, val.ValidTo, " +
                           "GL.CreditGLID, Gl.Description, inv.ARInvoiceID " +
                           "FROM tblARInvoices inv, tbl_LicenseValidityHistory val, client cus, " +
                           "tblARInvoiceDetail GL where inv.LicensevalidityHistoryID = val.LicenseValidityHistoryID " +
                           "AND val.ClientID = cus.clientID AND (cus.ccNum is not null) AND val.ValidTo >=GETDATE() AND inv.ARInvoiceID = GL.ARInvoiceID " +
                           "AND inv.ARInvoiceID =@invoiceId Group by cus.clientID, cus.ccNum, cus.clientCompany, cus.clientFname, cus.clientLname, " +
                           "inv.notes, inv.FeeType, inv.ARInvoiceID, inv.Amount, val.ValidFrom, val.ValidTo, GL.CreditGLID, GL.Description, inv.ARInvoiceID  order by inv.ARInvoiceID desc";

                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@invoiceId", invoiceId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            datetime = Convert.ToDateTime(reader[7]);
                        }
                    }
                }
            }
            return datetime;
        }

        public static void ResetInvoiceTotal(string databaseConnection = CURRENT_INTEGRATION_CONNECTION)
        {
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "update InvoiceTotal set total=0";
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public static string GetFreqUsage(int invoiceId, string databaseConnection = Constants.TEST_DB_GENERIC)
        {
            string result = "";
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "select FreqUsage FROM tblARInvoiceDetail where ARInvoiceID=@invoiceId";
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@invoiceId", invoiceId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            result = reader[0].ToString();
                        }
                    }
                }
            }
            return result;
        }

        public static int GetCreditMemoNumber(string databaseConnection = CURRENT_INTEGRATION_CONNECTION)
        {
            int num = -1;
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "select creditMemoSequence from counters " +
                                "update counters set creditMemoSequence = creditMemoSequence + 1";

                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            num = Convert.ToInt32(reader[0]);
                        }
                    }
                }
            }
            return num;
        }

        public static void UpdateAsmsCreditMemoNumber(int docId, int newCredNum, string databaseConnection = Constants.TEST_DB_GENERIC)
        {
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "update tblGLDocuments set DocumentDisplayNumber = @newCredNum where DocumentID=@documentId";
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@documentId", docId);
                    command.Parameters.AddWithValue("@newCredNum", newCredNum);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static InvoiceInfo GetInvoiceInfo(int invoiceId, string databaseConnection = Constants.TEST_DB_GENERIC)
        {
            InvoiceInfo inv = new InvoiceInfo();
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "Declare @Glid integer " +
                        "Declare @FreqUsage varchar(50) " +
                        "set @Glid = (select top 1 CreditGLID from tblARInvoiceDetail where ARInvoiceID = @invoiceId) " +
                        "set @FreqUsage = (select top 1 FreqUsage from tblARInvoiceDetail where ARInvoiceID = @invoiceId) " +
                        "select CustomerId, FeeType, notes, Amount, isvoided, @Glid as Glid, @FreqUsage as FreqUsage, Author, ARBalance from tblARInvoices where ARInvoiceID = @invoiceId";

                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@invoiceId", invoiceId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            inv.customerId = Convert.ToInt32(reader["CustomerId"]);
                            inv.feeType = reader["FeeType"].ToString();
                            inv.notes = reader["notes"].ToString();
                            inv.amount = Convert.ToDecimal(reader["Amount"]);
                            inv.arBalance = Convert.ToDecimal(reader["ARBalance"]);
                            inv.isVoided = Convert.ToInt32(reader["isvoided"]);
                            inv.glid = Convert.ToInt32(reader["Glid"]);
                            inv.freqUsage = reader["FreqUsage"].ToString();
                            inv.author = reader["Author"].ToString();
                        }
                    }
                }
            }
            return inv;
        }

        public static PaymentInfo GetReceiptInfo(int originalDocNum, string databaseConnection = Constants.TEST_DB_GENERIC)
        {
            PaymentInfo rct = new PaymentInfo();
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "select ReceiptNumber, GLTransactionID, CustomerID, Debit, InvoiceID, Date1, GLID from tblARPayments where GLTransactionID=@originalDocNum";
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@originalDocNum", originalDocNum);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            rct.ReceiptNumber = Convert.ToInt32(reader["ReceiptNumber"]);
                            rct.GLTransactionID = Convert.ToInt32(reader["GLTransactionID"]);
                            rct.CustomerID = Convert.ToInt32(reader["CustomerID"]);
                            rct.Debit = Convert.ToDecimal(reader["Debit"]);
                            rct.InvoiceID = Convert.ToInt32(reader["InvoiceId"]);
                            rct.Date1 = Convert.ToDateTime(reader["Date1"]);
                            rct.GLID = Convert.ToInt32(reader["GLID"]);
                        }
                    }
                }
            }
            return rct;
        }

        public static CreditNoteInfo GetCreditNoteInfo(int creditMemoNum, int documentId, string databaseConnection = Constants.TEST_DB_GENERIC)
        {
            CreditNoteInfo creditNote = new CreditNoteInfo();
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "Declare @relatedInvoice integer " +
                           "Declare @cglid integer " +
                           "Declare @cmemoAmt decimal " +
                           "Declare @memoDesc varchar(max) " +
                           "select @relatedInvoice = relatedInvoice from ARCreditMemo where CreditMemoId = @creditMemoNum " +
                           "select @cglid = CreditGLID from TblARInvoiceDetail where ARInvoiceID = @relatedInvoice " +
                           "select @memoDesc = Memo from tblGLDocumentLines where DocumentID = @documentId " +

                           "set @cmemoAmt = (select BalanceCompanyCurrency from tblGLDocuments where OriginalDocumentID = @creditMemoNum and DocumentType = 5) " +
                           "select ARInvoiceID, @cglid as CreditGl, @cmemoAmt as Amount, CustomerID, FeeType, notes, canceledBy, @memoDesc as Remarks from TblARInvoices where ARInvoiceID = @relatedInvoice";

                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@creditMemoNum", creditMemoNum);
                    command.Parameters.AddWithValue("@documentId", documentId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            creditNote.ARInvoiceID = Convert.ToInt32(reader["ARInvoiceID"]);
                            creditNote.CreditGL = Convert.ToInt32(reader["CreditGl"]);
                            creditNote.amount = Convert.ToDecimal(reader["Amount"]);
                            creditNote.CustomerID = Convert.ToInt32(reader["CustomerID"]);
                            creditNote.FeeType = reader["FeeType"].ToString();
                            creditNote.notes = reader["notes"].ToString();
                            creditNote.remarks = reader["Remarks"].ToString();
                        }
                    }
                }
            }
            return creditNote;
        }

        public static string GetClientIdZRecord(bool stripExtention, string databaseConnection = CURRENT_INTEGRATION_CONNECTION)
        {
            string result = "";
            string temp = "";
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "select * from InvoiceList where invoiceId=0";
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            result = reader["clientId"].ToString();

                            if (stripExtention)
                            {
                                for (int i = 0; i < result.Length; i++)
                                {
                                    if (result[i] != '-')
                                    {
                                        temp += result[i];
                                    }
                                    else
                                    {
                                        i = result.Length;
                                    }
                                }
                                result = temp;
                            }
                        }
                    }
                }
            }
            return result;
        }

        public static PrepaymentData CheckPrepaymentAvail(string customerId, string databaseConnection = CURRENT_INTEGRATION_CONNECTION)
        {
            PrepaymentData data = new PrepaymentData();
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "declare @CustomerPrepaymentTable TABLE " +
                        "( " +
                            "amount decimal(19, 2), " +
                            "referenceNumber int, " +
                            "prepaymentRemainder decimal(19, 2), " +
                            "destinationBank int, " +
                            "sequence int, " +
                            "TotalPrepaymentRemainder decimal(19, 2) " +
                        ") " +
                        "declare @var decimal(19, 2) " +
                        "INSERT INTO @CustomerPrepaymentTable(amount, referenceNumber, prepaymentRemainder, destinationBank, sequence) select top 1  amount, referenceNumber, prepaymentRemainder, destinationBank, sequence from PaymentList where prepstat = 'Yes' and clientId = @customerId and prepaymentRemainder > 0 order by createdDate asc " +
                        "SELECT @var = sum(prepaymentRemainder) from PaymentList where prepstat = 'Yes' and clientId = @customerId and prepaymentRemainder > 0 " +
                        "UPDATE @CustomerPrepaymentTable SET TotalPrepaymentRemainder = @var " +
                        "SELECT* FROM @CustomerPrepaymentTable";

                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@customerId", customerId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            data.dataAvail = true;
                            data.originalAmount = Convert.ToDecimal(reader["amount"]);
                            data.remainder = Convert.ToDecimal(reader["prepaymentRemainder"]);
                            data.totalPrepaymentRemainder = Convert.ToDecimal(reader["TotalPrepaymentRemainder"]);
                            data.referenceNumber = reader["referenceNumber"].ToString();
                            data.sequenceNumber = Convert.ToInt32(reader["sequence"]);
                            data.destinationBank = Convert.ToInt32(reader["destinationBank"]);
                        }
                        else
                        {
                            data.dataAvail = false;
                        }
                    }
                }
            }
            return data;
        }

        public static void AdjustPrepaymentRemainder(decimal amount, int sequenceNumber, string databaseConnection = CURRENT_INTEGRATION_CONNECTION)
        {
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "update PaymentList set prepaymentRemainder=prepaymentRemainder-@amount where sequence=@sequenceNumber";
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@amount", amount);
                    command.Parameters.AddWithValue("@sequenceNumber", sequenceNumber);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static string GenerateReportId(string ReportType, string databaseConnection = CURRENT_INTEGRATION_CONNECTION)
        {
            string result = "";
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "declare @count as integer " +
                       "declare @id as integer " +
                       "declare @sql as nvarchar (max) " +

                       "create table #DIRrecordsCount (DIRrecordsCount int) " +
                       "select @sql = 'select count(*) from ' + @ReportType + 'DIR_ReportMain;' " +

                       "insert into #DIRrecordsCount (DIRrecordsCount) exec (@sql) " +

                       "set @count = (select DIRrecordsCount from #DIRrecordsCount) " +
                       "set @id = 100000 + @count + 1 " +
                       "select @sql = 'insert into ' + @ReportType + 'DIR_ReportMain (report_id, report_date) " +
                                      " values('+cast(@id as varchar (50))+', GETDATE()); ' " +

                       "exec(@sql) " +

                       "drop Table #DIRrecordsCount " +
                       "select @id";

                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ReportType", ReportType);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            result = reader[0].ToString();
                        }
                    }
                }
            }
            return result;
        }

        public static void DataRouter(string ReportType, DataWrapper data, string recordID, int destination, string databaseConnection = CURRENT_INTEGRATION_CONNECTION)
        {
            for (int i = 0; i < data.records.Count; i++)
            {
                using (SqlConnection connection = new SqlConnection(databaseConnection))
                {
                    string query = "declare @category as varchar (50) " +
                            "declare @sql as Nvarchar (max) " +
                            "select @category = Description from DIRcategories where Category = @destination " +

                            "set @sql = 'insert into ' + @ReportType + 'DIR_' + @category + ' (report_id, licenseNumber, clientCompany, " +
                                        "invoiceID, budget, invoiceTotal, thisPeriodsInvoice, balanceBFoward, fromRevenue, toRevenue, closingBalance, " +
                                        "totalMonths, monthsUtilized, monthsRemaining, validityStart, validityEnd) " +
                                        "values(@reportId, @licenseNumber, @clientCompany, @invoiceID, @budget, @invoiceTotal, @thisPeriodsInv, @balBFwd, " +
                                        "@fromRev, @toRev, @closingBal, @totalMonths, @monthUtil, @monthRemain, @valPStart, @valPEnd)'; " +

                            "exec sp_executesql @sql, " +
                                 "N'@reportId varchar(50),@licenseNumber varchar(50),@clientCompany nvarchar(100),@invoiceID varchar(50), " +

                                 "@budget varchar(50),@invoiceTotal varchar(50),@thisPeriodsInv varchar(3),@balBFwd varchar(50),@fromRev varchar(50),@toRev varchar(50), " +
                                 "@closingBal varchar(50),@totalMonths integer, @monthUtil integer,@monthRemain integer, @valPStart varchar(50),@valPEnd varchar(50)', " +

                                 "@reportId,@licenseNumber,@clientCompany,@invoiceID,@budget,@invoiceTotal,@thisPeriodsInv,@balBFwd,@fromRev,@toRev, " +
                                 "@closingBal,@totalMonths,@monthUtil,@monthRemain,@valPStart,@valPEnd";

                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ReportType", ReportType);
                        command.Parameters.AddWithValue("@reportId", recordID);
                        command.Parameters.AddWithValue("@licenseNumber", data.records[i].licenseNumber);
                        command.Parameters.AddWithValue("@clientCompany", data.records[i].clientCompany);
                        command.Parameters.AddWithValue("@invoiceID", data.records[i].invoiceID);
                        command.Parameters.AddWithValue("@budget", data.records[i].budget);
                        command.Parameters.AddWithValue("@invoiceTotal", data.records[i].invoiceTotal);
                        command.Parameters.AddWithValue("@thisPeriodsInv", data.records[i].thisPeriodsInv);
                        command.Parameters.AddWithValue("@balBFwd", data.records[i].balBFwd);
                        command.Parameters.AddWithValue("@fromRev", data.records[i].fromRev);
                        command.Parameters.AddWithValue("@toRev", data.records[i].toRev);
                        command.Parameters.AddWithValue("@closingBal", data.records[i].closingBal);
                        command.Parameters.AddWithValue("@totalMonths", data.records[i].totalMonths);
                        command.Parameters.AddWithValue("@monthUtil", data.records[i].monthUtil);
                        command.Parameters.AddWithValue("@monthRemain", data.records[i].monthRemain);
                        command.Parameters.AddWithValue("@valPStart", data.records[i].valPStart);
                        command.Parameters.AddWithValue("@valPEnd", data.records[i].valPEnd);
                        command.Parameters.AddWithValue("@destination", destination);
                        command.ExecuteNonQuery();
                    }
                }
            }
            InsertSubtotals(ReportType, recordID, data, destination);
        }

        public static string SaveReport(string ReportType, List<DataWrapper> categories, Totals total)
        {
            string id = GenerateReportId(ReportType);

            for (int i = 0; i < categories.Count; i++)
            {
                DataRouter(ReportType, categories[i], id, i);
            }

            InsertTotals(ReportType, id, total);
            return id;
        }

        public static void InsertSubtotals(string ReportType, string reportID, DataWrapper data, int destination, string databaseConnection = CURRENT_INTEGRATION_CONNECTION)
        {
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "declare @sql as nvarchar (max) " +
                        "set @sql = 'insert into ' + @ReportType + 'DIR_SubTotals (record_id, category, invoiceTotal, balanceBFwd, toRev, closingBal, fromRev, budget) " +
                                    "values(@record_id, @category, @invoiceTotal, @balanceBFwd, @toRev, @closingBal, @fromRev, @budget)'; " +

                        "exec sp_executesql @sql, " +
                            "N'@record_id varchar(50),@category integer,@invoiceTotal varchar(50),@balanceBFwd varchar(50),@toRev varchar(50),@closingBal varchar(50), " +
                            "@fromRev varchar(50),@budget varchar(50)', " +
                            "@record_id,@category,@invoiceTotal,@balanceBFwd,@toRev,@closingBal,@fromRev,@budget";

                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ReportType", ReportType);
                    command.Parameters.AddWithValue("@record_id", reportID);
                    command.Parameters.AddWithValue("@category", destination);
                    command.Parameters.AddWithValue("@invoiceTotal", data.subT_invoiceTotal);
                    command.Parameters.AddWithValue("@balanceBFwd", data.subT_balBFwd);
                    command.Parameters.AddWithValue("@toRev", data.subT_toRev);
                    command.Parameters.AddWithValue("@closingBal", data.subT_closingBal);
                    command.Parameters.AddWithValue("@fromRev", data.subT_fromRev);
                    command.Parameters.AddWithValue("@budget", data.subT_budget);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void InsertTotals(string ReportType, string reportID, Totals total, string databaseConnection = CURRENT_INTEGRATION_CONNECTION)
        {
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "declare @sql as nvarchar (max) " +
                        "set @sql = 'insert into ' + @ReportType + 'DIR_Totals (record_id, invoiceTotal, balanceBFwd, toRev, closingBal, fromRev, budget) " +
                                    "values(@recordID, @invoiceTotal, @balanceBFwd, @toRev, @closingBal, @fromRev, @budget)'; " +

                        "exec sp_executesql @sql, " +
                            "N'@recordID varchar(50),@invoiceTotal varchar(50),@balanceBFwd varchar(50),@toRev varchar(50),@closingBal varchar(50), " +

                            "@fromRev varchar(50),@budget varchar(50)', " +
                            "@recordID,@invoiceTotal,@balanceBFwd,@toRev,@closingBal,@fromRev,@budget";

                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ReportType", ReportType);
                    command.Parameters.AddWithValue("@recordID", reportID);
                    command.Parameters.AddWithValue("@invoiceTotal", total.tot_invoiceTotal);
                    command.Parameters.AddWithValue("@balanceBFwd", total.tot_balBFwd);
                    command.Parameters.AddWithValue("@toRev", total.tot_toRev);
                    command.Parameters.AddWithValue("@closingBal", total.tot_closingBal);
                    command.Parameters.AddWithValue("@fromRev", total.tot_fromRev);
                    command.Parameters.AddWithValue("@budget", total.tot_budget);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void SetNextGenDate(string ReportType, DateTime date, string databaseConnection = CURRENT_INTEGRATION_CONNECTION)
        {
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "declare @month as integer " +
                        "declare @year as integer " +
                        "declare @sql as nvarchar(max) " +
                        "declare @nextReportDate as datetime " +

                        "set @month = MONTH(@date) " +
                        "set @year = YEAR(@date) " +

                        "create table #NextReportsDate (NextReportsDate Datetime) " +
                        "set @sql = 'select date from ' + @ReportType + 'DIR_NextGenDate where MONTH(date) = ' " +
                                " + cast(@month as varchar (10)) + ' and YEAR(date) = ' + cast(@year as varchar(10)) + ';' " +

                        "insert into #NextReportsDate (NextReportsDate) exec (@sql) " +
                        "set @nextReportDate = (select NextReportsDate from #NextReportsDate) " +
                        "drop Table #NextReportsDate " +

                        "if (@nextReportDate is null) " +
                        "begin " +
                            "set @sql = 'insert into ' + @ReportType + 'DIR_NextGenDate values(@date)'; " +
                            "exec sp_executesql @sql, " +
                            "N'@date datetime', " +
                            "@date " +
                        "end";

                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ReportType", ReportType);
                    command.Parameters.AddWithValue("@date", date);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static DateTime GetNextGenDate(string ReportType, string databaseConnection = CURRENT_INTEGRATION_CONNECTION)
        {
            DateTime date = DateTime.Now;
            using (SqlConnection connection = new SqlConnection(databaseConnection))
            {
                string query = "declare @sql as nvarchar (max) " +
                        "declare @lastReportsDate as datetime " +
                        "declare @month as integer " +
                        "declare @year as integer " +

                        "create table #LastReportsDate (LastReportsDate Datetime) " +
                        "select @sql = 'select top 1 report_date from ' + @ReportType + 'DIR_ReportMain order by report_date desc;' " +

                        "insert into #LastReportsDate (LastReportsDate) exec (@sql) " +
                        "set @lastReportsDate = (select LastReportsDate from #LastReportsDate) " +
                        "drop Table #LastReportsDate " +

                        "if (@ReportType = 'Monthly') " +
                            "begin " +
                                "set @month = MONTH(@lastReportsDate) + 1 " +
                                "set @year = YEAR(@lastReportsDate) " +

                                "if (@month = 13) " +
                                "begin " +
                                    "set @month = 1 " +
                                    "set @year = @year + 1 " +
                                "end " +
                            "end " +

                            "if (@ReportType = 'Annual') " +
                            "begin " +
                                "set @month = MONTH(@lastReportsDate) " +
                                "set @year = YEAR(@lastReportsDate) + 1 " +
                            "end " +

                            "select @sql = 'select date from ' + @ReportType + 'DIR_NextGenDate where MONTH(date) = ' + " +
                                        "cast(@month as varchar (10)) + ' and YEAR(date) = ' + cast(@year as varchar(10)) + ';' " +

                            "exec(@sql)";

                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ReportType", ReportType);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            date = Convert.ToDateTime(reader["date"]);
                        }
                    }
                }
            }
            return date;
        }
    }
}
