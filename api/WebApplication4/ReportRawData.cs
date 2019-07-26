using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4
{
    public class ReportRawData
    {
        public int clientID { get; set; }
        public string ccNum { get; set; }
        public string clientCompany { get; set; }
        public string clientFname { get; set; }
        public string clientLname { get; set; }
        public decimal Budget { get; set; }
        public decimal InvAmount { get; set; }
        public int ExistedBefore { get; set; }
        public string LastRptsClosingBal { get; set; }
        public string LastRptsStartValPeriod { get; set; }
        public string LastRptsEndValPeriod { get; set; }
        public DateTime CurrentStartValPeriod { get; set; }
        public DateTime CurrentEndValPeriod { get; set; }
        public int CreditGLID { get; set; }
        public string notes { get; set; }
        public string ARInvoiceID { get; set; }
        public DateTime InvoiceCreationDate { get; set; }
        public int isCancelled { get; set; }
        public int isCreditMemo { get; set; }
        public string CreditMemoNum { get; set; }
        public decimal CreditMemoAmt { get; set; }
    }
}