﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Web.Script.Serialization;
using System.ServiceProcess;


namespace WebApplication4
{
    /// <summary>
    /// Summary description for InteegrationService
    /// </summary>
    
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class InteegrationService : System.Web.Services.WebService
    {
      
        //Sector Details -----------------------------------------------------------------------------------------------
        [WebMethod]
        public List<InvoiceDetail> PendingInvoiceMessages()
        {
            Integration intlink = new Integration();
            List<InvoiceDetail> data = intlink.LatestPendingInvoice_Msg();
            return data;
        }

        [WebMethod]
        public List<Customer> CustomerCreatedMessages()
        {
            Integration intlink = new Integration();
            List<Customer> dt = intlink.LatestCreatedCustomer_Msg();
            return dt;
        }

        [WebMethod]
        public List<PaymentDetail> PaymentTransferredMessages()
        {
            Integration intlink = new Integration();
            List<PaymentDetail> data = intlink.LatestPaymentDetail_Msg();
            return data;
        }

        [WebMethod]
        public List<InvoiceDetail> InvoiceTransferredMessages()
        {
            Integration intlink = new Integration();
            List<InvoiceDetail> data = intlink.LatestTransferred_Msg();
            return data;
        }

        //Card Back Details ---------------------------------------------------------------------------------------------
        [WebMethod]
        public List<string> GetReceiptDetail()
        {
            List<string> data = new List<string>(7);
            Integration intLink = new Integration();

            data = intLink.ReceiptDetail();
            return data;
        }

        [WebMethod]
        public List<StoredInvoice> GetPendingInvoiceDetail()
        {
            List<StoredInvoice> data = new List<StoredInvoice>(7);
            Integration intLink = new Integration();

            data = intLink.PendingInvoices();
            return data;
        }

        [WebMethod]
        public List<InvoiceBatchInfo> GetInvoiceDetail()
        {
            List<InvoiceBatchInfo> data = new List<InvoiceBatchInfo>();
            Integration intLink = new Integration();

            data = intLink.InvoiceDetail();
            return data;
        }

        //Count ---------------------------------------------------------------------------------------------------------
        [WebMethod]
        public int GetInvoiceCount()
        {
            int i = 0;
            Integration intLink = new Integration();

            i = intLink.GetInvoiceCount();
            return i;
        }

        [WebMethod]
        public string GetInvoiceTotalAmount()
        {
            string i = "";
            Integration intlink = new Integration();
            i = intlink.InvoiceAmountTotal();
            return i;
        }

        [WebMethod]
        public int GetPaymentCount()
        {
            int i = 0;
            Integration intLink = new Integration();

            i = intLink.GetPaymentCount();
            return i;
        }

        [WebMethod]
        public int GetCustomerCount()
        {
            int i = 0;
            Integration intLink = new Integration();
            
            i = intLink.GetCustomerCount();
            return i;
        }

        [WebMethod]
        public int GetPendingCount()
        {
            int i = 0;
            Integration intLink = new Integration();
           
            i = intLink.GetPendiningInvCount();
            return i;
        }

        //Other ---------------------------------------------------------------------------------------------------------

        [WebMethod]
        public int GetMonStat()
        {
            ServiceController sc = new ServiceController("middleware_service");
            int result = -1;

            try
            {
                switch (sc.Status)
                {
                    case ServiceControllerStatus.Running:
                        result = 3;
                        break;
                    case ServiceControllerStatus.Stopped:
                        result = 2;
                        break;

                }
            }
            catch (Exception e)
            {
                result = -1;
            }
            return result;
        }

        [WebMethod]
        public void SendMessage(string msg)
        {
            Integration intlink = new Integration();
            intlink.SendToQueue(msg);
        }

        [WebMethod]
        public decimal GetRate()
        {
            Integration intlink = new Integration();
            return intlink.GetRate();
        }

        [WebMethod]
        public void SetMonStat(int status)
        {
            Integration intlink = new Integration();
            intlink.SetIntegrationStat(status);
        }

        [WebMethod]
        public int isOnline()
        {
            int i = 0;
            Process p = Process.GetCurrentProcess();
            if (Process.GetProcessesByName("WindowsFormsApplication1").Length > 0 || Process.GetProcessesByName("WindowsFormsApplication1.vshost").Length > 0)
            {
                i = 1;
            }

            return i;
        }

        [WebMethod]
        public int GetUserCount()
        {
            Integration intlink = new Integration();
           return intlink.GetUserCount();
        }

        [WebMethod]
        public List<Log> GetLog()
        {
            Integration intlink = new Integration();
            return intlink.Log("latest");
        }

        [WebMethod]
        public List<InvoiceDetail> GetCancellationsAndMemos()
        {
            Integration intlink = new Integration();
            return intlink.GetCancellationAndMemos();
        }

        [WebMethod]
        public string Generate_SaveDeferredRpt(string ReportType, int month, int year)
        {
            Integration intlink = new Integration();
            Report rpt = new Report();

            DeferredData data = rpt.gen_rpt(ReportType, intlink, 0, month, year);

            if (ReportType == "Monthly")
            {
                //here we set the next Report Generation Date
                int es = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) - DateTime.Now.Day;
                es++;
                DateTime nextMonth = DateTime.Now.AddDays(es);
                DateTime nextGenDate = new DateTime(nextMonth.Year, nextMonth.Month, 2);
                nextGenDate = nextGenDate.AddHours(2);
                intlink.SetNextGenDate(ReportType, nextGenDate);
            }

            if(ReportType == "Annual")
            {
                //here we set the next Report Generation Date
                DateTime nextGenDate = new DateTime(DateTime.Now.Year + 1, 4, 2);
                nextGenDate = nextGenDate.AddHours(3);
                intlink.SetNextGenDate(ReportType, nextGenDate);
            }

            return data.report_id;
        }

        [WebMethod]
        public DeferredModificationResult DeferredModification(List<ReportEntry> entries, int reportId, bool isForModification)
        {
            Integration intlink = new Integration();
            return intlink.MonthlyDeferredModification(entries, reportId, isForModification);
        }

        [WebMethod]
        public DeferredModificationResult AddToReport(ReportEntry entry, int reportId)
        {
            Integration intlink = new Integration();
            return intlink.AddToReport(entry, reportId);
        }

        [WebMethod]
        public DeferredModificationResult RemoveFromReport(int invoiceId, int reportId)
        {
            Integration intlink = new Integration();
            return intlink.RemoveFromReport(invoiceId, reportId);
        }


        [WebMethod]
        public UIData GetDeferredReportLineItem(int invoiceId, int reportId)
        {
            Integration intlink = new Integration();
            return intlink.GetDeferredLineItem(invoiceId, reportId);
        }

        // Report Annual Modification
        [WebMethod]
        public DeferredModificationResult AnnualDeferredModification(List<ReportEntry> entries, int reportId, bool isForModification)
        {
            Integration intlink = new Integration();
            return intlink.AnnualDeferredModification(entries, reportId, isForModification);
        }

        [WebMethod]
        public DeferredModificationResult AddToAnnualReport(ReportEntry entry, int reportId)
        {
            Integration intlink = new Integration();
            return intlink.AddToAnnualReport(entry, reportId);
        }

        [WebMethod]
        public DeferredModificationResult RemoveFromAnnualReport(int invoiceId, int reportId)
        {
            Integration intlink = new Integration();
            return intlink.RemoveFromAnnualReport(invoiceId, reportId);
        }


        [WebMethod]
        public UIData GetDeferredReportLineItemForAnnual(int invoiceId, int reportId)
        {
            Integration intlink = new Integration();
            return intlink.GetDeferredLineItemForAnnual(invoiceId, reportId);
        }
        // Report Annual Modification End

        [WebMethod]
        public DeferredData ViewMonDeferredRpt(string ReportType, int reportId)
        {
            Integration intlink = new Integration();
            return intlink.getDeferredRpt(ReportType, reportId.ToString());
        }

        [WebMethod]
        public List<ReportPeriod> GetReportPeriods(string reportType)
        {
            Integration intlink = new Integration();
            return intlink.GetReportPeriods(reportType);
        }

        [WebMethod]
        public DeferredData ViewAnnDeferredRpt(string ReportType, int fiscalyr)
        {
            if (fiscalyr == 2016) return null;

            Integration intlink = new Integration();

            DateTime nextRptDate = intlink.getNextRptDate(ReportType);
            int currYear = nextRptDate.Year - 1;

            if (fiscalyr == currYear)
            {
                //Generate temporary report here
                Report rpt = new Report();
                return rpt.gen_rpt(ReportType, intlink, 1, 4, fiscalyr);
            }

            String report_id = intlink.getReportID(ReportType, 3, fiscalyr + 1);
            if (report_id != "")
            {
                return intlink.getDeferredRpt(ReportType, report_id);
            }

            return null; //This means the report does not exist or cannot be produced at this time

        }

        [WebMethod]
        public DeferredData GenerateMonDeferredRptTemp(string ReportType, int month, int year)
        {
            Integration intlink = new Integration();
            Report rpt = new Report();

            return rpt.gen_rpt(ReportType, intlink, 1, month, year);
        }

        [WebMethod]
        public int UpdateBudget(string invoiceID, string budgetAmt)
        {
            int i = brian_businessClass.UpdateBudgetInfo(Convert.ToInt32(invoiceID), Convert.ToDecimal(budgetAmt));
            return i;
        }

        [WebMethod]
        public List<string> GetInvoiceIDs()
        {
            Integration intlink = new Integration();
            List<string> ids = intlink.getInvoiceIDs();
            return ids;
        }
    }
}
