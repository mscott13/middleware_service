using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace middleware_service.Database_Classes
{
    public class SubTotals
    {
        public decimal invoiceTotal { get; set; }
        public decimal balanceBFwd { get; set; }
        public decimal toRev { get; set; }
        public decimal closingBal { get; set; }
        public decimal fromRev { get; set; }
        public decimal budget { get; set; }
    }
}
