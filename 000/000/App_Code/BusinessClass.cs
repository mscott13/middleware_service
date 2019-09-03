using System;
using System.Collections.Generic;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.ComponentModel;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;

namespace _000.App_Code
{

    [DataObject(true)]
    public class BusinessClass
    {
        [DataObjectMethod(DataObjectMethodType.Insert)]
        public static void InsertTransferredInvoices(string CustomerID)
        {

            try
            {
                SqlConnection conn = new SqlConnection("Data Source=SMA-PE2850\\SQLEXPRESS2012;Initial Catalog=ASMSSAGEINTEGRATION;Integrated Security=True; MultipleActiveResultSets=true");
                SqlCommand cmd = new SqlCommand("EXEC sp_TransferredInvoices @id", conn);
                cmd.Parameters.AddWithValue("@id", CustomerID.ToString());


                conn.Open();
                cmd.ExecuteNonQuery();



            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [DataObjectMethod(DataObjectMethodType.Insert)]
        public static int InsertTransferredPayments(string CustomerID, DateTime transferreddate)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Integration"].ConnectionString);
                SqlCommand cmd = new SqlCommand("sp_TransferredPayments", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;



                cmd.Parameters.AddWithValue("@CustomerID", CustomerID);
                cmd.Parameters.AddWithValue("@DateTransferred", transferreddate);

                con.Open();
                int result = cmd.ExecuteNonQuery();
                con.Close();

                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }




        [DataObjectMethod(DataObjectMethodType.Insert)]
        public static int InsertTransferredCustomers(string CustomerID, DateTime transferreddate)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Integration"].ConnectionString);
                SqlCommand cmd = new SqlCommand("sp_TransferredCustomers", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;



                cmd.Parameters.AddWithValue("@CustomerID", CustomerID);
                cmd.Parameters.AddWithValue("@DateTransferred", transferreddate);

                con.Open();
                int result = cmd.ExecuteNonQuery();
                con.Close();

                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public static DataSet GetPaymentMethod()
        {
            try
            {

                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["1204842DB"].ConnectionString);

                SqlCommand cmd = new SqlCommand("spGetPaymentMethod", con);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter(cmd);

                DataSet ds = new DataSet();
                da.Fill(ds);

                return ds;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}