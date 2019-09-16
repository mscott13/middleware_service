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

            DocumentDisplayNumber = fixNulls(DocumentDisplayNumber);
            CreatedBy = fixNulls(CreatedBy);
            ModifiedBy = fixNulls(ModifiedBy);
            Remarks = fixNulls(Remarks);
            Proj = fixNulls(Proj);

            using (SqlConnection connection = new SqlConnection(Constants.DB_GENERIC))
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

                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IsSystemVoided", IsSystemVoided);
                    command.Parameters.AddWithValue("@IsVoided", IsVoided);
                    command.Parameters.AddWithValue("@IsValid", IsValid);
                    command.Parameters.AddWithValue("@DocumentType", DocumentType);
                    command.Parameters.AddWithValue("@DocumentDisplayNumber", DocumentDisplayNumber);
                    command.Parameters.AddWithValue("@FinancialDateTime", FinancialDateTime);
                    command.Parameters.AddWithValue("@CreatedByDocumentID", CreatedByDocumentID);
                    command.Parameters.AddWithValue("@OriginalDocumentID", OriginalDocumentID);
                    command.Parameters.AddWithValue("@CreatedBy", CreatedBy);
                    command.Parameters.AddWithValue("@CreatedDateTime", CreatedDateTime);
                    command.Parameters.AddWithValue("@ModifiedBy", ModifiedBy);
                    command.Parameters.AddWithValue("@ModifiedDateTime", ModifiedDateTime);
                    command.Parameters.AddWithValue("@UpdateFromEntityID", UpdateFromEntityID);
                    command.Parameters.AddWithValue("@Balance", Balance);
                    command.Parameters.AddWithValue("@CurrencyID", CurrencyID);
                    command.Parameters.AddWithValue("@ExchangeRate", ExchangeRate);
                    command.Parameters.AddWithValue("@BalanceCompanyCurrency", BalanceCompanyCurrency);
                    command.Parameters.AddWithValue("@TotalAmount", TotalAmount);
                    command.Parameters.AddWithValue("@TotalAmountCompanyCurrency", TotalAmountCompanyCurrency);
                    command.Parameters.AddWithValue("@TotalAmountBeforeTax", TotalAmountBeforeTax);
                    command.Parameters.AddWithValue("@TotalAmountBeforeTaxCompanyCurrency", TotalAmountBeforeTaxCompanyCurrency);
                    command.Parameters.AddWithValue("@PaymentMethod", PaymentMethod);
                    command.Parameters.AddWithValue("@clientId", clientId);
                    command.Parameters.AddWithValue("@Remarks", Remarks);
                    command.Parameters.AddWithValue("@PostingStatus", PostingStatus);
                    command.Parameters.AddWithValue("@Proj", Proj);
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