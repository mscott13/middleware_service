using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace middleware_service.Database_Classes
{
    public class Totals
    {
        public Totals(decimal invoiceTotal, decimal balanceBFwd, decimal toRev, decimal fromRev, decimal budget, decimal closingBal)
        {
            tot_invoiceTotal = invoiceTotal;
            tot_balBFwd = balanceBFwd;
            tot_toRev = toRev;
            tot_closingBal = closingBal;
            tot_fromRev = fromRev;
            tot_budget = budget;
        }

        public Totals()
        {
        }

        public decimal tot_invoiceTotal { get; set; }
        public decimal tot_balBFwd { get; set; }
        public decimal tot_toRev { get; set; }
        public decimal tot_closingBal { get; set; }
        public decimal tot_fromRev { get; set; }
        public decimal tot_budget { get; set; }
    }
}
