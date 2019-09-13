using middleware_service.Other_Classes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace middleware_service.TableDependencyDefinitions
{
    class SqlNotify_GlDocuments
    {
        public void TransferToGeneric(string database)
        {
            string query = "INSERT INTO dbo.tblGLDocuments(IsSystemVoided,IsVoided,IsValid,DocumentType,DocumentDisplayNumber,FinancialDateTime " +
                            ",CreatedByDocumentID,OriginalDocumentID,CreatedBy,CreatedDateTime,ModifiedBy,ModifiedDateTime,UpdateFromEntityID " +
                            ",Balance,CurrencyID,ExchangeRate,BalanceCompanyCurrency,TotalAmount,TotalAmountCompanyCurrency,TotalAmountBeforeTax " +
                            ",TotalAmountBeforeTaxCompanyCurrency,PaymentMethod,clientId,Remarks,PostingStatus,Proj) " +

                       "VALUES " +
                           "(@IsSystemVoided,@IsVoided,@IsValid,@DocumentType,@DocumentDisplayNumber,@FinancialDateTime,@CreatedByDocumentID,@OriginalDocumentID " +
                            ",@CreatedBy,@CreatedDateTime,@ModifiedBy,@ModifiedDateTime,@UpdateFromEntityID,@Balance,@CurrencyID,@ExchangeRate " +
                            ",@BalanceCompanyCurrency,@TotalAmount,@TotalAmountCompanyCurrency,@TotalAmountBeforeTax,@TotalAmountBeforeTaxCompanyCurrency, " +
                            "@PaymentMethod,@clientId,@Remarks,@PostingStatus,@Proj)";

            DocumentDisplayNumber = fixNulls(DocumentDisplayNumber);
            CreatedBy = fixNulls(CreatedBy);
            ModifiedBy = fixNulls(ModifiedBy);
            Remarks = fixNulls(Remarks);
            Proj = fixNulls(Proj);

            SqlConnection conn = new SqlConnection(database);
            conn.Open();


            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = query;

                cmd.Parameters.AddWithValue("@IsSystemVoided", IsSystemVoided);
                cmd.Parameters.AddWithValue("@IsVoided", IsVoided);
                cmd.Parameters.AddWithValue("@IsValid", IsValid);
                cmd.Parameters.AddWithValue("@DocumentType", DocumentType);
                cmd.Parameters.AddWithValue("@DocumentDisplayNumber", DocumentDisplayNumber);
                cmd.Parameters.AddWithValue("@FinancialDateTime", FinancialDateTime);
                cmd.Parameters.AddWithValue("@CreatedByDocumentID", CreatedByDocumentID);
                cmd.Parameters.AddWithValue("@OriginalDocumentID", OriginalDocumentID);
                cmd.Parameters.AddWithValue("@CreatedBy", CreatedBy);
                cmd.Parameters.AddWithValue("@CreatedDateTime", CreatedDateTime);
                cmd.Parameters.AddWithValue("@ModifiedBy", ModifiedBy);
                cmd.Parameters.AddWithValue("@ModifiedDateTime", ModifiedDateTime);
                cmd.Parameters.AddWithValue("@UpdateFromEntityID", UpdateFromEntityID);
                cmd.Parameters.AddWithValue("@Balance", Balance);
                cmd.Parameters.AddWithValue("@CurrencyID", CurrencyID);
                cmd.Parameters.AddWithValue("@ExchangeRate", ExchangeRate);
                cmd.Parameters.AddWithValue("@BalanceCompanyCurrency", BalanceCompanyCurrency);
                cmd.Parameters.AddWithValue("@TotalAmount", TotalAmount);
                cmd.Parameters.AddWithValue("@TotalAmountCompanyCurrency", TotalAmountCompanyCurrency);
                cmd.Parameters.AddWithValue("@TotalAmountBeforeTax", TotalAmountBeforeTax);
                cmd.Parameters.AddWithValue("@TotalAmountBeforeTaxCompanyCurrency", TotalAmountBeforeTaxCompanyCurrency);
                cmd.Parameters.AddWithValue("@PaymentMethod", PaymentMethod);
                cmd.Parameters.AddWithValue("@clientId", clientId);
                cmd.Parameters.AddWithValue("@Remarks", Remarks);
                cmd.Parameters.AddWithValue("@PostingStatus", PostingStatus);
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
        public int DocumentType { get; set; }
        public int OriginalDocumentID { get; set; }
        public int DocumentID { get; set; }
        public int PaymentMethod { get; set; }
        public bool IsSystemVoided { get; set; }
        public bool IsVoided { get; set; }
        public bool IsValid { get; set; }
        public string DocumentDisplayNumber { get; set; }
        public DateTime FinancialDateTime { get; set; }
        public int CreatedByDocumentID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public int UpdateFromEntityID { get; set; }
        public decimal Balance { get; set; }
        public int CurrencyID { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal BalanceCompanyCurrency { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalAmountCompanyCurrency { get; set; }
        public decimal TotalAmountBeforeTax { get; set; }
        public decimal TotalAmountBeforeTaxCompanyCurrency { get; set; }
        public int clientId { get; set; }
        public string Remarks { get; set; }
        public int PostingStatus { get; set; }
        public string Proj { get; set; }
    }
}