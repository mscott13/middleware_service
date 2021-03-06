﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
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
using System.Web.Script.Serialization;
using static middleware_service.EventBridge;
using Microsoft.Owin.Hosting;

namespace middleware_service
{
    public partial class middleware_service : ServiceBase
    {
        SqlTableDependency<SqlNotifyCancellation> tableDependCancellation;
        SqlTableDependency<SqlNotify_DocumentInfo> tableDependInfo;

        Session accpacSession;
        DBLink dbLink;
        Integration intLink;
        Log log;

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
        public IDisposable selfHost;

        System.Timers.Timer deferredTimer = new System.Timers.Timer();

        public middleware_service()
        {
            InitializeComponent();
        }

        void GenRptRequest(string ReportType)
        {
            log = new Log();
            try
            {
                int m = 0;
                int y = 0;

                if (ReportType == "Monthly")
                {
                    m = DateTime.Now.Month - 1;
                    y = DateTime.Now.Year;

                    if (m == 0)
                    {
                        m = 12;
                        y = y - 1;
                    }
                    log.Save("Generating Monthly Deferred Income Report... | Month: " + m + ", Year: " + y);
                }

                if (ReportType == "Annual")
                {
                    m = 4;
                    y = DateTime.Now.Year - 1;
                    log.Save("Generating Annual Deferred Income Report... | Month: " + m + ", Year: " + y);
                }

                JavaScriptSerializer serialize = new JavaScriptSerializer();
                var param = new { ReportType = ReportType, month = m, year = y };
                var json = serialize.Serialize(param);
                var client = new WebClient();
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                string id = client.UploadString("http://erp-srvr.sma.gov.jm:1786/integrationservice.asmx/Generate_SaveDeferredRpt", "POST", json);
                log.Save("Generate report request sent");
                log.WriteEnd();
            }
            catch (Exception ex)
            {
                log.Save(ex.InnerException.Message);
                log.WriteEnd();
            }
        }

        private void DeferredTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            log.Save("Checking deferred report status...");
            DateTime MonthlyRptDate = intLink.GetNextGenDate("Monthly");
            DateTime AnnualRptDate = intLink.GetNextGenDate("Annual");

            if (DateTime.Now.Year == MonthlyRptDate.Year && DateTime.Now.Month == MonthlyRptDate.Month && DateTime.Now.Day == MonthlyRptDate.Day)
            {
                if (DateTime.Now.Hour == MonthlyRptDate.Hour)
                {
                    GenRptRequest("Monthly");
                }
            }

            if (DateTime.Now.Year == AnnualRptDate.Year && DateTime.Now.Month == AnnualRptDate.Month && DateTime.Now.Day == AnnualRptDate.Day)
            {
                if (DateTime.Now.Hour == AnnualRptDate.Hour)
                {
                    new Log().Save("Generating Annual Deferred Income Report...");
                    GenRptRequest("Annual");
                }
            }
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                intLink = new Integration();
                accpacSession = new Session();
                log = new Log();

                using (tableDependCancellation = new SqlTableDependency<SqlNotifyCancellation>(Constants.dbGeneric, "tblARInvoices"))
                {
                    tableDependCancellation.OnChanged += TableDependCancellation_OnChanged;
                    tableDependCancellation.OnError += TableDependCancellation_OnError;
                }

                using (tableDependInfo = new SqlTableDependency<SqlNotify_DocumentInfo>(Constants.dbGeneric, "tblGLDocuments"))
                {
                    tableDependInfo.OnChanged += TableDependInfo_OnChanged;
                    tableDependInfo.OnError += TableDependInfo_OnError;
                }

                deferredTimer.Elapsed += DeferredTimer_Elapsed;
                deferredTimer.Enabled = true;
                deferredTimer.Interval = 3600000; 
                //////////////////////////////////////////////////////////////////// STARTING SESSION ///////////////////////////////////////////////////////////////////////
                log.Save("Starting accpac session...");
                accpacSession.Init("", "XY", "XY1000", "65A");
                accpacSession.Open("ADMIN", "SPECTRUM9", Constants.LIV_COMPANY_ID, DateTime.Today, 0);
                dbLink = accpacSession.OpenDBLink(DBLinkType.Company, DBLinkFlags.ReadWrite);

                log.Save("Accpac Version: " + accpacSession.AppVersion);
                log.Save("Company: " + accpacSession.CompanyName);
                log.Save("Session Status: " + accpacSession.IsOpened);

                tableDependCancellation.Start();
                tableDependInfo.Start();

                deferredTimer.Start();
                log.Save("middleware_service started.");
                log.WriteEnd();
                InitSignalR();
            }
            catch (Exception e)
            {
                log.Save(e.Message + " " + e.StackTrace);
                log.WriteEnd();
                Stop();
            }
        }

        protected override void OnShutdown()
        {
            new Log().Save("Machine shutdown event detected");
            base.OnShutdown();
        }

        protected override void OnStop()
        {
            log = new Log();
            try
            {
                tableDependCancellation.Stop();
                tableDependInfo.Stop();
                tableDependCancellation.Dispose();
                tableDependInfo.Dispose();
                deferredTimer.Stop();

                log.Save("middleware_service stopped.");
                log.WriteEnd();
            }
            catch (Exception e)
            {
                log.Save(e.Message + " " + e.StackTrace);
                log.WriteEnd();
            }
        }

        private void TableDependCancellation_OnError(object sender, TableDependency.EventArgs.ErrorEventArgs e)
        {
            new Log().Save("Table Dependency error: " + e.Error.Message);
            Stop();
        }

        private void TableDependInfo_OnError(object sender, TableDependency.EventArgs.ErrorEventArgs e)
        {
            new Log().Save("Table Dependency error: " + e.Error.Message);
            Stop();
        }

        private void InitSignalR()
        {
            log.Save("Starting server host...");
            EventBridge.SignalReceived += EventBridge_SignalReceived;
            string url = Constants.BASE_ADDRESS + Constants.PORT;
            selfHost = WebApp.Start<Startup>(url);
            log.Save("Server running on port: " + Constants.PORT);
        }

        private void EventBridge_SignalReceived(object source, SignalR.EventObjects.SignalArgs args)
        {
            try
            {
                log.Save("Command: " + args.action + " received from: " + args.clientId);
                if (args.action == Constants.INV_MANUAL_ENTRY)
                {
                    var documentInfo = intLink.GetGLDocumentInfo(Convert.ToInt32(args.data), INVOICE);
                    if (documentInfo != null)
                    {
                        ManualInvoiceEntry(documentInfo);
                    }
                    else
                        log.Save("Invoice information was not found for ID: " + args.data);
                }
                else if (args.action == Constants.INV_MANUAL_CANCEL)
                {
                    var cancellationInfo = intLink.GetInvoiceCanellationInfo(Convert.ToInt32(args.data));
                    if (cancellationInfo != null)
                    {
                        ManualInvoiceCancellation(cancellationInfo);
                    }
                    else
                        log.Save("Invoice information was not found for ID: " + args.data);
                }
                else if (args.action == Constants.RCT_MANUAL_ENTRY)
                {
                    var documentInfo = intLink.GetGLDocumentInfoViaId(Convert.ToInt32(args.data));
                    if(documentInfo != null)
                    {
                        ManualReceiptEntry(documentInfo);
                    }
                    else
                        log.Save("Document info was not found for ID: " + args.data);
                }
                else
                    log.Save($"No matching actions found. ignoring... | data submitted: {args.data}");
            }
            catch (Exception ex)
            {

                if (accpacSession.Errors.Count > 0)
                {
                    log.Save(ex.Message + " " + ex.StackTrace);
                    for (int i = 0; i < accpacSession.Errors.Count; i++)
                    {
                        log.Save(accpacSession.Errors[i].Message + ", Severity: " + accpacSession.Errors[i].Priority);
                    }
                    accpacSession.Errors.Clear();
                    log.WriteEnd();
                }
                else
                {
                    log.Save(ex.Message + " " + ex.StackTrace);
                    log.WriteEnd();
                }
            }
        }

        private void ManualReceiptEntry(SqlNotify_DocumentInfo docInfo)
        {
            if (docInfo.DocumentType == RECEIPT && docInfo.PaymentMethod != 99)
            {
                log.Save($"Manual Receipt entry for documentId: {docInfo.DocumentID}");
                Thread.Sleep(1500);
                Data dt = new Data();
                PaymentInfo pinfo = new PaymentInfo();

                List<string> paymentData = new List<string>(3);
                List<string> clientData = new List<string>(3);
                List<string> feeData = new List<string>(3);

                while (pinfo.ReceiptNumber == 0)
                {
                    pinfo = intLink.getPaymentInfo(docInfo.OriginalDocumentID);
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

                clientData = intLink.getClientInfo_inv(id);
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

                    log.Save("Invoice Id: " + invoiceId.ToString());
                    log.Save("Customer Id: " + customerId);

                    prepstat = "No";
                    valstart = intLink.GetValidity(Convert.ToInt32(invoiceId));
                    valend = intLink.GetValidityEnd(Convert.ToInt32(invoiceId));
                }
                else
                {
                    prepstat = "Yes";
                    log.Save("Prepayment");
                    log.Save("Customer Id: " + customerId);

                    var gl = intLink.GetCreditGlID((transid + 1).ToString());

                    if (gl == 5150)
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
                    dt = Translate(customerId, ftype, companyName, debit, notes, "PREPAYMENT", intLink.getFreqUsage(Convert.ToInt32(invoiceId)));
                }
                else
                {
                    dt = Translate(customerId, ftype, companyName, debit, notes, "", intLink.getFreqUsage(Convert.ToInt32(invoiceId)));
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
                        log.Save("Bank: FGB JA$ CURRENT A/C");
                        intLink.modifyInvoiceList(0, 1, dt.customerId);

                        if (dt.success)
                        {
                            if (receiptBatchAvail("FGBJMREC"))
                            {
                                string reference = intLink.GetCurrentRef("FGBJMREC");
                                log.Save("Target Batch: " + intLink.getRecieptBatch("FGBJMREC"));
                                log.Save("Transferring Receipt");

                                ReceiptTransfer(intLink.getRecieptBatch("FGBJMREC"), dt.customerId, dt.debit, dt.companyName, reference, invoiceId, paymentDate, dt.desc, customerId, valstart, valend);
                                intLink.UpdateBatchCountPayment(intLink.getRecieptBatch("FGBJMREC"));
                                intLink.UpdateReceiptNumber(receipt, intLink.GetCurrentRef("FGBJMREC"));
                                intLink.IncrementReferenceNumber(intLink.getBankCodeId("FGBJMREC"), Convert.ToDecimal(dt.debit));
                                intLink.storePayment(dt.customerId, companyName, DateTime.Now, invoiceId, Convert.ToDecimal(dt.debit), 0, prepstat, Convert.ToInt32(reference), Convert.ToInt32(glid), "No", 0);
                            }
                            else
                            {
                                string reference = intLink.GetCurrentRef("FGBJMREC");
                                CreateReceiptBatchEx("FGBJMREC", "Middleware Generated Batch for FGBJMREC");
                                intLink.openNewReceiptBatch(1, GetLastPaymentBatch(), "FGBJMREC");

                                log.Save("Target Batch: " + intLink.getRecieptBatch("FGBJMREC"));
                                log.Save("Transferring Receipt");

                                ReceiptTransfer(intLink.getRecieptBatch("FGBJMREC"), dt.customerId, dt.debit, dt.companyName, reference, invoiceId, paymentDate, dt.desc, customerId, valstart, valend);
                                intLink.UpdateBatchCountPayment(intLink.getRecieptBatch("FGBJMREC"));
                                intLink.UpdateReceiptNumber(receipt, intLink.GetCurrentRef("FGBJMREC"));
                                intLink.IncrementReferenceNumber(intLink.getBankCodeId("FGBJMREC"), Convert.ToDecimal(dt.debit));
                                intLink.storePayment(dt.customerId, companyName, DateTime.Now, invoiceId, Convert.ToDecimal(dt.debit), 0, prepstat, Convert.ToInt32(reference), Convert.ToInt32(glid), "No", 0);
                            }
                        }
                    }
                    else if (glid == "5147")
                    {
                        log.Save("Bank: FGB US$ SAVINGS A/C");
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

                        if (prepstat == "Yes")
                        {
                            if (CustomerExists(dt.customerId = clientIdPrefix + "-T")) currentRate = intLink.GetRate();
                            else dt.customerId = intLink.getClientIdZRecord(false);
                        }

                        if (dt.customerId[6] == 'T')
                        {
                            usamount = Math.Round(Convert.ToDecimal(dt.debit) / intLink.GetUsRateByInvoice(Convert.ToInt32(invoiceId)), 2);
                            intLink.modifyInvoiceList(0, intLink.GetUsRateByInvoice(Convert.ToInt32(invoiceId)), dt.customerId);
                            currentRate = intLink.GetRate();
                        }
                        else intLink.modifyInvoiceList(0, 1, dt.customerId);

                        if (dt.success)
                        {
                            if (receiptBatchAvail("FGBUSMRC"))
                            {
                                string reference = intLink.GetCurrentRef("FGBUSMRC");
                                log.Save("Target Batch: " + intLink.getRecieptBatch("FGBUSMRC"));
                                log.Save("Transferring Receipt");

                                ReceiptTransfer(intLink.getRecieptBatch("FGBUSMRC"), dt.customerId, Math.Round(transferedAmt, 2).ToString(), dt.companyName, reference, invoiceId, paymentDate, dt.desc, customerId, valstart, valend);
                                intLink.UpdateBatchCountPayment(intLink.getRecieptBatch("FGBUSMRC"));
                                intLink.UpdateReceiptNumber(receipt, intLink.GetCurrentRef("FGBUSMRC"));
                                intLink.IncrementReferenceNumber(intLink.getBankCodeId("FGBUSMRC"), Convert.ToDecimal(dt.debit));
                                intLink.storePayment(dt.customerId, companyName, DateTime.Now, invoiceId, Convert.ToDecimal(dt.debit), usamount, prepstat, Convert.ToInt32(reference), Convert.ToInt32(glid), "No", currentRate);
                            }
                            else
                            {
                                string reference = intLink.GetCurrentRef("FGBUSMRC");
                                CreateReceiptBatchEx("FGBUSMRC", "Middleware Generated Batch for FGBUSMRC");
                                intLink.openNewReceiptBatch(1, GetLastPaymentBatch(), "FGBUSMRC");

                                log.Save("Target Batch: " + intLink.getRecieptBatch("FGBUSMRC"));
                                log.Save("Transferring Receipt");

                                ReceiptTransfer(intLink.getRecieptBatch("FGBUSMRC"), dt.customerId, Math.Round(transferedAmt, 2).ToString(), dt.companyName, reference, invoiceId, paymentDate, dt.desc, customerId, valstart, valend);
                                intLink.UpdateBatchCountPayment(intLink.getRecieptBatch("FGBUSMRC"));
                                intLink.UpdateReceiptNumber(receipt, intLink.GetCurrentRef("FGBUSMRC"));
                                intLink.IncrementReferenceNumber(intLink.getBankCodeId("FGBUSMRC"), Convert.ToDecimal(dt.debit));
                                intLink.storePayment(dt.customerId, companyName, DateTime.Now, invoiceId, Convert.ToDecimal(dt.debit), usamount, prepstat, Convert.ToInt32(reference), Convert.ToInt32(glid), "No", currentRate);
                            }
                        }
                    }
                    else if (glid == "5148")
                    {
                        log.Save("NCB JA$ SAVINGS A/C");
                        intLink.modifyInvoiceList(0, 1, dt.customerId);

                        if (dt.success)
                        {
                            if (receiptBatchAvail("NCBJMREC"))
                            {
                                string reference = intLink.GetCurrentRef("NCBJMREC");
                                log.Save("Target Batch: " + intLink.getRecieptBatch("NCBJMREC"));
                                log.Save("Transferring Receipt");

                                ReceiptTransfer(intLink.getRecieptBatch("NCBJMREC"), dt.customerId, dt.debit, dt.companyName, reference, invoiceId, paymentDate, dt.desc, customerId, valstart, valend);
                                intLink.UpdateBatchCountPayment(intLink.getRecieptBatch("NCBJMREC"));
                                intLink.UpdateReceiptNumber(receipt, intLink.GetCurrentRef("NCBJMREC"));
                                intLink.IncrementReferenceNumber(intLink.getBankCodeId("NCBJMREC"), Convert.ToDecimal(dt.debit));
                                intLink.storePayment(dt.customerId, companyName, DateTime.Now, invoiceId, Convert.ToDecimal(dt.debit), 0, prepstat, Convert.ToInt32(reference), Convert.ToInt32(glid), "No", 1);
                            }
                            else
                            {
                                string reference = intLink.GetCurrentRef("NCBJMREC");
                                CreateReceiptBatchEx("NCBJMREC", "Middleware Generated Batch for NCBJMREC");
                                intLink.openNewReceiptBatch(1, GetLastPaymentBatch(), "NCBJMREC");

                                log.Save("Target Batch: " + intLink.getRecieptBatch("NCBJMREC"));
                                log.Save("Transferring Receipt");

                                ReceiptTransfer(intLink.getRecieptBatch("NCBJMREC"), dt.customerId, dt.debit, dt.companyName, reference, invoiceId, paymentDate, dt.desc, customerId, valstart, valend);
                                intLink.UpdateBatchCountPayment(intLink.getRecieptBatch("NCBJMREC"));
                                intLink.UpdateReceiptNumber(receipt, intLink.GetCurrentRef("NCBJMREC"));
                                intLink.IncrementReferenceNumber(intLink.getBankCodeId("NCBJMREC"), Convert.ToDecimal(dt.debit));
                                intLink.storePayment(dt.customerId, companyName, DateTime.Now, invoiceId, Convert.ToDecimal(dt.debit), 0, prepstat, Convert.ToInt32(reference), Convert.ToInt32(glid), "No", 1);
                            }
                        }
                    }
                }
                else
                {
                    log.Save("Customer not found in accpac");
                }
            }
            else if (docInfo.DocumentType == CREDIT_MEMO)
            {
                log.Save("Manual entry for new Credit Memo...");
                CreditNoteInfo creditNote = new CreditNoteInfo();

                while (creditNote.amount == 0)
                {
                    creditNote = intLink.getCreditNoteInfo(docInfo.OriginalDocumentID, docInfo.DocumentID);
                    Thread.Sleep(1000);
                }

                List<string> clientInfo = new List<string>(4);
                clientInfo = intLink.getClientInfo_inv(creditNote.CustomerID.ToString());
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

                Data dt = Translate(cNum, creditNote.FeeType, companyName, creditNote.amount.ToString(), creditNote.notes, accountNum, intLink.getFreqUsage(creditNote.ARInvoiceID));

                if (checkAccpacInvoiceAvail(creditNote.ARInvoiceID))
                {
                    cred_docNum = intLink.getCreditMemoNumber();
                    log.Save("Creating credit memo");
                    int batchNumber = getBatch(CREDIT_NOTE, creditNote.ARInvoiceID.ToString());

                    intLink.storeInvoice(creditNote.ARInvoiceID, batchNumber, creditNote.CreditGL, companyName, dt.customerId, DateTime.Now, "", creditNote.amount, "no modification", 1, 0, 0, 1, cred_docNum);
                    CreditNoteInsert(batchNumber.ToString(), dt.customerId, accountNum, creditNote.amount.ToString(), creditNote.ARInvoiceID.ToString(), cred_docNum.ToString(), creditNoteDesc);
                    intLink.updateAsmsCreditMemoNumber(docInfo.DocumentID, cred_docNum);
                }
                else
                {
                    log.Save("The Credit Memo was not created. The Invoice does not exist.");
                    cred_docNum = intLink.getCreditMemoNumber();
                    intLink.updateAsmsCreditMemoNumber(docInfo.DocumentID, cred_docNum);
                    log.Save("The Credit Memo number in ASMS updated.");
                }
            }
            else if (docInfo.DocumentType == RECEIPT && docInfo.PaymentMethod == 99)
            {
                log.Save("Manual entry for payment By Credit");
                PaymentInfo pinfo = intLink.getPaymentInfo(docInfo.OriginalDocumentID);
                List<string> clientData = intLink.getClientInfo_inv(pinfo.CustomerID.ToString());
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

                Data dt = Translate(customerId, ftype, companyName, pinfo.Debit.ToString(), notes, "", intLink.getFreqUsage(pinfo.InvoiceID).ToString());
                PrepaymentData pData = intLink.checkPrepaymentAvail(dt.customerId);
                int invoiceBatch = getIbatchNumber(pinfo.InvoiceID);            //This value represents the batch that the invoice belongs to.
                int receiptBatch = getRBatchNumber(pData.referenceNumber);      //This value represents the batch that the receipt belongs to.

                if (pData.dataAvail)
                {
                    if (checkAccpacIBatchPosted(invoiceBatch) && checkAccpacRBatchPosted(receiptBatch))
                    {
                        var glid = pData.destinationBank.ToString();

                        if (glid == "5146")
                        {
                            log.Save("Bank: FGB JA$ CURRENT A/C");
                            if (pData.totalPrepaymentRemainder >= pinfo.Debit)
                            {
                                log.Save("Carrying out Payment By Credit Transaction");
                                decimal reducingAmt = pinfo.Debit;

                                if (receiptBatchAvail("FGBJMREC"))
                                {
                                    while (reducingAmt > 0)
                                    {
                                        pData = intLink.checkPrepaymentAvail(dt.customerId);
                                        PayByCredit(dt.customerId, pinfo.InvoiceID.ToString(), intLink.getRecieptBatch("FGBJMREC"), getDocNumber(pData.referenceNumber));
                                        if (reducingAmt > pData.remainder) intLink.adjustPrepaymentRemainder(pData.remainder, pData.sequenceNumber);
                                        else intLink.adjustPrepaymentRemainder(reducingAmt, pData.sequenceNumber);
                                        reducingAmt = reducingAmt - pData.remainder;
                                    }
                                    intLink.storePayment(dt.customerId, companyName, DateTime.Now, pinfo.InvoiceID.ToString(), pinfo.Debit, 0, "No", 0, Convert.ToInt32(glid), "Yes", 1);
                                    log.Save("Payment by credit transaction complete");
                                }
                                else
                                {
                                    CreateReceiptBatchEx("FGBJMREC", "Middleware Generated Batch for FGBJMREC");
                                    intLink.openNewReceiptBatch(1, GetLastPaymentBatch(), "FGBJMREC");
                                    log.Save("Target Batch: " + intLink.getRecieptBatch("FGBJMREC"));
                                    while (reducingAmt > 0)
                                    {
                                        pData = intLink.checkPrepaymentAvail(dt.customerId);
                                        PayByCredit(dt.customerId, pinfo.InvoiceID.ToString(), intLink.getRecieptBatch("FGBJMREC"), getDocNumber(pData.referenceNumber));
                                        if (reducingAmt > pData.remainder) intLink.adjustPrepaymentRemainder(pData.remainder, pData.sequenceNumber);
                                        else intLink.adjustPrepaymentRemainder(reducingAmt, pData.sequenceNumber);
                                        reducingAmt = reducingAmt - pData.remainder;
                                    }
                                    intLink.storePayment(dt.customerId, companyName, DateTime.Now, pinfo.InvoiceID.ToString(), pinfo.Debit, 0, "No", 0, Convert.ToInt32(glid), "Yes", 1);
                                    log.Save("Payment by credit transaction complete");
                                }
                            }
                            else
                            {
                                log.Save("Prepayment balance not enough to carry out Transaction");
                            }
                        }

                        if (glid == "5147")
                        {
                            log.Save("Bank: FGB US$ SAVINGS A/C");
                            decimal usRate = intLink.GetUsRateByInvoice(pinfo.InvoiceID); //using the US rate from the Invoice at the time it was created.
                            decimal usAmount = Convert.ToDecimal(dt.debit) / usRate;
                            if (pData.totalPrepaymentRemainder >= usAmount)
                            {
                                log.Save("Carrying out Payment By Credit Transaction");
                                decimal reducingAmt = usAmount;

                                if (receiptBatchAvail("FGBUSMRC"))
                                {
                                    while (reducingAmt > 0)
                                    {
                                        pData = intLink.checkPrepaymentAvail(dt.customerId);
                                        PayByCredit(dt.customerId, pinfo.InvoiceID.ToString(), intLink.getRecieptBatch("FGBUSMRC"), getDocNumber(pData.referenceNumber));
                                        if (reducingAmt > pData.remainder) intLink.adjustPrepaymentRemainder(pData.remainder, pData.sequenceNumber);
                                        else intLink.adjustPrepaymentRemainder(reducingAmt, pData.sequenceNumber);
                                        reducingAmt = reducingAmt - pData.remainder;
                                    }
                                    if (usRate == 1) intLink.storePayment(dt.customerId, companyName, DateTime.Now, pinfo.InvoiceID.ToString(), pinfo.Debit, 0, "No", 0, Convert.ToInt32(glid), "Yes", 1);
                                    else intLink.storePayment(dt.customerId, companyName, DateTime.Now, pinfo.InvoiceID.ToString(), pinfo.Debit, usAmount, "No", 0, Convert.ToInt32(glid), "Yes", 1);
                                    log.Save("Payment by credit transaction complete");
                                }
                                else
                                {
                                    CreateReceiptBatchEx("FGBUSMRC", "Middleware Generated Batch for FGBUSMRC");
                                    intLink.openNewReceiptBatch(1, GetLastPaymentBatch(), "FGBUSMRC");
                                    log.Save("Target Batch: " + intLink.getRecieptBatch("FGBUSMRC"));

                                    while (reducingAmt > 0)
                                    {
                                        pData = intLink.checkPrepaymentAvail(dt.customerId);
                                        PayByCredit(dt.customerId, pinfo.InvoiceID.ToString(), intLink.getRecieptBatch("FGBUSMRC"), getDocNumber(pData.referenceNumber));
                                        if (reducingAmt > pData.remainder) intLink.adjustPrepaymentRemainder(pData.remainder, pData.sequenceNumber);
                                        else intLink.adjustPrepaymentRemainder(reducingAmt, pData.sequenceNumber);
                                        reducingAmt = reducingAmt - pData.remainder;
                                    }

                                    if (usRate == 1)
                                    {
                                        intLink.storePayment(dt.customerId, companyName, DateTime.Now, pinfo.InvoiceID.ToString(), pinfo.Debit, 0, "No", 0, Convert.ToInt32(glid), "Yes", 1);
                                    }
                                    else
                                    {
                                        intLink.storePayment(dt.customerId, companyName, DateTime.Now, pinfo.InvoiceID.ToString(), pinfo.Debit, usAmount, "No", 0, Convert.ToInt32(glid), "Yes", 1);

                                    }
                                    log.Save("Payment by credit transaction complete");
                                }
                            }
                            else
                            {
                                log.Save("Prepayment balance not enough to carry out Transaction");
                            }
                        }

                        if (glid == "5148")
                        {
                            log.Save("Bank: NCB JA$ SAVINGS A/C");
                            if (pData.totalPrepaymentRemainder >= pinfo.Debit)
                            {
                                log.Save("Carrying out Payment By Credit Transaction");
                                decimal reducingAmt = pinfo.Debit;

                                if (receiptBatchAvail("NCBJMREC"))
                                {
                                    while (reducingAmt > 0)
                                    {
                                        pData = intLink.checkPrepaymentAvail(dt.customerId);
                                        PayByCredit(dt.customerId, pinfo.InvoiceID.ToString(), intLink.getRecieptBatch("NCBJMREC"), getDocNumber(pData.referenceNumber));
                                        if (reducingAmt > pData.remainder) intLink.adjustPrepaymentRemainder(pData.remainder, pData.sequenceNumber);
                                        else intLink.adjustPrepaymentRemainder(reducingAmt, pData.sequenceNumber);
                                        reducingAmt = reducingAmt - pData.remainder;
                                    }
                                    intLink.storePayment(dt.customerId, companyName, DateTime.Now, pinfo.InvoiceID.ToString(), pinfo.Debit, 0, "No", 0, Convert.ToInt32(glid), "Yes", 1);
                                    log.Save("Payment by credit transaction complete");
                                }
                                else
                                {
                                    CreateReceiptBatchEx("NCBJMREC", "Middleware Generated Batch for NCBJMREC");
                                    intLink.openNewReceiptBatch(1, GetLastPaymentBatch(), "NCBJMREC");
                                    log.Save("Target Batch: " + intLink.getRecieptBatch("NCBJMREC"));
                                    while (reducingAmt > 0)
                                    {
                                        pData = intLink.checkPrepaymentAvail(dt.customerId);
                                        PayByCredit(dt.customerId, pinfo.InvoiceID.ToString(), intLink.getRecieptBatch("NCBJMREC"), getDocNumber(pData.referenceNumber));
                                        if (reducingAmt > pData.remainder) intLink.adjustPrepaymentRemainder(pData.remainder, pData.sequenceNumber);
                                        else intLink.adjustPrepaymentRemainder(reducingAmt, pData.sequenceNumber);
                                        reducingAmt = reducingAmt - pData.remainder;
                                    }
                                    intLink.storePayment(dt.customerId, companyName, DateTime.Now, pinfo.InvoiceID.ToString(), pinfo.Debit, 0, "No", 0, Convert.ToInt32(glid), "Yes", 1);
                                    log.Save("Payment by credit transaction complete");
                                }
                            }
                            else
                            {
                                log.Save("Prepayment balance not enough to carry out Transaction");
                            }
                        }

                        if (glid != "5146" && glid != "5147" && glid != "5148")
                        {
                            log.Save("Bank Selected Not found, Cannot complete Transaction");
                        }
                    }
                    else
                    {
                        log.Save("Both invoice and receipt must be posted before attempting this transaction");
                    }
                }
                else
                {
                    log.Save("No prepayment record found for customer: " + dt.customerId);
                }
            }
        }

        private void ManualInvoiceEntry(SqlNotify_DocumentInfo docInfo)
        {
            log.Save("Manual invoice entry started for ID: " + docInfo.OriginalDocumentID);
            if (isAccpacSessionOpen())
            {
                InvoiceInfo invoiceInfo = intLink.getInvoiceDetails(docInfo.OriginalDocumentID);
                log.Save("Invoice amount: " + invoiceInfo.amount);

                List<string> clientInfo = intLink.getClientInfo_inv(invoiceInfo.CustomerId.ToString());
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

                log.Save("Client name: " + companyName);
                Data dt = Translate(cNum, invoiceInfo.FeeType, companyName, "", invoiceInfo.notes, intLink.GetAccountNumber(invoiceInfo.Glid), invoiceInfo.FreqUsage); // application stops here...
                DateTime invoiceValidity = intLink.GetValidity(docInfo.OriginalDocumentID); // or here...
                log.Save("Invoice Validity: " + invoiceValidity);

                int financialyear = 0;
                if (invoiceValidity.Month > 3)
                {
                    financialyear = invoiceValidity.Year + 1;
                }
                else
                {
                    financialyear = invoiceValidity.Year;
                }
                log.Save("Financial Year: " + financialyear);

                List<string> data = intLink.checkInvoiceAvail(docInfo.OriginalDocumentID.ToString());
                int r = intLink.getInvoiceReference(docInfo.OriginalDocumentID);
                log.Save("Invoice Reference number: " + r);

                if (r != -1)
                {
                    log.Save("Getting Maj Details...");
                    m = intLink.getMajDetail(r);
                    log.Save(m.ToString());
                }

                if (isPeriodCreated(financialyear))
                {
                    if (invoiceInfo.Glid < 5000 || data != null)
                    {
                        log.Save("CreditGL: " + invoiceInfo.Glid);
                        if (data != null)
                        {
                            if (data[1].ToString() == "NT")
                            {
                                if (dt.feeType == "SLF" && invoiceInfo.notes == "Renewal")
                                {
                                    stat = InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), getBatch(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()).ToString());

                                    if (stat.status == "Not Exist")
                                    {
                                        CreateCustomer(dt.customerId, dt.companyName_NewCust);
                                        InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), getBatch(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()).ToString());

                                        intLink.UpdateBatchCount(RENEWAL_SPEC + "For " + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString());
                                        intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                        intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);
                                        intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                        intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                    }
                                    else
                                    {
                                        intLink.UpdateBatchCount(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString());
                                        intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                        intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);
                                        intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString() + " For " + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                        intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                    }
                                }
                                else if (dt.feeType == "RF" && invoiceInfo.notes == "Renewal")
                                {
                                    stat = InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), getBatch(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()).ToString());

                                    if (stat.status == "Not Exist")
                                    {
                                        CreateCustomer(dt.customerId, dt.companyName_NewCust);
                                        InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), getBatch(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()).ToString());

                                        intLink.UpdateBatchCount(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString());
                                        intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                        intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);
                                        intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                        intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                    }
                                    else
                                    {
                                        intLink.UpdateBatchCount(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString());
                                        intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                        intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);
                                        intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                        intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                    }
                                }
                                else if ((invoiceInfo.notes == "Annual Fee" && m.stationType == "SSL" && m.certificateType == 0 && m.proj == "JMC") || (invoiceInfo.FreqUsage == "PRS55"))
                                {
                                    stat = InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), getBatch(MAJ, docInfo.OriginalDocumentID.ToString()).ToString());

                                    if (stat.status == "Not Exist")
                                    {
                                        CreateCustomer(dt.customerId, dt.companyName_NewCust);
                                        InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), getBatch(MAJ, docInfo.OriginalDocumentID.ToString()).ToString());

                                        intLink.UpdateBatchCount(MAJ);
                                        intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                        intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);
                                        intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(MAJ, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                        intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                    }
                                    else
                                    {
                                        intLink.UpdateBatchCount(MAJ);
                                        intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                        intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);
                                        intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(MAJ, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                        intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                    }
                                }
                                else if (invoiceInfo.notes == "Type Approval" || invoiceInfo.notes == "TypeApproval" || invoiceInfo.FreqUsage == "TA-ProAmend")
                                {
                                    stat = InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, changetous(invoiceInfo.amount).ToString(), getBatch(TYPE_APPROVAL, docInfo.OriginalDocumentID.ToString()).ToString());

                                    if (stat.status == "Not Exist")
                                    {
                                        CreateCustomer(dt.customerId, dt.companyName_NewCust);
                                        InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, changetous(invoiceInfo.amount).ToString(), getBatch(TYPE_APPROVAL, docInfo.OriginalDocumentID.ToString()).ToString());

                                        intLink.UpdateBatchCount(TYPE_APPROVAL);
                                        intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                        intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);
                                        intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(TYPE_APPROVAL, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", intLink.GetRate(), changetous(invoiceInfo.amount), invoiceInfo.isvoided, 0, 0);
                                        intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                    }
                                    else
                                    {
                                        intLink.UpdateBatchCount(TYPE_APPROVAL);
                                        intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                        intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);
                                        intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(TYPE_APPROVAL, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", intLink.GetRate(), changetous(invoiceInfo.amount), invoiceInfo.isvoided, 0, 0);
                                        intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                    }
                                }

                                else if (invoiceInfo.notes == "Annual Fee" || invoiceInfo.notes == "Modification" || invoiceInfo.notes == "Radio Operator")
                                {
                                    stat = InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), getBatch(NON_MAJ, docInfo.OriginalDocumentID.ToString()).ToString());

                                    if (stat.status == "Not Exist")
                                    {
                                        CreateCustomer(dt.customerId, dt.companyName_NewCust);
                                        InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), getBatch(NON_MAJ, docInfo.OriginalDocumentID.ToString()).ToString());

                                        intLink.UpdateBatchCount(NON_MAJ);
                                        intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                        intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);
                                        intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(NON_MAJ, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                        intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                    }
                                    else
                                    {
                                        intLink.UpdateBatchCount(NON_MAJ);
                                        intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                        intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);
                                        intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(NON_MAJ, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                        intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                    }
                                }
                            }
                            else if (data[1].ToString() == "T")
                            {
                                List<string> detail = new List<string>(3);
                                detail = intLink.GetInvoiceDetails(docInfo.OriginalDocumentID);
                                int batchNumber = getIbatchNumber(docInfo.OriginalDocumentID);


                                if (!checkAccpacIBatchPosted(batchNumber))
                                {
                                    if (invoiceInfo.Glid != Convert.ToInt32(detail[2].ToString()) || invoiceInfo.amount.ToString() != detail[3].ToString())
                                    {
                                        if (invoiceInfo.notes == "Type Approval" || invoiceInfo.notes == "TypeApproval" || invoiceInfo.FreqUsage == "TA-ProAmend")
                                        {
                                            string usamt = "";
                                            usamt = changetousupdated(invoiceInfo.amount, docInfo.OriginalDocumentID).ToString();
                                            UpdateInvoice(dt.fcode, Math.Round(changetousupdated(invoiceInfo.amount, docInfo.OriginalDocumentID), 2).ToString(), batchNumber.ToString().ToString(), getEntryNumber(docInfo.OriginalDocumentID).ToString());
                                            intLink.storeInvoice(docInfo.OriginalDocumentID, batchNumber, invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "updated", 1, Convert.ToDecimal(usamt), invoiceInfo.isvoided, 0, 0);
                                        }
                                        else
                                        {
                                            UpdateInvoice(dt.fcode, invoiceInfo.amount.ToString(), batchNumber.ToString().ToString(), getEntryNumber(docInfo.OriginalDocumentID).ToString());
                                            intLink.storeInvoice(docInfo.OriginalDocumentID, batchNumber, invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "updated", 1, 0, invoiceInfo.isvoided, 0, 0);

                                        }
                                        intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);
                                        log.Save("Updated Invoice: " + docInfo.OriginalDocumentID.ToString());
                                    }
                                    else
                                    {
                                        log.Save("Update is not needed." + docInfo.OriginalDocumentID.ToString());
                                    }
                                }
                                else
                                {
                                     log.Save($"Batch number already posted: {batchNumber}");
                                }
                            }
                        }
                        else
                        {
                            if (dt.feeType == "SLF" && invoiceInfo.notes == "Renewal")
                            {
                                intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                log.Save("Invoice Id: " + docInfo.OriginalDocumentID.ToString() + " Transferred");
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

                                intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                log.Save("Invoice Id: " + docInfo.OriginalDocumentID.ToString() + " Transferred");
                            }

                            else if ((invoiceInfo.notes == "Annual Fee" && m.stationType == "SSL" && m.certificateType == 0 && m.proj == "JMC") || (invoiceInfo.FreqUsage == "PRS55"))
                            {
                                intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(MAJ, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                log.Save("Invoice Id: " + docInfo.OriginalDocumentID.ToString() + " Transferred");
                            }
                            else if (invoiceInfo.notes == "Type Approval"  || invoiceInfo.notes == "TypeApproval" || invoiceInfo.FreqUsage == "TA-ProAmend")
                            {
                                intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(TYPE_APPROVAL, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", intLink.GetRate(), changetous(invoiceInfo.amount), invoiceInfo.isvoided, 0, 0);
                                log.Save("Invoice Id: " + docInfo.OriginalDocumentID.ToString() + " Transferred");
                            }
                            else if (invoiceInfo.notes == "Annual Fee" || invoiceInfo.notes == "Modification" || invoiceInfo.notes == "Radio Operator")
                            {
                                intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(NON_MAJ, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                log.Save("Invoice Id: " + docInfo.OriginalDocumentID.ToString() + " Transferred");
                            }
                        }
                    }
                    else
                    {
                        if (dt.feeType == "SLF" && invoiceInfo.notes == "Renewal")
                        {
                            stat = InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), getBatch(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()).ToString());

                            if (stat.status == "Not Exist")
                            {
                                CreateCustomer(dt.customerId, dt.companyName_NewCust);
                                InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), getBatch(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()).ToString());

                                intLink.UpdateBatchCount(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString());
                                intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);
                                intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                intLink.updateBatchAmount(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), invoiceInfo.amount);
                            }
                            else
                            {
                                intLink.UpdateBatchCount(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString());
                                intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);
                                intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                intLink.updateBatchAmount(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), invoiceInfo.amount);
                            }
                        }
                        else if (dt.feeType == "RF" && invoiceInfo.notes == "Renewal")
                        {
                            stat = InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), getBatch(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()).ToString());

                            if (stat.status == "Not Exist")
                            {
                                CreateCustomer(dt.customerId, dt.companyName_NewCust);
                                InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), getBatch(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()).ToString());

                                intLink.UpdateBatchCount(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString());
                                intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);
                                intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                intLink.updateBatchAmount(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), invoiceInfo.amount);
                            }
                            else
                            {
                                intLink.UpdateBatchCount(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString());
                                intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);
                                intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                intLink.updateBatchAmount(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), invoiceInfo.amount);
                            }
                        }

                        else if ((invoiceInfo.notes == "Annual Fee" && m.stationType == "SSL" && m.certificateType == 0 && m.proj == "JMC") || (invoiceInfo.FreqUsage == "PRS55"))
                        {
                            stat = InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), getBatch(MAJ, docInfo.OriginalDocumentID.ToString()).ToString());

                            if (stat.status == "Not Exist")
                            {
                                CreateCustomer(dt.customerId, dt.companyName_NewCust);
                                InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), getBatch(MAJ, docInfo.OriginalDocumentID.ToString()).ToString());

                                intLink.UpdateBatchCount(MAJ);
                                intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);
                                intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(MAJ, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                intLink.updateBatchAmount(MAJ, invoiceInfo.amount);
                            }
                            else
                            {
                                intLink.UpdateBatchCount(MAJ);
                                intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);
                                intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(MAJ, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                intLink.updateBatchAmount(MAJ, invoiceInfo.amount);
                            }
                        }
                        else if (invoiceInfo.notes == "Type Approval" || invoiceInfo.notes == "TypeApproval" ||  invoiceInfo.FreqUsage == "TA-ProAmend")
                        {
                            stat = InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, changetous(invoiceInfo.amount).ToString(), getBatch(TYPE_APPROVAL, docInfo.OriginalDocumentID.ToString()).ToString());

                            if (stat.status == "Not Exist")
                            {
                                CreateCustomer(dt.customerId, dt.companyName_NewCust);
                                InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, changetous(invoiceInfo.amount).ToString(), getBatch(TYPE_APPROVAL, docInfo.OriginalDocumentID.ToString()).ToString());

                                intLink.UpdateBatchCount(TYPE_APPROVAL);
                                intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);
                                intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(TYPE_APPROVAL, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", intLink.GetRate(), changetous(invoiceInfo.amount), invoiceInfo.isvoided, 0, 0);
                                intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                intLink.updateBatchAmount(TYPE_APPROVAL, invoiceInfo.amount);
                            }
                            else
                            {
                                intLink.UpdateBatchCount(TYPE_APPROVAL);
                                intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);
                                intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(TYPE_APPROVAL, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", intLink.GetRate(), changetous(invoiceInfo.amount), invoiceInfo.isvoided, 0, 0);
                                intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                intLink.updateBatchAmount(TYPE_APPROVAL, invoiceInfo.amount);
                            }
                        }

                        else if (invoiceInfo.notes == "Annual Fee" || invoiceInfo.notes == "Modification" || invoiceInfo.notes == "Radio Operator")
                        {
                            stat = InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), getBatch(NON_MAJ, docInfo.OriginalDocumentID.ToString()).ToString());

                            if (stat.status == "Not Exist")
                            {
                                CreateCustomer(dt.customerId, dt.companyName_NewCust);
                                InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), getBatch(NON_MAJ, docInfo.OriginalDocumentID.ToString()).ToString());

                                intLink.UpdateBatchCount(NON_MAJ);
                                intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);
                                intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(NON_MAJ, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                intLink.updateBatchAmount(NON_MAJ, invoiceInfo.amount);
                            }
                            else
                            {
                                intLink.UpdateBatchCount(NON_MAJ);
                                intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);
                                intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(NON_MAJ, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                intLink.updateBatchAmount(NON_MAJ, invoiceInfo.amount);
                            }
                        }
                        else
                        {
                            log.Save("Could not process this invoice. No matching conditions found to assign this invoice to an appropriate batch in SAGE");
                        }
                    }
                }

                else
                {
                    log.Save("Invoice not Transferred as fiscal year " + financialyear + " not yet Created in Sage. ");
                }

            }
            else
            {
                log.Save("Accpac Version: " + accpacSession.AppVersion);
                log.Save("Company: " + accpacSession.CompanyName);
                log.Save("Session Status: " + accpacSession.IsOpened);
            }
        }

        private void TableDependInfo_OnChanged(object sender, RecordChangedEventArgs<SqlNotify_DocumentInfo> e)
        {
            Thread.Sleep(500);
            log = new Log();
            if (isAccpacSessionOpen())
            {
                log.Save("Database change detected, operation: " + e.ChangeType);
                try
                {
                    var docInfo = e.Entity;
                    if (e.ChangeType == ChangeType.Insert)
                    {
                        if (docInfo.DocumentType == INVOICE)
                        {
                            log.Save($"Incoming invoice for invoice: {docInfo.OriginalDocumentID}");
                            InvoiceInfo invoiceInfo = new InvoiceInfo();

                            while (invoiceInfo.amount == 0)
                            {
                                log.Save("Waiting for invoice amount to update, current value: " + invoiceInfo.amount.ToString());
                                invoiceInfo = intLink.getInvoiceDetails(docInfo.OriginalDocumentID);
                            }
                            log.Save("Invoice amount: " + invoiceInfo.amount);

                            List<string> clientInfo = intLink.getClientInfo_inv(invoiceInfo.CustomerId.ToString());
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

                            log.Save("Client name: " + companyName);
                            Data dt = Translate(cNum, invoiceInfo.FeeType, companyName, "", invoiceInfo.notes, intLink.GetAccountNumber(invoiceInfo.Glid), invoiceInfo.FreqUsage); // application stops here...
                            DateTime invoiceValidity = intLink.GetValidity(docInfo.OriginalDocumentID); // or here...
                            log.Save("Invoice Validity: " + invoiceValidity);

                            int financialyear = 0;
                            if (invoiceValidity.Month > 3)
                            {
                                financialyear = invoiceValidity.Year + 1;
                            }
                            else
                            {
                                financialyear = invoiceValidity.Year;
                            }
                            log.Save("Financial Year: " + financialyear);

                            List<string> data = intLink.checkInvoiceAvail(docInfo.OriginalDocumentID.ToString());
                            int r = intLink.getInvoiceReference(docInfo.OriginalDocumentID);
                            log.Save("Invoice Reference number: " + r);

                            if (r != -1)
                            {
                                log.Save("Getting Maj Details...");
                                m = intLink.getMajDetail(r);
                                log.Save(m.ToString());
                            }

                            if (isPeriodCreated(financialyear))
                            {
                                if (invoiceInfo.Glid < 5000 || data != null)
                                {
                                    log.Save("CreditGL: " + invoiceInfo.Glid);
                                    if (data != null)
                                    {
                                        if (data[1].ToString() == "NT")
                                        {
                                            if (dt.feeType == "SLF" && invoiceInfo.notes == "Renewal")
                                            {
                                                stat = InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), getBatch(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()).ToString());

                                                if (stat.status == "Not Exist")
                                                {
                                                    CreateCustomer(dt.customerId, dt.companyName_NewCust);
                                                    InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), getBatch(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()).ToString());

                                                    intLink.UpdateBatchCount(RENEWAL_SPEC + "For " + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString());
                                                    intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                                    intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);
                                                    intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                                    intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                                }
                                                else
                                                {
                                                    intLink.UpdateBatchCount(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString());
                                                    intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                                    intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);
                                                    intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString() + " For " + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                                    intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                                }
                                            }
                                            else if (dt.feeType == "RF" && invoiceInfo.notes == "Renewal")
                                            {
                                                stat = InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), getBatch(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()).ToString());

                                                if (stat.status == "Not Exist")
                                                {
                                                    CreateCustomer(dt.customerId, dt.companyName_NewCust);
                                                    InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), getBatch(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()).ToString());

                                                    intLink.UpdateBatchCount(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString());
                                                    intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                                    intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);
                                                    intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                                    intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                                }
                                                else
                                                {
                                                    intLink.UpdateBatchCount(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString());
                                                    intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                                    intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);
                                                    intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                                    intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                                }
                                            }
                                            else if ((invoiceInfo.notes == "Annual Fee" && m.stationType == "SSL" && m.certificateType == 0 && m.proj == "JMC") || (invoiceInfo.FreqUsage == "PRS55"))
                                            {
                                                stat = InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), getBatch(MAJ, docInfo.OriginalDocumentID.ToString()).ToString());

                                                if (stat.status == "Not Exist")
                                                {
                                                    CreateCustomer(dt.customerId, dt.companyName_NewCust);
                                                    InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), getBatch(MAJ, docInfo.OriginalDocumentID.ToString()).ToString());

                                                    intLink.UpdateBatchCount(MAJ);
                                                    intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                                    intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);
                                                    intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(MAJ, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                                    intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                                }
                                                else
                                                {
                                                    intLink.UpdateBatchCount(MAJ);
                                                    intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                                    intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);
                                                    intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(MAJ, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                                    intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                                }
                                            }
                                            else if (invoiceInfo.notes == "Type Approval" ||  invoiceInfo.notes == "TypeApproval" || invoiceInfo.FreqUsage == "TA-ProAmend")
                                            {
                                                stat = InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, changetous(invoiceInfo.amount).ToString(), getBatch(TYPE_APPROVAL, docInfo.OriginalDocumentID.ToString()).ToString());

                                                if (stat.status == "Not Exist")
                                                {
                                                    CreateCustomer(dt.customerId, dt.companyName_NewCust);
                                                    InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, changetous(invoiceInfo.amount).ToString(), getBatch(TYPE_APPROVAL, docInfo.OriginalDocumentID.ToString()).ToString());

                                                    intLink.UpdateBatchCount(TYPE_APPROVAL);
                                                    intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                                    intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);
                                                    intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(TYPE_APPROVAL, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", intLink.GetRate(), changetous(invoiceInfo.amount), invoiceInfo.isvoided, 0, 0);
                                                    intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                                }
                                                else
                                                {
                                                    intLink.UpdateBatchCount(TYPE_APPROVAL);
                                                    intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                                    intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);
                                                    intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(TYPE_APPROVAL, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", intLink.GetRate(), changetous(invoiceInfo.amount), invoiceInfo.isvoided, 0, 0);
                                                    intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                                }
                                            }

                                            else if (invoiceInfo.notes == "Annual Fee" || invoiceInfo.notes == "Modification" || invoiceInfo.notes == "Radio Operator")
                                            {
                                                stat = InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), getBatch(NON_MAJ, docInfo.OriginalDocumentID.ToString()).ToString());

                                                if (stat.status == "Not Exist")
                                                {
                                                    CreateCustomer(dt.customerId, dt.companyName_NewCust);
                                                    InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), getBatch(NON_MAJ, docInfo.OriginalDocumentID.ToString()).ToString());

                                                    intLink.UpdateBatchCount(NON_MAJ);
                                                    intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                                    intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);
                                                    intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(NON_MAJ, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                                    intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                                }
                                                else
                                                {
                                                    intLink.UpdateBatchCount(NON_MAJ);
                                                    intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                                    intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);
                                                    intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(NON_MAJ, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                                    intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                                }
                                            }
                                            else
                                            {
                                                log.Save("Could not process this invoice. No matching conditions found to assign this invoice to an appropriate batch in SAGE");
                                            }
                                        }
                                        else if (data[1].ToString() == "T")
                                        {
                                            List<string> detail = new List<string>(3);
                                            detail = intLink.GetInvoiceDetails(docInfo.OriginalDocumentID);
                                            int batchNumber = getIbatchNumber(docInfo.OriginalDocumentID);


                                            if (!checkAccpacIBatchPosted(batchNumber))
                                            {
                                                if (invoiceInfo.Glid != Convert.ToInt32(detail[2].ToString()) || invoiceInfo.amount.ToString() != detail[3].ToString())
                                                {
                                                    if (invoiceInfo.notes == "Type Approval" || invoiceInfo.notes == "TypeApproval"  || invoiceInfo.FreqUsage == "TA-ProAmend")
                                                    {
                                                        string usamt = "";
                                                        usamt = changetousupdated(invoiceInfo.amount, docInfo.OriginalDocumentID).ToString();
                                                        UpdateInvoice(dt.fcode, Math.Round(changetousupdated(invoiceInfo.amount, docInfo.OriginalDocumentID), 2).ToString(), batchNumber.ToString().ToString(), getEntryNumber(docInfo.OriginalDocumentID).ToString());
                                                        intLink.storeInvoice(docInfo.OriginalDocumentID, batchNumber, invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "updated", 1, Convert.ToDecimal(usamt), invoiceInfo.isvoided, 0, 0);
                                                    }
                                                    else
                                                    {
                                                        UpdateInvoice(dt.fcode, invoiceInfo.amount.ToString(), batchNumber.ToString().ToString(), getEntryNumber(docInfo.OriginalDocumentID).ToString());
                                                        intLink.storeInvoice(docInfo.OriginalDocumentID, batchNumber, invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "updated", 1, 0, invoiceInfo.isvoided, 0, 0);

                                                    }
                                                    intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);
                                                    log.Save("Updated Invoice: " + docInfo.OriginalDocumentID.ToString());
                                                }
                                                else
                                                {
                                                    log.Save("Update is not needed." + docInfo.OriginalDocumentID.ToString());
                                                }
                                            }
                                            else
                                            {
                                                log.Save($"Batch number already posted: {batchNumber}");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (dt.feeType == "SLF" && invoiceInfo.notes == "Renewal")
                                        {
                                            intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                            log.Save("Invoice Id: " + docInfo.OriginalDocumentID.ToString() + " Transferred");
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

                                            intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                            log.Save("Invoice Id: " + docInfo.OriginalDocumentID.ToString() + " Transferred");
                                        }

                                        else if ((invoiceInfo.notes == "Annual Fee" && m.stationType == "SSL" && m.certificateType == 0 && m.proj == "JMC") || (invoiceInfo.FreqUsage == "PRS55"))
                                        {
                                            intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(MAJ, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                            log.Save("Invoice Id: " + docInfo.OriginalDocumentID.ToString() + " Transferred");
                                        }
                                        else if (invoiceInfo.notes == "Type Approval" || invoiceInfo.notes == "TypeApproval" || invoiceInfo.FreqUsage == "TA-ProAmend")
                                        {
                                            intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(TYPE_APPROVAL, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", intLink.GetRate(), changetous(invoiceInfo.amount), invoiceInfo.isvoided, 0, 0);
                                            log.Save("Invoice Id: " + docInfo.OriginalDocumentID.ToString() + " Transferred");
                                        }
                                        else if (invoiceInfo.notes == "Annual Fee" || invoiceInfo.notes == "Modification" || invoiceInfo.notes == "Radio Operator")
                                        {
                                            intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(NON_MAJ, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                            log.Save("Invoice Id: " + docInfo.OriginalDocumentID.ToString() + " Transferred");
                                        }
                                        else
                                        {
                                            log.Save("Could not process this invoice. No matching conditions found to assign this invoice to an appropriate batch in SAGE");
                                        }
                                    }
                                }
                                else
                                {
                                    if (dt.feeType == "SLF" && invoiceInfo.notes == "Renewal")
                                    {
                                        stat = InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), getBatch(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()).ToString());

                                        if (stat.status == "Not Exist")
                                        {
                                            CreateCustomer(dt.customerId, dt.companyName_NewCust);
                                            InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), getBatch(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()).ToString());

                                            intLink.UpdateBatchCount(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString());
                                            intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                            intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);
                                            intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                            intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                            intLink.updateBatchAmount(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), invoiceInfo.amount);
                                        }
                                        else
                                        {
                                            intLink.UpdateBatchCount(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString());
                                            intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                            intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);
                                            intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                            intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                            intLink.updateBatchAmount(RENEWAL_SPEC + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), invoiceInfo.amount);
                                        }
                                    }
                                    else if (dt.feeType == "RF" && invoiceInfo.notes == "Renewal")
                                    {
                                        stat = InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), getBatch(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()).ToString());

                                        if (stat.status == "Not Exist")
                                        {
                                            CreateCustomer(dt.customerId, dt.companyName_NewCust);
                                            InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), getBatch(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()).ToString());

                                            intLink.UpdateBatchCount(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString());
                                            intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                            intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);
                                            intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                            intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                            intLink.updateBatchAmount(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), invoiceInfo.amount);
                                        }
                                        else
                                        {
                                            intLink.UpdateBatchCount(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString());
                                            intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                            intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);
                                            intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                            intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                            intLink.updateBatchAmount(RENEWAL_REG + invoiceValidity.ToString("MMMM") + " " + invoiceValidity.Year.ToString(), invoiceInfo.amount);
                                        }
                                    }

                                    else if ((invoiceInfo.notes == "Annual Fee" && m.stationType == "SSL" && m.certificateType == 0 && m.proj == "JMC") || (invoiceInfo.FreqUsage == "PRS55"))
                                    {
                                        stat = InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), getBatch(MAJ, docInfo.OriginalDocumentID.ToString()).ToString());

                                        if (stat.status == "Not Exist")
                                        {
                                            CreateCustomer(dt.customerId, dt.companyName_NewCust);
                                            InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), getBatch(MAJ, docInfo.OriginalDocumentID.ToString()).ToString());

                                            intLink.UpdateBatchCount(MAJ);
                                            intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                            intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);
                                            intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(MAJ, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                            intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                            intLink.updateBatchAmount(MAJ, invoiceInfo.amount);
                                        }
                                        else
                                        {
                                            intLink.UpdateBatchCount(MAJ);
                                            intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                            intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);
                                            intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(MAJ, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                            intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                            intLink.updateBatchAmount(MAJ, invoiceInfo.amount);
                                        }
                                    }
                                    else if (invoiceInfo.notes == "Type Approval" || invoiceInfo.notes == "TypeApproval" || invoiceInfo.FreqUsage == "TA-ProAmend")
                                    {
                                        stat = InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, changetous(invoiceInfo.amount).ToString(), getBatch(TYPE_APPROVAL, docInfo.OriginalDocumentID.ToString()).ToString());

                                        if (stat.status == "Not Exist")
                                        {
                                            CreateCustomer(dt.customerId, dt.companyName_NewCust);
                                            InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, changetous(invoiceInfo.amount).ToString(), getBatch(TYPE_APPROVAL, docInfo.OriginalDocumentID.ToString()).ToString());

                                            intLink.UpdateBatchCount(TYPE_APPROVAL);
                                            intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                            intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);
                                            intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(TYPE_APPROVAL, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", intLink.GetRate(), changetous(invoiceInfo.amount), invoiceInfo.isvoided, 0, 0);
                                            intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                            intLink.updateBatchAmount(TYPE_APPROVAL, invoiceInfo.amount);
                                        }
                                        else
                                        {
                                            intLink.UpdateBatchCount(TYPE_APPROVAL);
                                            intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                            intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);
                                            intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(TYPE_APPROVAL, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", intLink.GetRate(), changetous(invoiceInfo.amount), invoiceInfo.isvoided, 0, 0);
                                            intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                            intLink.updateBatchAmount(TYPE_APPROVAL, invoiceInfo.amount);
                                        }
                                    }

                                    else if (invoiceInfo.notes == "Annual Fee" || invoiceInfo.notes == "Modification" || invoiceInfo.notes == "Radio Operator")
                                    {
                                        stat = InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), getBatch(NON_MAJ, docInfo.OriginalDocumentID.ToString()).ToString());

                                        if (stat.status == "Not Exist")
                                        {
                                            CreateCustomer(dt.customerId, dt.companyName_NewCust);
                                            InvBatchInsert(dt.customerId, docInfo.OriginalDocumentID.ToString(), dt.companyName, dt.fcode, invoiceInfo.amount.ToString(), getBatch(NON_MAJ, docInfo.OriginalDocumentID.ToString()).ToString());

                                            intLink.UpdateBatchCount(NON_MAJ);
                                            intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                            intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);
                                            intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(NON_MAJ, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                            intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                            intLink.updateBatchAmount(NON_MAJ, invoiceInfo.amount);
                                        }
                                        else
                                        {
                                            intLink.UpdateBatchCount(NON_MAJ);
                                            intLink.UpdateEntryNumber(docInfo.OriginalDocumentID);
                                            intLink.UpdateCreditGl(docInfo.OriginalDocumentID, invoiceInfo.Glid);
                                            intLink.storeInvoice(docInfo.OriginalDocumentID, getBatch(NON_MAJ, docInfo.OriginalDocumentID.ToString()), invoiceInfo.Glid, companyName, dt.customerId, DateTime.Now, invoiceInfo.Author, invoiceInfo.amount, "no modification", 1, 0, invoiceInfo.isvoided, 0, 0);
                                            intLink.MarkAsTransferred(docInfo.OriginalDocumentID);
                                            intLink.updateBatchAmount(NON_MAJ, invoiceInfo.amount);
                                        }
                                    }
                                    else
                                    {
                                        log.Save("Could not process this invoice. No matching conditions found to assign this invoice to an appropriate batch in SAGE");
                                    }
                                }
                            }

                            else
                            {
                                log.Save("Invoice not Transferred as fiscal year " + financialyear + " not yet Created in Sage. ");
                            }
                        }
                        else if (docInfo.DocumentType == RECEIPT && docInfo.PaymentMethod != 99)
                        {
                            log.Save($"Incoming Receipt for documentId: {docInfo.DocumentID}");
                            Thread.Sleep(1500);
                            Data dt = new Data();
                            PaymentInfo pinfo = new PaymentInfo();

                            List<string> paymentData = new List<string>(3);
                            List<string> clientData = new List<string>(3);
                            List<string> feeData = new List<string>(3);

                            while (pinfo.ReceiptNumber == 0)
                            {
                                pinfo = intLink.getPaymentInfo(docInfo.OriginalDocumentID);
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

                            clientData = intLink.getClientInfo_inv(id);
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

                                log.Save("Invoice Id: " + invoiceId.ToString());
                                log.Save("Customer Id: " + customerId);

                                prepstat = "No";
                                valstart = intLink.GetValidity(Convert.ToInt32(invoiceId));
                                valend = intLink.GetValidityEnd(Convert.ToInt32(invoiceId));
                            }
                            else
                            {
                                prepstat = "Yes";
                                log.Save("Prepayment");
                                log.Save("Customer Id: " + customerId);

                                var gl = intLink.GetCreditGlID((transid + 1).ToString());

                                if (gl == 5150)
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
                                dt = Translate(customerId, ftype, companyName, debit, notes, "PREPAYMENT", intLink.getFreqUsage(Convert.ToInt32(invoiceId)));
                            }
                            else
                            {
                                dt = Translate(customerId, ftype, companyName, debit, notes, "", intLink.getFreqUsage(Convert.ToInt32(invoiceId)));
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
                                    log.Save("Bank: FGB JA$ CURRENT A/C");
                                    intLink.modifyInvoiceList(0, 1, dt.customerId);

                                    if (dt.success)
                                    {
                                        if (receiptBatchAvail("FGBJMREC"))
                                        {
                                            string reference = intLink.GetCurrentRef("FGBJMREC");
                                            log.Save("Target Batch: " + intLink.getRecieptBatch("FGBJMREC"));
                                            log.Save("Transferring Receipt");

                                            ReceiptTransfer(intLink.getRecieptBatch("FGBJMREC"), dt.customerId, dt.debit, dt.companyName, reference, invoiceId, paymentDate, dt.desc, customerId, valstart, valend);
                                            intLink.UpdateBatchCountPayment(intLink.getRecieptBatch("FGBJMREC"));
                                            intLink.UpdateReceiptNumber(receipt, intLink.GetCurrentRef("FGBJMREC"));
                                            intLink.IncrementReferenceNumber(intLink.getBankCodeId("FGBJMREC"), Convert.ToDecimal(dt.debit));
                                            intLink.storePayment(dt.customerId, companyName, DateTime.Now, invoiceId, Convert.ToDecimal(dt.debit), 0, prepstat, Convert.ToInt32(reference), Convert.ToInt32(glid), "No", 0);
                                        }
                                        else
                                        {
                                            string reference = intLink.GetCurrentRef("FGBJMREC");
                                            CreateReceiptBatchEx("FGBJMREC", "Middleware Generated Batch for FGBJMREC");
                                            intLink.openNewReceiptBatch(1, GetLastPaymentBatch(), "FGBJMREC");

                                            log.Save("Target Batch: " + intLink.getRecieptBatch("FGBJMREC"));
                                            log.Save("Transferring Receipt");

                                            ReceiptTransfer(intLink.getRecieptBatch("FGBJMREC"), dt.customerId, dt.debit, dt.companyName, reference, invoiceId, paymentDate, dt.desc, customerId, valstart, valend);
                                            intLink.UpdateBatchCountPayment(intLink.getRecieptBatch("FGBJMREC"));
                                            intLink.UpdateReceiptNumber(receipt, intLink.GetCurrentRef("FGBJMREC"));
                                            intLink.IncrementReferenceNumber(intLink.getBankCodeId("FGBJMREC"), Convert.ToDecimal(dt.debit));
                                            intLink.storePayment(dt.customerId, companyName, DateTime.Now, invoiceId, Convert.ToDecimal(dt.debit), 0, prepstat, Convert.ToInt32(reference), Convert.ToInt32(glid), "No", 0);
                                        }
                                    }
                                }
                                else if (glid == "5147")
                                {
                                    log.Save("Bank: FGB US$ SAVINGS A/C");
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

                                    if (prepstat == "Yes")
                                    {
                                        if (CustomerExists(dt.customerId = clientIdPrefix + "-T")) currentRate = intLink.GetRate();
                                        else dt.customerId = intLink.getClientIdZRecord(false);
                                    }

                                    if (dt.customerId[6] == 'T')
                                    {
                                        usamount = Math.Round(Convert.ToDecimal(dt.debit) / intLink.GetUsRateByInvoice(Convert.ToInt32(invoiceId)), 2);
                                        intLink.modifyInvoiceList(0, intLink.GetUsRateByInvoice(Convert.ToInt32(invoiceId)), dt.customerId);
                                        currentRate = intLink.GetRate();
                                    }
                                    else intLink.modifyInvoiceList(0, 1, dt.customerId);

                                    if (dt.success)
                                    {
                                        if (receiptBatchAvail("FGBUSMRC"))
                                        {
                                            string reference = intLink.GetCurrentRef("FGBUSMRC");
                                            log.Save("Target Batch: " + intLink.getRecieptBatch("FGBUSMRC"));
                                            log.Save("Transferring Receipt");

                                            ReceiptTransfer(intLink.getRecieptBatch("FGBUSMRC"), dt.customerId, Math.Round(transferedAmt, 2).ToString(), dt.companyName, reference, invoiceId, paymentDate, dt.desc, customerId, valstart, valend);
                                            intLink.UpdateBatchCountPayment(intLink.getRecieptBatch("FGBUSMRC"));
                                            intLink.UpdateReceiptNumber(receipt, intLink.GetCurrentRef("FGBUSMRC"));
                                            intLink.IncrementReferenceNumber(intLink.getBankCodeId("FGBUSMRC"), Convert.ToDecimal(dt.debit));
                                            intLink.storePayment(dt.customerId, companyName, DateTime.Now, invoiceId, Convert.ToDecimal(dt.debit), usamount, prepstat, Convert.ToInt32(reference), Convert.ToInt32(glid), "No", currentRate);
                                        }
                                        else
                                        {
                                            string reference = intLink.GetCurrentRef("FGBUSMRC");
                                            CreateReceiptBatchEx("FGBUSMRC", "Middleware Generated Batch for FGBUSMRC");
                                            intLink.openNewReceiptBatch(1, GetLastPaymentBatch(), "FGBUSMRC");

                                            log.Save("Target Batch: " + intLink.getRecieptBatch("FGBUSMRC"));
                                            log.Save("Transferring Receipt");

                                            ReceiptTransfer(intLink.getRecieptBatch("FGBUSMRC"), dt.customerId, Math.Round(transferedAmt, 2).ToString(), dt.companyName, reference, invoiceId, paymentDate, dt.desc, customerId, valstart, valend);
                                            intLink.UpdateBatchCountPayment(intLink.getRecieptBatch("FGBUSMRC"));
                                            intLink.UpdateReceiptNumber(receipt, intLink.GetCurrentRef("FGBUSMRC"));
                                            intLink.IncrementReferenceNumber(intLink.getBankCodeId("FGBUSMRC"), Convert.ToDecimal(dt.debit));
                                            intLink.storePayment(dt.customerId, companyName, DateTime.Now, invoiceId, Convert.ToDecimal(dt.debit), usamount, prepstat, Convert.ToInt32(reference), Convert.ToInt32(glid), "No", currentRate);
                                        }
                                    }
                                }
                                else if (glid == "5148")
                                {
                                    log.Save("NCB JA$ SAVINGS A/C");
                                    intLink.modifyInvoiceList(0, 1, dt.customerId);

                                    if (dt.success)
                                    {
                                        if (receiptBatchAvail("NCBJMREC"))
                                        {
                                            string reference = intLink.GetCurrentRef("NCBJMREC");
                                            log.Save("Target Batch: " + intLink.getRecieptBatch("NCBJMREC"));
                                            log.Save("Transferring Receipt");

                                            ReceiptTransfer(intLink.getRecieptBatch("NCBJMREC"), dt.customerId, dt.debit, dt.companyName, reference, invoiceId, paymentDate, dt.desc, customerId, valstart, valend);
                                            intLink.UpdateBatchCountPayment(intLink.getRecieptBatch("NCBJMREC"));
                                            intLink.UpdateReceiptNumber(receipt, intLink.GetCurrentRef("NCBJMREC"));
                                            intLink.IncrementReferenceNumber(intLink.getBankCodeId("NCBJMREC"), Convert.ToDecimal(dt.debit));
                                            intLink.storePayment(dt.customerId, companyName, DateTime.Now, invoiceId, Convert.ToDecimal(dt.debit), 0, prepstat, Convert.ToInt32(reference), Convert.ToInt32(glid), "No", 1);
                                        }
                                        else
                                        {
                                            string reference = intLink.GetCurrentRef("NCBJMREC");
                                            CreateReceiptBatchEx("NCBJMREC", "Middleware Generated Batch for NCBJMREC");
                                            intLink.openNewReceiptBatch(1, GetLastPaymentBatch(), "NCBJMREC");

                                            log.Save("Target Batch: " + intLink.getRecieptBatch("NCBJMREC"));
                                            log.Save("Transferring Receipt");

                                            ReceiptTransfer(intLink.getRecieptBatch("NCBJMREC"), dt.customerId, dt.debit, dt.companyName, reference, invoiceId, paymentDate, dt.desc, customerId, valstart, valend);
                                            intLink.UpdateBatchCountPayment(intLink.getRecieptBatch("NCBJMREC"));
                                            intLink.UpdateReceiptNumber(receipt, intLink.GetCurrentRef("NCBJMREC"));
                                            intLink.IncrementReferenceNumber(intLink.getBankCodeId("NCBJMREC"), Convert.ToDecimal(dt.debit));
                                            intLink.storePayment(dt.customerId, companyName, DateTime.Now, invoiceId, Convert.ToDecimal(dt.debit), 0, prepstat, Convert.ToInt32(reference), Convert.ToInt32(glid), "No", 1);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                log.Save("Customer not found in accpac");
                            }
                        }
                        else if (docInfo.DocumentType == CREDIT_MEMO)
                        {
                            log.Save("New Credit Memo...");
                            CreditNoteInfo creditNote = new CreditNoteInfo();

                            while (creditNote.amount == 0)
                            {
                                creditNote = intLink.getCreditNoteInfo(docInfo.OriginalDocumentID, docInfo.DocumentID);
                                Thread.Sleep(1000);
                            }

                            List<string> clientInfo = new List<string>(4);
                            clientInfo = intLink.getClientInfo_inv(creditNote.CustomerID.ToString());
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

                            Data dt = Translate(cNum, creditNote.FeeType, companyName, creditNote.amount.ToString(), creditNote.notes, accountNum, intLink.getFreqUsage(creditNote.ARInvoiceID));

                            if (checkAccpacInvoiceAvail(creditNote.ARInvoiceID))
                            {
                                cred_docNum = intLink.getCreditMemoNumber();
                                log.Save("Creating credit memo");
                                int batchNumber = getBatch(CREDIT_NOTE, creditNote.ARInvoiceID.ToString());

                                intLink.storeInvoice(creditNote.ARInvoiceID, batchNumber, creditNote.CreditGL, companyName, dt.customerId, DateTime.Now, "", creditNote.amount, "no modification", 1, 0, 0, 1, cred_docNum);
                                CreditNoteInsert(batchNumber.ToString(), dt.customerId, accountNum, creditNote.amount.ToString(), creditNote.ARInvoiceID.ToString(), cred_docNum.ToString(), creditNoteDesc);
                                intLink.updateAsmsCreditMemoNumber(docInfo.DocumentID, cred_docNum);
                            }
                            else
                            {
                                log.Save("The Credit Memo was not created. The Invoice does not exist.");
                                cred_docNum = intLink.getCreditMemoNumber();
                                intLink.updateAsmsCreditMemoNumber(docInfo.DocumentID, cred_docNum);
                                log.Save("The Credit Memo number in ASMS updated.");
                            }
                        }
                        else if (docInfo.DocumentType == RECEIPT && docInfo.PaymentMethod == 99)
                        {
                            log.Save("Payment By Credit");
                            PaymentInfo pinfo = intLink.getPaymentInfo(docInfo.OriginalDocumentID);
                            List<string> clientData = intLink.getClientInfo_inv(pinfo.CustomerID.ToString());
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

                            Data dt = Translate(customerId, ftype, companyName, pinfo.Debit.ToString(), notes, "", intLink.getFreqUsage(pinfo.InvoiceID).ToString());
                            PrepaymentData pData = intLink.checkPrepaymentAvail(dt.customerId);
                            int invoiceBatch = getIbatchNumber(pinfo.InvoiceID);            //This value represents the batch that the invoice belongs to.
                            int receiptBatch = getRBatchNumber(pData.referenceNumber);      //This value represents the batch that the receipt belongs to.

                            if (pData.dataAvail)
                            {
                                if (checkAccpacIBatchPosted(invoiceBatch) && checkAccpacRBatchPosted(receiptBatch))
                                {
                                    var glid = pData.destinationBank.ToString();

                                    if (glid == "5146")
                                    {
                                        log.Save("Bank: FGB JA$ CURRENT A/C");
                                        if (pData.totalPrepaymentRemainder >= pinfo.Debit)
                                        {
                                            log.Save("Carrying out Payment By Credit Transaction");
                                            decimal reducingAmt = pinfo.Debit;

                                            if (receiptBatchAvail("FGBJMREC"))
                                            {
                                                while (reducingAmt > 0)
                                                {
                                                    pData = intLink.checkPrepaymentAvail(dt.customerId);
                                                    PayByCredit(dt.customerId, pinfo.InvoiceID.ToString(), intLink.getRecieptBatch("FGBJMREC"), getDocNumber(pData.referenceNumber));
                                                    if (reducingAmt > pData.remainder) intLink.adjustPrepaymentRemainder(pData.remainder, pData.sequenceNumber);
                                                    else intLink.adjustPrepaymentRemainder(reducingAmt, pData.sequenceNumber);
                                                    reducingAmt = reducingAmt - pData.remainder;
                                                }
                                                intLink.storePayment(dt.customerId, companyName, DateTime.Now, pinfo.InvoiceID.ToString(), pinfo.Debit, 0, "No", 0, Convert.ToInt32(glid), "Yes", 1);
                                                log.Save("Payment by credit transaction complete");
                                            }
                                            else
                                            {
                                                CreateReceiptBatchEx("FGBJMREC", "Middleware Generated Batch for FGBJMREC");
                                                intLink.openNewReceiptBatch(1, GetLastPaymentBatch(), "FGBJMREC");
                                                log.Save("Target Batch: " + intLink.getRecieptBatch("FGBJMREC"));
                                                while (reducingAmt > 0)
                                                {
                                                    pData = intLink.checkPrepaymentAvail(dt.customerId);
                                                    PayByCredit(dt.customerId, pinfo.InvoiceID.ToString(), intLink.getRecieptBatch("FGBJMREC"), getDocNumber(pData.referenceNumber));
                                                    if (reducingAmt > pData.remainder) intLink.adjustPrepaymentRemainder(pData.remainder, pData.sequenceNumber);
                                                    else intLink.adjustPrepaymentRemainder(reducingAmt, pData.sequenceNumber);
                                                    reducingAmt = reducingAmt - pData.remainder;
                                                }
                                                intLink.storePayment(dt.customerId, companyName, DateTime.Now, pinfo.InvoiceID.ToString(), pinfo.Debit, 0, "No", 0, Convert.ToInt32(glid), "Yes", 1);
                                                log.Save("Payment by credit transaction complete");
                                            }
                                        }
                                        else
                                        {
                                            log.Save("Prepayment balance not enough to carry out Transaction");
                                        }
                                    }

                                    if (glid == "5147")
                                    {
                                        log.Save("Bank: FGB US$ SAVINGS A/C");
                                        decimal usRate = intLink.GetUsRateByInvoice(pinfo.InvoiceID); //using the US rate from the Invoice at the time it was created.
                                        decimal usAmount = Convert.ToDecimal(dt.debit) / usRate;
                                        if (pData.totalPrepaymentRemainder >= usAmount)
                                        {
                                            log.Save("Carrying out Payment By Credit Transaction");
                                            decimal reducingAmt = usAmount;

                                            if (receiptBatchAvail("FGBUSMRC"))
                                            {
                                                while (reducingAmt > 0)
                                                {
                                                    pData = intLink.checkPrepaymentAvail(dt.customerId);
                                                    PayByCredit(dt.customerId, pinfo.InvoiceID.ToString(), intLink.getRecieptBatch("FGBUSMRC"), getDocNumber(pData.referenceNumber));
                                                    if (reducingAmt > pData.remainder) intLink.adjustPrepaymentRemainder(pData.remainder, pData.sequenceNumber);
                                                    else intLink.adjustPrepaymentRemainder(reducingAmt, pData.sequenceNumber);
                                                    reducingAmt = reducingAmt - pData.remainder;
                                                }
                                                if (usRate == 1) intLink.storePayment(dt.customerId, companyName, DateTime.Now, pinfo.InvoiceID.ToString(), pinfo.Debit, 0, "No", 0, Convert.ToInt32(glid), "Yes", 1);
                                                else intLink.storePayment(dt.customerId, companyName, DateTime.Now, pinfo.InvoiceID.ToString(), pinfo.Debit, usAmount, "No", 0, Convert.ToInt32(glid), "Yes", 1);
                                                log.Save("Payment by credit transaction complete");
                                            }
                                            else
                                            {
                                                CreateReceiptBatchEx("FGBUSMRC", "Middleware Generated Batch for FGBUSMRC");
                                                intLink.openNewReceiptBatch(1, GetLastPaymentBatch(), "FGBUSMRC");
                                                log.Save("Target Batch: " + intLink.getRecieptBatch("FGBUSMRC"));

                                                while (reducingAmt > 0)
                                                {
                                                    pData = intLink.checkPrepaymentAvail(dt.customerId);
                                                    PayByCredit(dt.customerId, pinfo.InvoiceID.ToString(), intLink.getRecieptBatch("FGBUSMRC"), getDocNumber(pData.referenceNumber));
                                                    if (reducingAmt > pData.remainder) intLink.adjustPrepaymentRemainder(pData.remainder, pData.sequenceNumber);
                                                    else intLink.adjustPrepaymentRemainder(reducingAmt, pData.sequenceNumber);
                                                    reducingAmt = reducingAmt - pData.remainder;
                                                }

                                                if (usRate == 1)
                                                {
                                                    intLink.storePayment(dt.customerId, companyName, DateTime.Now, pinfo.InvoiceID.ToString(), pinfo.Debit, 0, "No", 0, Convert.ToInt32(glid), "Yes", 1);
                                                }
                                                else
                                                {
                                                    intLink.storePayment(dt.customerId, companyName, DateTime.Now, pinfo.InvoiceID.ToString(), pinfo.Debit, usAmount, "No", 0, Convert.ToInt32(glid), "Yes", 1);

                                                }
                                                log.Save("Payment by credit transaction complete");
                                            }
                                        }
                                        else
                                        {
                                            log.Save("Prepayment balance not enough to carry out Transaction");
                                        }
                                    }

                                    if (glid == "5148")
                                    {
                                        log.Save("Bank: NCB JA$ SAVINGS A/C");
                                        if (pData.totalPrepaymentRemainder >= pinfo.Debit)
                                        {
                                            log.Save("Carrying out Payment By Credit Transaction");
                                            decimal reducingAmt = pinfo.Debit;

                                            if (receiptBatchAvail("NCBJMREC"))
                                            {
                                                while (reducingAmt > 0)
                                                {
                                                    pData = intLink.checkPrepaymentAvail(dt.customerId);
                                                    PayByCredit(dt.customerId, pinfo.InvoiceID.ToString(), intLink.getRecieptBatch("NCBJMREC"), getDocNumber(pData.referenceNumber));
                                                    if (reducingAmt > pData.remainder) intLink.adjustPrepaymentRemainder(pData.remainder, pData.sequenceNumber);
                                                    else intLink.adjustPrepaymentRemainder(reducingAmt, pData.sequenceNumber);
                                                    reducingAmt = reducingAmt - pData.remainder;
                                                }
                                                intLink.storePayment(dt.customerId, companyName, DateTime.Now, pinfo.InvoiceID.ToString(), pinfo.Debit, 0, "No", 0, Convert.ToInt32(glid), "Yes", 1);
                                                log.Save("Payment by credit transaction complete");
                                            }
                                            else
                                            {
                                                CreateReceiptBatchEx("NCBJMREC", "Middleware Generated Batch for NCBJMREC");
                                                intLink.openNewReceiptBatch(1, GetLastPaymentBatch(), "NCBJMREC");
                                                log.Save("Target Batch: " + intLink.getRecieptBatch("NCBJMREC"));
                                                while (reducingAmt > 0)
                                                {
                                                    pData = intLink.checkPrepaymentAvail(dt.customerId);
                                                    PayByCredit(dt.customerId, pinfo.InvoiceID.ToString(), intLink.getRecieptBatch("NCBJMREC"), getDocNumber(pData.referenceNumber));
                                                    if (reducingAmt > pData.remainder) intLink.adjustPrepaymentRemainder(pData.remainder, pData.sequenceNumber);
                                                    else intLink.adjustPrepaymentRemainder(reducingAmt, pData.sequenceNumber);
                                                    reducingAmt = reducingAmt - pData.remainder;
                                                }
                                                intLink.storePayment(dt.customerId, companyName, DateTime.Now, pinfo.InvoiceID.ToString(), pinfo.Debit, 0, "No", 0, Convert.ToInt32(glid), "Yes", 1);
                                                log.Save("Payment by credit transaction complete");
                                            }
                                        }
                                        else
                                        {
                                            log.Save("Prepayment balance not enough to carry out Transaction");
                                        }
                                    }

                                    if (glid != "5146" && glid != "5147" && glid != "5148")
                                    {
                                        log.Save("Bank Selected Not found, Cannot complete Transaction");
                                    }
                                }
                                else
                                {
                                    log.Save("Both invoice and receipt must be posted before attempting this transaction");
                                }
                            }
                            else
                            {
                                log.Save("No prepayment record found for customer: " + dt.customerId);
                            }
                        }
                    }
                    log.WriteEnd();
                }
                catch (Exception ex)
                {
                    if (accpacSession.Errors.Count > 0)
                    {
                        log.Save(ex.Message + " " + ex.StackTrace);
                        for (int i = 0; i < accpacSession.Errors.Count; i++)
                        {
                            log.Save(accpacSession.Errors[i].Message + ", Severity: " + accpacSession.Errors[i].Priority);
                        }
                        accpacSession.Errors.Clear();
                        log.WriteEnd();
                    }
                    else
                    {
                        log.Save(ex.Message + " " + ex.StackTrace);
                        log.WriteEnd();
                    }
                }
            }
            else
            {
                log.Save("Accpac Version: " + accpacSession.AppVersion);
                log.Save("Company: " + accpacSession.CompanyName);
                log.Save("Session Status: " + accpacSession.IsOpened);
            }
        }

        private void ManualInvoiceCancellation(SqlNotifyCancellation values)
        {
            log = new Log();
            if (isAccpacSessionOpen())
            {
                try
                {
                    var customerId = values.CustomerID;
                    var invoiceId = values.ARInvoiceID;
                    var amount = values.Amount;
                    var feeType = values.FeeType;
                    var notes = values.notes;
                    var cancelledBy = values.canceledBy;

                    if (cancelledBy != null)
                    {
                        string freqUsage = intLink.getFreqUsage(invoiceId);
                        DateTime invoiceValidity = intLink.GetValidity(invoiceId);
                        var creditGl = intLink.GetCreditGl(invoiceId.ToString());
                        var accountNum = intLink.GetAccountNumber(creditGl);
                        List<string> clientInfo = new List<string>(4);
                        clientInfo = intLink.getClientInfo_inv(customerId.ToString());
                        intLink.getClientInfo_inv(customerId.ToString());

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

                        if (values.isVoided == 1)
                        {
                            log.Save("Cancellation found.");
                            if (checkAccpacInvoiceAvail(invoiceId))
                            {
                                int postedBatch = getIbatchNumber(invoiceId);
                                if (!invoiceDelete(invoiceId))
                                {
                                    log.Save("Creating a credit memo");
                                    int cred_docNum = intLink.getCreditMemoNumber();
                                    int batchNumber = getBatch(CREDIT_NOTE, invoiceId.ToString());
                                    intLink.storeInvoice(invoiceId, batchNumber, creditGl, companyName, dt.customerId, DateTime.Now, cancelledBy, amount, "no modification", 1, 0, 0, 1, cred_docNum);
                                    CreditNoteInsert(batchNumber.ToString(), dt.customerId, accountNum, amount.ToString(), invoiceId.ToString(), cred_docNum.ToString(), creditNoteDesc);
                                }
                                else
                                {
                                    Maj m = new Maj();
                                    int r = intLink.getInvoiceReference(invoiceId);

                                    if (r != -1)
                                    {
                                        m = intLink.getMajDetail(r);
                                    }

                                    if (dt.feeType == "SLF" && notes == "Renewal")
                                    {
                                        intLink.storeInvoice(Convert.ToInt32(invoiceId), postedBatch, creditGl, companyName, dt.customerId, DateTime.Now, cancelledBy, Convert.ToDecimal(amount), "no modification", 1, 0, 1, 0, 0);
                                    }
                                    else if (dt.feeType == "RF" && notes == "Renewal")
                                    {
                                        intLink.storeInvoice(Convert.ToInt32(invoiceId), postedBatch, creditGl, companyName, dt.customerId, DateTime.Now, cancelledBy, Convert.ToDecimal(amount), "no modification", 1, 0, 1, 0, 0);
                                    }
                                    else if ((notes == "Annual Fee" && m.stationType == "SSL" && m.certificateType == 0 && m.proj == "JMC") || (freqUsage == "PRS55"))
                                    {
                                        intLink.storeInvoice(Convert.ToInt32(invoiceId), postedBatch, creditGl, companyName, dt.customerId, DateTime.Now, cancelledBy, Convert.ToDecimal(amount), "no modification", 1, 0, 1, 0, 0);
                                    }
                                    else if (notes == "Type Approval" || notes == "TypeApproval" || freqUsage == "TA-ProAmend")
                                    {
                                        intLink.storeInvoice(Convert.ToInt32(invoiceId), postedBatch, creditGl, companyName, dt.customerId, DateTime.Now, cancelledBy, Convert.ToDecimal(amount), "no modification", intLink.GetUsRateByInvoice(invoiceId), changetousupdated(Convert.ToDecimal(amount), invoiceId), 1, 0, 0);
                                    }
                                    else if (notes == "Annual Fee" || notes == "Modification" || notes == "Radio Operator")
                                    {
                                        intLink.storeInvoice(Convert.ToInt32(invoiceId), postedBatch, creditGl, companyName, dt.customerId, DateTime.Now, cancelledBy, Convert.ToDecimal(amount), "no modification", 1, 0, 1, 0, 0);
                                    }
                                }
                            }
                            else
                            {
                                log.Save("Invoice: " + invoiceId.ToString() + " was not found in Sage. Cannot delete.");
                            }
                        }
                        log.WriteEnd();
                    }
                }
                catch (Exception ex)
                {
                    if (accpacSession.Errors.Count > 0)
                    {
                        log.Save(ex.Message + " " + ex.StackTrace);
                        for (int i = 0; i < accpacSession.Errors.Count; i++)
                        {
                            log.Save(accpacSession.Errors[i].Message + ", Severity: " + accpacSession.Errors[i].Priority);
                        }
                        accpacSession.Errors.Clear();
                        log.WriteEnd();
                    }
                    else
                    {
                        log.Save(ex.Message + " " + ex.StackTrace);
                        log.WriteEnd();
                    }
                }
            }
            else
            {
                log.Save("Accpac Version: " + accpacSession.AppVersion);
                log.Save("Company: " + accpacSession.CompanyName);
                log.Save("Session Status: " + accpacSession.IsOpened);
            }
        }

        private void TableDependCancellation_OnChanged(object sender, RecordChangedEventArgs<SqlNotifyCancellation> e)
        {
            log = new Log();
            if (isAccpacSessionOpen())
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
                        string freqUsage = intLink.getFreqUsage(invoiceId);
                        DateTime invoiceValidity = intLink.GetValidity(invoiceId);
                        var creditGl = intLink.GetCreditGl(invoiceId.ToString());
                        var accountNum = intLink.GetAccountNumber(creditGl);
                        List<string> clientInfo = new List<string>(4);
                        clientInfo = intLink.getClientInfo_inv(customerId.ToString());
                        intLink.getClientInfo_inv(customerId.ToString());

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
                            log.Save("Cancellation found.");
                            if (checkAccpacInvoiceAvail(invoiceId))
                            {
                                int postedBatch = getIbatchNumber(invoiceId);
                                if (!invoiceDelete(invoiceId))
                                {
                                    log.Save("Creating a credit memo");
                                    int cred_docNum = intLink.getCreditMemoNumber();
                                    int batchNumber = getBatch(CREDIT_NOTE, invoiceId.ToString());
                                    intLink.storeInvoice(invoiceId, batchNumber, creditGl, companyName, dt.customerId, DateTime.Now, cancelledBy, amount, "no modification", 1, 0, 0, 1, cred_docNum);
                                    CreditNoteInsert(batchNumber.ToString(), dt.customerId, accountNum, amount.ToString(), invoiceId.ToString(), cred_docNum.ToString(), creditNoteDesc);
                                }
                                else
                                {
                                    Maj m = new Maj();
                                    int r = intLink.getInvoiceReference(invoiceId);

                                    if (r != -1)
                                    {
                                        m = intLink.getMajDetail(r);
                                    }

                                    if (dt.feeType == "SLF" && notes == "Renewal")
                                    {
                                        intLink.storeInvoice(Convert.ToInt32(invoiceId), postedBatch, creditGl, companyName, dt.customerId, DateTime.Now, cancelledBy, Convert.ToDecimal(amount), "no modification", 1, 0, 1, 0, 0);
                                    }
                                    else if (dt.feeType == "RF" && notes == "Renewal")
                                    {
                                        intLink.storeInvoice(Convert.ToInt32(invoiceId), postedBatch, creditGl, companyName, dt.customerId, DateTime.Now, cancelledBy, Convert.ToDecimal(amount), "no modification", 1, 0, 1, 0, 0);
                                    }
                                    else if ((notes == "Annual Fee" && m.stationType == "SSL" && m.certificateType == 0 && m.proj == "JMC") || (freqUsage == "PRS55"))
                                    {
                                        intLink.storeInvoice(Convert.ToInt32(invoiceId), postedBatch, creditGl, companyName, dt.customerId, DateTime.Now, cancelledBy, Convert.ToDecimal(amount), "no modification", 1, 0, 1, 0, 0);
                                    }
                                    else if (notes == "Type Approval" || notes == "TypeApproval" || freqUsage == "TA-ProAmend")
                                    {
                                        intLink.storeInvoice(Convert.ToInt32(invoiceId), postedBatch, creditGl, companyName, dt.customerId, DateTime.Now, cancelledBy, Convert.ToDecimal(amount), "no modification", intLink.GetUsRateByInvoice(invoiceId), changetousupdated(Convert.ToDecimal(amount), invoiceId), 1, 0, 0);
                                    }
                                    else if (notes == "Annual Fee" || notes == "Modification" || notes == "Radio Operator")
                                    {
                                        intLink.storeInvoice(Convert.ToInt32(invoiceId), postedBatch, creditGl, companyName, dt.customerId, DateTime.Now, cancelledBy, Convert.ToDecimal(amount), "no modification", 1, 0, 1, 0, 0);
                                    }
                                }
                            }
                            else
                            {
                                log.Save("Invoice: " + invoiceId.ToString() + " was not found in Sage. Cannot delete.");
                            }
                        }
                        log.WriteEnd();
                    }
                }
                catch (Exception ex)
                {
                    if (accpacSession.Errors.Count > 0)
                    {
                        log.Save(ex.Message + " " + ex.StackTrace);
                        for (int i = 0; i < accpacSession.Errors.Count; i++)
                        {
                            log.Save(accpacSession.Errors[i].Message + ", Severity: " + accpacSession.Errors[i].Priority);
                        }
                        accpacSession.Errors.Clear();
                        log.WriteEnd();
                    }
                    else
                    {
                        log.Save(ex.Message + " " + ex.StackTrace);
                        log.WriteEnd();
                    }
                }
            }
            else
            {
                log.Save("Accpac Version: " + accpacSession.AppVersion);
                log.Save("Company: " + accpacSession.CompanyName);
                log.Save("Session Status: " + accpacSession.IsOpened);
            }
        }

        InsertionReturn InvBatchInsert(string idCust, string docNum, string desc, string feeCode, string amt, string batchId)
        {
            log = new Log();
            var b1_arInvoiceBatch = dbLink.OpenView("AR0031");
            var b1_arInvoiceHeader = dbLink.OpenView("AR0032");
            var b1_arInvoiceDetail = dbLink.OpenView("AR0033");
            var b1_arInvoicePaymentSchedules = dbLink.OpenView("AR0034");
            var b1_arInvoiceHeaderOptFields = dbLink.OpenView("AR0402");
            var b1_arInvoiceDetailOptFields = dbLink.OpenView("AR0401");

            try
            {
                log.Save("Transfering Invoice " + docNum + " to batch: " + batchId);
                InsertionReturn success = new InsertionReturn();
                DateTime postDate = intLink.GetValidity(Convert.ToInt32(docNum));

                if (postDate < DateTime.Now) postDate = DateTime.Now;

                DateTime docDate = intLink.getDocDate(Convert.ToInt32(docNum));
                DateTime now = DateTime.Now;
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
                    log.Save("Invoice Id: " + docNum + " Transferred");
                }
                else
                {
                    success.status = "Not Exist";
                    log.Save("Customer does not exist.");
                }

                b1_arInvoiceBatch.Dispose();
                b1_arInvoiceDetail.Dispose();
                b1_arInvoiceDetailOptFields.Dispose();
                b1_arInvoiceHeader.Dispose();
                b1_arInvoiceHeaderOptFields.Dispose();
                b1_arInvoicePaymentSchedules.Dispose();
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

                return exist;
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

        Data Translate(string cNum, string feeType, string companyName, string debit, string notes, string _fcode, string FreqUsage)
        {
            string temp = "";
            string iv_customerId = "";
            Data dt = new Data();

            if (_fcode == "PREPAYMENT" && intLink.getClientIdZRecord(false).Contains("-T"))
            {
                dt.customerId = intLink.getClientIdZRecord(false);
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

                    else if (notes == "Type Approval" || notes == "TypeApproval" || FreqUsage == "TA-ProAmend")
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
            log = new Log();
            View ARCUSTOMER1header = dbLink.OpenView("AR0024");
            View ARCUSTOMER1detail = dbLink.OpenView("AR0400");
            View ARCUSTSTAT2 = dbLink.OpenView("AR0022");
            View ARCUSTCMT3 = dbLink.OpenView("AR0021");

            try
            {
                log.Save("Creating customer: " + nameCust + ", ID: " + idCust);
                string groupCode = "";
                ARCUSTOMER1header.Compose(new View[] { ARCUSTOMER1detail });
                ARCUSTOMER1detail.Compose(new View[] { ARCUSTOMER1header });

                if (idCust.Contains("-L"))
                {
                    groupCode = "LICCOM";
                }
                else if (idCust.Contains("-R"))
                {
                    groupCode = "REGCOM";
                }

                else if (idCust.Contains("-T"))
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
                log.Save("Customer created.");
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

        public string createBatchDesc(string batchType)
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

        public int getBatch(string batchType, string invoiceid)
        {
            DateTime val = intLink.GetValidity(Convert.ToInt32(invoiceid));
            string renspec = RENEWAL_SPEC + val.ToString("MMMM") + " " + val.Year.ToString();
            string renreg = RENEWAL_REG + val.ToString("MMMM") + " " + val.Year.ToString();

            if (intLink.batchAvail(batchType))
            {
                int batch = intLink.getAvailBatch(batchType);
                if (!intLink.isBatchExpired(batch))
                {
                    if (!checkAccpacIBatchPosted(batch))
                    {
                        return batch;
                    }
                    else
                    {
                        intLink.closeInvoiceBatch(batch);

                        int newbatch = GetLastInvoiceBatch() + 1;

                        if (batchType == renreg)
                        {
                            intLink.createInvoiceBatch(generateDaysExpire(batchType), newbatch, batchType, "Regulatory");
                        }

                        else if (batchType == renspec)
                        {
                            intLink.createInvoiceBatch(generateDaysExpire(batchType), newbatch, batchType, "Spectrum");
                        }

                        else
                        {
                            intLink.createInvoiceBatch(generateDaysExpire(batchType), newbatch, batchType, "");
                        }


                        if (batchType == renreg || batchType == renspec)
                        {
                            CreateInvoiceBatch(batchType);
                        }
                        else
                            CreateInvoiceBatch(createBatchDesc(batchType));

                        return newbatch;
                    }
                }
                else
                {
                    if (batchType == "" || batchType == "")
                    {
                        intLink.resetInvoiceTotal();
                    }

                    intLink.closeInvoiceBatch(batch);
                    int newbatch = GetLastInvoiceBatch() + 1;

                    if (batchType == renreg)
                    {
                        intLink.createInvoiceBatch(generateDaysExpire(batchType), newbatch, batchType, "Regulatory");
                    }
                    else if (batchType == renspec)
                    {
                        intLink.createInvoiceBatch(generateDaysExpire(batchType), newbatch, batchType, "Spectrum");
                    }
                    else
                    {
                        intLink.createInvoiceBatch(generateDaysExpire(batchType), newbatch, batchType, "");
                    }

                    if (batchType == renreg || batchType == renspec)
                    {
                        CreateInvoiceBatch(batchType);
                    }
                    else
                    {
                        CreateInvoiceBatch(createBatchDesc(batchType));
                    }

                    return newbatch;
                }

            }
            else
            {
                int newbatch = GetLastInvoiceBatch() + 1;
                if (batchType == renreg)
                {
                    intLink.createInvoiceBatch(generateDaysExpire(batchType), newbatch, batchType, "Regulatory");
                }

                else if (batchType == renspec)
                {
                    intLink.createInvoiceBatch(generateDaysExpire(batchType), newbatch, batchType, "Spectrum");
                }

                else
                {
                    intLink.createInvoiceBatch(generateDaysExpire(batchType), newbatch, batchType, "");
                }


                if (batchType == renreg || batchType == renspec)
                {
                    CreateInvoiceBatch(batchType);
                }
                else
                {
                    CreateInvoiceBatch(createBatchDesc(batchType));
                }
                return newbatch;
            }
        }

        public bool isAccpacSessionOpen()
        {
            log = new Log();
            if (accpacSession != null)
            {
                if (accpacSession.IsOpened)
                {
                    return true;
                }
                else
                {
                    log.Save("Accpac session is not opened");
                    return false;
                }
            }
            else
            {
                log.Save("Accpac session is not initialized");
                return false;
            }
        }


        public bool checkAccpacIBatchPosted(int batchNumber)
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
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

        int GetLastInvoiceBatch()
        {
            var b1_arInvoiceBatch = dbLink.OpenView("AR0031");

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
            var b1_arInvoiceBatch = dbLink.OpenView("AR0031");
            var b1_arInvoiceHeader = dbLink.OpenView("AR0032");
            var b1_arInvoiceDetail = dbLink.OpenView("AR0033");
            var b1_arInvoicePaymentSchedules = dbLink.OpenView("AR0034");
            var b1_arInvoiceHeaderOptFields = dbLink.OpenView("AR0402");
            var b1_arInvoiceDetailOptFields = dbLink.OpenView("AR0401");

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

        public int generateDaysExpire(string batchType)
        {
            if (batchType != NON_MAJ && batchType != TYPE_APPROVAL && batchType != ONE_DAY)
            {
                int expiry = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) - DateTime.Now.Day;
                expiry++;

                return expiry;
            }
            else return 1;
        }

        public bool isPeriodCreated(int finyear)
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

        public decimal changetous(decimal regamt)
        {
            decimal rate = intLink.GetRate();
            decimal usamt = regamt / rate;
            return Math.Round(usamt, 2);
        }

        public decimal changetousupdated(decimal regamt, int invnum)
        {
            decimal rate = intLink.GetUsRateByInvoice(invnum);
            decimal usamt = regamt / rate;
            return Math.Round(usamt, 2);
        }

        public int getIbatchNumber(int docNumber)
        {
            var cssql = dbLink.OpenView("CS0120");

            try
            {
                int batchNum = -1;
                cssql.Browse("SELECT CNTBTCH FROM ARIBH WHERE IDINVC = '" + docNumber.ToString() + "'", true);
                cssql.InternalSet(256);

                if (cssql.GoNext())
                {
                    batchNum = Convert.ToInt32(cssql.Fields.FieldByName("CNTBTCH").Value);
                }

                return batchNum;
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

        void UpdateInvoice(string accountNumber, string amt, string BatchId, string entryNumber)
        {
            var b1_arInvoiceBatch = dbLink.OpenView("AR0031");
            var b1_arInvoiceHeader = dbLink.OpenView("AR0032");
            var b1_arInvoiceDetail = dbLink.OpenView("AR0033");
            var b1_arInvoicePaymentSchedules = dbLink.OpenView("AR0034");
            var b1_arInvoiceHeaderOptFields = dbLink.OpenView("AR0402");
            var b1_arInvoiceDetailOptFields = dbLink.OpenView("AR0401");

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

        public int getEntryNumber(int docNumber)
        {
            var b1_arInvoiceHeader = dbLink.OpenView("AR0032");
            int entry = -1;

            try
            {
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
                    new Log().Save("Invoice ID: " + docNum + " was not found");
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

        public bool receiptBatchAvail(string bankcode)
        {
            var connIntegration = new SqlConnection(Constants.dbIntegration);
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
                        if (!checkAccpacRBatchPosted(receiptBatch))
                        {
                            truth = true;
                        }
                        else
                        {
                            intLink.closeReceiptBatch(receiptBatch);
                        }
                    }
                    else
                    {
                        intLink.closeReceiptBatch(receiptBatch);
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
                new Log().Save(e.Message + " " + e.StackTrace);
                connIntegration.Close();
                return false;
            }
        }

        public bool checkAccpacRBatchPosted(int batchNumber)
        {
            View cssql = dbLink.OpenView("CS0120"); ;

            try
            {
                cssql.Browse("SELECT CNTBTCH, BATCHSTAT FROM ARBTA WHERE CNTBTCH = '" + batchNumber.ToString() + "'", true);
                cssql.InternalSet(256);

                if (cssql.GoNext())
                {
                    string val = Convert.ToString(cssql.Fields.FieldByName("BATCHSTAT").Value);
                    cssql.Dispose();

                    if (val == "1")
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    cssql.Dispose();
                    return true;
                }
            }
            catch (Exception e)
            {
                cssql.Dispose();
                throw (e);
            }
        }

        public bool ReceiptTransfer(string batchNumber, string customerId, string amount, string receiptDescription, string referenceNumber, string invnum, DateTime paymentDate, string findesc, string cid, DateTime valstart, DateTime valend)
        {
            log = new Log();
            try
            {
                string notes = intLink.isAnnualFee(Convert.ToInt32(invnum));
                string receiptDescriptionEx;

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
                        var CBBTCH1batch = dbLink.OpenView("AR0041");
                        var CBBTCH1header = dbLink.OpenView("AR0042");
                        var CBBTCH1detail1 = dbLink.OpenView("AR0044");
                        var CBBTCH1detail2 = dbLink.OpenView("AR0045");
                        var CBBTCH1detail3 = dbLink.OpenView("AR0043");
                        var CBBTCH1detail4 = dbLink.OpenView("AR0061");
                        var CBBTCH1detail5 = dbLink.OpenView("AR0406");
                        var CBBTCH1detail6 = dbLink.OpenView("AR0170");

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

                        log.Save("Prepayment Transferred");
                        return true;
                    }
                    else
                    {
                        receiptDescription = receiptDescriptionEx;
                        int batchNum = getIbatchNumber(Convert.ToInt32(invnum));
                        bool shouldAllocate = false;

                        if (checkAccpacInvoiceAvail(Convert.ToInt32(invnum)))
                        {
                            if (checkAccpacIBatchPosted(batchNum))
                            {
                                shouldAllocate = true;
                            }
                        }

                        bool flagInsert;

                        var arRecptBatch = dbLink.OpenView("AR0041");
                        var arRecptHeader = dbLink.OpenView("AR0042");
                        var arRecptDetail1 = dbLink.OpenView("AR0044");
                        var arRecptDetail2 = dbLink.OpenView("AR0045");
                        var arRecptDetail3 = dbLink.OpenView("AR0043");
                        var arRecptDetail4 = dbLink.OpenView("AR0061");
                        var arRecptDetail5 = dbLink.OpenView("AR0406");
                        var arRecptDetail6 = dbLink.OpenView("AR0170");

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
                            log.Save("Applying receipt to invoice: " + invnum);
                            arRecptDetail4.Fields.FieldByName("SHOWTYPE").SetValue("2", false);
                            arRecptDetail4.Fields.FieldByName("STDOCSTR").SetValue(invnum, false);

                            arRecptDetail4.Process();
                            arRecptDetail4.Fields.FieldByName("CNTKEY").SetValue("-1", false);
                            arRecptDetail4.Read(false);

                            var netAmount = Convert.ToInt32(arRecptDetail4.Fields.FieldByName("AMTNET").Value);

                            if (netAmount == 0)
                            {
                                flagInsert = true;
                                log.Save("The net balance is zero for this invoice. Cannot apply an additional amount");
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
                            log.Save("Cannot apply to invoice: " + invnum + ". Check if invoice exist and the batch is posted.");
                            flagInsert = true;
                        }

                        if (flagInsert)
                        {
                            arRecptHeader.Insert();
                            arRecptHeader.RecordCreate(ViewRecordCreate.DelayKey);
                            log.Save("Receipt Transferred");
                        }
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

        public bool checkAccpacInvoiceAvail(int invoiceId)
        {
            View cssql = dbLink.OpenView("CS0120");

            try
            {
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
            catch (Exception e)
            {
                throw (e);
            }
        }

        void CreateReceiptBatchEx(string bankcode, string batchDesc)
        {
            var CBBTCH1batch = dbLink.OpenView("AR0041");
            var CBBTCH1header = dbLink.OpenView("AR0042");
            var CBBTCH1detail1 = dbLink.OpenView("AR0044");
            var CBBTCH1detail2 = dbLink.OpenView("AR0045");
            var CBBTCH1detail3 = dbLink.OpenView("AR0043");
            var CBBTCH1detail4 = dbLink.OpenView("AR0061");
            var CBBTCH1detail5 = dbLink.OpenView("AR0406");
            var CBBTCH1detail6 = dbLink.OpenView("AR0170");

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
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

        int GetLastPaymentBatch()
        {
            var CBBTCH1batch = dbLink.OpenView("AR0041");

            try
            {
                int BatchId = 0;
                bool gotIt;
                gotIt = CBBTCH1batch.GoBottom();

                if (gotIt)
                {
                    BatchId = Convert.ToInt32(CBBTCH1batch.Fields.FieldByName("CNTBTCH").Value);
                }

                return BatchId;
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

        public void CreditNoteInsert(string batchNumber, string customerId, string acctNumber, string amount, string invoiceToApply, string docNumber, string description)
        {
            var b1_arInvoiceBatch = dbLink.OpenView("AR0031");
            var b1_arInvoiceHeader = dbLink.OpenView("AR0032");
            var b1_arInvoiceDetail = dbLink.OpenView("AR0033");
            var b1_arInvoicePaymentSchedules = dbLink.OpenView("AR0034");
            var b1_arInvoiceHeaderOptFields = dbLink.OpenView("AR0402");
            var b1_arInvoiceDetailOptFields = dbLink.OpenView("AR0401");

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
                new Log().Save("Credit Memo transferred");

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

        public int getRBatchNumber(string referenceNumber)
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

        public string getDocNumber(string referenceNumber)
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
            var arRecptBatch = dbLink.OpenView("AR0041");
            var arRecptHeader = dbLink.OpenView("AR0042");
            var arRecptDetail1 = dbLink.OpenView("AR0044");
            var arRecptDetail2 = dbLink.OpenView("AR0045");
            var arRecptDetail3 = dbLink.OpenView("AR0043");
            var arRecptDetail4 = dbLink.OpenView("AR0061");
            var arRecptDetail5 = dbLink.OpenView("AR0406");
            var arRecptDetail6 = dbLink.OpenView("AR0170");

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

        public bool invoiceDelete(int invoiceId)
        {
            log = new Log();
            var b1_arInvoiceBatch = dbLink.OpenView("AR0031");
            var b1_arInvoiceHeader = dbLink.OpenView("AR0032");
            var b1_arInvoiceDetail = dbLink.OpenView("AR0033");
            var b1_arInvoicePaymentSchedules = dbLink.OpenView("AR0034");
            var b1_arInvoiceHeaderOptFields = dbLink.OpenView("AR0402");
            var b1_arInvoiceDetailOptFields = dbLink.OpenView("AR0401");

            try
            {
                int entryNumber = getEntryNumber(invoiceId);
                int batchNumber = getIbatchNumber(invoiceId);

                if (!checkAccpacIBatchPosted(batchNumber))
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

                    b1_arInvoiceBatch.Dispose();
                    b1_arInvoiceHeader.Dispose();
                    b1_arInvoiceDetail.Dispose();
                    b1_arInvoicePaymentSchedules.Dispose();
                    b1_arInvoiceHeaderOptFields.Dispose();
                    b1_arInvoiceDetailOptFields.Dispose();

                    log.Save("Invoice: " + invoiceId.ToString() + " was deleted from the batch (" + batchNumber + ")");
                    return true;
                }
                else
                {
                    b1_arInvoiceBatch.Dispose();
                    b1_arInvoiceHeader.Dispose();
                    b1_arInvoiceDetail.Dispose();
                    b1_arInvoicePaymentSchedules.Dispose();
                    b1_arInvoiceHeaderOptFields.Dispose();
                    b1_arInvoiceDetailOptFields.Dispose();

                    log.Save("The batch is already posted, cannot delete invoice");
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

        int countBatchPaymentEntries(string batchId)
        {
            var CBBTCH1batch = dbLink.OpenView("CB0009");
            var CBBTCH1header = dbLink.OpenView("CB0010");
            var CBBTCH1detail1 = dbLink.OpenView("CB0011");
            var CBBTCH1detail2 = dbLink.OpenView("CB0012");
            var CBBTCH1detail3 = dbLink.OpenView("CB0013");
            var CBBTCH1detail4 = dbLink.OpenView("CB0014");
            var CBBTCH1detail5 = dbLink.OpenView("CB0015");
            var CBBTCH1detail6 = dbLink.OpenView("CB0016");
            var CBBTCH1detail7 = dbLink.OpenView("CB0403");
            var CBBTCH1detail8 = dbLink.OpenView("CB0404");

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

                return count;
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

        public void OnDebug()
        {
            OnStart(null);
        }

        public void XrateInsert(string amt)
        {
            var csRateHeader = dbLink.OpenView("CS0005");
            var csRateDetail = dbLink.OpenView("CS0006");

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
                throw (e);
            }
        }
    }
}
