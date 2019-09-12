using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using System.Timers;
using ACCPAC.Advantage;
using middleware_service.Database_Classes;
using middleware_service.Database_Operations;
using middleware_service.Other_Classes;
using middleware_service.TableDependencyDefinitions;
using TableDependency.Enums;
using TableDependency.EventArgs;
using TableDependency.SqlClient;
using Microsoft.Owin;
using Microsoft.Owin.Hosting;
using Microsoft.AspNet.SignalR;

namespace middleware_service
{
    public partial class MiddlewareService : ServiceBase
    {
        #region Definitions
        SqlTableDependency<SqlNotifyCancellation> tableDependCancellation;
        SqlTableDependency<SqlNotify_DocumentInfo> tableDependInfo;

        public SqlConnection connGeneric;
        public SqlConnection connIntegration;
        public SqlConnection connMsgQueue;

        Session accpacSession;
        DBLink dbLink;
        Integration intLink;

        View CBBTCH1batch;
        View CBBTCH1header;
        View CBBTCH1detail1;
        View CBBTCH1detail2;
        View CBBTCH1detail3;
        View CBBTCH1detail4;
        View CBBTCH1detail5;
        View CBBTCH1detail6;
        View CBBTCH1detail7;
        View CBBTCH1detail8;

        View b1_arInvoiceBatch;
        View b1_arInvoiceHeader;
        View b1_arInvoiceDetail;
        View b1_arInvoicePaymentSchedules;
        View b1_arInvoiceHeaderOptFields;
        View b1_arInvoiceDetailOptFields;

        View arRecptBatch;
        View arRecptHeader;
        View arRecptDetail1;
        View arRecptDetail2;
        View arRecptDetail3;
        View arRecptDetail4;
        View arRecptDetail5;
        View arRecptDetail6;

        View csRateHeader;
        View csRateDetail;

        public const string TYPE_APPROVAL = "Type Approval";
        public const string RENEWAL_REG = "Renewals - Reg Fees - For ";
        public const string RENEWAL_SPEC = "Renewals - Spec Fees - For ";
        public const string MAJ = "Maj";
        public const string NON_MAJ = "Non Maj";
        public const string CREDIT_NOTE = "Credit Note";
        public const string ONE_DAY = "1";
        public const int INVOICE = 4;
        public const int CREDIT_MEMO = 5;
        public const int RECEIPT = 11;

        int prevInvoice = -100;
        int currentInvoice = -1;
        DateTime prevTime;
        DateTime currentTime;
        System.Timers.Timer deferredTimer = new System.Timers.Timer();
        public IDisposable selfHost;
        #endregion
        public MiddlewareService()
        {
            InitializeComponent();
            event_logger = new EventLog();

            event_logger.Source = "middleware_service";
            event_logger.Log = "Application";
        }
      
        private void DeferredTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            DateTime MonthlyRptDate = intLink.GetNextGenDate("Monthly");
            DateTime AnnualRptDate = intLink.GetNextGenDate("Annual");

            if (DateTime.Now.Year == MonthlyRptDate.Year && DateTime.Now.Month == MonthlyRptDate.Month && DateTime.Now.Day == MonthlyRptDate.Day)
            {
                if (DateTime.Now.Hour == MonthlyRptDate.Hour)
                {
                    int m = DateTime.Now.Month - 1;
                    int y = DateTime.Now.Year;

                    if (m == 0)
                    {
                        m = 12;
                        y = y - 1;
                    }

                    Log.Save("Generating Monthly Deferred Income Report... | Month: " + m + ", Year: " + y);
                    Database_Operations.Report rpt = new Database_Operations.Report(intLink);
                    var result = rpt.gen_rpt("Monthly", 0, m, y);

                    if (result != null)
                    {
                        int es = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) - DateTime.Now.Day;
                        es++;
                        DateTime nextMonth = DateTime.Now.AddDays(es);
                        DateTime nextGenDate = new DateTime(nextMonth.Year, nextMonth.Month, 2);
                        nextGenDate = nextGenDate.AddHours(2);
                        intLink.SetNextGenDate("Monthly", nextGenDate);
                        Log.Save("Monthly Deferred Report Generated.");
                    }
                }
            }

            if (DateTime.Now.Year == AnnualRptDate.Year && DateTime.Now.Month == AnnualRptDate.Month && DateTime.Now.Day == AnnualRptDate.Day)
            {
                if (DateTime.Now.Hour == AnnualRptDate.Hour)
                {
                    int m = 4;
                    int y = DateTime.Now.Year - 1;

                    Log.Save("Generating Annual Deferred Income Report...");
                    Database_Operations.Report rpt = new Database_Operations.Report(intLink);
                    var result = rpt.gen_rpt("Monthly", 0, m, y);

                    if (result != null)
                    {
                        DateTime nextGenDate = new DateTime(DateTime.Now.Year + 1, 4, 2);
                        nextGenDate = nextGenDate.AddHours(3);
                        intLink.SetNextGenDate("Annual", nextGenDate);
                        Log.Save("Annual Deferred Report Generated.");
                    }
                }
            }
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                intLink = new Integration();
                Log.Init(intLink);
               var detail = intLink.GetMajDetail(2069);
                accpacSession = new Session();

                using (tableDependCancellation = new SqlTableDependency<SqlNotifyCancellation>(Constants.DBGENERIC, "tblARInvoices"))
                {
                    tableDependCancellation.OnChanged += TableDependCancellation_OnChanged;
                    tableDependCancellation.OnError += TableDependCancellation_OnError;
                }

                using (tableDependInfo = new SqlTableDependency<SqlNotify_DocumentInfo>(Constants.DBGENERIC, "tblGLDocuments"))
                {
                    tableDependInfo.OnChanged += TableDependInfo_OnChanged;
                    tableDependInfo.OnError += TableDependInfo_OnError;
                }

                deferredTimer.Elapsed += DeferredTimer_Elapsed;
                deferredTimer.Enabled = true;
                deferredTimer.Interval = 3600000;

                //////////////////////////////////////////////////////////////////// STARTING SESSION ///////////////////////////////////////////////////////////////////////
                Log.Save("Starting accpac session...");
                accpacSession.Init("", "XY", "XY1000", "65A");
                accpacSession.Open("ADMIN", "SPECTRUM9", "SMALTD", DateTime.Today, 0);
                dbLink = accpacSession.OpenDBLink(DBLinkType.Company, DBLinkFlags.ReadWrite);

                Log.Save("Accpac Version: " + accpacSession.AppVersion);
                Log.Save("Company: " + accpacSession.CompanyName);
                Log.Save("Session Status: " + accpacSession.IsOpened);

                tableDependCancellation.Start();
                tableDependInfo.Start();

                deferredTimer.Start();
                Log.Save("middleware_service started.");
                Log.WriteEnd();
                DeferredTimer_Elapsed(null, null);
                currentTime = DateTime.Now;
                //InitSignalR();
            }
            catch (Exception e)
            {
                Log.Save(e.Message + " " + e.StackTrace);
                Log.WriteEnd();
                Stop();
            }
        }

        protected override void OnShutdown()
        {
            Log.Save("Machine shutdown event detected");
            base.OnShutdown();
        }

        protected override void OnStop()
        {
            try
            {
                tableDependCancellation.Stop();
                tableDependInfo.Stop();

                tableDependCancellation.Dispose();
                tableDependInfo.Dispose();
                deferredTimer.Stop();

                if (selfHost != null)
                {
                    selfHost.Dispose();
                }

                Log.Save("middleware_service stopped.");
                Log.WriteEnd();
            }
            catch (Exception e)
            {
                Log.Save(e.Message + " " + e.StackTrace);
                Log.WriteEnd();
            }
        }

        public void SignalEventHandler(object source, SignalR.EventObjects.SignalArgs args)
        {
            Log.Save("Command: " + args.message+ " received from: " + args.username);
        }

        private void InitSignalR()
        {
            Log.Save("Starting server host...");
            EventBridge.SignalReceived += SignalEventHandler;
            string url = Constants.BASE_ADDRESS + Constants.PORT;
            selfHost = WebApp.Start<Startup>(url);
            Log.Save("Server running on port: " + Constants.PORT);
        }

        public static void BroadcastEvent(object e)
        {
            IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<EventHub>();
            hub.Clients.All.Event(e);
        }

        private void TableDependCancellation_OnError(object sender, TableDependency.EventArgs.ErrorEventArgs e)
        {
            Log.Save("Table Dependency error: " + e.Error.Message);
        }

        private void TableDependInfo_OnError(object sender, TableDependency.EventArgs.ErrorEventArgs e)
        {
            Log.Save("Table Dependency error: " + e.Error.Message);
        }

        private void TableDependInfo_OnChanged(object sender, RecordChangedEventArgs<SqlNotify_DocumentInfo> e)
        {
            if (IsAccpacSessionOpen())
            {
                Log.Save("Database change detected, operation: " + e.ChangeType);
                try
                {
                    var docInfo = e.Entity;
                    if (e.ChangeType == ChangeType.Insert)
                    {
                        if (docInfo.DocumentType == INVOICE)
                        {
                            Log.Save("Incoming invoice...");
                            InvoiceInfo invoiceInfo = new InvoiceInfo();

                            while (invoiceInfo.amount == 0)
                            {
                                Log.Save("Waiting for invoice amount to update, current value: " + invoiceInfo.amount.ToString());
                                invoiceInfo = intLink.GetInvoiceInfo(docInfo.OriginalDocumentID);
                            }
                            Log.Save("Invoice amount: " + invoiceInfo.amount);

                            List<string> clientInfo = intLink.GetClientInfoInv(invoiceInfo.CustomerId.ToString());
                            string companyName = clientInfo[0].ToString();
                            string cNum = clientInfo[1].ToString();
                            string fname = clientInfo[2].ToString();
                            string lname = clientInfo[3].ToString();
                            Maj m = new Maj();

                            InsertionReturn stat = new InsertionReturn();
                            if (companyName == "" || companyName == " " || companyName == null)
                            {
                                companyName = fname + " " + lname;
                            }

                            Log.Save("Client name: " + companyName);
                            Data dt = Translate(cNum, invoiceInfo.FeeType, companyName, "", invoiceInfo.notes, intLink.GetAccountNumber(invoiceInfo.Glid), invoiceInfo.FreqUsage);
                            DateTime invoiceValidity = intLink.GetValidity(docInfo.OriginalDocumentID);
                            Log.Save("Invoice Validity: " + invoiceValidity);

                            int financialyear = 0;
                            if (invoiceValidity.Month > 3)
                            {
                                financialyear = invoiceValidity.Year + 1;
                            }
                            else
                            {
                                financialyear = invoiceValidity.Year;
                            }

                            Log.Save("Financial Year: " + financialyear);
                            List<string> data = intLink.CheckInvoiceAvail(docInfo.OriginalDocumentID.ToString());
                            int r = intLink.GetInvoiceReference(docInfo.OriginalDocumentID);
                            Log.Save("Invoice Reference number: " + r);

                            if (r != -1)
                            {
                                Log.Save("Getting Maj Details...");
                                m = intLink.GetMajDetail(r);
                                Log.Save(m.ToString());
                            }

                            if (IsPeriodCreated(financialyear))
                            {
                                if (invoiceInfo.Glid < 5000 || data != null)
                                {
                                    Log.Save("CreditGL: " + invoiceInfo.Glid);
                                    if (data != null)
                                    {
                                        if (data[1].ToString() == "NT")
                                        {
                                            if (dt.feeType == "SLF" && invoiceInfo.notes == "Renewal")
                                            {
                                                stat = InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), GetBatch(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()).ToString());

                                                if (stat.status == "Not Exist")
                                                {
                                                    CreateCustomer(dt.customerId, dt.companyName_NewCust);
                                                    InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), GetBatch(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()).ToString());

                                                    intLink.UpdateBatchCount(RENEWAL_SPEC + "For " + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString());
                                                    intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                                    intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);

                                                    prevInvoice = currentInvoice;

                                                    intLink.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                                    intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                                    prevInvoice = currentInvoice;
                                                }
                                                else
                                                {
                                                    intLink.UpdateBatchCount(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString());
                                                    intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                                    intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);

                                                    prevInvoice = currentInvoice;
                                                    intLink.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString() + " For " + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                                    intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                                    prevInvoice = currentInvoice;
                                                }
                                            }
                                            else if (dt.feeType == "RF" && invoiceInfo.notes == "Renewal")
                                            {
                                                stat = InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), GetBatch(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()).ToString());

                                                if (stat.status == "Not Exist")
                                                {
                                                    CreateCustomer(dt.customerId, dt.companyName_NewCust);
                                                    InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), GetBatch(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()).ToString());

                                                    intLink.UpdateBatchCount(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString());
                                                    intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                                    intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);

                                                    prevInvoice = currentInvoice;
                                                    intLink.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                                    intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                                    prevInvoice = currentInvoice;
                                                }
                                                else
                                                {
                                                    intLink.UpdateBatchCount(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString());
                                                    intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);

                                                    intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);

                                                    prevInvoice = currentInvoice;
                                                    intLink.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                                    intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                                    prevInvoice = currentInvoice;
                                                }
                                            }
                                            else if ((invoiceInfo.notes == "Annual Fee" && m.stationType == "SSL" && m.certificateType == 0 && m.proj == "JMC") || (invoiceInfo.FreqUsage == "PRS55"))
                                            {
                                                stat = InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), GetBatch(MAJ, docInfo.OriginalDocumentID.ToString()).ToString());

                                                if (stat.status == "Not Exist")
                                                {
                                                    CreateCustomer(dt.customerId, dt.companyName_NewCust);
                                                    InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), GetBatch(MAJ, docInfo.OriginalDocumentID.ToString()).ToString());

                                                    intLink.UpdateBatchCount(MAJ);
                                                    intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                                    intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);

                                                    prevInvoice = currentInvoice;
                                                    intLink.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(MAJ, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                                    intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                                    prevInvoice = currentInvoice;
                                                }
                                                else
                                                {
                                                    intLink.UpdateBatchCount(MAJ);
                                                    intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                                    intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);

                                                    prevInvoice = currentInvoice;
                                                    intLink.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(MAJ, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                                    intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                                    prevInvoice = currentInvoice;
                                                }
                                            }
                                            else if (invoiceInfo.notes == "Type Approval" || invoiceInfo.FreqUsage == "TA-ProAmend")
                                            {
                                                stat = InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, ChangeToUS(invoiceInfo.amount).ToString(), GetBatch(TYPE_APPROVAL, docInfo.OriginalDocumentID.ToString()).ToString());

                                                if (stat.status == "Not Exist")
                                                {
                                                    CreateCustomer(dt.customerId, dt.companyName_NewCust);
                                                    InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, ChangeToUS(invoiceInfo.amount).ToString(), GetBatch(TYPE_APPROVAL, docInfo.OriginalDocumentID.ToString()).ToString());

                                                    intLink.UpdateBatchCount(TYPE_APPROVAL);
                                                    intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                                    intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);

                                                    prevInvoice = currentInvoice;
                                                    intLink.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(TYPE_APPROVAL, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", intLink.GetRate(), ChangeToUS(invoiceInfo.amount), invoiceInfo.isvoided, 0, 0);
                                                    intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                                    prevInvoice = currentInvoice;
                                                }
                                                else
                                                {
                                                    intLink.UpdateBatchCount(TYPE_APPROVAL);
                                                    intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                                    intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);

                                                    prevInvoice = currentInvoice;
                                                    intLink.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(TYPE_APPROVAL, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", intLink.GetRate(), ChangeToUS(invoiceInfo.amount), invoiceInfo.isvoided, 0, 0);
                                                    intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                                    prevInvoice = currentInvoice;
                                                }
                                            }

                                            else if (invoiceInfo.notes == "Annual Fee" || invoiceInfo.notes == "Modification" || invoiceInfo.notes == "Radio Operator")
                                            {
                                                stat = InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), GetBatch(NON_MAJ, docInfo.OriginalDocumentID.ToString()).ToString());

                                                if (stat.status == "Not Exist")
                                                {
                                                    CreateCustomer(dt.customerId, dt.companyName_NewCust);
                                                    InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), GetBatch(NON_MAJ, docInfo.OriginalDocumentID.ToString()).ToString());

                                                    intLink.UpdateBatchCount(NON_MAJ);
                                                    intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                                    intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);

                                                    prevInvoice = currentInvoice;
                                                    intLink.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(NON_MAJ, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                                    intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                                    prevInvoice = currentInvoice;
                                                }
                                                else
                                                {
                                                    intLink.UpdateBatchCount(NON_MAJ);
                                                    intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                                    intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);

                                                    prevInvoice = currentInvoice;
                                                    intLink.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(NON_MAJ, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                                    intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                                    prevInvoice = currentInvoice;
                                                }
                                            }
                                        }
                                        else if (data[1].ToString() == "T")
                                        {
                                            List<string> detail = new List<string>(3);
                                            detail = intLink.GetInvoiceDetails(docInfo.OriginalDocumentID);
                                            int batchNumber = GetIbatchNumber(docInfo.OriginalDocumentID);


                                            if (!CheckAccpacIBatchPosted(batchNumber))
                                            {
                                                if (invoiceInfo.Glid != Convert.ToInt32(detail[2].ToString()) || invoiceInfo.amount.ToString() != detail[3].ToString())
                                                {
                                                    if (invoiceInfo.notes == "Type Approval" || invoiceInfo.FreqUsage == "TA-ProAmend")
                                                    {
                                                        string usamt = "";
                                                        usamt = ChangeToUSupdated(invoiceInfo.amount, docInfo.OriginalDocumentID).ToString();
                                                        UpdateInvoice(dt.fcode, Math.Round(ChangeToUSupdated(invoiceInfo.amount, docInfo.OriginalDocumentID), 2).ToString(), batchNumber.ToString().ToString(), GetEntryNumber(docInfo.OriginalDocumentID).ToString());
                                                        intLink.StoreInvoice(docInfo.OriginalDocumentID, batchNumber, invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "updated", 1, Convert.ToDecimal(usamt), invoiceInfo.isvoided, 0, 0);
                                                    }
                                                    else
                                                    {
                                                        UpdateInvoice(dt.fcode, invoiceInfo.amount.ToString(), batchNumber.ToString().ToString(), GetEntryNumber(docInfo.OriginalDocumentID).ToString());
                                                        intLink.StoreInvoice(docInfo.OriginalDocumentID, batchNumber, invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "updated", 1, 0, invoiceInfo.isvoided, 0, 0);

                                                    }
                                                    intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);
                                                    Log.Save("Updated Invoice: " + docInfo.OriginalDocumentID.ToString());
                                                    prevInvoice = currentInvoice;
                                                }
                                                else
                                                {
                                                    prevInvoice = currentInvoice;
                                                    Log.Save("Update is not needed." + docInfo.OriginalDocumentID.ToString());
                                                }
                                            }
                                            else
                                            {
                                                Log.Save("Batch number already posted");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (dt.feeType == "SLF" && invoiceInfo.notes == "Renewal")
                                        {
                                            intLink.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                            Log.Save("Invoice Id: " + docInfo.OriginalDocumentID.ToString() + " Transferred");
                                            prevInvoice = currentInvoice;
                                            prevTime = DateTime.Now;
                                        }
                                        else if (dt.feeType == "RF" && invoiceInfo.notes == "Renewal")
                                        {
                                            DataSet df = new DataSet();
                                            df = intLink.GetRenewalInvoiceValidity(docInfo.OriginalDocumentID);
                                            DateTime val = DateTime.Now;
                                            if (!IsEmpty(df))
                                            {
                                                DataRow dr = df.Tables[0].Rows[0];
                                                string date = dr.ItemArray.GetValue(0).ToString();
                                                val = Convert.ToDateTime(date);
                                            }

                                            intLink.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                            Log.Save("Invoice Id: " + docInfo.OriginalDocumentID.ToString() + " Transferred");
                                            prevInvoice = currentInvoice;
                                            prevTime = DateTime.Now;
                                        }

                                        else if ((invoiceInfo.notes == "Annual Fee" && m.stationType == "SSL" && m.certificateType == 0 && m.proj == "JMC") || (invoiceInfo.FreqUsage == "PRS55"))
                                        {
                                            intLink.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(MAJ, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                            Log.Save("Invoice Id: " + docInfo.OriginalDocumentID.ToString() + " Transferred");
                                            prevInvoice = currentInvoice;
                                            prevTime = DateTime.Now;
                                        }
                                        else if (invoiceInfo.notes == "Type Approval" || invoiceInfo.FreqUsage == "TA-ProAmend")
                                        {
                                            intLink.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(TYPE_APPROVAL, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", intLink.GetRate(), ChangeToUS(invoiceInfo.amount), invoiceInfo.isvoided, 0, 0);
                                            Log.Save("Invoice Id: " + docInfo.OriginalDocumentID.ToString() + " Transferred");
                                            prevInvoice = currentInvoice;
                                            prevTime = DateTime.Now;
                                        }
                                        else if (invoiceInfo.notes == "Annual Fee" || invoiceInfo.notes == "Modification" || invoiceInfo.notes == "Radio Operator")
                                        {
                                            intLink.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(NON_MAJ, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                            Log.Save("Invoice Id: " + docInfo.OriginalDocumentID.ToString() + " Transferred");
                                            prevInvoice = currentInvoice;
                                            prevTime = DateTime.Now;
                                        }
                                    }
                                }
                                else
                                {
                                    if (dt.feeType == "SLF" && invoiceInfo.notes == "Renewal")
                                    {
                                        stat = InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), GetBatch(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()).ToString());

                                        if (stat.status == "Not Exist")
                                        {
                                            CreateCustomer(dt.customerId, dt.companyName_NewCust);
                                            InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), GetBatch(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()).ToString());

                                            intLink.UpdateBatchCount(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString());
                                            intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                            intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);

                                            prevInvoice = currentInvoice;
                                            intLink.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                            intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                            prevInvoice = currentInvoice;
                                            intLink.UpdateBatchAmount(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), invoiceInfo.amount);
                                        }
                                        else
                                        {
                                            intLink.UpdateBatchCount(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString());
                                            intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);

                                            intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);
                                            prevInvoice = currentInvoice;
                                            intLink.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                            intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                            prevInvoice = currentInvoice;
                                            intLink.UpdateBatchAmount(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), invoiceInfo.amount);
                                        }
                                    }
                                    else if (dt.feeType == "RF" && invoiceInfo.notes == "Renewal")
                                    {
                                        stat = InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), GetBatch(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()).ToString());

                                        if (stat.status == "Not Exist")
                                        {
                                            CreateCustomer(dt.customerId, dt.companyName_NewCust);
                                            InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), GetBatch(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()).ToString());

                                            intLink.UpdateBatchCount(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString());
                                            intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                            intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);

                                            prevInvoice = currentInvoice;
                                            intLink.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                            intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                            prevInvoice = currentInvoice;
                                            intLink.UpdateBatchAmount(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), invoiceInfo.amount);
                                        }
                                        else
                                        {
                                            intLink.UpdateBatchCount(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString());
                                            intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);

                                            intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);

                                            prevInvoice = currentInvoice;
                                            intLink.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                            intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                            prevInvoice = currentInvoice;
                                            intLink.UpdateBatchAmount(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), invoiceInfo.amount);
                                        }
                                    }
                                    else if ((invoiceInfo.notes == "Annual Fee" && m.stationType == "SSL" && m.certificateType == 0 && m.proj == "JMC") || (invoiceInfo.FreqUsage == "PRS55"))
                                    {
                                        stat = InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), GetBatch(MAJ, docInfo.OriginalDocumentID.ToString()).ToString());

                                        if (stat.status == "Not Exist")
                                        {
                                            CreateCustomer(dt.customerId, dt.companyName_NewCust);
                                            InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), GetBatch(MAJ, docInfo.OriginalDocumentID.ToString()).ToString());

                                            intLink.UpdateBatchCount(MAJ);
                                            intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);

                                            intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);

                                            prevInvoice = currentInvoice;
                                            intLink.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(MAJ, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                            intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                            prevInvoice = currentInvoice;
                                            intLink.UpdateBatchAmount(MAJ, invoiceInfo.amount);
                                        }
                                        else
                                        {
                                            intLink.UpdateBatchCount(MAJ);
                                            intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                            intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);

                                            prevInvoice = currentInvoice;
                                            intLink.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(MAJ, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                            intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                            prevInvoice = currentInvoice;
                                            intLink.UpdateBatchAmount(MAJ, invoiceInfo.amount);
                                        }
                                    }
                                    else if (invoiceInfo.notes == "Type Approval" || invoiceInfo.FreqUsage == "TA-ProAmend")
                                    {
                                        stat = InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, ChangeToUS(invoiceInfo.amount).ToString(), GetBatch(TYPE_APPROVAL, docInfo.OriginalDocumentID.ToString()).ToString());

                                        if (stat.status == "Not Exist")
                                        {
                                            CreateCustomer(dt.customerId, dt.companyName_NewCust);
                                            InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, ChangeToUS(invoiceInfo.amount).ToString(), GetBatch(TYPE_APPROVAL, docInfo.OriginalDocumentID.ToString()).ToString());

                                            intLink.UpdateBatchCount(TYPE_APPROVAL);
                                            intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                            intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);

                                            prevInvoice = currentInvoice;
                                            intLink.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(TYPE_APPROVAL, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", intLink.GetRate(), ChangeToUS(invoiceInfo.amount), invoiceInfo.isvoided, 0, 0);
                                            intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                            prevInvoice = currentInvoice;
                                            intLink.UpdateBatchAmount(TYPE_APPROVAL, invoiceInfo.amount);
                                        }
                                        else
                                        {
                                            intLink.UpdateBatchCount(TYPE_APPROVAL);
                                            intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                            intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);

                                            prevInvoice = currentInvoice;
                                            intLink.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(TYPE_APPROVAL, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", intLink.GetRate(), ChangeToUS(invoiceInfo.amount), invoiceInfo.isvoided, 0, 0);
                                            intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                            prevInvoice = currentInvoice;
                                            intLink.UpdateBatchAmount(TYPE_APPROVAL, invoiceInfo.amount);
                                        }
                                    }

                                    else if (invoiceInfo.notes == "Annual Fee" || invoiceInfo.notes == "Modification" || invoiceInfo.notes == "Radio Operator")
                                    {
                                        stat = InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), GetBatch(NON_MAJ, docInfo.OriginalDocumentID.ToString()).ToString());

                                        if (stat.status == "Not Exist")
                                        {
                                            CreateCustomer(dt.customerId, dt.companyName_NewCust);
                                            InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), GetBatch(NON_MAJ, docInfo.OriginalDocumentID.ToString()).ToString());

                                            intLink.UpdateBatchCount(NON_MAJ);
                                            intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                            intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);

                                            prevInvoice = currentInvoice;
                                            intLink.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(NON_MAJ, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                            intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                            prevInvoice = currentInvoice;
                                            intLink.UpdateBatchAmount(NON_MAJ, invoiceInfo.amount);
                                        }
                                        else
                                        {
                                            intLink.UpdateBatchCount(NON_MAJ);
                                            intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                            intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);

                                            prevInvoice = currentInvoice;
                                            intLink.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(NON_MAJ, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                            intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                            prevInvoice = currentInvoice;
                                            intLink.UpdateBatchAmount(NON_MAJ, invoiceInfo.amount);
                                        }
                                    }
                                }

                                prevInvoice = currentInvoice;
                                prevTime = DateTime.Now;
                            }
                            else
                            {
                                Log.Save("Invoice not Transferred as fiscal year " + financialyear + " not yet Created in Sage. ");
                            }
                        }
                        else if (docInfo.DocumentType == RECEIPT && docInfo.PaymentMethod != 99)
                        {
                            Log.Save("Incoming Receipt");
                            Data dt = new Data();
                            PaymentInfo pinfo = new PaymentInfo();

                            List<string> paymentData = new List<string>(3);
                            List<string> clientData = new List<string>(3);
                            List<string> feeData = new List<string>(3);

                            while (pinfo.ReceiptNumber == 0)
                            {
                                pinfo = intLink.GetReceiptInfo(docInfo.OriginalDocumentID);
                                Thread.Sleep(500);
                            }

                            var receipt = pinfo.ReceiptNumber;
                            var transid = pinfo.GLTransactionID;
                            var id = pinfo.CustomerID.ToString();

                            paymentData = intLink.GetPaymentInfo(transid);
                            var debit = pinfo.Debit.ToString();
                            var glid = pinfo.GLID.ToString();
                            var invoiceId = pinfo.InvoiceID.ToString();

                            DateTime paymentDate = pinfo.Date1;
                            string prepstat = " ";
                            DateTime valstart = DateTime.Now.Date;
                            DateTime valend = DateTime.Now.Date;

                            clientData = intLink.GetClientInfoInv(id);
                            var companyName = clientData[0].ToString();
                            var customerId = clientData[1].ToString();
                            var fname = clientData[2].ToString();
                            var lname = clientData[3].ToString();
                            var ftype = " ";
                            var notes = " ";

                            if (companyName == "" || companyName == " " || companyName == null)
                            {
                                companyName = fname + " " + lname;
                            }

                            if (Convert.ToInt32(invoiceId) > 0)
                            {
                                feeData = intLink.GetFeeInfo(Convert.ToInt32(invoiceId));
                                ftype = feeData[0].ToString();
                                notes = feeData[1].ToString();

                                Log.Save("Invoice Id: " + invoiceId.ToString());
                                Log.Save("Customer Id: " + customerId);

                                prepstat = "No";
                                valstart = intLink.GetValidity(Convert.ToInt32(invoiceId));
                                valend = intLink.GetValidityEnd(Convert.ToInt32(invoiceId));
                            }
                            else
                            {
                                prepstat = "Yes";
                                Log.Save("Prepayment");
                                Log.Save("Customer Id: " + customerId);

                                var gl = intLink.GetCreditGlID((transid + 1).ToString());

                                if (gl == 5321)
                                {
                                    ftype = "SLF";
                                }
                                else if (gl == 5149)
                                {
                                    ftype = "RF";
                                }
                            }

                            if (prepstat == "Yes")
                            {
                                dt = Translate(customerId, ftype, companyName, debit, notes, "PREPAYMENT", intLink.GetFreqUsage(Convert.ToInt32(invoiceId)));
                            }
                            else
                            {
                                dt = Translate(customerId, ftype, companyName, debit, notes, "", intLink.GetFreqUsage(Convert.ToInt32(invoiceId)));
                            }

                            bool cusexists;
                            cusexists = CustomerExists(dt.customerId);
                            if (Convert.ToInt32(invoiceId) == 0)
                            {
                                if (!cusexists)
                                {
                                    CreateCustomer(dt.customerId, companyName);
                                    intLink.StoreCustomer(dt.customerId, companyName);
                                }
                            }

                            if (cusexists || Convert.ToInt32(invoiceId) == 0 && Convert.ToInt32(glid) > 0)
                            {
                                if (glid == "5146")
                                {
                                    Log.Save("Bank: FGB JA$ CURRENT A/C");
                                    if (dt.success)
                                    {
                                        if (ReceiptBatchAvail("FGBJMREC"))
                                        {
                                            string reference = intLink.GetCurrentRef("FGBJMREC");
                                            Log.Save("Target Batch: " + intLink.GetRecieptBatch("FGBJMREC"));
                                            Log.Save("Transferring Receipt");

                                            ReceiptTransfer(intLink.GetRecieptBatch("FGBJMREC"), dt.customerId, dt.debit, dt.companyName, reference, invoiceId, paymentDate, dt.desc, customerId, valstart, valend);
                                            intLink.UpdateBatchCountPayment(intLink.GetRecieptBatch("FGBJMREC"));
                                            intLink.UpdateReceiptNumber(receipt, intLink.GetCurrentRef("FGBJMREC"));
                                            intLink.IncrementReferenceNumber(intLink.GetBankCodeId("FGBJMREC"), Convert.ToDecimal(dt.debit));
                                            intLink.StorePayment(dt.customerId, companyName, DateTime.Now, invoiceId, Convert.ToDecimal(dt.debit), 0, prepstat, Convert.ToInt32(reference), Convert.ToInt32(glid), "No", 0);
                                        }
                                        else
                                        {
                                            string reference = intLink.GetCurrentRef("FGBJMREC");
                                            CreateReceiptBatchEx("FGBJMREC", "Middleware Generated Batch for FGBJMREC");
                                            intLink.OpenNewReceiptBatch(1, GetLastPaymentBatch(), "FGBJMREC");

                                            Log.Save("Target Batch: " + intLink.GetRecieptBatch("FGBJMREC"));
                                            Log.Save("Transferring Receipt");

                                            ReceiptTransfer(intLink.GetRecieptBatch("FGBJMREC"), dt.customerId, dt.debit, dt.companyName, reference, invoiceId, paymentDate, dt.desc, customerId, valstart, valend);
                                            intLink.UpdateBatchCountPayment(intLink.GetRecieptBatch("FGBJMREC"));
                                            intLink.UpdateReceiptNumber(receipt, intLink.GetCurrentRef("FGBJMREC"));
                                            intLink.IncrementReferenceNumber(intLink.GetBankCodeId("FGBJMREC"), Convert.ToDecimal(dt.debit));
                                            intLink.StorePayment(dt.customerId, companyName, DateTime.Now, invoiceId, Convert.ToDecimal(dt.debit), 0, prepstat, Convert.ToInt32(reference), Convert.ToInt32(glid), "No", 0);
                                        }
                                    }
                                }
                                else if (glid == "5147")
                                {
                                    Log.Save("Bank: FGB US$ SAVINGS A/C");
                                    decimal usamount = 0;
                                    decimal transferedAmt = Convert.ToDecimal(dt.debit) / intLink.GetUsRateByInvoice(Convert.ToInt32(invoiceId));
                                    string clientIdPrefix = "";
                                    decimal currentRate = 1;

                                    for (int i = 0; i < dt.customerId.Length; i++)
                                    {
                                        if (dt.customerId[i] != '-')
                                        {
                                            clientIdPrefix += dt.customerId[i];
                                        }
                                        else
                                        {
                                            i = dt.customerId.Length;
                                        }
                                    }

                                    if (prepstat == "Yes" && clientIdPrefix == intLink.GetClientIdZRecord(true))
                                    {
                                        dt.customerId = clientIdPrefix + "-T";
                                        currentRate = intLink.GetRate();
                                    }

                                    if (dt.customerId[6] == 'T')
                                    {
                                        usamount = Math.Round(Convert.ToDecimal(dt.debit) / intLink.GetUsRateByInvoice(Convert.ToInt32(invoiceId)), 2);
                                        intLink.ModifyInvoiceList(0, intLink.GetUsRateByInvoice(Convert.ToInt32(invoiceId)), dt.customerId);
                                        currentRate = intLink.GetRate();
                                    }

                                    if (dt.success)
                                    {
                                        if (ReceiptBatchAvail("FGBUSMRC"))
                                        {
                                            string reference = intLink.GetCurrentRef("FGBUSMRC");
                                            Log.Save("Target Batch: " + intLink.GetRecieptBatch("FGBUSMRC"));
                                            Log.Save("Transferring Receipt");

                                            ReceiptTransfer(intLink.GetRecieptBatch("FGBUSMRC"), dt.customerId, Math.Round(transferedAmt, 2).ToString(), dt.companyName, reference, invoiceId, paymentDate, dt.desc, customerId, valstart, valend);
                                            intLink.UpdateBatchCountPayment(intLink.GetRecieptBatch("FGBUSMRC"));
                                            intLink.UpdateReceiptNumber(receipt, intLink.GetCurrentRef("FGBUSMRC"));
                                            intLink.IncrementReferenceNumber(intLink.GetBankCodeId("FGBUSMRC"), Convert.ToDecimal(dt.debit));
                                            intLink.StorePayment(dt.customerId, companyName, DateTime.Now, invoiceId, Convert.ToDecimal(dt.debit), usamount, prepstat, Convert.ToInt32(reference), Convert.ToInt32(glid), "No", currentRate);
                                        }
                                        else
                                        {
                                            string reference = intLink.GetCurrentRef("FGBUSMRC");
                                            CreateReceiptBatchEx("FGBUSMRC", "Middleware Generated Batch for FGBUSMRC");
                                            intLink.OpenNewReceiptBatch(1, GetLastPaymentBatch(), "FGBUSMRC");

                                            Log.Save("Target Batch: " + intLink.GetRecieptBatch("FGBUSMRC"));
                                            Log.Save("Transferring Receipt");

                                            ReceiptTransfer(intLink.GetRecieptBatch("FGBUSMRC"), dt.customerId, Math.Round(transferedAmt, 2).ToString(), dt.companyName, reference, invoiceId, paymentDate, dt.desc, customerId, valstart, valend);
                                            intLink.UpdateBatchCountPayment(intLink.GetRecieptBatch("FGBUSMRC"));
                                            intLink.UpdateReceiptNumber(receipt, intLink.GetCurrentRef("FGBUSMRC"));
                                            intLink.IncrementReferenceNumber(intLink.GetBankCodeId("FGBUSMRC"), Convert.ToDecimal(dt.debit));
                                            intLink.StorePayment(dt.customerId, companyName, DateTime.Now, invoiceId, Convert.ToDecimal(dt.debit), usamount, prepstat, Convert.ToInt32(reference), Convert.ToInt32(glid), "No", currentRate);
                                        }
                                    }
                                }
                                else if (glid == "5148")
                                {
                                    Log.Save("NCB JA$ SAVINGS A/C");
                                    if (dt.success)
                                    {
                                        if (ReceiptBatchAvail("NCBJMREC"))
                                        {
                                            string reference = intLink.GetCurrentRef("NCBJMREC");
                                            Log.Save("Target Batch: " + intLink.GetRecieptBatch("NCBJMREC"));
                                            Log.Save("Transferring Receipt");

                                            ReceiptTransfer(intLink.GetRecieptBatch("NCBJMREC"), dt.customerId, dt.debit, dt.companyName, reference, invoiceId, paymentDate, dt.desc, customerId, valstart, valend);
                                            intLink.UpdateBatchCountPayment(intLink.GetRecieptBatch("NCBJMREC"));
                                            intLink.UpdateReceiptNumber(receipt, intLink.GetCurrentRef("NCBJMREC"));
                                            intLink.IncrementReferenceNumber(intLink.GetBankCodeId("NCBJMREC"), Convert.ToDecimal(dt.debit));
                                            intLink.StorePayment(dt.customerId, companyName, DateTime.Now, invoiceId, Convert.ToDecimal(dt.debit), 0, prepstat, Convert.ToInt32(reference), Convert.ToInt32(glid), "No", 1);
                                        }
                                        else
                                        {
                                            string reference = intLink.GetCurrentRef("NCBJMREC");
                                            CreateReceiptBatchEx("NCBJMREC", "Middleware Generated Batch for NCBJMREC");
                                            intLink.OpenNewReceiptBatch(1, GetLastPaymentBatch(), "NCBJMREC");

                                            Log.Save("Target Batch: " + intLink.GetRecieptBatch("NCBJMREC"));
                                            Log.Save("Transferring Receipt");

                                            ReceiptTransfer(intLink.GetRecieptBatch("NCBJMREC"), dt.customerId, dt.debit, dt.companyName, reference, invoiceId, paymentDate, dt.desc, customerId, valstart, valend);
                                            intLink.UpdateBatchCountPayment(intLink.GetRecieptBatch("NCBJMREC"));
                                            intLink.UpdateReceiptNumber(receipt, intLink.GetCurrentRef("NCBJMREC"));
                                            intLink.IncrementReferenceNumber(intLink.GetBankCodeId("NCBJMREC"), Convert.ToDecimal(dt.debit));
                                            intLink.StorePayment(dt.customerId, companyName, DateTime.Now, invoiceId, Convert.ToDecimal(dt.debit), 0, prepstat, Convert.ToInt32(reference), Convert.ToInt32(glid), "No", 1);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Log.Save("Customer not found in accpac");
                            }
                        }
                        else if (docInfo.DocumentType == CREDIT_MEMO)
                        {
                            Log.Save("New Credit Memo...");
                            CreditNoteInfo creditNote = new CreditNoteInfo();

                            while (creditNote.amount == 0)
                            {
                                creditNote = intLink.GetCreditNoteInfo(docInfo.OriginalDocumentID, docInfo.DocumentID);
                                Thread.Sleep(1000);
                            }

                            List<string> clientInfo = new List<string>(4);
                            clientInfo = intLink.GetClientInfoInv(creditNote.CustomerID.ToString());
                            var accountNum = intLink.GetAccountNumber(creditNote.CreditGL);
                            DateTime invoiceValidity = intLink.GetValidity(creditNote.ARInvoiceID);

                            string companyName = clientInfo[0].ToString();
                            string cNum = clientInfo[1].ToString();
                            string fname = clientInfo[2].ToString();
                            string lname = clientInfo[3].ToString();
                            string creditNoteDesc = creditNote.remarks;
                            int cred_docNum = 0;

                            if (creditNoteDesc == "" || creditNoteDesc == null)
                            {
                                creditNoteDesc = companyName + " - Credit Note";
                            }

                            if (companyName == "" || companyName == " " || companyName == null)
                            {
                                companyName = fname + " " + lname;
                            }

                            Data dt = Translate(cNum, creditNote.FeeType, companyName, creditNote.amount.ToString(), creditNote.notes, accountNum, intLink.GetFreqUsage(creditNote.ARInvoiceID));

                            if (CheckAccpacInvoiceAvail(creditNote.ARInvoiceID))
                            {
                                cred_docNum = intLink.GetCreditMemoNumber();
                                Log.Save("Creating credit memo");
                                int batchNumber = GetBatch(CREDIT_NOTE, creditNote.ARInvoiceID.ToString());

                                intLink.StoreInvoice(creditNote.ARInvoiceID, batchNumber, creditNote.CreditGL, companyName, dt.customerId, DateTime.Now, "", creditNote.amount, "no modification", 1, 0, 0, 1, cred_docNum);
                                CreditNoteInsert(batchNumber.ToString(), dt.customerId, accountNum, creditNote.amount.ToString(), creditNote.ARInvoiceID.ToString(), cred_docNum.ToString(), creditNoteDesc);
                                intLink.UpdateAsmsCreditMemoNumber(docInfo.DocumentID, cred_docNum);
                            }
                            else
                            {
                                Log.Save("The Credit Memo was not created. The Invoice does not exist.");
                                cred_docNum = intLink.GetCreditMemoNumber();
                                intLink.UpdateAsmsCreditMemoNumber(docInfo.DocumentID, cred_docNum);
                                Log.Save("The Credit Memo number in ASMS updated.");
                            }
                        }
                        else if (docInfo.DocumentType == RECEIPT && docInfo.PaymentMethod == 99)
                        {
                            Log.Save("Payment By Credit");
                            PaymentInfo pinfo = intLink.GetReceiptInfo(docInfo.OriginalDocumentID);
                            List<string> clientData = intLink.GetClientInfoInv(pinfo.CustomerID.ToString());
                            List<string> feeData = new List<string>(3);

                            var companyName = clientData[0].ToString();
                            var customerId = clientData[1].ToString();
                            var fname = clientData[2].ToString();
                            var lname = clientData[3].ToString();

                            if (companyName == "" || companyName == " " || companyName == null)
                            {
                                companyName = fname + " " + lname;
                            }

                            feeData = intLink.GetFeeInfo(pinfo.InvoiceID);
                            var ftype = feeData[0].ToString();
                            var notes = feeData[1].ToString();

                            Data dt = Translate(customerId, ftype, companyName, pinfo.Debit.ToString(), notes, "", intLink.GetFreqUsage(pinfo.InvoiceID).ToString());
                            PrepaymentData pData = intLink.CheckPrepaymentAvail(dt.customerId);
                            int invoiceBatch = GetIbatchNumber(pinfo.InvoiceID);
                            int receiptBatch = GetRBatchNumber(pData.referenceNumber);

                            if (pData.dataAvail)
                            {
                                if (CheckAccpacIBatchPosted(invoiceBatch) && CheckAccpacRBatchPosted(receiptBatch))
                                {
                                    var glid = pData.destinationBank.ToString();

                                    if (glid == "5146")
                                    {
                                        Log.Save("Bank: FGB JA$ CURRENT A/C");
                                        if (pData.totalPrepaymentRemainder >= pinfo.Debit)
                                        {
                                            Log.Save("Carrying out Payment By Credit Transaction");
                                            decimal reducingAmt = pinfo.Debit;

                                            if (ReceiptBatchAvail("FGBJMREC"))
                                            {
                                                while (reducingAmt > 0)
                                                {
                                                    pData = intLink.CheckPrepaymentAvail(dt.customerId);
                                                    ComApiPayByCredit(dt.customerId, pinfo.InvoiceID.ToString(), intLink.GetRecieptBatch("FGBJMREC"), GetDocNumber(pData.referenceNumber));
                                                    if (reducingAmt > pData.remainder) intLink.AdjustPrepaymentRemainder(pData.remainder, pData.sequenceNumber);
                                                    else intLink.AdjustPrepaymentRemainder(reducingAmt, pData.sequenceNumber);
                                                    reducingAmt = reducingAmt - pData.remainder;
                                                }
                                                intLink.StorePayment(dt.customerId, companyName, DateTime.Now, pinfo.InvoiceID.ToString(), pinfo.Debit, 0, "No", 0, Convert.ToInt32(glid), "Yes", 1);
                                                Log.Save("Payment by credit transaction complete");
                                            }
                                            else
                                            {
                                                CreateReceiptBatchEx("FGBJMREC", "Middleware Generated Batch for FGBJMREC");
                                                intLink.OpenNewReceiptBatch(1, GetLastPaymentBatch(), "FGBJMREC");
                                                Log.Save("Target Batch: " + intLink.GetRecieptBatch("FGBJMREC"));
                                                while (reducingAmt > 0)
                                                {
                                                    pData = intLink.CheckPrepaymentAvail(dt.customerId);
                                                    ComApiPayByCredit(dt.customerId, pinfo.InvoiceID.ToString(), intLink.GetRecieptBatch("FGBJMREC"), GetDocNumber(pData.referenceNumber));
                                                    if (reducingAmt > pData.remainder) intLink.AdjustPrepaymentRemainder(pData.remainder, pData.sequenceNumber);
                                                    else intLink.AdjustPrepaymentRemainder(reducingAmt, pData.sequenceNumber);
                                                    reducingAmt = reducingAmt - pData.remainder;
                                                }
                                                intLink.StorePayment(dt.customerId, companyName, DateTime.Now, pinfo.InvoiceID.ToString(), pinfo.Debit, 0, "No", 0, Convert.ToInt32(glid), "Yes", 1);
                                                Log.Save("Payment by credit transaction complete");
                                            }
                                        }
                                        else
                                        {
                                            Log.Save("Prepayment balance not enough to carry out Transaction");
                                        }
                                    }

                                    if (glid == "5147")
                                    {
                                        Log.Save("Bank: FGB US$ SAVINGS A/C");
                                        decimal usRate = intLink.GetUsRateByInvoice(pinfo.InvoiceID);
                                        decimal usAmount = Convert.ToDecimal(dt.debit) / usRate;
                                        if (pData.totalPrepaymentRemainder >= usAmount)
                                        {
                                            Log.Save("Carrying out Payment By Credit Transaction");
                                            decimal reducingAmt = usAmount;

                                            if (ReceiptBatchAvail("FGBUSMRC"))
                                            {
                                                while (reducingAmt > 0)
                                                {
                                                    pData = intLink.CheckPrepaymentAvail(dt.customerId);
                                                    ComApiPayByCredit(dt.customerId, pinfo.InvoiceID.ToString(), intLink.GetRecieptBatch("FGBUSMRC"), GetDocNumber(pData.referenceNumber));
                                                    if (reducingAmt > pData.remainder) intLink.AdjustPrepaymentRemainder(pData.remainder, pData.sequenceNumber);
                                                    else intLink.AdjustPrepaymentRemainder(reducingAmt, pData.sequenceNumber);
                                                    reducingAmt = reducingAmt - pData.remainder;
                                                }
                                                if (usRate == 1) intLink.StorePayment(dt.customerId, companyName, DateTime.Now, pinfo.InvoiceID.ToString(), pinfo.Debit, 0, "No", 0, Convert.ToInt32(glid), "Yes", 1);
                                                else intLink.StorePayment(dt.customerId, companyName, DateTime.Now, pinfo.InvoiceID.ToString(), pinfo.Debit, usAmount, "No", 0, Convert.ToInt32(glid), "Yes", 1);
                                                Log.Save("Payment by credit transaction complete");
                                            }
                                            else
                                            {
                                                CreateReceiptBatchEx("FGBUSMRC", "Middleware Generated Batch for FGBUSMRC");
                                                intLink.OpenNewReceiptBatch(1, GetLastPaymentBatch(), "FGBUSMRC");
                                                Log.Save("Target Batch: " + intLink.GetRecieptBatch("FGBUSMRC"));

                                                while (reducingAmt > 0)
                                                {
                                                    pData = intLink.CheckPrepaymentAvail(dt.customerId);
                                                    ComApiPayByCredit(dt.customerId, pinfo.InvoiceID.ToString(), intLink.GetRecieptBatch("FGBUSMRC"), GetDocNumber(pData.referenceNumber));
                                                    if (reducingAmt > pData.remainder) intLink.AdjustPrepaymentRemainder(pData.remainder, pData.sequenceNumber);
                                                    else intLink.AdjustPrepaymentRemainder(reducingAmt, pData.sequenceNumber);
                                                    reducingAmt = reducingAmt - pData.remainder;
                                                }

                                                if (usRate == 1)
                                                {
                                                    intLink.StorePayment(dt.customerId, companyName, DateTime.Now, pinfo.InvoiceID.ToString(), pinfo.Debit, 0, "No", 0, Convert.ToInt32(glid), "Yes", 1);
                                                }
                                                else
                                                {
                                                    intLink.StorePayment(dt.customerId, companyName, DateTime.Now, pinfo.InvoiceID.ToString(), pinfo.Debit, usAmount, "No", 0, Convert.ToInt32(glid), "Yes", 1);

                                                }
                                                Log.Save("Payment by credit transaction complete");
                                            }
                                        }
                                        else
                                        {
                                            Log.Save("Prepayment balance not enough to carry out Transaction");
                                        }
                                    }

                                    if (glid == "5148")
                                    {
                                        Log.Save("Bank: NCB JA$ SAVINGS A/C");
                                        if (pData.totalPrepaymentRemainder >= pinfo.Debit)
                                        {
                                            Log.Save("Carrying out Payment By Credit Transaction");
                                            decimal reducingAmt = pinfo.Debit;

                                            if (ReceiptBatchAvail("NCBJMREC"))
                                            {
                                                while (reducingAmt > 0)
                                                {
                                                    pData = intLink.CheckPrepaymentAvail(dt.customerId);
                                                    ComApiPayByCredit(dt.customerId, pinfo.InvoiceID.ToString(), intLink.GetRecieptBatch("NCBJMREC"), GetDocNumber(pData.referenceNumber));
                                                    if (reducingAmt > pData.remainder) intLink.AdjustPrepaymentRemainder(pData.remainder, pData.sequenceNumber);
                                                    else intLink.AdjustPrepaymentRemainder(reducingAmt, pData.sequenceNumber);
                                                    reducingAmt = reducingAmt - pData.remainder;
                                                }
                                                intLink.StorePayment(dt.customerId, companyName, DateTime.Now, pinfo.InvoiceID.ToString(), pinfo.Debit, 0, "No", 0, Convert.ToInt32(glid), "Yes", 1);
                                                Log.Save("Payment by credit transaction complete");
                                            }
                                            else
                                            {
                                                CreateReceiptBatchEx("NCBJMREC", "Middleware Generated Batch for NCBJMREC");
                                                intLink.OpenNewReceiptBatch(1, GetLastPaymentBatch(), "NCBJMREC");
                                                Log.Save("Target Batch: " + intLink.GetRecieptBatch("NCBJMREC"));
                                                while (reducingAmt > 0)
                                                {
                                                    pData = intLink.CheckPrepaymentAvail(dt.customerId);
                                                    ComApiPayByCredit(dt.customerId, pinfo.InvoiceID.ToString(), intLink.GetRecieptBatch("NCBJMREC"), GetDocNumber(pData.referenceNumber));
                                                    if (reducingAmt > pData.remainder) intLink.AdjustPrepaymentRemainder(pData.remainder, pData.sequenceNumber);
                                                    else intLink.AdjustPrepaymentRemainder(reducingAmt, pData.sequenceNumber);
                                                    reducingAmt = reducingAmt - pData.remainder;
                                                }
                                                intLink.StorePayment(dt.customerId, companyName, DateTime.Now, pinfo.InvoiceID.ToString(), pinfo.Debit, 0, "No", 0, Convert.ToInt32(glid), "Yes", 1);
                                                Log.Save("Payment by credit transaction complete");
                                            }
                                        }
                                        else
                                        {
                                            Log.Save("Prepayment balance not enough to carry out Transaction");
                                        }
                                    }

                                    if (glid != "5146" && glid != "5147" && glid != "5148")
                                    {
                                        Log.Save("Bank Selected Not found, Cannot complete Transaction");
                                    }
                                }
                                else
                                {
                                    Log.Save("Both invoice and receipt must be posted before attempting this transaction");
                                }
                            }
                            else
                            {
                                Log.Save("No prepayment record found for customer: " + dt.customerId);
                            }
                        }
                    }
                    Log.WriteEnd();
                }
                catch (Exception ex)
                {
                    if (accpacSession.Errors.Count > 0)
                    {
                        Log.Save(ex.Message + " " + ex.StackTrace);
                        for (int i = 0; i < accpacSession.Errors.Count; i++)
                        {
                            Log.Save(accpacSession.Errors[i].Message + ", Severity: " + accpacSession.Errors[i].Priority);
                        }
                        accpacSession.Errors.Clear();
                        Log.WriteEnd();
                    }
                    else
                    {
                        Log.Save(ex.Message + " " + ex.StackTrace);
                        Log.WriteEnd();
                    }
                }
            }
            else
            {
                Log.Save("Accpac Version: " + accpacSession.AppVersion);
                Log.Save("Company: " + accpacSession.CompanyName);
                Log.Save("Session Status: " + accpacSession.IsOpened);
            }
        }

        private void TableDependCancellation_OnChanged(object sender, RecordChangedEventArgs<SqlNotifyCancellation> e)
        {
            if (IsAccpacSessionOpen())
            {
                try
                {
                    var values = e.Entity;
                    var customerId = values.CustomerID;
                    var invoiceId = values.ARInvoiceID;
                    var amount = values.Amount;
                    var feeType = values.FeeType;
                    var notes = values.notes;
                    var cancelledBy = values.canceledBy;

                    if (cancelledBy != null)
                    {
                        string freqUsage = intLink.GetFreqUsage(invoiceId);
                        DateTime invoiceValidity = intLink.GetValidity(invoiceId);
                        var creditGl = intLink.GetCreditGl(invoiceId.ToString());
                        var accountNum = intLink.GetAccountNumber(creditGl);
                        List<string> clientInfo = new List<string>(4);
                        clientInfo = intLink.GetClientInfoInv(customerId.ToString());
                        intLink.GetClientInfoInv(customerId.ToString());

                        string companyName = clientInfo[0].ToString();
                        string cNum = clientInfo[1].ToString();
                        string fname = clientInfo[2].ToString();
                        string lname = clientInfo[3].ToString();

                        if (companyName == "" || companyName == " " || companyName == null)
                        {
                            companyName = fname + " " + lname;
                        }

                        string creditNoteDesc = companyName + " - Credit Note";
                        Data dt = Translate(cNum, feeType, companyName, "", notes, accountNum, freqUsage);

                        if (values.isVoided == 1 && e.ChangeType == ChangeType.Update)
                        {
                            Log.Save("Cancellation found.");
                            if (CheckAccpacInvoiceAvail(invoiceId))
                            {
                                int postedBatch = GetIbatchNumber(invoiceId);
                                if (!InvoiceDelete(invoiceId))
                                {
                                    Log.Save("Creating a credit memo");
                                    int cred_docNum = intLink.GetCreditMemoNumber();
                                    int batchNumber = GetBatch(CREDIT_NOTE, invoiceId.ToString());
                                    intLink.StoreInvoice(invoiceId, batchNumber, creditGl, companyName, dt.customerId, DateTime.Now, cancelledBy, amount, "no modification", 1, 0, 0, 1, cred_docNum);
                                    CreditNoteInsert(batchNumber.ToString(), dt.customerId, accountNum, amount.ToString(), invoiceId.ToString(), cred_docNum.ToString(), creditNoteDesc);
                                }
                                else
                                {
                                    Maj m = new Maj();
                                    int r = intLink.GetInvoiceReference(invoiceId);

                                    if (r != -1)
                                    {
                                        m = intLink.GetMajDetail(r);
                                    }

                                    if (dt.feeType == "SLF" && notes == "Renewal")
                                    {
                                        intLink.StoreInvoice(Convert.ToInt32(invoiceId), postedBatch, creditGl, companyName, dt.customerId, DateTime.Now, cancelledBy, Convert.ToDecimal(amount), "no modification", 1, 0, 1, 0, 0);
                                    }
                                    else if (dt.feeType == "RF" && notes == "Renewal")
                                    {
                                        intLink.StoreInvoice(Convert.ToInt32(invoiceId), postedBatch, creditGl, companyName, dt.customerId, DateTime.Now, cancelledBy, Convert.ToDecimal(amount), "no modification", 1, 0, 1, 0, 0);
                                    }
                                    else if ((notes == "Annual Fee" && m.stationType == "SSL" && m.certificateType == 0 && m.proj == "JMC") || (freqUsage == "PRS55"))
                                    {
                                        intLink.StoreInvoice(Convert.ToInt32(invoiceId), postedBatch, creditGl, companyName, dt.customerId, DateTime.Now, cancelledBy, Convert.ToDecimal(amount), "no modification", 1, 0, 1, 0, 0);
                                    }
                                    else if (notes == "Type Approval" || freqUsage == "TA-ProAmend")
                                    {
                                        intLink.StoreInvoice(Convert.ToInt32(invoiceId), postedBatch, creditGl, companyName, dt.customerId, DateTime.Now, cancelledBy, Convert.ToDecimal(amount), "no modification", intLink.GetRate(), ChangeToUS(Convert.ToDecimal(amount)), 1, 0, 0);
                                    }
                                    else if (notes == "Annual Fee" || notes == "Modification" || notes == "Radio Operator")
                                    {
                                        intLink.StoreInvoice(Convert.ToInt32(invoiceId), postedBatch, creditGl, companyName, dt.customerId, DateTime.Now, cancelledBy, Convert.ToDecimal(amount), "no modification", 1, 0, 1, 0, 0);
                                    }
                                }
                            }
                            else
                            {
                                Log.Save("Invoice: " + invoiceId.ToString() + " was not found in Sage. Cannot delete.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (accpacSession.Errors.Count > 0)
                    {
                        Log.Save(ex.Message + " " + ex.StackTrace);
                        for (int i = 0; i < accpacSession.Errors.Count; i++)
                        {
                            Log.Save(accpacSession.Errors[i].Message + ", Severity: " + accpacSession.Errors[i].Priority);
                        }
                        accpacSession.Errors.Clear();
                        Log.WriteEnd();
                    }
                    else
                    {
                        Log.Save(ex.Message + " " + ex.StackTrace);
                        Log.WriteEnd();
                    }
                }
            }
            else
            {
                Log.Save("Accpac Version: " + accpacSession.AppVersion);
                Log.Save("Company: " + accpacSession.CompanyName);
                Log.Save("Session Status: " + accpacSession.IsOpened);
            }
        }

        InsertionReturn InvBatchInsert(string idCust, string docNum, string desc, string feeCode, string amt, string batchId)
        {
            Log.Save("Transfering Invoice " + docNum + " to batch: " + batchId);
            InsertionReturn success = new InsertionReturn();
            DateTime postDate = intLink.GetValidity(Convert.ToInt32(docNum));

            if (postDate < DateTime.Now) postDate = DateTime.Now;

            DateTime docDate = intLink.GetDocDate(Convert.ToInt32(docNum));
            DateTime now = DateTime.Now;
            bool gotOne;

            b1_arInvoiceBatch = dbLink.OpenView("AR0031");
            b1_arInvoiceHeader = dbLink.OpenView("AR0032");
            b1_arInvoiceDetail = dbLink.OpenView("AR0033");
            b1_arInvoicePaymentSchedules = dbLink.OpenView("AR0034");
            b1_arInvoiceHeaderOptFields = dbLink.OpenView("AR0402");
            b1_arInvoiceDetailOptFields = dbLink.OpenView("AR0401");


            b1_arInvoiceBatch.Compose(new View[] { b1_arInvoiceHeader });
            b1_arInvoiceHeader.Compose(new View[] { b1_arInvoiceBatch, b1_arInvoiceDetail, b1_arInvoicePaymentSchedules, b1_arInvoiceHeaderOptFields });
            b1_arInvoiceDetail.Compose(new View[] { b1_arInvoiceHeader, b1_arInvoiceBatch, b1_arInvoiceDetailOptFields });
            b1_arInvoicePaymentSchedules.Compose(new View[] { b1_arInvoiceHeader });
            b1_arInvoiceHeaderOptFields.Compose(new View[] { b1_arInvoiceHeader });
            b1_arInvoiceDetailOptFields.Compose(new View[] { b1_arInvoiceDetail });

            gotOne = CustomerExists(idCust);


            if (gotOne)
            {
                b1_arInvoiceBatch.Fields.FieldByName("CNTBTCH").SetValue(batchId, false);
                b1_arInvoiceBatch.Read(false);
                b1_arInvoiceHeader.RecordCreate(ViewRecordCreate.DelayKey);
                b1_arInvoiceDetail.Cancel();
                b1_arInvoiceHeader.Fields.FieldByName("DATEBUS").SetValue(postDate.ToString(), false);
                b1_arInvoiceHeader.Fields.FieldByName("IDCUST").SetValue(idCust, false);

                b1_arInvoiceHeader.Fields.FieldByName("IDINVC").SetValue(docNum, false);

                var temp = b1_arInvoiceDetail.Exists;
                b1_arInvoiceDetail.RecordClear();
                temp = b1_arInvoiceDetail.Exists;
                b1_arInvoiceDetail.RecordCreate(ViewRecordCreate.NoInsert);

                b1_arInvoiceDetail.Fields.FieldByName("TEXTDESC").SetValue(desc, false);
                b1_arInvoiceDetail.Fields.FieldByName("IDACCTREV").SetValue(feeCode, false);
                b1_arInvoiceDetail.Fields.FieldByName("AMTEXTN").SetValue(amt, false);
                b1_arInvoiceDetail.Insert();

                b1_arInvoiceDetail.Read(false);
                b1_arInvoiceHeader.Insert();
                b1_arInvoiceDetail.Read(false);
                b1_arInvoiceDetail.Read(false);
                b1_arInvoiceBatch.Read(false);
                b1_arInvoiceHeader.RecordCreate(ViewRecordCreate.DelayKey);
                b1_arInvoiceDetail.Cancel();

                b1_arInvoiceBatch.Dispose();
                b1_arInvoiceDetail.Dispose();
                b1_arInvoiceDetailOptFields.Dispose();
                b1_arInvoiceHeader.Dispose();
                b1_arInvoiceHeaderOptFields.Dispose();
                b1_arInvoicePaymentSchedules.Dispose();
                Log.Save("Invoice Id: " + docNum + " Transferred");
            }

            else
            {
                success.status = "Not Exist";
                Log.Save("Customer does not exist.");
            }
            return success;
        }

        bool CustomerExists(string idCust)
        {
            bool exist = false;
            View cssql = dbLink.OpenView("CS0120");

            cssql.Browse("SELECT IDCUST FROM ARCUS WHERE IDCUST = '" + idCust + "'", true);
            cssql.InternalSet(256);

            if (cssql.GoNext())
            {
                exist = true;
            }

            return exist;
        }

        Data Translate(string cNum, string feeType, string companyName, string debit, string notes, string _fcode, string FreqUsage)
        {
            string temp = "";
            string iv_customerId = "";
            Data dt = new Data();

            if (_fcode == "PREPAYMENT" && intLink.GetClientIdZRecord(false).Contains("-T"))
            {
                dt.customerId = intLink.GetClientIdZRecord(false);
                dt.companyName = "Processing Fee for Type Approval Certification";
                dt.desc = "Processing Fee";
                dt.debit = debit;
                dt.success = true;
                return dt;
            }
            else
            {
                if (_fcode == "PAYMENT")
                {
                    dt.fcode = "";
                }
                else
                {
                    dt.fcode = _fcode;
                }

                if (debit != "0.0000" && debit != "")
                {
                    dt.debit = debit;

                    for (int i = 0; i < cNum.Length; i++)
                    {
                        if (cNum[i] != '-')
                        {
                            temp += cNum[i];
                        }
                        else
                        {
                            i = cNum.Length;
                            cNum = temp;
                        }
                    }

                    if (feeType == "SLF")
                    {
                        iv_customerId = cNum + "-L";
                        dt.customerId = iv_customerId;
                        dt.feeType = "SLF";
                        dt.companyName = companyName + " - Spec Fee";
                        dt.desc = "Spec Fee";
                    }

                    else if (notes == "Radio Operator")
                    {
                        iv_customerId = cNum + "-L";
                        dt.customerId = iv_customerId;
                        dt.feeType = "SLF";
                        dt.companyName = companyName + " - Spec Fee";
                        dt.desc = "Spec Fee";
                    }

                    else if (notes == "Type Approval" || FreqUsage == "TA-ProAmend")
                    {
                        iv_customerId = cNum + "-T";
                        dt.customerId = iv_customerId;
                        dt.feeType = "RF";
                        dt.companyName = companyName + " - Type Approval";
                        dt.desc = "Processing Fee";
                    }

                    else if (notes != "Type Approval" && feeType == "APF")
                    {
                        iv_customerId = cNum + "-R";
                        dt.customerId = iv_customerId;
                        dt.feeType = "RF";
                        dt.companyName = companyName + " - Processing Fee";
                        dt.desc = "Processing Fee";
                    }
                    else
                    {
                        iv_customerId = cNum + "-R";
                        dt.customerId = iv_customerId;
                        dt.companyName = companyName + " - Reg Fee";
                        dt.feeType = "RF";
                        dt.desc = "Reg Fee";
                    }

                    dt.success = true;
                    return dt;
                }
                else if (debit == "")
                {
                    if (companyName == "")
                    {
                        companyName = " ";
                        dt.companyName = companyName;
                    }
                    else
                    {
                        dt.companyName = companyName;
                    }

                    for (int i = 0; i < cNum.Length; i++)
                    {
                        if (cNum[i] != '-')
                        {
                            temp += cNum[i];
                        }
                        else
                        {
                            i = cNum.Length;
                            cNum = temp;
                        }
                    }

                    if (feeType == "SLF")
                    {
                        iv_customerId = cNum + "-L";
                        dt.customerId = iv_customerId;
                        dt.feeType = "SLF";
                        dt.companyName = companyName + " - Spec Fee";
                        dt.companyName_NewCust = companyName;
                        dt.desc = "Spec Fee";
                    }

                    else if (feeType == "Reg")
                    {
                        iv_customerId = cNum + "-R";
                        dt.customerId = iv_customerId;
                        dt.feeType = "RF";
                        dt.companyName = companyName + " - Reg Fee";
                        dt.companyName_NewCust = companyName;
                        dt.desc = "Reg Fee";
                    }
                    else if (notes == "Radio Operator")
                    {
                        iv_customerId = cNum + "-L";
                        dt.customerId = iv_customerId;
                        dt.feeType = "SLF";
                        dt.companyName = companyName + " - Spec Fee";
                        dt.companyName_NewCust = companyName;
                        dt.desc = "Spec Fee";
                    }

                    else if (notes == "Type Approval" || FreqUsage == "TA-ProAmend")
                    {
                        iv_customerId = cNum + "-T";
                        dt.customerId = iv_customerId;
                        dt.feeType = "RF";
                        dt.companyName = companyName + " - Type Approval";
                        dt.companyName_NewCust = companyName;
                        dt.desc = "Processing Fee";
                    }

                    else if (notes != "Type Approval" && feeType == "APF")
                    {
                        iv_customerId = cNum + "-R";
                        dt.customerId = iv_customerId;
                        dt.feeType = "RF";
                        dt.companyName = companyName + " - Processing Fee";
                        dt.companyName_NewCust = companyName;
                        dt.desc = "Processing Fee";
                    }
                    else
                    {
                        iv_customerId = cNum + "-R";
                        dt.customerId = iv_customerId;
                        dt.feeType = "RF";
                        dt.companyName = companyName + " - Reg Fee";
                        dt.companyName_NewCust = companyName;
                        dt.desc = "Reg Fee";
                    }

                    dt.success = true;
                    return dt;
                }
                else
                {
                    dt.success = false;
                    return dt;
                }
            }
        }

        void CreateCustomer(string idCust, string nameCust)
        {
            Log.Save("Creating customer: " + nameCust + ", ID: " + idCust);
            string groupCode = "";
            View ARCUSTOMER1header = dbLink.OpenView("AR0024"); ;
            View ARCUSTOMER1detail = dbLink.OpenView("AR0400"); ;
            View ARCUSTSTAT2 = dbLink.OpenView("AR0022"); ;
            View ARCUSTCMT3 = dbLink.OpenView("AR0021"); ;

            ARCUSTOMER1header.Compose(new View[] { ARCUSTOMER1detail });
            ARCUSTOMER1detail.Compose(new View[] { ARCUSTOMER1header });

            if (idCust[5].ToString() + idCust[6].ToString() == "-L")
            {
                groupCode = "LICCOM";
            }
            else if (idCust[5].ToString() + idCust[6].ToString() == "-R")
            {
                groupCode = "REGCOM";
            }

            else if (idCust[5].ToString() + idCust[6].ToString() == "-T")
            {
                groupCode = "TYPEUS";
            }

            ARCUSTOMER1header.Fields.FieldByName("IDCUST").SetValue(idCust, false);
            ARCUSTOMER1header.Process();
            ARCUSTOMER1header.Fields.FieldByName("NAMECUST").SetValue(nameCust, false);
            ARCUSTOMER1header.Fields.FieldByName("IDGRP").SetValue(groupCode, false);
            ARCUSTOMER1header.Process();
            ARCUSTOMER1header.Fields.FieldByName("CODETAXGRP").SetValue("JATAX", false);
            ARCUSTOMER1header.Insert();
            intLink.UpdateCustomerCount();
            intLink.StoreCustomer(idCust, nameCust);
            Log.Save("Customer created.");
        }

        public string CreateBatchDesc(string batchType)
        {
            string result = "";
            switch (batchType)
            {
                case TYPE_APPROVAL:
                    result = "New Applications - Type Approvals - " + DateTime.Now.ToString("dd/MM/yyyy");
                    break;
                case MAJ:
                    result = "New Applications - MAJ Licences - " + DateTime.Now.ToString("MMMM") + " " + DateTime.Now.Year.ToString();
                    break;
                case NON_MAJ:
                    result = "New Applications - Non-MAJ Licences - " + DateTime.Now.ToString("dd/MM/yyyy");
                    break;
                case CREDIT_NOTE:
                    result = "Credit Notes for " + DateTime.Now.ToString("MMMM") + " " + DateTime.Now.Year.ToString();
                    break;
            }
            return result;
        }

        public int GetBatch(string batchType, string invoiceid)
        {
            DateTime val = intLink.GetValidity(Convert.ToInt32(invoiceid));
            string renspec = RENEWAL_SPEC + val.ToString("MMMM") + " " + val.Year.ToString();
            string renreg = RENEWAL_REG + val.ToString("MMMM") + " " + val.Year.ToString();

            if (intLink.BatchAvail(batchType))
            {
                int batch = intLink.GetAvailBatch(batchType);
                if (!intLink.IsBatchExpired(batch))
                {
                    if (!CheckAccpacIBatchPosted(batch))
                    {
                        return batch;
                    }
                    else
                    {
                        intLink.CloseInvoiceBatch();

                        int newbatch = GetLastInvoiceBatch() + 1;

                        if (batchType == renreg)
                        {
                            intLink.CreateInvoiceBatch(GenerateDaysExpire(batchType), newbatch, batchType, "Regulatory");
                        }

                        else if (batchType == renspec)
                        {
                            intLink.CreateInvoiceBatch(GenerateDaysExpire(batchType), newbatch, batchType, "Spectrum");
                        }

                        else
                        {
                            intLink.CreateInvoiceBatch(GenerateDaysExpire(batchType), newbatch, batchType, "");
                        }


                        if (batchType == renreg || batchType == renspec)
                        {
                            CreateInvoiceBatch(batchType);
                        }
                        else
                            CreateInvoiceBatch(CreateBatchDesc(batchType));
                        return newbatch;
                    }
                }
                else
                {
                    if (batchType == "" || batchType == "")
                    {
                        intLink.ResetInvoiceTotal();
                    }

                    intLink.CloseInvoiceBatch();
                    int newbatch = GetLastInvoiceBatch() + 1;

                    if (batchType == renreg)
                    {
                        intLink.CreateInvoiceBatch(GenerateDaysExpire(batchType), newbatch, batchType, "Regulatory");
                    }
                    else if (batchType == renspec)
                    {
                        intLink.CreateInvoiceBatch(GenerateDaysExpire(batchType), newbatch, batchType, "Spectrum");
                    }
                    else
                    {
                        intLink.CreateInvoiceBatch(GenerateDaysExpire(batchType), newbatch, batchType, "");
                    }

                    if (batchType == renreg || batchType == renspec)
                    {
                        CreateInvoiceBatch(batchType);
                    }
                    else
                    {
                        CreateInvoiceBatch(CreateBatchDesc(batchType));
                    }

                    return newbatch;
                }
            }
            else
            {
                int newbatch = GetLastInvoiceBatch() + 1;
                if (batchType == renreg)
                {
                    intLink.CreateInvoiceBatch(GenerateDaysExpire(batchType), newbatch, batchType, "Regulatory");
                }

                else if (batchType == renspec)
                {
                    intLink.CreateInvoiceBatch(GenerateDaysExpire(batchType), newbatch, batchType, "Spectrum");
                }

                else
                {
                    intLink.CreateInvoiceBatch(GenerateDaysExpire(batchType), newbatch, batchType, "");
                }


                if (batchType == renreg || batchType == renspec)
                {
                    CreateInvoiceBatch(batchType);
                }
                else
                {
                    CreateInvoiceBatch(CreateBatchDesc(batchType));
                }
                return newbatch;
            }
        }

        public bool IsAccpacSessionOpen()
        {
            if (accpacSession != null)
            {
                if (accpacSession.IsOpened)
                {
                    return true;
                }
                else
                {
                    Log.Save("Accpac session is not opened");
                    return false;
                }
            }
            else
            {
                Log.Save("Accpac session is not initialized");
                return false;
            }
        }

        public bool CheckAccpacIBatchPosted(int batchNumber)
        {
            View cssql = dbLink.OpenView("CS0120");


            cssql.Browse("SELECT BTCHSTTS FROM ARIBC WHERE CNTBTCH = '" + batchNumber.ToString() + "'", true);
            cssql.InternalSet(256);

            if (cssql.GoNext())
            {
                string val = Convert.ToString(cssql.Fields.FieldByName("BTCHSTTS").Value);

                if (val == "1")
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return true;
        }

        int GetLastInvoiceBatch()
        {
            int BatchId = 0;
            b1_arInvoiceBatch = dbLink.OpenView("AR0031");
            b1_arInvoiceBatch.GoBottom();
            BatchId = Convert.ToInt32(b1_arInvoiceBatch.Fields.FieldByName("CNTBTCH").Value);

            b1_arInvoiceBatch.Dispose();
            return BatchId;
        }

        void CreateInvoiceBatch(string description)
        {
            b1_arInvoiceBatch = dbLink.OpenView("AR0031");
            b1_arInvoiceHeader = dbLink.OpenView("AR0032");
            b1_arInvoiceDetail = dbLink.OpenView("AR0033");
            b1_arInvoicePaymentSchedules = dbLink.OpenView("AR0034");
            b1_arInvoiceHeaderOptFields = dbLink.OpenView("AR0402");
            b1_arInvoiceDetailOptFields = dbLink.OpenView("AR0401");

            b1_arInvoiceBatch.Compose(new View[] { b1_arInvoiceHeader });
            b1_arInvoiceHeader.Compose(new View[] { b1_arInvoiceBatch, b1_arInvoiceDetail, b1_arInvoicePaymentSchedules, b1_arInvoiceHeaderOptFields });
            b1_arInvoiceDetail.Compose(new View[] { b1_arInvoiceHeader, b1_arInvoiceBatch, b1_arInvoiceDetailOptFields });
            b1_arInvoicePaymentSchedules.Compose(new View[] { b1_arInvoiceHeader });
            b1_arInvoiceHeaderOptFields.Compose(new View[] { b1_arInvoiceHeader });
            b1_arInvoiceDetailOptFields.Compose(new View[] { b1_arInvoiceDetail });

            b1_arInvoiceBatch.RecordCreate(ViewRecordCreate.Insert);
            b1_arInvoiceBatch.Read(false);

            b1_arInvoiceHeader.RecordCreate(ViewRecordCreate.DelayKey);
            b1_arInvoiceDetail.Cancel();
            b1_arInvoiceBatch.Fields.FieldByName("BTCHDESC").SetValue(description, false);
            b1_arInvoiceBatch.Fields.FieldByName("DATEBTCH").SetValue(DateTime.Now.Date.ToString(), false);
            b1_arInvoiceBatch.Update();

            b1_arInvoiceBatch.Dispose();
            b1_arInvoiceDetail.Dispose();
            b1_arInvoiceDetailOptFields.Dispose();
            b1_arInvoiceHeader.Dispose();
            b1_arInvoiceHeaderOptFields.Dispose();
            b1_arInvoicePaymentSchedules.Dispose();
        }

        public int GenerateDaysExpire(string batchType)
        {
            if (batchType != NON_MAJ && batchType != TYPE_APPROVAL && batchType != ONE_DAY)
            {
                int expiry = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) - DateTime.Now.Day;
                expiry++;

                return expiry;
            }
            else return 1;
        }

        public bool IsPeriodCreated(int finyear)
        {
            string fiscYear = "";
            View cssql = dbLink.OpenView("CS0120");

            cssql.Browse("SELECT FSCYEAR FROM CSFSC WHERE FSCYEAR = '" + finyear.ToString() + "'", true);
            cssql.InternalSet(256);

            if (cssql.GoNext())
            {
                fiscYear = Convert.ToString(cssql.Fields.FieldByName("FSCYEAR").Value);
            }

            if (fiscYear != "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public decimal ChangeToUS(decimal regamt)
        {
            decimal rate = intLink.GetRate();
            decimal usamt = regamt / rate;
            return Math.Round(usamt, 2);
        }

        public decimal ChangeToUSupdated(decimal regamt, int invnum)
        {
            decimal rate = intLink.GetUsRateByInvoice(invnum);
            decimal usamt = regamt / rate;
            return Math.Round(usamt, 2);
        }

        public int GetIbatchNumber(int docNumber)
        {
            int batchNum = -1;
            View cssql = dbLink.OpenView("CS0120"); ;

            cssql.Browse("SELECT CNTBTCH FROM ARIBH WHERE IDINVC = '" + docNumber.ToString() + "'", true);
            cssql.InternalSet(256);

            if (cssql.GoNext())
            {
                batchNum = Convert.ToInt32(cssql.Fields.FieldByName("CNTBTCH").Value);
            }
            return batchNum;
        }

        void UpdateInvoice(string accountNumber, string amt, string BatchId, string entryNumber)
        {
            b1_arInvoiceBatch = dbLink.OpenView("AR0031");
            b1_arInvoiceHeader = dbLink.OpenView("AR0032");
            b1_arInvoiceDetail = dbLink.OpenView("AR0033");
            b1_arInvoicePaymentSchedules = dbLink.OpenView("AR0034");
            b1_arInvoiceHeaderOptFields = dbLink.OpenView("AR0402");
            b1_arInvoiceDetailOptFields = dbLink.OpenView("AR0401");

            b1_arInvoiceBatch.Compose(new View[] { b1_arInvoiceHeader });
            b1_arInvoiceHeader.Compose(new View[] { b1_arInvoiceBatch, b1_arInvoiceDetail, b1_arInvoicePaymentSchedules, b1_arInvoiceHeaderOptFields });
            b1_arInvoiceDetail.Compose(new View[] { b1_arInvoiceHeader, b1_arInvoiceBatch, b1_arInvoiceDetailOptFields });
            b1_arInvoicePaymentSchedules.Compose(new View[] { b1_arInvoiceHeader });
            b1_arInvoiceHeaderOptFields.Compose(new View[] { b1_arInvoiceHeader });
            b1_arInvoiceDetailOptFields.Compose(new View[] { b1_arInvoiceDetail });

            b1_arInvoiceBatch.Fields.FieldByName("CNTBTCH").SetValue(BatchId, false);
            b1_arInvoiceBatch.Browse("((BTCHSTTS = 1) OR (BTCHSTTS = 7))", false);
            b1_arInvoiceBatch.Fetch(false);

            b1_arInvoiceBatch.Process();
            b1_arInvoiceHeader.Fields.FieldByName("CNTITEM").SetValue(entryNumber, false);
            b1_arInvoiceHeader.Browse("", true);

            b1_arInvoiceHeader.Fetch(false);
            b1_arInvoiceDetail.Read(false);
            b1_arInvoiceDetail.Read(false);
            b1_arInvoiceDetail.Fields.FieldByName("CNTLINE").SetValue("20", false);

            b1_arInvoiceDetail.Read(false);
            b1_arInvoiceDetail.Fields.FieldByName("IDACCTREV").SetValue(accountNumber, false);
            b1_arInvoiceDetail.Fields.FieldByName("AMTEXTN").SetValue(amt, false);
            b1_arInvoiceDetail.Update();
            b1_arInvoiceDetail.Fields.FieldByName("CNTLINE").SetValue("20", false);

            b1_arInvoiceDetail.Read(false);
            b1_arInvoiceHeader.Update();
            b1_arInvoiceDetail.Read(false);
            b1_arInvoiceDetail.Read(false);
        }

        public int GetEntryNumber(int docNumber)
        {
            int entry = -1;
            string docNum = docNumber.ToString();
            b1_arInvoiceHeader = dbLink.OpenView("AR0032");

            string searchFilter = "IDINVC = " + docNum + "";
            b1_arInvoiceHeader.Browse(searchFilter, true);

            bool gotIt = b1_arInvoiceHeader.GoBottom();

            if (gotIt)
            {
                entry = Convert.ToInt32(b1_arInvoiceHeader.Fields.FieldByName("CNTITEM").Value);
            }
            else
            {
                Log.Save("Invoice number was not found: " + docNum + ", while getting entry number");
            }
            return entry;
        }

        bool IsEmpty(DataSet dataSet)
        {
            foreach (DataTable table in dataSet.Tables)
                if (table.Rows.Count != 0) return false;

            return true;
        }

        public bool ReceiptBatchAvail(string bankcode)
        {
            connIntegration = new SqlConnection(Constants.DBINTEGRATION);
            connIntegration.Open();

            try
            {
                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;
                bool truth = false;
                int receiptBatch = -1;
                DateTime expiryDate = DateTime.Now;

                cmd.CommandText = "exec sp_rBatchAvail @bankcode";
                cmd.Parameters.AddWithValue("@bankcode", bankcode);
                cmd.Connection = connIntegration;

                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    receiptBatch = Convert.ToInt32(reader[0]);
                    expiryDate = Convert.ToDateTime(reader["ExpiryDate"]);
                    connIntegration.Close();

                    if (DateTime.Now < expiryDate)
                    {
                        if (!CheckAccpacRBatchPosted(receiptBatch))
                        {
                            truth = true;
                        }
                        else
                        {
                            intLink.CloseReceiptBatch(receiptBatch);
                        }
                    }
                    else
                    {
                        intLink.CloseReceiptBatch(receiptBatch);
                    }

                    return truth;
                }
                else
                {
                    connIntegration.Close();
                    return truth;
                }
            }
            catch (Exception e)
            {
                Log.Save(e.Message + " " + e.StackTrace);
                connIntegration.Close();
                return false;
            }
        }

        public bool CheckAccpacRBatchPosted(int batchNumber)
        {
            View cssql = dbLink.OpenView("CS0120"); ;

            cssql.Browse("SELECT CNTBTCH, BATCHSTAT FROM ARBTA WHERE CNTBTCH = '" + batchNumber.ToString() + "'", true);
            cssql.InternalSet(256);

            if (cssql.GoNext())
            {
                string val = Convert.ToString(cssql.Fields.FieldByName("BATCHSTAT").Value);

                if (val == "1")
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return true;
        }

        public bool ReceiptTransfer(string batchNumber, string customerId, string amount, string receiptDescription, string referenceNumber, string invnum, DateTime paymentDate, string findesc, string cid, DateTime valstart, DateTime valend)
        {
            string notes = intLink.IsAnnualFee(Convert.ToInt32(invnum));
            string receiptDescriptionEx = "";

            if (notes == "Annual Fee")
            {
                receiptDescriptionEx = findesc + " for Licence " + cid;
            }
            else if (findesc == "Processing Fee" && customerId[6].ToString() == "T")
            {
                receiptDescriptionEx = findesc + " for Type Approval Certification";
            }
            else if (invnum == "0")
            {
                receiptDescriptionEx = findesc + " for Lic# " + cid + " for Period " + valstart.Date.ToString("dd/MM/yy") + " to " + valend.Date.ToString("dd/MM/yy");
            }
            else
            {
                receiptDescriptionEx = findesc + " for Lic# " + cid + " for Period " + valstart.Date.ToString("dd/MM/yy") + " to " + valend.Date.ToString("dd/MM/yy");
            }

            if (!CustomerExists(customerId))
            {
                return false;
            }
            else
            {
                if (invnum == "0")
                {
                    CBBTCH1batch = dbLink.OpenView("AR0041");
                    CBBTCH1header = dbLink.OpenView("AR0042");
                    CBBTCH1detail1 = dbLink.OpenView("AR0044");
                    CBBTCH1detail2 = dbLink.OpenView("AR0045");
                    CBBTCH1detail3 = dbLink.OpenView("AR0043");
                    CBBTCH1detail4 = dbLink.OpenView("AR0061");
                    CBBTCH1detail5 = dbLink.OpenView("AR0406");
                    CBBTCH1detail6 = dbLink.OpenView("AR0170");

                    CBBTCH1batch.Compose(new View[] { CBBTCH1header });
                    CBBTCH1header.Compose(new View[] { CBBTCH1batch, CBBTCH1detail3, CBBTCH1detail1, CBBTCH1detail5, CBBTCH1detail6 });
                    CBBTCH1detail1.Compose(new View[] { CBBTCH1header, CBBTCH1detail2, CBBTCH1detail4 });
                    CBBTCH1detail2.Compose(new View[] { CBBTCH1detail1 });
                    CBBTCH1detail3.Compose(new View[] { CBBTCH1header });
                    CBBTCH1detail4.Compose(new View[] { CBBTCH1batch, CBBTCH1header, CBBTCH1header, CBBTCH1detail3, CBBTCH1detail1, CBBTCH1detail2 });
                    CBBTCH1detail5.Compose(new View[] { CBBTCH1header });
                    CBBTCH1detail6.Compose(new View[] { CBBTCH1header });

                    CBBTCH1batch.RecordClear();
                    CBBTCH1batch.Fields.FieldByName("CODEPYMTYP").SetValue("CA", false);
                    CBBTCH1header.Fields.FieldByName("CODEPYMTYP").SetValue("CA", false);
                    CBBTCH1detail3.Fields.FieldByName("CODEPAYM").SetValue("CA", false);
                    CBBTCH1detail1.Fields.FieldByName("CODEPAYM").SetValue("CA", false);
                    CBBTCH1detail2.Fields.FieldByName("CODEPAYM").SetValue("CA", false);
                    CBBTCH1detail4.Fields.FieldByName("PAYMTYPE").SetValue("CA", false);
                    CBBTCH1batch.Fields.FieldByName("CNTBTCH").SetValue(batchNumber, false);
                    CBBTCH1batch.Read(false);

                    CBBTCH1header.RecordCreate(ViewRecordCreate.DelayKey);
                    CBBTCH1header.Fields.FieldByName("RMITTYPE").SetValue("2", false);
                    CBBTCH1detail1.RecordCreate(ViewRecordCreate.NoInsert);
                    CBBTCH1header.Fields.FieldByName("IDCUST").SetValue(customerId, false);

                    CBBTCH1detail1.RecordCreate(ViewRecordCreate.NoInsert);
                    CBBTCH1header.Fields.FieldByName("TEXTRMIT").SetValue(receiptDescription, false);
                    CBBTCH1header.Fields.FieldByName("CODEPAYM").SetValue("CASH", false);
                    CBBTCH1header.Fields.FieldByName("IDRMIT").SetValue(referenceNumber, false);
                    CBBTCH1header.Fields.FieldByName("AMTRMIT").SetValue(amount, false);
                    CBBTCH1header.Fields.FieldByName("DATERMIT").SetValue(paymentDate, false);
                    CBBTCH1detail1.Fields.FieldByName("AMTPAYM").SetValue(amount, false);

                    CBBTCH1detail1.Insert();
                    CBBTCH1header.Insert();
                    CBBTCH1header.RecordCreate(ViewRecordCreate.DelayKey);

                    Log.Save("Prepayment Transferred");
                    return true;
                }
                else
                {
                    receiptDescription = receiptDescriptionEx;
                    int batchNum = GetIbatchNumber(Convert.ToInt32(invnum));
                    bool shouldAllocate = false;

                    if (CheckAccpacInvoiceAvail(Convert.ToInt32(invnum)))
                    {
                        if (CheckAccpacIBatchPosted(batchNum))
                        {
                            shouldAllocate = true;
                        }
                    }

                    bool flagInsert = false;

                    arRecptBatch = dbLink.OpenView("AR0041");
                    arRecptHeader = dbLink.OpenView("AR0042");
                    arRecptDetail1 = dbLink.OpenView("AR0044");
                    arRecptDetail2 = dbLink.OpenView("AR0045");
                    arRecptDetail3 = dbLink.OpenView("AR0043");
                    arRecptDetail4 = dbLink.OpenView("AR0061");
                    arRecptDetail5 = dbLink.OpenView("AR0406");
                    arRecptDetail6 = dbLink.OpenView("AR0170");

                    arRecptBatch.Compose(new View[] { arRecptHeader });
                    arRecptHeader.Compose(new View[] { arRecptBatch, arRecptDetail3, arRecptDetail1, arRecptDetail5, arRecptDetail6 });
                    arRecptDetail1.Compose(new View[] { arRecptHeader, arRecptDetail2, arRecptDetail4 });
                    arRecptDetail2.Compose(new View[] { arRecptDetail1 });
                    arRecptDetail3.Compose(new View[] { arRecptHeader });
                    arRecptDetail4.Compose(new View[] { arRecptBatch, arRecptHeader, arRecptDetail3, arRecptDetail1, arRecptDetail2 });
                    arRecptDetail5.Compose(new View[] { arRecptHeader });
                    arRecptDetail6.Compose(new View[] { arRecptHeader });

                    arRecptBatch.RecordClear();

                    arRecptBatch.Fields.FieldByName("CODEPYMTYP").SetValue("CA", false);
                    arRecptHeader.Fields.FieldByName("CODEPYMTYP").SetValue("CA", false);
                    arRecptDetail3.Fields.FieldByName("CODEPAYM").SetValue("CA", false);
                    arRecptDetail1.Fields.FieldByName("CODEPAYM").SetValue("CA", false);
                    arRecptDetail2.Fields.FieldByName("CODEPAYM").SetValue("CA", false);
                    arRecptDetail4.Fields.FieldByName("PAYMTYPE").SetValue("CA", false);
                    arRecptBatch.Fields.FieldByName("CNTBTCH").SetValue(batchNumber, false);
                    arRecptBatch.Read(false);

                    arRecptDetail4.Fields.FieldByName("PAYMTYPE").SetValue("CA", false);
                    arRecptDetail4.Fields.FieldByName("CNTBTCH").SetValue(batchNumber, false);
                    arRecptDetail4.Fields.FieldByName("CNTITEM").SetValue("1", false);
                    arRecptDetail4.Fields.FieldByName("IDCUST").SetValue(customerId, false);
                    arRecptDetail4.Fields.FieldByName("AMTRMIT").SetValue(amount, false);
                    arRecptDetail4.Fields.FieldByName("STDOCDTE").SetValue(paymentDate.ToString(), false);

                    arRecptHeader.RecordCreate(ViewRecordCreate.DelayKey);

                    arRecptHeader.Fields.FieldByName("TEXTRMIT").SetValue(receiptDescription, false);
                    arRecptHeader.Fields.FieldByName("IDCUST").SetValue(customerId, false);
                    arRecptHeader.Fields.FieldByName("CODEPAYM").SetValue("CASH", false);
                    arRecptHeader.Fields.FieldByName("DATEBUS").SetValue(paymentDate.ToString(), false);
                    arRecptHeader.Fields.FieldByName("IDRMIT").SetValue(referenceNumber, false);
                    arRecptHeader.Fields.FieldByName("AMTRMIT").SetValue(amount, false);

                    if (shouldAllocate)
                    {
                        Log.Save("Applying receipt to invoice: " + invnum);
                        arRecptDetail4.Fields.FieldByName("SHOWTYPE").SetValue("2", false);
                        arRecptDetail4.Fields.FieldByName("STDOCSTR").SetValue(invnum, false);

                        arRecptDetail4.Process();
                        arRecptDetail4.Fields.FieldByName("CNTKEY").SetValue("-1", false);
                        arRecptDetail4.Read(false);

                        var netAmount = Convert.ToInt32(arRecptDetail4.Fields.FieldByName("AMTNET").Value);

                        if (netAmount == 0)
                        {
                            flagInsert = true;
                            Log.Save("The net balance is zero for this invoice. Cannot apply an additional amount");
                        }
                        else
                        {
                            flagInsert = true;
                            arRecptDetail4.Fields.FieldByName("APPLY").SetValue("Y", false);
                            arRecptDetail4.Update();
                        }
                    }
                    else
                    {
                        Log.Save("Cannot apply to invoice: " + invnum + ". Check if invoice exist and the batch is posted.");
                        flagInsert = true;
                    }

                    if (flagInsert)
                    {
                        arRecptHeader.Insert();
                        arRecptHeader.RecordCreate(ViewRecordCreate.DelayKey);
                        Log.Save("Receipt Transferred");
                    }

                    arRecptBatch.Dispose();
                    arRecptHeader.Dispose();
                    arRecptDetail1.Dispose();
                    arRecptDetail2.Dispose();
                    arRecptDetail3.Dispose();
                    arRecptDetail4.Dispose();
                    arRecptDetail5.Dispose();
                    arRecptDetail6.Dispose();

                    return true;
                }
            }
        }

        public bool CheckAccpacInvoiceAvail(int invoiceId)
        {
            View cssql = dbLink.OpenView("CS0120");
            cssql.Browse("SELECT IDINVC FROM ARIBH WHERE IDINVC = '" + invoiceId.ToString() + "'", true);
            cssql.InternalSet(256);

            if (cssql.GoNext())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        void CreateReceiptBatchEx(string bankcode, string batchDesc)
        {
            CBBTCH1batch = dbLink.OpenView("AR0041");
            CBBTCH1header = dbLink.OpenView("AR0042");
            CBBTCH1detail1 = dbLink.OpenView("AR0044");
            CBBTCH1detail2 = dbLink.OpenView("AR0045");
            CBBTCH1detail3 = dbLink.OpenView("AR0043");
            CBBTCH1detail4 = dbLink.OpenView("AR0061");
            CBBTCH1detail5 = dbLink.OpenView("AR0406");
            CBBTCH1detail6 = dbLink.OpenView("AR0170");

            CBBTCH1batch.Compose(new View[] { CBBTCH1header });
            CBBTCH1header.Compose(new View[] { CBBTCH1batch, CBBTCH1detail3, CBBTCH1detail1, CBBTCH1detail5, CBBTCH1detail6 });
            CBBTCH1detail1.Compose(new View[] { CBBTCH1header, CBBTCH1detail2, CBBTCH1detail4 });
            CBBTCH1detail2.Compose(new View[] { CBBTCH1detail1 });
            CBBTCH1detail3.Compose(new View[] { CBBTCH1header });
            CBBTCH1detail4.Compose(new View[] { CBBTCH1batch, CBBTCH1header, CBBTCH1header, CBBTCH1detail3, CBBTCH1detail1, CBBTCH1detail2 });
            CBBTCH1detail5.Compose(new View[] { CBBTCH1header });
            CBBTCH1detail6.Compose(new View[] { CBBTCH1header });

            CBBTCH1batch.Fields.FieldByName("CODEPYMTYP").SetValue("CA", false);
            CBBTCH1header.Fields.FieldByName("CODEPYMTYP").SetValue("CA", false);
            CBBTCH1detail3.Fields.FieldByName("CODEPAYM").SetValue("CA", false);
            CBBTCH1detail1.Fields.FieldByName("CODEPAYM").SetValue("CA", false);
            CBBTCH1detail2.Fields.FieldByName("CODEPAYM").SetValue("CA", false);
            CBBTCH1detail4.Fields.FieldByName("PAYMTYPE").SetValue("CA", false);
            CBBTCH1batch.Fields.FieldByName("CODEPYMTYP").SetValue("CA", false);

            CBBTCH1batch.RecordClear();
            CBBTCH1batch.RecordCreate(ViewRecordCreate.Insert);
            CBBTCH1header.RecordCreate(ViewRecordCreate.DelayKey);
            CBBTCH1batch.Fields.FieldByName("BATCHDESC").SetValue(batchDesc, false);
            CBBTCH1batch.Fields.FieldByName("DATEBTCH").SetValue(DateTime.Now.ToString(), false);
            CBBTCH1batch.Update();
            CBBTCH1batch.Fields.FieldByName("IDBANK").SetValue(bankcode, false);
            CBBTCH1batch.Update();
            CBBTCH1header.RecordCreate(ViewRecordCreate.DelayKey);
        }

        int GetLastPaymentBatch()
        {
            int BatchId = 0;
            bool gotIt;
            CBBTCH1batch = dbLink.OpenView("AR0041");
            gotIt = CBBTCH1batch.GoBottom();

            if (gotIt)
            {
                BatchId = Convert.ToInt32(CBBTCH1batch.Fields.FieldByName("CNTBTCH").Value);
            }
            return BatchId;
        }

        public void CreditNoteInsert(string batchNumber, string customerId, string acctNumber, string amount, string invoiceToApply, string docNumber, string description)
        {
            b1_arInvoiceBatch = dbLink.OpenView("AR0031");
            b1_arInvoiceHeader = dbLink.OpenView("AR0032");
            b1_arInvoiceDetail = dbLink.OpenView("AR0033");
            b1_arInvoicePaymentSchedules = dbLink.OpenView("AR0034");
            b1_arInvoiceHeaderOptFields = dbLink.OpenView("AR0402");
            b1_arInvoiceDetailOptFields = dbLink.OpenView("AR0401");

            b1_arInvoiceBatch.Compose(new View[] { b1_arInvoiceHeader });
            b1_arInvoiceHeader.Compose(new View[] { b1_arInvoiceBatch, b1_arInvoiceDetail, b1_arInvoicePaymentSchedules, b1_arInvoiceHeaderOptFields });
            b1_arInvoiceDetail.Compose(new View[] { b1_arInvoiceHeader, b1_arInvoiceBatch, b1_arInvoiceDetailOptFields });
            b1_arInvoicePaymentSchedules.Compose(new View[] { b1_arInvoiceHeader });
            b1_arInvoiceHeaderOptFields.Compose(new View[] { b1_arInvoiceHeader });
            b1_arInvoiceDetailOptFields.Compose(new View[] { b1_arInvoiceDetail });

            b1_arInvoiceBatch.Fields.FieldByName("CNTBTCH").SetValue(batchNumber, false);
            b1_arInvoiceHeader.RecordCreate(ViewRecordCreate.DelayKey);
            b1_arInvoiceHeader.Fields.FieldByName("IDCUST").SetValue(customerId, false);
            b1_arInvoiceHeader.Fields.FieldByName("TEXTTRX").SetValue("3", false);
            b1_arInvoiceDetail.RecordCreate(ViewRecordCreate.NoInsert);

            b1_arInvoiceDetail.Fields.FieldByName("IDACCTREV").SetValue(acctNumber, false);
            b1_arInvoiceDetail.Fields.FieldByName("AMTEXTN").SetValue(amount, false);
            b1_arInvoiceDetail.Fields.FieldByName("TEXTDESC").SetValue(description, false);
            b1_arInvoiceDetail.Insert();

            b1_arInvoiceHeader.Fields.FieldByName("INVCAPPLTO").SetValue(invoiceToApply, false);
            b1_arInvoiceHeader.Fields.FieldByName("IDINVC").SetValue(docNumber, false);
            b1_arInvoiceHeader.Insert();
            b1_arInvoiceHeader.RecordCreate(ViewRecordCreate.DelayKey);
            Log.Save("Credit Memo transferred");
        }

        public int GetRBatchNumber(string referenceNumber)
        {
            int batchNum = -1;
            View cssql = dbLink.OpenView("CS0120"); ;

            cssql.Browse("SELECT CNTBTCH FROM ARTCR WHERE IDRMIT = '" + referenceNumber + "'", true);
            cssql.InternalSet(256);

            if (cssql.GoNext())
            {
                batchNum = Convert.ToInt32(cssql.Fields.FieldByName("CNTBTCH").Value);
            }
            return batchNum;
        }

        public string GetDocNumber(string referenceNumber)
        {
            string docNum = "";
            View cssql = dbLink.OpenView("CS0120"); ;

            cssql.Browse("SELECT DOCNBR FROM ARTCR WHERE IDRMIT = '" + referenceNumber + "'", true);
            cssql.InternalSet(256);

            if (cssql.GoNext())
            {
                docNum = Convert.ToString(cssql.Fields.FieldByName("DOCNBR").Value);
            }
            return docNum;
        }

        public void ComApiPayByCredit(string customerId, string invoiceId, string batchNumber, string documentNumber)
        {
            arRecptBatch = dbLink.OpenView("AR0041");
            arRecptHeader = dbLink.OpenView("AR0042");
            arRecptDetail1 = dbLink.OpenView("AR0044");
            arRecptDetail2 = dbLink.OpenView("AR0045");
            arRecptDetail3 = dbLink.OpenView("AR0043");
            arRecptDetail4 = dbLink.OpenView("AR0061");
            arRecptDetail5 = dbLink.OpenView("AR0406");
            arRecptDetail6 = dbLink.OpenView("AR0170");

            arRecptBatch.Compose(new View[] { arRecptHeader });
            arRecptHeader.Compose(new View[] { arRecptBatch, arRecptDetail3, arRecptDetail1, arRecptDetail5, arRecptDetail6 });
            arRecptDetail1.Compose(new View[] { arRecptHeader, arRecptDetail2, arRecptDetail4 });
            arRecptDetail2.Compose(new View[] { arRecptDetail1 });
            arRecptDetail3.Compose(new View[] { arRecptHeader });
            arRecptDetail4.Compose(new View[] { arRecptBatch, arRecptHeader, arRecptDetail3, arRecptDetail1, arRecptDetail2 });
            arRecptDetail5.Compose(new View[] { arRecptHeader });
            arRecptDetail6.Compose(new View[] { arRecptHeader });

            arRecptBatch.RecordClear();
            arRecptBatch.Fields.FieldByName("CODEPYMTYP").SetValue("CA", false);
            arRecptHeader.Fields.FieldByName("CODEPYMTYP").SetValue("CA", false);
            arRecptDetail3.Fields.FieldByName("CODEPAYM").SetValue("CA", false);
            arRecptDetail1.Fields.FieldByName("CODEPAYM").SetValue("CA", false);
            arRecptDetail2.Fields.FieldByName("CODEPAYM").SetValue("CA", false);
            arRecptDetail4.Fields.FieldByName("PAYMTYPE").SetValue("CA", false);
            arRecptBatch.Fields.FieldByName("CNTBTCH").SetValue(batchNumber, false);
            arRecptBatch.Read(false);

            arRecptDetail4.Cancel();
            arRecptDetail4.Fields.FieldByName("PAYMTYPE").SetValue("CA", false);
            arRecptDetail4.Fields.FieldByName("CNTBTCH").SetValue(batchNumber, false);
            arRecptDetail4.Fields.FieldByName("CNTITEM").SetValue("1", false);
            arRecptDetail4.Fields.FieldByName("IDCUST").SetValue(customerId, false);
            arRecptDetail4.Fields.FieldByName("AMTRMIT").SetValue("0.000", false);
            arRecptDetail4.Fields.FieldByName("STDOCDTE").SetValue(DateTime.Now.ToShortDateString(), false);


            arRecptHeader.RecordCreate(ViewRecordCreate.DelayKey);
            arRecptHeader.Fields.FieldByName("RMITTYPE").SetValue("4", false);
            arRecptHeader.Fields.FieldByName("IDCUST").SetValue(customerId, false);

            arRecptDetail4.Cancel();
            arRecptHeader.Fields.FieldByName("DOCNBR").SetValue(documentNumber, false);
            arRecptDetail4.Fields.FieldByName("STDOCSTR").SetValue(invoiceId, false);

            arRecptDetail4.Fields.FieldByName("PAYMTYPE").SetValue("CA", false);
            arRecptDetail4.Fields.FieldByName("CNTBTCH").SetValue(batchNumber, false);
            arRecptDetail4.Fields.FieldByName("CNTITEM").SetValue("0", false);
            arRecptDetail4.Fields.FieldByName("IDCUST").SetValue(customerId, false);
            arRecptDetail4.Fields.FieldByName("AMTRMIT").SetValue("0.000", false);

            arRecptDetail4.Process();

            arRecptDetail4.Fields.FieldByName("CNTITEM").SetValue("0", false);
            arRecptDetail4.Fields.FieldByName("CNTKEY").SetValue("-1", false);
            arRecptDetail4.Read(false);

            arRecptDetail4.Fields.FieldByName("APPLY").SetValue("Y", false);
            arRecptDetail4.Update();

            arRecptDetail4.Read(false);
            arRecptHeader.Insert();

            arRecptBatch.Read(false);
            arRecptHeader.RecordCreate(ViewRecordCreate.DelayKey);
        }

        public bool InvoiceDelete(int invoiceId)
        {
            int entryNumber = -1;
            int batchNumber = -1;

            entryNumber = GetEntryNumber(invoiceId);
            batchNumber = GetIbatchNumber(invoiceId);


            if (!CheckAccpacIBatchPosted(batchNumber))
            {
                b1_arInvoiceBatch = dbLink.OpenView("AR0031");
                b1_arInvoiceHeader = dbLink.OpenView("AR0032");
                b1_arInvoiceDetail = dbLink.OpenView("AR0033");
                b1_arInvoicePaymentSchedules = dbLink.OpenView("AR0034");
                b1_arInvoiceHeaderOptFields = dbLink.OpenView("AR0402");
                b1_arInvoiceDetailOptFields = dbLink.OpenView("AR0401");

                b1_arInvoiceBatch.Compose(new View[] { b1_arInvoiceHeader });
                b1_arInvoiceHeader.Compose(new View[] { b1_arInvoiceBatch, b1_arInvoiceDetail, b1_arInvoicePaymentSchedules, null });
                b1_arInvoiceDetail.Compose(new View[] { b1_arInvoiceHeader, b1_arInvoiceBatch, b1_arInvoiceDetailOptFields });
                b1_arInvoicePaymentSchedules.Compose(new View[] { b1_arInvoiceHeader });
                b1_arInvoiceHeaderOptFields.Compose(new View[] { b1_arInvoiceHeader });
                b1_arInvoiceDetailOptFields.Compose(new View[] { b1_arInvoiceDetail });

                b1_arInvoiceBatch.Fields.FieldByName("CNTBTCH").SetValue(batchNumber.ToString(), false);
                string searchFilter = "CNTITEM = " + entryNumber;
                b1_arInvoiceHeader.Browse(searchFilter, true);

                b1_arInvoiceHeader.Fetch(false);
                b1_arInvoiceHeader.Delete();

                Log.Save("Invoice: " + invoiceId.ToString() + " was deleted from the batch (" + batchNumber + ")");
                return true;
            }
            else
            {
                Log.Save("The batch is already posted, cannot delete invoice");
                return false;
            }
        }

        void GetViewInfo(View ax, string filename)
        {
            string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            StreamWriter output = new StreamWriter(mydocpath + @"\" + filename + ".txt");

            int count = ax.Fields.Count;
            output.WriteLine(count.ToString() + " fields found - " + ax.Description);
            output.WriteLine(" ");
            output.WriteLine("------------------");
            output.WriteLine(" ");

            string name, desc;
            for (int i = 1; i <= count; i++)
            {
                var x = ax.Fields.FieldByID(i);
                name = x.Name;
                desc = x.Description;

                output.WriteLine(i.ToString() + ". " + name + " ----------- " + desc);
            }
            output.Close();
        }

        int CountBatchPaymentEntries(string batchId)
        {
            int count = 0;
            CBBTCH1batch = dbLink.OpenView("CB0009");
            CBBTCH1header = dbLink.OpenView("CB0010");
            CBBTCH1detail1 = dbLink.OpenView("CB0011");
            CBBTCH1detail2 = dbLink.OpenView("CB0012");
            CBBTCH1detail3 = dbLink.OpenView("CB0013");
            CBBTCH1detail4 = dbLink.OpenView("CB0014");
            CBBTCH1detail5 = dbLink.OpenView("CB0015");
            CBBTCH1detail6 = dbLink.OpenView("CB0016");
            CBBTCH1detail7 = dbLink.OpenView("CB0403");
            CBBTCH1detail8 = dbLink.OpenView("CB0404");

            CBBTCH1batch.Compose(new View[] { CBBTCH1header });
            CBBTCH1header.Compose(new View[] { CBBTCH1batch, CBBTCH1detail1, CBBTCH1detail4, CBBTCH1detail8 });
            CBBTCH1detail1.Compose(new View[] { CBBTCH1header, CBBTCH1detail2, CBBTCH1detail5, CBBTCH1detail7 });
            CBBTCH1detail2.Compose(new View[] { CBBTCH1detail1, CBBTCH1detail3, CBBTCH1detail6 });
            CBBTCH1detail3.Compose(new View[] { CBBTCH1detail2 });
            CBBTCH1detail4.Compose(new View[] { CBBTCH1header });
            CBBTCH1detail5.Compose(new View[] { CBBTCH1detail1 });
            CBBTCH1detail6.Compose(new View[] { CBBTCH1detail2 });
            CBBTCH1detail7.Compose(new View[] { CBBTCH1detail1 });
            CBBTCH1detail8.Compose(new View[] { CBBTCH1header });

            CBBTCH1header.Init();
            CBBTCH1batch.Fields.FieldByName("BATCHID").SetValue(batchId, false);
            CBBTCH1batch.Read(false);
            CBBTCH1header.Read(false);

            bool gotIt = CBBTCH1header.GoTop();

            while (gotIt)
            {
                CBBTCH1header.Fields.FieldByName("REFERENCE").Value.ToString();
                gotIt = CBBTCH1header.GoNext();
                count++;
            }
            return count;
        }

        public void OnDebug()
        {
            OnStart(null);
        }

        public void XrateInsert(string amt)
        {
            csRateHeader = dbLink.OpenView("CS0005");
            csRateDetail = dbLink.OpenView("CS0006");
            csRateHeader.Compose(new View[] { csRateDetail });
            csRateDetail.Compose(new View[] { csRateHeader });

            csRateHeader.Fields.FieldByName("PRGTNOW").SetValue("1", false);
            csRateHeader.Fields.FieldByName("HOMECUR").SetValue("JAD", false);
            csRateHeader.Fields.FieldByName("RATETYPE").SetValue("BB", false);
            csRateHeader.Read(false);

            csRateDetail.Read(false);
            csRateDetail.RecordCreate(ViewRecordCreate.NoInsert);
            csRateDetail.Fields.FieldByName("SOURCECUR").SetValue("USD", false);
            csRateDetail.Fields.FieldByName("RATEDATE").SetValue(DateTime.Now.ToString(), false);
            csRateDetail.Fields.FieldByName("RATE").SetValue(amt, false);

            csRateDetail.Insert();
            csRateDetail.Fields.FieldByName("SOURCECUR").SetValue("USD", false);
            csRateDetail.Read(false);
            csRateHeader.Update();
        }
    }
}
