using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4
{
    public class ReportEntry
    {
        public string licenseNumber { get; set; }
        public string clientCompany { get; set; }
        public int invoiceId { get; set; }
        public decimal budget { get; set; }
        public decimal invoiceTotal { get; set; }
        public string thisMonthInvoice { get; set; }
        public decimal balanceBroughtFoward { get; set; }
        public decimal fromRevenue { get; set; }
        public decimal toRevenue { get; set; }
        public decimal closingBalance { get; set; }
        public int totalMonths { get; set; }
        public int monthsUtilized { get; set; }
        public int monthsRemaining { get; set; }
        public DateTime validityStart { get; set; }
        public DateTime validityEnd { get; set; }
        public string group { get; set; }
    }
}