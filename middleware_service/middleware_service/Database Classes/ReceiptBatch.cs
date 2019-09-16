using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace middleware_service.Database_Classes
{
    public class ReceiptBatch
    {
        public int batchId { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime expiryDate { get; set; }
        public string status { get; set; }
        public string bankCodeId { get; set; }
        public int count { get; set; }
        public decimal total { get; set; }
    }
}