using middleware_service.Other_Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace middleware_service.TableDependencyDefinitions
{
    class SqlNotify_ArPayments
    {
        public void ArPaymentsToGeneric()
        {
            using (SqlConnection connection = new SqlConnection(Constants.TEST_DB_GENERIC))
            {
                string query = "INSERT INTO dbo.tblARPayments " +
                                "(Date1,InvoiceID,Batch#,PaymentType,Source,Check#,Ref#,GLID,Debit,Credit,GLUpdateDate " +
		                                ",CustomerID,Quarter,year,FeeTaxesId,BankName,CheckNumber,Status,ReceiptNumber,isVoided "+
                                ",CreatedBy,canceledDate,canceledBy,CanceledReason,Remarks,PostingDate,PaymentTranType,MiscCode " +
                                ",TaxGroup,LogId,CurrencyId,DebitInternationalAmount,CreditInternationalAmount,ExchangeRate,exported " +
                                ",exportdate,Proj,relatedDocType) " +
                                "VALUES " +
                                "(@Date1,@InvoiceID, @Batch#,@PaymentType,@Source,@Check# " +
                                ",@Ref#,@GLID,@Debit,@Credit,@GLUpdateDate,@CustomerID,@Quarter " +
                                ",@year,@FeeTaxesId,@BankName,@CheckNumber,@Status,@ReceiptNumber " +
                                ",@isVoided,@CreatedBy,@canceledDate,@canceledBy,@CanceledReason " +
                                ",@Remarks,@PostingDate,@PaymentTranType,@MiscCode,@TaxGroup " +
                                ",@LogId,@CurrencyId,@DebitInternationalAmount,@CreditInternationalAmount " +
                                ",@ExchangeRate,@exported,@exportdate,@Proj,@relatedDocType)";


                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@GLID", GLID);
                    command.Parameters.AddWithValue("@CustomerID", FixNulls(CustomerID));
                    command.Parameters.AddWithValue("@Debit", Math.Round(Debit, 2));
                    command.Parameters.AddWithValue("@InvoiceID", InvoiceID);
                    command.Parameters.AddWithValue("@ReceiptNumber", ReceiptNumber);
                    command.Parameters.AddWithValue("@Date1", FixDate(Date1));
                    command.Parameters.AddWithValue("@Batch#", Batch);
                    command.Parameters.AddWithValue("@PaymentType", PaymentType);
                    command.Parameters.AddWithValue("@Source", FixNulls(Source));
                    command.Parameters.AddWithValue("@Check#", FixNulls(Check));
                    command.Parameters.AddWithValue("@Ref#", FixNulls(Ref));
                    command.Parameters.AddWithValue("@Credit", Math.Round(Credit, 2));
                    command.Parameters.AddWithValue("@GLUpdateDate", FixDate(GLUpdateDate));
                    command.Parameters.AddWithValue("@Quarter", Quarter);
                    command.Parameters.AddWithValue("@year", year);
                    command.Parameters.AddWithValue("@FeeTaxesId", FeeTaxesId);
                    command.Parameters.AddWithValue("@BankName", FixNulls(BankName));
                    command.Parameters.AddWithValue("@CheckNumber", FixNulls(CheckNumber));
                    command.Parameters.AddWithValue("@Status", FixNulls(Status));
                    command.Parameters.AddWithValue("@isVoided", isVoided);
                    command.Parameters.AddWithValue("@CreatedBy", FixNulls(CreatedBy));
                    command.Parameters.AddWithValue("@canceledDate", FixDate(canceledDate));
                    command.Parameters.AddWithValue("@canceledBy", FixNulls(canceledBy));
                    command.Parameters.AddWithValue("@CanceledReason", FixNulls(CanceledReason));
                    command.Parameters.AddWithValue("@Remarks", FixNulls(Remarks));
                    command.Parameters.AddWithValue("@PostingDate", FixDate(PostingDate));
                    command.Parameters.AddWithValue("@PaymentTranType", FixNulls(PaymentTranType));
                    command.Parameters.AddWithValue("@MiscCode", FixNulls(MiscCode));
                    command.Parameters.AddWithValue("@TaxGroup", FixNulls(TaxGroup));
                    command.Parameters.AddWithValue("@LogId", LogId);
                    command.Parameters.AddWithValue("@CurrencyId", CurrencyId);
                    command.Parameters.AddWithValue("@DebitInternationalAmount", DebitInternationalAmount);
                    command.Parameters.AddWithValue("@CreditInternationalAmount", CreditInternationalAmount);
                    command.Parameters.AddWithValue("@ExchangeRate", ExchangeRate);
                    command.Parameters.AddWithValue("@exported", exported);
                    command.Parameters.AddWithValue("@exportdate", FixDate(exportdate));
                    command.Parameters.AddWithValue("@Proj", FixNulls(Proj));
                    command.Parameters.AddWithValue("@relatedDocType", relatedDocType);
                    command.ExecuteNonQuery();
                    Log.Save("Parallel transfer completed for ARPayments");
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

        public int GLID { get; set; }
        public string CustomerID { get; set; }
        public float Debit { get; set; }
        public int InvoiceID { get; set; }
        public int GLTransactionID { get; set; }
        public int ReceiptNumber { get; set; }
        public DateTime Date1 { get; set; }
        public int Batch { get; set; }
        public int PaymentType { get; set; }
        public string Source { get; set; }
        public string Check { get; set; }
        public string Ref { get; set; }
        public decimal Credit { get; set; }
        public DateTime GLUpdateDate { get; set; }
        public int Quarter { get; set; }
        public int year { get; set; }
        public int FeeTaxesId { get; set; }
        public string BankName { get; set; }
        public string CheckNumber { get; set; }
        public string Status { get; set; }
        public bool isVoided { get; set; }
        public string CreatedBy { get; set; }
        public DateTime canceledDate { get; set; }
        public string canceledBy { get; set; }
        public string CanceledReason { get; set; }
        public string Remarks { get; set; }
        public DateTime PostingDate { get; set; }
        public string PaymentTranType { get; set; }
        public string MiscCode { get; set; }
        public string TaxGroup { get; set; }
        public int LogId { get; set; }
        public int CurrencyId { get; set; }
        public decimal DebitInternationalAmount { get; set; }
        public decimal CreditInternationalAmount { get; set; }
        public decimal ExchangeRate { get; set; }
        public bool exported { get; set; }
        public DateTime exportdate { get; set; }
        public string Proj { get; set; }
        public int relatedDocType { get; set; }
    }
}


