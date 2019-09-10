namespace middleware_service
{
    public static class Constants
    {
        //--> Production  @"Data Source=ERP-SRVR\TCIASMS;Initial Catalog=ASMSGenericMaster;Integrated Security=True";
        public static string DBGENERIC = @"Data Source=SERVER-ERP2\ASMSDEV;Initial Catalog=ASMSGenericMaster;Integrated Security=True";
        public static string DBINTEGRATION = @"Data Source=SERVER-ERP2\ASMSDEV;Initial Catalog=ASMSSAGEINTEGRATION;Integrated Security=True";
        public static int PORT = 8080;
        public static string BASE_ADDRESS = "http://*:";
    }
}
