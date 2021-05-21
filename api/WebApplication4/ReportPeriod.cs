using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4
{
    public class ReportPeriod
    {
        public string reportLabel { get; set; }
        public int reportId { get; set; }
        public string month { get; set; }
        public string year { get; set; }
        public DateTime date { get; set; }
    }
}