using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace middleware_service.Database_Classes
{
    public class InvoiceInfo
    {
        public int CustomerId { get; set; }
        public string FeeType { get; set; }
        public string notes { get; set; }
        public decimal amount { get; set; }
        public int isvoided { get; set; }
        public int Glid { get; set; }
        public string FreqUsage { get; set; }
        public string Author { get; set; }
    }
}
