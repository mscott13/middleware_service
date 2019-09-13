using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using middleware_service.Database_Classes;

namespace middleware_service.Database_Operations
{
    public static class Integration
    {
        private const string CINTEGRATION = "cIntegration";
        private const string CGENERIC = "cGeneric";
        private static SqlConnection cGeneric, cIntegration;

        private static void OpenConnection(string target)
        {
            switch (target)
            {
                case "cIntegration":
                    cIntegration = new SqlConnection(Constants.TEST_DB_INTEGRATION);
                    cIntegration.Open();
                    break;
                case "cGeneric":
                    cGeneric = new SqlConnection(Constants.TEST_DB_GENERIC);
                    cGeneric.Open();
                    break;
            }
        }

        private static void CloseConnection(string target)
        {
            switch (target)
            {
                case "cIntegration":
                    if (cIntegration != null)
                    {
                        cIntegration.Close();
                    }
                    break;
                case "cGeneric":
                    if (cGeneric != null)
                    {
                        cGeneric.Close();
                    }
                    break;
            }
        }

        public static bool IsBrokerEnabled(string databaseName)
        {
            string query = "select name, database_id, IS_BROKER_ENABLED from sys.databases where name=@dbname";
            OpenConnection(CGENERIC);

            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;

            cmd.Connection = cGeneric;
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@dbname", databaseName);

            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                if (Convert.ToInt32(reader["IS_BROKER_ENABLED"]) == 1)
                {
                    CloseConnection(CGENERIC);
                    return true;
                }
                else
                {
                    CloseConnection(CGENERIC);
                    return false;
                }
            }
            else
            {
                CloseConnection(CGENERIC);
                return false;
            }
        }

        public static void SetBrokerEnabled(string databaseName)
        {
            string query = "alter database " + databaseName + " set ENABLE_BROKER WITH ROLLBACK IMMEDIATE";
            OpenConnection(CGENERIC);

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cGeneric;
            cmd.CommandText = query;

            cmd.ExecuteNonQuery();
            CloseConnection(CGENERIC);
        }

        public static List<ReportRawData> GetDIRInformation(string ReportType, DateTime searchStartDate, DateTime searchEndDate)
        {
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

            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;
            cmd.Connection = cIntegration;

            List<ReportRawData> reportInfo = new List<ReportRawData>();
            ReportRawData record = new ReportRawData();
            cmd.Connection = cIntegration;
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@ReportType", ReportType);
            cmd.Parameters.AddWithValue("@searchStartDate", searchStartDate);
            cmd.Parameters.AddWithValue("@searchEndDate", searchEndDate);

            reader = cmd.ExecuteReader();

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

                CloseConnection(CINTEGRATION);
                return reportInfo;
            }
            else
            {
                CloseConnection(CINTEGRATION);
                return reportInfo;
            }
        }

        public static void CloseInvoiceBatch()
        {
            string query =
                     "Update InvoiceBatch set Status='Closed' where Status='Open' " +
                     "Update counters set transferredInvoices = 0 where id = 1 " +
                     "Update counters set createdCustomers = 0 where id = 1;";


            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = query;
            cmd.Connection = cIntegration;

            cmd.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);
        }

        public static Maj GetMajDetail(int referenceNumber)
        {
            string query =
                "select TOP 1 stationType, CertificateType, subStationType, Proj from tbl_site where referenceNum=@referenceNumber";
            OpenConnection(CGENERIC);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;

            Maj mj = new Maj();
            cmd.Connection = cGeneric;
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@referenceNumber", referenceNumber);
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                mj.stationType = reader["stationType"].ToString();
                if (reader["CertificateType"].ToString() != "") mj.certificateType = Convert.ToInt32(reader["CertificateType"]);
                if (reader["subStationType"].ToString() != "") mj.substationType = Convert.ToInt32(reader["subStationType"]);
                mj.proj = reader["Proj"].ToString();
            }

            CloseConnection(CGENERIC);
            return mj;
        }

        public static DataSet GetRenewalInvoiceValidity(int invoiceid) // could not find stored procedure
        {
            OpenConnection(CGENERIC);

            SqlCommand cmd = new SqlCommand("EXEC sp_getValidityRenewalInvoice", cGeneric);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@invoiceid", invoiceid);

            SqlDataAdapter da = new SqlDataAdapter(cmd);

            DataSet ds = new DataSet();
            da.Fill(ds);
            da.Dispose();
            CloseConnection(CGENERIC);
            return ds;
        }

        public static int GetInvoiceReference(int invoiceId)
        {
            string query = "select Ref# from tblARInvoices where ARInvoiceID = @invoiceId";
            OpenConnection(CGENERIC);

            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            int refNumber = -1;

            cmd.Connection = cGeneric;
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@invoiceId", invoiceId);

            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                if (reader[0].ToString() != "")
                {
                    refNumber = Convert.ToInt32(reader[0]);
                }
            }

            CloseConnection(CGENERIC);
            return refNumber;
        }

        public static void CreateInvoiceBatch(double daysExpire, int batchId, string batchType, string renstat)
        {
            string query = "INSERT INTO InvoiceBatch VALUES(@batchId, @date, @expiryDate, 'Open', 0, @batchType, 0, @renstat)";
            OpenConnection(CINTEGRATION);

            SqlCommand cmd = new SqlCommand();
            var expiryDate = DateTime.Now.AddDays(daysExpire);

            cmd.CommandText = query;
            cmd.Connection = cIntegration;
            cmd.Parameters.AddWithValue("@batchId", batchId);
            cmd.Parameters.AddWithValue("@expirydate", expiryDate);
            cmd.Parameters.AddWithValue("@batchType", batchType);
            cmd.Parameters.AddWithValue("@renstat", renstat);
            cmd.Parameters.AddWithValue("@date", DateTime.Now);
            cmd.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);
        }

        public static bool BatchAvail(string batchType)
        {
            string query = "select count(*) from InvoiceBatch where BatchType=@batchType and Status = 'Open'";
            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            int result = -1;

            cmd.Connection = cIntegration;
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@batchType", batchType);


            reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                result = Convert.ToInt32(reader[0]);
            }

            CloseConnection(CINTEGRATION);

            if (result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsBatchExpired(int batchId)
        {
            string query = "select ExpiryDate, renstat from InvoiceBatch where BatchId=@batchId";
            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            DateTime expiryDate = DateTime.Now;
            string renstat = " ";

            cmd.Connection = cIntegration;
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@batchId", batchId);


            reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                expiryDate = Convert.ToDateTime(reader[0]);
                renstat = Convert.ToString(reader[1]);
            }

            CloseConnection(CINTEGRATION);

            if (renstat == "Regulatory" || renstat == "Spectrum")
            {
                return false;
            }
            else if (DateTime.Now > expiryDate)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static int GetAvailBatch(string batchType)
        {
            string query = "select batchId from InvoiceBatch where BatchType=@batchType and status='Open'";
            OpenConnection(CINTEGRATION);

            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            int result = -1;

            cmd.Connection = cIntegration;
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@batchType", batchType);

            reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                result = Convert.ToInt32(reader[0]);
            }

            CloseConnection(CINTEGRATION);
            return result;
        }

        public static void OpenNewReceiptBatch(double DaysTillExpired, int LastBatchId, string bankcode)
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

            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();

            var CreatedDate = DateTime.Now;
            var ExpiryDate = DateTime.Now.AddDays(DaysTillExpired);

            cmd.Connection = cIntegration;
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@batchId", LastBatchId);
            cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
            cmd.Parameters.AddWithValue("@ExpiryDate", ExpiryDate);
            cmd.Parameters.AddWithValue("@bankcode", bankcode);

            cmd.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);
        }

        public static void CloseReceiptBatch(int batchId)
        {
            string query = "update PaymentBatch set Status='Closed' where BatchId=@batchId";
            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = cIntegration;
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@batchId", batchId);

            cmd.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);
        }

        public static void UpdateBatchAmount(string batchType, decimal amount)
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

            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = cIntegration;
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@batchType", batchType);
            cmd.Parameters.AddWithValue("@amount", amount);

            cmd.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);
        }

        public static string GetBankCodeId(string bankcode)
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

            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            string bankcodeid = "";

            cmd.Connection = cIntegration;
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@bankcode", bankcode);

            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                bankcodeid = reader[0].ToString();
            }

            CloseConnection(CINTEGRATION);
            return bankcodeid;
        }

        public static DateTime GetDocDate(int docNumber)
        {
            string query = "select top 1 StartPeriod from tblArInvoiceDetail where arinvoiceid=@docNumber ";
            OpenConnection(CGENERIC);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            DateTime date = DateTime.Now;

            cmd.Connection = cGeneric;
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@docNumber", docNumber);

            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                date = Convert.ToDateTime(reader[0]);
            }

            CloseConnection(CGENERIC);
            return date;
        }

        public static void UpdateBatchCount(string BatchType)
        {
            string query = "Declare @count integer " +
                           "select @count = Count from InvoiceBatch where BatchType = @BatchType_ AND Status = 'Open' " +
                           "set @count = @count + 1 " +
                           "Update InvoiceBatch set Count = @count where BatchType = @BatchType_ AND Status = 'Open'";

            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = cIntegration;
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@BatchType_", BatchType);

            cmd.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);
        }

        public static void IncrementReferenceNumber(string BankCode, decimal amount)
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

            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = cIntegration;
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@BankCodeId", BankCode);
            cmd.Parameters.AddWithValue("@amount", amount);

            cmd.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);
        }

        public static decimal GetRate()
        {
            string query = "select top 1 CurrencyExchangeRate from tbl_CurrencyExchangeRates order by SavedDate desc";
            OpenConnection(CGENERIC);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;
            decimal result = 0;

            cmd.Connection = cGeneric;
            cmd.CommandText = query;
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                result = Convert.ToDecimal(reader[0].ToString());
            }

            CloseConnection(CGENERIC);
            return result;
        }

        public static void UpdateBatchCountPayment(string BatchId)
        {
            string query = "Declare @count integer " +
                          "select @count = [Count] from PaymentBatch where BatchId = @BatchId " +
                          "set @count = @count + 1 " +
                          "update PaymentBatch set[Count] = @count where BatchId = @BatchId";

            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = cIntegration;
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@BatchId", BatchId);


            cmd.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);
        }

        public static decimal GetUsRateByInvoice(int invoiceid)
        {
            string query = "select usrate from InvoiceList where invoiceId = @invoiceid";
            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            decimal rate = 1;

            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@invoiceid", invoiceid);

            cmd.Connection = cIntegration;
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                rate = Convert.ToDecimal(reader[0].ToString());
            }


            CloseConnection(CINTEGRATION);
            return rate;
        }

        public static string GetCurrentRef(string BankCode)
        {
            string query = "select CurrentRefNumber from BankCode where BankCode=@BankCode";
            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            string refNumber = "";

            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@BankCode", BankCode);
            cmd.Connection = cIntegration;
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                int i = Convert.ToInt32(reader[0]);
                refNumber = i.ToString();
            }

            CloseConnection(CINTEGRATION);
            return refNumber;
        }

        public static string GetRecieptBatch(string bankcode)
        {
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

            string query = "select BatchId from PaymentBatch where BankCodeId = @bankcode and Status = 'Open'";

            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            string batch = "";

            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@bankcode", bankcode);
            cmd.Connection = cIntegration;

            reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                batch = reader[0].ToString();
            }

            CloseConnection(CINTEGRATION);
            return batch;
        }

        public static List<string> CheckInvoiceAvail(string invoiceId)
        {
            string query = "select * from InvoiceList where invoiceId=@invoiceId";
            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand(query, cIntegration);
            SqlDataReader reader;
            cmd.Parameters.AddWithValue("@invoiceId", invoiceId);
            List<string> data = new List<string>(3);

            reader = cmd.ExecuteReader();
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

            CloseConnection(CINTEGRATION);
            return data;
        }

        public static void StoreInvoice(int invoiceId, int batchTarget, int CreditGL, string clientName, string clientId, DateTime date, string author, decimal amount, string state, decimal usrate, decimal usamount, int isvoid, int isCreditMemo, int creditMemoNumber)
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

            MiddlewareService.BroadcastEvent(new EventObjects.Invoice(invoiceId.ToString(), clientName, clientId, batchTarget.ToString(), amount.ToString(), DateTime.Now, author, state));
            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand(query, cIntegration);

            cmd.Parameters.AddWithValue("@invoiceId", invoiceId);
            cmd.Parameters.AddWithValue("@targetBatch", batchTarget);
            cmd.Parameters.AddWithValue("@CreditGL", CreditGL);
            cmd.Parameters.AddWithValue("@clientName", clientName);
            cmd.Parameters.AddWithValue("@clientId", clientId);
            cmd.Parameters.AddWithValue("@dateCreated", date);
            cmd.Parameters.AddWithValue("@author", author);
            cmd.Parameters.AddWithValue("@amount", amount);
            cmd.Parameters.AddWithValue("@state", state);
            cmd.Parameters.AddWithValue("@usrate", usrate);
            cmd.Parameters.AddWithValue("@usamount", usamount);
            cmd.Parameters.AddWithValue("@isvoid", isvoid);
            cmd.Parameters.AddWithValue("@isCreditMemo", isCreditMemo);
            cmd.Parameters.AddWithValue("@credMemoNum", creditMemoNumber);

            cmd.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);
        }

        public static void StorePayment(string clientId, string clientName, DateTime createdDate, string invoiceId, decimal amount, decimal usamount, string prepstat, int referenceNumber, int destinationBank, string isPayByCredit, decimal prepaymentUsRate)
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

            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand(query, cIntegration);

            cmd.Parameters.AddWithValue("@clientId", clientId);
            cmd.Parameters.AddWithValue("@clientName", clientName);
            cmd.Parameters.AddWithValue("@createdDate", createdDate);
            cmd.Parameters.AddWithValue("@invoiceId", invoiceId);
            cmd.Parameters.AddWithValue("@amount", amount);
            cmd.Parameters.AddWithValue("@usamount", usamount);
            cmd.Parameters.AddWithValue("@prepstat", prepstat);
            cmd.Parameters.AddWithValue("@referenceNumber", referenceNumber);
            cmd.Parameters.AddWithValue("@destinationBank", destinationBank);
            cmd.Parameters.AddWithValue("@isPayByCredit", isPayByCredit);
            cmd.Parameters.AddWithValue("@prepaymentUsRate", prepaymentUsRate);
            cmd.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);
        }

        public static string GetAccountNumber(int GLID)
        {
            string query = "select GLAccountNumber from tblGLAccounts where GLAccountID=@GLID";
            OpenConnection(CGENERIC);
            SqlCommand cmd = new SqlCommand(query, cGeneric);
            SqlDataReader reader;

            string accountNumber = "";
            cmd.Parameters.AddWithValue("@GLID", GLID);
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                accountNumber = reader[0].ToString();
            }

            CloseConnection(CGENERIC);
            return accountNumber;
        }

        public static List<string> GetInvoiceDetails(int invoiceId)
        {
            string query = "select TargetBatch, EntryNumber, CreditGL, Amount from InvoiceList where invoiceId=@invoiceId ";
            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand(query, cIntegration);
            SqlDataReader reader;
            List<string> data = new List<string>(3);

            cmd.Parameters.AddWithValue("@invoiceId", invoiceId);
            reader = cmd.ExecuteReader();

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

            CloseConnection(CINTEGRATION);
            return data;
        }

        public static void MarkTransferred(int invoiceId)
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

            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand(query, cIntegration);

            cmd.Parameters.AddWithValue("@invoiceId", invoiceId);
            cmd.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);
        }

        public static int GetCreditGl(string invoiceiD)
        {
            string query = "SELECT top 1 CreditGLID FROM tblARInvoiceDetail where ARInvoiceID = @invoiceId ";
            OpenConnection(CGENERIC);
            SqlCommand cmd = new SqlCommand(query, cGeneric);
            SqlDataReader reader;
            cmd.Parameters.AddWithValue("@invoiceId", invoiceiD);

            reader = cmd.ExecuteReader();
            int i = 0;

            if (reader.HasRows)
            {
                reader.Read();
                i = Convert.ToInt32(reader[0].ToString());
            }

            CloseConnection(CGENERIC);
            return i;
        }

        public static int GetCreditGlID(string GLTransactionID)
        {
            string query = "SELECT GLID FROM tblARPayments WHERE GLTransactionID = @GLTransactionID ";
            OpenConnection(CGENERIC);
            SqlCommand cmd = new SqlCommand(query, cGeneric);
            SqlDataReader reader;
            cmd.Parameters.AddWithValue("@GLTransactionID", GLTransactionID);


            reader = cmd.ExecuteReader();
            int i = 0;

            if (reader.HasRows)
            {
                reader.Read();
                i = Convert.ToInt32(reader[0].ToString());
            }

            CloseConnection(CGENERIC);
            return i;
        }

        public static string IsAnnualFee(int invoiceid)
        {
            string query = "select notes from TblARInvoices where ARInvoiceID = @invoiceId";
            OpenConnection(CGENERIC);
            SqlCommand cmd = new SqlCommand(query, cGeneric);
            SqlDataReader reader;
            cmd.Parameters.AddWithValue("@invoiceid", invoiceid);

            reader = cmd.ExecuteReader();
            string notes = " ";

            if (reader.HasRows)
            {
                reader.Read();
                notes = reader[0].ToString();
            }

            CloseConnection(CGENERIC);
            return notes;
        }

        public static void UpdateCreditGl(int invoiceId, int newCreditGl)
        {
            string query = "update InvoiceList set CreditGl=@newCreditGl where invoiceId=@invoiceId";
            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand(query, cIntegration);
            cmd.Parameters.AddWithValue("@invoiceId", invoiceId);
            cmd.Parameters.AddWithValue("@newCreditGl", newCreditGl);

            cmd.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);
        }

        public static void ModifyInvoiceList(int invoiceId, decimal rate, string customerId)
        {
            string query = "update InvoiceList set usrate = @usrate where invoiceId = @invoiceid " +
                          "update InvoiceList set clientId = @customerId where invoiceId = @invoiceid ";
            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand(query, cIntegration);
            cmd.Parameters.AddWithValue("@invoiceId", invoiceId);
            cmd.Parameters.AddWithValue("@usrate", rate);
            cmd.Parameters.AddWithValue("@customerId", customerId);

            cmd.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);
        }

        public static void UpdateEntryNumber(int invoiceId)
        {
            string query = "declare @var1 int " +
                        "declare @var2 int " +
                        "select @var2 = TargetBatch from InvoiceList where invoiceId = @invoiceId " +
                        "select @var1 =[Count] from InvoiceBatch where BatchId = @var2 " +
                        "update InvoiceList set EntryNumber = @var1 where invoiceId = @invoiceId";

            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand(query, cIntegration);
            cmd.Parameters.AddWithValue("@invoiceId", invoiceId);

            cmd.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);
        }

        public static List<string> GetPaymentInfo(int gl_id)
        {
            string query = "SELECT Debit, GLID, InvoiceID, Date1 from tblARPayments where GLTransactionID=@id ";
            OpenConnection(CGENERIC);
            SqlCommand cmd_pay = new SqlCommand();
            SqlDataReader reader;

            List<string> data = new List<string>(4);
            cmd_pay.Connection = cGeneric;
            cmd_pay.CommandText = query;
            cmd_pay.Parameters.AddWithValue("@id", gl_id);

            reader = cmd_pay.ExecuteReader();
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

            CloseConnection(CGENERIC);
            return data;
        }

        public static List<string> GetClientInfoInv(string id)
        {
            string query = "SELECT clientCompany, ccNum, clientFname, clientLname from client where clientId=@clientId ";
            OpenConnection(CGENERIC);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            List<string> data = new List<string>(4);

            cmd.Connection = cGeneric;
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@clientId", id);

            reader = cmd.ExecuteReader();
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

            CloseConnection(CGENERIC);
            return data;
        }

        public static List<string> GetFeeInfo(int invoiceId)
        {
            OpenConnection(CGENERIC);
            SqlCommand cmd_inv = new SqlCommand();
            SqlDataReader reader;
            List<string> data = new List<string>(2);

            cmd_inv.Connection = cGeneric;
            cmd_inv.CommandText = "Select FeeType, notes from tblARInvoices where ARInvoiceID=@id_inv";
            cmd_inv.Parameters.AddWithValue("@id_inv", invoiceId);

            reader = cmd_inv.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                var ftype = reader[0].ToString();
                var notes = reader[1].ToString();

                data.Add(ftype);
                data.Add(notes);
            }

            CloseConnection(CGENERIC);
            return data;
        }

        public static void UpdateCustomerCount()
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

            OpenConnection(CINTEGRATION);
            SqlCommand cmd_inv = new SqlCommand();

            cmd_inv.CommandText = query;
            cmd_inv.Connection = cIntegration;

            cmd_inv.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);
        }

        public static void StoreCustomer(string clientId, string clientName)
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

            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@clientId", clientId);
            cmd.Parameters.AddWithValue("@clientName", clientName);
            cmd.Connection = cIntegration;


            cmd.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);
        }

        public static void UpdateReceiptNumber(int transactionId, string referenceNumber)
        {
            string query = "update tblARPayments set ReceiptNumber=@reference where ReceiptNumber=@receiptNum";
            OpenConnection(CGENERIC);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@receiptNum", transactionId.ToString());
            cmd.Parameters.AddWithValue("@reference", referenceNumber);
            cmd.Connection = cGeneric;

            cmd.ExecuteNonQuery();
            CloseConnection(CGENERIC);
        }

        public static DateTime GetValidity(int invoiceId)
        {
            string query = "SELECT cus.clientID, cus.ccNum, cus.clientCompany, cus.clientFname, cus.clientLname, Amount, val.ValidFrom, val.ValidTo, " +
                            "GL.CreditGLID, Gl.Description, inv.ARInvoiceID " +
                            "FROM  [ASMSGenericMaster].[dbo].tblARInvoices inv, tbl_LicenseValidityHistory val, client cus, " +
                            "tblARInvoiceDetail GL where inv.LicensevalidityHistoryID = val.LicenseValidityHistoryID " +
                            "AND val.ClientID = cus.clientID AND (cus.ccNum is not null) AND val.ValidTo >=GETDATE() AND inv.ARInvoiceID = GL.ARInvoiceID " +
                            "AND inv.ARInvoiceID =@invoiceId Group by cus.clientID, cus.ccNum, cus.clientCompany, cus.clientFname, cus.clientLname, " +
                            "inv.notes, inv.FeeType, inv.ARInvoiceID, inv.Amount, val.ValidFrom, val.ValidTo, GL.CreditGLID, GL.Description, inv.ARInvoiceID  order by inv.ARInvoiceID desc";

            OpenConnection(CGENERIC);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;

            var datetime = DateTime.Now;
            DateTime startdate = DateTime.Now;

            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@invoiceId", invoiceId);
            cmd.Connection = cGeneric;
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                datetime = Convert.ToDateTime(reader[6]);
            }

            CloseConnection(CGENERIC);
            return datetime;
        }

        public static DateTime GetValidityEnd(int invoiceId)
        {

            string query = "SELECT cus.clientID, cus.ccNum, cus.clientCompany, cus.clientFname, cus.clientLname, Amount, val.ValidFrom, val.ValidTo, " +
                            "GL.CreditGLID, Gl.Description, inv.ARInvoiceID " +
                            "FROM  [ASMSGenericMaster].[dbo].tblARInvoices inv, tbl_LicenseValidityHistory val, client cus, " +
                            "tblARInvoiceDetail GL where inv.LicensevalidityHistoryID = val.LicenseValidityHistoryID " +
                            "AND val.ClientID = cus.clientID AND (cus.ccNum is not null) AND val.ValidTo >=GETDATE() AND inv.ARInvoiceID = GL.ARInvoiceID " +
                            "AND inv.ARInvoiceID =@invoiceId Group by cus.clientID, cus.ccNum, cus.clientCompany, cus.clientFname, cus.clientLname, " +
                            "inv.notes, inv.FeeType, inv.ARInvoiceID, inv.Amount, val.ValidFrom, val.ValidTo, GL.CreditGLID, GL.Description, inv.ARInvoiceID  order by inv.ARInvoiceID desc";

            OpenConnection(CGENERIC);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;
            var datetime = DateTime.Now;

            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@invoiceId", invoiceId);
            cmd.Connection = cGeneric;
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                datetime = Convert.ToDateTime(reader[7]);
            }

            CloseConnection(CGENERIC);
            return datetime;
        }

        public static void ResetInvoiceTotal()
        {
            string query = "update InvoiceTotal set total=0";
            OpenConnection(CINTEGRATION);
            SqlCommand cmd_inv = new SqlCommand();

            cmd_inv.CommandText = query;
            cmd_inv.Connection = cIntegration;

            cmd_inv.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);
        }

        public static string GetFreqUsage(int invoiceId)
        {
            string query = "select FreqUsage FROM tblARInvoiceDetail where ARInvoiceID=@invoiceId";
            OpenConnection(CGENERIC);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;

            string result = "";
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@invoiceId", invoiceId);
            cmd.Connection = cGeneric;

            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                result = reader[0].ToString();
            }

            CloseConnection(CGENERIC);
            return result;
        }

        public static int GetCreditMemoNumber()
        {
            string query = "select creditMemoSequence from counters " +
                          "update counters set creditMemoSequence = creditMemoSequence + 1";

            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;
            int num = -1;

            cmd.CommandText = query;
            cmd.Connection = cIntegration;
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                num = Convert.ToInt32(reader[0]);
            }

            CloseConnection(CINTEGRATION);
            return num;
        }

        public static void UpdateAsmsCreditMemoNumber(int docId, int newCredNum)
        {
            string query = "update tblGLDocuments set DocumentDisplayNumber = @newCredNum where DocumentID=@documentId";
            OpenConnection(CGENERIC);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@documentId", docId);
            cmd.Parameters.AddWithValue("@newCredNum", newCredNum);
            cmd.Connection = cGeneric;

            cmd.ExecuteNonQuery();
            CloseConnection(CGENERIC);
        }

        public static InvoiceInfo GetInvoiceInfo(int invoiceId)
        {
            string query = "Declare @Glid integer " +
                        "Declare @FreqUsage varchar(50) " +
                        "set @Glid = (select top 1 CreditGLID from tblARInvoiceDetail where ARInvoiceID = @invoiceId) " +
                        "set @FreqUsage = (select top 1 FreqUsage from tblARInvoiceDetail where ARInvoiceID = @invoiceId) " +
                        "select CustomerId, FeeType, notes, Amount, isvoided, @Glid as Glid, @FreqUsage as FreqUsage, Author from tblARInvoices where ARInvoiceID = @invoiceId";

            OpenConnection(CGENERIC);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;
            InvoiceInfo inv = new InvoiceInfo();

            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@invoiceId", invoiceId);
            cmd.Connection = cGeneric;
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                inv.CustomerId = Convert.ToInt32(reader["CustomerId"]);
                inv.FeeType = reader["FeeType"].ToString();
                inv.notes = reader["notes"].ToString();
                inv.amount = Convert.ToDecimal(reader["Amount"]);
                inv.isvoided = Convert.ToInt32(reader["isvoided"]);
                inv.Glid = Convert.ToInt32(reader["Glid"]);
                inv.FreqUsage = reader["FreqUsage"].ToString();
                inv.Author = reader["Author"].ToString();
            }

            CloseConnection(CGENERIC);
            return inv;
        }

        public static PaymentInfo GetReceiptInfo(int originalDocNum)
        {
            string query = "select ReceiptNumber, GLTransactionID, CustomerID, Debit, InvoiceID, Date1, GLID from tblARPayments where GLTransactionID=@originalDocNum";
            OpenConnection(CGENERIC);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;
            PaymentInfo rct = new PaymentInfo();

            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@originalDocNum", originalDocNum);
            cmd.Connection = cGeneric;

            reader = cmd.ExecuteReader();

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
            CloseConnection(CGENERIC);
            return rct;
        }

        public static CreditNoteInfo GetCreditNoteInfo(int creditMemoNum, int documentId)
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

            OpenConnection(CGENERIC);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;
            CreditNoteInfo creditNote = new CreditNoteInfo();

            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@creditMemoNum", creditMemoNum);
            cmd.Parameters.AddWithValue("@documentId", documentId);
            cmd.Connection = cGeneric;
            reader = cmd.ExecuteReader();

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

            CloseConnection(CGENERIC);
            return creditNote;
        }

        public static string GetClientIdZRecord(bool stripExtention)
        {
            string query = "select * from InvoiceList where invoiceId=0";
            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;
            string result = "";
            string temp = "";

            cmd.CommandText = query;
            cmd.Connection = cIntegration;
            reader = cmd.ExecuteReader();

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
                else
                {
                    CloseConnection(CINTEGRATION);
                    return result;
                }
            }

            CloseConnection(CINTEGRATION);
            return result;
        }

        public static PrepaymentData CheckPrepaymentAvail(string customerId)
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

            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;
            PrepaymentData data = new PrepaymentData();

            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@customerId", customerId);
            cmd.Connection = cIntegration;
            reader = cmd.ExecuteReader();

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

            CloseConnection(CINTEGRATION);
            return data;
        }

        public static void AdjustPrepaymentRemainder(decimal amount, int sequenceNumber)
        {
            string query = "update PaymentList set prepaymentRemainder=prepaymentRemainder-@amount where sequence=@sequenceNumber";
            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@amount", amount);
            cmd.Parameters.AddWithValue("@sequenceNumber", sequenceNumber);
            cmd.Connection = cIntegration;

            cmd.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);
        }

        public static string GenerateReportId(string ReportType)
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

            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@ReportType", ReportType);
            cmd.Connection = cIntegration;
            string result = "";

            reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                result = reader[0].ToString();
            }

            CloseConnection(CINTEGRATION);
            return result;
        }

        public static void DataRouter(string ReportType, DataWrapper data, string recordID, int destination)
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

            OpenConnection(CINTEGRATION);
            for (int i = 0; i < data.records.Count; i++)
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = query;
                cmd.Connection = cIntegration;

                cmd.Parameters.AddWithValue("@ReportType", ReportType);
                cmd.Parameters.AddWithValue("@reportId", recordID);
                cmd.Parameters.AddWithValue("@licenseNumber", data.records[i].licenseNumber);
                cmd.Parameters.AddWithValue("@clientCompany", data.records[i].clientCompany);
                cmd.Parameters.AddWithValue("@invoiceID", data.records[i].invoiceID);
                cmd.Parameters.AddWithValue("@budget", data.records[i].budget);
                cmd.Parameters.AddWithValue("@invoiceTotal", data.records[i].invoiceTotal);
                cmd.Parameters.AddWithValue("@thisPeriodsInv", data.records[i].thisPeriodsInv);
                cmd.Parameters.AddWithValue("@balBFwd", data.records[i].balBFwd);
                cmd.Parameters.AddWithValue("@fromRev", data.records[i].fromRev);
                cmd.Parameters.AddWithValue("@toRev", data.records[i].toRev);
                cmd.Parameters.AddWithValue("@closingBal", data.records[i].closingBal);
                cmd.Parameters.AddWithValue("@totalMonths", data.records[i].totalMonths);
                cmd.Parameters.AddWithValue("@monthUtil", data.records[i].monthUtil);
                cmd.Parameters.AddWithValue("@monthRemain", data.records[i].monthRemain);
                cmd.Parameters.AddWithValue("@valPStart", data.records[i].valPStart);
                cmd.Parameters.AddWithValue("@valPEnd", data.records[i].valPEnd);
                cmd.Parameters.AddWithValue("@destination", destination);
                cmd.ExecuteNonQuery();
            }

            CloseConnection(CINTEGRATION);
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

        public static void InsertSubtotals(string ReportType, string reportID, DataWrapper data, int destination)
        {
            string query = "declare @sql as nvarchar (max) " +
                        "set @sql = 'insert into ' + @ReportType + 'DIR_SubTotals (record_id, category, invoiceTotal, balanceBFwd, toRev, closingBal, fromRev, budget) " +
                                    "values(@record_id, @category, @invoiceTotal, @balanceBFwd, @toRev, @closingBal, @fromRev, @budget)'; " +

                        "exec sp_executesql @sql, " +
                            "N'@record_id varchar(50),@category integer,@invoiceTotal varchar(50),@balanceBFwd varchar(50),@toRev varchar(50),@closingBal varchar(50), " +
                            "@fromRev varchar(50),@budget varchar(50)', " +
                            "@record_id,@category,@invoiceTotal,@balanceBFwd,@toRev,@closingBal,@fromRev,@budget";

            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = query;
            cmd.Connection = cIntegration;

            cmd.Parameters.AddWithValue("@ReportType", ReportType);
            cmd.Parameters.AddWithValue("@record_id", reportID);
            cmd.Parameters.AddWithValue("@category", destination);
            cmd.Parameters.AddWithValue("@invoiceTotal", data.subT_invoiceTotal);
            cmd.Parameters.AddWithValue("@balanceBFwd", data.subT_balBFwd);
            cmd.Parameters.AddWithValue("@toRev", data.subT_toRev);
            cmd.Parameters.AddWithValue("@closingBal", data.subT_closingBal);
            cmd.Parameters.AddWithValue("@fromRev", data.subT_fromRev);
            cmd.Parameters.AddWithValue("@budget", data.subT_budget);

            cmd.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);
        }

        public static void InsertTotals(string ReportType, string reportID, Totals total)
        {
            string query = "declare @sql as nvarchar (max) " +
                        "set @sql = 'insert into ' + @ReportType + 'DIR_Totals (record_id, invoiceTotal, balanceBFwd, toRev, closingBal, fromRev, budget) " +
                                    "values(@recordID, @invoiceTotal, @balanceBFwd, @toRev, @closingBal, @fromRev, @budget)'; " +

                        "exec sp_executesql @sql, " +
                            "N'@recordID varchar(50),@invoiceTotal varchar(50),@balanceBFwd varchar(50),@toRev varchar(50),@closingBal varchar(50), " +

                            "@fromRev varchar(50),@budget varchar(50)', " +
                            "@recordID,@invoiceTotal,@balanceBFwd,@toRev,@closingBal,@fromRev,@budget";

            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = query;
            cmd.Connection = cIntegration;

            cmd.Parameters.AddWithValue("@ReportType", ReportType);
            cmd.Parameters.AddWithValue("@recordID", reportID);
            cmd.Parameters.AddWithValue("@invoiceTotal", total.tot_invoiceTotal);
            cmd.Parameters.AddWithValue("@balanceBFwd", total.tot_balBFwd);
            cmd.Parameters.AddWithValue("@toRev", total.tot_toRev);
            cmd.Parameters.AddWithValue("@closingBal", total.tot_closingBal);
            cmd.Parameters.AddWithValue("@fromRev", total.tot_fromRev);
            cmd.Parameters.AddWithValue("@budget", total.tot_budget);

            cmd.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);
        }

        public static void SetNextGenDate(string ReportType, DateTime date)
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

            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = query;
            cmd.Connection = cIntegration;

            cmd.Parameters.AddWithValue("@ReportType", ReportType);
            cmd.Parameters.AddWithValue("@date", date);
            cmd.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);
        }

        public static DateTime GetNextGenDate(string ReportType)
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

            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            DateTime date = DateTime.Now;

            cmd.Connection = cIntegration;
            cmd.CommandText = query;

            cmd.Parameters.AddWithValue("@ReportType", ReportType);
            reader = cmd.ExecuteReader();

            reader.Read();
            date = Convert.ToDateTime(reader["date"]);

            CloseConnection(CINTEGRATION);
            return date;
        }
    }
}
