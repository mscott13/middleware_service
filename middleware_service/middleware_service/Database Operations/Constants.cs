using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace middleware_service.Database_Operations
{
    public static class Constants
    {
        //--> Production  @"Data Source=ERP-SRVR\TCIASMS;Initial Catalog=ASMSGenericMaster;Integrated Security=True";
        public static string dbGeneric = @"Data Source=SERVER-ERP2\TCIASMS;Initial Catalog=ASMSGenericMaster;Integrated Security=True";
        public static string dbIntegration = @"Data Source=SERVER-ERP2\TCIASMS;Initial Catalog=ASMSSAGEINTEGRATION;Integrated Security=True";
    }
}
