using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace middleware_service.Database_Classes
{
    public class UIData
    {
        public string licenseNumber { get; set; }
        public string clientCompany { get; set; }
        public string invoiceID { get; set; }
        public string budget { get; set; }
        public string invoiceTotal { get; set; }
        public string thisPeriodsInv { get; set; }
        public string balBFwd { get; set; }
        public string fromRev { get; set; }
        public string toRev { get; set; }
        public string closingBal { get; set; }
        public int totalMonths { get; set; }
        public int monthUtil { get; set; }
        public int monthRemain { get; set; }
        public string valPStart { get; set; }
        public string valPEnd { get; set; }
    }
}
