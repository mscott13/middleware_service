using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace middleware_service.TableDependencyDefinitions
{
    class SqlNotify_ArPayments
    {
        public void TransferToGeneric(string databaseName)
        {
            CustomerID = fixNulls(CustomerID);
            Source = fixNulls(Source);
            Check = fixNulls(Check);
            Ref = fixNulls(Ref);
            BankName = fixNulls(BankName);
            CheckNumber = fixNulls(CheckNumber);
            Status = fixNulls(Status);
            CreatedBy = fixNulls(CreatedBy);
            canceledBy = fixNulls(canceledBy);
            CanceledReason = fixNulls(CanceledReason);
            Remarks = fixNulls(Remarks);
            PaymentTranType = fixNulls(PaymentTranType);
            MiscCode = fixNulls(MiscCode);
            TaxGroup = fixNulls(TaxGroup);
            Proj = fixNulls(Proj);

            using (SqlConnection connection = new SqlConnection(databaseName))
            {
                string query = "";
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@GLID", GLID);
                    command.Parameters.AddWithValue("@CustomerID", CustomerID);
                    command.Parameters.AddWithValue("@Debit", Debit);
                    command.Parameters.AddWithValue("@InvoiceID", InvoiceID);
                    command.Parameters.AddWithValue("@GLTransactionID", GLTransactionID);
                    command.Parameters.AddWithValue("@ReceiptNumber", ReceiptNumber);
                    command.Parameters.AddWithValue("@Date1", Date1);
                    command.Parameters.AddWithValue("@Batch", Batch);
                    command.Parameters.AddWithValue("@PaymentType", PaymentType);
                    command.Parameters.AddWithValue("@Source", Source);
                    command.Parameters.AddWithValue("@Check", Check);
                    command.Parameters.AddWithValue("@Ref", Ref);
                    command.Parameters.AddWithValue("@Credit", Credit);
                    command.Parameters.AddWithValue("@GLUpdateDate", GLUpdateDate);
                    command.Parameters.AddWithValue("@Quarter", Quarter);
                    command.Parameters.AddWithValue("@year", year);
                    command.Parameters.AddWithValue("@FeeTaxesId", FeeTaxesId);
                    command.Parameters.AddWithValue("@BankName", BankName);
                    command.Parameters.AddWithValue("@CheckNumber", CheckNumber);
                    command.Parameters.AddWithValue("@Status", Status);
                    command.Parameters.AddWithValue("@isVoided", isVoided);
                    command.Parameters.AddWithValue("@CreatedBy", CreatedBy);
                    command.Parameters.AddWithValue("@canceledDate", canceledDate);
                    command.Parameters.AddWithValue("@canceledBy", canceledBy);
                    command.Parameters.AddWithValue("@CanceledReason", CanceledReason);
                    command.Parameters.AddWithValue("@Remarks", Remarks);
                    command.Parameters.AddWithValue("@PostingDate", PostingDate);
                    command.Parameters.AddWithValue("@PaymentTranType", PaymentTranType);
                    command.Parameters.AddWithValue("@MiscCode", MiscCode);
                    command.Parameters.AddWithValue("@TaxGroup", TaxGroup);
                    command.Parameters.AddWithValue("@LogId", LogId);
                    command.Parameters.AddWithValue("@CurrencyId", CurrencyId);
                    command.Parameters.AddWithValue("@DebitInternationalAmount", DebitInternationalAmount);
                    command.Parameters.AddWithValue("@CreditInternationalAmount", CreditInternationalAmount);
                    command.Parameters.AddWithValue("@ExchangeRate", ExchangeRate);
                    command.Parameters.AddWithValue("@exported", exported);
                    command.Parameters.AddWithValue("@exportdate", exportdate);
                    command.Parameters.AddWithValue("@Proj", Proj);
                    command.Parameters.AddWithValue("@relatedDocType", relatedDocType);
                    command.ExecuteNonQuery();
                }
            }
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


