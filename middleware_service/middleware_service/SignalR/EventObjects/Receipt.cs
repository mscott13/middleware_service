using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace middleware_service.EventObjects
{
    class Receipt
    {
        public Receipt()
        {
            eventType = "EVT_RECEIPT";
        }

        public string eventType { get; }
        public string customerName { get; set; }
        public string clientId { get; set; }
        public int invoiceId { get; set; }
        public string amount { get; set; }
        public DateTime date { get; set; }

    }
}
