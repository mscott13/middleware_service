using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Diagnostics;
using System.Drawing;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using System.Web.UI.WebControls;
using System.Globalization;



namespace WebApplication4
{
    public class Report
    {
        public DeferredData gen_rpt(string ReportType, Integration intlink, int action, int month, int year)
        {
            DateTime startDate = new DateTime(year, month, 1);
            DateTime endDate = new DateTime();
            if (ReportType == "Monthly") endDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));
            if (ReportType == "Annual") endDate = new DateTime(year+1, month-1, DateTime.DaysInMonth(year+1, month-1));

            List<ReportRawData> ReportInfo = intlink.getDIRInformation(ReportType, startDate, endDate);

            List<DataWrapper> ReportCategories = new List<DataWrapper>();

            DataWrapper cell_category = new DataWrapper();
            DataWrapper bband_category = new DataWrapper();
            DataWrapper micro_category = new DataWrapper();
            DataWrapper vsat_category = new DataWrapper();
            DataWrapper marine_category = new DataWrapper();
            DataWrapper dservices_category = new DataWrapper();
            DataWrapper aero_category = new DataWrapper();
            DataWrapper trunking_category = new DataWrapper();
            DataWrapper other_category = new DataWrapper();

            cell_category.label = "Cellular";
            bband_category.label = "P/R Commercial (Broadband)";
            micro_category.label = "P/R Commercial (Microwave)";
            vsat_category.label = "Vsat";
            marine_category.label = "P/R - Marine";
            dservices_category.label = "P/R Commercial (Data & Services)";
            aero_category.label = "P/R - Aeronautical";
            trunking_category.label = "P/R - Trunking";
            other_category.label = "Other P/R Non-Commercial Clients";

            List<String> ReportColumnNames = new List<string>();
            ReportColumnNames.Add("License Number");
            ReportColumnNames.Add("Client Company");
            ReportColumnNames.Add("Invoice ID");
            ReportColumnNames.Add("Budget");
            ReportColumnNames.Add("Invoice Total");
            if (ReportType == "Monthly") ReportColumnNames.Add("This Month's Invoice");
            if (ReportType == "Annual") ReportColumnNames.Add("This Year's Invoice");
            ReportColumnNames.Add("Balance B/FWD");
            ReportColumnNames.Add("From Revenue");
            ReportColumnNames.Add("To Revenue");
            ReportColumnNames.Add("Closing Balance");
            ReportColumnNames.Add("Total Months");
            ReportColumnNames.Add("Months Utilized");
            ReportColumnNames.Add("Months Remaining");
            ReportColumnNames.Add("Validity Period Start");
            ReportColumnNames.Add("Validity Period End");

            UIData record_info = new UIData();
            
            decimal OpeningBal = 0;
            decimal ClosingBal = 0;
            DateTime RecordsStartValPeriod = new DateTime();
            DateTime RecordsEndValPeriod = new DateTime();
            DateTime LastRptsStartValPeriod = new DateTime();
            DateTime LastRptsEndValPeriod = new DateTime();
            string ThisPeriodsInv = "";
            string clientCompany = "";
            string invoiceID = "";
            int TotalMonths = 0;
            int MonthsUtilized = 0;
            int MonthsRemaining = 0;
            decimal fromRevAmt = 0;
            decimal toRevAmt = 0;

            decimal cell_SubT_budget = 0;
            decimal cell_SubT_invoiceTotal = 0;
            decimal cell_SubT_balBFwd = 0;
            decimal cell_SubT_closingBal = 0;
            decimal cell_SubT_fromRev = 0;
            decimal cell_SubT_toRev = 0;

            decimal bband_SubT_budget = 0;
            decimal bband_SubT_invoiceTotal = 0;
            decimal bband_SubT_balBFwd = 0;
            decimal bband_SubT_closingBal = 0;
            decimal bband_SubT_fromRev = 0;
            decimal bband_SubT_toRev = 0;

            decimal micro_SubT_budget = 0;
            decimal micro_SubT_invoiceTotal = 0;
            decimal micro_SubT_balBFwd = 0;
            decimal micro_SubT_closingBal = 0;
            decimal micro_SubT_fromRev = 0;
            decimal micro_SubT_toRev = 0;

            decimal vsat_SubT_budget = 0;
            decimal vsat_SubT_invoiceTotal = 0;
            decimal vsat_SubT_balBFwd = 0;
            decimal vsat_SubT_closingBal = 0;
            decimal vsat_SubT_fromRev = 0;
            decimal vsat_SubT_toRev = 0;

            decimal marine_SubT_budget = 0;
            decimal marine_SubT_invoiceTotal = 0;
            decimal marine_SubT_balBFwd = 0;
            decimal marine_SubT_closingBal = 0;
            decimal marine_SubT_fromRev = 0;
            decimal marine_SubT_toRev = 0;

            decimal dservices_SubT_budget = 0;
            decimal dservices_SubT_invoiceTotal = 0;
            decimal dservices_SubT_balBFwd = 0;
            decimal dservices_SubT_closingBal = 0;
            decimal dservices_SubT_fromRev = 0;
            decimal dservices_SubT_toRev = 0;

            decimal aero_SubT_budget = 0;
            decimal aero_SubT_invoiceTotal = 0;
            decimal aero_SubT_balBFwd = 0;
            decimal aero_SubT_closingBal = 0;
            decimal aero_SubT_fromRev = 0;
            decimal aero_SubT_toRev = 0;

            decimal trunking_SubT_budget = 0;
            decimal trunking_SubT_invoiceTotal = 0;
            decimal trunking_SubT_balBFwd = 0;
            decimal trunking_SubT_closingBal = 0;
            decimal trunking_SubT_fromRev = 0;
            decimal trunking_SubT_toRev = 0;

            decimal other_SubT_budget = 0;
            decimal other_SubT_invoiceTotal = 0;
            decimal other_SubT_balBFwd = 0;
            decimal other_SubT_closingBal = 0;
            decimal other_SubT_fromRev = 0;
            decimal other_SubT_toRev = 0;

            decimal tot_budget = 0;
            decimal tot_invoiceTotal = 0;
            decimal tot_balBFwd = 0;
            decimal tot_closingBal = 0;
            decimal tot_fromRev = 0;
            decimal tot_toRev = 0;


            for (int i = 0; i < ReportInfo.Count; i++)
            {
                record_info = new UIData();
                OpeningBal = 0;
                fromRevAmt = 0;
                ThisPeriodsInv = "No";
                RecordsStartValPeriod = ReportInfo[i].CurrentStartValPeriod.Date;
                RecordsEndValPeriod = ReportInfo[i].CurrentEndValPeriod.Date;
                invoiceID = ReportInfo[i].ARInvoiceID;
                if (invoiceID == "0") invoiceID = "";

                if (invoiceID == "21749")
                {
                    string test = "test";
                }

                if (ReportInfo[i].ExistedBefore == 1)
                {
                    LastRptsStartValPeriod = DateTime.ParseExact(ReportInfo[i].LastRptsStartValPeriod, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    LastRptsEndValPeriod = DateTime.ParseExact(ReportInfo[i].LastRptsEndValPeriod, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    OpeningBal = Convert.ToDecimal(ReportInfo[i].LastRptsClosingBal);
                }

                if (ReportInfo[i].isCancelled == 1)
                {
                    RecordsStartValPeriod = LastRptsStartValPeriod;
                    RecordsEndValPeriod = LastRptsEndValPeriod;
                }

                if (((ReportInfo[i].isCreditMemo == 1 || ReportInfo[i].isCancelled == 1) && ReportInfo[i].ExistedBefore == 0) || 
                    (ReportInfo[i].ExistedBefore == 1 && Convert.ToDecimal(ReportInfo[i].LastRptsClosingBal) == 0 && 
                    LastRptsStartValPeriod == ReportInfo[i].CurrentStartValPeriod.Date && LastRptsEndValPeriod == ReportInfo[i].CurrentEndValPeriod.Date) ||
                    (ReportInfo[i].CurrentEndValPeriod.Year == year && ReportInfo[i].CurrentEndValPeriod.Month == month && ReportInfo[i].CurrentEndValPeriod.Day < 15 
                    && ReportInfo[i].CurrentEndValPeriod.Date > ReportInfo[i].InvoiceCreationDate.Date))
                    continue;

                if(ReportInfo[i].ExistedBefore == 0)
                {
                    ThisPeriodsInv = "Yes";
                    fromRevAmt = ReportInfo[i].InvAmount;
                }

                TotalMonths = getMonths(RecordsStartValPeriod, RecordsEndValPeriod);
                MonthsUtilized = getMonths(RecordsStartValPeriod, endDate);

                if (RecordsStartValPeriod.Day != 1 && RecordsStartValPeriod.Day <= 15) MonthsUtilized++;

                if (ReportInfo[i].notes == "Modification")
                {
                    TotalMonths = getMonths(ReportInfo[i].InvoiceCreationDate, RecordsEndValPeriod);
                    MonthsUtilized = getMonths(ReportInfo[i].InvoiceCreationDate, endDate);
                    if (ReportInfo[i].InvoiceCreationDate.Day != 1 && RecordsEndValPeriod.Day < 15) MonthsUtilized++;
                    if (DateTime.DaysInMonth(ReportInfo[i].InvoiceCreationDate.Year, ReportInfo[i].InvoiceCreationDate.Month) - ReportInfo[i].InvoiceCreationDate.Day + RecordsEndValPeriod.Day >= 45)
                    {
                        TotalMonths++;
                        MonthsUtilized++;
                    }
                }

                if (MonthsUtilized > TotalMonths) MonthsUtilized = TotalMonths;

                MonthsRemaining = TotalMonths - MonthsUtilized;

                ClosingBal = ReportInfo[i].InvAmount / TotalMonths * MonthsRemaining;
                
                clientCompany = ReportInfo[i].clientCompany;
                if (ReportInfo[i].clientCompany == "") clientCompany = ReportInfo[i].clientFname + " " + ReportInfo[i].clientLname;

                if (ReportInfo[i].isCancelled == 1)
                {
                    ClosingBal = 0;
                    fromRevAmt = -ReportInfo[i].InvAmount;                    
                }

                if (ReportInfo[i].isCreditMemo == 1)
                {
                    ClosingBal = 0;
                    fromRevAmt = -ReportInfo[i].CreditMemoAmt;
                    invoiceID = ReportInfo[i].ARInvoiceID.ToString() + "/" + ReportInfo[i].CreditMemoNum;
                }

                toRevAmt = ClosingBal - OpeningBal - fromRevAmt;

                record_info.licenseNumber = ReportInfo[i].ccNum;
                record_info.clientCompany = clientCompany;
                record_info.invoiceID = invoiceID;
                record_info.budget = formatMoney(Math.Round(ReportInfo[i].Budget, 2));
                record_info.invoiceTotal = formatMoney(Math.Round(ReportInfo[i].InvAmount, 2));
                record_info.thisPeriodsInv = ThisPeriodsInv;
                record_info.balBFwd = formatMoney(Math.Round(OpeningBal, 2));
                record_info.fromRev = formatMoney(Math.Round(fromRevAmt, 2));
                record_info.toRev = formatMoney(Math.Round(toRevAmt, 2));
                record_info.closingBal = formatMoney(Math.Round(ClosingBal, 2));
                record_info.totalMonths = TotalMonths;
                record_info.monthUtil = MonthsUtilized;
                record_info.monthRemain = MonthsRemaining;
                record_info.valPStart = RecordsStartValPeriod.Day.ToString("00") + "/" + RecordsStartValPeriod.Month.ToString("00") + "/" + RecordsStartValPeriod.Year.ToString();
                record_info.valPEnd = RecordsEndValPeriod.Day.ToString("00") + "/" + RecordsEndValPeriod.Month.ToString("00") + "/" + RecordsEndValPeriod.Year.ToString();

                if (ReportInfo[i].CreditGLID == 5156)
                {
                    cell_category.records.Add(record_info);
                    cell_SubT_budget = cell_SubT_budget + Math.Round(ReportInfo[i].Budget, 2);
                    cell_SubT_invoiceTotal = cell_SubT_invoiceTotal + Math.Round(ReportInfo[i].InvAmount, 2);
                    cell_SubT_balBFwd = cell_SubT_balBFwd + Math.Round(OpeningBal, 2);
                    cell_SubT_closingBal = cell_SubT_closingBal + Math.Round(ClosingBal, 2);
                    cell_SubT_fromRev = cell_SubT_fromRev + Math.Round(fromRevAmt, 2);
                    cell_SubT_toRev = cell_SubT_toRev + Math.Round(toRevAmt, 2);
                }

                if (ReportInfo[i].CreditGLID == 5157)
                {
                    bband_category.records.Add(record_info);
                    bband_SubT_budget = bband_SubT_budget + Math.Round(ReportInfo[i].Budget, 2);
                    bband_SubT_invoiceTotal = bband_SubT_invoiceTotal + Math.Round(ReportInfo[i].InvAmount, 2);
                    bband_SubT_balBFwd = bband_SubT_balBFwd + Math.Round(OpeningBal, 2);
                    bband_SubT_closingBal = bband_SubT_closingBal + Math.Round(ClosingBal, 2);
                    bband_SubT_fromRev = bband_SubT_fromRev + Math.Round(fromRevAmt, 2);
                    bband_SubT_toRev = bband_SubT_toRev + Math.Round(toRevAmt, 2);
                }

                if (ReportInfo[i].CreditGLID == 5158)
                {
                    micro_category.records.Add(record_info);
                    micro_SubT_budget = micro_SubT_budget + Math.Round(ReportInfo[i].Budget, 2);
                    micro_SubT_invoiceTotal = micro_SubT_invoiceTotal + Math.Round(ReportInfo[i].InvAmount, 2);
                    micro_SubT_balBFwd = micro_SubT_balBFwd + Math.Round(OpeningBal, 2);
                    micro_SubT_closingBal = micro_SubT_closingBal + Math.Round(ClosingBal, 2);
                    micro_SubT_fromRev = micro_SubT_fromRev + Math.Round(fromRevAmt, 2);
                    micro_SubT_toRev = micro_SubT_toRev + Math.Round(toRevAmt, 2);
                }

                if (ReportInfo[i].CreditGLID == 5160)
                {
                    vsat_category.records.Add(record_info);
                    vsat_SubT_budget = vsat_SubT_budget + Math.Round(ReportInfo[i].Budget, 2);
                    vsat_SubT_invoiceTotal = vsat_SubT_invoiceTotal + Math.Round(ReportInfo[i].InvAmount, 2);
                    vsat_SubT_balBFwd = vsat_SubT_balBFwd + Math.Round(OpeningBal, 2);
                    vsat_SubT_closingBal = vsat_SubT_closingBal + Math.Round(ClosingBal, 2);
                    vsat_SubT_fromRev = vsat_SubT_fromRev + Math.Round(fromRevAmt, 2);
                    vsat_SubT_toRev = vsat_SubT_toRev + Math.Round(toRevAmt, 2);
                }

                if (ReportInfo[i].CreditGLID == 5162)
                {
                    marine_category.records.Add(record_info);
                    marine_SubT_budget = marine_SubT_budget + Math.Round(ReportInfo[i].Budget, 2);
                    marine_SubT_invoiceTotal = marine_SubT_invoiceTotal + Math.Round(ReportInfo[i].InvAmount, 2);
                    marine_SubT_balBFwd = marine_SubT_balBFwd + Math.Round(OpeningBal, 2);
                    marine_SubT_closingBal = marine_SubT_closingBal + Math.Round(ClosingBal, 2);
                    marine_SubT_fromRev = marine_SubT_fromRev + Math.Round(fromRevAmt, 2);
                    marine_SubT_toRev = marine_SubT_toRev + Math.Round(toRevAmt, 2);
                }

                if (ReportInfo[i].CreditGLID == 5159)
                {
                    dservices_category.records.Add(record_info);
                    dservices_SubT_budget = dservices_SubT_budget + Math.Round(ReportInfo[i].Budget, 2);
                    dservices_SubT_invoiceTotal = dservices_SubT_invoiceTotal + Math.Round(ReportInfo[i].InvAmount, 2);
                    dservices_SubT_balBFwd = dservices_SubT_balBFwd + Math.Round(OpeningBal, 2);
                    dservices_SubT_closingBal = dservices_SubT_closingBal + Math.Round(ClosingBal, 2);
                    dservices_SubT_fromRev = dservices_SubT_fromRev + Math.Round(fromRevAmt, 2);
                    dservices_SubT_toRev = dservices_SubT_toRev + Math.Round(toRevAmt, 2);
                }

                if (ReportInfo[i].CreditGLID == 5161)
                {
                    aero_category.records.Add(record_info);
                    aero_SubT_budget = aero_SubT_budget + Math.Round(ReportInfo[i].Budget, 2);
                    aero_SubT_invoiceTotal = aero_SubT_invoiceTotal + Math.Round(ReportInfo[i].InvAmount, 2);
                    aero_SubT_balBFwd = aero_SubT_balBFwd + Math.Round(OpeningBal, 2);
                    aero_SubT_closingBal = aero_SubT_closingBal + Math.Round(ClosingBal, 2);
                    aero_SubT_fromRev = aero_SubT_fromRev + Math.Round(fromRevAmt, 2);
                    aero_SubT_toRev = aero_SubT_toRev + Math.Round(toRevAmt, 2);
                }

                if (ReportInfo[i].CreditGLID == 5163)
                {
                    trunking_category.records.Add(record_info);
                    trunking_SubT_budget = trunking_SubT_budget + Math.Round(ReportInfo[i].Budget, 2);
                    trunking_SubT_invoiceTotal = trunking_SubT_invoiceTotal + Math.Round(ReportInfo[i].InvAmount, 2);
                    trunking_SubT_balBFwd = trunking_SubT_balBFwd + Math.Round(OpeningBal, 2);
                    trunking_SubT_closingBal = trunking_SubT_closingBal + Math.Round(ClosingBal, 2);
                    trunking_SubT_fromRev = trunking_SubT_fromRev + Math.Round(fromRevAmt, 2);
                    trunking_SubT_toRev = trunking_SubT_toRev + Math.Round(toRevAmt, 2);
                }

                if (ReportInfo[i].CreditGLID == 5164)
                {
                    other_category.records.Add(record_info);
                    other_SubT_budget = other_SubT_budget + Math.Round(ReportInfo[i].Budget, 2);
                    other_SubT_invoiceTotal = other_SubT_invoiceTotal + Math.Round(ReportInfo[i].InvAmount, 2);
                    other_SubT_balBFwd = other_SubT_balBFwd + Math.Round(OpeningBal, 2);
                    other_SubT_closingBal = other_SubT_closingBal + Math.Round(ClosingBal, 2);
                    other_SubT_fromRev = other_SubT_fromRev + Math.Round(fromRevAmt, 2);
                    other_SubT_toRev = other_SubT_toRev + Math.Round(toRevAmt, 2);
                }

                tot_budget = tot_budget + Math.Round(ReportInfo[i].Budget, 2);
                tot_invoiceTotal = tot_invoiceTotal + Math.Round(ReportInfo[i].InvAmount, 2);
                tot_balBFwd = tot_balBFwd + Math.Round(OpeningBal, 2);
                tot_closingBal = tot_closingBal + Math.Round(ClosingBal, 2);
                tot_fromRev = tot_fromRev + Math.Round(fromRevAmt, 2);
                tot_toRev = tot_toRev + Math.Round(toRevAmt, 2);
            }

            //if (month == 3 && year == 2018)
            //{
            //    marine_SubT_balBFwd = 2181581.04M;
            //    other_SubT_balBFwd = 625410.33M;
            //    tot_balBFwd = 106986679.21M;
            //}

            cell_category.subT_budget = formatMoney(cell_SubT_budget);
            cell_category.subT_invoiceTotal = formatMoney(cell_SubT_invoiceTotal);
            cell_category.subT_balBFwd = formatMoney(cell_SubT_balBFwd);
            cell_category.subT_closingBal = formatMoney(cell_SubT_closingBal);
            cell_category.subT_fromRev = formatMoney(cell_SubT_fromRev);
            cell_category.subT_toRev = formatMoney(cell_SubT_toRev);

            bband_category.subT_budget = formatMoney(bband_SubT_budget);
            bband_category.subT_invoiceTotal = formatMoney(bband_SubT_invoiceTotal);
            bband_category.subT_balBFwd = formatMoney(bband_SubT_balBFwd);
            bband_category.subT_closingBal = formatMoney(bband_SubT_closingBal);
            bband_category.subT_fromRev = formatMoney(bband_SubT_fromRev);
            bband_category.subT_toRev = formatMoney(bband_SubT_toRev);

            micro_category.subT_budget = formatMoney(micro_SubT_budget);
            micro_category.subT_invoiceTotal = formatMoney(micro_SubT_invoiceTotal);
            micro_category.subT_balBFwd = formatMoney(micro_SubT_balBFwd);
            micro_category.subT_closingBal = formatMoney(micro_SubT_closingBal);
            micro_category.subT_fromRev = formatMoney(micro_SubT_fromRev);
            micro_category.subT_toRev = formatMoney(micro_SubT_toRev);

            vsat_category.subT_budget = formatMoney(vsat_SubT_budget);
            vsat_category.subT_invoiceTotal = formatMoney(vsat_SubT_invoiceTotal);
            vsat_category.subT_balBFwd = formatMoney(vsat_SubT_balBFwd);
            vsat_category.subT_closingBal = formatMoney(vsat_SubT_closingBal);
            vsat_category.subT_fromRev = formatMoney(vsat_SubT_fromRev);
            vsat_category.subT_toRev = formatMoney(vsat_SubT_toRev);

            marine_category.subT_budget = formatMoney(marine_SubT_budget);
            marine_category.subT_invoiceTotal = formatMoney(marine_SubT_invoiceTotal);
            marine_category.subT_balBFwd = formatMoney(marine_SubT_balBFwd);
            marine_category.subT_closingBal = formatMoney(marine_SubT_closingBal);
            marine_category.subT_fromRev = formatMoney(marine_SubT_fromRev);
            marine_category.subT_toRev = formatMoney(marine_SubT_toRev);

            dservices_category.subT_budget = formatMoney(dservices_SubT_budget);
            dservices_category.subT_invoiceTotal = formatMoney(dservices_SubT_invoiceTotal);
            dservices_category.subT_balBFwd = formatMoney(dservices_SubT_balBFwd);
            dservices_category.subT_closingBal = formatMoney(dservices_SubT_closingBal);
            dservices_category.subT_fromRev = formatMoney(dservices_SubT_fromRev);
            dservices_category.subT_toRev = formatMoney(dservices_SubT_toRev);

            aero_category.subT_budget = formatMoney(aero_SubT_budget);
            aero_category.subT_invoiceTotal = formatMoney(aero_SubT_invoiceTotal);
            aero_category.subT_balBFwd = formatMoney(aero_SubT_balBFwd);
            aero_category.subT_closingBal = formatMoney(aero_SubT_closingBal);
            aero_category.subT_fromRev = formatMoney(aero_SubT_fromRev);
            aero_category.subT_toRev = formatMoney(aero_SubT_toRev);

            trunking_category.subT_budget = formatMoney(trunking_SubT_budget);
            trunking_category.subT_invoiceTotal = formatMoney(trunking_SubT_invoiceTotal);
            trunking_category.subT_balBFwd = formatMoney(trunking_SubT_balBFwd);
            trunking_category.subT_closingBal = formatMoney(trunking_SubT_closingBal);
            trunking_category.subT_fromRev = formatMoney(trunking_SubT_fromRev);
            trunking_category.subT_toRev = formatMoney(trunking_SubT_toRev);

            other_category.subT_budget = formatMoney(other_SubT_budget);
            other_category.subT_invoiceTotal = formatMoney(other_SubT_invoiceTotal);
            other_category.subT_balBFwd = formatMoney(other_SubT_balBFwd);
            other_category.subT_closingBal = formatMoney(other_SubT_closingBal);
            other_category.subT_fromRev = formatMoney(other_SubT_fromRev);
            other_category.subT_toRev = formatMoney(other_SubT_toRev);

            
            ReportCategories.Add(cell_category);
            ReportCategories.Add(bband_category);
            ReportCategories.Add(micro_category);
            ReportCategories.Add(vsat_category);
            ReportCategories.Add(marine_category);
            ReportCategories.Add(dservices_category);
            ReportCategories.Add(aero_category);
            ReportCategories.Add(trunking_category);
            ReportCategories.Add(other_category);

            Totals ReportTotal = new Totals();
            ReportTotal.tot_budget = formatMoney(tot_budget);
            ReportTotal.tot_invoiceTotal = formatMoney(tot_invoiceTotal);
            ReportTotal.tot_balBFwd = formatMoney(tot_balBFwd);
            ReportTotal.tot_closingBal = formatMoney(tot_closingBal);
            ReportTotal.tot_toRev = formatMoney(tot_toRev);
            ReportTotal.tot_fromRev = formatMoney(tot_fromRev);
            
            DeferredData Report = new DeferredData();

            Report.ColumnNames = ReportColumnNames;
            Report.Categories = ReportCategories;
            Report.Total = ReportTotal;

            if (action == 0)
            {
                Report.report_id = intlink.saveReport(ReportType, ReportCategories, ReportTotal);
            }
            
            //createPdfReport(ReportType, Report, startDate);
            //createPdfTotalsReport(ReportType, Report, startDate);

            return Report;
        }

        public int getMonths(DateTime sdate, DateTime edate)
        {
            int months = ((edate.Year - sdate.Year) * 12) + edate.Month - sdate.Month;
            if (sdate.Day == 1 && edate.Day == DateTime.DaysInMonth(edate.Year, edate.Month)) months++;
            if (edate.Day == 28 && edate.Month == 2 && DateTime.IsLeapYear(edate.Year) && sdate.Day == 1 && sdate.Month == 3) months++; // facilitates asms not considering leap years
            return months;
        }

        bool IsEmpty(DataSet dataSet)
        {
            foreach (DataTable table in dataSet.Tables)
                if (table.Rows.Count != 0) return false;

            return true;
        }

        string formatMoney(decimal inputs)
        {
            string input = Convert.ToString(inputs);
            string neg = " ";
            if (input[0] == '-')
            {
                neg = input;
                input = input.TrimStart('-');
            }
            if (inputs == 0) input = "0.00";
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
            if (neg[0] == '-')
                return "(" + formatted + ")";
            else return formatted;

        }

        public void createPdfReport(string ReportType, DeferredData Report, DateTime startDate)
        {
            string path_local = @"C:\Program Files (x86)\M.S\SMA Middleware\resources\reports\" + ReportType + "DefferedIncomeReport" + startDate.Year.ToString() + startDate.Month.ToString("00") + "01.pdf";

            // Document doc = new Document(iTextSharp.text.PageSize._11X17, 10, 10, 42, 35);
            Document doc = new Document(iTextSharp.text.PageSize.LEDGER);
            try
            {
                PdfWriter deffered = PdfWriter.GetInstance(doc, new FileStream(path_local, FileMode.OpenOrCreate));
            }

            catch (Exception ex)
            {
                //handle exception here
            }

            Paragraph paragraph = new Paragraph();
            Paragraph newLine = new Paragraph("\n");

            PdfPTable table = new PdfPTable(Report.ColumnNames.Count);

            doc.Open();

            var imagePath = @"C:\Program Files (x86)\M.S\SMA Middleware\resources\spec.jpg";
            iTextSharp.text.Image PNG = iTextSharp.text.Image.GetInstance(imagePath);
            PNG.ScaleToFit(100f, 100f);
            PNG.Alignment = 1;
            doc.Add(PNG);
            doc.Add(newLine);
            if (ReportType == "Monthly") paragraph = new Paragraph(new Phrase("Monthly Deferred Income Report For " + startDate.ToString("MMMM") + " " + startDate.Year.ToString(), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 14f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            if (ReportType == "Annual") paragraph = new Paragraph(new Phrase("Annual Deferred Income Report For Fiscal Year " + startDate.Year.ToString() + "/" + (startDate.Year + 1).ToString(), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 14f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            paragraph.Alignment = 1;
            doc.Add(paragraph);
            doc.Add(newLine);
            doc.Add(newLine);

            for (int i = 0; i < Report.Categories.Count; i++)
            {
                paragraph = new Paragraph(new Phrase(Report.Categories[i].label, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                paragraph.Alignment = 1; paragraph.SpacingAfter = 6f;
                doc.Add(paragraph);
                doc.Add(newLine);

                table = new PdfPTable(Report.ColumnNames.Count);
                for (int j = 0; j < Report.ColumnNames.Count; j++)
                {
                    table.AddCell(new Phrase(Report.ColumnNames[j], new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                }
                table.HeaderRows = 1;

                for (int j = 0; j < Report.Categories[i].records.Count; j++)
                {
                    table.AddCell(new Phrase(Report.Categories[i].records[j].licenseNumber, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                    table.AddCell(new Phrase(Report.Categories[i].records[j].clientCompany, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                    table.AddCell(new Phrase(Report.Categories[i].records[j].invoiceID, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                    table.AddCell(new Phrase(Report.Categories[i].records[j].budget, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                    table.AddCell(new Phrase(Report.Categories[i].records[j].invoiceTotal, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                    table.AddCell(new Phrase(Report.Categories[i].records[j].thisPeriodsInv, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                    table.AddCell(new Phrase(Report.Categories[i].records[j].balBFwd, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                    table.AddCell(new Phrase(Report.Categories[i].records[j].fromRev, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                    table.AddCell(new Phrase(Report.Categories[i].records[j].toRev, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                    table.AddCell(new Phrase(Report.Categories[i].records[j].closingBal, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                    table.AddCell(new Phrase(Report.Categories[i].records[j].totalMonths.ToString(), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                    table.AddCell(new Phrase(Report.Categories[i].records[j].monthUtil.ToString(), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                    table.AddCell(new Phrase(Report.Categories[i].records[j].monthRemain.ToString(), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                    table.AddCell(new Phrase(Report.Categories[i].records[j].valPStart, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                    table.AddCell(new Phrase(Report.Categories[i].records[j].valPEnd, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                }

                table.AddCell(new Phrase("SubTotal", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                table.AddCell(new Phrase("", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                table.AddCell(new Phrase("", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                table.AddCell(new Phrase(Report.Categories[i].subT_budget, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                table.AddCell(new Phrase(Report.Categories[i].subT_invoiceTotal, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                table.AddCell(new Phrase("", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                table.AddCell(new Phrase(Report.Categories[i].subT_balBFwd, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                table.AddCell(new Phrase(Report.Categories[i].subT_fromRev, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                table.AddCell(new Phrase(Report.Categories[i].subT_toRev, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                table.AddCell(new Phrase(Report.Categories[i].subT_closingBal, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                table.AddCell(new Phrase("", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                table.AddCell(new Phrase("", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                table.AddCell(new Phrase("", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                table.AddCell(new Phrase("", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                table.AddCell(new Phrase("", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

                doc.Add(table);
                doc.Add(newLine);
                doc.Add(newLine);
            }

            table = new PdfPTable(Report.ColumnNames.Count);
            for (int i = 0; i < Report.ColumnNames.Count; i++)
            {
                table.AddCell(new Phrase(Report.ColumnNames[i], new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            }
            table.HeaderRows = 1;

            table.AddCell(new Phrase("Total", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            table.AddCell(new Phrase("", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            table.AddCell(new Phrase("", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            table.AddCell(new Phrase(Report.Total.tot_budget, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            table.AddCell(new Phrase(Report.Total.tot_invoiceTotal, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            table.AddCell(new Phrase("", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            table.AddCell(new Phrase(Report.Total.tot_balBFwd, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            table.AddCell(new Phrase(Report.Total.tot_fromRev, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            table.AddCell(new Phrase(Report.Total.tot_toRev, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            table.AddCell(new Phrase(Report.Total.tot_closingBal, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            table.AddCell(new Phrase("", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            table.AddCell(new Phrase("", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            table.AddCell(new Phrase("", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            table.AddCell(new Phrase("", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            table.AddCell(new Phrase("", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            doc.Add(table);
            doc.Close();
        }

        public void createPdfTotalsReport(String ReportType, DeferredData Report, DateTime startDate)
        {
            string path_local = @"C:\Program Files (x86)\M.S\SMA Middleware\resources\reports\" + ReportType + "DefferedIncomeSummaryReport" + startDate.Year.ToString() + startDate.Month.ToString("00") + "01.pdf";

            // Document doc = new Document(iTextSharp.text.PageSize._11X17, 10, 10, 42, 35);
            Document doc = new Document(iTextSharp.text.PageSize.LETTER, 0, 0, 20, 0);
            try
            {
                PdfWriter deffered = PdfWriter.GetInstance(doc, new FileStream(path_local, FileMode.OpenOrCreate));
            }

            catch (Exception ex)
            {
                //handle exception here
            }

            Paragraph paragraph = new Paragraph();
            Paragraph newLine = new Paragraph("\n");

            PdfPTable table = new PdfPTable(7);

            doc.Open();

            doc.Add(newLine);
            doc.Add(newLine);
            doc.Add(newLine);

            var imagePath = @"C:\Program Files (x86)\M.S\SMA Middleware\resources\spec.jpg";
            iTextSharp.text.Image PNG = iTextSharp.text.Image.GetInstance(imagePath);
            PNG.ScaleToFit(100f, 100f);
            PNG.Alignment = 1;
            doc.Add(PNG);
            if (ReportType == "Monthly") paragraph = new Paragraph(new Phrase("Monthly Deferred Income Summary Report For " + startDate.ToString("MMMM") + " " + startDate.Year.ToString(), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 13f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            if (ReportType == "Annual") paragraph = new Paragraph(new Phrase("Annual Deferred Income Summary Report For Fiscal Year " + startDate.Year.ToString() + "/" + (startDate.Year + 1).ToString(), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 13f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            paragraph.Alignment = 1;
            doc.Add(paragraph);
            doc.Add(newLine);
            doc.Add(newLine);

            table.AddCell(new Phrase("Category", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            table.AddCell(new Phrase("Budget Total", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            table.AddCell(new Phrase("Invoice Total", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            table.AddCell(new Phrase("Balance B/FWD", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            table.AddCell(new Phrase("From Revenue", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            table.AddCell(new Phrase("To Revenue", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            table.AddCell(new Phrase("Balance C/FWD", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            for (int i = 0; i < Report.Categories.Count; i++)
            {
                table.AddCell(new Phrase(Report.Categories[i].label, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                table.AddCell(new Phrase(Report.Categories[i].subT_budget, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                table.AddCell(new Phrase(Report.Categories[i].subT_invoiceTotal, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                table.AddCell(new Phrase(Report.Categories[i].subT_balBFwd, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                table.AddCell(new Phrase(Report.Categories[i].subT_fromRev, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                table.AddCell(new Phrase(Report.Categories[i].subT_toRev, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                table.AddCell(new Phrase(Report.Categories[i].subT_closingBal, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            }

            table.AddCell(new Phrase("Total", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            table.AddCell(new Phrase(Report.Total.tot_budget, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            table.AddCell(new Phrase(Report.Total.tot_invoiceTotal, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            table.AddCell(new Phrase(Report.Total.tot_balBFwd, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            table.AddCell(new Phrase(Report.Total.tot_fromRev, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            table.AddCell(new Phrase(Report.Total.tot_toRev, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            table.AddCell(new Phrase(Report.Total.tot_closingBal, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            doc.Add(table);
            doc.Close();
        }
    }
}