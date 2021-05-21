using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace middleware_service.Database_Operations
{
    public static class Constants
    {
        public static string dbGeneric = @"Data Source=ERP-SRVR\TCIASMS;Initial Catalog=ASMSGenericMaster;Integrated Security=True";
        public static string dbIntegration = @"Data Source=ERP-SRVR\TCIASMS;Initial Catalog=ASMSSAGEINTEGRATION;Integrated Security=True";

        public static int PORT = 8080;
        public static string BASE_ADDRESS = "http://*:";

        public static string INV_MANUAL_ENTRY = "INVOICE_MANUAL_ENTRY";
        public static string RCT_MANUAL_ENTRY = "RECEIPT_MANUAL_ENTRY";
        public static string INV_MANUAL_CANCEL = "INVOICE_MANUAL_CANCEL";

        public static string DEV_COMPANY_ID = "SMALTD";
        public static string LIV_COMPANY_ID = "SMJLTD";

    }
}
