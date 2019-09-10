using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace middleware_service.EventObjects
{
    public class Receipt
    {
        public Receipt()
        {
            eventType = "evt_receipt";
        }

        public Receipt(string customerName, string clientId, string invoiceId, string amount)
        {
            this.eventType = "evt_receipt";
            this.customerName = customerName;
            this.clientId = clientId;
            this.invoiceId = invoiceId;
            this.amount = amount;
            this.date = DateTime.Now;
        }

        public string eventType { get; }
        public string customerName { get; set; }
        public string clientId { get; set; }
        public string invoiceId { get; set; }
        public string amount { get; set; }
        public DateTime date { get; set; }

    }
}
