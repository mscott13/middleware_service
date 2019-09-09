using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace middleware_service.EventObjects
{
    class Customer
    {
        public Customer()
        {
            eventType = "EVT_CUSTOMER";
        }

        public string eventType { get; }
        public string customerName { get; set; }
        public string clientId { get; set; }
        public DateTime date { get; set; }
    }
}
