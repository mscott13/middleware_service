using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

        public void OpenConnection(string target)
        {
            

            switch (target)
            {
                case "cIntegration":
                    cIntegration = new SqlConnection(Constants.DBINTEGRATION);
                    cIntegration.Open();
                    break;
                case "cGeneric":
                    cGeneric = new SqlConnection(Constants.DBGENERIC);
                    cGeneric.Open();
                    break;
                case "g":
                    cMsgQueue = new SqlConnection(Constants.DBINTEGRATION);
                    cMsgQueue.Open();
                    break;
            }
        }

        public void CloseConnection(string target)
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

        public List<ReportRawData> GetDIRInformation(string ReportType, DateTime searchStartDate, DateTime searchEndDate)
        {
            OpenConnection(CINTEGRATION);
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

                CloseConnection(CINTEGRATION);
                return reportInfo;
            }
            else
            {
                CloseConnection(CINTEGRATION);
                return reportInfo;
            }
        }

        public List<Batch> GetExpiryBatchDate()
        {
            OpenConnection(CINTEGRATION);
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


            CloseConnection(CINTEGRATION);
            return lstInvoiceBatchDataRet;
        }

        public void CloseInvoiceBatch(int batchId)
        {
            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "EXEC closeBatch @batchId";
            cmd.Parameters.AddWithValue("@batchId", batchId);
            cmd.Connection = cIntegration;

            cmd.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);
        }

        public Maj GetMajDetail(int referenceNumber)
        {
            OpenConnection(CGENERIC);
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

            CloseConnection(CGENERIC);
            return mj;
        }

        public DataSet GetRenewalInvoiceValidity(int invoiceid)
        {
            OpenConnection(CGENERIC);

            SqlCommand cmd = new SqlCommand("EXEC sp_getValidityRenewalInvoice", cGeneric);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@invoiceid", invoiceid);

            SqlDataAdapter da = new SqlDataAdapter(cmd);

            DataSet ds = new DataSet();
            da.Fill(ds);
            da.Dispose();
            CloseConnection(CGENERIC);
            return ds;


        }

        public DataSet GetRenewalInvoiceValidity1(int invoiceid)
        {
            OpenConnection(CGENERIC);

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cGeneric;
            cmd.CommandText = "EXEC sp_getValidityRenewalInvoice @invoiceid";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@invoiceid", invoiceid);

            SqlDataAdapter da = new SqlDataAdapter(cmd);

            DataSet ds = new DataSet();
            da.Fill(ds);
            da.Dispose();
            CloseConnection(CGENERIC);
            return ds;

        }
        public int GetInvoiceReference(int invoiceId)
        {
            OpenConnection(CGENERIC);

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

            CloseConnection(CGENERIC);
            return refNumber;
        }

        public void CreateInvoiceBatch(double daysTillEx, int batchId, string batchType, string renstat)
        {
            OpenConnection(CINTEGRATION);

            SqlCommand cmd = new SqlCommand();
            var expiryDate = DateTime.Now.AddDays(daysTillEx);

            cmd.CommandText = "EXEC createBatch @batchId, @expirydate, @batchType, @renstat";
            cmd.Connection = cIntegration;
            cmd.Parameters.AddWithValue("@batchId", batchId);
            cmd.Parameters.AddWithValue("@expirydate", expiryDate);
            cmd.Parameters.AddWithValue("@batchType", batchType);
            cmd.Parameters.AddWithValue("@renstat", renstat);

            cmd.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);
        }

        public bool BatchAvail(string batchType)
        {
            OpenConnection(CINTEGRATION);
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

            CloseConnection(CINTEGRATION);

            if (result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsBatchExpired(int batchId)
        {
            OpenConnection(CINTEGRATION);
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

            CloseConnection(CINTEGRATION);

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

        public int GetAvailBatch(string batchType)
        {
            OpenConnection(CINTEGRATION);

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


            CloseConnection(CINTEGRATION);
            return result;
        }

        public List<Batch> GetExpiryBatchDate_Payment()
        {
            OpenConnection(CINTEGRATION);
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


            CloseConnection(CINTEGRATION);
            return lstInvoiceBatchData;
        }

        public void OpenNewBatchSet(double DaysTillExpired, int LastBatchId)
        {
            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();

            var CreatedDate = DateTime.Now;
            var ExpiryDate = DateTime.Now.AddDays(DaysTillExpired);

            cmd.Connection = cIntegration;
            cmd.CommandText = "Exec sp_NewBatchSet @FirstBatchId, @CreatedDate, @ExpiryDate";
            cmd.Parameters.AddWithValue("@FirstBatchId", LastBatchId + 1);
            cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
            cmd.Parameters.AddWithValue("@ExpiryDate", ExpiryDate);

            cmd.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);

        }

        public void OpenNewReceiptBatch(double DaysTillExpired, int LastBatchId, string bankcode)
        {

            OpenConnection(CINTEGRATION);
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
            CloseConnection(CINTEGRATION);
        }

        public void CloseReceiptBatch(int batchId)
        {

            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = cIntegration;
            cmd.CommandText = "EXEC closeReceiptBatch @batchId";
            cmd.Parameters.AddWithValue("@batchId", batchId);

            cmd.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);

        }

        public void UpdateBatchAmount(string batchType, decimal amount)
        {

            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = cIntegration;
            cmd.CommandText = "EXEC updateBatchAmount @batchType, @amount";
            cmd.Parameters.AddWithValue("@batchType", batchType);
            cmd.Parameters.AddWithValue("@amount", amount);

            cmd.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);

        }

        public string GetBankCodeId(string bankcode)
        {
            OpenConnection(CINTEGRATION);
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

            CloseConnection(CINTEGRATION);
            return bankcodeid;
        }

        public DateTime GetDocDate(int docNumber)
        {
            OpenConnection(CINTEGRATION);
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


            CloseConnection(CINTEGRATION);
            return date;
        }

        public void CloseOldBatchSet()
        {
            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = cIntegration;
            cmd.CommandText = "Exec sp_CloseBatch";


            cmd.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);
        }

        public void CloseOldBatchSet_payment()
        {
            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = cIntegration;
            cmd.CommandText = "Exec sp_CloseBatch_Payment";
            cmd.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);
        }

        public int GetLastBatchId()
        {
            OpenConnection(CINTEGRATION);
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


            CloseConnection(CINTEGRATION);
            return BatchId;
        }

        public int GetLastBatchId_payment()
        {
            OpenConnection(CINTEGRATION);
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


            CloseConnection(CINTEGRATION);
            return BatchId;
        }

        public void Init(int LastBatchId, double DaysTillExpired)
        {

            OpenConnection(CINTEGRATION);
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
            CloseConnection(CINTEGRATION);
        }

        public void UpdateReference(string Bank, string reference)
        {
            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = cIntegration;
            cmd.CommandText = "Exec sp_UpdateReference @Bank, @reference";
            cmd.Parameters.AddWithValue("@Bank", Bank);
            cmd.Parameters.AddWithValue("@reference", reference);


            cmd.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);
        }

        public int GetReferenceCount(string bank)
        {
            OpenConnection(CINTEGRATION);
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

            CloseConnection(CINTEGRATION);
            return count;
        }

        public void InitPayment(int LastBatchId, double DaysTillExpired)
        {
            OpenConnection(CINTEGRATION);
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
            CloseConnection(CINTEGRATION);
        }

        public bool IsInitialized()
        {
            OpenConnection(CINTEGRATION);
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

            CloseConnection(CINTEGRATION);
            return truth;
        }

        public bool IsInitializedPayment()
        {
            OpenConnection(CINTEGRATION);
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

            CloseConnection(CINTEGRATION);
            return truth;
        }

        public void UpdateBatchCount(string BatchType)
        {
            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = cIntegration;
            cmd.CommandText = "Exec UpdateBatchCount @BatchType";
            cmd.Parameters.AddWithValue("@BatchType", BatchType);


            cmd.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);
        }

        public void IncrementReferenceNumber(string BankCode, decimal amount)
        {
            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = cIntegration;
            cmd.CommandText = "Exec sp_IncrementRefNumber @BankCode, @amount";
            cmd.Parameters.AddWithValue("@BankCode", BankCode);
            cmd.Parameters.AddWithValue("@amount", amount);

            cmd.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);
        }

        public decimal GetRate()
        {
            OpenConnection(CINTEGRATION);
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

            CloseConnection(CINTEGRATION);
            return result;
        }

        public void UpdateBatchCountPayment(string BatchId)
        {
            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = cIntegration;
            cmd.CommandText = "Exec sp_IncrementEntryCount_payment @BatchId";
            cmd.Parameters.AddWithValue("@BatchId", BatchId);


            cmd.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);
        }

        public string GetInitialRef(string BankCodeId)
        {
            OpenConnection(CINTEGRATION);
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

            CloseConnection(CINTEGRATION);
            return refNumber;
        }

        public decimal GetUsRateByInvoice(int invoiceid)
        {
            OpenConnection(CINTEGRATION);
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


            CloseConnection(CINTEGRATION);
            return rate;
        }

        public string GetCurrentRef(string BankCode)
        {
            OpenConnection(CINTEGRATION);
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

            CloseConnection(CINTEGRATION);
            return refNumber;
        }

        public string GetRecieptBatch(string bankcode)
        {
            OpenConnection(CINTEGRATION);
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

            CloseConnection(CINTEGRATION);
            return batch;
        }

        public List<string> CheckInvoiceAvail(string invoiceId)
        {
            OpenConnection(CINTEGRATION);
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

            CloseConnection(CINTEGRATION);
            return data;
        }

        public void StoreInvoice(int invoiceId, int batchTarget, int CreditGL, string clientName, string clientId, DateTime date, string author, decimal amount, string state, decimal usrate, decimal usamount, int isvoid, int isCreditMemo, int creditMemoNumber)
        {
            OpenConnection(CINTEGRATION);
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
            CloseConnection(CINTEGRATION);
        }

        public void StorePayment(string clientId, string clientName, DateTime createdDate, string invoiceId, decimal amount, decimal usamount, string prepstat, int referenceNumber, int destinationBank, string isPayByCredit, decimal prepaymentUsRate)
        {
            OpenConnection(CINTEGRATION);
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
            CloseConnection(CINTEGRATION);
        }

        public string GetAccountNumber(int GLID)
        {
            OpenConnection(CINTEGRATION);
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


            CloseConnection(CINTEGRATION);
            return accountNumber;
        }

        public int GetIsInvoiceCancelled(int invoiceid)
        {
            OpenConnection(CINTEGRATION);
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


            CloseConnection(CINTEGRATION);
            return isvoided;
        }

        public List<string> GetInvoiceDetails(int invoiceId)
        {
            OpenConnection(CINTEGRATION);
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

            CloseConnection(CINTEGRATION);
            return data;
        }

        public void MarkAsTransferred(int invoiceId)
        {
            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand("exec sp_UpdateInvoiceToTransferred @invoiceId", cIntegration);

            cmd.Parameters.AddWithValue("@invoiceId", invoiceId);
            cmd.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);
        }

        public int GetInvDetailOccurence(string invoiceId)
        {
            OpenConnection(CINTEGRATION);
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

            CloseConnection(CINTEGRATION);
            return i;
        }

        public int GetInvoicePosted(int invoiceId)
        {
            OpenConnection(CINTEGRATION);
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


            CloseConnection(CINTEGRATION);
            return batchid;
        }

        public int GetCreditGl(string invoiceiD)
        {

            OpenConnection(CINTEGRATION);
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

            CloseConnection(CINTEGRATION);
            return i;
        }

        public int GetCreditGlID(string GLTransactionID)
        {
            OpenConnection(CINTEGRATION);
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

            CloseConnection(CINTEGRATION);
            return i;
        }

        public string IsAnnualFee(int invoiceid)
        {

            OpenConnection(CINTEGRATION);
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

            CloseConnection(CINTEGRATION);
            return notes;
        }


        public void UpdateCreditGl(int invoiceId, int newCreditGl)
        {
            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand("exec sp_UpdateCreditGl @invoiceId, @newCreditGl", cIntegration);
            cmd.Parameters.AddWithValue("@invoiceId", invoiceId);
            cmd.Parameters.AddWithValue("@newCreditGl", newCreditGl);

            cmd.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);
        }

        public void ModifyInvoiceList(int invoiceId, decimal rate, string customerId)
        {
            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand("exec sp_UpdateInvoice @invoiceid, @usrate, @customerId", cIntegration);
            cmd.Parameters.AddWithValue("@invoiceId", invoiceId);
            cmd.Parameters.AddWithValue("@usrate", rate);
            cmd.Parameters.AddWithValue("@customerId", customerId);

            cmd.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);
        }
        public void UpdateEntryNumber(int invoiceId)
        {
            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand("exec sp_UpdateEntry @invoiceId", cIntegration);
            cmd.Parameters.AddWithValue("@invoiceId", invoiceId);

            cmd.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);
        }

        public List<string> GetPaymentInfo(int gl_id)
        {
            OpenConnection(CINTEGRATION);
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
            CloseConnection(CINTEGRATION);
            return data;
        }

        public List<string> GetClientInfoInv(string id)
        {
            OpenConnection(CINTEGRATION);
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


            CloseConnection(CINTEGRATION);
            return data;
        }

        public List<string> GetClientInfoPay(string id)
        {
            OpenConnection(CGENERIC);
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

            CloseConnection(CGENERIC);
            return data;
        }

        public List<string> GetInvoiceInfo(string pInvoice)
        {
            OpenConnection(CGENERIC);
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

            CloseConnection(CGENERIC);
            return data;
        }

        public List<string> GetFeeInfo(int invoiceId)
        {
            OpenConnection(CGENERIC);
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

            CloseConnection(CGENERIC);
            return data;
        }

        public bool GetInvoiceExists(int invoiceId)
        {
            OpenConnection(CINTEGRATION);
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


            CloseConnection(CINTEGRATION);
            return ans;
        }

        public void UpdateCustomerCount()
        {
            OpenConnection(CINTEGRATION);
            SqlCommand cmd_inv = new SqlCommand();

            cmd_inv.CommandText = "exec sp_UpdateCustomerCount";
            cmd_inv.Connection = cIntegration;

            cmd_inv.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);
        }

        public void StoreCustomer(string clientId, string clientName)
        {
            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = "exec sp_StoreCreatedCustomer @clientId, @clientName";
            cmd.Parameters.AddWithValue("@clientId", clientId);
            cmd.Parameters.AddWithValue("@clientName", clientName);
            cmd.Connection = cIntegration;


            cmd.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);
        }

        public List<Queue> ReadMessageQueue()
        {
            OpenConnection(CMSGQUEUE);
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

            CloseConnection(CMSGQUEUE);
            return data;
        }

        public void UpdateReceiptNumber(int transactionId, string referenceNumber)
        {
            OpenConnection(CGENERIC);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = "exec sp_UpdateReceipt @receiptNum, @reference";
            cmd.Parameters.AddWithValue("@receiptNum", transactionId.ToString());
            cmd.Parameters.AddWithValue("@reference", referenceNumber);
            cmd.Connection = cGeneric;

            cmd.ExecuteNonQuery();
            CloseConnection(CGENERIC);
        }

        public void Log(string msg)
        {
            OpenConnection(CINTEGRATION);
            SqlCommand cmd_inv = new SqlCommand();

            cmd_inv.CommandText = "exec sp_Log @msg";
            cmd_inv.Parameters.AddWithValue("@msg", msg);
            cmd_inv.Connection = cIntegration;
            cmd_inv.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);

        }

        public DateTime GetValidity(int invoiceId)
        {
            OpenConnection(CINTEGRATION);
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


            CloseConnection(CINTEGRATION);
            return datetime;
        }

        public DateTime GetValidityEnd(int invoiceId)
        {
            OpenConnection(CINTEGRATION);
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

            CloseConnection(CINTEGRATION);
            return datetime;
        }

        public void ResetInvoiceTotal()
        {
            OpenConnection(CINTEGRATION);
            SqlCommand cmd_inv = new SqlCommand();

            cmd_inv.CommandText = "exec resetInvoiceTotal";
            cmd_inv.Connection = cIntegration;

            cmd_inv.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);
        }

        public string GetFreqUsage(int invoiceId)
        {
            OpenConnection(CINTEGRATION);
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

            CloseConnection(CINTEGRATION);
            return result;
        }

        public int GetCreditMemoNumber()
        {
            OpenConnection(CINTEGRATION);
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

            CloseConnection(CINTEGRATION);
            return num;
        }

        public void UpdateAsmsCreditMemoNumber(int docId, int newCredNum)
        {
            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = "exec sp_UpdateCreditMemoNum @documentId, @newCredNum";
            cmd.Parameters.AddWithValue("@documentId", docId);
            cmd.Parameters.AddWithValue("@newCredNum", newCredNum);
            cmd.Connection = cIntegration;


            cmd.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);

        }

        public InvoiceInfo GetInvoiceInfo(int invoiceId)
        {
            OpenConnection(CINTEGRATION);

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

            CloseConnection(CINTEGRATION);
            return inv;
        }

        public PaymentInfo GetReceiptInfo(int originalDocNum)
        {
            OpenConnection(CINTEGRATION);
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
            CloseConnection(CINTEGRATION);
            return rct;
        }

        public CreditNoteInfo GetCreditNoteInfo(int creditMemoNum, int documentId)
        {
            OpenConnection(CINTEGRATION);
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

            CloseConnection(CINTEGRATION);
            return creditNote;
        }

        public void UpdateAsmsCreditMNum(int currentNum, int newNum)
        {
            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = "exec sp_UpdateCreditMemoNum @currentNum, @newNum";
            cmd.Parameters.AddWithValue("@currentNum", currentNum);
            cmd.Parameters.AddWithValue("@newNum", newNum);
            cmd.Connection = cIntegration;

            cmd.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);
        }

        public string GetClientIdZRecord(bool stripExtention)
        {
            OpenConnection(CINTEGRATION);
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


            CloseConnection(CINTEGRATION);
            return result;
        }

        public void CheckResetCounters(int mExpiry, int dExpiry)
        {
            OpenConnection(CINTEGRATION);
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

            CloseConnection(CINTEGRATION);

            if (monthlyExpiry.Day == DateTime.Now.Day && monthlyExpiry.Month == DateTime.Now.Month && monthlyExpiry.Year == DateTime.Now.Year)
            {
                ResetMonthlyCounters(mExpiry);
            }

            if (dailyExpiry.Day == DateTime.Now.Day && dailyExpiry.Month == DateTime.Now.Month && dailyExpiry.Year == DateTime.Now.Year)
            {
                ResetDailyCounters(dExpiry);
            }
        }

        public void ResetMonthlyCounters(int daysToNExpiry)
        {

            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = "exec resetMonthlyCounters @nextExpiry";
            cmd.Parameters.AddWithValue("@nextExpiry", DateTime.Now.AddDays(daysToNExpiry));
            cmd.Connection = cIntegration;

            cmd.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);
        }

        public void ResetDailyCounters(int daysToNExpiry)
        {
            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();


            cmd.CommandText = "exec resetDailyCounters @nextExpiry";
            cmd.Parameters.AddWithValue("@nextExpiry", DateTime.Now.AddDays(daysToNExpiry));
            cmd.Connection = cIntegration;

            cmd.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);
        }

        public PrepaymentData CheckPrepaymentAvail(string customerId)
        {
            OpenConnection(CINTEGRATION);
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

            CloseConnection(CINTEGRATION);
            return data;
        }

        public void AdjustPrepaymentRemainder(decimal amount, int sequenceNumber)
        {
            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = "exec sp_adjustPrepaymentRemainder @amount, @sequenceNumber";
            cmd.Parameters.AddWithValue("@amount", amount);
            cmd.Parameters.AddWithValue("@sequenceNumber", sequenceNumber);
            cmd.Connection = cIntegration;

            cmd.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);
        }

        public decimal GetTotalPrepaymentRemainder(string customerId)
        {
            decimal result = 0;
            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "exec sp_getTotalPrepaymentRemainder @customerId";
            cmd.Parameters.AddWithValue("@customerId", customerId);
            cmd.Connection = cIntegration;

            cmd.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);
            return result;
        }

        public decimal GetPrepaymentURate(int sequence)
        {
            OpenConnection(CINTEGRATION);
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

            CloseConnection(CINTEGRATION);
            return urate;
        }


        public string GenerateReportId(string ReportType)
        {
            OpenConnection(CINTEGRATION);
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

            CloseConnection(CINTEGRATION);
            return result;
        }

        private void DataRouter(string ReportType, DataWrapper data, string recordID, int destination)
        {
            OpenConnection(CINTEGRATION);
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

            CloseConnection(CINTEGRATION);
            InsertSubtotals(ReportType, recordID, data, destination);
        }

        public string SaveReport(string ReportType, List<DataWrapper> categories, Totals total)
        {
            string id = GenerateReportId(ReportType);

            for (int i = 0; i < categories.Count; i++)
            {
                DataRouter(ReportType, categories[i], id, i);
            }

            InsertTotals(ReportType, id, total);
            return id;
        }

        public void InsertSubtotals(string ReportType, string reportID, DataWrapper data, int destination)
        {
            OpenConnection(CINTEGRATION);
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
            CloseConnection(CINTEGRATION);
        }


        public void InsertTotals(string ReportType, string reportID, Totals total)
        {
            OpenConnection(CINTEGRATION);
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
            CloseConnection(CINTEGRATION);
        }

        public DeferredData GetDeferredRpt(string ReportType, string report_id)
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
            cell_table.records = GetDeferredPartial(ReportType, 0, report_id);
            cell_table.setSubTotals(GetDeferredPartialSubs(ReportType, 0, report_id));

            bbrand_table.label = "Broadband";
            bbrand_table.records = GetDeferredPartial(ReportType, 1, report_id);
            bbrand_table.setSubTotals(GetDeferredPartialSubs(ReportType, 1, report_id));

            micro_table.label = "Microwave";
            micro_table.records = GetDeferredPartial(ReportType, 2, report_id);
            micro_table.setSubTotals(GetDeferredPartialSubs(ReportType, 2, report_id));

            vsat_table.label = "Vsat";
            vsat_table.records = GetDeferredPartial(ReportType, 3, report_id);
            vsat_table.setSubTotals(GetDeferredPartialSubs(ReportType, 3, report_id));

            marine_table.label = "Marine";
            marine_table.records = GetDeferredPartial(ReportType, 4, report_id);
            marine_table.setSubTotals(GetDeferredPartialSubs(ReportType, 4, report_id));

            dservices_table.label = "Data & Services";
            dservices_table.records = GetDeferredPartial(ReportType, 5, report_id);
            dservices_table.setSubTotals(GetDeferredPartialSubs(ReportType, 5, report_id));

            aero_table.label = "Aeronautical";
            aero_table.records = GetDeferredPartial(ReportType, 6, report_id);
            aero_table.setSubTotals(GetDeferredPartialSubs(ReportType, 6, report_id));

            trunking_table.label = "Trunking";
            trunking_table.records = GetDeferredPartial(ReportType, 7, report_id);
            trunking_table.setSubTotals(GetDeferredPartialSubs(ReportType, 7, report_id));

            other_table.label = "Other";
            other_table.records = GetDeferredPartial(ReportType, 8, report_id);
            other_table.setSubTotals(GetDeferredPartialSubs(ReportType, 8, report_id));

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
            d.Total = GetDeferredTotal(ReportType, report_id);

            return d;
        }

        private List<UIData> GetDeferredPartial(string ReportType, int index, string report_id)
        {
            OpenConnection(CINTEGRATION);
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

                CloseConnection(CINTEGRATION);
                return udt;
            }
            else
            {
                CloseConnection(CINTEGRATION);
                return udt;
            }
        }

        private SubTotals GetDeferredPartialSubs(string ReportType, int index, string report_id)
        {
            OpenConnection(CINTEGRATION);
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

                CloseConnection(CINTEGRATION);
                return subs;
            }
            else
            {
                CloseConnection(CINTEGRATION);
                return subs;
            }
        }


        public Totals GetDeferredTotal(string ReportType, string recordID)
        {
            OpenConnection(CINTEGRATION);
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

                CloseConnection(CINTEGRATION);
                return totals;
            }
            else
            {
                CloseConnection(CINTEGRATION);
                return totals;
            }
        }

        public void SetNextGenDate(string ReportType, DateTime date)
        {
            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "exec sp_setNewRptDate @ReportType, @date";
            cmd.Connection = cIntegration;

            cmd.Parameters.AddWithValue("@ReportType", ReportType);
            cmd.Parameters.AddWithValue("@date", date);
            cmd.ExecuteNonQuery();
            CloseConnection(CINTEGRATION);
        }

        public DateTime GetNextGenDate(string ReportType)
        {
            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            DateTime date = DateTime.Now;

            cmd.Connection = cIntegration;
            cmd.CommandText = "EXEC sp_getNextRptDate @ReportType";

            cmd.Parameters.AddWithValue("@ReportType", ReportType);
            reader = cmd.ExecuteReader();

            reader.Read();
            date = Convert.ToDateTime(reader["date"]);

            CloseConnection(CINTEGRATION);
            return date;
        }

        public void SetIntegrationStat(int stat)
        {
            System.Threading.Thread.Sleep(1000);
            OpenConnection(CMSGQUEUE);
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = cMsgQueue;
            cmd.CommandText = "sp_UpdateStat @stat";
            cmd.Parameters.AddWithValue("@stat", stat);
            cmd.ExecuteNonQuery();
            CloseConnection(CMSGQUEUE);
        }

        public bool CheckReportExist(DateTime date)
        {
            OpenConnection(CINTEGRATION);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;

            cmd.Connection = cIntegration;
            cmd.CommandText = "EXEC sp_checkRptExist @period";
            cmd.Parameters.AddWithValue("@period", date);
       
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                CloseConnection(CINTEGRATION);
                return true;
            }
            else
            {
                CloseConnection(CINTEGRATION);
                return false;
            }
        }
    }
}
