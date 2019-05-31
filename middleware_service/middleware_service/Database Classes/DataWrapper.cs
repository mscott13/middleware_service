using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace middleware_service.Database_Classes
{
    public class DataWrapper
    {
        public DataWrapper(string lbl)
        {
            label = lbl;
        }

        public void setSubTotals(SubTotals subs)
        {
            subT_invoiceTotal = subs.invoiceTotal;
            subT_balBFwd = subs.balanceBFwd;
            subT_toRev = subs.toRev;
            subT_closingBal = subs.closingBal;
            subT_fromRev = subs.closingBal;
            subT_budget = subs.budget;
        }

        public string label { get; }
        public List<UIData> records = new List<UIData>();

        public decimal subT_invoiceTotal { get; set; }
        public decimal subT_balBFwd { get; set; }
        public decimal subT_toRev { get; set; }
        public decimal subT_closingBal { get; set; }
        public decimal subT_fromRev { get; set; }
        public decimal subT_budget { get; set; }
    }
}
