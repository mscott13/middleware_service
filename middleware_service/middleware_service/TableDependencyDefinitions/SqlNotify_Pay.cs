using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace middleware_service.TableDependencyDefinitions
{
    class SqlNotify_Pay
    {
        public int GLID { get; set; } //Document Number
        public string CustomerID { get; set; } //Id used to get name
        public float Debit { get; set; } //Amount
        public int InvoiceID { get; set; } //Spectrum fee or Regulatory Fee
        public int GLTransactionID { get; set; }
        public int ReceiptNumber { get; set; }
    }
}
