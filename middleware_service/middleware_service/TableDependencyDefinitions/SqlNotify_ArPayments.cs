using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace middleware_service.TableDependencyDefinitions
{
    class SqlNotify_ArPayments
    {
        public void TransferToGeneric(string databaseName)
        {
            string query = "";
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


