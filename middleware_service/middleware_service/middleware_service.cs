using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using Microsoft.Owin.Hosting;
using Microsoft.AspNet.SignalR;
using TableDependency.Mappers;

namespace middleware_service
{
    public partial class MiddlewareService : ServiceBase
    {
        #region Definitions
        SqlTableDependency<SqlNotify_ArInvoices> tableDependencyArInvoices;
        SqlTableDependency<SqlNotify_ArInvoiceDetail> tableDependencyArInvoiceDetail;
        SqlTableDependency<SqlNotify_GlDocuments> tableDependencyGlDocuments;
        SqlTableDependency<SqlNotify_ArPayments> tableDependencyArPayments;

        public SqlConnection connGeneric;
        public SqlConnection connIntegration;
        public SqlConnection connMsgQueue;

        Session accpacSession;
        DBLink dbLink;

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
        }

        private void DeferredTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            DateTime MonthlyRptDate = Integration.GetNextGenDate("Monthly");
            DateTime AnnualRptDate = Integration.GetNextGenDate("Annual");

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
                    Database_Operations.Report rpt = new Database_Operations.Report();
                    var result = rpt.gen_rpt("Monthly", 0, m, y);

                    if (result != null)
                    {
                        int es = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) - DateTime.Now.Day;
                        es++;
                        DateTime nextMonth = DateTime.Now.AddDays(es);
                        DateTime nextGenDate = new DateTime(nextMonth.Year, nextMonth.Month, 2);
                        nextGenDate = nextGenDate.AddHours(2);
                        Integration.SetNextGenDate("Monthly", nextGenDate);
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
                    Database_Operations.Report rpt = new Database_Operations.Report();
                    var result = rpt.gen_rpt("Monthly", 0, m, y);

                    if (result != null)
                    {
                        DateTime nextGenDate = new DateTime(DateTime.Now.Year + 1, 4, 2);
                        nextGenDate = nextGenDate.AddHours(3);
                        Integration.SetNextGenDate("Annual", nextGenDate);
                        Log.Save("Annual Deferred Report Generated.");
                    }
                }
            }
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                Log.Open("Log.txt");
                accpacSession = new Session();
                
                if (!Integration.IsBrokerEnabled(Constants.DB_GENERIC_NAME))
                {
                    Log.Save("Enabling broker on database: " + Constants.DB_GENERIC_NAME);
                    Integration.SetBrokerEnabled(Constants.DB_GENERIC_NAME);
                }

                var mapperArInvoices = new ModelToTableMapper<SqlNotify_ArInvoices>();
                mapperArInvoices.AddMapping(inv => inv.Invoice, "Invoice#");
                mapperArInvoices.AddMapping(inv => inv.Batch, "Batch#");
                mapperArInvoices.AddMapping(inv => inv.Ref, "Ref#");
                mapperArInvoices.AddMapping(inv => inv.PO, "PO#");
                mapperArInvoices.AddMapping(inv => inv.Job, "Job#");

                using (tableDependencyArInvoices = new SqlTableDependency<SqlNotify_ArInvoices>(Constants.DB_GENERIC, "tblARInvoices", mapper: mapperArInvoices))
                {
                    tableDependencyArInvoices.OnChanged += TableDependencyArInvoices_OnChanged;
                    tableDependencyArInvoices.OnError += TableDependencyArInvoices_OnError;
                }

                using (tableDependencyGlDocuments = new SqlTableDependency<SqlNotify_GlDocuments>(Constants.DB_GENERIC, "tblGLDocuments"))
                {
                    tableDependencyGlDocuments.OnChanged += TableDependencyGlDocuments_OnChanged;
                    tableDependencyGlDocuments.OnError += TableDependencyGlDocuments_OnError;
                }


                if (Constants.PARALLEL_RUN)
                {
                    using (tableDependencyArInvoiceDetail = new SqlTableDependency<SqlNotify_ArInvoiceDetail>(Constants.DB_GENERIC, "tblARInvoiceDetail"))
                    {
                        tableDependencyArInvoiceDetail.OnChanged += TableDependencyArInvoiceDetail_OnChanged;
                        tableDependencyArInvoiceDetail.OnError += TableDependencyArInvoiceDetail_OnError;
                    }

                    var mapperArPayments = new ModelToTableMapper<SqlNotify_ArPayments>();
                    mapperArPayments.AddMapping(pay => pay.Batch, "Batch#");
                    mapperArPayments.AddMapping(pay => pay.Check, "Check#");
                    mapperArPayments.AddMapping(pay => pay.Ref, "Ref#");

                    using (tableDependencyArPayments = new SqlTableDependency<SqlNotify_ArPayments>(Constants.DB_GENERIC, "tblARPayments", mapper: mapperArPayments))
                    {
                        tableDependencyArPayments.OnChanged += TableDependencyArPayments_OnChanged;
                        tableDependencyArPayments.OnError += TableDependencyArPayments_OnError;
                    }

                    tableDependencyArInvoiceDetail.Start();
                    tableDependencyArPayments.Start();
                    tableDependencyArInvoices.Start();
                    tableDependencyGlDocuments.Start();
                }
                else
                {
                    tableDependencyArInvoices.Start();
                    tableDependencyGlDocuments.Start();
                }

                deferredTimer.Elapsed += DeferredTimer_Elapsed;
                deferredTimer.Enabled = true;
                deferredTimer.Interval = 3600000;

                //////////////////////////////////////////////////////////////////// STARTING SESSION ///////////////////////////////////////////////////////////////////////
                Log.Save("Starting accpac session...");
                accpacSession.Init("", "XY", "XY1000", "65A");
                accpacSession.Open(Constants.ACCPAC_USER, Constants.ACCPAC_CRED, Constants.ACCPAC_COMPANY, DateTime.Today, 0);
                dbLink = accpacSession.OpenDBLink(DBLinkType.Company, DBLinkFlags.ReadWrite);

                Log.Save("Accpac Version: " + accpacSession.AppVersion);
                Log.Save("Company: " + accpacSession.CompanyName);
                Log.Save("Session Status: " + accpacSession.IsOpened);


                deferredTimer.Start();
                Log.Save("middleware_service started.");
                Log.WriteEnd();
                DeferredTimer_Elapsed(null, null);
                currentTime = DateTime.Now;
                InitSignalR();
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
                tableDependencyArInvoices.Stop();
                tableDependencyGlDocuments.Stop();

                if (Constants.PARALLEL_RUN)
                {
                    tableDependencyArInvoiceDetail.Stop();
                    tableDependencyArPayments.Stop();
                    tableDependencyArInvoiceDetail.Dispose();
                    tableDependencyArPayments.Dispose();
                }

                tableDependencyArInvoices.Dispose();
                tableDependencyGlDocuments.Dispose();
                deferredTimer.Stop();

                Log.Save("middleware_service stopped.");
                Log.WriteEnd();
                Log.Close();
                accpacSession.Dispose();

                if (selfHost != null)
                {
                    selfHost.Dispose();
                }
            }
            catch (Exception e)
            {
                Log.Save(e.Message + " " + e.StackTrace);
                Log.WriteEnd();
                Log.Close();
            }
        }

        public void SignalEventHandler(object source, SignalR.EventObjects.SignalArgs args)
        {
            Log.Save("Command: " + args.message + " received from: " + args.username);
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

        private void TableDependencyArInvoices_OnError(object sender, TableDependency.EventArgs.ErrorEventArgs e)
        {
            Log.Save("Table Dependency error: " + e.Error.Message);
            Stop();
        }

        private void TableDependencyGlDocuments_OnError(object sender, TableDependency.EventArgs.ErrorEventArgs e)
        {
            Log.Save("Table Dependency error: " + e.Error.Message);
            Stop();
        }

        private void TableDependencyArInvoiceDetail_OnError(object sender, TableDependency.EventArgs.ErrorEventArgs e)
        {
            Log.Save("Table Dependency error: " + e.Error.Message);
            Stop();
        }

        private void TableDependencyArPayments_OnError(object sender, TableDependency.EventArgs.ErrorEventArgs e)
        {
            Log.Save("Table Dependency error: " + e.Error.Message);
            Stop();
        }

        private void TableDependencyArPayments_OnChanged(object sender, RecordChangedEventArgs<SqlNotify_ArPayments> e)
        {
            try
            {
                if (e.ChangeType == ChangeType.Insert)
                {
                    e.Entity.ArPaymentsToGeneric();
                }
            }
            catch (Exception ex)
            {
                Log.Save(ex.Message + " " + ex.StackTrace);
                Stop();
            }
        }

        private void TableDependencyArInvoiceDetail_OnChanged(object sender, RecordChangedEventArgs<SqlNotify_ArInvoiceDetail> e)
        {
            try
            {
                e.Entity.ARInvoiceDetailToGenric();
            }
            catch (Exception ex)
            {
                Log.Save(ex.Message + " " + ex.StackTrace);
                Stop();
            }
        }

        private void TableDependencyGlDocuments_OnChanged(object sender, RecordChangedEventArgs<SqlNotify_GlDocuments> e)
        {
            try
            {
                Thread.Sleep(500);
                if (!Constants.IGNORE_EVENTS)
                {
                    if (IsAccpacSessionOpen())
                    {
                        Log.Save("Database change detected, operation: " + e.ChangeType);

                        var docInfo = e.Entity;
                        if (e.ChangeType == ChangeType.Insert)
                        {
                            if (docInfo.DocumentType == INVOICE)
                            {
                                if (Constants.PARALLEL_RUN)
                                {
                                    e.Entity.GlDocumentsToGenric();
                                }

                                Log.Save("Incoming invoice...");
                                InvoiceInfo invoiceInfo = new InvoiceInfo();

                                while (invoiceInfo.amount == 0)
                                {
                                    Log.Save("Waiting for invoice amount to update, current value: " + invoiceInfo.amount.ToString());
                                    invoiceInfo = Integration.GetInvoiceInfo(docInfo.OriginalDocumentID);
                                }
                                Log.Save("Invoice amount: " + invoiceInfo.amount);

                                List<string> clientInfo = Integration.GetClientInfoInv(invoiceInfo.customerId.ToString());
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
                                Data dt = Translate(cNum, invoiceInfo.feeType, companyName, "", invoiceInfo.notes, Integration.GetAccountNumber(invoiceInfo.glid), invoiceInfo.freqUsage);
                                DateTime invoiceValidity = Integration.GetValidity(docInfo.OriginalDocumentID);
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
                                List<string> data = Integration.CheckInvoiceAvail(docInfo.OriginalDocumentID.ToString());
                                int r = Integration.GetInvoiceReference(docInfo.OriginalDocumentID);
                                Log.Save("Invoice Reference number: " + r);

                                if (r != -1)
                                {
                                    Log.Save("Getting Maj Details...");
                                    m = Integration.GetMajDetail(r);
                                    Log.Save(m.ToString());
                                }

                                if (IsPeriodCreated(financialyear))
                                {
                                    if (invoiceInfo.glid < 5000 || data != null)
                                    {
                                        Log.Save("CreditGL: " + invoiceInfo.glid);
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

                                                        Integration.UpdateBatchCount(RENEWAL_SPEC + "For " + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString());
                                                        Integration.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                                        Integration.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.glid);

                                                        prevInvoice = currentInvoice;

                                                        Integration.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()), invoiceInfo.glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isVoided, 0, 0);
                                                        Integration.MarkTransferred(docInfo.OriginalDocumentID);
                                                        prevInvoice = currentInvoice;
                                                    }
                                                    else
                                                    {
                                                        Integration.UpdateBatchCount(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString());
                                                        Integration.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                                        Integration.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.glid);

                                                        prevInvoice = currentInvoice;
                                                        Integration.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString() + " For " + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()), invoiceInfo.glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isVoided, 0, 0);
                                                        Integration.MarkTransferred(docInfo.OriginalDocumentID);
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

                                                        Integration.UpdateBatchCount(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString());
                                                        Integration.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                                        Integration.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.glid);

                                                        prevInvoice = currentInvoice;
                                                        Integration.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()), invoiceInfo.glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isVoided, 0, 0);
                                                        Integration.MarkTransferred(docInfo.OriginalDocumentID);
                                                        prevInvoice = currentInvoice;
                                                    }
                                                    else
                                                    {
                                                        Integration.UpdateBatchCount(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString());
                                                        Integration.UpdateEntryNumber(docInfo.OriginalDocumentID);

                                                        Integration.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.glid);

                                                        prevInvoice = currentInvoice;
                                                        Integration.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()), invoiceInfo.glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isVoided, 0, 0);
                                                        Integration.MarkTransferred(docInfo.OriginalDocumentID);
                                                        prevInvoice = currentInvoice;
                                                    }
                                                }
                                                else if ((invoiceInfo.notes == "Annual Fee" && m.stationType == "SSL" && m.certificateType == 0 && m.proj == "JMC") || (invoiceInfo.freqUsage == "PRS55"))
                                                {
                                                    stat = InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), GetBatch(MAJ, docInfo.OriginalDocumentID.ToString()).ToString());

                                                    if (stat.status == "Not Exist")
                                                    {
                                                        CreateCustomer(dt.customerId, dt.companyName_NewCust);
                                                        InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), GetBatch(MAJ, docInfo.OriginalDocumentID.ToString()).ToString());

                                                        Integration.UpdateBatchCount(MAJ);
                                                        Integration.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                                        Integration.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.glid);

                                                        prevInvoice = currentInvoice;
                                                        Integration.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(MAJ, docInfo.OriginalDocumentID.ToString()), invoiceInfo.glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isVoided, 0, 0);
                                                        Integration.MarkTransferred(docInfo.OriginalDocumentID);
                                                        prevInvoice = currentInvoice;
                                                    }
                                                    else
                                                    {
                                                        Integration.UpdateBatchCount(MAJ);
                                                        Integration.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                                        Integration.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.glid);

                                                        prevInvoice = currentInvoice;
                                                        Integration.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(MAJ, docInfo.OriginalDocumentID.ToString()), invoiceInfo.glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isVoided, 0, 0);
                                                        Integration.MarkTransferred(docInfo.OriginalDocumentID);
                                                        prevInvoice = currentInvoice;
                                                    }
                                                }
                                                else if (invoiceInfo.notes == "Type Approval" || invoiceInfo.freqUsage == "TA-ProAmend")
                                                {
                                                    stat = InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, ChangeToUS(invoiceInfo.amount).ToString(), GetBatch(TYPE_APPROVAL, docInfo.OriginalDocumentID.ToString()).ToString());

                                                    if (stat.status == "Not Exist")
                                                    {
                                                        CreateCustomer(dt.customerId, dt.companyName_NewCust);
                                                        InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, ChangeToUS(invoiceInfo.amount).ToString(), GetBatch(TYPE_APPROVAL, docInfo.OriginalDocumentID.ToString()).ToString());

                                                        Integration.UpdateBatchCount(TYPE_APPROVAL);
                                                        Integration.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                                        Integration.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.glid);

                                                        prevInvoice = currentInvoice;
                                                        Integration.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(TYPE_APPROVAL, docInfo.OriginalDocumentID.ToString()), invoiceInfo.glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.author, invoiceInfo.amount, "no modification", Integration.GetRate(), ChangeToUS(invoiceInfo.amount), invoiceInfo.isVoided, 0, 0);
                                                        Integration.MarkTransferred(docInfo.OriginalDocumentID);
                                                        prevInvoice = currentInvoice;
                                                    }
                                                    else
                                                    {
                                                        Integration.UpdateBatchCount(TYPE_APPROVAL);
                                                        Integration.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                                        Integration.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.glid);

                                                        prevInvoice = currentInvoice;
                                                        Integration.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(TYPE_APPROVAL, docInfo.OriginalDocumentID.ToString()), invoiceInfo.glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.author, invoiceInfo.amount, "no modification", Integration.GetRate(), ChangeToUS(invoiceInfo.amount), invoiceInfo.isVoided, 0, 0);
                                                        Integration.MarkTransferred(docInfo.OriginalDocumentID);
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

                                                        Integration.UpdateBatchCount(NON_MAJ);
                                                        Integration.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                                        Integration.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.glid);

                                                        prevInvoice = currentInvoice;
                                                        Integration.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(NON_MAJ, docInfo.OriginalDocumentID.ToString()), invoiceInfo.glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isVoided, 0, 0);
                                                        Integration.MarkTransferred(docInfo.OriginalDocumentID);
                                                        prevInvoice = currentInvoice;
                                                    }
                                                    else
                                                    {
                                                        Integration.UpdateBatchCount(NON_MAJ);
                                                        Integration.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                                        Integration.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.glid);

                                                        prevInvoice = currentInvoice;
                                                        Integration.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(NON_MAJ, docInfo.OriginalDocumentID.ToString()), invoiceInfo.glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isVoided, 0, 0);
                                                        Integration.MarkTransferred(docInfo.OriginalDocumentID);
                                                        prevInvoice = currentInvoice;
                                                    }
                                                }
                                            }
                                            else if (data[1].ToString() == "T")
                                            {
                                                List<string> detail = new List<string>(3);
                                                detail = Integration.GetInvoiceDetails(docInfo.OriginalDocumentID);
                                                int batchNumber = GetIbatchNumber(docInfo.OriginalDocumentID);


                                                if (!CheckAccpacIBatchPosted(batchNumber))
                                                {
                                                    if (invoiceInfo.glid != Convert.ToInt32(detail[2].ToString()) || invoiceInfo.amount.ToString() != detail[3].ToString())
                                                    {
                                                        if (invoiceInfo.notes == "Type Approval" || invoiceInfo.freqUsage == "TA-ProAmend")
                                                        {
                                                            string usamt = "";
                                                            usamt = ChangeToUSupdated(invoiceInfo.amount, docInfo.OriginalDocumentID).ToString();
                                                            UpdateInvoice(dt.fcode, Math.Round(ChangeToUSupdated(invoiceInfo.amount, docInfo.OriginalDocumentID), 2).ToString(), batchNumber.ToString().ToString(), GetEntryNumber(docInfo.OriginalDocumentID).ToString());
                                                            Integration.StoreInvoice(docInfo.OriginalDocumentID, batchNumber, invoiceInfo.glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.author, invoiceInfo.amount, "updated", 1, Convert.ToDecimal(usamt), invoiceInfo.isVoided, 0, 0);
                                                        }
                                                        else
                                                        {
                                                            UpdateInvoice(dt.fcode, invoiceInfo.amount.ToString(), batchNumber.ToString().ToString(), GetEntryNumber(docInfo.OriginalDocumentID).ToString());
                                                            Integration.StoreInvoice(docInfo.OriginalDocumentID, batchNumber, invoiceInfo.glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.author, invoiceInfo.amount, "updated", 1, 0, invoiceInfo.isVoided, 0, 0);
                                                        }
                                                        Integration.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.glid);
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
                                                Integration.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()), invoiceInfo.glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isVoided, 0, 0);
                                                Log.Save("Invoice Id: " + docInfo.OriginalDocumentID.ToString() + " Transferred");
                                                prevInvoice = currentInvoice;
                                                prevTime = DateTime.Now;
                                            }
                                            else if (dt.feeType == "RF" && invoiceInfo.notes == "Renewal")
                                            {
                                                DataSet df = new DataSet();
                                                df = Integration.GetRenewalInvoiceValidity(docInfo.OriginalDocumentID);
                                                DateTime val = DateTime.Now;
                                                if (!IsEmpty(df))
                                                {
                                                    DataRow dr = df.Tables[0].Rows[0];
                                                    string date = dr.ItemArray.GetValue(0).ToString();
                                                    val = Convert.ToDateTime(date);
                                                }

                                                Integration.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()), invoiceInfo.glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isVoided, 0, 0);
                                                Log.Save("Invoice Id: " + docInfo.OriginalDocumentID.ToString() + " Transferred");
                                                prevInvoice = currentInvoice;
                                                prevTime = DateTime.Now;
                                            }

                                            else if ((invoiceInfo.notes == "Annual Fee" && m.stationType == "SSL" && m.certificateType == 0 && m.proj == "JMC") || (invoiceInfo.freqUsage == "PRS55"))
                                            {
                                                Integration.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(MAJ, docInfo.OriginalDocumentID.ToString()), invoiceInfo.glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isVoided, 0, 0);
                                                Log.Save("Invoice Id: " + docInfo.OriginalDocumentID.ToString() + " Transferred");
                                                prevInvoice = currentInvoice;
                                                prevTime = DateTime.Now;
                                            }
                                            else if (invoiceInfo.notes == "Type Approval" || invoiceInfo.freqUsage == "TA-ProAmend")
                                            {
                                                Integration.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(TYPE_APPROVAL, docInfo.OriginalDocumentID.ToString()), invoiceInfo.glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.author, invoiceInfo.amount, "no modification", Integration.GetRate(), ChangeToUS(invoiceInfo.amount), invoiceInfo.isVoided, 0, 0);
                                                Log.Save("Invoice Id: " + docInfo.OriginalDocumentID.ToString() + " Transferred");
                                                prevInvoice = currentInvoice;
                                                prevTime = DateTime.Now;
                                            }
                                            else if (invoiceInfo.notes == "Annual Fee" || invoiceInfo.notes == "Modification" || invoiceInfo.notes == "Radio Operator")
                                            {
                                                Integration.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(NON_MAJ, docInfo.OriginalDocumentID.ToString()), invoiceInfo.glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isVoided, 0, 0);
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

                                                Integration.UpdateBatchCount(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString());
                                                Integration.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                                Integration.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.glid);

                                                prevInvoice = currentInvoice;
                                                Integration.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()), invoiceInfo.glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isVoided, 0, 0);
                                                Integration.MarkTransferred(docInfo.OriginalDocumentID);
                                                prevInvoice = currentInvoice;
                                                Integration.UpdateBatchAmount(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), invoiceInfo.amount);
                                            }
                                            else
                                            {
                                                Integration.UpdateBatchCount(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString());
                                                Integration.UpdateEntryNumber(docInfo.OriginalDocumentID);

                                                Integration.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.glid);
                                                prevInvoice = currentInvoice;
                                                Integration.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()), invoiceInfo.glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isVoided, 0, 0);
                                                Integration.MarkTransferred(docInfo.OriginalDocumentID);
                                                prevInvoice = currentInvoice;
                                                Integration.UpdateBatchAmount(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), invoiceInfo.amount);
                                            }
                                        }
                                        else if (dt.feeType == "RF" && invoiceInfo.notes == "Renewal")
                                        {
                                            stat = InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), GetBatch(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()).ToString());

                                            if (stat.status == "Not Exist")
                                            {
                                                CreateCustomer(dt.customerId, dt.companyName_NewCust);
                                                InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), GetBatch(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()).ToString());

                                                Integration.UpdateBatchCount(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString());
                                                Integration.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                                Integration.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.glid);

                                                prevInvoice = currentInvoice;
                                                Integration.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()), invoiceInfo.glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isVoided, 0, 0);
                                                Integration.MarkTransferred(docInfo.OriginalDocumentID);
                                                prevInvoice = currentInvoice;
                                                Integration.UpdateBatchAmount(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), invoiceInfo.amount);
                                            }
                                            else
                                            {
                                                Integration.UpdateBatchCount(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString());
                                                Integration.UpdateEntryNumber(docInfo.OriginalDocumentID);

                                                Integration.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.glid);

                                                prevInvoice = currentInvoice;
                                                Integration.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()), invoiceInfo.glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isVoided, 0, 0);
                                                Integration.MarkTransferred(docInfo.OriginalDocumentID);
                                                prevInvoice = currentInvoice;
                                                Integration.UpdateBatchAmount(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), invoiceInfo.amount);
                                            }
                                        }
                                        else if ((invoiceInfo.notes == "Annual Fee" && m.stationType == "SSL" && m.certificateType == 0 && m.proj == "JMC") || (invoiceInfo.freqUsage == "PRS55"))
                                        {
                                            stat = InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), GetBatch(MAJ, docInfo.OriginalDocumentID.ToString()).ToString());

                                            if (stat.status == "Not Exist")
                                            {
                                                CreateCustomer(dt.customerId, dt.companyName_NewCust);
                                                InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), GetBatch(MAJ, docInfo.OriginalDocumentID.ToString()).ToString());

                                                Integration.UpdateBatchCount(MAJ);
                                                Integration.UpdateEntryNumber(docInfo.OriginalDocumentID);

                                                Integration.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.glid);

                                                prevInvoice = currentInvoice;
                                                Integration.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(MAJ, docInfo.OriginalDocumentID.ToString()), invoiceInfo.glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isVoided, 0, 0);
                                                Integration.MarkTransferred(docInfo.OriginalDocumentID);
                                                prevInvoice = currentInvoice;
                                                Integration.UpdateBatchAmount(MAJ, invoiceInfo.amount);
                                            }
                                            else
                                            {
                                                Integration.UpdateBatchCount(MAJ);
                                                Integration.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                                Integration.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.glid);

                                                prevInvoice = currentInvoice;
                                                Integration.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(MAJ, docInfo.OriginalDocumentID.ToString()), invoiceInfo.glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isVoided, 0, 0);
                                                Integration.MarkTransferred(docInfo.OriginalDocumentID);
                                                prevInvoice = currentInvoice;
                                                Integration.UpdateBatchAmount(MAJ, invoiceInfo.amount);
                                            }
                                        }
                                        else if (invoiceInfo.notes == "Type Approval" || invoiceInfo.freqUsage == "TA-ProAmend")
                                        {
                                            stat = InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, ChangeToUS(invoiceInfo.amount).ToString(), GetBatch(TYPE_APPROVAL, docInfo.OriginalDocumentID.ToString()).ToString());

                                            if (stat.status == "Not Exist")
                                            {
                                                CreateCustomer(dt.customerId, dt.companyName_NewCust);
                                                InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, ChangeToUS(invoiceInfo.amount).ToString(), GetBatch(TYPE_APPROVAL, docInfo.OriginalDocumentID.ToString()).ToString());

                                                Integration.UpdateBatchCount(TYPE_APPROVAL);
                                                Integration.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                                Integration.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.glid);

                                                prevInvoice = currentInvoice;
                                                Integration.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(TYPE_APPROVAL, docInfo.OriginalDocumentID.ToString()), invoiceInfo.glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.author, invoiceInfo.amount, "no modification", Integration.GetRate(), ChangeToUS(invoiceInfo.amount), invoiceInfo.isVoided, 0, 0);
                                                Integration.MarkTransferred(docInfo.OriginalDocumentID);
                                                prevInvoice = currentInvoice;
                                                Integration.UpdateBatchAmount(TYPE_APPROVAL, invoiceInfo.amount);
                                            }
                                            else
                                            {
                                                Integration.UpdateBatchCount(TYPE_APPROVAL);
                                                Integration.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                                Integration.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.glid);

                                                prevInvoice = currentInvoice;
                                                Integration.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(TYPE_APPROVAL, docInfo.OriginalDocumentID.ToString()), invoiceInfo.glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.author, invoiceInfo.amount, "no modification", Integration.GetRate(), ChangeToUS(invoiceInfo.amount), invoiceInfo.isVoided, 0, 0);
                                                Integration.MarkTransferred(docInfo.OriginalDocumentID);
                                                prevInvoice = currentInvoice;
                                                Integration.UpdateBatchAmount(TYPE_APPROVAL, invoiceInfo.amount);
                                            }
                                        }

                                        else if (invoiceInfo.notes == "Annual Fee" || invoiceInfo.notes == "Modification" || invoiceInfo.notes == "Radio Operator")
                                        {
                                            stat = InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), GetBatch(NON_MAJ, docInfo.OriginalDocumentID.ToString()).ToString());

                                            if (stat.status == "Not Exist")
                                            {
                                                CreateCustomer(dt.customerId, dt.companyName_NewCust);
                                                InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), GetBatch(NON_MAJ, docInfo.OriginalDocumentID.ToString()).ToString());

                                                Integration.UpdateBatchCount(NON_MAJ);
                                                Integration.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                                Integration.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.glid);

                                                prevInvoice = currentInvoice;
                                                Integration.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(NON_MAJ, docInfo.OriginalDocumentID.ToString()), invoiceInfo.glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isVoided, 0, 0);
                                                Integration.MarkTransferred(docInfo.OriginalDocumentID);
                                                prevInvoice = currentInvoice;
                                                Integration.UpdateBatchAmount(NON_MAJ, invoiceInfo.amount);
                                            }
                                            else
                                            {
                                                Integration.UpdateBatchCount(NON_MAJ);
                                                Integration.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                                Integration.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.glid);
                                                prevInvoice = currentInvoice;
                                                Integration.StoreInvoice(docInfo.OriginalDocumentID, GetBatch(NON_MAJ, docInfo.OriginalDocumentID.ToString()), invoiceInfo.glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isVoided, 0, 0);
                                                Integration.MarkTransferred(docInfo.OriginalDocumentID);
                                                prevInvoice = currentInvoice;
                                                Integration.UpdateBatchAmount(NON_MAJ, invoiceInfo.amount);
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
                                if (Constants.PARALLEL_RUN)
                                {
                                    e.Entity.GlDocumentsToGenric();
                                }

                                Data dt = new Data();
                                PaymentInfo pinfo = new PaymentInfo();

                                List<string> paymentData = new List<string>(3);
                                List<string> clientData = new List<string>(3);
                                List<string> feeData = new List<string>(3);

                                while (pinfo.ReceiptNumber == 0)
                                {
                                    pinfo = Integration.GetReceiptInfo(docInfo.OriginalDocumentID);
                                    Thread.Sleep(500);
                                }

                                var receipt = pinfo.ReceiptNumber;
                                var transid = pinfo.GLTransactionID;
                                var id = pinfo.CustomerID.ToString();

                                paymentData = Integration.GetPaymentInfo(transid);
                                var debit = pinfo.Debit.ToString();
                                var glid = pinfo.GLID.ToString();
                                var invoiceId = pinfo.InvoiceID.ToString();

                                DateTime paymentDate = pinfo.Date1;
                                string prepstat = " ";
                                DateTime valstart = DateTime.Now.Date;
                                DateTime valend = DateTime.Now.Date;

                                clientData = Integration.GetClientInfoInv(id);
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
                                    feeData = Integration.GetFeeInfo(Convert.ToInt32(invoiceId));
                                    ftype = feeData[0].ToString();
                                    notes = feeData[1].ToString();

                                    Log.Save("Invoice Id: " + invoiceId.ToString());
                                    Log.Save("Customer Id: " + customerId);
                                    Log.Save("Client Name: " + companyName);
                                    Log.Save("Receipt Amount: " + pinfo.Debit);

                                    prepstat = "No";
                                    valstart = Integration.GetValidity(Convert.ToInt32(invoiceId));
                                    valend = Integration.GetValidityEnd(Convert.ToInt32(invoiceId));
                                }
                                else
                                {
                                    prepstat = "Yes";
                                    Log.Save("Prepayment");
                                    Log.Save("Customer Id: " + customerId);

                                    var gl = Integration.GetCreditGlID((transid + 1).ToString());

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
                                    dt = Translate(customerId, ftype, companyName, debit, notes, "PREPAYMENT", Integration.GetFreqUsage(Convert.ToInt32(invoiceId)));
                                }
                                else
                                {
                                    dt = Translate(customerId, ftype, companyName, debit, notes, "", Integration.GetFreqUsage(Convert.ToInt32(invoiceId)));
                                }

                                bool cusexists;
                                cusexists = CustomerExists(dt.customerId);
                                if (Convert.ToInt32(invoiceId) == 0)
                                {
                                    if (!cusexists)
                                    {
                                        CreateCustomer(dt.customerId, companyName);
                                        Integration.StoreCustomer(dt.customerId, companyName);
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
                                                string reference = Integration.GetCurrentRef("FGBJMREC");
                                                Log.Save("Target Batch: " + Integration.GetRecieptBatchId("FGBJMREC"));
                                                Log.Save("Transferring Receipt");

                                                ReceiptTransfer(Integration.GetRecieptBatchId("FGBJMREC"), dt.customerId, dt.debit, dt.companyName, reference, invoiceId, paymentDate, dt.desc, customerId, valstart, valend);
                                                Integration.UpdateBatchCountPayment(Integration.GetRecieptBatchId("FGBJMREC"));
                                                Integration.UpdateReceiptNumber(receipt, Integration.GetCurrentRef("FGBJMREC"));
                                                Integration.IncrementReferenceNumber(Integration.GetBankCodeId("FGBJMREC"), Convert.ToDecimal(dt.debit));
                                                Integration.StorePayment(dt.customerId, companyName, DateTime.Now, invoiceId, Convert.ToDecimal(dt.debit), 0, prepstat, Convert.ToInt32(reference), Convert.ToInt32(glid), "No", 0);
                                            }
                                            else
                                            {
                                                string reference = Integration.GetCurrentRef("FGBJMREC");
                                                CreateReceiptBatchEx("FGBJMREC", "Middleware Generated Batch for FGBJMREC");
                                                Integration.OpenNewReceiptBatch(1, GetLastPaymentBatch(), "FGBJMREC");

                                                Log.Save("Target Batch: " + Integration.GetRecieptBatchId("FGBJMREC"));
                                                Log.Save("Transferring Receipt");

                                                ReceiptTransfer(Integration.GetRecieptBatchId("FGBJMREC"), dt.customerId, dt.debit, dt.companyName, reference, invoiceId, paymentDate, dt.desc, customerId, valstart, valend);
                                                Integration.UpdateBatchCountPayment(Integration.GetRecieptBatchId("FGBJMREC"));
                                                Integration.UpdateReceiptNumber(receipt, Integration.GetCurrentRef("FGBJMREC"));
                                                Integration.IncrementReferenceNumber(Integration.GetBankCodeId("FGBJMREC"), Convert.ToDecimal(dt.debit));
                                                Integration.StorePayment(dt.customerId, companyName, DateTime.Now, invoiceId, Convert.ToDecimal(dt.debit), 0, prepstat, Convert.ToInt32(reference), Convert.ToInt32(glid), "No", 0);
                                            }
                                        }
                                    }
                                    else if (glid == "5147")
                                    {
                                        Log.Save("Bank: FGB US$ SAVINGS A/C");
                                        decimal usamount = 0;
                                        decimal transferedAmt = Convert.ToDecimal(dt.debit) / Integration.GetUsRateByInvoice(Convert.ToInt32(invoiceId));
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

                                        if (prepstat == "Yes" && clientIdPrefix == Integration.GetClientIdZRecord(true))
                                        {
                                            dt.customerId = clientIdPrefix + "-T";
                                            currentRate = Integration.GetRate();
                                        }

                                        if (dt.customerId[6] == 'T')
                                        {
                                            usamount = Math.Round(Convert.ToDecimal(dt.debit) / Integration.GetUsRateByInvoice(Convert.ToInt32(invoiceId)), 2);
                                            Integration.ModifyInvoiceList(0, Integration.GetUsRateByInvoice(Convert.ToInt32(invoiceId)), dt.customerId);
                                            currentRate = Integration.GetRate();
                                        }

                                        if (dt.success)
                                        {
                                            if (ReceiptBatchAvail("FGBUSMRC"))
                                            {
                                                string reference = Integration.GetCurrentRef("FGBUSMRC");
                                                Log.Save("Target Batch: " + Integration.GetRecieptBatchId("FGBUSMRC"));
                                                Log.Save("Transferring Receipt");

                                                ReceiptTransfer(Integration.GetRecieptBatchId("FGBUSMRC"), dt.customerId, Math.Round(transferedAmt, 2).ToString(), dt.companyName, reference, invoiceId, paymentDate, dt.desc, customerId, valstart, valend);
                                                Integration.UpdateBatchCountPayment(Integration.GetRecieptBatchId("FGBUSMRC"));
                                                Integration.UpdateReceiptNumber(receipt, Integration.GetCurrentRef("FGBUSMRC"));
                                                Integration.IncrementReferenceNumber(Integration.GetBankCodeId("FGBUSMRC"), Convert.ToDecimal(dt.debit));
                                                Integration.StorePayment(dt.customerId, companyName, DateTime.Now, invoiceId, Convert.ToDecimal(dt.debit), usamount, prepstat, Convert.ToInt32(reference), Convert.ToInt32(glid), "No", currentRate);
                                            }
                                            else
                                            {
                                                string reference = Integration.GetCurrentRef("FGBUSMRC");
                                                CreateReceiptBatchEx("FGBUSMRC", "Middleware Generated Batch for FGBUSMRC");
                                                Integration.OpenNewReceiptBatch(1, GetLastPaymentBatch(), "FGBUSMRC");

                                                Log.Save("Target Batch: " + Integration.GetRecieptBatchId("FGBUSMRC"));
                                                Log.Save("Transferring Receipt");

                                                ReceiptTransfer(Integration.GetRecieptBatchId("FGBUSMRC"), dt.customerId, Math.Round(transferedAmt, 2).ToString(), dt.companyName, reference, invoiceId, paymentDate, dt.desc, customerId, valstart, valend);
                                                Integration.UpdateBatchCountPayment(Integration.GetRecieptBatchId("FGBUSMRC"));
                                                Integration.UpdateReceiptNumber(receipt, Integration.GetCurrentRef("FGBUSMRC"));
                                                Integration.IncrementReferenceNumber(Integration.GetBankCodeId("FGBUSMRC"), Convert.ToDecimal(dt.debit));
                                                Integration.StorePayment(dt.customerId, companyName, DateTime.Now, invoiceId, Convert.ToDecimal(dt.debit), usamount, prepstat, Convert.ToInt32(reference), Convert.ToInt32(glid), "No", currentRate);
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
                                                string reference = Integration.GetCurrentRef("NCBJMREC");
                                                Log.Save("Target Batch: " + Integration.GetRecieptBatchId("NCBJMREC"));
                                                Log.Save("Transferring Receipt");

                                                ReceiptTransfer(Integration.GetRecieptBatchId("NCBJMREC"), dt.customerId, dt.debit, dt.companyName, reference, invoiceId, paymentDate, dt.desc, customerId, valstart, valend);
                                                Integration.UpdateBatchCountPayment(Integration.GetRecieptBatchId("NCBJMREC"));
                                                Integration.UpdateReceiptNumber(receipt, Integration.GetCurrentRef("NCBJMREC"));
                                                Integration.IncrementReferenceNumber(Integration.GetBankCodeId("NCBJMREC"), Convert.ToDecimal(dt.debit));
                                                Integration.StorePayment(dt.customerId, companyName, DateTime.Now, invoiceId, Convert.ToDecimal(dt.debit), 0, prepstat, Convert.ToInt32(reference), Convert.ToInt32(glid), "No", 1);
                                            }
                                            else
                                            {
                                                string reference = Integration.GetCurrentRef("NCBJMREC");
                                                CreateReceiptBatchEx("NCBJMREC", "Middleware Generated Batch for NCBJMREC");
                                                Integration.OpenNewReceiptBatch(1, GetLastPaymentBatch(), "NCBJMREC");

                                                Log.Save("Target Batch: " + Integration.GetRecieptBatchId("NCBJMREC"));
                                                Log.Save("Transferring Receipt");

                                                ReceiptTransfer(Integration.GetRecieptBatchId("NCBJMREC"), dt.customerId, dt.debit, dt.companyName, reference, invoiceId, paymentDate, dt.desc, customerId, valstart, valend);
                                                Integration.UpdateBatchCountPayment(Integration.GetRecieptBatchId("NCBJMREC"));
                                                Integration.UpdateReceiptNumber(receipt, Integration.GetCurrentRef("NCBJMREC"));
                                                Integration.IncrementReferenceNumber(Integration.GetBankCodeId("NCBJMREC"), Convert.ToDecimal(dt.debit));
                                                Integration.StorePayment(dt.customerId, companyName, DateTime.Now, invoiceId, Convert.ToDecimal(dt.debit), 0, prepstat, Convert.ToInt32(reference), Convert.ToInt32(glid), "No", 1);
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
                                    creditNote = Integration.GetCreditNoteInfo(docInfo.OriginalDocumentID, docInfo.DocumentID);
                                    Thread.Sleep(1000);
                                }

                                List<string> clientInfo = new List<string>(4);
                                clientInfo = Integration.GetClientInfoInv(creditNote.CustomerID.ToString());
                                var accountNum = Integration.GetAccountNumber(creditNote.CreditGL);
                                DateTime invoiceValidity = Integration.GetValidity(creditNote.ARInvoiceID);

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

                                Data dt = Translate(cNum, creditNote.FeeType, companyName, creditNote.amount.ToString(), creditNote.notes, accountNum, Integration.GetFreqUsage(creditNote.ARInvoiceID));

                                if (CheckAccpacInvoiceAvail(creditNote.ARInvoiceID))
                                {
                                    cred_docNum = Integration.GetCreditMemoNumber();
                                    Log.Save("Creating credit memo");
                                    int batchNumber = GetBatch(CREDIT_NOTE, creditNote.ARInvoiceID.ToString());

                                    Integration.StoreInvoice(creditNote.ARInvoiceID, batchNumber, creditNote.CreditGL, companyName, dt.customerId, DateTime.Now, "", creditNote.amount, "no modification", 1, 0, 0, 1, cred_docNum);
                                    CreditNoteInsert(batchNumber.ToString(), dt.customerId, accountNum, creditNote.amount.ToString(), creditNote.ARInvoiceID.ToString(), cred_docNum.ToString(), creditNoteDesc);
                                    Integration.UpdateAsmsCreditMemoNumber(docInfo.DocumentID, cred_docNum);
                                }
                                else
                                {
                                    Log.Save("The Credit Memo was not created. The Invoice does not exist.");
                                    cred_docNum = Integration.GetCreditMemoNumber();
                                    Integration.UpdateAsmsCreditMemoNumber(docInfo.DocumentID, cred_docNum);
                                    Log.Save("The Credit Memo number in ASMS updated.");
                                }
                            }
                            else if (docInfo.DocumentType == RECEIPT && docInfo.PaymentMethod == 99)
                            {
                                Log.Save("Payment By Credit");
                                PaymentInfo pinfo = Integration.GetReceiptInfo(docInfo.OriginalDocumentID);
                                List<string> clientData = Integration.GetClientInfoInv(pinfo.CustomerID.ToString());
                                List<string> feeData = new List<string>(3);

                                var companyName = clientData[0].ToString();
                                var customerId = clientData[1].ToString();
                                var fname = clientData[2].ToString();
                                var lname = clientData[3].ToString();

                                if (companyName == "" || companyName == " " || companyName == null)
                                {
                                    companyName = fname + " " + lname;
                                }

                                feeData = Integration.GetFeeInfo(pinfo.InvoiceID);
                                var ftype = feeData[0].ToString();
                                var notes = feeData[1].ToString();

                                Data dt = Translate(customerId, ftype, companyName, pinfo.Debit.ToString(), notes, "", Integration.GetFreqUsage(pinfo.InvoiceID).ToString());
                                PrepaymentData pData = Integration.CheckPrepaymentAvail(dt.customerId);
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
                                                        pData = Integration.CheckPrepaymentAvail(dt.customerId);
                                                        PayByCredit(dt.customerId, pinfo.InvoiceID.ToString(), Integration.GetRecieptBatchId("FGBJMREC"), GetDocNumber(pData.referenceNumber));
                                                        if (reducingAmt > pData.remainder) Integration.AdjustPrepaymentRemainder(pData.remainder, pData.sequenceNumber);
                                                        else Integration.AdjustPrepaymentRemainder(reducingAmt, pData.sequenceNumber);
                                                        reducingAmt = reducingAmt - pData.remainder;
                                                    }
                                                    Integration.StorePayment(dt.customerId, companyName, DateTime.Now, pinfo.InvoiceID.ToString(), pinfo.Debit, 0, "No", 0, Convert.ToInt32(glid), "Yes", 1);
                                                    Log.Save("Payment by credit transaction complete");
                                                }
                                                else
                                                {
                                                    CreateReceiptBatchEx("FGBJMREC", "Middleware Generated Batch for FGBJMREC");
                                                    Integration.OpenNewReceiptBatch(1, GetLastPaymentBatch(), "FGBJMREC");
                                                    Log.Save("Target Batch: " + Integration.GetRecieptBatchId("FGBJMREC"));
                                                    while (reducingAmt > 0)
                                                    {
                                                        pData = Integration.CheckPrepaymentAvail(dt.customerId);
                                                        PayByCredit(dt.customerId, pinfo.InvoiceID.ToString(), Integration.GetRecieptBatchId("FGBJMREC"), GetDocNumber(pData.referenceNumber));
                                                        if (reducingAmt > pData.remainder) Integration.AdjustPrepaymentRemainder(pData.remainder, pData.sequenceNumber);
                                                        else Integration.AdjustPrepaymentRemainder(reducingAmt, pData.sequenceNumber);
                                                        reducingAmt = reducingAmt - pData.remainder;
                                                    }
                                                    Integration.StorePayment(dt.customerId, companyName, DateTime.Now, pinfo.InvoiceID.ToString(), pinfo.Debit, 0, "No", 0, Convert.ToInt32(glid), "Yes", 1);
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
                                            decimal usRate = Integration.GetUsRateByInvoice(pinfo.InvoiceID);
                                            decimal usAmount = Convert.ToDecimal(dt.debit) / usRate;
                                            if (pData.totalPrepaymentRemainder >= usAmount)
                                            {
                                                Log.Save("Carrying out Payment By Credit Transaction");
                                                decimal reducingAmt = usAmount;

                                                if (ReceiptBatchAvail("FGBUSMRC"))
                                                {
                                                    while (reducingAmt > 0)
                                                    {
                                                        pData = Integration.CheckPrepaymentAvail(dt.customerId);
                                                        PayByCredit(dt.customerId, pinfo.InvoiceID.ToString(), Integration.GetRecieptBatchId("FGBUSMRC"), GetDocNumber(pData.referenceNumber));
                                                        if (reducingAmt > pData.remainder) Integration.AdjustPrepaymentRemainder(pData.remainder, pData.sequenceNumber);
                                                        else Integration.AdjustPrepaymentRemainder(reducingAmt, pData.sequenceNumber);
                                                        reducingAmt = reducingAmt - pData.remainder;
                                                    }
                                                    if (usRate == 1) Integration.StorePayment(dt.customerId, companyName, DateTime.Now, pinfo.InvoiceID.ToString(), pinfo.Debit, 0, "No", 0, Convert.ToInt32(glid), "Yes", 1);
                                                    else Integration.StorePayment(dt.customerId, companyName, DateTime.Now, pinfo.InvoiceID.ToString(), pinfo.Debit, usAmount, "No", 0, Convert.ToInt32(glid), "Yes", 1);
                                                    Log.Save("Payment by credit transaction complete");
                                                }
                                                else
                                                {
                                                    CreateReceiptBatchEx("FGBUSMRC", "Middleware Generated Batch for FGBUSMRC");
                                                    Integration.OpenNewReceiptBatch(1, GetLastPaymentBatch(), "FGBUSMRC");
                                                    Log.Save("Target Batch: " + Integration.GetRecieptBatchId("FGBUSMRC"));

                                                    while (reducingAmt > 0)
                                                    {
                                                        pData = Integration.CheckPrepaymentAvail(dt.customerId);
                                                        PayByCredit(dt.customerId, pinfo.InvoiceID.ToString(), Integration.GetRecieptBatchId("FGBUSMRC"), GetDocNumber(pData.referenceNumber));
                                                        if (reducingAmt > pData.remainder) Integration.AdjustPrepaymentRemainder(pData.remainder, pData.sequenceNumber);
                                                        else Integration.AdjustPrepaymentRemainder(reducingAmt, pData.sequenceNumber);
                                                        reducingAmt = reducingAmt - pData.remainder;
                                                    }

                                                    if (usRate == 1)
                                                    {
                                                        Integration.StorePayment(dt.customerId, companyName, DateTime.Now, pinfo.InvoiceID.ToString(), pinfo.Debit, 0, "No", 0, Convert.ToInt32(glid), "Yes", 1);
                                                    }
                                                    else
                                                    {
                                                        Integration.StorePayment(dt.customerId, companyName, DateTime.Now, pinfo.InvoiceID.ToString(), pinfo.Debit, usAmount, "No", 0, Convert.ToInt32(glid), "Yes", 1);

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
                                                        pData = Integration.CheckPrepaymentAvail(dt.customerId);
                                                        PayByCredit(dt.customerId, pinfo.InvoiceID.ToString(), Integration.GetRecieptBatchId("NCBJMREC"), GetDocNumber(pData.referenceNumber));
                                                        if (reducingAmt > pData.remainder) Integration.AdjustPrepaymentRemainder(pData.remainder, pData.sequenceNumber);
                                                        else Integration.AdjustPrepaymentRemainder(reducingAmt, pData.sequenceNumber);
                                                        reducingAmt = reducingAmt - pData.remainder;
                                                    }
                                                    Integration.StorePayment(dt.customerId, companyName, DateTime.Now, pinfo.InvoiceID.ToString(), pinfo.Debit, 0, "No", 0, Convert.ToInt32(glid), "Yes", 1);
                                                    Log.Save("Payment by credit transaction complete");
                                                }
                                                else
                                                {
                                                    CreateReceiptBatchEx("NCBJMREC", "Middleware Generated Batch for NCBJMREC");
                                                    Integration.OpenNewReceiptBatch(1, GetLastPaymentBatch(), "NCBJMREC");
                                                    Log.Save("Target Batch: " + Integration.GetRecieptBatchId("NCBJMREC"));
                                                    while (reducingAmt > 0)
                                                    {
                                                        pData = Integration.CheckPrepaymentAvail(dt.customerId);
                                                        PayByCredit(dt.customerId, pinfo.InvoiceID.ToString(), Integration.GetRecieptBatchId("NCBJMREC"), GetDocNumber(pData.referenceNumber));
                                                        if (reducingAmt > pData.remainder) Integration.AdjustPrepaymentRemainder(pData.remainder, pData.sequenceNumber);
                                                        else Integration.AdjustPrepaymentRemainder(reducingAmt, pData.sequenceNumber);
                                                        reducingAmt = reducingAmt - pData.remainder;
                                                    }
                                                    Integration.StorePayment(dt.customerId, companyName, DateTime.Now, pinfo.InvoiceID.ToString(), pinfo.Debit, 0, "No", 0, Convert.ToInt32(glid), "Yes", 1);
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

                }
                else
                {
                    Log.Save("Accpac Version: " + accpacSession.AppVersion);
                    Log.Save("Company: " + accpacSession.CompanyName);
                    Log.Save("Session Status: " + accpacSession.IsOpened);
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

        private void TableDependencyArInvoices_OnChanged(object sender, RecordChangedEventArgs<SqlNotify_ArInvoices> e)
        {
            Thread.Sleep(150);
            if (!Constants.IGNORE_EVENTS)
            {
                if (IsAccpacSessionOpen())
                {
                    try
                    {
                        if (Constants.PARALLEL_RUN)
                        {
                            if (e.ChangeType == ChangeType.Insert)
                            {
                                e.Entity.ARInvoicesToGenric();
                            }
                        }

                        var values = e.Entity;
                        var customerId = values.CustomerID;
                        var invoiceId = values.ARInvoiceID;
                        var amount = values.Amount;
                        var feeType = values.FeeType;
                        var notes = values.notes;
                        var cancelledBy = values.canceledBy;

                        if (cancelledBy != null)
                        {
                            string freqUsage = Integration.GetFreqUsage(invoiceId);
                            DateTime invoiceValidity = Integration.GetValidity(invoiceId);
                            var creditGl = Integration.GetCreditGl(invoiceId.ToString());
                            var accountNum = Integration.GetAccountNumber(creditGl);
                            List<string> clientInfo = new List<string>(4);
                            clientInfo = Integration.GetClientInfoInv(customerId.ToString());
                            Integration.GetClientInfoInv(customerId.ToString());

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
                                Log.Save("Cancellation request for invoice: " + invoiceId);
                                if (CheckAccpacInvoiceAvail(invoiceId))
                                {
                                    int postedBatch = GetIbatchNumber(invoiceId);
                                    if (!InvoiceDelete(invoiceId))
                                    {
                                        Log.Save("Creating a credit memo");
                                        int cred_docNum = Integration.GetCreditMemoNumber();
                                        int batchNumber = GetBatch(CREDIT_NOTE, invoiceId.ToString());
                                        Integration.StoreInvoice(invoiceId, batchNumber, creditGl, companyName, dt.customerId, DateTime.Now, cancelledBy, amount, "no modification", 1, 0, 0, 1, cred_docNum);
                                        CreditNoteInsert(batchNumber.ToString(), dt.customerId, accountNum, amount.ToString(), invoiceId.ToString(), cred_docNum.ToString(), creditNoteDesc);
                                    }
                                    else
                                    {
                                        Maj m = new Maj();
                                        int r = Integration.GetInvoiceReference(invoiceId);

                                        if (r != -1)
                                        {
                                            m = Integration.GetMajDetail(r);
                                        }

                                        if (dt.feeType == "SLF" && notes == "Renewal")
                                        {
                                            Integration.StoreInvoice(Convert.ToInt32(invoiceId), postedBatch, creditGl, companyName, dt.customerId, DateTime.Now, cancelledBy, Convert.ToDecimal(amount), "no modification", 1, 0, 1, 0, 0);
                                        }
                                        else if (dt.feeType == "RF" && notes == "Renewal")
                                        {
                                            Integration.StoreInvoice(Convert.ToInt32(invoiceId), postedBatch, creditGl, companyName, dt.customerId, DateTime.Now, cancelledBy, Convert.ToDecimal(amount), "no modification", 1, 0, 1, 0, 0);
                                        }
                                        else if ((notes == "Annual Fee" && m.stationType == "SSL" && m.certificateType == 0 && m.proj == "JMC") || (freqUsage == "PRS55"))
                                        {
                                            Integration.StoreInvoice(Convert.ToInt32(invoiceId), postedBatch, creditGl, companyName, dt.customerId, DateTime.Now, cancelledBy, Convert.ToDecimal(amount), "no modification", 1, 0, 1, 0, 0);
                                        }
                                        else if (notes == "Type Approval" || freqUsage == "TA-ProAmend")
                                        {
                                            Integration.StoreInvoice(Convert.ToInt32(invoiceId), postedBatch, creditGl, companyName, dt.customerId, DateTime.Now, cancelledBy, Convert.ToDecimal(amount), "no modification", Integration.GetRate(), ChangeToUS(Convert.ToDecimal(amount)), 1, 0, 0);
                                        }
                                        else if (notes == "Annual Fee" || notes == "Modification" || notes == "Radio Operator")
                                        {
                                            Integration.StoreInvoice(Convert.ToInt32(invoiceId), postedBatch, creditGl, companyName, dt.customerId, DateTime.Now, cancelledBy, Convert.ToDecimal(amount), "no modification", 1, 0, 1, 0, 0);
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
        }

        InsertionReturn InvBatchInsert(string idCust, string docNum, string desc, string feeCode, string amt, string batchId)
        {
            b1_arInvoiceBatch = dbLink.OpenView("AR0031");
            b1_arInvoiceHeader = dbLink.OpenView("AR0032");
            b1_arInvoiceDetail = dbLink.OpenView("AR0033");
            b1_arInvoicePaymentSchedules = dbLink.OpenView("AR0034");
            b1_arInvoiceHeaderOptFields = dbLink.OpenView("AR0402");
            b1_arInvoiceDetailOptFields = dbLink.OpenView("AR0401");

            try
            {
                Log.Save("Transfering Invoice " + docNum + " to batch: " + batchId);
                InsertionReturn success = new InsertionReturn();
                DateTime postDate = Integration.GetValidity(Convert.ToInt32(docNum));

                if (postDate < DateTime.Now)
                    postDate = DateTime.Now;

                bool gotOne;
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
                }
                else
                {
                    success.status = "Not Exist";
                    Log.Save("Customer does not exist.");
                }

                b1_arInvoiceBatch.Dispose();
                b1_arInvoiceDetail.Dispose();
                b1_arInvoiceDetailOptFields.Dispose();
                b1_arInvoiceHeader.Dispose();
                b1_arInvoiceHeaderOptFields.Dispose();
                b1_arInvoicePaymentSchedules.Dispose();
                Log.Save("Invoice Id: " + docNum + " Transferred");
                return success;
            }
            catch (Exception e)
            {
                b1_arInvoiceBatch.Dispose();
                b1_arInvoiceDetail.Dispose();
                b1_arInvoiceDetailOptFields.Dispose();
                b1_arInvoiceHeader.Dispose();
                b1_arInvoiceHeaderOptFields.Dispose();
                b1_arInvoicePaymentSchedules.Dispose();
                throw (e);
            }
        }

        bool CustomerExists(string idCust)
        {
            View cssql = dbLink.OpenView("CS0120");
            try
            {
                bool exist = false;
                cssql.Browse("SELECT IDCUST FROM ARCUS WHERE IDCUST = '" + idCust + "'", true);
                cssql.InternalSet(256);

                if (cssql.GoNext())
                {
                    exist = true;
                }

                cssql.Dispose();
                return exist;
            }
            catch (Exception e)
            {
                cssql.Dispose();
                throw (e);
            }
        }

        Data Translate(string cNum, string feeType, string companyName, string debit, string notes, string _fcode, string FreqUsage)
        {
            string temp = "";
            string iv_customerId;
            Data dt = new Data();

            if (_fcode == "PREPAYMENT" && Integration.GetClientIdZRecord(false).Contains("-T"))
            {
                dt.customerId = Integration.GetClientIdZRecord(false);
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
            View ARCUSTOMER1header = dbLink.OpenView("AR0024");
            View ARCUSTOMER1detail = dbLink.OpenView("AR0400");
            View ARCUSTSTAT2 = dbLink.OpenView("AR0022");
            View ARCUSTCMT3 = dbLink.OpenView("AR0021");

            try
            {
                Log.Save("Creating customer: " + nameCust + ", ID: " + idCust);
                string groupCode = "";

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
                Integration.UpdateCustomerCount();
                Integration.StoreCustomer(idCust, nameCust);
                Log.Save("Customer created.");

                ARCUSTOMER1header.Dispose();
                ARCUSTOMER1detail.Dispose();
                ARCUSTSTAT2.Dispose();
                ARCUSTCMT3.Dispose();
            }
            catch (Exception e)
            {
                ARCUSTOMER1header.Dispose();
                ARCUSTOMER1detail.Dispose();
                ARCUSTSTAT2.Dispose();
                ARCUSTCMT3.Dispose();
                throw (e);
            }
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
            DateTime val = Integration.GetValidity(Convert.ToInt32(invoiceid));
            string renspec = RENEWAL_SPEC + val.ToString("MMMM") + " " + val.Year.ToString();
            string renreg = RENEWAL_REG + val.ToString("MMMM") + " " + val.Year.ToString();

            if (Integration.BatchAvail(batchType))
            {
                int batch = Integration.GetAvailBatch(batchType);
                if (!Integration.IsBatchExpired(batch))
                {
                    if (!CheckAccpacIBatchPosted(batch))
                    {
                        return batch;
                    }
                    else
                    {
                        Integration.CloseInvoiceBatch();
                        int newbatch = GetLastInvoiceBatch() + 1;

                        if (batchType == renreg)
                        {
                            Integration.CreateInvoiceBatch(GenerateDaysExpire(batchType), newbatch, batchType, "Regulatory");
                        }
                        else if (batchType == renspec)
                        {
                            Integration.CreateInvoiceBatch(GenerateDaysExpire(batchType), newbatch, batchType, "Spectrum");
                        }
                        else
                        {
                            Integration.CreateInvoiceBatch(GenerateDaysExpire(batchType), newbatch, batchType, "");
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
                        Integration.ResetInvoiceTotal();
                    }

                    Integration.CloseInvoiceBatch();
                    int newbatch = GetLastInvoiceBatch() + 1;

                    if (batchType == renreg)
                    {
                        Integration.CreateInvoiceBatch(GenerateDaysExpire(batchType), newbatch, batchType, "Regulatory");
                    }
                    else if (batchType == renspec)
                    {
                        Integration.CreateInvoiceBatch(GenerateDaysExpire(batchType), newbatch, batchType, "Spectrum");
                    }
                    else
                    {
                        Integration.CreateInvoiceBatch(GenerateDaysExpire(batchType), newbatch, batchType, "");
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
                    Integration.CreateInvoiceBatch(GenerateDaysExpire(batchType), newbatch, batchType, "Regulatory");
                }

                else if (batchType == renspec)
                {
                    Integration.CreateInvoiceBatch(GenerateDaysExpire(batchType), newbatch, batchType, "Spectrum");
                }

                else
                {
                    Integration.CreateInvoiceBatch(GenerateDaysExpire(batchType), newbatch, batchType, "");
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
            try
            {
                cssql.Browse("SELECT BTCHSTTS FROM ARIBC WHERE CNTBTCH = '" + batchNumber.ToString() + "'", true);
                cssql.InternalSet(256);

                if (cssql.GoNext())
                {
                    string val = Convert.ToString(cssql.Fields.FieldByName("BTCHSTTS").Value);
                    if (val == "1")
                    {
                        cssql.Dispose();
                        return false;
                    }
                    else
                    {
                        cssql.Dispose();
                        return true;
                    }
                }
                cssql.Dispose();
                return true;
            }
            catch (Exception e)
            {
                cssql.Dispose();
                throw (e);
            }
        }

        int GetLastInvoiceBatch()
        {
            b1_arInvoiceBatch = dbLink.OpenView("AR0031");
            try
            {
                int BatchId;
                b1_arInvoiceBatch.GoBottom();
                BatchId = Convert.ToInt32(b1_arInvoiceBatch.Fields.FieldByName("CNTBTCH").Value);
                b1_arInvoiceBatch.Dispose();
                return BatchId;
            }
            catch (Exception e)
            {
                b1_arInvoiceBatch.Dispose();
                throw (e);
            }
        }

        void CreateInvoiceBatch(string description)
        {
            b1_arInvoiceBatch = dbLink.OpenView("AR0031");
            b1_arInvoiceHeader = dbLink.OpenView("AR0032");
            b1_arInvoiceDetail = dbLink.OpenView("AR0033");
            b1_arInvoicePaymentSchedules = dbLink.OpenView("AR0034");
            b1_arInvoiceHeaderOptFields = dbLink.OpenView("AR0402");
            b1_arInvoiceDetailOptFields = dbLink.OpenView("AR0401");

            try
            {
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
            catch (Exception e)
            {
                b1_arInvoiceBatch.Dispose();
                b1_arInvoiceDetail.Dispose();
                b1_arInvoiceDetailOptFields.Dispose();
                b1_arInvoiceHeader.Dispose();
                b1_arInvoiceHeaderOptFields.Dispose();
                b1_arInvoicePaymentSchedules.Dispose();
                throw (e);
            }
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
            View cssql = dbLink.OpenView("CS0120");
            try
            {
                string fiscYear = "";
                cssql.Browse("SELECT FSCYEAR FROM CSFSC WHERE FSCYEAR = '" + finyear.ToString() + "'", true);
                cssql.InternalSet(256);

                if (cssql.GoNext())
                {
                    fiscYear = Convert.ToString(cssql.Fields.FieldByName("FSCYEAR").Value);
                }
                cssql.Dispose();

                if (fiscYear != "")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                cssql.Dispose();
                throw (e);
            }
        }

        public decimal ChangeToUS(decimal regamt)
        {
            decimal rate = Integration.GetRate();
            decimal usamt = regamt / rate;
            return Math.Round(usamt, 2);
        }

        public decimal ChangeToUSupdated(decimal regamt, int invnum)
        {
            decimal rate = Integration.GetUsRateByInvoice(invnum);
            decimal usamt = regamt / rate;
            return Math.Round(usamt, 2);
        }

        public int GetIbatchNumber(int docNumber)
        {
            View cssql = dbLink.OpenView("CS0120");
            int batchNum = -1;

            try
            {
                cssql.Browse("SELECT CNTBTCH FROM ARIBH WHERE IDINVC = '" + docNumber.ToString() + "'", true);
                cssql.InternalSet(256);

                if (cssql.GoNext())
                {
                    batchNum = Convert.ToInt32(cssql.Fields.FieldByName("CNTBTCH").Value);
                }
                cssql.Dispose();
                return batchNum;
            }
            catch (Exception e)
            {
                cssql.Dispose();
                throw (e);
            }
        }

        void UpdateInvoice(string accountNumber, string amt, string BatchId, string entryNumber)
        {
            b1_arInvoiceBatch = dbLink.OpenView("AR0031");
            b1_arInvoiceHeader = dbLink.OpenView("AR0032");
            b1_arInvoiceDetail = dbLink.OpenView("AR0033");
            b1_arInvoicePaymentSchedules = dbLink.OpenView("AR0034");
            b1_arInvoiceHeaderOptFields = dbLink.OpenView("AR0402");
            b1_arInvoiceDetailOptFields = dbLink.OpenView("AR0401");

            try
            {
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

                b1_arInvoiceBatch.Dispose();
                b1_arInvoiceHeader.Dispose();
                b1_arInvoiceDetail.Dispose();
                b1_arInvoicePaymentSchedules.Dispose();
                b1_arInvoiceHeaderOptFields.Dispose();
                b1_arInvoiceDetailOptFields.Dispose();
            }
            catch (Exception e)
            {
                b1_arInvoiceBatch.Dispose();
                b1_arInvoiceHeader.Dispose();
                b1_arInvoiceDetail.Dispose();
                b1_arInvoicePaymentSchedules.Dispose();
                b1_arInvoiceHeaderOptFields.Dispose();
                b1_arInvoiceDetailOptFields.Dispose();
                throw (e);
            }
        }

        public int GetEntryNumber(int docNumber)
        {
            b1_arInvoiceHeader = dbLink.OpenView("AR0032");

            try
            {
                int entry = -1;
                string docNum = docNumber.ToString();

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

                b1_arInvoiceHeader.Dispose();
                return entry;
            }
            catch (Exception e)
            {
                b1_arInvoiceHeader.Dispose();
                throw (e);
            }
        }

        bool IsEmpty(DataSet dataSet)
        {
            foreach (DataTable table in dataSet.Tables)
                if (table.Rows.Count != 0) return false;

            return true;
        }

        public bool ReceiptBatchAvail(string bankcode)
        {
            var batch = Integration.GetReceiptBatchDetail(bankcode);
            if (batch != null)
            {
                if (DateTime.Now < batch.expiryDate)
                {
                    if (!CheckAccpacRBatchPosted(batch.batchId))
                    {
                        return true;
                    }
                    else
                    {
                        Integration.CloseReceiptBatch(batch.batchId);
                        return false;
                    }
                }
                else
                {
                    Integration.CloseReceiptBatch(batch.batchId);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool CheckAccpacRBatchPosted(int batchNumber)
        {
            View cssql = dbLink.OpenView("CS0120"); ;
            try
            {
                cssql.Browse("SELECT CNTBTCH, BATCHSTAT FROM ARBTA WHERE CNTBTCH = '" + batchNumber.ToString() + "'", true);
                cssql.InternalSet(256);

                if (cssql.GoNext())
                {
                    string val = Convert.ToString(cssql.Fields.FieldByName("BATCHSTAT").Value);
                    if (val == "1")
                    {
                        cssql.Dispose();
                        return false;
                    }
                    else
                    {
                        cssql.Dispose();
                        return true;
                    }
                }
                cssql.Dispose();
                return true;
            }
            catch (Exception e)
            {
                cssql.Dispose();
                throw (e);
            }
        }

        public bool ReceiptTransfer(string batchNumber, string customerId, string amount, string receiptDescription, string referenceNumber, string invnum, DateTime paymentDate, string findesc, string cid, DateTime valstart, DateTime valend)
        {
            string notes = Integration.IsAnnualFee(Convert.ToInt32(invnum));
            string receiptDescriptionEx;

            CBBTCH1batch = dbLink.OpenView("AR0041");
            CBBTCH1header = dbLink.OpenView("AR0042");
            CBBTCH1detail1 = dbLink.OpenView("AR0044");
            CBBTCH1detail2 = dbLink.OpenView("AR0045");
            CBBTCH1detail3 = dbLink.OpenView("AR0043");
            CBBTCH1detail4 = dbLink.OpenView("AR0061");
            CBBTCH1detail5 = dbLink.OpenView("AR0406");
            CBBTCH1detail6 = dbLink.OpenView("AR0170");

            arRecptBatch = dbLink.OpenView("AR0041");
            arRecptHeader = dbLink.OpenView("AR0042");
            arRecptDetail1 = dbLink.OpenView("AR0044");
            arRecptDetail2 = dbLink.OpenView("AR0045");
            arRecptDetail3 = dbLink.OpenView("AR0043");
            arRecptDetail4 = dbLink.OpenView("AR0061");
            arRecptDetail5 = dbLink.OpenView("AR0406");
            arRecptDetail6 = dbLink.OpenView("AR0170");

            try
            {
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

                        bool flagInsert;
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

                        CBBTCH1batch.Dispose();
                        CBBTCH1header.Dispose();
                        CBBTCH1detail1.Dispose();
                        CBBTCH1detail2.Dispose();
                        CBBTCH1detail3.Dispose();
                        CBBTCH1detail4.Dispose();
                        CBBTCH1detail5.Dispose();
                        CBBTCH1detail6.Dispose();
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                arRecptBatch.Dispose();
                arRecptHeader.Dispose();
                arRecptDetail1.Dispose();
                arRecptDetail2.Dispose();
                arRecptDetail3.Dispose();
                arRecptDetail4.Dispose();
                arRecptDetail5.Dispose();
                arRecptDetail6.Dispose();

                CBBTCH1batch.Dispose();
                CBBTCH1header.Dispose();
                CBBTCH1detail1.Dispose();
                CBBTCH1detail2.Dispose();
                CBBTCH1detail3.Dispose();
                CBBTCH1detail4.Dispose();
                CBBTCH1detail5.Dispose();
                CBBTCH1detail6.Dispose();
                throw (e);
            }
        }

        public bool CheckAccpacInvoiceAvail(int invoiceId)
        {
            View cssql = dbLink.OpenView("CS0120");
            try
            {
                cssql.Browse("SELECT IDINVC FROM ARIBH WHERE IDINVC = '" + invoiceId.ToString() + "'", true);
                cssql.InternalSet(256);

                if (cssql.GoNext())
                {
                    cssql.Dispose();
                    return true;
                }
                else
                {
                    cssql.Dispose();
                    return false;
                }
            }
            catch (Exception e)
            {
                cssql.Dispose();
                throw (e);
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

            try
            {
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

                CBBTCH1batch.Dispose();
                CBBTCH1header.Dispose();
                CBBTCH1detail1.Dispose();
                CBBTCH1detail2.Dispose();
                CBBTCH1detail3.Dispose();
                CBBTCH1detail4.Dispose();
                CBBTCH1detail5.Dispose();
                CBBTCH1detail6.Dispose();
            }
            catch (Exception e)
            {
                CBBTCH1batch.Dispose();
                CBBTCH1header.Dispose();
                CBBTCH1detail1.Dispose();
                CBBTCH1detail2.Dispose();
                CBBTCH1detail3.Dispose();
                CBBTCH1detail4.Dispose();
                CBBTCH1detail5.Dispose();
                CBBTCH1detail6.Dispose();
                throw (e);
            }
        }

        int GetLastPaymentBatch()
        {
           CBBTCH1batch = dbLink.OpenView("AR0041");
            try
            {
                int BatchId = 0;
                bool gotIt;
                gotIt = CBBTCH1batch.GoBottom();

                if (gotIt)
                {
                    BatchId = Convert.ToInt32(CBBTCH1batch.Fields.FieldByName("CNTBTCH").Value);
                }
                CBBTCH1batch.Dispose();
                return BatchId;
            }
            catch (Exception e)
            {
                CBBTCH1batch.Dispose();
                throw (e);
            }
        }

        public void CreditNoteInsert(string batchNumber, string customerId, string acctNumber, string amount, string invoiceToApply, string docNumber, string description)
        {
            b1_arInvoiceBatch = dbLink.OpenView("AR0031");
            b1_arInvoiceHeader = dbLink.OpenView("AR0032");
            b1_arInvoiceDetail = dbLink.OpenView("AR0033");
            b1_arInvoicePaymentSchedules = dbLink.OpenView("AR0034");
            b1_arInvoiceHeaderOptFields = dbLink.OpenView("AR0402");
            b1_arInvoiceDetailOptFields = dbLink.OpenView("AR0401");

            try
            {
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

                b1_arInvoiceBatch.Dispose();
                b1_arInvoiceHeader.Dispose();
                b1_arInvoiceDetail.Dispose();
                b1_arInvoicePaymentSchedules.Dispose();
                b1_arInvoiceHeaderOptFields.Dispose();
                b1_arInvoiceDetailOptFields.Dispose();
            }
            catch(Exception e)
            {
                b1_arInvoiceBatch.Dispose();
                b1_arInvoiceHeader.Dispose();
                b1_arInvoiceDetail.Dispose();
                b1_arInvoicePaymentSchedules.Dispose();
                b1_arInvoiceHeaderOptFields.Dispose();
                b1_arInvoiceDetailOptFields.Dispose();
                throw (e);
            }
        }

        public int GetRBatchNumber(string referenceNumber)
        {
            View cssql = dbLink.OpenView("CS0120");
            try
            {
                int batchNum = -1;
                cssql.Browse("SELECT CNTBTCH FROM ARTCR WHERE IDRMIT = '" + referenceNumber + "'", true);
                cssql.InternalSet(256);

                if (cssql.GoNext())
                {
                    batchNum = Convert.ToInt32(cssql.Fields.FieldByName("CNTBTCH").Value);
                }
                cssql.Dispose();
                return batchNum;
            }
            catch (Exception e)
            {
                cssql.Dispose();
                throw (e);
            }
        }

        public string GetDocNumber(string referenceNumber)
        {
            View cssql = dbLink.OpenView("CS0120");
            try
            {
                string docNum = "";
                cssql.Browse("SELECT DOCNBR FROM ARTCR WHERE IDRMIT = '" + referenceNumber + "'", true);
                cssql.InternalSet(256);

                if (cssql.GoNext())
                {
                    docNum = Convert.ToString(cssql.Fields.FieldByName("DOCNBR").Value);
                }
                cssql.Dispose();
                return docNum;
            }
            catch (Exception e)
            {
                cssql.Dispose();
                throw (e);
            }
        }

        public void PayByCredit(string customerId, string invoiceId, string batchNumber, string documentNumber)
        {
            arRecptBatch = dbLink.OpenView("AR0041");
            arRecptHeader = dbLink.OpenView("AR0042");
            arRecptDetail1 = dbLink.OpenView("AR0044");
            arRecptDetail2 = dbLink.OpenView("AR0045");
            arRecptDetail3 = dbLink.OpenView("AR0043");
            arRecptDetail4 = dbLink.OpenView("AR0061");
            arRecptDetail5 = dbLink.OpenView("AR0406");
            arRecptDetail6 = dbLink.OpenView("AR0170");

            try
            {
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

                arRecptBatch.Dispose();
                arRecptHeader.Dispose();
                arRecptDetail1.Dispose();
                arRecptDetail2.Dispose();
                arRecptDetail3.Dispose();
                arRecptDetail4.Dispose();
                arRecptDetail5.Dispose();
                arRecptDetail6.Dispose();
            }
            catch (Exception e)
            {
                arRecptBatch.Dispose();
                arRecptHeader.Dispose();
                arRecptDetail1.Dispose();
                arRecptDetail2.Dispose();
                arRecptDetail3.Dispose();
                arRecptDetail4.Dispose();
                arRecptDetail5.Dispose();
                arRecptDetail6.Dispose();
                throw (e);
            }
        }

        public bool InvoiceDelete(int invoiceId)
        {
            b1_arInvoiceBatch = dbLink.OpenView("AR0031");
            b1_arInvoiceHeader = dbLink.OpenView("AR0032");
            b1_arInvoiceDetail = dbLink.OpenView("AR0033");
            b1_arInvoicePaymentSchedules = dbLink.OpenView("AR0034");
            b1_arInvoiceHeaderOptFields = dbLink.OpenView("AR0402");
            b1_arInvoiceDetailOptFields = dbLink.OpenView("AR0401");

            try
            {
                int entryNumber = GetEntryNumber(invoiceId);
                int batchNumber = GetIbatchNumber(invoiceId);

                if (!CheckAccpacIBatchPosted(batchNumber))
                {
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

                    b1_arInvoiceBatch.Dispose();
                    b1_arInvoiceHeader.Dispose();
                    b1_arInvoiceDetail.Dispose();
                    b1_arInvoicePaymentSchedules.Dispose();
                    b1_arInvoiceHeaderOptFields.Dispose();
                    b1_arInvoiceDetailOptFields.Dispose();
                    return true;
                }
                else
                {
                    Log.Save("The batch is already posted, cannot delete invoice");
                    return false;
                }
            }
            catch (Exception e)
            {
                b1_arInvoiceBatch.Dispose();
                b1_arInvoiceHeader.Dispose();
                b1_arInvoiceDetail.Dispose();
                b1_arInvoicePaymentSchedules.Dispose();
                b1_arInvoiceHeaderOptFields.Dispose();
                b1_arInvoiceDetailOptFields.Dispose();
                throw (e);
            }
        }

        void GetViewInfo(View ax, string filename)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + @"resources";
            StreamWriter output = new StreamWriter(path + @"\" + filename + ".txt");

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

            try
            {
                int count = 0;
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

                CBBTCH1batch.Dispose();
                CBBTCH1header.Dispose();
                CBBTCH1detail1.Dispose();
                CBBTCH1detail2.Dispose();
                CBBTCH1detail3.Dispose();
                CBBTCH1detail4.Dispose();
                CBBTCH1detail5.Dispose();
                CBBTCH1detail6.Dispose();
                CBBTCH1detail7.Dispose();
                CBBTCH1detail8.Dispose();
                return count;
            }
            catch (Exception e)
            {
                CBBTCH1batch.Dispose();
                CBBTCH1header.Dispose();
                CBBTCH1detail1.Dispose();
                CBBTCH1detail2.Dispose();
                CBBTCH1detail3.Dispose();
                CBBTCH1detail4.Dispose();
                CBBTCH1detail5.Dispose();
                CBBTCH1detail6.Dispose();
                CBBTCH1detail7.Dispose();
                CBBTCH1detail8.Dispose();
                throw (e);
            }
        }

        public void OnDebug()
        {
            OnStart(null);
        }

        public void XrateInsert(string amt)
        {
            csRateHeader = dbLink.OpenView("CS0005");
            csRateDetail = dbLink.OpenView("CS0006");

            try
            {
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

                csRateHeader.Dispose();
                csRateDetail.Dispose();
            }
            catch (Exception e)
            {
                csRateHeader.Dispose();
                csRateDetail.Dispose();
                throw(e);
            }
        }
    }
}
