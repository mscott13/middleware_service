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

        public Receipt(DateTime createdDate, string customerName, string clientId, string invoiceId, decimal amount, decimal usamount, int referenceNumber, string prepstat, int destinationBank, string isPayByCredit)
        {
            this.eventType = "evt_receipt_transferred";
            this.customerName = customerName;
            this.clientId = clientId;
            this.invoiceId = invoiceId;
            this.amount = amount;
            this.date = createdDate;
            this.usamount = usamount;
            this.referenceNumber = referenceNumber;
            this.prepstat = prepstat;
            this.destinationBank = destinationBank;
            this.isPayByCredit = isPayByCredit;
        }

        public string eventType { get; }
        public string customerName { get; set; }
        public string clientId { get; set; }
        public string invoiceId { get; set; }
        public decimal amount { get; set; }
        public decimal usamount { get; set; }
        public int referenceNumber { get; set; }
        public string prepstat { get; set; }
        public int destinationBank { get; set; }
        public string isPayByCredit { get; set; }
        public DateTime date { get; set; }

    }
}
