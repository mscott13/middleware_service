using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace middleware_service.Database_Classes
{
    public class Totals
    {
        public Totals()
        {
        }

        public string tot_invoiceTotal { get; set; }
        public string tot_balBFwd { get; set; }
        public string tot_toRev { get; set; }
        public string tot_closingBal { get; set; }
        public string tot_fromRev { get; set; }
        public string tot_budget { get; set; }
    }
}
