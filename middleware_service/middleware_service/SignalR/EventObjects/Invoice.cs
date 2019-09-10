using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace middleware_service.EventObjects
{
    public class Invoice
    {
        public Invoice()
        {
            eventType = "evt_invoice";
        }
        public Invoice(string invoiceId, string customerName, string customerId, string batch, string amount, DateTime date, string author, string status)
        {
            eventType = "evt_invoice";
            this.invoiceId = invoiceId;
            this.customerId = customerId;
            this.customerName = customerName;
            this.batch = batch;
            this.amount = amount;
            this.date = date;
            this.status = status;
            this.author = author;
        }

        public string eventType {get;}
        public string invoiceId { get; set; }
        public string customerId { get; set; }
        public string customerName { get; set; }
        public string batch { get; set; }
        public string amount { get; set; }
        public DateTime date { get; set; }
        public string author { get; set; }
        public string status { get; set; }
    }
}
