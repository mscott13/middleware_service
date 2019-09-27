using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4
{
    public class SubTotals
    {
        public string invoiceTotal { get; set; }
        public string balanceBFwd { get; set; }
        public string toRev { get; set; }
        public string closingBal { get; set; }
        public string fromRev { get; set; }
        public string budget { get; set; }
    }
}