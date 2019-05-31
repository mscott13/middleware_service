using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace middleware_service.Database_Classes
{
    public class Batch
    {
        public int BatchId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string BatchType { get; set; }
        public string Status { get; set; }
        public string BankCode { get; set; }
        public int Count { get; set; }
    }
}
