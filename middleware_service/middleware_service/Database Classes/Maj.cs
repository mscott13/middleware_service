using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace middleware_service.Database_Classes
{
    public class Maj
    {
        public Maj()
        {
            stationType = "";
            certificateType = -999;
            substationType = -999;
            proj = "";
        }
        public string stationType { get; set; }
        public int certificateType { get; set; }
        public int substationType { get; set; }
        public string proj { get; set; }
    }
}
