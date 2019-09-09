using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace middleware_service.EventObjects
{
    class Invoice
    {
        public Invoice(int invoiceId, string customerId, int batch, string amount, DateTime date)
        {
            this.invoiceId = invoiceId;
            this.customerId = customerId;
            this.batch = batch;
            this.amount = amount;
            this.date = date;
        }

        public int invoiceId { get; set; }
        public string customerId { get; set; }
        public int batch { get; set; }
        public string amount { get; set; }
        public DateTime date { get; set; }
    }
}
