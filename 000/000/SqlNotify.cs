using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _000
{
    public class SqlNotify
    {
       public int ARInvoiceID { get; set; } //Document Number
       public string CustomerID { get; set; } //Id used to get name
       public float Amount { get; set; } //Amount
       public string FeeType { get; set; } //Spectrum fee or Regulatory Fee
     
    }
}