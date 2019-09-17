using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace middleware_service.Database_Classes
{
    public class InvoiceInfo
    {
        public int customerId { get; set; }
        public string feeType { get; set; }
        public string notes { get; set; }
        public decimal amount { get; set; }
        public decimal arBalance { get; set; }
        public int isVoided { get; set; }
        public int glid { get; set; }
        public string freqUsage { get; set; }
        public string author { get; set; }
    }
}
