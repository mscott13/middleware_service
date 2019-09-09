using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace middleware_service.EventObjects
{
    class Invoice
    {
        public Invoice()
        {
            eventType = "EVT_INVOICE";
        }
        public Invoice(int invoiceId, string customerName, string customerId, int batch, string amount, DateTime date, string author)
        {
            eventType = "EVT_INVOICE";
            this.invoiceId = invoiceId;
            this.customerId = customerId;
            this.batch = batch;
            this.amount = amount;
            this.date = date;
        }

        public string eventType {get;}
        public int invoiceId { get; set; }
        public string customerId { get; set; }
        public string customerName { get; set; }
        public int batch { get; set; }
        public string amount { get; set; }
        public DateTime date { get; set; }
        public string author { get; set; }
        public string status { get; set; }
    }
}
