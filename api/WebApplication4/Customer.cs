using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4
{
    public class Customer
    {
        public string ClientId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public string formattedDate { get; set; }
    }
}