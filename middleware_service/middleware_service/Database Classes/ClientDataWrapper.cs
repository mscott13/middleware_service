using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace middleware_service.Database_Classes
{
    public class ClientDataWrapper
    {
        public ClientDataWrapper()
        {

        }

        public void setSubTotals(SubTotals subs)
        {
            subT_invoiceTotal = subs.invoiceTotal;
            subT_balBFwd = subs.balanceBFwd;
            subT_toRev = subs.toRev;
            subT_closingBal = subs.closingBal;
            subT_fromRev = subs.fromRev;
            subT_budget = subs.budget;
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
