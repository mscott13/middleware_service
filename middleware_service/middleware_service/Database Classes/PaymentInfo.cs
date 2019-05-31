using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace middleware_service.Database_Classes
{
    public class PaymentInfo
    {
        public int ReceiptNumber { get; set; }
        public int GLTransactionID { get; set; }
        public int CustomerID { get; set; }
        public decimal Debit { get; set; }
        public int InvoiceID { get; set; }
        public DateTime Date1 { get; set; }
        public int GLID { get; set; }
    }
}
