using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WebApplication4
{
    class brian_businessClass
    {
        //static string connection = "Data Source=SMA-DBSRV\\TCIASMS;Initial Catalog=ASMSSAGEINTEGRATION; MultipleActiveResultSets=True; Integrated Security=True";
        static string connection = "Data Source=ERP-SRVR\\ASMSDEV;Initial Catalog=ASMSSAGEINTEGRATION; MultipleActiveResultSets=True; Integrated Security=True";
        [DataObjectMethod(DataObjectMethodType.Insert)]
        public static int InsertTransferredPayments(string CustomerID)
        {
            try
            {
                SqlConnection con = new SqlConnection(connection);
                SqlCommand cmd = new SqlCommand("sp_TransferredPayments", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CustomerID", CustomerID);

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
        public static int InsertTransferredCustomers(string CustomerID)
        {
            try
            {
                SqlConnection con = new SqlConnection(connection);
                SqlCommand cmd = new SqlCommand("sp_TransferredCustomers", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CustomerID", CustomerID);

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
        public static int InsertTransferredInvoices(string CustomerID, int inv)
        {
            try
            {
                SqlConnection con = new SqlConnection(connection);
                SqlCommand cmd = new SqlCommand("sp_TransferredInvoices", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;



                cmd.Parameters.AddWithValue("@CustomerID", CustomerID);

                cmd.Parameters.AddWithValue("@inv", inv);

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
        public static int InsertBudgetInfo(string CustomerID, Decimal budget, int invoiceid)
        {
            try
            {
                SqlConnection con = new SqlConnection(connection);
                SqlCommand cmd = new SqlCommand("sp_InsertBudget", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Cid", CustomerID);
                cmd.Parameters.AddWithValue("@budget", budget);
                cmd.Parameters.AddWithValue("@invoiceid", invoiceid);

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
        public static int InsertOpBal(string CustomerID, decimal op, DateTime tdate, int invoiceid)
        {
            try
            {
                SqlConnection con = new SqlConnection(connection);
                SqlCommand cmd = new SqlCommand("sp_InsertOpeningBalance ", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ccNum", CustomerID);
                cmd.Parameters.AddWithValue("@opbal", op);
                cmd.Parameters.AddWithValue("@tdate", tdate);
                cmd.Parameters.AddWithValue("@invoiceid", invoiceid);

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
        public static int InsertOpBalNew(string CustomerID, decimal op, DateTime tdate, int invoiceid, DateTime vs, DateTime vf)
        {
            try
            {
                SqlConnection con = new SqlConnection(connection);
                SqlCommand cmd = new SqlCommand("sp_InsertOpeningBalanceNew", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ccNum", CustomerID);
                cmd.Parameters.AddWithValue("@opbal", op);
                cmd.Parameters.AddWithValue("@tdate", tdate);
                cmd.Parameters.AddWithValue("@invoiceid", invoiceid);
                cmd.Parameters.AddWithValue("@validityS", vs);
                cmd.Parameters.AddWithValue("@validityF", vf);

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
        public static int InsertInvoice(int invoiceid, string cid, string cname)
        {
            try
            {
                SqlConnection con = new SqlConnection(connection);
                SqlCommand cmd = new SqlCommand("sp_StoreInvoice", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@invoiceId", invoiceid);
                cmd.Parameters.AddWithValue("@targetBatch", 0);
                cmd.Parameters.AddWithValue("@CreditGL", 0);
                cmd.Parameters.AddWithValue("@clientName", cname);
                cmd.Parameters.AddWithValue("@clientId", cid);
                cmd.Parameters.AddWithValue("@dateCreated", DateTime.Now);
                cmd.Parameters.AddWithValue("@author", "Default");
                cmd.Parameters.AddWithValue("@amount", 0);
                cmd.Parameters.AddWithValue("@state", "no modification");
                cmd.Parameters.AddWithValue("@usrate", 1);
                cmd.Parameters.AddWithValue("@usamount", 0);
                cmd.Parameters.AddWithValue("@isvoid", 0);
                cmd.Parameters.AddWithValue("@isCreditMemo", 0);
                cmd.Parameters.AddWithValue("@credMemoNum", 0);

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
        public static int UpdateOpBal(string CustomerID, decimal op, DateTime tdate, int invoiceid)
        {
            try
            {
                SqlConnection con = new SqlConnection(connection);
                SqlCommand cmd = new SqlCommand("sp_UpdateOpeningBalance", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ccNum", CustomerID);
                cmd.Parameters.AddWithValue("@opbal", op);
                cmd.Parameters.AddWithValue("@tdate", tdate);
                cmd.Parameters.AddWithValue("@invoiceid", invoiceid);

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
        public static int UpdateOpBalNew(string CustomerID, decimal op, DateTime tdate, int invoiceid, DateTime vs, DateTime vf)
        {
            try
            {
                SqlConnection con = new SqlConnection(connection);
                SqlCommand cmd = new SqlCommand("sp_UpdateOpeningBalanceNew", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ccNum", CustomerID);
                cmd.Parameters.AddWithValue("@opbal", op);
                cmd.Parameters.AddWithValue("@tdate", tdate);
                cmd.Parameters.AddWithValue("@invoiceid", invoiceid);
                cmd.Parameters.AddWithValue("@validityS", vs);
                cmd.Parameters.AddWithValue("@validityF", vf);
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
        public static int UpdateBudgetInfo(int invoiceID, Decimal budgetAmt)
        {
            try
            {
                SqlConnection con = new SqlConnection(connection);
                SqlCommand cmd = new SqlCommand("sp_UpdateBudget", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@invoiceID", invoiceID);
                cmd.Parameters.AddWithValue("@budgetAmt", budgetAmt);

                SqlParameter parm = new SqlParameter("@return", SqlDbType.Int);
                parm.Direction = ParameterDirection.ReturnValue;
                cmd.Parameters.Add(parm);
                con.Open();
                cmd.ExecuteNonQuery();

                con.Close();
                int res = Convert.ToInt32(parm.Value);

                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [DataObjectMethod(DataObjectMethodType.Select)]
        public static DataSet GetPaymentsForMonth(int month, int year)
        {
            try
            {
                SqlConnection con = new SqlConnection(connection);

                SqlCommand cmd = new SqlCommand("sp_getPaymentsBymonth", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("year", year);
                cmd.Parameters.AddWithValue("month", month);

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


        [DataObjectMethod(DataObjectMethodType.Select)]
        public static DataSet GetInv(int invoiceid)
        {
            try
            {

                SqlConnection con = new SqlConnection(connection);

                SqlCommand cmd = new SqlCommand("sp_getInv", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@invoiceid", invoiceid);


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

        [DataObjectMethod(DataObjectMethodType.Select)]
        public static DataSet GetIsInvoiceCreditMemo(int invoiceid)
        {
            try
            {

                SqlConnection con = new SqlConnection(connection);

                SqlCommand cmd = new SqlCommand("sp_getIfInvoiceIsCreditMemo", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@invoiceid", invoiceid);


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


        [DataObjectMethod(DataObjectMethodType.Select)]
        public static DataSet GetInvoiceAmountCredMemo(int invoiceid)
        {
            try
            {

                SqlConnection con = new SqlConnection(connection);

                SqlCommand cmd = new SqlCommand("sp_getInvoiceAmountCredMemo", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@invoiceid", invoiceid);


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

        [DataObjectMethod(DataObjectMethodType.Select)]
        public static DataSet GetClientName(int clientid)
        {
            try
            {

                SqlConnection con = new SqlConnection(connection);

                SqlCommand cmd = new SqlCommand("sp_getClientName", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@clientid", clientid);

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

        [DataObjectMethod(DataObjectMethodType.Select)]
        public static DataSet GetValidtyCancellations(int month, int year, int invoiceid)
        {
            try
            {

                SqlConnection con = new SqlConnection(connection);

                SqlCommand cmd = new SqlCommand("sp_getValidityCancellations", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@year", year);
                cmd.Parameters.AddWithValue("@month", month);
                cmd.Parameters.AddWithValue("@invoiceid", invoiceid);

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

        [DataObjectMethod(DataObjectMethodType.Select)]
        public static DataSet GetClientId(int invoiceid)
        {
            try
            {

                SqlConnection con = new SqlConnection(connection);

                SqlCommand cmd = new SqlCommand("sp_getClientsId", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@invoiceid", invoiceid);

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

        [DataObjectMethod(DataObjectMethodType.Select)]
        public static DataSet GetValidity(int invoiceid, int year, int month)
        {
            try
            {

                SqlConnection con = new SqlConnection(connection);

                SqlCommand cmd = new SqlCommand("sp_getValidityCancellations", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@year", year);
                cmd.Parameters.AddWithValue("@month", month);
                cmd.Parameters.AddWithValue("@invoiceid", invoiceid);

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

        [DataObjectMethod(DataObjectMethodType.Select)]
        public static DataSet GetValidityCM(int invoiceid)
        {
            try
            {

                SqlConnection con = new SqlConnection(connection);

                SqlCommand cmd = new SqlCommand("sp_getValidity", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@invoiceid", invoiceid);

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


        [DataObjectMethod(DataObjectMethodType.Select)]
        public static DataSet GetCreditGl(int invoiceid)
        {
            try
            {

                SqlConnection con = new SqlConnection(connection);

                SqlCommand cmd = new SqlCommand("sp_GetCreditGl", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@invoiceid", invoiceid);

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
        [DataObjectMethod(DataObjectMethodType.Select)]
        public static DataSet GetRegPaymentsForMonth(int month, int year)
        {
            try
            {

                SqlConnection con = new SqlConnection(connection);

                SqlCommand cmd = new SqlCommand("sp_getRegPaymentsBymonth", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("year", year);
                cmd.Parameters.AddWithValue("month", month);

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

        [DataObjectMethod(DataObjectMethodType.Select)]
        public static DataSet GetSpecPaymentsForMonth(int month, int year)
        {
            try
            {

                SqlConnection con = new SqlConnection(connection);

                SqlCommand cmd = new SqlCommand("sp_getSpecPaymentsBymonth", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("year", year);
                cmd.Parameters.AddWithValue("month", month);

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

        [DataObjectMethod(DataObjectMethodType.Select)]
        public static DataSet GetTypeApprovalPaymentsForMonth(int month, int year)
        {
            try
            {

                SqlConnection con = new SqlConnection(connection);

                SqlCommand cmd = new SqlCommand("sp_getTypeApprovalPaymentsBymonth", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("year", year);
                cmd.Parameters.AddWithValue("month", month);

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



        [DataObjectMethod(DataObjectMethodType.Select)]
        public static DataSet GetCustBudget(string cid, int invoiceid)
        {
            try
            {

                SqlConnection con = new SqlConnection(connection);

                SqlCommand cmd = new SqlCommand("sp_getBudgetCust", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ccnum", cid);
                cmd.Parameters.AddWithValue("invoiceid", invoiceid);

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

        [DataObjectMethod(DataObjectMethodType.Select)]
        public static DataSet GetOpeningBalStat(string cid, int invoiceid, DateTime tdate)
        {
            try
            {

                SqlConnection con = new SqlConnection(connection);

                SqlCommand cmd = new SqlCommand("sp_GetOpeningbalstat", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ccNum", cid);
                cmd.Parameters.AddWithValue("invoiceid", invoiceid);
                cmd.Parameters.AddWithValue("tdate", tdate);

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





        [DataObjectMethod(DataObjectMethodType.Select)]
        public static DataSet GetDefferdInfo()
        {
            try
            {

                SqlConnection con = new SqlConnection(connection);

                SqlCommand cmd = new SqlCommand("sp_getDefferedInfo", con);
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


        [DataObjectMethod(DataObjectMethodType.Select)]
        public static DataSet GetOpeningBalanceForMonth(string cid, int month, int year, int invoiceid)
        {
            try
            {

                SqlConnection con = new SqlConnection(connection);

                SqlCommand cmd = new SqlCommand("sp_GetOpeningBalance", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ccNum", cid);
                cmd.Parameters.AddWithValue("Tyear", year);
                cmd.Parameters.AddWithValue("Tmonth", month);
                cmd.Parameters.AddWithValue("invoiceid", invoiceid);

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



        [DataObjectMethod(DataObjectMethodType.Select)]
        public static DataSet GetInvoicesFormonth(int month, int year)
        {
            try
            {

                SqlConnection con = new SqlConnection(connection);

                SqlCommand cmd = new SqlCommand("sp_getInvoicesBymonth", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("year", year);
                cmd.Parameters.AddWithValue("month", month);

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

        [DataObjectMethod(DataObjectMethodType.Select)]
        public static ReturnData GetTypeApprovalInvoicesFormonth(int month, int year)
        {
            try
            {

                SqlConnection con = new SqlConnection(connection);

                SqlCommand cmd = new SqlCommand("sp_getTypeApprovalInvoicesBymonth", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("year", year);
                cmd.Parameters.AddWithValue("month", month);

                SqlDataAdapter da = new SqlDataAdapter(cmd);

                DataSet ds = new DataSet();
                da.Fill(ds);

                return refineDataset(ds);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public static ReturnData GetRegInvoicesFormonth(int month, int year)
        {
            try
            {

                SqlConnection con = new SqlConnection(connection);

                SqlCommand cmd = new SqlCommand("sp_getRegInvoicesBymonth", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("year", year);
                cmd.Parameters.AddWithValue("month", month);

                SqlDataAdapter da = new SqlDataAdapter(cmd);

                DataSet ds = new DataSet();
                da.Fill(ds);

                return refineDataset(ds);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public static ReturnData GetSpecInvoicesFormonth(int month, int year)
        {
            try
            {

                SqlConnection con = new SqlConnection(connection);

                SqlCommand cmd = new SqlCommand("sp_getSpecInvoicesBymonth", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("year", year);
                cmd.Parameters.AddWithValue("month", month);

                SqlDataAdapter da = new SqlDataAdapter(cmd);

                DataSet ds = new DataSet();
                da.Fill(ds);

                return refineDataset(ds);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        [DataObjectMethod(DataObjectMethodType.Select)]
        public static DataSet GetCustomersFormonth(int month, int year)
        {
            try
            {

                SqlConnection con = new SqlConnection(connection);

                SqlCommand cmd = new SqlCommand("sp_getCustomersTransferredBymonth", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("year", year);
                cmd.Parameters.AddWithValue("month", month);

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

        [DataObjectMethod(DataObjectMethodType.Select)]
        public static DataSet GetNumberCustomersFormonth(int month, int year)
        {
            try
            {

                SqlConnection con = new SqlConnection(connection);

                SqlCommand cmd = new SqlCommand("sp_getNumberOfCustomersBymonth", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("year", year);
                cmd.Parameters.AddWithValue("month", month);

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


        [DataObjectMethod(DataObjectMethodType.Select)]
        public static DataSet GetInvoiceTotalFormonth(int month, int year)
        {
            try
            {

                SqlConnection con = new SqlConnection(connection);

                SqlCommand cmd = new SqlCommand("sp_getInvoicesTotalBymonth", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("year", year);
                cmd.Parameters.AddWithValue("month", month);

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

        [DataObjectMethod(DataObjectMethodType.Select)]
        public static DataSet GetRegInvoiceTotalFormonth(int month, int year)
        {
            try
            {

                SqlConnection con = new SqlConnection(connection);

                SqlCommand cmd = new SqlCommand("sp_getRegInvoicesTotalBymonth", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("year", year);
                cmd.Parameters.AddWithValue("month", month);

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

        [DataObjectMethod(DataObjectMethodType.Select)]
        public static DataSet GetSpecInvoiceTotalFormonth(int month, int year)
        {
            try
            {

                SqlConnection con = new SqlConnection(connection);

                SqlCommand cmd = new SqlCommand("sp_getSpecInvoicesTotalBymonth", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("year", year);
                cmd.Parameters.AddWithValue("month", month);

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

        [DataObjectMethod(DataObjectMethodType.Select)]
        public static DataSet GetTypeApprovalInvoiceTotalFormonth(int month, int year)
        {
            try
            {

                SqlConnection con = new SqlConnection(connection);

                SqlCommand cmd = new SqlCommand("sp_getTypeApprovalInvoicesTotalBymonth", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("year", year);
                cmd.Parameters.AddWithValue("month", month);

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


        [DataObjectMethod(DataObjectMethodType.Select)]
        public static DataSet GetPaymentsTotalFormonth(int month, int year)
        {
            try
            {

                SqlConnection con = new SqlConnection(connection);

                SqlCommand cmd = new SqlCommand("sp_getPaymentsTotalBymonth", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("year", year);
                cmd.Parameters.AddWithValue("month", month);

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


        [DataObjectMethod(DataObjectMethodType.Select)]
        public static DataSet GetRegPaymentsTotalFormonth(int month, int year)
        {
            try
            {

                SqlConnection con = new SqlConnection(connection);

                SqlCommand cmd = new SqlCommand("sp_getRegPaymentsTotalBymonth", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("year", year);
                cmd.Parameters.AddWithValue("month", month);

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


        [DataObjectMethod(DataObjectMethodType.Select)]
        public static DataSet GetSpecPaymentsTotalFormonth(int month, int year)
        {
            try
            {

                SqlConnection con = new SqlConnection(connection);

                SqlCommand cmd = new SqlCommand("sp_getSpecPaymentsTotalBymonth", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("year", year);
                cmd.Parameters.AddWithValue("month", month);

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


        [DataObjectMethod(DataObjectMethodType.Select)]
        public static DataSet GetTypeApprovalPaymentsTotalFormonth(int month, int year)
        {
            try
            {

                SqlConnection con = new SqlConnection(connection);

                SqlCommand cmd = new SqlCommand("sp_getTypeApprovalPaymentsTotalBymonth", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("year", year);
                cmd.Parameters.AddWithValue("month", month);

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

        [DataObjectMethod(DataObjectMethodType.Select)]
        public static DataSet GetNumberInvoicesFormonth(int month, int year)
        {
            try
            {

                SqlConnection con = new SqlConnection(connection);

                SqlCommand cmd = new SqlCommand("sp_getNumberOfInvoicesBymonth", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("year", year);
                cmd.Parameters.AddWithValue("month", month);

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

        [DataObjectMethod(DataObjectMethodType.Select)]
        public static DataSet GetNumberPaymentsFormonth(int month, int year)
        {
            try
            {

                SqlConnection con = new SqlConnection(connection);

                SqlCommand cmd = new SqlCommand("sp_getNumberOfPaymentsBymonth", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("year", year);
                cmd.Parameters.AddWithValue("month", month);

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

        [DataObjectMethod(DataObjectMethodType.Select)]
        public static DataSet GetBudget(string cid, int invoiceid)
        {
            try
            {

                SqlConnection con = new SqlConnection(connection);

                SqlCommand cmd = new SqlCommand("sp_GetBudget", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Cid", cid);
                cmd.Parameters.AddWithValue("invoiceid", invoiceid);
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

        [DataObjectMethod(DataObjectMethodType.Select)]
        public static DataSet GetCreditMemoDisplayNo(int docid)
        {
            try
            {

                SqlConnection con = new SqlConnection(connection);

                SqlCommand cmd = new SqlCommand("sp_GetCreditMemoDisplayNo", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@docid", docid);
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

        [DataObjectMethod(DataObjectMethodType.Select)]
        public static DataSet RetrieveUsers()
        {
            DataSet ds;
            try
            {

                SqlConnection con = new SqlConnection(connection);

                SqlCommand cmd = new SqlCommand("spGetRegUsers", con);
                cmd.CommandType = CommandType.StoredProcedure;


                SqlDataAdapter da = new SqlDataAdapter(cmd);

                ds = new DataSet();
                da.Fill(ds);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return ds;
        }

        public static ReturnData refineDataset(DataSet ds)
        {
            int invoiceCount = 0;
            decimal totalAmount = 0;
            ReturnData rData = new ReturnData();

            DataSet modifiedSet = new DataSet();
            DataTable dt = new DataTable("Table");
            dt.Columns.Add(new DataColumn("Invoice Id", typeof(int)));
            dt.Columns.Add(new DataColumn("Target Batch", typeof(int)));
            dt.Columns.Add(new DataColumn("Client Name", typeof(string)));
            dt.Columns.Add(new DataColumn("Client Id", typeof(string)));
            dt.Columns.Add(new DataColumn("Author", typeof(string)));
            dt.Columns.Add(new DataColumn("Amount", typeof(string)));

            List<InvoiceList> iList = new List<InvoiceList>();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                var invoiceID = Convert.ToInt32(row["Invoice Id"]);
                var targetBatch = Convert.ToInt32(row["Target Batch"]);
                var clientName = row["Client Name"].ToString();
                var clientId = row["Client Id"].ToString();
                var author = row["Author"].ToString();
                var amount = Convert.ToDecimal(row["Amount"]);
                var lastModified = Convert.ToDateTime(row["LastModified"]);

                int invoiceIndex = checkListRecordAvail(invoiceID, iList);
                if (invoiceIndex > 0)
                {
                    if (iList[invoiceIndex].lastModified < lastModified)
                    {
                        iList[invoiceIndex].targetBatch = targetBatch;
                        iList[invoiceIndex].clientName = clientName;
                        iList[invoiceIndex].clientID = clientId;
                        iList[invoiceIndex].author = author;
                        iList[invoiceIndex].amount = amount;
                        iList[invoiceIndex].lastModified = lastModified;


                    }
                }
                else
                {
                    InvoiceList iData = new InvoiceList();
                    iData.invoiceID = invoiceID;
                    iData.targetBatch = targetBatch;
                    iData.clientName = clientName;
                    iData.clientID = clientId;
                    iData.author = author;
                    iData.amount = amount;
                    iData.lastModified = lastModified;
                    iList.Add(iData);
                }
            }

            for (int i = 0; i < iList.Count; i++)
            {
                DataRow row = dt.NewRow();
                row["Invoice Id"] = iList[i].invoiceID;
                row["Target Batch"] = iList[i].targetBatch;
                row["Client Name"] = iList[i].clientName;
                row["Client Id"] = iList[i].clientID;
                row["Author"] = iList[i].author;
                row["Invoice Id"] = iList[i].invoiceID;
                row["Amount"] = iList[i].amount;

                dt.Rows.Add(row);
                rData.count += 1;
                rData.amount += iList[i].amount;
            }

            modifiedSet.Tables.Add(dt);
            rData.ds = modifiedSet;
            return rData;
        }

        public static int checkListRecordAvail(int invoiceId, List<InvoiceList> iList)
        {
            if (iList.Count == 0)
            {
                return -1;
            }

            for (int i = 0; i < iList.Count; i++)
            {
                if (invoiceId == iList[i].invoiceID)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}