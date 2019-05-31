using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace middleware_service.TableDependencyDefinitions
{
    class SqlNotifyCancellation
    {
        public int ARInvoiceID { get; set; }
        public decimal Amount { get; set; }
        public int isVoided { get; set; }
        public string canceledBy { get; set; }
        public int CustomerID { get; set; }
        public string notes { get; set; }
        public string FeeType { get; set; }
    }
}
