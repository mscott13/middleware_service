using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace middleware_service.Database_Classes
{
    public class PrepaymentData
    {
        public bool dataAvail { get; set; }
        public decimal originalAmount { get; set; }
        public decimal remainder { get; set; }
        public decimal totalPrepaymentRemainder { get; set; }
        public string referenceNumber { get; set; }
        public int sequenceNumber { get; set; }
        public int destinationBank { get; set; }
    }
}
