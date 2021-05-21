using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Data;
using middleware_service.Database_Classes;
using middleware_service.TableDependencyDefinitions;

namespace middleware_service.Database_Operations
{
    public class Integration
    {
        public List<ReportRawData> getDIRInformation(string ReportType, DateTime searchStartDate, DateTime searchEndDate)
        {
            List<ReportRawData> reportInfo = new List<ReportRawData>();
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("EXEC sp_getDIRInformation @ReportType, @searchStartDate, @searchEndDate", connection))
                {
                    command.Parameters.AddWithValue("@ReportType", ReportType);
                    command.Parameters.AddWithValue("@searchStartDate", searchStartDate);
                    command.Parameters.AddWithValue("@searchEndDate", searchEndDate);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        ReportRawData record = new ReportRawData();
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
                            return reportInfo;
                        }
                        else
                        {
                            return reportInfo;
                        }
                    }
                }
            }
        }

        public List<Batch> GetExpiryBatchDate()
        {
            List<Batch> lstInvoiceBatchData = new List<Batch>(2);
            List<Batch> lstInvoiceBatchDataRet = new List<Batch>(2);
            Batch batch;

            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("EXEC sp_GetOpenBatch", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
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
                    }
                }
            }
            return lstInvoiceBatchDataRet;
        }

        public void closeInvoiceBatch(int batchId)
        {
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("EXEC closeBatch @batchId", connection))
                {
                    command.Parameters.AddWithValue("@batchId", batchId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public Maj getMajDetail(int referenceNumber)
        {
            Maj mj = new Maj();
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("EXEC getMajDetail @referenceNumber", connection))
                {
                    command.Parameters.AddWithValue("@referenceNumber", referenceNumber);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        if (reader.HasRows)
                        {
                            reader.Read();
                            mj.stationType = reader["stationType"].ToString();
                            if (reader["CertificateType"].ToString() != "") mj.certificateType = Convert.ToInt32(reader["CertificateType"]);
                            if (reader["subStationType"].ToString() != "") mj.substationType = Convert.ToInt32(reader["subStationType"]);
                            mj.proj = reader["Proj"].ToString();
                        }

                    }
                }
            }
            return mj;
        }


        public DataSet GetRenewalInvoiceValidity(int invoiceid)
        {
            using (SqlConnection connection = new SqlConnection(Constants.dbGeneric)) // could not find stored procedure
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("EXEC sp_getValidityRenewalInvoice", connection))
                {
                    command.Parameters.AddWithValue("@invoiceid", invoiceid);
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataSet dataSet = new DataSet();
                        adapter.Fill(dataSet);
                        return dataSet;
                    }
                }
            }
        }


        public int getInvoiceReference(int invoiceId)
        {
            int refNumber = -1;
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("EXEC getInvoiceRef @invoiceId", connection))
                {
                    command.Parameters.AddWithValue("@invoiceId", invoiceId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        if (reader.HasRows)
                        {
                            reader.Read();
                            if (reader[0].ToString() != "")
                            {
                                refNumber = Convert.ToInt32(reader[0]);
                            }
                        }
                    }
                }
            }
            return refNumber;
        }

        public void createInvoiceBatch(double daysTillEx, int batchId, string batchType, string renstat)
        {
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("EXEC createBatch @batchId, @expirydate, @batchType, @renstat", connection))
                {
                    command.Parameters.AddWithValue("@batchId", batchId);
                    command.Parameters.AddWithValue("@expirydate", DateTime.Now.AddDays(daysTillEx));
                    command.Parameters.AddWithValue("@batchType", batchType);
                    command.Parameters.AddWithValue("@renstat", renstat);
                    command.ExecuteNonQuery();
                }
            }
        }

        public bool batchAvail(string batchType)
        {
            int result = -1;
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("EXEC batchAvail @batchType", connection))
                {
                    command.Parameters.AddWithValue("@batchType", batchType);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        if (reader.HasRows)
                        {
                            reader.Read();
                            result = Convert.ToInt32(reader[0]);

                            if (result > 0)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public bool isBatchExpired(int batchId)
        {
            DateTime expiryDate = DateTime.Now;
            string renstat = " ";
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("EXEC getBatchExpiry @batchId", connection))
                {
                    command.Parameters.AddWithValue("@batchId", batchId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            expiryDate = Convert.ToDateTime(reader[0]);
                            renstat = Convert.ToString(reader[1]);
                        }
                    }
                }
            }

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
            int result = -1;
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("EXEC getBatch @batchType", connection))
                {
                    command.Parameters.AddWithValue("@batchType", batchType);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        if (reader.HasRows)
                        {
                            reader.Read();
                            result = Convert.ToInt32(reader[0]);
                        }
                    }
                }
            }
            return result;
        }


        public void openNewReceiptBatch(double DaysTillExpired, int LastBatchId, string bankcode)
        {
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("Exec sp_NewBatchSet_Payment @batchId, @CreatedDate, @ExpiryDate, @bankcode", connection))
                {
                    command.Parameters.AddWithValue("@batchId", LastBatchId);
                    command.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                    command.Parameters.AddWithValue("@ExpiryDate", DateTime.Now.AddDays(DaysTillExpired));
                    command.Parameters.AddWithValue("@bankcode", bankcode);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void closeReceiptBatch(int batchId)
        {
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("EXEC closeReceiptBatch @batchId", connection))
                {
                    command.Parameters.AddWithValue("@batchId", batchId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void updateBatchAmount(string batchType, decimal amount)
        {
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("EXEC updateBatchAmount @batchType, @amount", connection))
                {
                    command.Parameters.AddWithValue("@batchType", batchType);
                    command.Parameters.AddWithValue("@amount", amount);
                    command.ExecuteNonQuery();
                }
            }
        }

        public string getBankCodeId(string bankcode)
        {
            string bankcodeid = "";
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("EXEC getBankCode @bankcode", connection))
                {
                    command.Parameters.AddWithValue("@bankcode", bankcode);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            bankcodeid = reader[0].ToString();
                        }
                    }
                }
            }
            return bankcodeid;
        }


        public DateTime getDocDate(int docNumber)
        {
            DateTime date = DateTime.Now;
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("EXEC getInvDocDate @docNumber", connection))
                {
                    command.Parameters.AddWithValue("@docNumber", docNumber);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        if (reader.HasRows)
                        {
                            reader.Read();
                            date = Convert.ToDateTime(reader[0]);
                        }
                    }
                }
            }
            return date;
        }


        public void UpdateBatchCount(string BatchType)
        {
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("Exec UpdateBatchCount @BatchType", connection))
                {
                    command.Parameters.AddWithValue("@BatchType", BatchType);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void IncrementReferenceNumber(string BankCode, decimal amount)
        {
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("Exec sp_IncrementRefNumber @BankCode, @amount", connection))
                {
                    command.Parameters.AddWithValue("@BankCode", BankCode);
                    command.Parameters.AddWithValue("@amount", amount);
                    command.ExecuteNonQuery();
                }
            }
        }

        public decimal GetRate()
        {
            decimal result = 0;
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("EXEC sp_GetAsmsRate", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        if (reader.HasRows)
                        {
                            reader.Read();
                            result = Convert.ToDecimal(reader[0].ToString());
                        }
                    }
                }
            }
            return result;
        }


        public void UpdateBatchCountPayment(string BatchId)
        {
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("Exec sp_IncrementEntryCount_payment @BatchId", connection))
                {
                    command.Parameters.AddWithValue("@BatchId", BatchId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public string GetInitialRef(string BankCodeId)
        {
            string refNumber = "";
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("exec sp_GetInitialRefNumber @BankCodeId", connection))
                {
                    command.Parameters.AddWithValue("@BankCodeId", BankCodeId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            refNumber = reader[0].ToString();
                        }
                    }
                }
            }
            return refNumber;
        }

        public decimal GetUsRateByInvoice(int invoiceid)
        {
            decimal rate = 1;
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("exec sp_GetUsRateByInvoice @invoiceid", connection))
                {
                    command.Parameters.AddWithValue("@invoiceid", invoiceid);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        if (reader.HasRows)
                        {
                            rate = Convert.ToDecimal(reader[0].ToString());
                        }
                    }
                }
            }
            return rate;
        }

        public string GetCurrentRef(string BankCode)
        {
            string refNumber = "";
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("exec sp_GetLastRefNumber @BankCodeId", connection))
                {
                    command.Parameters.AddWithValue("@BankCodeId", BankCode);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        if (reader.HasRows)
                        {
                            int i = Convert.ToInt32(reader[0]);
                            refNumber = i.ToString();
                        }
                    }
                }
            }
            return refNumber;
        }

        public string getRecieptBatch(string bankcode)
        {
            string batch = "";
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("EXEC sp_getReceiptBatch @bankcode", connection))
                {
                    command.Parameters.AddWithValue("@bankcode", bankcode);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            batch = reader[0].ToString();
                        }
                    }
                }
            }
            return batch;
        }

        public List<string> checkInvoiceAvail(string invoiceId)
        {
            List<string> data = new List<string>(3);
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("exec sp_GetInvoice @id", connection))
                {
                    command.Parameters.AddWithValue("@id", invoiceId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
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
                    }
                }
            }
            return data;
        }

        public void storeInvoice(int invoiceId, int batchTarget, int CreditGL, string clientName, string clientId, DateTime date, string author, decimal amount, string state, decimal usrate, decimal usamount, int isvoid, int isCreditMemo, int creditMemoNumber)
        {
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("exec sp_StoreInvoice @id, @target, @CreditGL, @clientName, @clientId, @dateCreated, @author, @amount, @state, @usrate, @usamount, @isvoid, @isCreditMemo, @credMemoNum", connection))
                {
                    command.Parameters.AddWithValue("@id", invoiceId);
                    command.Parameters.AddWithValue("@target", batchTarget);
                    command.Parameters.AddWithValue("@CreditGL", CreditGL);
                    command.Parameters.AddWithValue("@clientName", clientName);
                    command.Parameters.AddWithValue("@clientId", clientId);
                    command.Parameters.AddWithValue("@dateCreated", date);
                    command.Parameters.AddWithValue("@author", author);
                    command.Parameters.AddWithValue("@amount", amount);
                    command.Parameters.AddWithValue("@state", state);
                    command.Parameters.AddWithValue("@usrate", usrate);
                    command.Parameters.AddWithValue("@usamount", usamount);
                    command.Parameters.AddWithValue("@isvoid", isvoid);
                    command.Parameters.AddWithValue("@isCreditMemo", isCreditMemo);
                    command.Parameters.AddWithValue("@credMemoNum", creditMemoNumber);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void storePayment(string clientId, string clientName, DateTime createdDate, string invoiceId, decimal amount, decimal usamount, string prepstat, int referenceNumber, int destinationBank, string isPayByCredit, decimal prepaymentUsRate)
        {
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("exec sp_StorePayment @clientId, @clientName, @createdDate, @invoiceId, @amount, @usamount, @prepstat, @referenceNumber, @destinationBank, @isPayByCredit, @prepaymentUsRate", connection))
                {
                    command.Parameters.AddWithValue("@clientId", clientId);
                    command.Parameters.AddWithValue("@clientName", clientName);
                    command.Parameters.AddWithValue("@createdDate", createdDate);
                    command.Parameters.AddWithValue("@invoiceId", invoiceId);
                    command.Parameters.AddWithValue("@amount", amount);
                    command.Parameters.AddWithValue("@usamount", usamount);
                    command.Parameters.AddWithValue("@prepstat", prepstat);
                    command.Parameters.AddWithValue("@referenceNumber", referenceNumber);
                    command.Parameters.AddWithValue("@destinationBank", destinationBank);
                    command.Parameters.AddWithValue("@isPayByCredit", isPayByCredit);
                    command.Parameters.AddWithValue("@prepaymentUsRate", prepaymentUsRate);
                    command.ExecuteNonQuery();
                }
            }
        }

        public SqlNotifyCancellation GetInvoiceCanellationInfo(int invoiceId)
        {
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("select ARInvoiceID, Amount, isVoided, canceledBy, CustomerID, notes, FeeType from ASMSGenericMaster.dbo.tblArInvoices where ARInvoiceID=@invoiceId", connection))
                {
                    command.Parameters.AddWithValue("@invoiceId", invoiceId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            var data = new SqlNotifyCancellation();

                            if (reader.IsDBNull(0))
                                data.ARInvoiceID = 0;
                            else
                                data.ARInvoiceID = Convert.ToInt32(reader["ARInvoiceID"]);

                            if (reader.IsDBNull(1))
                                data.Amount = 0;
                            else
                                data.Amount = Convert.ToDecimal(reader["Amount"]);

                            if (reader.IsDBNull(2))
                                data.isVoided = 0;
                            else
                                data.isVoided = Convert.ToInt32(reader["isVoided"]);

                            if (reader.IsDBNull(3))
                                data.canceledBy = "";
                            else
                                data.canceledBy = reader["canceledBy"].ToString();

                            if (reader.IsDBNull(4))
                                data.CustomerID = 0;
                            else
                                data.CustomerID = Convert.ToInt32(reader["CustomerID"]);

                            if (reader.IsDBNull(5))
                                data.notes = "";
                            else
                                data.notes = reader["notes"].ToString();

                            if (reader.IsDBNull(6))
                                data.FeeType = "";
                            else
                                data.FeeType = reader["FeeType"].ToString();

                            return data;
                        }
                        return null;
                    }
                }
            }
        }

        public SqlNotify_DocumentInfo GetGLDocumentInfo(int originalDocumentId, int documentType)
        {
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("Select top 1 DocumentType, OriginalDocumentID, DocumentID, PaymentMethod from ASMSGenericMaster.dbo.tblGLDocuments where OriginalDocumentId=@originalDocumentId and DocumentType=@documentType", connection))
                {
                    command.Parameters.AddWithValue("@originalDocumentId", originalDocumentId);
                    command.Parameters.AddWithValue("@documentType", documentType);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            var data = new SqlNotify_DocumentInfo();

                            if (reader.IsDBNull(0))
                            {
                                data.DocumentType = 0;
                            }
                            else
                                data.DocumentType = Convert.ToInt32(reader["DocumentType"]);

                            if (reader.IsDBNull(1))
                            {
                                data.OriginalDocumentID = 0;
                            }
                            else
                                data.OriginalDocumentID = Convert.ToInt32(reader["OriginalDocumentID"]);

                            if (reader.IsDBNull(2))
                            {
                                data.DocumentID = 0;
                            }
                            else
                                data.DocumentID = Convert.ToInt32(reader["DocumentID"]);

                            if (reader.IsDBNull(3))
                            {
                                data.PaymentMethod = 0;
                            }
                            else
                                data.PaymentMethod = Convert.ToInt32(reader["PaymentMethod"]);

                            return data;
                        }
                    }
                }
            }
            return null;
        }

        public SqlNotify_DocumentInfo GetGLDocumentInfoViaId(int documentId)
        {
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("Select DocumentType, OriginalDocumentID, DocumentID, PaymentMethod from ASMSGenericMaster.dbo.tblGLDocuments where DocumentId=@documentId", connection))
                {
                    command.Parameters.AddWithValue("@documentId", documentId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            var data = new SqlNotify_DocumentInfo();

                            if (reader.IsDBNull(0))
                            {
                                data.DocumentType = 0;
                            }
                            else
                                data.DocumentType = Convert.ToInt32(reader["DocumentType"]);

                            if (reader.IsDBNull(1))
                            {
                                data.OriginalDocumentID = 0;
                            }
                            else
                                data.OriginalDocumentID = Convert.ToInt32(reader["OriginalDocumentID"]);

                            if (reader.IsDBNull(2))
                            {
                                data.DocumentID = 0;
                            }
                            else
                                data.DocumentID = Convert.ToInt32(reader["DocumentID"]);

                            if (reader.IsDBNull(3))
                            {
                                data.PaymentMethod = 0;
                            }
                            else
                                data.PaymentMethod = Convert.ToInt32(reader["PaymentMethod"]);

                            return data;
                        }
                    }
                }
            }
            return null;
        }

        public string GetAccountNumber(int GLID)
        {
            string accountNumber = "";
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("exec sp_GetGLAcctNumber @GLID", connection))
                {
                    command.Parameters.AddWithValue("@GLID", GLID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        if (reader.HasRows)
                        {
                            reader.Read();
                            accountNumber = reader[0].ToString();
                        }
                    }
                }
            }
            return accountNumber;
        }


        public List<string> GetInvoiceDetails(int invoiceId)
        {
            List<string> data = new List<string>(3);
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("exec sp_GetInvoiceDetail @invoiceId", connection))
                {
                    command.Parameters.AddWithValue("@invoiceId", invoiceId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
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
                    }
                }
            }
            return data;
        }

        public void MarkAsTransferred(int invoiceId)
        {
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("exec sp_UpdateInvoiceToTransferred @invoiceId", connection))
                {
                    command.Parameters.AddWithValue("@invoiceId", invoiceId);
                    command.ExecuteNonQuery();
                }
            }
        }


        public int GetCreditGl(string invoiceiD)
        {
            int i = 0;
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("exec sp_GetCreditGL @invoiceId", connection))
                {
                    command.Parameters.AddWithValue("@invoiceId", invoiceiD);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            i = Convert.ToInt32(reader[0].ToString());
                        }
                    }
                }
            }
            return i;
        }

        public int GetCreditGlID(string GLTransactionID)
        {
            int i = 0;
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("exec sp_GetPrepaymentGlID @GLTransactionID", connection))
                {
                    command.Parameters.AddWithValue("@GLTransactionID", GLTransactionID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        if (reader.HasRows)
                        {
                            reader.Read();
                            i = Convert.ToInt32(reader[0].ToString());
                        }
                    }
                }
            }
            return i;
        }

        public string isAnnualFee(int invoiceid)
        {
            string notes = " ";
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("exec sp_isAnnualFee @invoiceid", connection))
                {
                    command.Parameters.AddWithValue("@invoiceid", invoiceid);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        if (reader.HasRows)
                        {
                            reader.Read();
                            notes = reader[0].ToString();
                        }

                    }
                }
            }
            return notes;
        }


        public void UpdateCreditGl(int invoiceId, int newCreditGl)
        {
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("exec sp_UpdateCreditGl @invoiceId, @newCreditGl", connection))
                {
                    command.Parameters.AddWithValue("@invoiceId", invoiceId);
                    command.Parameters.AddWithValue("@newCreditGl", newCreditGl);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void modifyInvoiceList(int invoiceId, decimal rate, string customerId)
        {
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("exec sp_UpdateInvoice @invoiceid, @usrate, @customerId", connection))
                {
                    command.Parameters.AddWithValue("@invoiceId", invoiceId);
                    command.Parameters.AddWithValue("@usrate", rate);
                    command.Parameters.AddWithValue("@customerId", customerId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateEntryNumber(int invoiceId)
        {
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("exec sp_UpdateEntry @invoiceId", connection))
                {
                    command.Parameters.AddWithValue("@invoiceId", invoiceId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<string> GetPaymentInfo(int gl_id)
        {
            List<string> data = new List<string>(4);
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("EXEC sp_GetPayInfo @id", connection))
                {
                    command.Parameters.AddWithValue("@id", gl_id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        if (reader.HasRows)
                        {
                            reader.Read();

                            var debit = reader[0].ToString();
                            var glid = reader[1].ToString();
                            var invoiceId = reader[2].ToString();
                            var paymentDate = reader[3].ToString();

                            data.Add(debit);
                            data.Add(glid);
                            data.Add(invoiceId);
                            data.Add(paymentDate);
                        }
                    }
                }
            }
            return data;
        }

        public List<string> getClientInfo_inv(string id)
        {
            List<string> data = new List<string>(4);
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("EXEC sp_GetClientInfo @id", connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
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
                    }
                }
            }
            return data;
        }


        public List<string> GetFeeInfo(int invoiceId)
        {
            List<string> data = new List<string>(2);
            using (SqlConnection connection = new SqlConnection(Constants.dbGeneric))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("Select FeeType, notes from tblARInvoices where ARInvoiceID=@id_inv", connection))
                {
                    command.Parameters.AddWithValue("@id_inv", invoiceId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            var ftype = reader[0].ToString();
                            var notes = reader[1].ToString();

                            data.Add(ftype);
                            data.Add(notes);
                        }
                    }
                }
            }
            return data;
        }


        public void UpdateCustomerCount()
        {
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("exec sp_UpdateCustomerCount", connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public void StoreCustomer(string clientId, string clientName)
        {
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("exec sp_StoreCreatedCustomer @clientId, @clientName", connection))
                {
                    command.Parameters.AddWithValue("@clientId", clientId);
                    command.Parameters.AddWithValue("@clientName", clientName);
                    command.ExecuteNonQuery();
                }
            }
        }



        public void UpdateReceiptNumber(int transactionId, string referenceNumber)
        {
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("exec sp_UpdateReceipt @receiptNum, @reference", connection))
                {
                    command.Parameters.AddWithValue("@receiptNum", transactionId.ToString());
                    command.Parameters.AddWithValue("@reference", referenceNumber);
                    command.ExecuteNonQuery();
                }
            }
        }


        public DateTime GetValidity(int invoiceId)
        {
            var datetime = DateTime.Now;
            DateTime startdate = DateTime.Now;

            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("exec sp_GetValidity @invoiceId", connection))
                {
                    command.Parameters.AddWithValue("@invoiceId", invoiceId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        if (reader.HasRows)
                        {
                            reader.Read();
                            datetime = Convert.ToDateTime(reader[6]);
                        }

                    }
                }
            }
            return datetime;
        }

        public DateTime GetValidityEnd(int invoiceId)
        {
            var datetime = DateTime.Now;
            DateTime startdate = DateTime.Now;

            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("exec sp_GetValidity @invoiceId", connection))
                {
                    command.Parameters.AddWithValue("@invoiceId", invoiceId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            datetime = Convert.ToDateTime(reader[7]);
                        }
                    }
                }
            }

            return datetime;
        }

        public void resetInvoiceTotal()
        {
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("exec resetInvoiceTotal", connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public string getFreqUsage(int invoiceId)
        {
            string result = "";
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("exec sp_freqUsage @invoiceId", connection))
                {
                    command.Parameters.AddWithValue("@invoiceId", invoiceId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        if (reader.HasRows)
                        {
                            reader.Read();
                            result = reader[0].ToString();
                        }
                    }
                }
            }
            return result;
        }

        public int getCreditMemoNumber()
        {
            int num = -1;
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("exec sp_getCMemoSeq", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            num = Convert.ToInt32(reader[0]);
                        }
                    }
                }
            }
            return num;
        }

        public void updateAsmsCreditMemoNumber(int docId, int newCredNum)
        {
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("exec sp_UpdateCreditMemoNum @documentId, @newCredNum", connection))
                {
                    command.Parameters.AddWithValue("@documentId", docId);
                    command.Parameters.AddWithValue("@newCredNum", newCredNum);
                    command.ExecuteNonQuery();
                }

            }
        }

        public InvoiceInfo getInvoiceDetails(int invoiceId)
        {
            InvoiceInfo inv = new InvoiceInfo();
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("exec sp_getInvoiceInfo @invoiceId", connection))
                {
                    command.Parameters.AddWithValue("@invoiceId", invoiceId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
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
                    }
                }
            }
            return inv;
        }

        public PaymentInfo getPaymentInfo(int originalDocNum)
        {
            PaymentInfo rct = new PaymentInfo();
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("exec sp_getReceiptInfo @originalDocNum", connection))
                {
                    command.Parameters.AddWithValue("@originalDocNum", originalDocNum);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

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
                    }
                }
            }
            return rct;
        }

        public CreditNoteInfo getCreditNoteInfo(int creditMemoNum, int documentId)
        {
            CreditNoteInfo creditNote = new CreditNoteInfo();
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("exec sp_getCreditMemoInfo @creditNoteNum, @documentId", connection))
                {
                    command.Parameters.AddWithValue("@creditNoteNum", creditMemoNum);
                    command.Parameters.AddWithValue("@documentId", documentId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
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
                    }
                }
            }
            return creditNote;
        }


        public string getClientIdZRecord(bool stripExtention)
        {
            string result = "";
            string temp = "";
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("exec sp_getZeroRecord", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
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
                    }
                }

            }
            return result;
        }


        public PrepaymentData checkPrepaymentAvail(string customerId)
        {
            PrepaymentData data = new PrepaymentData();
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("exec sp_getCustomerPrepayment @customerId", connection))
                {
                    command.Parameters.AddWithValue("@customerId", customerId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

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
                    }
                }
            }
            return data;
        }

        public void adjustPrepaymentRemainder(decimal amount, int sequenceNumber)
        {
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("exec sp_adjustPrepaymentRemainder @amount, @sequenceNumber", connection))
                {
                    command.Parameters.AddWithValue("@amount", amount);
                    command.Parameters.AddWithValue("@sequenceNumber", sequenceNumber);
                    command.ExecuteNonQuery();
                }
            }
        }


        public string generateReportId(string ReportType)
        {
            string result = "";
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("exec sp_DIRnewReportID @ReportType", connection))
                {
                    command.Parameters.AddWithValue("@ReportType", ReportType);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            result = reader[0].ToString();
                        }
                    }
                }
            }
            return result;
        }

        private void dataRouter(string ReportType, DataWrapper data, string recordID, int destination)
        {
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("exec sp_rptRecInsert @ReportType, @reportId, @licenseNumber, @clientCompany, @invoiceID, @budget, @invoiceTotal, @thisPeriodsInv, @balBFwd, @fromRev, @toRev, @closingBal, @totalMonths, @monthUtil, @monthRemain,  @valPStart, @valPEnd, @destination", connection))
                {
                    for (int i = 0; i < data.records.Count; i++)
                    {
                        command.Parameters.AddWithValue("@ReportType", ReportType);
                        command.Parameters.AddWithValue("@reportId", recordID);
                        command.Parameters.AddWithValue("@licenseNumber", data.records[i].licenseNumber);
                        command.Parameters.AddWithValue("@clientCompany", data.records[i].clientCompany);
                        command.Parameters.AddWithValue("@invoiceID", data.records[i].invoiceID);
                        command.Parameters.AddWithValue("@budget", data.records[i].budget);
                        command.Parameters.AddWithValue("@invoiceTotal", data.records[i].invoiceTotal);
                        command.Parameters.AddWithValue("@thisPeriodsInv", data.records[i].thisPeriodsInv);
                        command.Parameters.AddWithValue("@balBFwd", data.records[i].balBFwd);
                        command.Parameters.AddWithValue("@fromRev", data.records[i].fromRev);
                        command.Parameters.AddWithValue("@toRev", data.records[i].toRev);
                        command.Parameters.AddWithValue("@closingBal", data.records[i].closingBal);
                        command.Parameters.AddWithValue("@totalMonths", data.records[i].totalMonths);
                        command.Parameters.AddWithValue("@monthUtil", data.records[i].monthUtil);
                        command.Parameters.AddWithValue("@monthRemain", data.records[i].monthRemain);
                        command.Parameters.AddWithValue("@valPStart", data.records[i].valPStart);
                        command.Parameters.AddWithValue("@valPEnd", data.records[i].valPEnd);
                        command.Parameters.AddWithValue("@destination", destination);
                        command.ExecuteNonQuery();
                    }
                }
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
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("exec sp_insertSubtotals @ReportType, @reportId, @category, @invoiceTotal, @balanceBFwd, @toRev, @closingBal, @fromRev, @budget", connection))
                {
                    command.Parameters.AddWithValue("@ReportType", ReportType);
                    command.Parameters.AddWithValue("@reportId", reportID);
                    command.Parameters.AddWithValue("@category", destination);
                    command.Parameters.AddWithValue("@invoiceTotal", data.subT_invoiceTotal);
                    command.Parameters.AddWithValue("@balanceBFwd", data.subT_balBFwd);
                    command.Parameters.AddWithValue("@toRev", data.subT_toRev);
                    command.Parameters.AddWithValue("@closingBal", data.subT_closingBal);
                    command.Parameters.AddWithValue("@fromRev", data.subT_fromRev);
                    command.Parameters.AddWithValue("@budget", data.subT_budget);
                    command.ExecuteNonQuery();
                }
            }
        }


        public void insertTotals(string ReportType, string reportID, Totals total)
        {
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("exec sp_insertTotals @ReportType, @recordID, @invoiceTotal, @balanceBFwd, @toRev, @closingBal, @fromRev, @budget", connection))
                {
                    command.Parameters.AddWithValue("@ReportType", ReportType);
                    command.Parameters.AddWithValue("@recordID", reportID);
                    command.Parameters.AddWithValue("@invoiceTotal", total.tot_invoiceTotal);
                    command.Parameters.AddWithValue("@balanceBFwd", total.tot_balBFwd);
                    command.Parameters.AddWithValue("@toRev", total.tot_toRev);
                    command.Parameters.AddWithValue("@closingBal", total.tot_closingBal);
                    command.Parameters.AddWithValue("@fromRev", total.tot_fromRev);
                    command.Parameters.AddWithValue("@budget", total.tot_budget);
                    command.ExecuteNonQuery();
                }
            }
        }

        public DateTime GetNextGenDate(string ReportType)
        {
            DateTime date = DateTime.Now;
            using (SqlConnection connection = new SqlConnection(Constants.dbIntegration))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("EXEC sp_getNextRptDate @ReportType", connection))
                {
                    command.Parameters.AddWithValue("@ReportType", ReportType);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        reader.Read();
                        date = Convert.ToDateTime(reader["date"]);
                    }
                }
            }
            return date;
        }
    }
}
