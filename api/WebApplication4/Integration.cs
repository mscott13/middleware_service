using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace WebApplication4
{
    public class Integration
    {
        //string dbsrvIntegration = @"Data Source=SMA-DBSRV\TCIASMS;Initial Catalog = ASMSSAGEINTEGRATION; Integrated Security = True";
        string dbsrvIntegration = @"Data Source=server-erp2\ASMSDEV;Initial Catalog=ASMSSAGEINTEGRATION;Integrated Security=True";

        public int GetInvoiceCount()
        {

            SqlConnection conn = new SqlConnection(dbsrvIntegration);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            int i = 0;


            cmd.Connection = conn;
            cmd.CommandText = "sp_GetInvoiceCount";
            try
            {

                conn.Open();
                reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    i = Convert.ToInt32(reader[0]);
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
            }
            finally
            {
                conn.Close();
            }
            return i;
        }

        public int GetPaymentCount()
        {
            int i = 0;
            SqlConnection conn = new SqlConnection(dbsrvIntegration);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;

            try
            {
                cmd.Connection = conn;
                cmd.CommandText = "sp_GetPaymentCount";

                conn.Open();
                reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    i = Convert.ToInt32(reader[0]);
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
            }
            finally
            {
                conn.Close();
            }
            return i;
        }

        public int GetCustomerCount()
        {

            int i = 0;
            SqlConnection conn = new SqlConnection(dbsrvIntegration);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;

            try
            {
                cmd.Connection = conn;
                cmd.CommandText = "sp_GetCustomerCount";

                conn.Open();
                reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    i = Convert.ToInt32(reader[0]);
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
            }
            finally
            {
                conn.Close();
            }
            return i;
        }

        public int GetPendiningInvCount()
        {
            int i = 0;

            SqlConnection conn = new SqlConnection(dbsrvIntegration);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;

            try
            {
                cmd.Connection = conn;
                cmd.CommandText = "sp_GetPending";

                conn.Open();
                reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    i = Convert.ToInt32(reader[0]);
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
            }
            finally
            {
                conn.Close();
            }
            return i;
        }


        public List<InvoiceBatchInfo> InvoiceDetail()
        {
            SqlConnection conn = new SqlConnection(dbsrvIntegration);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            List<InvoiceBatchInfo> i = new List<InvoiceBatchInfo>(8);
            InvoiceBatchInfo info = new InvoiceBatchInfo();

            cmd.Connection = conn;
            cmd.CommandText = "getInvoiceBatchDetails";

            try
            {
                conn.Open();
                reader = cmd.ExecuteReader();

                decimal regamount = 0;
                int regcount = 0;
                int speccount = 0;
                int nonmajcount = 0;
                int typeAppCount = 0;
                int majcount = 0;

                decimal typeAppAmount = 0;
                decimal majamount = 0;
                decimal nonmajamount = 0;
                decimal specamount = 0;
                while (reader.Read())
                {
                    info = new InvoiceBatchInfo();
                    info.batchId = Convert.ToInt32(reader["batchId"]);
                    info.count = Convert.ToInt32(reader["Count"]);
                    info.amount = formatMoney(reader["amount"].ToString());
                    info.batchType = reader["BatchType"].ToString();

                    if (reader["renstat"].ToString() == "Regulatory")
                    {
                        regamount += Convert.ToDecimal(reader["amount"]);
                        regcount += Convert.ToInt32(reader["Count"]);
                    }

                    if (reader["renstat"].ToString() == "Spectrum")
                    {
                        specamount += Convert.ToDecimal(reader["amount"]);
                        speccount += Convert.ToInt32(reader["Count"]);
                    }

                    if (reader["BatchType"].ToString() == "Non Maj")
                    {
                        nonmajamount += Convert.ToDecimal(reader["amount"]);
                        nonmajcount += Convert.ToInt32(reader["Count"]);

                        info.amount = formatMoney(nonmajamount.ToString()); ;
                        info.count = nonmajcount;
                    }

                    if (reader["BatchType"].ToString() == "Type Approval")
                    {
                        typeAppAmount += Convert.ToDecimal(reader["amount"]);
                        typeAppCount += Convert.ToInt32(reader["Count"]);

                        info.amount = formatMoney(typeAppAmount.ToString()); ;
                        info.count = typeAppCount;
                    }

                    if (reader["BatchType"].ToString() == "Maj")
                    {
                        majamount += Convert.ToDecimal(reader["amount"]);
                        majcount += Convert.ToInt32(reader["Count"]);

                        info.amount = formatMoney(majamount.ToString()); ;
                        info.count = majcount;
                    }

                    info.renstat = reader["renstat"].ToString();
                    info.regamt = formatMoney(regamount.ToString());
                    info.specamt = formatMoney(specamount.ToString());
                    info.regcount = regcount.ToString();
                    info.speccount = speccount.ToString();

                    i.Add(info);
                }
            }
            catch (Exception Ex)
            {
                conn.Close();
            }
            finally
            {
                conn.Close();
            }
            return i;
        }

        public string InvoiceAmountTotal()
        {
            SqlConnection conn = new SqlConnection(dbsrvIntegration);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            string result = "";

            try
            {
                cmd.Connection = conn;
                cmd.CommandText = "invoiceTotalAmount";

                conn.Open();
                reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    result = formatMoney(reader[0].ToString());
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        public List<string> ReceiptDetail()
        {
            SqlConnection conn = new SqlConnection(dbsrvIntegration);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            List<string> data = new List<string>(8);

            try
            {
                cmd.Connection = conn;
                cmd.CommandText = "sp_svc_GetReceiptDetail";

                conn.Open();
                reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    data.Add(formatMoney(reader[0].ToString()));
                    data.Add(reader[1].ToString());
                    data.Add(reader[2].ToString());
                    data.Add(reader[3].ToString());
                    data.Add(reader[4].ToString());
                    data.Add(reader[5].ToString());
                    data.Add(reader[6].ToString());
                    DateTime date = Convert.ToDateTime(reader[7].ToString());
                    data.Add(date.ToString("dd/MM/yyyy"));
                }
                else
                {
                    data.Add(" ");
                    data.Add(" ");
                    data.Add(" ");
                    data.Add(" ");
                    data.Add(" ");
                    data.Add(" ");
                    data.Add(" ");
                    data.Add(" ");
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
            }
            finally
            {
                conn.Close();
            }

            return data;
        }

        public List<StoredInvoice> PendingInvoices()
        {
            SqlConnection conn = new SqlConnection(dbsrvIntegration);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            List<StoredInvoice> data = new List<StoredInvoice>(2);
            StoredInvoice invoice;

            try
            {
                cmd.Connection = conn;
                cmd.CommandText = "sp_svc_GetPendingInvoice";
                string concat = "";
                string temp = "";

                conn.Open();
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        invoice = new StoredInvoice();
                        invoice.invoiceId = reader[0].ToString();

                        if (reader[1].ToString().Length > 27)
                        {
                            temp = reader[1].ToString();
                            for (int i = 0; i < 27; i++)
                            {
                                concat += temp[i];
                            }
                        }
                        else
                        {
                            concat = reader[1].ToString();
                        }

                        invoice.clientName = concat;
                        data.Add(invoice);
                    }
                }
                else
                {

                }
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
            }
            return data;
        }

        public int GetIntegrationStat()
        {
            SqlConnection conn = new SqlConnection(dbsrvIntegration);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            List<string> data = new List<string>(2);
            int i = 0;
            int res = 0;

            try
            {
                cmd.Connection = conn;
                cmd.CommandText = "sp_GetIntegrationStat";

                conn.Open();
                reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    data.Add(reader[0].ToString());
                    data.Add(reader[1].ToString());

                }

                DateTime date = Convert.ToDateTime(data[1]);
                DateTime now = DateTime.Now;
                int stat = Convert.ToInt32(data[0]);
                double diff = (date - now).TotalSeconds;


                if (diff < 0)
                {
                    diff = diff * -1;
                }

                if (diff < 5)
                {
                    if (stat == -1)
                    {
                        SetIntegrationStat(-1);
                        res = -1;
                    }
                    else if (stat == 2)
                    {
                        res = 2;
                    }
                    else if (stat == 3)
                    {
                        res = 3;
                    }
                    else if (stat == 31)
                    {
                        res = 31;
                    }
                    else if (stat == 21)
                    {
                        res = 21;
                    }

                }
                else
                {
                    SetIntegrationStat(-1);
                    res = -1;
                }

                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
            }
            finally
            {
                conn.Close();
            }

            return res;
        }


        public void SetIntegrationStat(int stat)
        {
            SqlConnection conn = new SqlConnection(dbsrvIntegration);
            SqlCommand cmd = new SqlCommand();

            try
            {
                cmd.Connection = conn;
                cmd.CommandText = "sp_UpdateStat @stat";
                cmd.Parameters.AddWithValue("@stat", stat);

                conn.Open();
                cmd.ExecuteNonQuery();

                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
            }
            finally
            {
                conn.Close();
            }
        }

        public List<InvoiceDetail> LatestPendingInvoice_Msg()
        {
            SqlConnection conn = new SqlConnection(dbsrvIntegration);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            List<InvoiceDetail> data = new List<InvoiceDetail>(5);
            InvoiceDetail detail;

            try
            {
                cmd.Connection = conn;
                cmd.CommandText = "getLatestPendingInvoice";
                conn.Open();
                reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        detail = new InvoiceDetail();
                        detail.clientName = reader[0].ToString();
                        detail.clientId = reader[1].ToString();
                        detail.invoiceId = reader[2].ToString();
                        detail.dateCreated = Convert.ToDateTime(reader[3].ToString());
                        detail.formattedDate = detail.dateCreated.ToString("MMM") + " " + detail.dateCreated.Day.ToString() + ", " + detail.dateCreated.Year.ToString() + " | " + detail.dateCreated.ToShortTimeString();
                        detail.Author = reader[4].ToString();
                        detail.amount = formatMoney(reader[5].ToString());

                        data.Add(detail);
                    }
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
            }
            finally
            {
                conn.Close();
            }
            return data;
        }

        public List<PaymentDetail> LatestPaymentDetail_Msg()
        {
            SqlConnection conn = new SqlConnection(dbsrvIntegration);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            List<PaymentDetail> data = new List<PaymentDetail>(6);
            PaymentDetail detail;

            try
            {
                cmd.Connection = conn;
                cmd.CommandText = "getLatestPaymentDetail";
                conn.Open();
                reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        detail = new PaymentDetail();
                        detail.clientId = reader[0].ToString();
                        detail.clientName = reader[1].ToString();
                        detail.dateCreated = Convert.ToDateTime(reader[2].ToString());
                        detail.prepstat = reader[7].ToString();
                        detail.payByCred = reader[8].ToString();

                        if (detail.prepstat == "No" && detail.payByCred=="No")
                        {
                            detail.invoiceId = reader[3].ToString();
                        }
                        else if (detail.prepstat == "Yes")
                        {
                            detail.invoiceId = "Prepayment";
                        }
                        else if (detail.payByCred == "Yes")
                        {
                            detail.invoiceId = "Payment By Credit";
                        }

                        detail.formattedDate = detail.dateCreated.ToString("MMM") + " " + detail.dateCreated.Day.ToString() + ", " + detail.dateCreated.Year.ToString() + " | " + detail.dateCreated.ToShortTimeString();
                        detail.usamount = reader[6].ToString();

                        if (detail.usamount == "" || detail.usamount == "0.00")
                        {
                            detail.amount = formatMoney(reader[4].ToString());
                            detail.currency = "JM";
                            detail.amount = "J$ "+detail.amount;
                        } 
                        else
                        {
                            detail.amount = formatMoney(reader[6].ToString());
                            detail.currency = "US";
                            detail.amount = "US$ " + detail.amount;
                        }

                        detail.sequence = Convert.ToInt32(reader[5]);
                        data.Add(detail);
                    }
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
            }
            finally
            {
                conn.Close();
            }
            return data;
        }

        public List<InvoiceDetail> LatestTransferred_Msg()
        {

            SqlConnection conn = new SqlConnection(dbsrvIntegration);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            List<InvoiceDetail> data = new List<InvoiceDetail>(5);
            InvoiceDetail detail;

            try
            {
                cmd.Connection = conn;
                cmd.CommandText = "sp_GetTransferredInvoices";
                conn.Open();
                reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        detail = new InvoiceDetail();
                        detail.clientName = reader[0].ToString();
                        detail.clientId = reader[1].ToString();
                        detail.invoiceId = reader[2].ToString();
                        detail.dateCreated = Convert.ToDateTime(reader[3].ToString());
                        detail.formattedDate = detail.dateCreated.ToString("MMM") + " " + detail.dateCreated.Day.ToString() + ", " + detail.dateCreated.Year.ToString() + " | " + detail.dateCreated.ToShortTimeString();
                        detail.Author = reader[4].ToString();
                        detail.usamount = reader[9].ToString();

                        if (detail.usamount == "" || detail.usamount == "0.00")
                        {
                            detail.amount = formatMoney(reader[5].ToString());
                            detail.currency = "JM";
                            detail.amount = "J$ " + detail.amount;
                        }
                            
                        else
                        {
                            detail.amount = formatMoney(reader[9].ToString());
                            detail.currency = "US";
                            detail.amount = "US$ " + detail.amount;
                        }
                        
                        detail.sequence = Convert.ToInt32(reader[7]);
                        detail.state = reader[8].ToString();

                        data.Add(detail);
                    }
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
            }
            finally
            {
                conn.Close();
            }
            return data;
        }

        public List<Customer> LatestCreatedCustomer_Msg()
        {

            SqlConnection conn = new SqlConnection(dbsrvIntegration);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            List<Customer> data = new List<Customer>(5);
            Customer detail;

            try
            {
                cmd.Connection = conn;
                cmd.CommandText = "sp_GetCustomerDetail";
                conn.Open();
                reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        detail = new Customer();
                        detail.ClientId = reader[0].ToString();
                        detail.Name = reader[1].ToString();
                        detail.CreatedDate = Convert.ToDateTime(reader[2].ToString());
                        detail.formattedDate = detail.CreatedDate.ToString("MMM") + " " + detail.CreatedDate.Day.ToString() + ", " + detail.CreatedDate.Year.ToString() + " | " + detail.CreatedDate.ToShortTimeString();

                        data.Add(detail);
                    }
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
            }
            finally
            {
                conn.Close();
            }
            return data;
        }

        public void LatestCreatedCustomers()
        {
        }

        string formatMoney(string input)
        {
            bool append = true;
            string decival = "";
            string temp = "";
            string input2 = "";
            string formatted = "";
            int len = 0;
            int b = 1;

            for (int g = 0; g < input.Length; g++)
            {
                if (input[g] != '.' && append)
                {
                    input2 += input[g];
                }
                else
                {
                    if (append)
                    {
                        g++;
                    }

                    append = false;
                    decival += input[g];
                }
            }

            len = input2.Length - 1;
            if (input.Length > 3)
            {
                for (int i = len; i >= 0; i--)
                {
                    temp += input2[i];

                    if (b == 3 && i != 0)
                    {
                        temp += ",";
                        b = 0;
                    }

                    b++;
                }

                for (int l = temp.Length - 1; l >= 0; l--)
                {
                    formatted += temp[l];
                }

                if (decival.Length > 0)
                {
                    formatted += '.' + decival;
                }
                else
                {
                    formatted += ".00";
                }
            }
            else
            {
                formatted = input;
            }
            return formatted;
        }

        public void SendToQueue(string msg)
        {
            SqlConnection conn = new SqlConnection(dbsrvIntegration);
            SqlCommand cmd = new SqlCommand();

            try
            {

                cmd.Connection = conn;
                cmd.CommandText = "sp_sendMessageToQueue @msg";
                cmd.Parameters.AddWithValue("@msg", msg);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
            }
            finally
            {
                conn.Close();
            }
        }

        public Decimal GetRate()
        {
            SqlConnection conn = new SqlConnection(dbsrvIntegration);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            decimal result = 0;

            try
            {

                cmd.Connection = conn;
                cmd.CommandText = "sp_GetAsmsRate";
                conn.Open();
                reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    result = Convert.ToDecimal(reader[0].ToString());
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        public int GetUserCount()
        {

            SqlConnection conn = new SqlConnection(dbsrvIntegration);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            int result = 0;

            try
            {

                cmd.Connection = conn;
                cmd.CommandText = "sp_GetUserCount";
                conn.Open();
                reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    result = Convert.ToInt32(reader[0].ToString());
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        public List<Log> Log(string param)
        {
            SqlConnection conn = new SqlConnection(dbsrvIntegration);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            List<Log> result = new List<Log>();
            Log data;


            try
            {
                cmd.Connection = conn;
                cmd.CommandText = "retrieveLog @param";
                cmd.Parameters.AddWithValue("@param", param);

                conn.Open();
                reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        data = new Log();
                        data.date = Convert.ToDateTime(reader[0]);
                        data.formattedDate = data.date.ToString("MMM") + " " + data.date.Day.ToString() + ", " + data.date.Year.ToString() + " | " + data.date.ToLongTimeString();
                        data.msg = reader[1].ToString();
                        data.id = Convert.ToInt32(reader[2].ToString());
                        result.Add(data);
                    }
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        public List<InvoiceDetail> GetCancellationAndMemos()
        {
            SqlConnection conn = new SqlConnection(dbsrvIntegration);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            List<InvoiceDetail> result = new List<InvoiceDetail>();
            InvoiceDetail data;


            try
            {
                cmd.Connection = conn;
                cmd.CommandText = "sp_getCancellationsAndMemos";
                
                conn.Open();
                reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        data = new InvoiceDetail();
                        DateTime createdDate = Convert.ToDateTime(reader["dateCreated"]);
                        data.formattedDate = createdDate.ToString("MMM") + " " + createdDate.Day.ToString() + ", " + createdDate.Year.ToString() + " | " + createdDate.ToShortTimeString();
                        data.amount = reader["amount"].ToString();
                        data.Author = reader["author"].ToString();
                        data.clientName = reader["clientName"].ToString();
                        data.clientId = reader["clientId"].ToString();
                        data.sequence = Convert.ToInt32(reader["sequence"]);
                        data.invoiceId = reader["invoiceId"].ToString();
                        data.amount = formatMoney(data.amount);

                        int isCreditMemo = Convert.ToInt32(reader["isCreditMemo"]);
                        if (isCreditMemo == 1)
                        {
                            data.Author = "Document Number: "+ reader["credMemoNum"].ToString();
                        }

                        int isVoid = Convert.ToInt32(reader["isvoid"]);
                        int usrate = Convert.ToInt32(reader["usrate"]);

                        if (usrate == 1)
                        {
                            data.currency = "JM";
                            data.amount = "J$ " + data.amount;
                        }
                        else if (usrate > 1)
                        {
                            data.currency = "US";
                            data.amount = "US$ " + data.amount;
                        }

                        if (isVoid == 1)
                        {
                            data.docType = "cancelled_invoice";
                        }
                        else if (isCreditMemo == 1)
                        {
                            data.docType = "credit_memo";
                        }

                        result.Add(data);
                    }
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
            }
            finally
            {
                conn.Close();
            }

            return result;
        }


        ///////////////////////////FOR DEFERRED INCOME REPORTS/////////////////////////////////////////////////////

        public string generateReportId(String ReportType)
        {
            SqlConnection conn = new SqlConnection(dbsrvIntegration);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;
            cmd.CommandText = "exec sp_DIRnewReportID @ReportType";
            cmd.Parameters.AddWithValue("@ReportType", ReportType);
            cmd.Connection = conn;
            string result = "";

            conn.Open();

            reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                result = reader[0].ToString();
            }

            reader.Close();
            conn.Close();
            return result;
        }

        private void dataRouter(string ReportType, DataWrapper data, string recordID, int destination)
        {
            for (int i = 0; i < data.records.Count; i++)
            {
                SqlConnection conn = new SqlConnection(dbsrvIntegration);
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "exec sp_rptRecInsert @ReportType, @reportId, @licenseNumber, @clientCompany, @invoiceID, @budget, @invoiceTotal, @thisPeriodsInv, @balBFwd, @fromRev, @toRev, @closingBal, @totalMonths, @monthUtil, @monthRemain,  @valPStart, @valPEnd, @destination";
                cmd.Connection = conn ;

                cmd.Parameters.AddWithValue("@ReportType", ReportType);
                cmd.Parameters.AddWithValue("@reportId", recordID);
                cmd.Parameters.AddWithValue("@licenseNumber", data.records[i].licenseNumber);
                cmd.Parameters.AddWithValue("@clientCompany", data.records[i].clientCompany);
                cmd.Parameters.AddWithValue("@invoiceID", data.records[i].invoiceID);
                cmd.Parameters.AddWithValue("@budget", data.records[i].budget);
                cmd.Parameters.AddWithValue("@invoiceTotal", data.records[i].invoiceTotal);
                cmd.Parameters.AddWithValue("@thisPeriodsInv", data.records[i].thisPeriodsInv);
                cmd.Parameters.AddWithValue("@balBFwd", data.records[i].balBFwd);
                cmd.Parameters.AddWithValue("@fromRev", data.records[i].fromRev);
                cmd.Parameters.AddWithValue("@toRev", data.records[i].toRev);
                cmd.Parameters.AddWithValue("@closingBal", data.records[i].closingBal);
                cmd.Parameters.AddWithValue("@totalMonths", data.records[i].totalMonths);
                cmd.Parameters.AddWithValue("@monthUtil", data.records[i].monthUtil);
                cmd.Parameters.AddWithValue("@monthRemain", data.records[i].monthRemain);
                cmd.Parameters.AddWithValue("@valPStart", data.records[i].valPStart);
                cmd.Parameters.AddWithValue("@valPEnd", data.records[i].valPEnd);
                cmd.Parameters.AddWithValue("@destination", destination);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }

            insertSubtotals(ReportType, recordID, data, destination);
        }

        public string saveReport(string ReportType, List<DataWrapper> categories, Totals total)
        {
            string id = generateReportId(ReportType);

            for (int i = 0; i < categories.Count; i++)
            {
                dataRouter(ReportType, categories[i], id, i);
            }

            insertTotals(ReportType, id, total);
            return id;
        }

        public void insertSubtotals(string ReportType, string reportID, DataWrapper data, int destination)
        {
            SqlConnection conn = new SqlConnection(dbsrvIntegration);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "exec sp_insertSubtotals @ReportType, @reportId, @category, @invoiceTotal, @balanceBFwd, @toRev, @closingBal, @fromRev, @budget";
            cmd.Connection = conn;

            cmd.Parameters.AddWithValue("@ReportType", ReportType);
            cmd.Parameters.AddWithValue("@reportId", reportID);
            cmd.Parameters.AddWithValue("@category", destination);
            cmd.Parameters.AddWithValue("@invoiceTotal", data.subT_invoiceTotal);
            cmd.Parameters.AddWithValue("@balanceBFwd", data.subT_balBFwd);
            cmd.Parameters.AddWithValue("@toRev", data.subT_toRev);
            cmd.Parameters.AddWithValue("@closingBal", data.subT_closingBal);
            cmd.Parameters.AddWithValue("@fromRev", data.subT_fromRev);
            cmd.Parameters.AddWithValue("@budget", data.subT_budget);

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }


        public void insertTotals(string ReportType, string reportID, Totals total)
        {
            SqlConnection conn = new SqlConnection(dbsrvIntegration);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "exec sp_insertTotals @ReportType, @recordID, @invoiceTotal, @balanceBFwd, @toRev, @closingBal, @fromRev, @budget";
            cmd.Connection = conn;

            cmd.Parameters.AddWithValue("@ReportType", ReportType);
            cmd.Parameters.AddWithValue("@recordID", reportID);
            cmd.Parameters.AddWithValue("@invoiceTotal", total.tot_invoiceTotal);
            cmd.Parameters.AddWithValue("@balanceBFwd", total.tot_balBFwd);
            cmd.Parameters.AddWithValue("@toRev", total.tot_toRev);
            cmd.Parameters.AddWithValue("@closingBal", total.tot_closingBal);
            cmd.Parameters.AddWithValue("@fromRev", total.tot_fromRev);
            cmd.Parameters.AddWithValue("@budget", total.tot_budget);

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public DeferredData getDeferredRpt(string ReportType, string report_id)
        {
            List<DataWrapper> tables = new List<DataWrapper>();
            DataWrapper cell_table = new DataWrapper();
            DataWrapper micro_table = new DataWrapper();
            DataWrapper bbrand_table = new DataWrapper();
            DataWrapper vsat_table = new DataWrapper();
            DataWrapper other_table = new DataWrapper();
            DataWrapper trunking_table = new DataWrapper();
            DataWrapper aero_table = new DataWrapper();
            DataWrapper marine_table = new DataWrapper();
            DataWrapper dservices_table = new DataWrapper();

            cell_table.label = "Cellular";
            cell_table.records = getDeferredPartial(ReportType, 0, report_id);
            cell_table.setSubTotals(getDeferredPartialSubs(ReportType, 0, report_id));

            bbrand_table.label = "Broadband";
            bbrand_table.records = getDeferredPartial(ReportType, 1, report_id);
            bbrand_table.setSubTotals(getDeferredPartialSubs(ReportType, 1, report_id));

            micro_table.label = "Microwave";
            micro_table.records = getDeferredPartial(ReportType, 2, report_id);
            micro_table.setSubTotals(getDeferredPartialSubs(ReportType, 2, report_id));

            vsat_table.label = "Vsat";
            vsat_table.records = getDeferredPartial(ReportType, 3, report_id);
            vsat_table.setSubTotals(getDeferredPartialSubs(ReportType, 3, report_id));

            marine_table.label = "Marine";
            marine_table.records = getDeferredPartial(ReportType, 4, report_id);
            marine_table.setSubTotals(getDeferredPartialSubs(ReportType, 4, report_id));

            dservices_table.label = "Data & Services";
            dservices_table.records = getDeferredPartial(ReportType, 5, report_id);
            dservices_table.setSubTotals(getDeferredPartialSubs(ReportType, 5, report_id));

            aero_table.label = "Aeronautical";
            aero_table.records = getDeferredPartial(ReportType, 6, report_id);
            aero_table.setSubTotals(getDeferredPartialSubs(ReportType, 6, report_id));

            trunking_table.label = "Trunking";
            trunking_table.records = getDeferredPartial(ReportType, 7, report_id);
            trunking_table.setSubTotals(getDeferredPartialSubs(ReportType, 7, report_id));

            other_table.label = "Other";
            other_table.records = getDeferredPartial(ReportType, 8, report_id);
            other_table.setSubTotals(getDeferredPartialSubs(ReportType, 8, report_id));

            tables.Add(cell_table);
            tables.Add(bbrand_table);
            tables.Add(micro_table);
            tables.Add(vsat_table);
            tables.Add(marine_table);
            tables.Add(dservices_table);
            tables.Add(aero_table);
            tables.Add(trunking_table);
            tables.Add(other_table);
            
            DeferredData d = new DeferredData();
            d.Categories = tables;
            d.Total = getDeferredTotal(ReportType, report_id);

            return d;
        }

        private List<UIData> getDeferredPartial(string ReportType, int index, string report_id)
        {
            SqlConnection conn = new SqlConnection(dbsrvIntegration);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;

            List<UIData> udt = new List<UIData>();
            cmd.Connection = conn;
            cmd.CommandText = "EXEC sp_getDeferredPartial @ReportType, @index, @report_id";
            cmd.Parameters.AddWithValue("@ReportType", ReportType);
            cmd.Parameters.AddWithValue("@index", index);
            cmd.Parameters.AddWithValue("@report_id", report_id);

            conn.Open();
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    UIData record = new UIData();
                    record.licenseNumber = reader["licenseNumber"].ToString();
                    record.clientCompany = reader["clientCompany"].ToString();
                    record.invoiceID = reader["invoiceID"].ToString();
                    record.budget = reader["budget"].ToString();
                    record.invoiceTotal = reader["invoiceTotal"].ToString();
                    record.thisPeriodsInv = reader["thisPeriodsInvoice"].ToString();
                    record.balBFwd = reader["balanceBFoward"].ToString();
                    record.fromRev = reader["fromRevenue"].ToString();
                    record.toRev = reader["toRevenue"].ToString();
                    record.closingBal = reader["closingBalance"].ToString();
                    record.totalMonths = Convert.ToInt32(reader["totalMonths"]);
                    record.monthUtil = Convert.ToInt32(reader["monthsUtilized"]);
                    record.monthRemain = Convert.ToInt32(reader["monthsRemaining"]);
                    record.valPStart = reader["validityStart"].ToString();
                    record.valPEnd = reader["validityEnd"].ToString();

                    udt.Add(record);
                }

                reader.Close();
                conn.Close();
                return udt;
            }
            else
            {
                reader.Close();
                conn.Close();
                return udt;
            }
        }

        private SubTotals getDeferredPartialSubs(string ReportType, int index, string report_id)
        {
            SqlConnection conn = new SqlConnection(dbsrvIntegration);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;

            SubTotals subs = new SubTotals();
            cmd.Connection = conn;
            cmd.CommandText = "EXEC sp_getDeferredPartialSubs @ReportType, @index, @record_id";
            cmd.Parameters.AddWithValue("@ReportType", ReportType);
            cmd.Parameters.AddWithValue("@index", index);
            cmd.Parameters.AddWithValue("@record_id", report_id);

            conn.Open();
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                subs.invoiceTotal = reader["invoiceTotal"].ToString();
                subs.balanceBFwd = reader["balanceBFwd"].ToString();
                subs.toRev = reader["toRev"].ToString();
                subs.closingBal = reader["closingBal"].ToString();
                subs.fromRev = reader["fromRev"].ToString();
                subs.budget = reader["budget"].ToString();

                reader.Close();
                conn.Close();
                return subs;
            }
            else
            {
                reader.Close();
                conn.Close();
                return subs;
            }
        }

        public Totals getDeferredTotal(string ReportType, string recordID)
        {
            SqlConnection conn = new SqlConnection(dbsrvIntegration);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;

            Totals totals = new Totals();
            cmd.Connection = conn;
            cmd.CommandText = "EXEC sp_getDeferredRptTotals @ReportType, @record_id";
            cmd.Parameters.AddWithValue("@ReportType", ReportType);
            cmd.Parameters.AddWithValue("@record_id", recordID);

            conn.Open();
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                totals.tot_invoiceTotal = reader["invoiceTotal"].ToString();
                totals.tot_balBFwd = reader["balanceBFwd"].ToString();
                totals.tot_toRev = reader["toRev"].ToString();
                totals.tot_closingBal = reader["closingBal"].ToString();
                totals.tot_fromRev = reader["fromRev"].ToString();
                totals.tot_budget = reader["budget"].ToString();

                reader.Close();
                conn.Close();
                return totals;
            }
            else
            {
                reader.Close();
                conn.Close();
                return totals;
            }
        }

        public string getReportID(string ReportType, int month, int year)
        {
            string result = "";
            SqlConnection conn = new SqlConnection(dbsrvIntegration);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;

            Totals totals = new Totals();
            cmd.Connection = conn;
            cmd.CommandText = "EXEC sp_getDeferredReportId @ReportType, @month, @year";
            cmd.Parameters.AddWithValue("@ReportType", ReportType);
            cmd.Parameters.AddWithValue("@month", month);
            cmd.Parameters.AddWithValue("@year", year);

            conn.Open();
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                result = reader["report_id"].ToString();

                reader.Close();
                conn.Close();
                return result;
            }
            else
            {
                reader.Close();
                conn.Close();
                return result;
            }
        }

        public List<string> getInvoiceIDs()
        {
            List<string> ids = new List<string>();
            SqlConnection conn = new SqlConnection(dbsrvIntegration);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;

            Totals totals = new Totals();
            cmd.Connection = conn;
            cmd.CommandText = "EXEC sp_getInvoiceIds";

            conn.Open();
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    ids.Add(reader["ARInvoiceID"].ToString());
                }
                reader.Close();
                conn.Close();
                return ids;
            }
            else
            {
                reader.Close();
                conn.Close();
                return ids;
            }
        }

        public List<ReportRawData> getDIRInformation(string ReportType, DateTime searchStartDate, DateTime searchEndDate)
        {
            SqlConnection conn = new SqlConnection(dbsrvIntegration);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;

            List<ReportRawData> reportInfo = new List<ReportRawData>();
            ReportRawData record = new ReportRawData();
            cmd.Connection = conn;
            cmd.CommandText = "EXEC sp_getDIRInformation @ReportType, @searchStartDate, @searchEndDate";
            cmd.Parameters.AddWithValue("@ReportType", ReportType);
            cmd.Parameters.AddWithValue("@searchStartDate", searchStartDate);
            cmd.Parameters.AddWithValue("@searchEndDate", searchEndDate);

            conn.Open();
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    record = new ReportRawData();
                    record.clientID = reader.GetInt32(0);
                    record.ccNum = reader["ccNum"].ToString();
                    record.clientCompany = reader["clientCompany"].ToString();
                    record.clientFname = reader["clientFname"].ToString();
                    record.clientLname = reader["clientLname"].ToString();
                    record.Budget = Convert.ToDecimal(reader["budget"]);
                    record.InvAmount = Convert.ToDecimal(reader["InvAmount"]);
                    record.ExistedBefore = Convert.ToInt32(reader["ExistedBefore"]);
                    record.LastRptsClosingBal = reader["LastRptsClosingBal"].ToString();
                    record.LastRptsStartValPeriod = reader["LastRptsStartValPeriod"].ToString();
                    record.LastRptsEndValPeriod = reader["LastRptsEndValPeriod"].ToString();
                    record.CurrentStartValPeriod = reader.GetDateTime(11);
                    record.CurrentEndValPeriod = reader.GetDateTime(12);
                    record.CreditGLID = reader.GetInt32(13);
                    record.notes = reader["notes"].ToString();
                    record.ARInvoiceID = reader["ARInvoiceID"].ToString();
                    record.InvoiceCreationDate = reader.GetDateTime(16);
                    record.isCancelled = Convert.ToInt32(reader["isCancelled"]);
                    record.isCreditMemo = Convert.ToInt32(reader["isCreditMemo"]);
                    record.CreditMemoNum = reader["CreditMemoNum"].ToString();
                    record.CreditMemoAmt = Convert.ToDecimal(reader["CreditMemoAmt"]);

                    reportInfo.Add(record);
                }

                reader.Close();
                conn.Close();
                return reportInfo;
            }
            else
            {
                reader.Close();
                conn.Close();
                return reportInfo;
            }            
        }

        public DateTime getNextRptDate(String ReportType)
        { 
            DateTime nextRptDate = new DateTime();
            SqlConnection conn = new SqlConnection(dbsrvIntegration);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;

            cmd.Connection = conn;
            cmd.CommandText = "EXEC sp_getNextRptDate @ReportType";
            cmd.Parameters.AddWithValue("@ReportType", ReportType);

            conn.Open();
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                nextRptDate = reader.GetDateTime(0);

                reader.Close();
                conn.Close();
                return nextRptDate;
            }
            else
            {
                reader.Close();
                conn.Close();
                return nextRptDate;
            }
        }
    }
}