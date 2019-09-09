using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace middleware_service.EventObjects
{
    class Cancellation_CreditMemo
    {
        public Cancellation_CreditMemo()
        {
            eventType = "EVT_CANCELLATION_CREDIT_MEMO";
        }
        public string eventType { get; }
        public int invoiceId { get; set; }
        public string customerId { get; set; }
        public string customerName { get; set; }
        public int batch { get; set; }
        public string amount { get; set; }
        public DateTime date { get; set; }
        public string author { get; set; }
        public string type { get; set; }
    }
}
