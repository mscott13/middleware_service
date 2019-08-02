using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace middleware_service.Database_Classes
{
    public class DataWrapper
    {
        public DataWrapper()
        {

        }

        public void setSubTotals(SubTotals subs)
        {
            subT_invoiceTotal = subs.invoiceTotal.ToString();
            subT_balBFwd = subs.balanceBFwd.ToString();
            subT_toRev = subs.toRev.ToString();
            subT_closingBal = subs.closingBal.ToString();
            subT_fromRev = subs.fromRev.ToString();
            subT_budget = subs.budget.ToString();
        }

        public string label { get; set; }
        public List<UIData> records = new List<UIData>();

        public string subT_invoiceTotal { get; set; }
        public string subT_balBFwd { get; set; }
        public string subT_toRev { get; set; }
        public string subT_closingBal { get; set; }
        public string subT_fromRev { get; set; }
        public string subT_budget { get; set; }
    }
}
