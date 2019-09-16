﻿using middleware_service.Other_Classes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace middleware_service.TableDependencyDefinitions
{
    class SqlNotify_ArInvoices
    {
        public void TransferToGeneric(string database)
        {
            using (SqlConnection connection = new SqlConnection(database))
            {
                string query = "INSERT INTO dbo.tblARInvoices " +
                        "(Invoice#,Batch#,Ref#,Date1,DebitGLID,Amount,ARBalance,CustomerID,Processed,Author,PO#,ToApply,AdjustmentAmount " +
                        ",GLUpdateDate,Job#,SalesmanID,DocType,DueDate,FCDate,DiscountDate,DTotal,NextPostingDate,notes,InvoiceCreationDate " +
                        ",Transferred,Quarter,Year,FeeTaxesId,relatedInvoice,DocId,LiqNum,NotificationId,LegalStatus,Form,ResolutionNum " +
                        ",Printed,ARGLID,Exported,ExportDate,InternationalID,InternationalAmount,FeeType,ExchangeRate,EndofPeriodInvoiced " +
                        ",Remarks,isvoided,canceledDate,canceledBy,subType,LicensevalidityHistoryID,LogId,AutoGenerated,reasonforCancellation " +
                        ",PurchaseOrder,ExportedToEpayment,EpaymentExportDate,InvoiceType,Proj,InvoiceCode,BalanceForward,IDINVC) " +


                        "VALUES(@Invoice#,@Batch#,@Ref#,@Date1,@DebitGLID,@Amount,@ARBalance,@CustomerID,@Processed,@Author,@PO#,@ToApply " +
                        ",@AdjustmentAmount, @GLUpdateDate, @Job#,@SalesmanID,@DocType,@DueDate,@FCDate,@DiscountDate,@DTotal,@NextPostingDate " +
                        ",@notes, @InvoiceCreationDate, @Transferred, @Quarter, @Year, @FeeTaxesId, @relatedInvoice, @DocId, @LiqNum, @NotificationId " +
                        ",@LegalStatus, @Form, @ResolutionNum, @Printed, @ARGLID, @Exported, @ExportDate, @InternationalID, @InternationalAmount, @FeeType " +
                        ",@ExchangeRate, @EndofPeriodInvoiced, @Remarks, @isvoided, @canceledDate, @canceledBy, @subType, @LicensevalidityHistoryID " +
                        ",@LogId, @AutoGenerated, @reasonforCancellation, @PurchaseOrder, @ExportedToEpayment, @EpaymentExportDate, @InvoiceType, @Proj " +
                        ",@InvoiceCode, @BalanceForward, @IDINVC)";

                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Invoice#", Invoice);
                    command.Parameters.AddWithValue("@Batch#", Batch);
                    command.Parameters.AddWithValue("@Ref#", FixNulls(Ref));
                    command.Parameters.AddWithValue("@Date1", FixDate(Date1));
                    command.Parameters.AddWithValue("@DebitGLID", DebitGLID);
                    command.Parameters.AddWithValue("@Amount", Amount);
                    command.Parameters.AddWithValue("@ARBalance", ARBalance);
                    command.Parameters.AddWithValue("@CustomerID", CustomerID);
                    command.Parameters.AddWithValue("@Processed", Processed);
                    command.Parameters.AddWithValue("@Author", FixNulls(Author));
                    command.Parameters.AddWithValue("@PO#", FixNulls(PO));
                    command.Parameters.AddWithValue("@ToApply", ToApply);
                    command.Parameters.AddWithValue("@AdjustmentAmount", AdjustmentAmount);
                    command.Parameters.AddWithValue("@GLUpdateDate", FixDate(GLUpdateDate));
                    command.Parameters.AddWithValue("@Job#", FixNulls(Job));
                    command.Parameters.AddWithValue("@SalesmanID", SalesmanID);
                    command.Parameters.AddWithValue("@DocType", DocType);
                    command.Parameters.AddWithValue("@DueDate", FixDate(DueDate));
                    command.Parameters.AddWithValue("@FCDate", FixDate(FCDate));
                    command.Parameters.AddWithValue("@DiscountDate", FixDate(DiscountDate));
                    command.Parameters.AddWithValue("@DTotal", DTotal);
                    command.Parameters.AddWithValue("@NextPostingDate", FixDate(NextPostingDate));
                    command.Parameters.AddWithValue("@notes", FixNulls(notes));
                    command.Parameters.AddWithValue("@InvoiceCreationDate", FixDate(InvoiceCreationDate));
                    command.Parameters.AddWithValue("@Transferred", Transferred);
                    command.Parameters.AddWithValue("@Quarter", Quarter);
                    command.Parameters.AddWithValue("@Year", Year);
                    command.Parameters.AddWithValue("@FeeTaxesId", FeeTaxesId);
                    command.Parameters.AddWithValue("@relatedInvoice", relatedInvoice);
                    command.Parameters.AddWithValue("@DocId", DocId);
                    command.Parameters.AddWithValue("@LiqNum", FixNulls(LiqNum));
                    command.Parameters.AddWithValue("@NotificationId", NotificationId);
                    command.Parameters.AddWithValue("@LegalStatus", LegalStatus);
                    command.Parameters.AddWithValue("@Form", FixNulls(Form));
                    command.Parameters.AddWithValue("@ResolutionNum", FixNulls(ResolutionNum));
                    command.Parameters.AddWithValue("@Printed", FixNulls(Printed));
                    command.Parameters.AddWithValue("@ARGLID", ARGLID);
                    command.Parameters.AddWithValue("@Exported", Exported);
                    command.Parameters.AddWithValue("@ExportDate", FixDate(ExportDate));
                    command.Parameters.AddWithValue("@InternationalID", InternationalID);
                    command.Parameters.AddWithValue("@InternationalAmount", InternationalAmount);
                    command.Parameters.AddWithValue("@FeeType", FixNulls(FeeType));
                    command.Parameters.AddWithValue("@ExchangeRate", ExchangeRate);
                    command.Parameters.AddWithValue("@EndofPeriodInvoiced", FixDate(EndofPeriodInvoiced));
                    command.Parameters.AddWithValue("@Remarks", FixNulls(Remarks));
                    command.Parameters.AddWithValue("@isvoided", isVoided);
                    command.Parameters.AddWithValue("@canceledDate", canceledDate);
                    command.Parameters.AddWithValue("@canceledBy", FixNulls(canceledBy));
                    command.Parameters.AddWithValue("@subType", subType);
                    command.Parameters.AddWithValue("@LicensevalidityHistoryID", LicensevalidityHistoryID);
                    command.Parameters.AddWithValue("@LogId", LogId);
                    command.Parameters.AddWithValue("@AutoGenerated", AutoGenerated);
                    command.Parameters.AddWithValue("@reasonforCancellation", FixNulls(reasonforCancellation));
                    command.Parameters.AddWithValue("@PurchaseOrder", FixNulls(PurchaseOrder));
                    command.Parameters.AddWithValue("@ExportedToEpayment", ExportedToEpayment);
                    command.Parameters.AddWithValue("@EpaymentExportDate", EpaymentExportDate);
                    command.Parameters.AddWithValue("@InvoiceType", InvoiceType);
                    command.Parameters.AddWithValue("@Proj", FixNulls(Proj));
                    command.Parameters.AddWithValue("@InvoiceCode", FixNulls(InvoiceCode));
                    command.Parameters.AddWithValue("@BalanceForward", BalanceForward);
                    command.Parameters.AddWithValue("@IDINVC", FixNulls(IDINVC));
                    command.ExecuteNonQuery();
                    Log.Save("Parallel transfer completed");
                }
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

        private string FixNulls(string input)
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
        public int ARInvoiceID { get; set; }
        public decimal Amount { get; set; }
        public int isVoided { get; set; }
        public string canceledBy { get; set; }
        public int CustomerID { get; set; }
        public string notes { get; set; }
        public string FeeType { get; set; }
        public int Invoice { get; set; }
        public int Batch { get; set; }
        public string Ref { get; set; }
        public DateTime Date1 { get; set; }
        public int DebitGLID { get; set; }
        public decimal ARBalance { get; set; }
        public bool Processed { get; set; }
        public string Author { get; set; }
        public string PO { get; set; }
        public decimal ToApply { get; set; }
        public decimal AdjustmentAmount { get; set; }
        public DateTime GLUpdateDate { get; set; }
        public string Job { get; set; }
        public int SalesmanID { get; set; }
        public int DocType { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime FCDate { get; set; }
        public DateTime DiscountDate { get; set; }
        public decimal DTotal { get; set; }
        public DateTime NextPostingDate { get; set; }
        public DateTime InvoiceCreationDate { get; set; }
        public bool Transferred { get; set; }
        public int Quarter { get; set; }
        public int Year { get; set; }
        public int FeeTaxesId { get; set; }
        public int relatedInvoice { get; set; }
        public int DocId { get; set; }
        public string LiqNum { get; set; }
        public int NotificationId { get; set; }
        public string LegalStatus { get; set; }
        public string Form { get; set; }
        public string ResolutionNum { get; set; }
        public string Printed { get; set; }
        public int ARGLID { get; set; }
        public bool Exported { get; set; }
        public DateTime ExportDate { get; set; }
        public int InternationalID { get; set; }
        public decimal InternationalAmount { get; set; }
        public decimal ExchangeRate { get; set; }
        public DateTime EndofPeriodInvoiced { get; set; }
        public string Remarks { get; set; }
        public DateTime canceledDate { get; set; }
        public int subType { get; set; }
        public int LicensevalidityHistoryID { get; set; }
        public int LogId { get; set; }
        public bool AutoGenerated { get; set; }
        public string reasonforCancellation { get; set; }
        public string PurchaseOrder { get; set; }
        public bool ExportedToEpayment { get; set; }
        public DateTime EpaymentExportDate { get; set; }
        public int InvoiceType { get; set; }
        public string Proj { get; set; }
        public string InvoiceCode { get; set; }
        public decimal BalanceForward { get; set; }
        public string IDINVC { get; set; }
    }
}


