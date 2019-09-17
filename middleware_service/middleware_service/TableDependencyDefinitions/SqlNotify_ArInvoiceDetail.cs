using middleware_service.Other_Classes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace middleware_service.TableDependencyDefinitions
{
    public class SqlNotify_ArInvoiceDetail
    {
        public  void ARInvoiceDetailToGenric()
        {
            using (SqlConnection connection = new SqlConnection(Constants.TEST_DB_GENERIC))
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

                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ARInvoiceID", ARInvoiceID);
                    command.Parameters.AddWithValue("@CreditGLID", CreditGLID);
                    command.Parameters.AddWithValue("@UnitPrice", UnitPrice);
                    command.Parameters.AddWithValue("@Quantity", Quantity);
                    command.Parameters.AddWithValue("@SplitAmount", SplitAmount);
                    command.Parameters.AddWithValue("@Description", FixNulls(Description));
                    command.Parameters.AddWithValue("@InternationalID", InternationalID);
                    command.Parameters.AddWithValue("@InternationalAmount", InternationalAmount);
                    command.Parameters.AddWithValue("@CountryID", CountryID);
                    command.Parameters.AddWithValue("@CountryAmount", CountryAmount);
                    command.Parameters.AddWithValue("@StateID", StateID);
                    command.Parameters.AddWithValue("@StateAmount", StateAmount);
                    command.Parameters.AddWithValue("@CountyID", CountyID);
                    command.Parameters.AddWithValue("@CountyAmount", CountyAmount);
                    command.Parameters.AddWithValue("@CityID", CityID);
                    command.Parameters.AddWithValue("@CityAmount", CityAmount);
                    command.Parameters.AddWithValue("@OtherID", OtherID);
                    command.Parameters.AddWithValue("@OtherAmount", OtherAmount);
                    command.Parameters.AddWithValue("@Type", Type);
                    command.Parameters.AddWithValue("@Disc", Disc);
                    command.Parameters.AddWithValue("@Discount", Discount);
                    command.Parameters.AddWithValue("@YPeriod", YPeriod);
                    command.Parameters.AddWithValue("@DQty", DQty);
                    command.Parameters.AddWithValue("@DExt", DExt);
                    command.Parameters.AddWithValue("@FreqUsage", FixNulls(FreqUsage));
                    command.Parameters.AddWithValue("@CostCenter", FixNulls(CostCenter));
                    command.Parameters.AddWithValue("@StartPeriod", FixDate(StartPeriod));
                    command.Parameters.AddWithValue("@EndPeriod", FixDate(EndPeriod));
                    command.Parameters.AddWithValue("@ARItemId", ARItemId);
                    command.Parameters.AddWithValue("@DebitGLId", DebitGLId);
                    command.Parameters.AddWithValue("@ExchangeRate", ExchangeRate);
                    command.Parameters.AddWithValue("@SystemCode", FixNulls(SystemCode));
                    command.Parameters.AddWithValue("@ECCoefficient", ECCoefficient);
                    command.Parameters.AddWithValue("@NCCoefficient", NCCoefficient);
                    command.Parameters.AddWithValue("@POCoefficient", POCoefficient);
                    command.Parameters.AddWithValue("@TCCoefficient", TCCoefficient);
                    command.Parameters.AddWithValue("@SUCoefficient", SUCoefficient);
                    command.Parameters.AddWithValue("@QECoefficient", QECoefficient);
                    command.Parameters.AddWithValue("@VRCoefficient", VRCoefficient);
                    command.Parameters.AddWithValue("@TypeOfSystem", FixNulls(TypeOfSystem));
                    command.Parameters.AddWithValue("@AntennaPower", AntennaPower);
                    command.Parameters.AddWithValue("@SiteCode", FixNulls(SiteCode));
                    command.Parameters.AddWithValue("@SiteCategory", FixNulls(SiteCategory));
                    command.Parameters.AddWithValue("@StationName", FixNulls(StationName));
                    command.Parameters.AddWithValue("@ServiceTypeCode", FixNulls(ServiceTypeCode));
                    command.Parameters.AddWithValue("@Proj", FixNulls(Proj));
                    command.ExecuteNonQuery();
                    Log.Save("Parallel transfer completed for ArInvoiceDetail");
                }
            }
        }

        private object FixNulls(string input)
        {
            if (input == null)
            {
                return DBNull.Value;
            }
            else
            {
                return input;
            }
        }

        private object FixDate(DateTime date)
        {
            if (date == null || date == DateTime.MinValue)
            {
                return DBNull.Value;
            }
            else
            {
                return date;
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
