using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace middleware_service.EventObjects
{
    public class Customer
    {
        public Customer()
        {
            eventType = "evt_customer";
        }

        public Customer(string customerName, string clientId)
        {
            this.eventType = "evt_customer";
            this.clientId = clientId;
            this.customerName = customerName;
            this.date = DateTime.Now;
        }

        public string eventType { get; }
        public string customerName { get; set; }
        public string clientId { get; set; }
        public DateTime date { get; set; }
    }
}
