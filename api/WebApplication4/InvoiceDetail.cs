using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4
{
    public class InvoiceDetail
    {
        public string clientName { get; set; }
        public string clientId { get; set; }
        public string invoiceId { get; set; }
        public DateTime dateCreated { get; set; }
        public string formattedDate { get; set; }
        public string Author { get; set; }
        public string amount { get; set; }
        public int sequence { get; set; }
        public string state { get; set; }
        public string usamount { get; set; }
        public string docType { get; set; }
        public string currency { get; set; }
    }
}