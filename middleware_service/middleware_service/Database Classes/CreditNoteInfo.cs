using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace middleware_service.Database_Classes
{
    public class CreditNoteInfo
    {
        public int ARInvoiceID { get; set; }
        public int CreditGL { get; set; }
        public decimal amount { get; set; }
        public int CustomerID { get; set; }
        public string FeeType { get; set; }
        public string notes { get; set; }
        public string cancelledBy { get; set; }
        public string remarks { get; set; }
    }
}
