using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Data;
using middleware_service.Database_Classes;

namespace middleware_service.Database_Operations
{
    public class Integration
    {
        public const string CINTEGRATION = "cIntegration";
        public const string CGENERIC = "cGeneric";
        public const string CMSGQUEUE = "cMsgQueue";
        public static SqlConnection cGeneric, cIntegration, cMsgQueue;

        public void openConnection(string target)
        {
            switch (target)
            {
                case "cIntegration":
                    cIntegration = new SqlConnection(Constants.dbIntegration);
                    cIntegration.Open();
                    break;
                case "cGeneric":
                    cGeneric = new SqlConnection(Constants.dbGeneric);
                    cGeneric.Open();
                    break;
                case "cMsgQueue":
                    cMsgQueue = new SqlConnection(Constants.dbIntegration);
                    cMsgQueue.Open();
                    break;
            }
        }

        public void closeConnection(string target)
        {
            switch (target)
            {
                case "cIntegration":
                    if (cIntegration != null)
                    {
                        cIntegration.Close();
                    }
                    break;
                case "cGeneric":
                    if (cGeneric != null)
                    {
                        cGeneric.Close();
                    }
                    break;
                case "cMsgQueue":
                    if (cMsgQueue != null)
                    {
                        cMsgQueue.Close();
                    }
                    break;
            }
        }

        public List<ReportRawData> getDIRInformation(string ReportType, DateTime searchStartDate, DateTime searchEndDate)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;
            cmd.Connection = cIntegration;

            List<ReportRawData> reportInfo = new List<ReportRawData>();
            ReportRawData record = new ReportRawData();
            cmd.Connection = cIntegration;
            cmd.CommandText = "EXEC sp_getDIRInformation @ReportType, @searchStartDate, @searchEndDate";
            cmd.Parameters.AddWithValue("@ReportType", ReportType);
            cmd.Parameters.AddWithValue("@searchStartDate", searchStartDate);
            cmd.Parameters.AddWithValue("@searchEndDate", searchEndDate);
            
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

                closeConnection(CINTEGRATION);
                return reportInfo;
            }
            else
            {
                closeConnection(CINTEGRATION);
                return reportInfo;
            }
        }

        public List<Batch> GetExpiryBatchDate()
        {
            openConnection(CINTEGRATION);
            List<Batch> lstInvoiceBatchData = new List<Batch>(2);
            List<Batch> lstInvoiceBatchDataRet = new List<Batch>(2);
            Batch batch;

            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;
            cmd.Connection = cIntegration;
            cmd.CommandText = "EXEC sp_GetOpenBatch";
            reader = cmd.ExecuteReader();
            int i = 0;
            while (reader.Read())
            {
                batch = new Batch();
                batch.BatchId = Convert.ToInt32(reader["BatchId"].ToString());
                batch.CreatedDate = Convert.ToDateTime(reader["CreatedDate"].ToString());
                batch.ExpiryDate = Convert.ToDateTime(reader["ExpiryDate"].ToString());
                batch.BatchType = reader["BatchType"].ToString();
                batch.Status = reader["Status"].ToString();
                batch.Count = Convert.ToInt32(reader["Count"]);

                lstInvoiceBatchData.Add(batch);
                i++;
            }

            lstInvoiceBatchDataRet.Add(new Batch());
            lstInvoiceBatchDataRet.Add(new Batch());

            for (int b = 0; b < lstInvoiceBatchData.Count; b++)
            {
                if (lstInvoiceBatchData[b].BatchType == "Spectrum")
                {
                    lstInvoiceBatchDataRet[0].BankCode = lstInvoiceBatchData[b].BankCode;
                    lstInvoiceBatchDataRet[0].BatchId = lstInvoiceBatchData[b].BatchId;
                    lstInvoiceBatchDataRet[0].BatchType = lstInvoiceBatchData[b].BatchType;
                    lstInvoiceBatchDataRet[0].Count = lstInvoiceBatchData[b].Count;
                    lstInvoiceBatchDataRet[0].CreatedDate = lstInvoiceBatchData[b].CreatedDate;
                    lstInvoiceBatchDataRet[0].ExpiryDate = lstInvoiceBatchData[b].ExpiryDate;
                }
                else
                {
                    lstInvoiceBatchDataRet[1].BankCode = lstInvoiceBatchData[b].BankCode;
                    lstInvoiceBatchDataRet[1].BatchId = lstInvoiceBatchData[b].BatchId;
                    lstInvoiceBatchDataRet[1].BatchType = lstInvoiceBatchData[b].BatchType;
                    lstInvoiceBatchDataRet[1].Count = lstInvoiceBatchData[b].Count;
                    lstInvoiceBatchDataRet[1].CreatedDate = lstInvoiceBatchData[b].CreatedDate;
                    lstInvoiceBatchDataRet[1].ExpiryDate = lstInvoiceBatchData[b].ExpiryDate;
                }
            }


            closeConnection(CINTEGRATION);
            return lstInvoiceBatchDataRet;
        }

        public void closeInvoiceBatch(int batchId)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "EXEC closeBatch @batchId";
            cmd.Parameters.AddWithValue("@batchId", batchId);
            cmd.Connection = cIntegration;

            cmd.ExecuteNonQuery();
            closeConnection(CINTEGRATION);
        }

        public Maj getMajDetail(int referenceNumber)
        {
            openConnection(CGENERIC);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;

            Maj mj = new Maj();
            cmd.Connection = cGeneric;
            cmd.CommandText = "EXEC getMajDetail @referenceNumber";
            cmd.Parameters.AddWithValue("@referenceNumber", referenceNumber);
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                mj.stationType = reader["stationType"].ToString();
                if (reader["CertificateType"].ToString() != "") mj.certificateType = Convert.ToInt32(reader["CertificateType"]);
                if (reader["subStationType"].ToString() != "") mj.substationType = Convert.ToInt32(reader["subStationType"]);
                mj.proj = reader["Proj"].ToString();
            }

            closeConnection(CGENERIC);
            return mj;
        }


        public DataSet GetRenewalInvoiceValidity(int invoiceid)
        {
            openConnection(CGENERIC);

            SqlCommand cmd = new SqlCommand("EXEC sp_getValidityRenewalInvoice", cGeneric);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@invoiceid", invoiceid);

            SqlDataAdapter da = new SqlDataAdapter(cmd);

            DataSet ds = new DataSet();
            da.Fill(ds);
            da.Dispose();
            closeConnection(CGENERIC);
            return ds;


        }
        public DataSet GetRenewalInvoiceValidity1(int invoiceid)
        {
            openConnection(CGENERIC);

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cGeneric;
            cmd.CommandText = "EXEC sp_getValidityRenewalInvoice @invoiceid";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@invoiceid", invoiceid);

            SqlDataAdapter da = new SqlDataAdapter(cmd);

            DataSet ds = new DataSet();
            da.Fill(ds);
            da.Dispose();
            closeConnection(CGENERIC);
            return ds;

        }
        public int getInvoiceReference(int invoiceId)
        {
            openConnection(CINTEGRATION);

            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            int refNumber = -1;

            cmd.Connection = cGeneric;
            cmd.CommandText = "EXEC getInvoiceRef @invoiceId";
            cmd.Parameters.AddWithValue("@invoiceId", invoiceId);

            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                if (reader[0].ToString() != "")
                {
                    refNumber = Convert.ToInt32(reader[0]);
                }
            }

            closeConnection(CGENERIC);
            return refNumber;
        }

        public void createInvoiceBatch(double daysTillEx, int batchId, string batchType, string renstat)
        {
            openConnection(CINTEGRATION);

            SqlCommand cmd = new SqlCommand();
            var expiryDate = DateTime.Now.AddDays(daysTillEx);

            cmd.CommandText = "EXEC createBatch @batchId, @expirydate, @batchType, @renstat";
            cmd.Connection = cIntegration;
            cmd.Parameters.AddWithValue("@batchId", batchId);
            cmd.Parameters.AddWithValue("@expirydate", expiryDate);
            cmd.Parameters.AddWithValue("@batchType", batchType);
            cmd.Parameters.AddWithValue("@renstat", renstat);

            cmd.ExecuteNonQuery();
            closeConnection(CINTEGRATION);
        }

        public bool batchAvail(string batchType)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            int result = -1;

            cmd.Connection = cIntegration;
            cmd.CommandText = "EXEC batchAvail @batchType";
            cmd.Parameters.AddWithValue("@batchType", batchType);


            reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                result = Convert.ToInt32(reader[0]);
            }

            closeConnection(CINTEGRATION);

            if (result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool isBatchExpired(int batchId)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            DateTime expiryDate = DateTime.Now;
            string renstat = " ";

            cmd.Connection = cIntegration;
            cmd.CommandText = "EXEC getBatchExpiry @batchId";
            cmd.Parameters.AddWithValue("@batchId", batchId);


            reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                expiryDate = Convert.ToDateTime(reader[0]);
                renstat = Convert.ToString(reader[1]);
            }

            closeConnection(CINTEGRATION);

            if (renstat == "Regulatory" || renstat == "Spectrum")
            {
                return false;
            }
            else if (DateTime.Now > expiryDate)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int getAvailBatch(string batchType)
        {
            openConnection(CINTEGRATION);

            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            int result = -1;

            cmd.Connection = cIntegration;
            cmd.CommandText = "EXEC getBatch @batchType";
            cmd.Parameters.AddWithValue("@batchType", batchType);


            reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                result = Convert.ToInt32(reader[0]);
            }


            closeConnection(CINTEGRATION);
            return result;
        }

        public List<Batch> GetExpiryBatchDate_Payment()
        {
            openConnection(CINTEGRATION);
            List<Batch> lstInvoiceBatchData = new List<Batch>(2);
            Batch batch;

            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;
            cmd.Connection = cIntegration;
            cmd.CommandText = "EXEC sp_GetOpenBatch_Payment";

            reader = cmd.ExecuteReader();
            int i = 0;
            while (reader.Read())
            {
                batch = new Batch();
                batch.BatchId = Convert.ToInt32(reader[0].ToString());
                batch.CreatedDate = Convert.ToDateTime(reader[1].ToString());
                batch.ExpiryDate = Convert.ToDateTime(reader[2].ToString());

                batch.Status = reader[3].ToString();
                batch.BankCode = reader[4].ToString();
                batch.Count = Convert.ToInt32(reader[5].ToString());

                lstInvoiceBatchData.Add(batch);
                i++;
            }


            closeConnection(CINTEGRATION);
            return lstInvoiceBatchData;
        }

        public void OpenNewBatchSet(double DaysTillExpired, int LastBatchId)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();

            var CreatedDate = DateTime.Now;
            var ExpiryDate = DateTime.Now.AddDays(DaysTillExpired);

            cmd.Connection = cIntegration;
            cmd.CommandText = "Exec sp_NewBatchSet @FirstBatchId, @CreatedDate, @ExpiryDate";
            cmd.Parameters.AddWithValue("@FirstBatchId", LastBatchId + 1);
            cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
            cmd.Parameters.AddWithValue("@ExpiryDate", ExpiryDate);

            cmd.ExecuteNonQuery();
            closeConnection(CINTEGRATION);

        }

        public void openNewReceiptBatch(double DaysTillExpired, int LastBatchId, string bankcode)
        {

            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();

            var CreatedDate = DateTime.Now;
            var ExpiryDate = DateTime.Now.AddDays(DaysTillExpired);

            cmd.Connection = cIntegration;
            cmd.CommandText = "Exec sp_NewBatchSet_Payment @batchId, @CreatedDate, @ExpiryDate, @bankcode";
            cmd.Parameters.AddWithValue("@batchId", LastBatchId);
            cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
            cmd.Parameters.AddWithValue("@ExpiryDate", ExpiryDate);
            cmd.Parameters.AddWithValue("@bankcode", bankcode);


            int i = cmd.ExecuteNonQuery();
            closeConnection(CINTEGRATION);
        }

        public void closeReceiptBatch(int batchId)
        {

            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = cIntegration;
            cmd.CommandText = "EXEC closeReceiptBatch @batchId";
            cmd.Parameters.AddWithValue("@batchId", batchId);

            cmd.ExecuteNonQuery();
            closeConnection(CINTEGRATION);

        }

        public void updateBatchAmount(string batchType, decimal amount)
        {

            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = cIntegration;
            cmd.CommandText = "EXEC updateBatchAmount @batchType, @amount";
            cmd.Parameters.AddWithValue("@batchType", batchType);
            cmd.Parameters.AddWithValue("@amount", amount);

            cmd.ExecuteNonQuery();
            closeConnection(CINTEGRATION);

        }

        public string getBankCodeId(string bankcode)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            string bankcodeid = "";

            cmd.Connection = cIntegration;
            cmd.CommandText = "EXEC getBankCode @bankcode";
            cmd.Parameters.AddWithValue("@bankcode", bankcode);

            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                bankcodeid = reader[0].ToString();
            }

            closeConnection(CINTEGRATION);
            return bankcodeid;
        }


        public DateTime getDocDate(int docNumber)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            DateTime date = DateTime.Now;

            cmd.Connection = cIntegration;
            cmd.CommandText = "EXEC getInvDocDate @docNumber";
            cmd.Parameters.AddWithValue("@docNumber", docNumber);

            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                date = Convert.ToDateTime(reader[0]);
            }


            closeConnection(CINTEGRATION);
            return date;
        }

        public void CloseOldBatchSet()
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = cIntegration;
            cmd.CommandText = "Exec sp_CloseBatch";


            cmd.ExecuteNonQuery();
            closeConnection(CINTEGRATION);
        }

        public void CloseOldBatchSet_payment()
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = cIntegration;
            cmd.CommandText = "Exec sp_CloseBatch_Payment";
            cmd.ExecuteNonQuery();
            closeConnection(CINTEGRATION);
        }

        public int getLastBatchId()
        {
            openConnection(CINTEGRATION);
            int BatchId = -1;

            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;

            cmd.Connection = cIntegration;
            cmd.CommandText = "Exec sp_GetLastBatchId";

            reader = cmd.ExecuteReader();

            reader.Read();
            if (reader.HasRows)
            {
                BatchId = Convert.ToInt32(reader[0]);
            }


            closeConnection(CINTEGRATION);
            return BatchId;
        }

        public int getLastBatchId_payment()
        {
            openConnection(CINTEGRATION);
            int BatchId = -1;

            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;


            cmd.Connection = cIntegration;
            cmd.CommandText = "Exec sp_GetLastBatchId_Payment";

            reader = cmd.ExecuteReader();

            reader.Read();
            if (reader.HasRows)
            {
                BatchId = Convert.ToInt32(reader[0]);
            }


            closeConnection(CINTEGRATION);
            return BatchId;
        }

        public void Init(int LastBatchId, double DaysTillExpired)
        {

            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();


            var CreatedDate = DateTime.Now;
            var ExpiryDate = DateTime.Now.AddDays(DaysTillExpired);
            LastBatchId += 1;

            cmd.Connection = cIntegration;
            cmd.CommandText = "Exec sp_NewBatchSet @FirstBatchId, @CreatedDate, @ExpiryDate";
            cmd.Parameters.AddWithValue("@FirstBatchId", LastBatchId);
            cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
            cmd.Parameters.AddWithValue("@ExpiryDate", ExpiryDate);


            cmd.ExecuteNonQuery();
            closeConnection(CINTEGRATION);
        }

        public void UpdateReference(string Bank, string reference)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = cIntegration;
            cmd.CommandText = "Exec sp_UpdateReference @Bank, @reference";
            cmd.Parameters.AddWithValue("@Bank", Bank);
            cmd.Parameters.AddWithValue("@reference", reference);


            cmd.ExecuteNonQuery();
            closeConnection(CINTEGRATION);
        }

        public int GetReferenceCount(string bank)
        {
            openConnection(CINTEGRATION);
            string bankcode = "";
            int count = 0;

            if (bank == "FGBJMREC")
            {
                bankcode = "10010-100";
            }
            else if (bank == "FGBUSMRC")
            {
                bankcode = "10012-100";
            }
            else if (bank == "NCBJMREC")
            {
                bankcode = "10020-100";
            }


            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;


            cmd.Connection = cIntegration;
            cmd.CommandText = "Exec sp_CountReference @Bank";
            cmd.Parameters.AddWithValue("@Bank", bankcode);


            reader = cmd.ExecuteReader();
            reader.Read();

            if (reader.HasRows)
            {
                count = Convert.ToInt32(reader[0].ToString());
            }

            closeConnection(CINTEGRATION);
            return count;
        }

        public void Init_Payment(int LastBatchId, double DaysTillExpired)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();

            var CreatedDate = DateTime.Now;
            var ExpiryDate = DateTime.Now.AddDays(DaysTillExpired);
            LastBatchId += 1;

            cmd.Connection = cIntegration;
            cmd.CommandText = "Exec sp_NewBatchSet_Payment @FirstBatchId, @CreatedDate, @ExpiryDate";
            cmd.Parameters.AddWithValue("@FirstBatchId", LastBatchId);
            cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
            cmd.Parameters.AddWithValue("@ExpiryDate", ExpiryDate);


            cmd.ExecuteNonQuery();
            closeConnection(CINTEGRATION);
        }

        public bool isInitialized()
        {
            openConnection(CINTEGRATION);
            bool truth = false;

            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;
            int count = 0;

            cmd.CommandText = "Exec sp_getCount";
            cmd.Connection = cIntegration;


            reader = cmd.ExecuteReader();
            reader.Read();

            if (reader.HasRows)
            {
                count = Convert.ToInt32(reader[0].ToString());
            }
            if (count > 0)
            {
                truth = true;
            }

            closeConnection(CINTEGRATION);
            return truth;
        }

        public bool isInitialized_payment()
        {
            openConnection(CINTEGRATION);
            bool truth = false;

            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;
            int count = 0;

            cmd.CommandText = "Exec sp_GetCount_payment";
            cmd.Connection = cIntegration;


            reader = cmd.ExecuteReader();
            reader.Read();

            if (reader.HasRows)
            {
                count = Convert.ToInt32(reader[0].ToString());
            }
            if (count > 0)
            {
                truth = true;
            }

            closeConnection(CINTEGRATION);
            return truth;
        }

        public void UpdateBatchCount(string BatchType)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = cIntegration;
            cmd.CommandText = "Exec UpdateBatchCount @BatchType";
            cmd.Parameters.AddWithValue("@BatchType", BatchType);


            cmd.ExecuteNonQuery();
            closeConnection(CINTEGRATION);
        }

        public void IncrementReferenceNumber(string BankCode, decimal amount)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = cIntegration;
            cmd.CommandText = "Exec sp_IncrementRefNumber @BankCode, @amount";
            cmd.Parameters.AddWithValue("@BankCode", BankCode);
            cmd.Parameters.AddWithValue("@amount", amount);

            cmd.ExecuteNonQuery();
            closeConnection(CINTEGRATION);
        }

        public decimal GetRate()
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;
            decimal result = 0;

            cmd.Connection = cIntegration;
            cmd.CommandText = "EXEC sp_GetAsmsRate";
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                result = Convert.ToDecimal(reader[0].ToString());
            }

            closeConnection(CINTEGRATION);
            return result;
        }


        public void UpdateBatchCountPayment(string BatchId)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = cIntegration;
            cmd.CommandText = "Exec sp_IncrementEntryCount_payment @BatchId";
            cmd.Parameters.AddWithValue("@BatchId", BatchId);


            cmd.ExecuteNonQuery();
            closeConnection(CINTEGRATION);
        }

        public string GetInitialRef(string BankCodeId)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            string refNumber = "";

            cmd.CommandText = "exec sp_GetInitialRefNumber @BankCodeId";
            cmd.Parameters.AddWithValue("@BankCodeId", BankCodeId);

            cmd.Connection = cIntegration;
            reader = cmd.ExecuteReader();
            reader.Read();

            if (reader.HasRows)
            {
                refNumber = reader[0].ToString();
            }

            closeConnection(CINTEGRATION);
            return refNumber;
        }

        public decimal GetUsRateByInvoice(int invoiceid)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            decimal rate = 1;

            cmd.CommandText = "exec sp_GetUsRateByInvoice @invoiceid";
            cmd.Parameters.AddWithValue("@invoiceid", invoiceid);

            cmd.Connection = cIntegration;
            reader = cmd.ExecuteReader();
            reader.Read();

            if (reader.HasRows)
            {
                rate = Convert.ToDecimal(reader[0].ToString());
            }


            closeConnection(CINTEGRATION);
            return rate;
        }

        public string GetCurrentRef(string BankCode)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            string refNumber = "";

            cmd.CommandText = "exec sp_GetLastRefNumber @BankCodeId";
            cmd.Parameters.AddWithValue("@BankCodeId", BankCode);

            cmd.Connection = cIntegration;

            reader = cmd.ExecuteReader();
            reader.Read();

            if (reader.HasRows)
            {
                int i = Convert.ToInt32(reader[0]);
                refNumber = i.ToString();
            }

            closeConnection(CINTEGRATION);
            return refNumber;
        }

        public string getRecieptBatch(string bankcode)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            string batch = "";

            cmd.CommandText = "EXEC sp_getReceiptBatch @bankcode";
            cmd.Parameters.AddWithValue("@bankcode", bankcode);
            cmd.Connection = cIntegration;

            reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                batch = reader[0].ToString();
            }

            closeConnection(CINTEGRATION);
            return batch;
        }

        public List<string> checkInvoiceAvail(string invoiceId)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand("exec sp_GetInvoice @id", cIntegration);
            SqlDataReader reader;
            cmd.Parameters.AddWithValue("@id", invoiceId);
            List<string> data = new List<string>(3);

            reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                data.Add(reader[0].ToString());
                data.Add(reader[1].ToString());
                data.Add(reader[2].ToString());
            }
            else
            {
                data = null;
            }

            closeConnection(CINTEGRATION);
            return data;
        }

        public void storeInvoice(int invoiceId, int batchTarget, int CreditGL, string clientName, string clientId, DateTime date, string author, decimal amount, string state, decimal usrate, decimal usamount, int isvoid, int isCreditMemo, int creditMemoNumber)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand("exec sp_StoreInvoice @id, @target, @CreditGL, @clientName, @clientId, @dateCreated, @author, @amount, @state, @usrate, @usamount, @isvoid, @isCreditMemo, @credMemoNum", cIntegration);

            cmd.Parameters.AddWithValue("@id", invoiceId);
            cmd.Parameters.AddWithValue("@target", batchTarget);
            cmd.Parameters.AddWithValue("@CreditGL", CreditGL);
            cmd.Parameters.AddWithValue("@clientName", clientName);
            cmd.Parameters.AddWithValue("@clientId", clientId);
            cmd.Parameters.AddWithValue("@dateCreated", date);
            cmd.Parameters.AddWithValue("@author", author);
            cmd.Parameters.AddWithValue("@amount", amount);
            cmd.Parameters.AddWithValue("@state", state);
            cmd.Parameters.AddWithValue("@usrate", usrate);
            cmd.Parameters.AddWithValue("@usamount", usamount);
            cmd.Parameters.AddWithValue("@isvoid", isvoid);
            cmd.Parameters.AddWithValue("@isCreditMemo", isCreditMemo);
            cmd.Parameters.AddWithValue("@credMemoNum", creditMemoNumber);

            cmd.ExecuteNonQuery();
            closeConnection(CINTEGRATION);
        }

        public void storePayment(string clientId, string clientName, DateTime createdDate, string invoiceId, decimal amount, decimal usamount, string prepstat, int referenceNumber, int destinationBank, string isPayByCredit, decimal prepaymentUsRate)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand("exec sp_StorePayment @clientId, @clientName, @createdDate, @invoiceId, @amount, @usamount, @prepstat, @referenceNumber, @destinationBank, @isPayByCredit, @prepaymentUsRate", cIntegration);

            cmd.Parameters.AddWithValue("@clientId", clientId);
            cmd.Parameters.AddWithValue("@clientName", clientName);
            cmd.Parameters.AddWithValue("@createdDate", createdDate);
            cmd.Parameters.AddWithValue("@invoiceId", invoiceId);
            cmd.Parameters.AddWithValue("@amount", amount);
            cmd.Parameters.AddWithValue("@usamount", usamount);
            cmd.Parameters.AddWithValue("@prepstat", prepstat);
            cmd.Parameters.AddWithValue("@referenceNumber", referenceNumber);
            cmd.Parameters.AddWithValue("@destinationBank", destinationBank);
            cmd.Parameters.AddWithValue("@isPayByCredit", isPayByCredit);
            cmd.Parameters.AddWithValue("@prepaymentUsRate", prepaymentUsRate);
            cmd.ExecuteNonQuery();
            closeConnection(CINTEGRATION);
        }

        public string GetAccountNumber(int GLID)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand("exec sp_GetGLAcctNumber @GLID", cIntegration);
            SqlDataReader reader;

            string accountNumber = "";
            cmd.Parameters.AddWithValue("@GLID", GLID);

            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                accountNumber = reader[0].ToString();
            }


            closeConnection(CINTEGRATION);
            return accountNumber;
        }

        public int GetIsInvoiceCancelled(int invoiceid)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand("exec sp_GetIsInvoiceCancelled @invoiceid", cIntegration);
            SqlDataReader reader;
            int isvoided = 0;
            cmd.Parameters.AddWithValue("@invoiceid", invoiceid);


            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                isvoided = Convert.ToInt32(reader[0].ToString());
            }


            closeConnection(CINTEGRATION);
            return isvoided;
        }

        public List<string> GetInvoiceDetails(int invoiceId)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand("exec sp_GetInvoiceDetail @invoiceId", cIntegration);
            SqlDataReader reader;
            List<string> data = new List<string>(3);

            cmd.Parameters.AddWithValue("@invoiceId", invoiceId);


            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                data.Add(reader[0].ToString());
                data.Add(reader[1].ToString());
                data.Add(reader[2].ToString());
                data.Add(reader[3].ToString());
            }
            else
            {
                data = null;
            }

            closeConnection(CINTEGRATION);
            return data;
        }

        public void MarkAsTransferred(int invoiceId)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand("exec sp_UpdateInvoiceToTransferred @invoiceId", cIntegration);

            cmd.Parameters.AddWithValue("@invoiceId", invoiceId);
            cmd.ExecuteNonQuery();
            closeConnection(CINTEGRATION);
        }

        public int GetInvDetailOccurence(string invoiceId)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand("exec sp_InvoiceDetailOccurenceCount @invoiceId", cIntegration);
            SqlDataReader reader;
            cmd.Parameters.AddWithValue("@invoiceId", invoiceId);

            reader = cmd.ExecuteReader();
            int i = 0;

            if (reader.HasRows)
            {
                reader.Read();
                i = Convert.ToInt32(reader[0].ToString());
            }

            closeConnection(CINTEGRATION);
            return i;
        }

        public int GetInvoicePosted(int invoiceId)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand("exec sp_GetInvoicePosted @invoiceId", cIntegration);
            SqlDataReader reader;
            cmd.Parameters.AddWithValue("@invoiceId", invoiceId);

            reader = cmd.ExecuteReader();
            int batchid = 0;
            if (reader.HasRows)
            {
                reader.Read();
                batchid = Convert.ToInt32(reader[0].ToString());
            }


            closeConnection(CINTEGRATION);
            return batchid;
        }


        public int GetCreditGl(string invoiceiD)
        {

            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand("exec sp_GetCreditGL @invoiceId", cIntegration);
            SqlDataReader reader;
            cmd.Parameters.AddWithValue("@invoiceId", invoiceiD);

            reader = cmd.ExecuteReader();
            int i = 0;

            if (reader.HasRows)
            {
                reader.Read();
                i = Convert.ToInt32(reader[0].ToString());
            }

            closeConnection(CINTEGRATION);
            return i;
        }

        public int GetCreditGlID(string GLTransactionID)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand("exec sp_GetPrepaymentGlID @GLTransactionID", cIntegration);
            SqlDataReader reader;
            cmd.Parameters.AddWithValue("@GLTransactionID", GLTransactionID);


            reader = cmd.ExecuteReader();
            int i = 0;

            if (reader.HasRows)
            {
                reader.Read();
                i = Convert.ToInt32(reader[0].ToString());
            }

            closeConnection(CINTEGRATION);
            return i;
        }

        public string isAnnualFee(int invoiceid)
        {

            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand("exec sp_isAnnualFee @invoiceid", cIntegration);
            SqlDataReader reader;
            cmd.Parameters.AddWithValue("@invoiceid", invoiceid);

            reader = cmd.ExecuteReader();
            string notes = " ";

            if (reader.HasRows)
            {
                reader.Read();
                notes = reader[0].ToString();
            }

            closeConnection(CINTEGRATION);
            return notes;
        }


        public void UpdateCreditGl(int invoiceId, int newCreditGl)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand("exec sp_UpdateCreditGl @invoiceId, @newCreditGl", cIntegration);
            cmd.Parameters.AddWithValue("@invoiceId", invoiceId);
            cmd.Parameters.AddWithValue("@newCreditGl", newCreditGl);

            cmd.ExecuteNonQuery();
            closeConnection(CINTEGRATION);
        }

        public void modifyInvoiceList(int invoiceId, decimal rate, string customerId)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand("exec sp_UpdateInvoice @invoiceid, @usrate, @customerId", cIntegration);
            cmd.Parameters.AddWithValue("@invoiceId", invoiceId);
            cmd.Parameters.AddWithValue("@usrate", rate);
            cmd.Parameters.AddWithValue("@customerId", customerId);

            cmd.ExecuteNonQuery();
            closeConnection(CINTEGRATION);
        }
        public void UpdateEntryNumber(int invoiceId)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand("exec sp_UpdateEntry @invoiceId", cIntegration);
            cmd.Parameters.AddWithValue("@invoiceId", invoiceId);

            cmd.ExecuteNonQuery();
            closeConnection(CINTEGRATION);
        }

        public List<string> GetPaymentInfo(int gl_id)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd_pay = new SqlCommand();
            SqlDataReader reader_pay;

            List<string> data = new List<string>(4);
            cmd_pay.Connection = cIntegration;
            cmd_pay.CommandText = "EXEC sp_GetPayInfo @id";
            cmd_pay.Parameters.AddWithValue("@id", gl_id);

            reader_pay = cmd_pay.ExecuteReader();
            if (reader_pay.HasRows)
            {
                reader_pay.Read();

                var debit = reader_pay[0].ToString();
                var glid = reader_pay[1].ToString();
                var invoiceId = reader_pay[2].ToString();
                var paymentDate = reader_pay[3].ToString();

                data.Add(debit);
                data.Add(glid);
                data.Add(invoiceId);
                data.Add(paymentDate);
            }
            reader_pay.Close();
            closeConnection(CINTEGRATION);
            return data;
        }

        public List<string> getClientInfo_inv(string id)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            List<string> data = new List<string>(4);

            cmd.Connection = cIntegration;
            cmd.CommandText = "EXEC sp_GetClientInfo @id";
            cmd.Parameters.AddWithValue("@id", id);

            reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                var companyName = reader[0].ToString();
                var ccNum = reader[1].ToString();
                var clientFname = reader[2].ToString();
                var clientLname = reader[3].ToString();

                data.Add(companyName);
                data.Add(ccNum);
                data.Add(clientFname);
                data.Add(clientLname);
            }


            closeConnection(CINTEGRATION);
            return data;
        }

        public List<string> GetClientInfo_Pay(string id)
        {
            openConnection(CGENERIC);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            List<string> data = new List<string>(4);

            cmd.Connection = cGeneric;
            cmd.CommandText = "SELECT clientCompany, ccNum from client where clientId=@id";
            cmd.Parameters.AddWithValue("@id", id);


            reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                var companyName = reader[0].ToString();
                var ccNum = reader[1].ToString();
                var clientFname = reader[2].ToString();
                var clientLname = reader[3].ToString();

                data.Add(companyName);
                data.Add(ccNum);
                data.Add(clientFname);
                data.Add(clientLname);
            }

            closeConnection(CGENERIC);
            return data;
        }

        public List<string> GetInvoiceInfo(string pInvoice)
        {
            openConnection(CGENERIC);
            SqlCommand cmdAmt = new SqlCommand();
            SqlDataReader readerAmt;
            List<string> data = new List<string>(2);

            cmdAmt.Connection = cGeneric;
            cmdAmt.CommandText = "select Amount, Author from tblArInvoices where ArInvoiceId=@arInv";
            cmdAmt.Parameters.AddWithValue("@arInv", pInvoice);


            readerAmt = cmdAmt.ExecuteReader();
            if (readerAmt.HasRows)
            {
                readerAmt.Read();
                data.Add(readerAmt[0].ToString());
                data.Add(readerAmt[1].ToString());
            }

            closeConnection(CGENERIC);
            return data;
        }

        public List<string> GetFeeInfo(int invoiceId)
        {
            openConnection(CGENERIC);
            SqlCommand cmd_inv = new SqlCommand();
            SqlDataReader reader_inv;
            List<string> data = new List<string>(2);

            cmd_inv.Connection = cGeneric;
            cmd_inv.CommandText = "Select FeeType, notes from tblARInvoices where ARInvoiceID=@id_inv";
            cmd_inv.Parameters.AddWithValue("@id_inv", invoiceId);

            reader_inv = cmd_inv.ExecuteReader();

            if (reader_inv.HasRows)
            {
                reader_inv.Read();
                var ftype = reader_inv[0].ToString();
                var notes = reader_inv[1].ToString();

                data.Add(ftype);
                data.Add(notes);
            }

            closeConnection(CGENERIC);
            return data;
        }

        public bool GetInvoiceExists(int invoiceId)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd_inv = new SqlCommand();
            SqlDataReader reader_inv;

            cmd_inv.Connection = cIntegration;
            cmd_inv.CommandText = "Exec sp_GetInvoiceExists @invoiceid";
            cmd_inv.Parameters.AddWithValue("@invoiceid", invoiceId);


            reader_inv = cmd_inv.ExecuteReader();
            bool ans;
            if (reader_inv.HasRows)
            {
                ans = true;
            }
            else
            {
                ans = false;
            }


            closeConnection(CINTEGRATION);
            return ans;
        }

        public void UpdateCustomerCount()
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd_inv = new SqlCommand();

            cmd_inv.CommandText = "exec sp_UpdateCustomerCount";
            cmd_inv.Connection = cIntegration;

            cmd_inv.ExecuteNonQuery();
            closeConnection(CINTEGRATION);
        }

        public void StoreCustomer(string clientId, string clientName)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = "exec sp_StoreCreatedCustomer @clientId, @clientName";
            cmd.Parameters.AddWithValue("@clientId", clientId);
            cmd.Parameters.AddWithValue("@clientName", clientName);
            cmd.Connection = cIntegration;


            cmd.ExecuteNonQuery();
            closeConnection(CINTEGRATION);
        }

        public List<Queue> ReadMessageQueue()
        {
            openConnection(CMSGQUEUE);
            SqlCommand cmd_inv = new SqlCommand();
            SqlDataReader reader_inv;
            List<Queue> data = new List<Queue>();
            Queue q;

            cmd_inv.Connection = cMsgQueue;
            cmd_inv.CommandText = "EXEC sp_ReadQueue";

            reader_inv = cmd_inv.ExecuteReader();

            if (reader_inv.HasRows)
            {
                reader_inv.Read();
                q = new Queue();
                q.date = Convert.ToDateTime(reader_inv[0].ToString());
                q.msg = reader_inv[1].ToString();

                data.Add(q);
                data.Add(q);
            }

            closeConnection(CMSGQUEUE);
            return data;
        }

        public void UpdateReceiptNumber(int transactionId, string referenceNumber)
        {
            openConnection(CGENERIC);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = "exec sp_UpdateReceipt @receiptNum, @reference";
            cmd.Parameters.AddWithValue("@receiptNum", transactionId.ToString());
            cmd.Parameters.AddWithValue("@reference", referenceNumber);
            cmd.Connection = cGeneric;

            cmd.ExecuteNonQuery();
            closeConnection(CGENERIC);
        }

        public void Log(string msg)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd_inv = new SqlCommand();

            cmd_inv.CommandText = "exec sp_Log @msg";
            cmd_inv.Parameters.AddWithValue("@msg", msg);
            cmd_inv.Connection = cIntegration;
            cmd_inv.ExecuteNonQuery();
            closeConnection(CINTEGRATION);

        }

        public DateTime GetValidity(int invoiceId)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;

            var datetime = DateTime.Now;
            DateTime startdate = DateTime.Now;

            cmd.CommandText = "exec sp_GetValidity @invoiceId";
            cmd.Parameters.AddWithValue("@invoiceId", invoiceId);
            cmd.Connection = cIntegration;
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                datetime = Convert.ToDateTime(reader[6]);
            }


            closeConnection(CINTEGRATION);
            return datetime;
        }

        public DateTime GetValidityEnd(int invoiceId)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;

            var datetime = DateTime.Now;
            DateTime startdate = DateTime.Now;
            cmd.CommandText = "exec sp_GetValidity @invoiceId";
            cmd.Parameters.AddWithValue("@invoiceId", invoiceId);
            cmd.Connection = cIntegration;


            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                datetime = Convert.ToDateTime(reader[7]);
            }

            closeConnection(CINTEGRATION);
            return datetime;
        }

        public void resetInvoiceTotal()
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd_inv = new SqlCommand();

            cmd_inv.CommandText = "exec resetInvoiceTotal";
            cmd_inv.Connection = cIntegration;

            cmd_inv.ExecuteNonQuery();
            closeConnection(CINTEGRATION);
        }

        public string getFreqUsage(int invoiceId)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;

            string result = "";
            cmd.CommandText = "exec sp_freqUsage @invoiceId";
            cmd.Parameters.AddWithValue("@invoiceId", invoiceId);
            cmd.Connection = cIntegration;


            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                result = reader[0].ToString();
            }

            closeConnection(CINTEGRATION);
            return result;
        }

        public int getCreditMemoNumber()
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;
            int num = -1;

            cmd.CommandText = "exec sp_getCMemoSeq";
            cmd.Connection = cIntegration;


            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                num = Convert.ToInt32(reader[0]);
            }

            closeConnection(CINTEGRATION);
            return num;
        }

        public void updateAsmsCreditMemoNumber(int docId, int newCredNum)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = "exec sp_UpdateCreditMemoNum @documentId, @newCredNum";
            cmd.Parameters.AddWithValue("@documentId", docId);
            cmd.Parameters.AddWithValue("@newCredNum", newCredNum);
            cmd.Connection = cIntegration;


            cmd.ExecuteNonQuery();
            closeConnection(CINTEGRATION);

        }

        public InvoiceInfo getInvoiceDetails(int invoiceId)
        {
            openConnection(CINTEGRATION);

            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;
            InvoiceInfo inv = new InvoiceInfo();

            cmd.CommandText = "exec sp_getInvoiceInfo @invoiceId";
            cmd.Parameters.AddWithValue("@invoiceId", invoiceId);
            cmd.Connection = cIntegration;
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                inv.CustomerId = Convert.ToInt32(reader["CustomerId"]);
                inv.FeeType = reader["FeeType"].ToString();
                inv.notes = reader["notes"].ToString();
                inv.amount = Convert.ToDecimal(reader["Amount"]);
                inv.isvoided = Convert.ToInt32(reader["isvoided"]);
                inv.Glid = Convert.ToInt32(reader["Glid"]);
                inv.FreqUsage = reader["FreqUsage"].ToString();
                inv.Author = reader["Author"].ToString();

            }

            closeConnection(CINTEGRATION);
            return inv;
        }

        public PaymentInfo getPaymentInfo(int originalDocNum)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;
            PaymentInfo rct = new PaymentInfo();

            cmd.CommandText = "exec sp_getReceiptInfo @originalDocNum";
            cmd.Parameters.AddWithValue("@originalDocNum", originalDocNum);
            cmd.Connection = cIntegration;

            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                rct.ReceiptNumber = Convert.ToInt32(reader["ReceiptNumber"]);
                rct.GLTransactionID = Convert.ToInt32(reader["GLTransactionID"]);
                rct.CustomerID = Convert.ToInt32(reader["CustomerID"]);
                rct.Debit = Convert.ToDecimal(reader["Debit"]);
                rct.InvoiceID = Convert.ToInt32(reader["InvoiceId"]);
                rct.Date1 = Convert.ToDateTime(reader["Date1"]);
                rct.GLID = Convert.ToInt32(reader["GLID"]);

            }
            closeConnection(CINTEGRATION);
            return rct;
        }

        public CreditNoteInfo getCreditNoteInfo(int creditMemoNum, int documentId)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;
            CreditNoteInfo creditNote = new CreditNoteInfo();

            cmd.CommandText = "exec sp_getCreditMemoInfo @creditNoteNum, @documentId";
            cmd.Parameters.AddWithValue("@creditNoteNum", creditMemoNum);
            cmd.Parameters.AddWithValue("@documentId", documentId);
            cmd.Connection = cIntegration;
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                creditNote.ARInvoiceID = Convert.ToInt32(reader["ARInvoiceID"]);
                creditNote.CreditGL = Convert.ToInt32(reader["CreditGl"]);
                creditNote.amount = Convert.ToDecimal(reader["Amount"]);
                creditNote.CustomerID = Convert.ToInt32(reader["CustomerID"]);
                creditNote.FeeType = reader["FeeType"].ToString();
                creditNote.notes = reader["notes"].ToString();
                creditNote.remarks = reader["Remarks"].ToString();
            }

            closeConnection(CINTEGRATION);
            return creditNote;
        }

        public void updateAsmsCreditMNum(int currentNum, int newNum)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = "exec sp_UpdateCreditMemoNum @currentNum, @newNum";
            cmd.Parameters.AddWithValue("@currentNum", currentNum);
            cmd.Parameters.AddWithValue("@newNum", newNum);
            cmd.Connection = cIntegration;

            cmd.ExecuteNonQuery();
            closeConnection(CINTEGRATION);
        }

        public string getClientIdZRecord(bool stripExtention)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;
            string result = "";
            string temp = "";

            cmd.CommandText = "exec sp_getZeroRecord";
            cmd.Connection = cIntegration;
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                result = reader["clientId"].ToString();

                if (stripExtention)
                {
                    for (int i = 0; i < result.Length; i++)
                    {
                        if (result[i] != '-')
                        {
                            temp += result[i];
                        }
                        else
                        {
                            i = result.Length;
                        }
                    }
                    result = temp;
                }
                else
                {
                    return result;
                }
            }


            closeConnection(CINTEGRATION);
            return result;
        }

        public void checkResetCounters(int mExpiry, int dExpiry)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;

            DateTime monthlyExpiry = DateTime.Now;
            DateTime dailyExpiry = DateTime.Now;

            cmd.CommandText = "exec getCountersExpiry";
            cmd.Connection = cIntegration;

            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                monthlyExpiry = Convert.ToDateTime(reader["monthlyReset"]);
                dailyExpiry = Convert.ToDateTime(reader["dailyReset"]);
            }

            closeConnection(CINTEGRATION);

            if (monthlyExpiry.Day == DateTime.Now.Day && monthlyExpiry.Month == DateTime.Now.Month && monthlyExpiry.Year == DateTime.Now.Year)
            {
                resetMonthlyCounters(mExpiry);
            }

            if (dailyExpiry.Day == DateTime.Now.Day && dailyExpiry.Month == DateTime.Now.Month && dailyExpiry.Year == DateTime.Now.Year)
            {
                resetDailyCounters(dExpiry);
            }
        }

        public void resetMonthlyCounters(int daysToNExpiry)
        {

            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = "exec resetMonthlyCounters @nextExpiry";
            cmd.Parameters.AddWithValue("@nextExpiry", DateTime.Now.AddDays(daysToNExpiry));
            cmd.Connection = cIntegration;

            cmd.ExecuteNonQuery();
            closeConnection(CINTEGRATION);
        }

        public void resetDailyCounters(int daysToNExpiry)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();


            cmd.CommandText = "exec resetDailyCounters @nextExpiry";
            cmd.Parameters.AddWithValue("@nextExpiry", DateTime.Now.AddDays(daysToNExpiry));
            cmd.Connection = cIntegration;

            cmd.ExecuteNonQuery();
            closeConnection(CINTEGRATION);
        }

        public PrepaymentData checkPrepaymentAvail(string customerId)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;
            PrepaymentData data = new PrepaymentData();

            cmd.CommandText = "exec sp_getCustomerPrepayment @customerId";
            cmd.Parameters.AddWithValue("@customerId", customerId);
            cmd.Connection = cIntegration;
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                data.dataAvail = true;
                data.originalAmount = Convert.ToDecimal(reader["amount"]);
                data.remainder = Convert.ToDecimal(reader["prepaymentRemainder"]);
                data.totalPrepaymentRemainder = Convert.ToDecimal(reader["TotalPrepaymentRemainder"]);
                data.referenceNumber = reader["referenceNumber"].ToString();
                data.sequenceNumber = Convert.ToInt32(reader["sequence"]);
                data.destinationBank = Convert.ToInt32(reader["destinationBank"]);
            }
            else
            {
                data.dataAvail = false;
            }

            closeConnection(CINTEGRATION);
            return data;
        }

        public void adjustPrepaymentRemainder(decimal amount, int sequenceNumber)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = "exec sp_adjustPrepaymentRemainder @amount, @sequenceNumber";
            cmd.Parameters.AddWithValue("@amount", amount);
            cmd.Parameters.AddWithValue("@sequenceNumber", sequenceNumber);
            cmd.Connection = cIntegration;

            cmd.ExecuteNonQuery();
            closeConnection(CINTEGRATION);
        }

        public decimal getTotalPrepaymentRemainder(string customerId)
        {
            decimal result = 0;
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "exec sp_getTotalPrepaymentRemainder @customerId";
            cmd.Parameters.AddWithValue("@customerId", customerId);
            cmd.Connection = cIntegration;

            cmd.ExecuteNonQuery();
            closeConnection(CINTEGRATION);
            return result;
        }

        public decimal getPrepaymentURate(int sequence)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;
            decimal urate = 0;

            cmd.CommandText = "exec sp_getPrepRate @sequence";
            cmd.Parameters.AddWithValue("@sequence", sequence);
            cmd.Connection = cIntegration;

            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                urate = Convert.ToDecimal(reader["usrate"]);
            }

            closeConnection(CINTEGRATION);
            return urate;
        }


        public string generateReportId(string ReportType)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader = null;
            cmd.CommandText = "exec sp_DIRnewReportID @ReportType";
            cmd.Parameters.AddWithValue("@ReportType", ReportType);
            cmd.Connection = cIntegration;
            string result = "";

            reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                result = reader[0].ToString();
            }

            closeConnection(CINTEGRATION);
            return result;
        }

        private void dataRouter(string ReportType, DataWrapper data, string recordID, int destination)
        {
            openConnection(CINTEGRATION);
            for (int i = 0; i < data.records.Count; i++)
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "exec sp_rptRecInsert @ReportType, @reportId, @licenseNumber, @clientCompany, @invoiceID, @budget, @invoiceTotal, @thisPeriodsInv, @balBFwd, @fromRev, @toRev, @closingBal, @totalMonths, @monthUtil, @monthRemain,  @valPStart, @valPEnd, @destination";
                cmd.Connection = cIntegration;

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
                cmd.ExecuteNonQuery();
            }

            closeConnection(CINTEGRATION);
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
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "exec sp_insertSubtotals @ReportType, @reportId, @category, @invoiceTotal, @balanceBFwd, @toRev, @closingBal, @fromRev, @budget";
            cmd.Connection = cIntegration;

            cmd.Parameters.AddWithValue("@ReportType", ReportType);
            cmd.Parameters.AddWithValue("@reportId", reportID);
            cmd.Parameters.AddWithValue("@category", destination);
            cmd.Parameters.AddWithValue("@invoiceTotal", data.subT_invoiceTotal);
            cmd.Parameters.AddWithValue("@balanceBFwd", data.subT_balBFwd);
            cmd.Parameters.AddWithValue("@toRev", data.subT_toRev);
            cmd.Parameters.AddWithValue("@closingBal", data.subT_closingBal);
            cmd.Parameters.AddWithValue("@fromRev", data.subT_fromRev);
            cmd.Parameters.AddWithValue("@budget", data.subT_budget);
            
            cmd.ExecuteNonQuery();
            closeConnection(CINTEGRATION);
        }


        public void insertTotals(string ReportType, string reportID, Totals total)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "exec sp_insertTotals @ReportType, @recordID, @invoiceTotal, @balanceBFwd, @toRev, @closingBal, @fromRev, @budget";
            cmd.Connection = cIntegration;

            cmd.Parameters.AddWithValue("@ReportType", ReportType);
            cmd.Parameters.AddWithValue("@recordID", reportID);
            cmd.Parameters.AddWithValue("@invoiceTotal", total.tot_invoiceTotal);
            cmd.Parameters.AddWithValue("@balanceBFwd", total.tot_balBFwd);
            cmd.Parameters.AddWithValue("@toRev", total.tot_toRev);
            cmd.Parameters.AddWithValue("@closingBal", total.tot_closingBal);
            cmd.Parameters.AddWithValue("@fromRev", total.tot_fromRev);
            cmd.Parameters.AddWithValue("@budget", total.tot_budget);

            cmd.ExecuteNonQuery();
            closeConnection(CINTEGRATION);
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
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;

            List<UIData> udt = new List<UIData>();
            cmd.CommandText = "EXEC sp_getDeferredPartial @ReportType, @index, @report_id";
            cmd.Parameters.AddWithValue("@ReportType", ReportType);
            cmd.Parameters.AddWithValue("@index", index);
            cmd.Parameters.AddWithValue("@report_id", report_id);
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

                closeConnection(CINTEGRATION);
                return udt;
            }
            else
            {
                closeConnection(CINTEGRATION);
                return udt;
            }
        }

        private SubTotals getDeferredPartialSubs(string ReportType, int index, string report_id)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;

            SubTotals subs = new SubTotals();
            cmd.CommandText = "EXEC sp_getDeferredPartialSubs @ReportType, @index, @record_id";
            cmd.Parameters.AddWithValue("@ReportType", ReportType);
            cmd.Parameters.AddWithValue("@index", index);
            cmd.Parameters.AddWithValue("@record_id", report_id);
            cmd.Connection = cIntegration;

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

                closeConnection(CINTEGRATION);
                return subs;
            }
            else
            {
                closeConnection(CINTEGRATION);
                return subs;
            }
        }


        public Totals getDeferredTotal(string ReportType, string recordID)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            Totals totals = new Totals();
            
            cmd.CommandText = "EXEC sp_getDeferredRptTotals @ReportType, @record_id";
            cmd.Parameters.AddWithValue("@ReportType", ReportType);
            cmd.Parameters.AddWithValue("@record_id", recordID);
            cmd.Connection = cIntegration;
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

                closeConnection(CINTEGRATION);
                return totals;
            }
            else
            {
                closeConnection(CINTEGRATION);
                return totals;
            }
        }

        public void SetNextGenDate(string ReportType, DateTime date)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "exec sp_setNewRptDate @ReportType, @date";
            cmd.Connection = cIntegration;

            cmd.Parameters.AddWithValue("@ReportType", ReportType);
            cmd.Parameters.AddWithValue("@date", date);
            cmd.ExecuteNonQuery();
            closeConnection(CINTEGRATION);
        }

        public DateTime GetNextGenDate(string ReportType)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            DateTime date = DateTime.Now;

            cmd.Connection = cIntegration;
            cmd.CommandText = "EXEC sp_getNextRptDate @ReportType";

            cmd.Parameters.AddWithValue("@ReportType", ReportType);
            reader = cmd.ExecuteReader();

            reader.Read();
            date = Convert.ToDateTime(reader["date"]);

            closeConnection(CINTEGRATION);
            return date;
        }

        public void SetIntegrationStat(int stat)
        {
            System.Threading.Thread.Sleep(1000);
            openConnection(CMSGQUEUE);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = cMsgQueue;
            cmd.CommandText = "sp_UpdateStat @stat";
            cmd.Parameters.AddWithValue("@stat", stat);
            cmd.ExecuteNonQuery();
            closeConnection(CMSGQUEUE);
        }

        public bool checkReportExist(DateTime date)
        {
            openConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;

            cmd.Connection = cIntegration;
            cmd.CommandText = "EXEC sp_checkRptExist @period";
            cmd.Parameters.AddWithValue("@period", date);
       
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                closeConnection(CINTEGRATION);
                return true;
            }
            else
            {
                closeConnection(CINTEGRATION);
                return false;
            }
        }
    }
}
