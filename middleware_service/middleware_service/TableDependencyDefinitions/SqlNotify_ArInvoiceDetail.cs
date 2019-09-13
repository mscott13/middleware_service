using middleware_service.Other_Classes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace middleware_service.TableDependencyDefinitions
{
    class SqlNotify_ArInvoiceDetail
    {
        public void TransferToGeneric(string database)
        {
            string query = "INSERT INTO tblARInvoiceDetail (ARInvoiceID,CreditGLID,UnitPrice,Quantity,SplitAmount,Description,InternationalID,InternationalAmount,CountryID " +
                          ",CountryAmount,StateID,StateAmount,CountyID,CountyAmount,CityID,CityAmount,OtherID,OtherAmount,Type,Disc,Discount " +
                          ",YPeriod,DQty,DExt,FreqUsage,CostCenter,StartPeriod,EndPeriod,ARItemId,DebitGLId,ExchangeRate,SystemCode,ECCoefficient,NCCoefficient " +
                          ",POCoefficient,TCCoefficient,SUCoefficient,QECoefficient,VRCoefficient,TypeOfSystem,AntennaPower,SiteCode,SiteCategory,StationName,ServiceTypeCode,Proj) " +
                          "VALUES " +
                               "(@ARInvoiceID,@CreditGLID,@UnitPrice,@Quantity,@SplitAmount,@Description,@InternationalID,@InternationalAmount,@CountryID,@CountryAmount " +
                                ",@StateID,@StateAmount,@CountyID,@CountyAmount,@CityID,@CityAmount,@OtherID,@OtherAmount,@Type,@Disc,@Discount " +
                                ",@YPeriod,@DQty,@DExt,@FreqUsage,@CostCenter " +
                                ",@StartPeriod,@EndPeriod,@ARItemId,@DebitGLId,@ExchangeRate " +
                                ",@SystemCode,@ECCoefficient,@NCCoefficient,@POCoefficient " +
                                ",@TCCoefficient,@SUCoefficient,@QECoefficient,@VRCoefficient " +
                                ",@TypeOfSystem,@AntennaPower,@SiteCode,@SiteCategory " +
                                ",@StationName,@ServiceTypeCode,@Proj)";

            SqlConnection conn = new SqlConnection(database);
            conn.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = query;

            Description = fixNulls(Description);
            FreqUsage = fixNulls(FreqUsage);
            CostCenter = fixNulls(CostCenter);
            SystemCode = fixNulls(SystemCode);
            TypeOfSystem = fixNulls(TypeOfSystem);
            SiteCode = fixNulls(SiteCode);
            SiteCategory = fixNulls(SiteCategory);
            StationName = fixNulls(StationName);
            ServiceTypeCode = fixNulls(ServiceTypeCode);
            Proj = fixNulls(Proj);

            try
            {
                cmd.Parameters.AddWithValue("@ARInvoiceID", ARInvoiceID);
                cmd.Parameters.AddWithValue("@CreditGLID", CreditGLID);
                cmd.Parameters.AddWithValue("@UnitPrice", UnitPrice);
                cmd.Parameters.AddWithValue("@Quantity", Quantity);
                cmd.Parameters.AddWithValue("@SplitAmount", SplitAmount);
                cmd.Parameters.AddWithValue("@Description", Description);
                cmd.Parameters.AddWithValue("@InternationalID", InternationalID);
                cmd.Parameters.AddWithValue("@InternationalAmount", InternationalAmount);
                cmd.Parameters.AddWithValue("@CountryID", CountryID);
                cmd.Parameters.AddWithValue("@CountryAmount", CountryAmount);
                cmd.Parameters.AddWithValue("@StateID", StateID);
                cmd.Parameters.AddWithValue("@StateAmount", StateAmount);
                cmd.Parameters.AddWithValue("@CountyID", CountyID);
                cmd.Parameters.AddWithValue("@CountyAmount", CountyAmount);
                cmd.Parameters.AddWithValue("@CityID", CityID);
                cmd.Parameters.AddWithValue("@CityAmount", CityAmount);
                cmd.Parameters.AddWithValue("@OtherID", OtherID);
                cmd.Parameters.AddWithValue("@OtherAmount", OtherAmount);
                cmd.Parameters.AddWithValue("@Type", Type);
                cmd.Parameters.AddWithValue("@Disc", Disc);
                cmd.Parameters.AddWithValue("@Discount", Discount);
                cmd.Parameters.AddWithValue("@YPeriod", YPeriod);
                cmd.Parameters.AddWithValue("@DQty", DQty);
                cmd.Parameters.AddWithValue("@DExt", DExt);
                cmd.Parameters.AddWithValue("@FreqUsage", FreqUsage);
                cmd.Parameters.AddWithValue("@CostCenter", CostCenter);
                cmd.Parameters.AddWithValue("@StartPeriod", StartPeriod);
                cmd.Parameters.AddWithValue("@EndPeriod", EndPeriod);
                cmd.Parameters.AddWithValue("@ARItemId", ARItemId);
                cmd.Parameters.AddWithValue("@DebitGLId", DebitGLId);
                cmd.Parameters.AddWithValue("@ExchangeRate", ExchangeRate);
                cmd.Parameters.AddWithValue("@SystemCode", SystemCode);
                cmd.Parameters.AddWithValue("@ECCoefficient", ECCoefficient);
                cmd.Parameters.AddWithValue("@NCCoefficient", NCCoefficient);
                cmd.Parameters.AddWithValue("@POCoefficient", POCoefficient);
                cmd.Parameters.AddWithValue("@TCCoefficient", TCCoefficient);
                cmd.Parameters.AddWithValue("@SUCoefficient", SUCoefficient);
                cmd.Parameters.AddWithValue("@QECoefficient", QECoefficient);
                cmd.Parameters.AddWithValue("@VRCoefficient", VRCoefficient);
                cmd.Parameters.AddWithValue("@TypeOfSystem", TypeOfSystem);
                cmd.Parameters.AddWithValue("@AntennaPower", AntennaPower);
                cmd.Parameters.AddWithValue("@SiteCode", SiteCode);
                cmd.Parameters.AddWithValue("@SiteCategory", SiteCategory);
                cmd.Parameters.AddWithValue("@StationName", StationName);
                cmd.Parameters.AddWithValue("@ServiceTypeCode", ServiceTypeCode);
                cmd.Parameters.AddWithValue("@Proj", Proj);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Log.Save(e.Message + " " + e.StackTrace);
            }
            conn.Close();
        }

        private string fixNulls(string input)
        {
            if (input == null)
            {
                return "";
            }
            else
            {
                return input;
            }
        }

        public int ARInvoiceID { get; set; }
        public int ARInvoiceDetailID { get; set; }
        public int CreditGLID { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
        public decimal SplitAmount { get; set; }
        public string Description { get; set; }
        public int InternationalID { get; set; }
        public decimal InternationalAmount { get; set; }
        public int CountryID { get; set; }
        public decimal CountryAmount { get; set; }
        public int StateID { get; set; }
        public decimal StateAmount { get; set; }
        public int CountyID { get; set; }
        public decimal CountyAmount { get; set; }
        public int CityID { get; set; }
        public decimal CityAmount { get; set; }
        public int OtherID { get; set; }
        public decimal OtherAmount { get; set; }
        public int Type { get; set; }
        public int Disc { get; set; }
        public float Discount { get; set; }
        public float YPeriod { get; set; }
        public float DQty { get; set; }
        public decimal DExt { get; set; }
        public string FreqUsage { get; set; }
        public string CostCenter { get; set; }
        public DateTime StartPeriod { get; set; }
        public DateTime EndPeriod { get; set; }
        public int ARItemId { get; set; }
        public int DebitGLId { get; set; }
        public float ExchangeRate { get; set; }
        public string SystemCode { get; set; }
        public float ECCoefficient { get; set; }
        public float NCCoefficient { get; set; }
        public float POCoefficient { get; set; }
        public float TCCoefficient { get; set; }
        public float SUCoefficient { get; set; }
        public float QECoefficient { get; set; }
        public float VRCoefficient { get; set; }
        public string TypeOfSystem { get; set; }
        public float AntennaPower { get; set; }
        public string SiteCode { get; set; }
        public string SiteCategory { get; set; }
        public string StationName { get; set; }
        public string ServiceTypeCode { get; set; }
        public string Proj { get; set; }
    }
}
