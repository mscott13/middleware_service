using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4
{
    class InvoiceList
    {
        public int invoiceID { get; set; }
        public int targetBatch { get; set; }
        public string clientName { get; set; }
        public string clientID { get; set; }
        public string author { get; set; }
        public decimal amount { get; set; }
        public DateTime lastModified { get; set; }
    }
}