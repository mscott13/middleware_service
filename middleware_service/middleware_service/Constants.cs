namespace middleware_service
{
    public static class Constants
    {
        public static string DB_GENERIC_NAME = "ASMSGenericMaster";
        public static string DB_INTEGRATION_NAME = "ASMSSAGEINTEGRATION";
        public static string DB_GENERIC = @"Data Source=ERP-SRVR\ASMSDEV;Initial Catalog="+ DB_GENERIC_NAME + ";Integrated Security=True; MultipleActiveResultSets=true";
        public static string DB_INTEGRATION = @"Data Source=ERP-SRVR\ASMSDEV;Initial Catalog="+DB_INTEGRATION_NAME+ ";Integrated Security=True; MultipleActiveResultSets=true";
        public static string TEST_DB_GENERIC = @"Data Source=SERVER-ERP2\ASMSDEV;Initial Catalog=" + DB_GENERIC_NAME + ";Integrated Security=True; MultipleActiveResultSets=true";
        public static string TEST_DB_INTEGRATION = @"Data Source=SERVER-ERP2\ASMSDEV;Initial Catalog=" + DB_INTEGRATION_NAME + ";Integrated Security=True; MultipleActiveResultSets=true";

        public static int PORT = 8080;
        public static string BASE_ADDRESS = "http://*:";

        public static string ACCPAC_USER = "ADMIN";
        public static string ACCPAC_CRED = "SPECTRUM9";
        public static string ACCPAC_COMPANY = "SMALTD";
        public static bool IGNORE_EVENTS = false;
        public static bool PARALLEL_RUN = true;
    }
}
