
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Timers;
using System.Data;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Diagnostics;
using System.Drawing;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using System.Text;
using System.Net;
using System.Configuration;




namespace _000
{
    public partial class WebForm1 : System.Web.UI.Page
    {

        decimal budgettotal;
        decimal invoiceTotal = 0;
        decimal fromRev = 0;
        decimal toRev = 0;
        decimal closeBal = 0;
        decimal balancebf = 0;
        decimal invoiceTotalForYes = 0;


        decimal invoicetotcell = 0;
        decimal fromremcell = 0;
        decimal torevcell = 0;
        decimal closingtotcell = 0;
        decimal balancebfcell = 0;
        decimal budgettotcell = 0;


        decimal invoicetotbb = 0;
        decimal fromrembb = 0;
        decimal torevbb = 0;
        decimal closingtotbb = 0;
        decimal balancebfbb = 0;
        decimal budgettotbb = 0;


        decimal invoicetotmicro = 0;
        decimal fromrevmicro = 0;
        decimal torevmicro = 0;
        decimal closingtotmicro = 0;
        decimal balancebfmicro = 0;
        decimal budgettotmic = 0;


        decimal invoicetottrunk = 0;
        decimal fromrevtrunk = 0;
        decimal torevtrunk = 0;
        decimal closingtottrunk = 0;
        decimal balancebftrunk = 0;
        decimal budgettottrunk = 0;


        decimal invoicetotaero = 0;
        decimal fromrevaero = 0;
        decimal torevaero = 0;
        decimal closingtotaero = 0;
        decimal balancebfaero = 0;
        decimal budgettotaero = 0;

        decimal invoicetotds = 0;
        decimal fromrevds = 0;
        decimal torevds = 0;
        decimal closingtotds = 0;
        decimal balancebfds = 0;
        decimal budgettotds = 0;

        decimal invoicetotmar = 0;
        decimal fromrevmar = 0;
        decimal torevmar = 0;
        decimal closingtotmar = 0;
        decimal balancebfmar = 0;
        decimal budgettotmar = 0;


        decimal invoicetotother = 0;
        decimal fromrevother = 0;
        decimal torevother = 0;
        decimal closingtotother = 0;
        decimal balancebfother = 0;
        decimal budgettotother = 0;


        decimal invoicetotvsat = 0;
        decimal fromrevvsat = 0;
        decimal torevvsat = 0;
        decimal closingtotvsat = 0;
        decimal balancebfvsat = 0;
        decimal budgettotvsat = 0;

        public void getdetails1()
        {
            string month = "";

            if (ddl1.SelectedValue == "1")
            {
                month = "January";
            }

            else if (ddl1.SelectedValue == "2")
            {
                month = "February";
            }

            else if (ddl1.SelectedValue == "3")
            {
                month = "March";
            }

            else if (ddl1.SelectedValue == "4")
            {
                month = "April";
            }

            else if (ddl1.SelectedValue == "5")
            {
                month = "May";
            }

            else if (ddl1.SelectedValue == "6")
            {
                month = "June";
            }

            else if (ddl1.SelectedValue == "7")
            {
                month = "July";
            }

            else if (ddl1.SelectedValue == "8")
            {
                month = "August";
            }

            else if (ddl1.SelectedValue == "9")
            {
                month = "September";
            }

            else if (ddl1.SelectedValue == "10")
            {
                month = "October";
            }

            else if (ddl1.SelectedValue == "11")
            {
                month = "November";
            }

            else if (ddl1.SelectedValue == "12")
            {
                month = "December";
            }


            int year = 0;

            if (DropDownList1.SelectedValue == "2016")
            {
                year = 2016;
            }
            else if (DropDownList1.SelectedValue == "2017")
            {
                year = 2017;
            }

            else if (DropDownList1.SelectedValue == "2018")
            {
                year = 2018;
            }

            else if (DropDownList1.SelectedValue == "2019")
            {
                year = 2019;
            }

            else if (DropDownList1.SelectedValue == "2020")
            {
                year = 2020;
            }

            DataSet ds = generateDeferredIncome();
            DataTable cell = ds.Tables[0];
            DataTable vsat = ds.Tables[1];
            DataTable mar = ds.Tables[2];
            DataTable dservice = ds.Tables[3];
            DataTable table = ds.Tables[4];
            DataTable aero = ds.Tables[5];
            DataTable trunk = ds.Tables[6];
            DataTable other = ds.Tables[7];
            DataTable bb = ds.Tables[8];
            DataTable micro = ds.Tables[9];


            CreateDeferredTotals();

            string now = DateTime.Now.ToString();
            string FormatDate = DateTime.Now.ToString("dddd");
            string FormatDate1 = DateTime.Now.ToString("MMMM");
            string FormatDate2 = DateTime.Now.ToString("yyyy");
            string FormatDate3 = DateTime.Now.ToString("HH");
            string FormatDate4 = DateTime.Now.ToString("mm");
            string FormatDate5 = DateTime.Now.ToString("tt");

            DataTable dt;
            dt = new DataTable();

            dt.Columns.Add("License Number", typeof(string));
            dt.Columns.Add("Client Company", typeof(string));
            dt.Columns.Add("Invoice ID", typeof(string));
            dt.Columns.Add("Budget", typeof(string));
            dt.Columns.Add("Invoice Total", typeof(string));
            dt.Columns.Add("This Month Invoice", typeof(string));
            dt.Columns.Add("Balance B/FWD", typeof(string));
            dt.Columns.Add("From Revenue", typeof(string));
            dt.Columns.Add("To Revenue", typeof(string));
            dt.Columns.Add("Closing Balance", typeof(string));
            dt.Columns.Add("Total Months", typeof(int));
            dt.Columns.Add("Month Utilized", typeof(int));
            dt.Columns.Add("Months Remaining", typeof(int));
            dt.Columns.Add("Validity Period Start", typeof(string));
            dt.Columns.Add("Validity Period End", typeof(string));

            dt.Rows.Add("Total", "             ", "     ", formatMoney(Math.Round(budgettotal, 2)), formatMoney(Math.Round(invoiceTotal, 2)), " ", formatMoney(Math.Round(balancebf, 2)), formatMoney(Math.Round(fromRev, 2)), formatMoney(Math.Round(-toRev, 2)), formatMoney(Math.Round(closeBal, 2)), 0, 0, 0/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, DateTime.Now.ToString("MM/dd/yyyy"), DateTime.Now.ToString("MM/dd/yyyy"));

            GridView2.DataSource = dt;
            GridView2.DataBind();

            CellGrid.DataSource = cell;
            CellGrid.DataBind();

            VsatGrid.DataSource = vsat;
            VsatGrid.DataBind();
          
            TrunkGrid.DataSource = trunk;
            TrunkGrid.DataBind();

            MarineGrid.DataSource = mar;
            MarineGrid.DataBind();

            AeroGrid.DataSource = aero;
            AeroGrid.DataBind();

            DServicesGrid.DataSource = dservice;
            DServicesGrid.DataBind();

            OtherGrid.DataSource = other;
            OtherGrid.DataBind();

            BBGrid.DataSource = bb;
            BBGrid.DataBind();

            MicroGrid.DataSource = micro;
            MicroGrid.DataBind();


            if (table.Columns.Count > 0)
            {
                // var Testpath = @"C:\\Users\\asms-accpac-1\\Documents\\" + "DefferedincomeReport_Generated" + "_" + "On" + "_" + FormatDate + " " + FormatDate1 + " " + FormatDate2 + " " + "at" + " " + FormatDate3 + " " + FormatDate4 + " " + FormatDate5 + ".pdf";

                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var Testpath = @"C:\\Users\\asms-accpac-1\\Documents\\" + "DefferedincomeReportFor" + month + " " + year + ".pdf";
                // Document doc = new Document(iTextSharp.text.PageSize._11X17, 10, 10, 42, 35);
                Document doc = new Document(iTextSharp.text.PageSize.LEDGER);
                try
                {
                    PdfWriter deffered = PdfWriter.GetInstance(doc, new FileStream(Testpath, FileMode.OpenOrCreate));

                }

                catch (Exception ex)
                {
                    lblError.Text = "";
                }

                doc.Open();
                PdfPTable defferecdtable = new PdfPTable(table.Columns.Count);
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    defferecdtable.AddCell(new Phrase(table.Columns[j].ColumnName, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                }
                defferecdtable.HeaderRows = 1;

                for (int i = 0; i < GridView1.Rows.Count; i++)
                {
                    for (int k = 0; k < table.Columns.Count; k++)
                    {
                        if (GridView1.Rows[i].Cells[k] != null)
                        {
                            defferecdtable.AddCell(new Phrase(HttpUtility.HtmlDecode(GridView1.Rows[i].Cells[k].Text), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                        }
                    }
                }




                PdfPTable celltable = new PdfPTable(cell.Columns.Count);
                for (int j = 0; j < cell.Columns.Count; j++)
                {
                    celltable.AddCell(new Phrase(cell.Columns[j].ColumnName, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                }
                celltable.HeaderRows = 1;

                for (int i = 0; i < CellGrid.Rows.Count; i++)
                {
                    for (int k = 0; k < cell.Columns.Count; k++)
                    {
                        if (CellGrid.Rows[i].Cells[k] != null)
                        {
                            celltable.AddCell(new Phrase(HttpUtility.HtmlDecode(CellGrid.Rows[i].Cells[k].Text), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                        }
                    }
                }



                PdfPTable bbtable = new PdfPTable(bb.Columns.Count);
                for (int j = 0; j < bb.Columns.Count; j++)
                {
                    bbtable.AddCell(new Phrase(bb.Columns[j].ColumnName, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                }
                bbtable.HeaderRows = 1;

                for (int i = 0; i < BBGrid.Rows.Count; i++)
                {
                    for (int k = 0; k < bb.Columns.Count; k++)
                    {
                        if (BBGrid.Rows[i].Cells[k] != null)
                        {
                            bbtable.AddCell(new Phrase(HttpUtility.HtmlDecode(BBGrid.Rows[i].Cells[k].Text), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                        }
                    }
                }




                PdfPTable martable = new PdfPTable(mar.Columns.Count);
                for (int j = 0; j < mar.Columns.Count; j++)
                {
                    martable.AddCell(new Phrase(mar.Columns[j].ColumnName, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                }
                martable.HeaderRows = 1;

                for (int i = 0; i < MarineGrid.Rows.Count; i++)
                {
                    for (int k = 0; k < mar.Columns.Count; k++)
                    {
                        if (MarineGrid.Rows[i].Cells[k] != null)
                        {
                            martable.AddCell(new Phrase(HttpUtility.HtmlDecode(MarineGrid.Rows[i].Cells[k].Text), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                        }
                    }
                }



                PdfPTable Trunktable = new PdfPTable(trunk.Columns.Count);
                for (int j = 0; j < trunk.Columns.Count; j++)
                {
                    Trunktable.AddCell(new Phrase(trunk.Columns[j].ColumnName, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                }
                Trunktable.HeaderRows = 1;

                for (int i = 0; i < TrunkGrid.Rows.Count; i++)
                {
                    for (int k = 0; k < trunk.Columns.Count; k++)
                    {
                        if (TrunkGrid.Rows[i].Cells[k] != null)
                        {
                            Trunktable.AddCell(new Phrase(HttpUtility.HtmlDecode(TrunkGrid.Rows[i].Cells[k].Text), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                        }
                    }
                }




                PdfPTable vsattable = new PdfPTable(vsat.Columns.Count);
                for (int j = 0; j < vsat.Columns.Count; j++)
                {
                    vsattable.AddCell(new Phrase(vsat.Columns[j].ColumnName, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                }
                vsattable.HeaderRows = 1;

                for (int i = 0; i < VsatGrid.Rows.Count; i++)
                {
                    for (int k = 0; k < vsat.Columns.Count; k++)
                    {
                        if (VsatGrid.Rows[i].Cells[k] != null)
                        {
                            vsattable.AddCell(new Phrase(HttpUtility.HtmlDecode(VsatGrid.Rows[i].Cells[k].Text), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                        }
                    }
                }



                PdfPTable aerotable = new PdfPTable(aero.Columns.Count);
                for (int j = 0; j < aero.Columns.Count; j++)
                {
                    aerotable.AddCell(new Phrase(aero.Columns[j].ColumnName, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                }
                aerotable.HeaderRows = 1;

                for (int i = 0; i < AeroGrid.Rows.Count; i++)
                {
                    for (int k = 0; k < aero.Columns.Count; k++)
                    {
                        if (AeroGrid.Rows[i].Cells[k] != null)
                        {
                            aerotable.AddCell(new Phrase(HttpUtility.HtmlDecode(AeroGrid.Rows[i].Cells[k].Text), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                        }
                    }
                }




                PdfPTable microtable = new PdfPTable(micro.Columns.Count);
                for (int j = 0; j < micro.Columns.Count; j++)
                {
                    microtable.AddCell(new Phrase(micro.Columns[j].ColumnName, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                }
                microtable.HeaderRows = 1;

                for (int i = 0; i < MicroGrid.Rows.Count; i++)
                {
                    for (int k = 0; k < micro.Columns.Count; k++)
                    {
                        if (MicroGrid.Rows[i].Cells[k] != null)
                        {
                            microtable.AddCell(new Phrase(HttpUtility.HtmlDecode(MicroGrid.Rows[i].Cells[k].Text), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                        }
                    }
                }




                PdfPTable dstable = new PdfPTable(dservice.Columns.Count);
                for (int j = 0; j < dservice.Columns.Count; j++)
                {
                    dstable.AddCell(new Phrase(dservice.Columns[j].ColumnName, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                }
                dstable.HeaderRows = 1;

                for (int i = 0; i < DServicesGrid.Rows.Count; i++)
                {
                    for (int k = 0; k < dservice.Columns.Count; k++)
                    {
                        if (DServicesGrid.Rows[i].Cells[k] != null)
                        {
                            dstable.AddCell(new Phrase(HttpUtility.HtmlDecode(DServicesGrid.Rows[i].Cells[k].Text), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                        }
                    }
                }




                PdfPTable othertable = new PdfPTable(other.Columns.Count);
                for (int j = 0; j < other.Columns.Count; j++)
                {
                    othertable.AddCell(new Phrase(other.Columns[j].ColumnName, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                }
                othertable.HeaderRows = 1;

                for (int i = 0; i < OtherGrid.Rows.Count; i++)
                {
                    for (int k = 0; k < other.Columns.Count; k++)
                    {
                        if (OtherGrid.Rows[i].Cells[k] != null)
                        {
                            othertable.AddCell(new Phrase(HttpUtility.HtmlDecode(OtherGrid.Rows[i].Cells[k].Text), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                        }
                    }
                }


                PdfPTable total = new PdfPTable(15);
                //Headings

                total.AddCell(new Phrase("License Number", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                total.AddCell(new Phrase("Client Company", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                total.AddCell(new Phrase("Invoice ID", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));


                total.AddCell(new Phrase("Budget", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

                total.AddCell(new Phrase("Invoice Total", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

                total.AddCell(new Phrase("This Month Invoice", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

                total.AddCell(new Phrase("Balance B/FWD", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

                total.AddCell(new Phrase("From Revenue", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

                total.AddCell(new Phrase("To Revenue", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

                total.AddCell(new Phrase("Closing Balance", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

                total.AddCell(new Phrase("Total Months", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

                total.AddCell(new Phrase("Months Utilized", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

                total.AddCell(new Phrase("Months Remaining", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

                total.AddCell(new Phrase("Validity Period Start", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

                total.AddCell(new Phrase("Validity Period End", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));


                total.AddCell(new Phrase("Total ", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                total.AddCell(new Phrase(" ", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                total.AddCell(new Phrase(" ", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));


                total.AddCell(new Phrase(formatMoney(Math.Round(budgettotal, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

                total.AddCell(new Phrase(formatMoney(Math.Round(invoiceTotal, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

                total.AddCell(new Phrase(" ", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

                total.AddCell(new Phrase(formatMoney(Math.Round(balancebf, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

                total.AddCell(new Phrase(formatMoney(Math.Round(fromRev, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

                total.AddCell(new Phrase(formatMoney(Math.Round(-toRev, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

                total.AddCell(new Phrase(formatMoney(Math.Round(closeBal, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

                total.AddCell(new Phrase(" ", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

                total.AddCell(new Phrase(" ", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

                total.AddCell(new Phrase(" ", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

                total.AddCell(new Phrase(DateTime.Now.ToString(), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

                total.AddCell(new Phrase(DateTime.Now.ToString(), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));


                Paragraph space = new Paragraph("\n");

                var imagePath = Server.MapPath("~/spec.jpg");
                iTextSharp.text.Image PNG = iTextSharp.text.Image.GetInstance(imagePath);
                PNG.ScaleToFit(100f, 100f);
                PNG.Alignment = 1;
                Paragraph paragraph4 = new Paragraph(new Phrase("Spectrum Management Authority Deferred Income Report For the Month of " + month + " " + year, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                paragraph4.Alignment = 1;
                doc.Add(paragraph4);
                doc.Add(PNG);

                Paragraph invoice = new Paragraph((new Phrase("Invoices Total: " + Math.Round(invoiceTotal, 2), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.RED))));
                invoice.Alignment = 1;
                Paragraph from = new Paragraph((new Phrase("From Revenue Total: " + Math.Round(fromRev, 2), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.RED))));
                from.Alignment = 1;
                Paragraph to = new Paragraph((new Phrase("To Revenue Total: " + Math.Round(-toRev, 2), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.RED))));
                to.Alignment = 1;
                Paragraph closing = new Paragraph((new Phrase("Closing balance Total: " + Math.Round(closeBal, 2), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.RED))));
                closing.Alignment = 1;

                Paragraph paragraphcell = new Paragraph(new Phrase("Cellular", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                paragraphcell.Alignment = 1; paragraphcell.SpacingAfter = 6f;
                doc.Add(paragraphcell);
                doc.Add(space);
                doc.Add(celltable);
                doc.Add(space);
                doc.Add(space);
                Paragraph paragraphb = new Paragraph(new Phrase("P/R Commercial (Broadband)", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                paragraphb.Alignment = 1; paragraphb.SpacingAfter = 6f;

                doc.Add(paragraphb);
                doc.Add(space);
                doc.Add(bbtable);
                doc.Add(space);
                doc.Add(space);

                Paragraph paragraphm = new Paragraph(new Phrase("P/R Commercial (Microwave)", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                paragraphm.Alignment = 1; paragraphm.SpacingAfter = 6f;

                doc.Add(paragraphm);
                doc.Add(space);
                doc.Add(microtable);
                doc.Add(space);
                doc.Add(space);
                Paragraph paragraphvsat = new Paragraph(new Phrase("Vsat", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                paragraphvsat.Alignment = 1; paragraphvsat.SpacingAfter = 6f;

                doc.Add(paragraphvsat);
                doc.Add(space);
                doc.Add(vsattable);
                doc.Add(space);
                doc.Add(space);

                Paragraph paragraphtrunk = new Paragraph(new Phrase("P/R - Trunking", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                paragraphtrunk.Alignment = 1; paragraphtrunk.SpacingAfter = 6f;

                doc.Add(paragraphtrunk);
                doc.Add(space);
                doc.Add(Trunktable);
                doc.Add(space);
                doc.Add(space);

                Paragraph paragraphds = new Paragraph(new Phrase("P/R Commercial (Data & Services)", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                paragraphds.Alignment = 1; paragraphds.SpacingAfter = 6f;

                doc.Add(paragraphds);
                doc.Add(space);
                doc.Add(dstable);
                doc.Add(space);
                doc.Add(space);

                Paragraph paragraphaero = new Paragraph(new Phrase("P/R - Aerounautical ", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                paragraphaero.Alignment = 1; paragraphaero.SpacingAfter = 6f;

                doc.Add(paragraphaero);
                doc.Add(space);
                doc.Add(aerotable);
                doc.Add(space);
                doc.Add(space);

                Paragraph paragraphmar = new Paragraph(new Phrase("P/R - Marine", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                paragraphmar.Alignment = 1; paragraphmar.SpacingAfter = 6f;

                doc.Add(paragraphmar);
                doc.Add(space);
                doc.Add(martable);
                doc.Add(space);
                doc.Add(space);

                Paragraph paragraphother = new Paragraph(new Phrase("Other P/R Non - Commercial Clients", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                paragraphother.Alignment = 1; paragraphother.SpacingAfter = 6f;

                doc.Add(paragraphother);
                doc.Add(space);
                doc.Add(othertable);
                doc.Add(space);
                doc.Add(space);
                doc.Add(space);

                doc.Add(total);
                doc.Close();
                //Process p = new Process();
                //p.StartInfo.UseShellExecute = true;
                //p.StartInfo.FileName = Testpath;
                //p.Start();
            }
            else
            {
                Button1.Text = "Error";
            }
        }

        public DataSet generateDeferredIncome()
        {
            DataSet dsi = new DataSet();
            int year = 0, month = 0;

            if (DropDownList1.SelectedValue == "2016")
            {
                year = 2016;
            }
            else if (DropDownList1.SelectedValue == "2017")
            {
                year = 2017;
            }

            else if (DropDownList1.SelectedValue == "2018")
            {
                year = 2018;
            }

            else if (DropDownList1.SelectedValue == "2019")
            {
                year = 2019;
            }

            else if (DropDownList1.SelectedValue == "2020")
            {
                year = 2020;
            }


            if (ddl1.SelectedValue == "1")
            {
                month = 1;
            }

            else if (ddl1.SelectedValue == "2")
            {
                month = 2;
            }

            else if (ddl1.SelectedValue == "3")
            {
                month = 3;
            }

            else if (ddl1.SelectedValue == "4")
            {
                month = 4;
            }

            else if (ddl1.SelectedValue == "5")
            {
                month = 5;
            }

            else if (ddl1.SelectedValue == "6")
            {
                month = 6;
            }

            else if (ddl1.SelectedValue == "7")
            {
                month = 7;
            }

            else if (ddl1.SelectedValue == "8")
            {
                month = 8;
            }

            else if (ddl1.SelectedValue == "9")
            {
                month = 9;
            }

            else if (ddl1.SelectedValue == "10")
            {
                month = 10;
            }

            else if (ddl1.SelectedValue == "11")
            {
                month = 11;
            }

            else if (ddl1.SelectedValue == "12")
            {
                month = 12;
            }



            DateTime defered = new DateTime(year, month, 1);

            int mmonth = 0, yyear = 0;
            if (month == 12) { mmonth = 1; yyear = year + 1; }
            else { mmonth = month + 1; yyear = year; }


            Int32 id = 0;
            Decimal invoiceamount = 0;
            DateTime ValidityS;
            DateTime ValidityF;
            int invoiceid = 0;
            string ccnum = " ";
            string Company = " ";
            string fname = " ";
            string lname = " ";
            int glid = 0; string description = " ";
            DataTable table, cell, micro, bbrand, vsat, other, trunking, aero, marine, dServices;


            SqlConnection connection1 = new SqlConnection("Data Source=SMA-DBSRV\\ASMSDEV;Initial Catalog=ASMSGenericMaster;Integrated Security=True");

            SqlConnection connection = new SqlConnection("Data Source=SMA-DBSRV\\ASMSDEV;Initial Catalog=ASMSGenericMaster;Integrated Security=True");

            SqlConnection connection2 = new SqlConnection("Data Source=SMA-DBSRV\\ASMSDEV;Initial Catalog=ASMSSAGEINTEGRATION; MultipleActiveResultSets=True; Integrated Security=True");



            SqlCommand newCmd = new SqlCommand("sp_report", connection1);
            newCmd.CommandType = System.Data.CommandType.StoredProcedure;
            newCmd.Parameters.AddWithValue("@date", defered);


            SqlCommand newCmdd = new SqlCommand("sp_OpeningBalCleanUp", connection2);
            newCmdd.CommandType = System.Data.CommandType.StoredProcedure;
            newCmdd.Parameters.AddWithValue("@date", defered);
            connection2.Open();
            newCmdd.ExecuteNonQuery();
            connection2.Close();

            //SqlCommand newCmd = new SqlCommand();
            //newCmd.Connection = connection1;
            //newCmd.CommandText = "sp_report";
            //newCmd.Parameters.AddWithValue("@date", defered);


            SqlCommand newCmddd = new SqlCommand("sp_getCancellationsAndAdjustments", connection2);
            newCmddd.CommandType = System.Data.CommandType.StoredProcedure;
            newCmddd.Parameters.AddWithValue("@month", month);
            newCmddd.Parameters.AddWithValue("@year", year);


            SqlCommand newCmdddd = new SqlCommand("sp_getCreditMemos", connection2);
            newCmdddd.CommandType = System.Data.CommandType.StoredProcedure;
            newCmdddd.Parameters.AddWithValue("@month", month);
            newCmdddd.Parameters.AddWithValue("@year", year);

            SqlCommand newCmdCredFwd = new SqlCommand("sp_getCreditMemos", connection2);
            newCmdCredFwd.CommandType = System.Data.CommandType.StoredProcedure;
            newCmdCredFwd.Parameters.AddWithValue("@month", mmonth);
            newCmdCredFwd.Parameters.AddWithValue("@year", yyear);


            try
            {
                connection1.Open();
                connection2.Open();
                SqlDataReader rdr = newCmd.ExecuteReader();
                SqlDataReader rdr2 = newCmddd.ExecuteReader();
                SqlDataReader rdr3 = newCmdddd.ExecuteReader();
                SqlDataReader rdr4 = newCmdCredFwd.ExecuteReader();


                table = new DataTable();
                table.Columns.Add("License Number", typeof(string));
                table.Columns.Add("Client Company", typeof(string));
                table.Columns.Add("Invoice ID", typeof(string));
                table.Columns.Add("Budget", typeof(string));
                table.Columns.Add("Invoice Total", typeof(string));
                table.Columns.Add("This Month Invoice", typeof(string));
                table.Columns.Add("Balance B/FWD", typeof(string));
                table.Columns.Add("From Revenue", typeof(string));
                table.Columns.Add("To Revenue", typeof(string));
                table.Columns.Add("Closing Balance", typeof(string));
                table.Columns.Add("Total Months", typeof(int));
                table.Columns.Add("Month Utilized", typeof(int));
                table.Columns.Add("Months Remaining", typeof(int));
                table.Columns.Add("Validity Period Start", typeof(string));
                table.Columns.Add("Validity Period End", typeof(string));



                cell = new DataTable();
                cell.Columns.Add("License Number", typeof(string));
                cell.Columns.Add("Client Company", typeof(string));
                cell.Columns.Add("Invoice ID", typeof(string));
                cell.Columns.Add("Budget", typeof(string));
                cell.Columns.Add("Invoice Total", typeof(string));
                cell.Columns.Add("This Month Invoice", typeof(string));
                cell.Columns.Add("Balance B/FWD", typeof(string));
                cell.Columns.Add("From Revenue", typeof(string));
                cell.Columns.Add("To Revenue", typeof(string));
                cell.Columns.Add("Closing Balance", typeof(string));
                cell.Columns.Add("Total Months", typeof(int));
                cell.Columns.Add("Month Utilized", typeof(int));
                cell.Columns.Add("Months Remaining", typeof(int));
                cell.Columns.Add("Validity Period Start", typeof(string));
                cell.Columns.Add("Validity Period End", typeof(string));



                micro = new DataTable();
                micro.Columns.Add("License Number", typeof(string));
                micro.Columns.Add("Client Company", typeof(string));
                micro.Columns.Add("Invoice ID", typeof(string));
                micro.Columns.Add("Budget", typeof(string));
                micro.Columns.Add("Invoice Total", typeof(string));
                micro.Columns.Add("This Month Invoice", typeof(string));
                micro.Columns.Add("Balance B/FWD", typeof(string));
                micro.Columns.Add("From Revenue", typeof(string));
                micro.Columns.Add("To Revenue", typeof(string));
                micro.Columns.Add("Closing Balance", typeof(string));
                micro.Columns.Add("Total Months", typeof(int));
                micro.Columns.Add("Month Utilized", typeof(int));
                micro.Columns.Add("Months Remaining", typeof(int));
                micro.Columns.Add("Validity Period Start", typeof(string));
                micro.Columns.Add("Validity Period End", typeof(string));



                bbrand = new DataTable();
                bbrand.Columns.Add("License Number", typeof(string));
                bbrand.Columns.Add("Client Company", typeof(string));
                bbrand.Columns.Add("Invoice ID", typeof(string));
                bbrand.Columns.Add("Budget", typeof(string));
                bbrand.Columns.Add("Invoice Total", typeof(string));
                bbrand.Columns.Add("This Month Invoice", typeof(string));
                bbrand.Columns.Add("Balance B/FWD", typeof(string));
                bbrand.Columns.Add("From Revenue", typeof(string));
                bbrand.Columns.Add("To Revenue", typeof(string));
                bbrand.Columns.Add("Closing Balance", typeof(string));
                bbrand.Columns.Add("Total Months", typeof(int));
                bbrand.Columns.Add("Month Utilized", typeof(int));
                bbrand.Columns.Add("Months Remaining", typeof(int));
                bbrand.Columns.Add("Validity Period Start", typeof(string));
                bbrand.Columns.Add("Validity Period End", typeof(string));


                vsat = new DataTable();
                vsat.Columns.Add("License Number", typeof(string));
                vsat.Columns.Add("Client Company", typeof(string));
                vsat.Columns.Add("Invoice ID", typeof(string));
                vsat.Columns.Add("Budget", typeof(string));
                vsat.Columns.Add("Invoice Total", typeof(string));
                vsat.Columns.Add("This Month Invoice", typeof(string));
                vsat.Columns.Add("Balance B/FWD", typeof(string));
                vsat.Columns.Add("From Revenue", typeof(string));
                vsat.Columns.Add("To Revenue", typeof(string));
                vsat.Columns.Add("Closing Balance", typeof(string));
                vsat.Columns.Add("Total Months", typeof(int));
                vsat.Columns.Add("Month Utilized", typeof(int));
                vsat.Columns.Add("Months Remaining", typeof(int));
                vsat.Columns.Add("Validity Period Start", typeof(string));
                vsat.Columns.Add("Validity Period End", typeof(string));



                other = new DataTable();
                other.Columns.Add("License Number", typeof(string));
                other.Columns.Add("Client Company", typeof(string));
                other.Columns.Add("Invoice ID", typeof(string));
                other.Columns.Add("Budget", typeof(string));
                other.Columns.Add("Invoice Total", typeof(string));
                other.Columns.Add("This Month Invoice", typeof(string));
                other.Columns.Add("Balance B/FWD", typeof(string));
                other.Columns.Add("From Revenue", typeof(string));
                other.Columns.Add("To Revenue", typeof(string));
                other.Columns.Add("Closing Balance", typeof(string));
                other.Columns.Add("Total Months", typeof(int));
                other.Columns.Add("Month Utilized", typeof(int));
                other.Columns.Add("Months Remaining", typeof(int));
                other.Columns.Add("Validity Period Start", typeof(string));
                other.Columns.Add("Validity Period End", typeof(string));



                trunking = new DataTable();
                trunking.Columns.Add("License Number", typeof(string));
                trunking.Columns.Add("Client Company", typeof(string));
                trunking.Columns.Add("Invoice ID", typeof(string));
                trunking.Columns.Add("Budget", typeof(string));
                trunking.Columns.Add("Invoice Total", typeof(string));
                trunking.Columns.Add("This Month Invoice", typeof(string));
                trunking.Columns.Add("Balance B/FWD", typeof(string));
                trunking.Columns.Add("From Revenue", typeof(string));
                trunking.Columns.Add("To Revenue", typeof(string));
                trunking.Columns.Add("Closing Balance", typeof(string));
                trunking.Columns.Add("Total Months", typeof(int));
                trunking.Columns.Add("Month Utilized", typeof(int));
                trunking.Columns.Add("Months Remaining", typeof(int));
                trunking.Columns.Add("Validity Period Start", typeof(string));
                trunking.Columns.Add("Validity Period End", typeof(string));



                aero = new DataTable();
                aero.Columns.Add("License Number", typeof(string));
                aero.Columns.Add("Client Company", typeof(string));
                aero.Columns.Add("Invoice ID", typeof(string));
                aero.Columns.Add("Budget", typeof(string));
                aero.Columns.Add("Invoice Total", typeof(string));
                aero.Columns.Add("This Month Invoice", typeof(string));
                aero.Columns.Add("Balance B/FWD", typeof(string));
                aero.Columns.Add("From Revenue", typeof(string));
                aero.Columns.Add("To Revenue", typeof(string));
                aero.Columns.Add("Closing Balance", typeof(string));
                aero.Columns.Add("Total Months", typeof(int));
                aero.Columns.Add("Month Utilized", typeof(int));
                aero.Columns.Add("Months Remaining", typeof(int));
                aero.Columns.Add("Validity Period Start", typeof(string));
                aero.Columns.Add("Validity Period End", typeof(string));



                marine = new DataTable();
                marine.Columns.Add("License Number", typeof(string));
                marine.Columns.Add("Client Company", typeof(string));
                marine.Columns.Add("Invoice ID", typeof(string));
                marine.Columns.Add("Budget", typeof(string));
                marine.Columns.Add("Invoice Total", typeof(string));
                marine.Columns.Add("This Month Invoice", typeof(string));
                marine.Columns.Add("Balance B/FWD", typeof(string));
                marine.Columns.Add("From Revenue", typeof(string));
                marine.Columns.Add("To Revenue", typeof(string));
                marine.Columns.Add("Closing Balance", typeof(string));
                marine.Columns.Add("Total Months", typeof(int));
                marine.Columns.Add("Month Utilized", typeof(int));
                marine.Columns.Add("Months Remaining", typeof(int));
                marine.Columns.Add("Validity Period Start", typeof(string));
                marine.Columns.Add("Validity Period End", typeof(string));



                dServices = new DataTable();
                dServices.Columns.Add("License Number", typeof(string));
                dServices.Columns.Add("Client Company", typeof(string));
                dServices.Columns.Add("Invoice ID", typeof(string));
                dServices.Columns.Add("Budget", typeof(string));
                dServices.Columns.Add("Invoice Total", typeof(string));
                dServices.Columns.Add("This Month Invoice", typeof(string));
                dServices.Columns.Add("Balance B/FWD", typeof(string));
                dServices.Columns.Add("From Revenue", typeof(string));
                dServices.Columns.Add("To Revenue", typeof(string));
                dServices.Columns.Add("Closing Balance", typeof(string));
                dServices.Columns.Add("Total Months", typeof(int));
                dServices.Columns.Add("Month Utilized", typeof(int));
                dServices.Columns.Add("Months Remaining", typeof(int));
                dServices.Columns.Add("Validity Period Start", typeof(string));
                dServices.Columns.Add("Validity Period End", typeof(string));


                while (rdr.Read())
                    {
                        id = rdr.GetInt32(0);
                        ccnum = rdr.GetString(1);
                        Company = rdr.GetString(2);
                        fname = rdr.GetString(3);
                        lname = rdr.GetString(4);
                        invoiceamount = rdr.GetDecimal(5);
                        ValidityS = rdr.GetDateTime(6);
                        ValidityF = rdr.GetDateTime(7);
                        glid = rdr.GetInt32(8);
                        description = rdr.GetString(9);
                        invoiceid = rdr.GetInt32(10);
                        DateTime createdate = rdr.GetDateTime(11);
                        int Differencee = ((defered.Year - ValidityS.Year) * 12) + defered.Month - ValidityS.Month;
                        decimal budget = 0;
                        int validity = 0;
                        int Difference = 0;
                        //  int validity = ((ValidityF.Year - ValidityS.Year) * 12) + ValidityF.Month - ValidityS.Month;
                        var ValiditySS = DateTime.Now.ToString("dd/MM/yyyy");
                        if (description == "Modification")
                        {
                            int totmonths = CheckMonths(ValidityS, ValidityF);
                            int monthsgone = CheckMonthsModification(ValidityS, createdate);
                            validity = totmonths - monthsgone;
                            int Di = ((defered.Year - ValidityS.Year) * 12) + defered.Month - ValidityS.Month;
                            Difference = Di - monthsgone;
                            ValiditySS = createdate.ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            validity = CheckMonths(ValidityS, ValidityF);
                            if (validity == 2) validity = 3;
                            Difference = ((defered.Year - ValidityS.Year) * 12) + defered.Month - ValidityS.Month;
                            ValiditySS = ValidityS.ToString("dd/MM/yyyy");
                        }
                        int trial33 = validity - Differencee;
                        Differencee = Differencee + 1;
                        DataSet df = new DataSet();
                        df = _000.App_Code.BusinessClass.GetBudget(ccnum, invoiceid);
                        if (!IsEmpty(df) && Differencee > 1)
                        {

                            DataRow dr = df.Tables[0].Rows[0];
                            budget = Convert.ToDecimal(dr.ItemArray.GetValue(1));

                        }
                       
                      


                        if (!IsEmpty(df) && Differencee == 1)
                        {

                            DataRow dr = df.Tables[0].Rows[0];
                            DateTime ne = Convert.ToDateTime(dr.ItemArray.GetValue(2));
                            if (ne.Month >= ValidityS.Month /*&& ne.Year >= ValidityS.Year*/)
                            {
                                budget = Convert.ToDecimal(dr.ItemArray.GetValue(1));
                            }
                        }


                        DataSet budg = _000.App_Code.BusinessClass.GetCustBudget(ccnum, invoiceid);
                        if (IsEmpty(budg))
                        {
                            _000.App_Code.BusinessClass.InsertBudgetInfo(ccnum, budget, invoiceid);
                        }



                        var ValidityFF = ValidityF.ToString("dd/MM/yyyy");



                        //------------------------------------------------------------------------------------

                        string invoicestat = " ";
                        string clientCompany = " ";
                        decimal openingbalance = 0;
                        decimal closingbalance = 0;
                        decimal toRevenue = 0;
                        decimal fromRevenue = 0;



                        if (Company == null || Company == "")
                        {
                            clientCompany = fname + " " + lname;

                        }
                        else
                        {
                            clientCompany = Company;
                            clientCompany = System.Net.WebUtility.HtmlDecode(clientCompany);
                        }

                        //DataSet inv = new DataSet();
                        //inv = _000.App_Code.BusinessClass.GetInv(invoiceid);
                        //if (IsEmpty(inv) && Differencee > 1)
                        //{
                        //    _000.App_Code.BusinessClass.InsertInvoice(invoiceid, ccnum, clientCompany);


                        //}

                        int trial = ((defered.Year - ValidityS.Year) * 12) + validity - Difference;
                        int trial2 = validity - Difference;


                        decimal opp = 0;
                        string op = " ";
                        DataSet opset = new DataSet();

                        DataSet IfIsCredMemo = _000.App_Code.BusinessClass.GetIsInvoiceCreditMemo(invoiceid);
                        {
                            if (Difference > 0 && ValidityS.Month != defered.Month || ValidityS.Month == defered.Month && ValidityF.Year != defered.Year && ValidityS.Year != defered.Year || ValidityF.Month == defered.Month)
                            {
                                if (defered.Month - 1 == 0)
                                {
                                    opset = _000.App_Code.BusinessClass.GetOpeningBalanceForMonth(ccnum, 12, (defered.Year - 1), invoiceid);
                                }

                                else
                                    opset = _000.App_Code.BusinessClass.GetOpeningBalanceForMonth(ccnum, (defered.Month - 1), defered.Year, invoiceid);
                                if (!IsEmpty(opset))
                                {
                                    DataRow dr = opset.Tables[0].Rows[0];


                                    decimal.TryParse(dr["openingbalance"].ToString(), out opp);


                                }
                                else
                                    opp = 0;
                            }

                            if (opp == 0)
                            {
                                opp = 0;
                            }
                            else
                            {
                                opp = opp;
                            }
                        }

                        if (invoiceid == 16471)
                        {
                            int a = 0;
                        }


                        if (ValidityS.Month == defered.Month && ValidityS.Year == defered.Year && IsEmpty(IfIsCredMemo))
                        {
                            if (ValidityS.Day <= 15)
                            {
                                if (Difference >= 0 && trial2 >= 0)
                                {
                                    Difference = Difference + 1;
                                    trial2 = validity - Difference;
                                    openingbalance = 0;
                                    toRevenue = invoiceamount * 1 / validity;
                                    closingbalance = invoiceamount - toRevenue;

                                    fromRevenue = invoiceamount;
                                    invoicestat = "Yes";
                                }

                                if (glid == 5156)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        invoicetotcell += invoiceamount;
                                        torevcell += toRevenue;
                                        fromremcell += fromRevenue;
                                        closingtotcell += closingbalance;
                                        balancebfcell += openingbalance;
                                        budgettotcell += budget;
                                        fromRev += fromRevenue;
                                        balancebf += openingbalance;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        budgettotal += budget;
                                        closeBal += closingbalance;
                                        cell.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5157)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotbb += budget;
                                        invoicetotbb += invoiceamount;
                                        torevbb += toRevenue;
                                        fromrembb += fromRevenue;
                                        closingtotbb += closingbalance;
                                        balancebfbb += openingbalance;
                                        fromRev += fromRevenue;
                                        balancebf += openingbalance;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        budgettotal += budget;
                                        closeBal += closingbalance;
                                        bbrand.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5158)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotmic += budget;
                                        invoicetotmicro += invoiceamount;
                                        torevmicro += toRevenue;
                                        fromrevmicro += fromRevenue;
                                        closingtotmicro += closingbalance;
                                        balancebfmicro += openingbalance;
                                        fromRev += fromRevenue;
                                        balancebf += openingbalance;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        budgettotal += budget;
                                        closeBal += closingbalance;
                                        micro.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(fromRevenue, 2)),formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);

                                    }
                                }

                                else if (glid == 5159)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotds += budget;
                                        invoicetotds += invoiceamount;
                                        torevds += toRevenue;
                                        fromrevds += fromRevenue;
                                        closingtotds += closingbalance;
                                        balancebfds += openingbalance;
                                        fromRev += fromRevenue;
                                        balancebf += openingbalance;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        budgettotal += budget;
                                        closeBal += closingbalance;
                                        dServices.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(fromRevenue, 2)),formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5160)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotvsat += budget;
                                        invoicetotvsat += invoiceamount;
                                        torevvsat += toRevenue;
                                        fromrevvsat += fromRevenue;
                                        closingtotvsat += closingbalance;
                                        balancebfvsat += openingbalance;
                                        fromRev += fromRevenue;
                                        balancebf += openingbalance;
                                        budgettotal += budget;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        vsat.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(fromRevenue, 2)),formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);

                                    }
                                }

                                else if (glid == 5161)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotaero += budget;
                                        invoicetotaero += invoiceamount;
                                        torevaero += toRevenue;
                                        fromrevaero += fromRevenue;
                                        closingtotaero += closingbalance;
                                        balancebfaero += openingbalance;
                                        fromRev += fromRevenue;
                                        balancebf += openingbalance;
                                        budgettotal += budget;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        aero.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }


                                else if (glid == 5162)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotmar += budget;
                                        invoicetotmar += invoiceamount;
                                        torevmar += toRevenue;
                                        fromrevmar += fromRevenue;
                                        closingtotmar += closingbalance;
                                        balancebfmar += openingbalance;
                                        fromRev += fromRevenue;
                                        balancebf += openingbalance;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        budgettotal += budget;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        marine.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5163)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettottrunk += budget;
                                        invoicetottrunk += invoiceamount;
                                        torevtrunk += toRevenue;
                                        fromrevtrunk += fromRevenue;
                                        closingtottrunk += closingbalance;
                                        balancebftrunk += openingbalance;
                                        fromRev += fromRevenue;
                                        balancebf += openingbalance;
                                        budgettotal += budget;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        trunking.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(fromRevenue, 2)),formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5164)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotother += budget;
                                        invoicetotother += invoiceamount;
                                        torevother += toRevenue;
                                        fromrevother += fromRevenue;
                                        closingtotother += closingbalance;
                                        balancebfother += openingbalance;
                                        fromRev += fromRevenue;
                                        balancebf += openingbalance;
                                        budgettotal += budget;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        other.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }
                                else
                                {
                                    table.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                }
                            }

                            if (ValidityS.Day > 15)
                            {

                                if (Difference >= 0 && trial2 >= 0)
                                {
                                    openingbalance = 0;
                                    toRevenue = 0;
                                    fromRevenue = invoiceamount;
                                    closingbalance = invoiceamount;
                                    invoicestat = "Yes";

                                    opp = 0;

                                    if (Difference > 1 && IsEmpty(opset))
                                    {
                                        toRevenue = toRevenue * Difference;
                                    }
                                }
                                if (glid == 5156)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotcell += budget;
                                        invoicetotcell += invoiceamount;
                                        torevcell += toRevenue;
                                        fromremcell += fromRevenue;
                                        closingtotcell += closingbalance;
                                        balancebfcell += opp;
                                        fromRev += fromRevenue;
                                        balancebf += opp;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        budgettotal += budget;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        cell.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5157)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotbb += budget;
                                        invoicetotbb += invoiceamount;
                                        torevbb += toRevenue;
                                        fromrembb += fromRevenue;
                                        closingtotbb += closingbalance;
                                        balancebfbb += opp;
                                        fromRev += fromRevenue;
                                        balancebf += opp;
                                        budgettotal += budget;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        bbrand.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5158)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotmic += budget;
                                        invoicetotmicro += invoiceamount;
                                        torevmicro += toRevenue;
                                        fromrevmicro += fromRevenue;
                                        closingtotmicro += closingbalance;
                                        balancebfmicro += opp;
                                        fromRev += fromRevenue;
                                        balancebf += opp;
                                        budgettotal += budget;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        micro.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5159)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotds += budget;
                                        invoicetotds += invoiceamount;
                                        torevds += toRevenue;
                                        fromrevds += fromRevenue;
                                        closingtotds += closingbalance;
                                        balancebfds += opp;
                                        fromRev += fromRevenue;
                                        balancebf += opp;
                                        budgettotal += budget;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        dServices.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }


                                else if (glid == 5160)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotvsat += budget;
                                        invoicetotvsat += invoiceamount;
                                        torevvsat += toRevenue;
                                        fromrevvsat += fromRevenue;
                                        closingtotvsat += closingbalance;
                                        balancebfvsat += opp;
                                        fromRev += fromRevenue;
                                        balancebf += opp;
                                        budgettotal += budget;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        vsat.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }


                                else if (glid == 5161)
                                {

                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotaero += budget;
                                        invoicetotaero += invoiceamount;
                                        torevaero += toRevenue;
                                        fromrevaero += fromRevenue;
                                        closingtotaero += closingbalance;
                                        balancebfaero += opp;
                                        fromRev += fromRevenue;
                                        balancebf += opp;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        budgettotal += budget;
                                        closeBal += closingbalance;
                                        aero.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }


                                else if (glid == 5162)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotmar += budget;
                                        invoicetotmar += invoiceamount;
                                        torevmar += toRevenue;
                                        fromrevmar += fromRevenue;
                                        closingtotmar += closingbalance;
                                        balancebfmar += opp;
                                        fromRev += fromRevenue;
                                        budgettotal += budget;
                                        balancebf += opp;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        marine.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5163)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettottrunk += budget;
                                        invoicetottrunk += invoiceamount;
                                        torevtrunk += toRevenue;
                                        fromrevtrunk += fromRevenue;
                                        closingtottrunk += closingbalance;
                                        balancebftrunk += opp;
                                        fromRev += fromRevenue;
                                        balancebf += opp;
                                        budgettotal += budget;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        trunking.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }


                                else if (glid == 5164)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotother += budget;
                                        invoicetotother += invoiceamount;
                                        torevother += toRevenue;
                                        fromrevother += fromRevenue;
                                        closingtotother += closingbalance;
                                        balancebfother += opp;
                                        fromRev += fromRevenue;
                                        balancebf += opp;
                                        budgettotal += budget;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        other.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }
                                else
                                {
                                    table.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                }
                            }
                        }

                        //drop here 

                        else if (ValidityS.Month == (defered.Month - 1) && ValidityS.Day > 15 && ValidityS.Year == defered.Year && IsEmpty(IfIsCredMemo))
                        {
                            if (Difference >= 0 && trial2 >= 0)
                            {
                                openingbalance = 0;
                                toRevenue = invoiceamount * 1 / validity;
                                //changed
                                fromRevenue = 0;
                                closingbalance = invoiceamount - toRevenue;
                                invoicestat = "No";


                                //if (!IsEmpty(opset))
                                //{
                                //    opp = opp;
                                //}
                                //else
                                //    opp = 0;

                                opp = invoiceamount;

                                if (Difference > 1 && IsEmpty(opset))
                                {
                                    toRevenue = toRevenue * Difference;
                                }

                                if (description == "Modification" && Difference == 0)
                                {
                                    toRevenue = 0;
                                }
                                if (description == "Modification" && defered.Month == createdate.Month && defered.Year == createdate.Year)
                                {
                                    fromRevenue = invoiceamount;
                                    invoicestat = "Yes";
                                }
                            }
                            if (glid == 5156)
                            {
                                if (Difference >= 0 && trial2 >= 0)
                                {
                                    budgettotcell += budget;
                                    invoicetotcell += invoiceamount;
                                    torevcell += toRevenue;
                                    fromremcell += fromRevenue;
                                    closingtotcell += closingbalance;
                                    balancebfcell += opp;
                                    fromRev += fromRevenue;
                                    balancebf += opp;
                                    invoiceTotalForYes += invoiceamount;
                                    invoiceTotal += invoiceamount;
                                    budgettotal += budget;
                                    toRev += toRevenue;
                                    closeBal += closingbalance;
                                    cell.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                }
                            }

                            else if (glid == 5157)
                            {
                                if (Difference >= 0 && trial2 >= 0)
                                {
                                    budgettotbb += budget;
                                    invoicetotbb += invoiceamount;
                                    torevbb += toRevenue;
                                    fromrembb += fromRevenue;
                                    closingtotbb += closingbalance;
                                    balancebfbb += opp;
                                    fromRev += fromRevenue;
                                    balancebf += opp;
                                    budgettotal += budget;
                                    invoiceTotalForYes += invoiceamount;
                                    invoiceTotal += invoiceamount;
                                    toRev += toRevenue;
                                    closeBal += closingbalance;
                                    bbrand.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                }
                            }

                            else if (glid == 5158)
                            {
                                if (Difference >= 0 && trial2 >= 0)
                                {
                                    budgettotmic += budget;
                                    invoicetotmicro += invoiceamount;
                                    torevmicro += toRevenue;
                                    fromrevmicro += fromRevenue;
                                    closingtotmicro += closingbalance;
                                    balancebfmicro += opp;
                                    fromRev += fromRevenue;
                                    balancebf += opp;
                                    budgettotal += budget;
                                    invoiceTotalForYes += invoiceamount;
                                    invoiceTotal += invoiceamount;
                                    toRev += toRevenue;
                                    closeBal += closingbalance;
                                    micro.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                }
                            }

                            else if (glid == 5159)
                            {
                                if (Difference >= 0 && trial2 >= 0)
                                {
                                    budgettotds += budget;
                                    invoicetotds += invoiceamount;
                                    torevds += toRevenue;
                                    fromrevds += fromRevenue;
                                    closingtotds += closingbalance;
                                    balancebfds += opp;
                                    fromRev += fromRevenue;
                                    balancebf += opp;
                                    budgettotal += budget;
                                    invoiceTotalForYes += invoiceamount;
                                    invoiceTotal += invoiceamount;
                                    toRev += toRevenue;
                                    closeBal += closingbalance;
                                    dServices.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                }
                            }


                            else if (glid == 5160)
                            {
                                if (Difference >= 0 && trial2 >= 0)
                                {
                                    budgettotvsat += budget;
                                    invoicetotvsat += invoiceamount;
                                    torevvsat += toRevenue;
                                    fromrevvsat += fromRevenue;
                                    closingtotvsat += closingbalance;
                                    balancebfvsat += opp;
                                    fromRev += fromRevenue;
                                    balancebf += opp;
                                    budgettotal += budget;
                                    invoiceTotalForYes += invoiceamount;
                                    invoiceTotal += invoiceamount;
                                    toRev += toRevenue;
                                    closeBal += closingbalance;
                                    vsat.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                }
                            }


                            else if (glid == 5161)
                            {

                                if (Difference >= 0 && trial2 >= 0)
                                {
                                    budgettotaero += budget;
                                    invoicetotaero += invoiceamount;
                                    torevaero += toRevenue;
                                    fromrevaero += fromRevenue;
                                    closingtotaero += closingbalance;
                                    balancebfaero += opp;
                                    fromRev += fromRevenue;
                                    balancebf += opp;
                                    invoiceTotalForYes += invoiceamount;
                                    invoiceTotal += invoiceamount;
                                    toRev += toRevenue;
                                    budgettotal += budget;
                                    closeBal += closingbalance;
                                    aero.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                }
                            }


                            else if (glid == 5162)
                            {
                                if (Difference >= 0 && trial2 >= 0)
                                {
                                    budgettotmar += budget;
                                    invoicetotmar += invoiceamount;
                                    torevmar += toRevenue;
                                    fromrevmar += fromRevenue;
                                    closingtotmar += closingbalance;
                                    balancebfmar += opp;
                                    fromRev += fromRevenue;
                                    budgettotal += budget;
                                    balancebf += opp;
                                    invoiceTotalForYes += invoiceamount;
                                    invoiceTotal += invoiceamount;
                                    toRev += toRevenue;
                                    closeBal += closingbalance;
                                    marine.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                }
                            }

                            else if (glid == 5163)
                            {
                                if (Difference >= 0 && trial2 >= 0)
                                {
                                    budgettottrunk += budget;
                                    invoicetottrunk += invoiceamount;
                                    torevtrunk += toRevenue;
                                    fromrevtrunk += fromRevenue;
                                    closingtottrunk += closingbalance;
                                    balancebftrunk += opp;
                                    fromRev += fromRevenue;
                                    balancebf += opp;
                                    budgettotal += budget;
                                    invoiceTotalForYes += invoiceamount;
                                    invoiceTotal += invoiceamount;
                                    toRev += toRevenue;
                                    closeBal += closingbalance;
                                    trunking.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                }
                            }


                            else if (glid == 5164)
                            {
                                if (Difference >= 0 && trial2 >= 0)
                                {
                                    budgettotother += budget;
                                    invoicetotother += invoiceamount;
                                    torevother += toRevenue;
                                    fromrevother += fromRevenue;
                                    closingtotother += closingbalance;
                                    balancebfother += opp;
                                    fromRev += fromRevenue;
                                    balancebf += opp;
                                    budgettotal += budget;
                                    invoiceTotalForYes += invoiceamount;
                                    invoiceTotal += invoiceamount;
                                    toRev += toRevenue;
                                    closeBal += closingbalance;
                                    other.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                }
                            }
                            else
                            {
                                table.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                            }
                        }

                        else if (ValidityS.Month == defered.Month && ValidityF.Year != defered.Year && ValidityS.Day <= 15 && ValidityS.Year != defered.Year && IsEmpty(IfIsCredMemo))
                        {

                            if (Difference >= 0 && trial2 >= 0)
                            {

                                Difference = Difference + 1;
                                trial2 = validity - Difference;
                                toRevenue = invoiceamount * 1 / validity;

                                decimal amountalreadypaid = toRevenue * (Difference - 1);
                                openingbalance = invoiceamount - amountalreadypaid;
                                fromRevenue = 0;
                                closingbalance = invoiceamount - (amountalreadypaid + toRevenue);

                                invoicestat = "No";

                                if (!IsEmpty(opset))
                                {
                                    opp = opp;
                                }
                                else
                                    opp = 0;

                                if (Difference > 1 && IsEmpty(opset))
                                {
                                    toRevenue = toRevenue * Difference;
                                    fromRevenue = invoiceamount;
                                }
                                if (description == "Modification" && Difference == 0)
                                {
                                    toRevenue = 0;
                                }
                                if (description == "Modification" && defered.Month == createdate.Month && defered.Year == createdate.Year)
                                {
                                    fromRevenue = invoiceamount;
                                    invoicestat = "Yes";
                                }
                            }
                            if (glid == 5156)
                            {
                                if (Difference >= 0 && trial2 >= 0)
                                {
                                    budgettotcell += budget;
                                    invoicetotcell += invoiceamount;
                                    torevcell += toRevenue;
                                    fromremcell += fromRevenue;
                                    closingtotcell += closingbalance;
                                    balancebfcell += opp;
                                    fromRev += fromRevenue;
                                    balancebf += opp;
                                    invoiceTotalForYes += invoiceamount;
                                    invoiceTotal += invoiceamount;
                                    toRev += toRevenue;
                                    budgettotal += budget;
                                    closeBal += closingbalance;
                                    cell.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                }
                            }

                            else if (glid == 5157)
                            {
                                if (Difference >= 0 && trial2 >= 0)
                                {
                                    budgettotbb += budget;
                                    invoicetotbb += invoiceamount;
                                    torevbb += toRevenue;
                                    fromrembb += fromRevenue;
                                    closingtotbb += closingbalance;
                                    balancebfbb += opp;
                                    fromRev += fromRevenue;
                                    balancebf += opp;
                                    invoiceTotalForYes += invoiceamount;
                                    invoiceTotal += invoiceamount;
                                    toRev += toRevenue;
                                    budgettotal += budget;
                                    closeBal += closingbalance;
                                    bbrand.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                }
                            }
                            else if (glid == 5158)
                            {
                                if (Difference >= 0 && trial2 >= 0)
                                {
                                    budgettotmic += budget;
                                    invoicetotmicro += invoiceamount;
                                    torevmicro += toRevenue;
                                    fromrevmicro += fromRevenue;
                                    closingtotmicro += closingbalance;
                                    balancebfmicro += opp;
                                    fromRev += fromRevenue;
                                    balancebf += opp;
                                    invoiceTotalForYes += invoiceamount;
                                    invoiceTotal += invoiceamount;
                                    toRev += toRevenue;
                                    budgettotal += budget;
                                    closeBal += closingbalance;
                                    micro.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                }
                            }

                            else if (glid == 5159)
                            {
                                if (Difference >= 0 && trial2 >= 0)
                                {
                                    budgettotds += budget;
                                    invoicetotds += invoiceamount;
                                    torevds += toRevenue;
                                    fromrevds += fromRevenue;
                                    closingtotds += closingbalance;
                                    balancebfds += opp;
                                    fromRev += fromRevenue;
                                    balancebf += opp;
                                    budgettotal += budget;
                                    invoiceTotalForYes += invoiceamount;
                                    invoiceTotal += invoiceamount;
                                    toRev += toRevenue;
                                    closeBal += closingbalance;
                                    dServices.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                }
                            }

                            else if (glid == 5160)
                            {
                                if (Difference >= 0 && trial2 >= 0)
                                {
                                    budgettotvsat += budget;
                                    invoicetotvsat += invoiceamount;
                                    torevvsat += toRevenue;
                                    fromrevvsat += fromRevenue;
                                    closingtotvsat += closingbalance;
                                    balancebfvsat += opp;
                                    fromRev += fromRevenue;
                                    balancebf += opp;
                                    budgettotal += budget;
                                    invoiceTotalForYes += invoiceamount;
                                    invoiceTotal += invoiceamount;
                                    toRev += toRevenue;
                                    closeBal += closingbalance;
                                    vsat.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                }
                            }

                            else if (glid == 5161)
                            {
                                if (Difference >= 0 && trial2 >= 0)
                                {
                                    budgettotaero += budget;
                                    invoicetotaero += invoiceamount;
                                    torevaero += toRevenue;
                                    fromrevaero += fromRevenue;
                                    closingtotaero += closingbalance;
                                    balancebfaero += opp;
                                    fromRev += fromRevenue;
                                    balancebf += opp;
                                    budgettotal += budget;
                                    invoiceTotalForYes += invoiceamount;
                                    invoiceTotal += invoiceamount;
                                    toRev += toRevenue;
                                    closeBal += closingbalance;
                                    aero.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                }
                            }

                            else if (glid == 5162)
                            {
                                if (Difference >= 0 && trial2 >= 0)
                                {
                                    budgettotmar += budget;
                                    invoicetotmar += invoiceamount;
                                    torevmar += toRevenue;
                                    fromrevmar += fromRevenue;
                                    closingtotmar += closingbalance;
                                    balancebfmar += opp;
                                    fromRev += fromRevenue;
                                    balancebf += opp;
                                    budgettotal += budget;
                                    invoiceTotalForYes += invoiceamount;
                                    invoiceTotal += invoiceamount;
                                    toRev += toRevenue;
                                    closeBal += closingbalance;
                                    marine.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                }
                            }

                            else if (glid == 5163)
                            {
                                if (Difference >= 0 && trial2 >= 0)
                                {
                                    budgettottrunk += budget;
                                    invoicetottrunk += invoiceamount;
                                    torevtrunk += toRevenue;
                                    fromrevtrunk += fromRevenue;
                                    closingtottrunk += closingbalance;
                                    balancebftrunk += opp;
                                    fromRev += fromRevenue;
                                    balancebf += opp;
                                    budgettotal += budget;
                                    invoiceTotalForYes += invoiceamount;
                                    invoiceTotal += invoiceamount;
                                    toRev += toRevenue;
                                    closeBal += closingbalance;
                                    trunking.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                }
                            }

                            else if (glid == 5164)
                            {
                                if (Difference >= 0 && trial2 >= 0)
                                {
                                    budgettotother += budget;
                                    invoicetotother += invoiceamount;
                                    torevother += toRevenue;
                                    fromrevother += fromRevenue;
                                    closingtotother += closingbalance;
                                    balancebfother += opp;
                                    fromRev += fromRevenue;
                                    budgettotal += budget;
                                    balancebf += opp;
                                    invoiceTotalForYes += invoiceamount;
                                    invoiceTotal += invoiceamount;
                                    toRev += toRevenue;
                                    closeBal += closingbalance;
                                    other.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                }
                            }
                            else
                            {
                                table.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                            }
                        }

                        //Right here

                        else if (ValidityS.Month == defered.Month && ValidityF.Year != defered.Year && ValidityS.Day > 15 && ValidityS.Year != defered.Year && IsEmpty(IfIsCredMemo))
                        {
                            if (Difference >= 0 && trial2 >= 0)
                            {
                                toRevenue = invoiceamount * 1 / validity;

                                decimal amountalreadypaid = toRevenue * (Difference - 1);
                                openingbalance = invoiceamount - amountalreadypaid;
                                fromRevenue = 0;
                                closingbalance = invoiceamount - (amountalreadypaid + toRevenue);

                                invoicestat = "No";


                                if (!IsEmpty(opset))
                                {
                                    opp = opp;

                                }
                                else
                                    opp = 0;



                                if (Difference > 1 && IsEmpty(opset))
                                {
                                    toRevenue = toRevenue * Difference;
                                    fromRevenue = invoiceamount;
                                }

                                if (description == "Modification" && Difference == 0)
                                {
                                    toRevenue = 0;
                                }
                                if (description == "Modification" && defered.Month == createdate.Month && defered.Year == createdate.Year)
                                {
                                    fromRevenue = invoiceamount;
                                    invoicestat = "Yes";
                                }
                            }
                            if (glid == 5156)
                            {
                                if (Difference >= 0 && trial2 >= 0)
                                {
                                    budgettotcell += budget;
                                    invoicetotcell += invoiceamount;
                                    torevcell += toRevenue;
                                    fromremcell += fromRevenue;
                                    closingtotcell += closingbalance;
                                    balancebfcell += opp;
                                    fromRev += fromRevenue;
                                    balancebf += opp;
                                    budgettotal += budget;
                                    invoiceTotalForYes += invoiceamount;
                                    invoiceTotal += invoiceamount;
                                    toRev += toRevenue;
                                    closeBal += closingbalance;
                                    cell.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                }
                            }

                            else if (glid == 5157)
                            {
                                if (Difference >= 0 && trial2 >= 0)
                                {
                                    budgettotbb += budget;
                                    invoicetotbb += invoiceamount;
                                    torevbb += toRevenue;
                                    fromrembb += fromRevenue;
                                    closingtotbb += closingbalance;
                                    balancebfbb += opp;
                                    fromRev += fromRevenue;
                                    balancebf += opp;
                                    budgettotal += budget;
                                    invoiceTotalForYes += invoiceamount;
                                    invoiceTotal += invoiceamount;
                                    toRev += toRevenue;
                                    closeBal += closingbalance;
                                    bbrand.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                }
                            }
                            else if (glid == 5158)
                            {
                                if (Difference >= 0 && trial2 >= 0)
                                {
                                    budgettotmic += budget;
                                    invoicetotmicro += invoiceamount;
                                    torevmicro += toRevenue;
                                    fromrevmicro += fromRevenue;
                                    closingtotmicro += closingbalance;
                                    balancebfmicro += opp;
                                    fromRev += fromRevenue;
                                    balancebf += opp;
                                    budgettotal += budget;
                                    invoiceTotalForYes += invoiceamount;
                                    invoiceTotal += invoiceamount;
                                    toRev += toRevenue;
                                    closeBal += closingbalance;
                                    micro.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                }
                            }

                            else if (glid == 5159)
                            {
                                if (Difference >= 0 && trial2 >= 0)
                                {
                                    budgettotds += budget;
                                    invoicetotds += invoiceamount;
                                    torevds += toRevenue;
                                    fromrevds += fromRevenue;
                                    closingtotds += closingbalance;
                                    balancebfds += opp;
                                    fromRev += fromRevenue;
                                    budgettotal += budget;
                                    balancebf += opp;
                                    invoiceTotalForYes += invoiceamount;
                                    invoiceTotal += invoiceamount;
                                    toRev += toRevenue;
                                    closeBal += closingbalance;
                                    dServices.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                }
                            }

                            else if (glid == 5160)
                            {

                                if (Difference >= 0 && trial2 >= 0)
                                {
                                    budgettotvsat += budget;
                                    invoicetotvsat += invoiceamount;
                                    torevvsat += toRevenue;
                                    fromrevvsat += fromRevenue;
                                    closingtotvsat += closingbalance;
                                    balancebfvsat += opp;
                                    fromRev += fromRevenue;
                                    budgettotal += budget;
                                    balancebf += opp;
                                    invoiceTotalForYes += invoiceamount;
                                    invoiceTotal += invoiceamount;
                                    toRev += toRevenue;
                                    closeBal += closingbalance;
                                    vsat.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                }
                            }

                            else if (glid == 5161)
                            {
                                if (Difference >= 0 && trial2 >= 0)
                                {
                                    budgettotaero += budget;
                                    invoicetotaero += invoiceamount;
                                    torevaero += toRevenue;
                                    fromrevaero += fromRevenue;
                                    closingtotaero += closingbalance;
                                    balancebfaero += opp;
                                    fromRev += fromRevenue;
                                    budgettotal += budget;
                                    balancebf += opp;
                                    invoiceTotalForYes += invoiceamount;
                                    invoiceTotal += invoiceamount;
                                    toRev += toRevenue;
                                    closeBal += closingbalance;

                                    aero.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                }
                            }

                            else if (glid == 5162)
                            {
                                if (Difference >= 0 && trial2 >= 0)
                                {
                                    budgettotmar += budget;
                                    invoicetotmar += invoiceamount;
                                    torevmar += toRevenue;
                                    fromrevmar += fromRevenue;
                                    closingtotmar += closingbalance;
                                    balancebfmar += opp;
                                    fromRev += fromRevenue;
                                    budgettotal += budget;
                                    balancebf += opp;
                                    invoiceTotalForYes += invoiceamount;
                                    invoiceTotal += invoiceamount;
                                    toRev += toRevenue;
                                    closeBal += closingbalance;
                                    marine.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                }
                            }

                            else if (glid == 5163)
                            {
                                if (Difference >= 0 && trial2 >= 0)
                                {
                                    budgettottrunk += budget;
                                    invoicetottrunk += invoiceamount;
                                    torevtrunk += toRevenue;
                                    fromrevtrunk += fromRevenue;
                                    closingtottrunk += closingbalance;
                                    balancebftrunk += opp;
                                    fromRev += fromRevenue;
                                    budgettotal += budget;
                                    balancebf += opp;
                                    invoiceTotalForYes += invoiceamount;
                                    invoiceTotal += invoiceamount;
                                    toRev += toRevenue;
                                    closeBal += closingbalance;
                                    trunking.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                }
                            }

                            else if (glid == 5164)
                            {
                                if (Difference >= 0 && trial2 >= 0)
                                {
                                    budgettotother += budget;
                                    invoicetotother += invoiceamount;
                                    torevother += toRevenue;
                                    fromrevother += fromRevenue;
                                    closingtotother += closingbalance;
                                    balancebfother += opp;
                                    fromRev += fromRevenue;
                                    budgettotal += budget;
                                    balancebf += opp;
                                    invoiceTotalForYes += invoiceamount;
                                    invoiceTotal += invoiceamount;
                                    toRev += toRevenue;
                                    closeBal += closingbalance;
                                    other.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                }
                            }
                            else
                            {
                                table.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), Math.Round(budget, 2), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                            }

                        }
                        else
                        {

                            if (ValidityS.Month != defered.Month && ValidityS.Day <= 15 && IsEmpty(IfIsCredMemo))
                            {
                                if (Difference >= 0 && trial2 >= 0)
                                {

                                    Difference = Difference + 1;
                                    trial2 = validity - Difference;
                                    toRevenue = invoiceamount * 1 / validity;

                                    decimal amountalreadypaid = toRevenue * (Difference - 1);
                                    openingbalance = invoiceamount - amountalreadypaid;
                                    fromRevenue = 0;
                                    closingbalance = invoiceamount - (amountalreadypaid + toRevenue);

                                    invoicestat = "No";

                                    if (!IsEmpty(opset))
                                    {
                                        opp = opp;
                                    }
                                    else
                                        opp = 0;

                                    if (Difference > 1 && IsEmpty(opset))
                                    {
                                        toRevenue = toRevenue * Difference;
                                        fromRevenue = invoiceamount;
                                    }
                                    if (description == "Modification" && Difference == 0)
                                    {
                                        toRevenue = 0;
                                    }
                                    if (description == "Modification" && defered.Month == createdate.Month && defered.Year == createdate.Year)
                                    {
                                        fromRevenue = invoiceamount;
                                        invoicestat = "Yes";
                                    }
                                }
                                if (glid == 5156)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotcell += budget;
                                        invoicetotcell += invoiceamount;
                                        torevcell += toRevenue;
                                        fromremcell += fromRevenue;
                                        closingtotcell += closingbalance;
                                        balancebfcell += opp;
                                        fromRev += fromRevenue;
                                        balancebf += opp;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        budgettotal += budget;
                                        closeBal += closingbalance;
                                        cell.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5157)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotbb += budget;
                                        invoicetotbb += invoiceamount;
                                        torevbb += toRevenue;
                                        fromrembb += fromRevenue;
                                        closingtotbb += closingbalance;
                                        balancebfbb += opp;
                                        fromRev += fromRevenue;
                                        balancebf += opp;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        budgettotal += budget;
                                        closeBal += closingbalance;
                                        bbrand.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }
                                else if (glid == 5158)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotmic += budget;
                                        invoicetotmicro += invoiceamount;
                                        torevmicro += toRevenue;
                                        fromrevmicro += fromRevenue;
                                        closingtotmicro += closingbalance;
                                        balancebfmicro += opp;
                                        fromRev += fromRevenue;
                                        balancebf += opp;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        budgettotal += budget;
                                        closeBal += closingbalance;
                                        micro.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5159)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotds += budget;
                                        invoicetotds += invoiceamount;
                                        torevds += toRevenue;
                                        fromrevds += fromRevenue;
                                        closingtotds += closingbalance;
                                        balancebfds += opp;
                                        fromRev += fromRevenue;
                                        balancebf += opp;
                                        budgettotal += budget;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        dServices.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5160)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotvsat += budget;
                                        invoicetotvsat += invoiceamount;
                                        torevvsat += toRevenue;
                                        fromrevvsat += fromRevenue;
                                        closingtotvsat += closingbalance;
                                        balancebfvsat += opp;
                                        fromRev += fromRevenue;
                                        balancebf += opp;
                                        budgettotal += budget;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        vsat.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5161)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotaero += budget;
                                        invoicetotaero += invoiceamount;
                                        torevaero += toRevenue;
                                        fromrevaero += fromRevenue;
                                        closingtotaero += closingbalance;
                                        balancebfaero += opp;
                                        fromRev += fromRevenue;
                                        balancebf += opp;
                                        budgettotal += budget;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        aero.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5162)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotmar += budget;
                                        invoicetotmar += invoiceamount;
                                        torevmar += toRevenue;
                                        fromrevmar += fromRevenue;
                                        closingtotmar += closingbalance;
                                        balancebfmar += opp;
                                        fromRev += fromRevenue;
                                        balancebf += opp;
                                        budgettotal += budget;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        marine.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5163)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettottrunk += budget;
                                        invoicetottrunk += invoiceamount;
                                        torevtrunk += toRevenue;
                                        fromrevtrunk += fromRevenue;
                                        closingtottrunk += closingbalance;
                                        balancebftrunk += opp;
                                        fromRev += fromRevenue;
                                        balancebf += opp;
                                        budgettotal += budget;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        trunking.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5164)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotother += budget;
                                        invoicetotother += invoiceamount;
                                        torevother += toRevenue;
                                        fromrevother += fromRevenue;
                                        closingtotother += closingbalance;
                                        balancebfother += opp;
                                        fromRev += fromRevenue;
                                        budgettotal += budget;
                                        balancebf += opp;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        other.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }
                                else
                                {
                                    table.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                }

                            }

                            else if (ValidityS.Day > 15 && trial2 == 0 && IsEmpty(IfIsCredMemo))
                            {
                                if (Difference >= 0 && trial2 >= 0)
                                {
                                    toRevenue = invoiceamount * 1 / validity;

                                    decimal amountalreadypaid = toRevenue * (Difference - 1);
                                    openingbalance = invoiceamount - amountalreadypaid;
                                    fromRevenue = 0;
                                    closingbalance = invoiceamount - (amountalreadypaid + toRevenue);

                                    invoicestat = "No";


                                    if (!IsEmpty(opset))
                                    {
                                        opp = opp;

                                    }
                                    else
                                        opp = 0;



                                    if (Difference > 1 && IsEmpty(opset))
                                    {
                                        toRevenue = toRevenue * Difference;
                                        fromRevenue = invoiceamount;
                                    }
                                    if (description == "Modification" && Difference == 0)
                                    {
                                        toRevenue = 0;
                                    }
                                    if (description == "Modification" && defered.Month == createdate.Month && defered.Year == createdate.Year)
                                    {
                                        fromRevenue = invoiceamount;
                                        invoicestat = "Yes";
                                    }
                                }
                                if (glid == 5156)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotcell += budget;
                                        invoicetotcell += invoiceamount;
                                        torevcell += toRevenue;
                                        fromremcell += fromRevenue;
                                        closingtotcell += closingbalance;
                                        balancebfcell += opp;
                                        fromRev += fromRevenue;
                                        balancebf += opp;
                                        budgettotal += budget;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        cell.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5157)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotbb += budget;
                                        invoicetotbb += invoiceamount;
                                        torevbb += toRevenue;
                                        fromrembb += fromRevenue;
                                        closingtotbb += closingbalance;
                                        balancebfbb += opp;
                                        fromRev += fromRevenue;
                                        balancebf += opp;
                                        budgettotal += budget;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        bbrand.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }
                                else if (glid == 5158)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotmic += budget;
                                        invoicetotmicro += invoiceamount;
                                        torevmicro += toRevenue;
                                        fromrevmicro += fromRevenue;
                                        closingtotmicro += closingbalance;
                                        balancebfmicro += opp;
                                        fromRev += fromRevenue;
                                        balancebf += opp;
                                        budgettotal += budget;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        micro.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5159)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotds += budget;
                                        invoicetotds += invoiceamount;
                                        torevds += toRevenue;
                                        fromrevds += fromRevenue;
                                        closingtotds += closingbalance;
                                        balancebfds += opp;
                                        fromRev += fromRevenue;
                                        budgettotal += budget;
                                        balancebf += opp;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        dServices.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5160)
                                {

                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotvsat += budget;
                                        invoicetotvsat += invoiceamount;
                                        torevvsat += toRevenue;
                                        fromrevvsat += fromRevenue;
                                        closingtotvsat += closingbalance;
                                        balancebfvsat += opp;
                                        fromRev += fromRevenue;
                                        budgettotal += budget;
                                        balancebf += opp;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        vsat.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5161)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotaero += budget;
                                        invoicetotaero += invoiceamount;
                                        torevaero += toRevenue;
                                        fromrevaero += fromRevenue;
                                        closingtotaero += closingbalance;
                                        balancebfaero += opp;
                                        fromRev += fromRevenue;
                                        budgettotal += budget;
                                        balancebf += opp;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;

                                        aero.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5162)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotmar += budget;
                                        invoicetotmar += invoiceamount;
                                        torevmar += toRevenue;
                                        fromrevmar += fromRevenue;
                                        closingtotmar += closingbalance;
                                        balancebfmar += opp;
                                        fromRev += fromRevenue;
                                        budgettotal += budget;
                                        balancebf += opp;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        marine.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5163)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettottrunk += budget;
                                        invoicetottrunk += invoiceamount;
                                        torevtrunk += toRevenue;
                                        fromrevtrunk += fromRevenue;
                                        closingtottrunk += closingbalance;
                                        balancebftrunk += opp;
                                        fromRev += fromRevenue;
                                        budgettotal += budget;
                                        balancebf += opp;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        trunking.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5164)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotother += budget;
                                        invoicetotother += invoiceamount;
                                        torevother += toRevenue;
                                        fromrevother += fromRevenue;
                                        closingtotother += closingbalance;
                                        balancebfother += opp;
                                        fromRev += fromRevenue;
                                        budgettotal += budget;
                                        balancebf += opp;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        other.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }
                                else
                                {
                                    table.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                }
                            }
                            else if (ValidityS.Month != defered.Month && ValidityS.Day > 15 && IsEmpty(IfIsCredMemo))
                            {
                                if (Difference >= 0 && trial2 >= 0)
                                {
                                    toRevenue = invoiceamount * 1 / validity;
                                    decimal amountalreadypaid = toRevenue * (Difference - 1);
                                    openingbalance = invoiceamount - amountalreadypaid;
                                    fromRevenue = 0;
                                    closingbalance = invoiceamount - (amountalreadypaid + toRevenue);
                                    invoicestat = "No";

                                    if (!IsEmpty(opset))
                                    {
                                        opp = opp;
                                    }
                                    else
                                        opp = 0;
                                    //  balancebf += opp;

                                    if (Difference > 1 && IsEmpty(opset))
                                    {
                                        toRevenue = toRevenue * Difference;
                                        fromRevenue = invoiceamount;
                                    }
                                    if (description == "Modification" && Difference == 0)
                                    {
                                        toRevenue = 0;
                                    }

                                    if (description == "Modification" && defered.Month == createdate.Month && defered.Year == createdate.Year)
                                    {
                                        fromRevenue = invoiceamount;
                                        invoicestat = "Yes";
                                    }
                                }
                                if (glid == 5156)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotcell += budget;
                                        invoicetotcell += invoiceamount;
                                        torevcell += toRevenue;
                                        fromremcell += fromRevenue;
                                        closingtotcell += closingbalance;
                                        balancebfcell += opp;
                                        fromRev += fromRevenue;
                                        budgettotal += budget;
                                        balancebf += opp;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        cell.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5157)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotbb += budget;
                                        invoicetotbb += invoiceamount;
                                        torevbb += toRevenue;
                                        fromrembb += fromRevenue;
                                        closingtotbb += closingbalance;
                                        balancebfbb += opp;
                                        fromRev += fromRevenue;
                                        budgettotal += budget;
                                        balancebf += opp;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        bbrand.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }
                                else if (glid == 5158)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotmic += budget;
                                        invoicetotmicro += invoiceamount;
                                        torevmicro += toRevenue;
                                        fromrevmicro += fromRevenue;
                                        closingtotmicro += closingbalance;
                                        balancebfmicro += opp;
                                        fromRev += fromRevenue;
                                        budgettotal += budget;
                                        balancebf += opp;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        micro.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5159)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotds += budget;
                                        invoicetotds += invoiceamount;
                                        torevds += toRevenue;
                                        fromrevds += fromRevenue;
                                        closingtotds += closingbalance;
                                        balancebfds += opp;
                                        fromRev += fromRevenue;
                                        budgettotal += budget;
                                        balancebf += opp;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        dServices.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5160)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotvsat += budget;
                                        invoicetotvsat += invoiceamount;
                                        torevvsat += toRevenue;
                                        fromrevvsat += fromRevenue;
                                        closingtotvsat += closingbalance;
                                        balancebfvsat += opp;
                                        fromRev += fromRevenue;
                                        budgettotal += budget;
                                        balancebf += opp;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        vsat.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5161)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotaero += budget;
                                        invoicetotaero += invoiceamount;
                                        torevaero += toRevenue;
                                        fromrevaero += fromRevenue;
                                        closingtotaero += closingbalance;
                                        balancebfaero += opp;
                                        fromRev += fromRevenue;
                                        budgettotal += budget;
                                        balancebf += opp;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        aero.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5162)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotmar += budget;
                                        invoicetotmar += invoiceamount;
                                        torevmar += toRevenue;
                                        fromrevmar += fromRevenue;
                                        closingtotmar += closingbalance;
                                        balancebfmar += opp;
                                        budgettotal += budget;
                                        fromRev += fromRevenue;
                                        balancebf += opp;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        marine.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5163)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettottrunk += budget;
                                        invoicetottrunk += invoiceamount;
                                        torevtrunk += toRevenue;
                                        fromrevtrunk += fromRevenue;
                                        closingtottrunk += closingbalance;
                                        balancebftrunk += opp;
                                        fromRev += fromRevenue;
                                        budgettotal += budget;
                                        balancebf += opp;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        trunking.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5164)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotother += budget;
                                        invoicetotother += invoiceamount;
                                        torevother += toRevenue;
                                        fromrevother += fromRevenue;
                                        closingtotother += closingbalance;
                                        balancebfother += opp;
                                        budgettotal += budget;
                                        fromRev += fromRevenue;
                                        balancebf += opp;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        other.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }
                                else
                                {
                                    table.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), Math.Round(invoiceamount, 2), invoicestat, Math.Round(opp, 2), Math.Round(fromRevenue, 2), Math.Round(-toRevenue, 2), Math.Round(closingbalance, 2), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                }
                            }

                        }

                        if (invoiceid == 16302)
                        {
                            int a = 0;
                        }

                        if (Difference > 0 || description == "Modification" && Difference == 0 || Difference == 0 && ValidityS.Month == defered.Month && ValidityS.Year == defered.Year && ValidityS.Day > 15)
                        {
                          
                            DataSet opstatus = new DataSet();
                            opstatus = _000.App_Code.BusinessClass.GetOpeningBalStat(ccnum, invoiceid, defered);
                            if (IsEmpty(IfIsCredMemo))
                            {
                                if (IsEmpty(opstatus))
                                {
                                    _000.App_Code.BusinessClass.InsertOpBalNew(ccnum, closingbalance, defered, invoiceid, ValidityS, ValidityF);
                                }
                                else
                                {
                                    _000.App_Code.BusinessClass.UpdateOpBalNew(ccnum, closingbalance, defered, invoiceid, ValidityS, ValidityF);
                                }
                            }
                        }
                    }

                while (rdr2.Read())
                    {
                        //id = rdr.GetInt32(0);
                        ccnum = "";
                        // Company = rdr.GetString(2);
                        // fname = rdr.GetString(3);
                        //  lname = rdr.GetString(4);
                        invoiceamount = rdr2.GetDecimal(2);
                        string s = "04/01/2017";
                        string e = "03/31/2018";
                        ValidityS = Convert.ToDateTime(s);
                        ValidityF = Convert.ToDateTime(e);
                        glid = 0;
                        //  description = rdr.GetString(9);
                        invoiceid = rdr2.GetInt32(0);
                        DataSet clientidds = new DataSet();
                        clientidds = _000.App_Code.BusinessClass.GetClientId(invoiceid);
                        if (!IsEmpty(clientidds))
                        {

                            DataRow dr = clientidds.Tables[0].Rows[0];
                            id = Convert.ToInt32(dr.ItemArray.GetValue(0));
                            ccnum = id.ToString();
                        }

                        DataSet clientnameds = new DataSet();
                        clientnameds = _000.App_Code.BusinessClass.GetClientName(id);
                        if (!IsEmpty(clientnameds))
                        {

                            DataRow dr = clientnameds.Tables[0].Rows[0];
                            Company = dr.ItemArray.GetValue(0).ToString();
                            fname = dr.ItemArray.GetValue(1).ToString();
                            lname = dr.ItemArray.GetValue(2).ToString();
                            ccnum = dr.ItemArray.GetValue(3).ToString();
                        }

                        DataSet validityds = new DataSet();
                        validityds = _000.App_Code.BusinessClass.GetValidity(invoiceid, defered.Year, (defered.Month-1));
                        if (!IsEmpty(validityds))
                        {
                           
                            DataRow dr = validityds.Tables[0].Rows[0];
                            string val = dr.ItemArray.GetValue(0).ToString();

                            if (val != "")
                            {
                                ValidityS = Convert.ToDateTime(dr.ItemArray.GetValue(0).ToString());
                                ValidityF = Convert.ToDateTime(dr.ItemArray.GetValue(1).ToString());
                            }

                        }

                        DataSet glds = new DataSet();
                        glds = _000.App_Code.BusinessClass.GetCreditGl(invoiceid);
                        if (!IsEmpty(glds))
                        {

                            DataRow dr = glds.Tables[0].Rows[0];
                            glid = Convert.ToInt32(dr.ItemArray.GetValue(0));
                        }




                        // DateTime createdate = rdr.GetDateTime(11);
                        int Differencee = ((defered.Year - ValidityS.Year) * 12) + defered.Month - ValidityS.Month;
                        decimal budget = 0;
                        int validity = 0;
                        int Difference = 0;
                        //  int validity = ((ValidityF.Year - ValidityS.Year) * 12) + ValidityF.Month - ValidityS.Month;
                        var ValiditySS = DateTime.Now.ToString("dd/MM/yyyy");

                        validity = CheckMonths(ValidityS, ValidityF);
                        if (validity == 2) validity = 3;
                        Difference = ((defered.Year - ValidityS.Year) * 12) + defered.Month - ValidityS.Month;
                        ValiditySS = ValidityS.ToString("dd/MM/yyyy");

                        int trial33 = validity - Differencee;
                        Differencee = Differencee + 1;
                        DataSet df = new DataSet();
                        df = _000.App_Code.BusinessClass.GetBudget(ccnum, invoiceid);
                        if (!IsEmpty(df) && Differencee > 1)
                        {

                            DataRow dr = df.Tables[0].Rows[0];
                            budget = Convert.ToDecimal(dr.ItemArray.GetValue(1));

                        }


                        if (!IsEmpty(df) && Differencee == 1)
                        {

                            DataRow dr = df.Tables[0].Rows[0];
                            DateTime ne = Convert.ToDateTime(dr.ItemArray.GetValue(2));
                            if (ne.Month >= ValidityS.Month /*&& ne.Year >= ValidityS.Year*/)
                            {
                                budget = Convert.ToDecimal(dr.ItemArray.GetValue(1));
                            }
                        }


                        DataSet budg = _000.App_Code.BusinessClass.GetCustBudget(ccnum, invoiceid);
                        if (IsEmpty(budg))
                        {
                            _000.App_Code.BusinessClass.InsertBudgetInfo(ccnum, budget, invoiceid);
                        }



                        var ValidityFF = ValidityF.ToString("dd/MM/yyyy");



                        //------------------------------------------------------------------------------------

                        string invoicestat = " ";

                        string clientCompany = " ";
                        decimal openingbalance = 0;
                        decimal closingbalance = 0;
                        decimal toRevenue = 0;
                        decimal fromRevenue = 0;



                        if (Company == null || Company == "")
                        {
                            clientCompany = fname + " " + lname;

                        }
                        else
                        {
                            clientCompany = Company;
                            clientCompany = System.Net.WebUtility.HtmlDecode(clientCompany);
                        }


                        int trial = ((defered.Year - ValidityS.Year) * 12) + validity - Difference;
                        int trial2 = validity - Difference;


                        decimal opp = 0;
                        string op = " ";
                        DataSet opset = new DataSet();


                        {
                            if (Difference > 0 && ValidityS.Month != defered.Month || ValidityS.Month == defered.Month && ValidityF.Year != defered.Year && ValidityS.Year != defered.Year || ValidityF.Month == defered.Month)
                            {
                                if (defered.Month - 1 == 0)
                                {
                                    opset = _000.App_Code.BusinessClass.GetOpeningBalanceForMonth(ccnum, 12, (defered.Year - 1), invoiceid);
                                }

                                else
                                    opset = _000.App_Code.BusinessClass.GetOpeningBalanceForMonth(ccnum, (defered.Month - 1), defered.Year, invoiceid);
                                if (!IsEmpty(opset))
                                {
                                    DataRow dr = opset.Tables[0].Rows[0];


                                    decimal.TryParse(dr["openingbalance"].ToString(), out opp);


                                }
                                else
                                    opp = 0;
                            }

                            if (opp == 0)
                            {
                                opp = 0;
                            }
                            else
                            {
                                opp = opp;
                            }
                        }



                        if (Difference >= 0 && trial2 >= 0 && opp!=0)
                        {
                            if (ValidityS.Day <= 15)
                            {
                                Difference = Difference + 1;
                            }
                 
                            trial2 = validity - Difference;
                            openingbalance = opp;
                            toRevenue = invoiceamount * 1 / validity;
                            closingbalance = 0;
                            decimal amountalreadypaid = toRevenue * (Difference - 1);
                            toRevenue = amountalreadypaid;
                            fromRevenue = invoiceamount;
                            invoicestat = "No";
                        }

                        if (glid == 5156 && opp != 0)
                        {
                            if (Difference >= 0 && trial2 >= 0)
                            {
                                invoicetotcell += invoiceamount;
                                torevcell += -toRevenue;
                                fromremcell += -fromRevenue;
                                closingtotcell += closingbalance;
                                balancebfcell += openingbalance;
                                budgettotcell += budget;
                                fromRev += -fromRevenue;
                                balancebf += opp;
                                invoiceTotalForYes += invoiceamount;
                                invoiceTotal += invoiceamount;
                                toRev += -toRevenue;
                                budgettotal += budget;
                                closeBal += closingbalance;
                                cell.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(-fromRevenue, 2)), formatMoney(Math.Round(toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                            }
                        }

                        else if (glid == 5157 && opp != 0)
                        {
                            if (Difference >= 0 && trial2 >= 0)
                            {
                                budgettotbb += budget;
                                invoicetotbb += invoiceamount;
                                torevbb += -toRevenue;
                                fromrembb += -fromRevenue;
                                closingtotbb += closingbalance;
                                balancebfbb += openingbalance;
                                fromRev += -fromRevenue;
                                balancebf += opp;
                                invoiceTotalForYes += invoiceamount;
                                invoiceTotal += invoiceamount;
                                toRev += -toRevenue;
                                budgettotal += budget;
                                closeBal += closingbalance;
                                bbrand.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(-fromRevenue, 2)), formatMoney(Math.Round(toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                            }
                        }

                        else if (glid == 5158 && opp != 0)
                        {
                            if (Difference >= 0 && trial2 >= 0)
                            {
                                budgettotmic += budget;
                                invoicetotmicro += invoiceamount;
                                torevmicro += -toRevenue;
                                fromrevmicro += -fromRevenue;
                                closingtotmicro += closingbalance;
                                balancebfmicro += openingbalance;
                                fromRev += -fromRevenue;
                                balancebf += opp;
                                invoiceTotalForYes += invoiceamount;
                                invoiceTotal += invoiceamount;
                                toRev += -toRevenue;
                                budgettotal += budget;
                                closeBal += closingbalance;
                                micro.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(-fromRevenue, 2)), formatMoney(Math.Round(toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);

                            }
                        }

                        else if (glid == 5159 && opp != 0)
                        {
                            if (Difference >= 0 && trial2 >= 0)
                            {
                                budgettotds += budget;
                                invoicetotds += invoiceamount;
                                torevds += -toRevenue;
                                fromrevds += -fromRevenue;
                                closingtotds += closingbalance;
                                balancebfds += openingbalance;
                                fromRev += -fromRevenue;
                                balancebf += opp;
                                invoiceTotalForYes += invoiceamount;
                                invoiceTotal += invoiceamount;
                                toRev += -toRevenue;
                                budgettotal += budget;
                                closeBal += closingbalance;
                                dServices.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(-fromRevenue, 2)), formatMoney(Math.Round(toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                            }
                        }

                        else if (glid == 5160 && opp != 0)
                        {
                            if (Difference >= 0 && trial2 >= 0)
                            {
                                budgettotvsat += budget;
                                invoicetotvsat += invoiceamount;
                                torevvsat += -toRevenue;
                                fromrevvsat += -fromRevenue;
                                closingtotvsat += closingbalance;
                                balancebfvsat += openingbalance;
                                fromRev += -fromRevenue;
                                balancebf += opp;
                                budgettotal += budget;
                                invoiceTotalForYes += invoiceamount;
                                invoiceTotal += invoiceamount;
                                toRev += -toRevenue;
                                closeBal += closingbalance;
                                vsat.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(-fromRevenue, 2)), formatMoney(Math.Round(toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);

                            }
                        }

                        else if (glid == 5161 && opp != 0)
                        {
                            if (Difference >= 0 && trial2 >= 0)
                            {
                                budgettotaero += budget;
                                invoicetotaero += invoiceamount;
                                torevaero += -toRevenue;
                                fromrevaero += -fromRevenue;
                                closingtotaero += closingbalance;
                                balancebfaero += openingbalance;
                                fromRev += -fromRevenue;
                                balancebf += opp;
                                budgettotal += budget;
                                invoiceTotalForYes += invoiceamount;
                                invoiceTotal += invoiceamount;
                                toRev += -toRevenue;
                                closeBal += closingbalance;
                                aero.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(-fromRevenue, 2)), formatMoney(Math.Round(toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                            }
                        }


                        else if (glid == 5162 && opp != 0)
                        {
                            if (Difference >= 0 && trial2 >= 0)
                            {
                                budgettotmar += budget;
                                invoicetotmar += invoiceamount;
                                torevmar += -toRevenue;
                                fromrevmar += -fromRevenue;
                                closingtotmar += closingbalance;
                                balancebfmar += openingbalance;
                                fromRev += -fromRevenue;
                                balancebf += opp;
                                invoiceTotalForYes += invoiceamount;
                                invoiceTotal += invoiceamount;
                                budgettotal += budget;
                                toRev += -toRevenue;
                                closeBal += closingbalance;
                                marine.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(-fromRevenue, 2)), formatMoney(Math.Round(toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                            }
                        }

                        else if (glid == 5163 && opp != 0)
                        {
                            if (Difference >= 0 && trial2 >= 0)
                            {
                                budgettottrunk += budget;
                                invoicetottrunk += invoiceamount;
                                torevtrunk += -toRevenue;
                                fromrevtrunk += -fromRevenue;
                                closingtottrunk += closingbalance;
                                balancebftrunk += openingbalance;
                                fromRev += -fromRevenue;
                                balancebf += opp;
                                budgettotal += budget;
                                invoiceTotalForYes += invoiceamount;
                                invoiceTotal += invoiceamount;
                                toRev += -toRevenue;
                                closeBal += closingbalance;
                                trunking.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(-fromRevenue, 2)), formatMoney(Math.Round(toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                            }
                        }

                        else if (glid == 5164 && opp != 0)
                        {
                            if (Difference >= 0 && trial2 >= 0)
                            {
                                budgettotother += budget;
                                invoicetotother += invoiceamount;
                                torevother += -toRevenue;
                                fromrevother += -fromRevenue;
                                closingtotother += closingbalance;
                                balancebfother += openingbalance;
                                fromRev += -fromRevenue;
                                balancebf += opp;
                                budgettotal += budget;
                                invoiceTotalForYes += invoiceamount;
                                invoiceTotal += invoiceamount;
                                toRev += -toRevenue;
                                closeBal += closingbalance;
                                other.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(-fromRevenue, 2)), formatMoney(Math.Round(toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                            }
                        }
                        else
                        {
                            table.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(-fromRevenue, 2)), formatMoney(Math.Round(toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                        }



                        if (Difference > 0 || description == "Modification" && Difference == 0 && opp!=0)
                        {
                            DataSet opstatus = new DataSet();
                            opstatus = _000.App_Code.BusinessClass.GetOpeningBalStat(ccnum, invoiceid, defered);
                            if (IsEmpty(opstatus))
                            {
                                _000.App_Code.BusinessClass.InsertOpBal(ccnum, closingbalance, defered, invoiceid);
                            }
                            else
                            {
                                _000.App_Code.BusinessClass.UpdateOpBal(ccnum, closingbalance, defered, invoiceid);
                            }
                        }
                    }

                while (rdr3.Read())
                    {
                        //id = rdr.GetInt32(0);
                        ccnum = "";
                        // Company = rdr.GetString(2);
                        // fname = rdr.GetString(3);
                        //  lname = rdr.GetString(4);
                        invoiceamount = rdr3.GetDecimal(2);
                        string s = "04/01/2017";
                        string e = "03/31/2018";
                        ValidityS = Convert.ToDateTime(s);
                        ValidityF = Convert.ToDateTime(e);
                        int creditmemonum = rdr3.GetInt32(3);

                        DataSet creditmemono = new DataSet();
                        creditmemono = _000.App_Code.BusinessClass.GetCreditMemoDisplayNo(creditmemonum);
                        if (!IsEmpty(creditmemono))
                        {

                            DataRow dr = creditmemono.Tables[0].Rows[0];
                            creditmemonum = Convert.ToInt32(dr.ItemArray.GetValue(0));
                        }
                        glid = 0;
                        //  description = rdr.GetString(9);
                        invoiceid = rdr3.GetInt32(0);
                        DataSet clientidds = new DataSet();
                        clientidds = _000.App_Code.BusinessClass.GetClientId(invoiceid);
                        if (!IsEmpty(clientidds))
                        {

                            DataRow dr = clientidds.Tables[0].Rows[0];
                            id = Convert.ToInt32(dr.ItemArray.GetValue(0));
                        }

                        DataSet invamountds = new DataSet();
                        invamountds = _000.App_Code.BusinessClass.GetInvoiceAmountCredMemo(invoiceid);
                        if (!IsEmpty(invamountds))
                        {

                            DataRow dr = invamountds.Tables[0].Rows[0];
                            invoiceamount = Convert.ToDecimal(dr.ItemArray.GetValue(0));
                        }

                        DataSet clientnameds = new DataSet();
                        clientnameds = _000.App_Code.BusinessClass.GetClientName(id);
                        if (!IsEmpty(clientnameds))
                        {

                            DataRow dr = clientnameds.Tables[0].Rows[0];
                            Company = dr.ItemArray.GetValue(0).ToString();
                            fname = dr.ItemArray.GetValue(1).ToString();
                            lname = dr.ItemArray.GetValue(2).ToString();
                            ccnum = dr.ItemArray.GetValue(3).ToString();
                           
                        }

                        DataSet validityds = new DataSet();
                        validityds = _000.App_Code.BusinessClass.GetValidityCM(invoiceid);
                        if (!IsEmpty(validityds))
                        {

                            DataRow dr = validityds.Tables[0].Rows[0];
                            ValidityS = Convert.ToDateTime(dr.ItemArray.GetValue(6).ToString());
                            ValidityF = Convert.ToDateTime(dr.ItemArray.GetValue(7).ToString());

                        }

                        DataSet glds = new DataSet();
                        glds = _000.App_Code.BusinessClass.GetCreditGl(invoiceid);
                        if (!IsEmpty(glds))
                        {

                            DataRow dr = glds.Tables[0].Rows[0];
                            glid = Convert.ToInt32(dr.ItemArray.GetValue(0));
                        }




                        // DateTime createdate = rdr.GetDateTime(11);
                        int Differencee = ((defered.Year - ValidityS.Year) * 12) + defered.Month - ValidityS.Month;
                        decimal budget = 0;
                        int validity = 0;
                        int Difference = 0;
                        //  int validity = ((ValidityF.Year - ValidityS.Year) * 12) + ValidityF.Month - ValidityS.Month;
                        var ValiditySS = DateTime.Now.ToString("dd/MM/yyyy");

                        validity = CheckMonths(ValidityS, ValidityF);
                        if (validity == 2) validity = 3;
                        Difference = ((defered.Year - ValidityS.Year) * 12) + defered.Month - ValidityS.Month;
                        ValiditySS = ValidityS.ToString("dd/MM/yyyy");

                        int trial33 = validity - Differencee;
                        Differencee = Differencee + 1;
                        DataSet df = new DataSet();
                        df = _000.App_Code.BusinessClass.GetBudget(ccnum, invoiceid);
                        if (!IsEmpty(df) && Differencee > 1)
                        {

                            DataRow dr = df.Tables[0].Rows[0];
                            budget = Convert.ToDecimal(dr.ItemArray.GetValue(1));

                        }


                        if (!IsEmpty(df) && Differencee == 1)
                        {

                            DataRow dr = df.Tables[0].Rows[0];
                            DateTime ne = Convert.ToDateTime(dr.ItemArray.GetValue(2));
                            if (ne.Month >= ValidityS.Month /*&& ne.Year >= ValidityS.Year*/)
                            {
                                budget = Convert.ToDecimal(dr.ItemArray.GetValue(1));
                            }
                        }


                        DataSet budg = _000.App_Code.BusinessClass.GetCustBudget(ccnum, invoiceid);
                        if (IsEmpty(budg))
                        {
                            _000.App_Code.BusinessClass.InsertBudgetInfo(ccnum, budget, invoiceid);
                        }



                        var ValidityFF = ValidityF.ToString("dd/MM/yyyy");



                        //------------------------------------------------------------------------------------

                        string invoicestat = " ";
                        string clientCompany = " ";
                        decimal openingbalance = 0;
                        decimal closingbalance = 0;
                        decimal toRevenue = 0;
                        decimal fromRevenue = 0;



                        if (Company == null || Company == "")
                        {
                            clientCompany = fname + " " + lname;

                        }
                        else
                        {
                            clientCompany = Company;
                            clientCompany = System.Net.WebUtility.HtmlDecode(clientCompany);
                        }


                        int trial = ((defered.Year - ValidityS.Year) * 12) + validity - Difference;
                        int trial2 = validity - Difference;


                        decimal opp = 0;
                        string op = " ";
                        DataSet opset = new DataSet();


                        {
                            if (Difference > 0 && ValidityS.Month != defered.Month || ValidityS.Month == defered.Month && ValidityF.Year != defered.Year && ValidityS.Year != defered.Year || ValidityF.Month == defered.Month)
                            {
                                if (defered.Month - 1 == 0)
                                {
                                    opset = _000.App_Code.BusinessClass.GetOpeningBalanceForMonth(ccnum, 12, (defered.Year - 1), invoiceid);
                                }

                                else
                                    opset = _000.App_Code.BusinessClass.GetOpeningBalanceForMonth(ccnum, (defered.Month - 1), defered.Year, invoiceid);
                                if (!IsEmpty(opset))
                                {
                                    DataRow dr = opset.Tables[0].Rows[0];


                                    decimal.TryParse(dr["openingbalance"].ToString(), out opp);


                                }
                                else
                                    opp = 0;
                            }

                            if (opp == 0)
                            {
                                opp = 0;
                            }
                            else
                            {
                                opp = opp;
                            }
                        }



                        if (Difference >= 0 && trial2 >= 0 && opp!=0)
                        {
                           if(ValidityS.Day <= 15)
                            {
                                Difference = Difference + 1;
                            }

                            trial2 = validity - Difference;
                            openingbalance = opp;
                            toRevenue = 0;
                            closingbalance = 0;
                            fromRevenue = opp;
                            invoicestat = "No";
                        }

                        if (glid == 5156 && opp != 0)
                        {
                            if (Difference >= 0 && trial2 >= 0)
                            {
                                invoicetotcell += invoiceamount;
                                torevcell += toRevenue;
                                fromremcell += -fromRevenue;
                                closingtotcell += closingbalance;
                                balancebfcell += openingbalance;
                                budgettotcell += budget;
                                fromRev += -fromRevenue;
                                balancebf += opp;
                                invoiceTotalForYes += invoiceamount;
                                invoiceTotal += invoiceamount;
                                toRev += toRevenue;
                                budgettotal += budget;
                                closeBal += closingbalance;
                                cell.Rows.Add(ccnum, clientCompany, invoiceid.ToString() + "/CN" + creditmemonum.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(-fromRevenue, 2)),formatMoney(Math.Round(toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2, ValiditySS, ValidityFF);
                            }
                        }

                        else if (glid == 5157 && opp != 0)
                        {
                            if (Difference >= 0 && trial2 >= 0)
                            {
                                budgettotbb += budget;
                                invoicetotbb += invoiceamount;
                                torevbb += toRevenue;
                                fromrembb += -fromRevenue;
                                closingtotbb += closingbalance;
                                balancebfbb += openingbalance;
                                fromRev += -fromRevenue;
                                balancebf += opp;
                                invoiceTotalForYes += invoiceamount;
                                invoiceTotal += invoiceamount;
                                toRev += toRevenue;
                                budgettotal += budget;
                                closeBal += closingbalance;
                                bbrand.Rows.Add(ccnum, clientCompany, invoiceid.ToString() + "/CN" + creditmemonum.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(-fromRevenue, 2)),formatMoney(Math.Round(toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                            }
                        }

                        else if (glid == 5158 && opp != 0)
                        {
                            if (Difference >= 0 && trial2 >= 0)
                            {
                                budgettotmic += budget;
                                invoicetotmicro += invoiceamount;
                                torevmicro += toRevenue;
                                fromrevmicro += -fromRevenue;
                                closingtotmicro += closingbalance;
                                balancebfmicro += openingbalance;
                                fromRev += -fromRevenue;
                                balancebf += opp;
                                invoiceTotalForYes += invoiceamount;
                                invoiceTotal += invoiceamount;
                                toRev += toRevenue;
                                budgettotal += budget;
                                closeBal += closingbalance;
                                micro.Rows.Add(ccnum, clientCompany, invoiceid.ToString() + "/CN" + creditmemonum.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(-fromRevenue, 2)), formatMoney(Math.Round(toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);

                            }
                        }

                        else if (glid == 5159 && opp != 0)
                        {
                            if (Difference >= 0 && trial2 >= 0)
                            {
                                budgettotds += budget;
                                invoicetotds += invoiceamount;
                                torevds += toRevenue;
                                fromrevds += -fromRevenue;
                                closingtotds += closingbalance;
                                balancebfds += openingbalance;
                                fromRev += -fromRevenue;
                                balancebf += opp;
                                invoiceTotalForYes += invoiceamount;
                                invoiceTotal += invoiceamount;
                                toRev += toRevenue;
                                budgettotal += budget;
                                closeBal += closingbalance;
                                dServices.Rows.Add(ccnum, clientCompany, invoiceid.ToString() + "/CN" + creditmemonum.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(-fromRevenue, 2)),formatMoney(Math.Round(toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                            }
                        }

                        else if (glid == 5160 && opp != 0)
                        {
                            if (Difference >= 0 && trial2 >= 0)
                            {
                                budgettotvsat += budget;
                                invoicetotvsat += invoiceamount;
                                torevvsat += toRevenue;
                                fromrevvsat += -fromRevenue;
                                closingtotvsat += closingbalance;
                                balancebfvsat += openingbalance;
                                fromRev += -fromRevenue;
                                balancebf += opp;
                                budgettotal += budget;
                                invoiceTotalForYes += invoiceamount;
                                invoiceTotal += invoiceamount;
                                toRev += toRevenue;
                                closeBal += closingbalance;
                                vsat.Rows.Add(ccnum, clientCompany, invoiceid.ToString() + "/CN" + creditmemonum.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(-fromRevenue, 2)),formatMoney(Math.Round(toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);

                            }
                        }

                        else if (glid == 5161 && opp != 0)
                        {
                            if (Difference >= 0 && trial2 >= 0)
                            {
                                budgettotaero += budget;
                                invoicetotaero += invoiceamount;
                                torevaero += toRevenue;
                                fromrevaero += -fromRevenue;
                                closingtotaero += closingbalance;
                                balancebfaero += openingbalance;
                                fromRev += -fromRevenue;
                                balancebf += opp;
                                budgettotal += budget;
                                invoiceTotalForYes += invoiceamount;
                                invoiceTotal += invoiceamount;
                                toRev += toRevenue;
                                closeBal += closingbalance;
                                aero.Rows.Add(ccnum, clientCompany, invoiceid.ToString() + "/CN" + creditmemonum.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(-fromRevenue, 2)),formatMoney(Math.Round(toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                            }
                        }


                        else if (glid == 5162 && opp != 0)
                        {
                            if (Difference >= 0 && trial2 >= 0)
                            {
                                budgettotmar += budget;
                                invoicetotmar += invoiceamount;
                                torevmar += toRevenue;
                                fromrevmar += -fromRevenue;
                                closingtotmar += closingbalance;
                                balancebfmar += openingbalance;
                                fromRev += -fromRevenue;
                                balancebf += opp;
                                invoiceTotalForYes += invoiceamount;
                                invoiceTotal += invoiceamount;
                                budgettotal += budget;
                                toRev += toRevenue;
                                closeBal += closingbalance;
                                marine.Rows.Add(ccnum, clientCompany, invoiceid.ToString() + "/CN" + creditmemonum.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(-fromRevenue, 2)),formatMoney(Math.Round(toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                            }
                        }

                        else if (glid == 5163 && opp != 0)
                        {
                            if (Difference >= 0 && trial2 >= 0)
                            {
                                budgettottrunk += budget;
                                invoicetottrunk += invoiceamount;
                                torevtrunk += toRevenue;
                                fromrevtrunk += -fromRevenue;
                                closingtottrunk += closingbalance;
                                balancebftrunk += openingbalance;
                                fromRev += -fromRevenue;
                                balancebf += opp;
                                budgettotal += budget;
                                invoiceTotalForYes += invoiceamount;
                                invoiceTotal += invoiceamount;
                                toRev += toRevenue;
                                closeBal += closingbalance;
                                trunking.Rows.Add(ccnum, clientCompany, invoiceid.ToString() + "/CN" + creditmemonum.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(-fromRevenue, 2)), formatMoney(Math.Round(toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                            }
                        }

                        else if (glid == 5164 && opp != 0)
                        {
                            if (Difference >= 0 && trial2 >= 0)
                            {
                                budgettotother += budget;
                                invoicetotother += invoiceamount;
                                torevother += toRevenue;
                                fromrevother += -fromRevenue;
                                closingtotother += closingbalance;
                                balancebfother += openingbalance;
                                fromRev += -fromRevenue;
                                balancebf += opp;
                                budgettotal += budget;
                                invoiceTotalForYes += invoiceamount;
                                invoiceTotal += invoiceamount;
                                toRev += toRevenue;
                                closeBal += closingbalance;
                                other.Rows.Add(ccnum, clientCompany, invoiceid.ToString() + "/CN" + creditmemonum.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(-fromRevenue, 2)), formatMoney(Math.Round(toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                            }
                        }
                        else
                        {
                            table.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(-fromRevenue, 2)), formatMoney(Math.Round(toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                        }





                        if (Difference > 0 || description == "Modification" && Difference == 0 && opp!=0)
                        {
                            DataSet opstatus = new DataSet();
                            opstatus = _000.App_Code.BusinessClass.GetOpeningBalStat(ccnum, invoiceid, defered);
                            if (IsEmpty(opstatus))
                            {
                                _000.App_Code.BusinessClass.InsertOpBal(ccnum, closingbalance, defered, invoiceid);
                            }
                            else
                            {
                                _000.App_Code.BusinessClass.UpdateOpBal(ccnum, closingbalance, defered, invoiceid);
                            }
                        }
                    }

                while (rdr4.Read())
                    {
                        //id = rdr.GetInt32(0);
                        ccnum = "";
                        // Company = rdr.GetString(2);
                        // fname = rdr.GetString(3);
                        //  lname = rdr.GetString(4);
                        invoiceamount = rdr4.GetDecimal(2);
                        string s = "04/01/2017";
                        string e = "03/31/2018";
                        ValidityS = Convert.ToDateTime(s);
                        ValidityF = Convert.ToDateTime(e);
                        int creditmemonum = rdr4.GetInt32(3);
                        DateTime creditmemodate = rdr4.GetDateTime(4);
                        DataSet IfIsCredMemo = _000.App_Code.BusinessClass.GetIsInvoiceCreditMemo(invoiceid);
                        DataSet creditmemono = new DataSet();
                        creditmemono = _000.App_Code.BusinessClass.GetCreditMemoDisplayNo(creditmemonum);
                        if (!IsEmpty(creditmemono))
                        {

                            DataRow dr = creditmemono.Tables[0].Rows[0];
                            creditmemonum = Convert.ToInt32(dr.ItemArray.GetValue(0));
                        }
                        glid = 0;
                        //  description = rdr.GetString(9);
                        invoiceid = rdr4.GetInt32(0);
                        DataSet clientidds = new DataSet();
                        clientidds = _000.App_Code.BusinessClass.GetClientId(invoiceid);
                        if (!IsEmpty(clientidds))
                        {

                            DataRow dr = clientidds.Tables[0].Rows[0];
                            id = Convert.ToInt32(dr.ItemArray.GetValue(0));
                        }

                        DataSet invamountds = new DataSet();
                        invamountds = _000.App_Code.BusinessClass.GetInvoiceAmountCredMemo(invoiceid);
                        if (!IsEmpty(invamountds))
                        {

                            DataRow dr = invamountds.Tables[0].Rows[0];
                            invoiceamount = Convert.ToDecimal(dr.ItemArray.GetValue(0));
                        }

                        DataSet clientnameds = new DataSet();
                        clientnameds = _000.App_Code.BusinessClass.GetClientName(id);
                        if (!IsEmpty(clientnameds))
                        {

                            DataRow dr = clientnameds.Tables[0].Rows[0];
                            Company = dr.ItemArray.GetValue(0).ToString();
                            fname = dr.ItemArray.GetValue(1).ToString();
                            lname = dr.ItemArray.GetValue(2).ToString();
                            ccnum = dr.ItemArray.GetValue(3).ToString();

                        }

                        DataSet validityds = new DataSet();
                        validityds = _000.App_Code.BusinessClass.GetValidityCM(invoiceid);
                        if (!IsEmpty(validityds))
                        {

                            DataRow dr = validityds.Tables[0].Rows[0];
                            ValidityS = Convert.ToDateTime(dr.ItemArray.GetValue(6).ToString());
                            ValidityF = Convert.ToDateTime(dr.ItemArray.GetValue(7).ToString());

                        }

                        DataSet glds = new DataSet();
                        glds = _000.App_Code.BusinessClass.GetCreditGl(invoiceid);
                        if (!IsEmpty(glds))
                        {

                            DataRow dr = glds.Tables[0].Rows[0];
                            glid = Convert.ToInt32(dr.ItemArray.GetValue(0));
                        }




                        // DateTime createdate = rdr.GetDateTime(11);
                        int Differencee = ((defered.Year - ValidityS.Year) * 12) + defered.Month - ValidityS.Month;
                        decimal budget = 0;
                        int validity = 0;
                        int Difference = 0;
                        //  int validity = ((ValidityF.Year - ValidityS.Year) * 12) + ValidityF.Month - ValidityS.Month;
                        var ValiditySS = DateTime.Now.ToString("dd/MM/yyyy");

                        validity = CheckMonths(ValidityS, ValidityF);
                        if (validity == 2) validity = 3;
                        Difference = ((defered.Year - ValidityS.Year) * 12) + defered.Month - ValidityS.Month;
                        ValiditySS = ValidityS.ToString("dd/MM/yyyy");

                        int trial33 = validity - Differencee;
                        Differencee = Differencee + 1;
                        DataSet df = new DataSet();
                        df = _000.App_Code.BusinessClass.GetBudget(ccnum, invoiceid);
                        if (!IsEmpty(df) && Differencee > 1)
                        {

                            DataRow dr = df.Tables[0].Rows[0];
                            budget = Convert.ToDecimal(dr.ItemArray.GetValue(1));

                        }


                        if (!IsEmpty(df) && Differencee == 1)
                        {

                            DataRow dr = df.Tables[0].Rows[0];
                            DateTime ne = Convert.ToDateTime(dr.ItemArray.GetValue(2));
                            if (ne.Month >= ValidityS.Month /*&& ne.Year >= ValidityS.Year*/)
                            {
                                budget = Convert.ToDecimal(dr.ItemArray.GetValue(1));
                            }
                        }


                        DataSet budg = _000.App_Code.BusinessClass.GetCustBudget(ccnum, invoiceid);
                        if (IsEmpty(budg))
                        {
                            _000.App_Code.BusinessClass.InsertBudgetInfo(ccnum, budget, invoiceid);
                        }



                        var ValidityFF = ValidityF.ToString("dd/MM/yyyy");



                        //------------------------------------------------------------------------------------

                        string invoicestat = " ";

                        string clientCompany = " ";
                        decimal openingbalance = 0;
                        decimal closingbalance = 0;
                        decimal toRevenue = 0;
                        decimal fromRevenue = 0;



                        if (Company == null || Company == "")
                        {
                            clientCompany = fname + " " + lname;

                        }
                        else
                        {
                            clientCompany = Company;
                            clientCompany = System.Net.WebUtility.HtmlDecode(clientCompany);
                        }


                        int trial = ((defered.Year - ValidityS.Year) * 12) + validity - Difference;
                        int trial2 = validity - Difference;


                        decimal opp = 0;
                        string op = " ";
                        DataSet opset = new DataSet();


                        {
                            if (Difference > 0 && ValidityS.Month != defered.Month || ValidityS.Month == defered.Month && ValidityF.Year != defered.Year && ValidityS.Year != defered.Year || ValidityF.Month == defered.Month)
                            {
                                if (defered.Month - 1 == 0)
                                {
                                    opset = _000.App_Code.BusinessClass.GetOpeningBalanceForMonth(ccnum, 12, (defered.Year - 1), invoiceid);
                                }

                                else
                                    opset = _000.App_Code.BusinessClass.GetOpeningBalanceForMonth(ccnum, (defered.Month - 1), defered.Year, invoiceid);
                                if (!IsEmpty(opset))
                                {
                                    DataRow dr = opset.Tables[0].Rows[0];


                                    decimal.TryParse(dr["openingbalance"].ToString(), out opp);


                                }
                                else
                                    opp = 0;
                            }

                            if (opp == 0)
                            {
                                opp = 0;
                            }
                            else
                            {
                                opp = opp;
                            }
                        }


                            if (ValidityS.Month == defered.Month && ValidityS.Year == defered.Year)
                            {
                                if (ValidityS.Day <= 15)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        Difference = Difference + 1;
                                        trial2 = validity - Difference;
                                        openingbalance = 0;
                                        toRevenue = invoiceamount * 1 / validity;
                                        closingbalance = invoiceamount - toRevenue;

                                        fromRevenue = invoiceamount;
                                        invoicestat = "Yes";
                                    }

                                    if (glid == 5156)
                                    {
                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            invoicetotcell += invoiceamount;
                                            torevcell += toRevenue;
                                            fromremcell += fromRevenue;
                                            closingtotcell += closingbalance;
                                            balancebfcell += openingbalance;
                                            budgettotcell += budget;
                                            fromRev += fromRevenue;
                                            balancebf += opp;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            toRev += toRevenue;
                                            budgettotal += budget;
                                            closeBal += closingbalance;
                                            cell.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                        }
                                    }

                                    else if (glid == 5157)
                                    {
                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            budgettotbb += budget;
                                            invoicetotbb += invoiceamount;
                                            torevbb += toRevenue;
                                            fromrembb += fromRevenue;
                                            closingtotbb += closingbalance;
                                            balancebfbb += openingbalance;
                                            fromRev += fromRevenue;
                                            balancebf += opp;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            toRev += toRevenue;
                                            budgettotal += budget;
                                            closeBal += closingbalance;
                                            bbrand.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                        }
                                    }

                                    else if (glid == 5158)
                                    {
                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            budgettotmic += budget;
                                            invoicetotmicro += invoiceamount;
                                            torevmicro += toRevenue;
                                            fromrevmicro += fromRevenue;
                                            closingtotmicro += closingbalance;
                                            balancebfmicro += openingbalance;
                                            fromRev += fromRevenue;
                                            balancebf += opp;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            toRev += toRevenue;
                                            budgettotal += budget;
                                            closeBal += closingbalance;
                                            micro.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);

                                        }
                                    }

                                    else if (glid == 5159)
                                    {
                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            budgettotds += budget;
                                            invoicetotds += invoiceamount;
                                            torevds += toRevenue;
                                            fromrevds += fromRevenue;
                                            closingtotds += closingbalance;
                                            balancebfds += openingbalance;
                                            fromRev += fromRevenue;
                                            balancebf += opp;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            toRev += toRevenue;
                                            budgettotal += budget;
                                            closeBal += closingbalance;
                                            dServices.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                        }
                                    }

                                    else if (glid == 5160)
                                    {
                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            budgettotvsat += budget;
                                            invoicetotvsat += invoiceamount;
                                            torevvsat += toRevenue;
                                            fromrevvsat += fromRevenue;
                                            closingtotvsat += closingbalance;
                                            balancebfvsat += openingbalance;
                                            fromRev += fromRevenue;
                                            balancebf += opp;
                                            budgettotal += budget;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            toRev += toRevenue;
                                            closeBal += closingbalance;
                                            vsat.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);

                                        }
                                    }

                                    else if (glid == 5161)
                                    {
                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            budgettotaero += budget;
                                            invoicetotaero += invoiceamount;
                                            torevaero += toRevenue;
                                            fromrevaero += fromRevenue;
                                            closingtotaero += closingbalance;
                                            balancebfaero += openingbalance;
                                            fromRev += fromRevenue;
                                            balancebf += opp;
                                            budgettotal += budget;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            toRev += toRevenue;
                                            closeBal += closingbalance;
                                            aero.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                        }
                                    }


                                    else if (glid == 5162)
                                    {
                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            budgettotmar += budget;
                                            invoicetotmar += invoiceamount;
                                            torevmar += toRevenue;
                                            fromrevmar += fromRevenue;
                                            closingtotmar += closingbalance;
                                            balancebfmar += openingbalance;
                                            fromRev += fromRevenue;
                                            balancebf += opp;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            budgettotal += budget;
                                            toRev += toRevenue;
                                            closeBal += closingbalance;
                                            marine.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                        }
                                    }

                                    else if (glid == 5163)
                                    {
                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            budgettottrunk += budget;
                                            invoicetottrunk += invoiceamount;
                                            torevtrunk += toRevenue;
                                            fromrevtrunk += fromRevenue;
                                            closingtottrunk += closingbalance;
                                            balancebftrunk += openingbalance;
                                            fromRev += fromRevenue;
                                            balancebf += opp;
                                            budgettotal += budget;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            toRev += toRevenue;
                                            closeBal += closingbalance;
                                            trunking.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                        }
                                    }

                                    else if (glid == 5164)
                                    {
                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            budgettotother += budget;
                                            invoicetotother += invoiceamount;
                                            torevother += toRevenue;
                                            fromrevother += fromRevenue;
                                            closingtotother += closingbalance;
                                            balancebfother += openingbalance;
                                            fromRev += fromRevenue;
                                            balancebf += opp;
                                            budgettotal += budget;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            toRev += toRevenue;
                                            closeBal += closingbalance;
                                            other.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                        }
                                    }
                                    else
                                    {
                                        table.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                if (ValidityS.Day > 15)
                                {

                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        openingbalance = 0;
                                        toRevenue = 0;
                                        fromRevenue = invoiceamount;
                                        closingbalance = invoiceamount;
                                        invoicestat = "Yes";


                                        //if (!IsEmpty(opset))
                                        //{
                                        //    opp = opp;
                                        //}
                                        //else
                                        //    opp = 0;

                                        opp = 0;

                                        if (Difference > 1 && IsEmpty(opset))
                                        {
                                            toRevenue = toRevenue * Difference;
                                        }
                                    }
                                    if (glid == 5156)
                                    {
                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            budgettotcell += budget;
                                            invoicetotcell += invoiceamount;
                                            torevcell += toRevenue;
                                            fromremcell += fromRevenue;
                                            closingtotcell += closingbalance;
                                            balancebfcell += opp;
                                            fromRev += fromRevenue;
                                            balancebf += opp;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            budgettotal += budget;
                                            toRev += toRevenue;
                                            closeBal += closingbalance;
                                            cell.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                        }
                                    }

                                    else if (glid == 5157)
                                    {
                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            budgettotbb += budget;
                                            invoicetotbb += invoiceamount;
                                            torevbb += toRevenue;
                                            fromrembb += fromRevenue;
                                            closingtotbb += closingbalance;
                                            balancebfbb += opp;
                                            fromRev += fromRevenue;
                                            balancebf += opp;
                                            budgettotal += budget;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            toRev += toRevenue;
                                            closeBal += closingbalance;
                                            bbrand.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                        }
                                    }

                                    else if (glid == 5158)
                                    {
                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            budgettotmic += budget;
                                            invoicetotmicro += invoiceamount;
                                            torevmicro += toRevenue;
                                            fromrevmicro += fromRevenue;
                                            closingtotmicro += closingbalance;
                                            balancebfmicro += opp;
                                            fromRev += fromRevenue;
                                            balancebf += opp;
                                            budgettotal += budget;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            toRev += toRevenue;
                                            closeBal += closingbalance;
                                            micro.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                        }
                                    }

                                    else if (glid == 5159)
                                    {
                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            budgettotds += budget;
                                            invoicetotds += invoiceamount;
                                            torevds += toRevenue;
                                            fromrevds += fromRevenue;
                                            closingtotds += closingbalance;
                                            balancebfds += opp;
                                            fromRev += fromRevenue;
                                            balancebf += opp;
                                            budgettotal += budget;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            toRev += toRevenue;
                                            closeBal += closingbalance;
                                            dServices.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                        }
                                    }


                                    else if (glid == 5160)
                                    {
                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            budgettotvsat += budget;
                                            invoicetotvsat += invoiceamount;
                                            torevvsat += toRevenue;
                                            fromrevvsat += fromRevenue;
                                            closingtotvsat += closingbalance;
                                            balancebfvsat += opp;
                                            fromRev += fromRevenue;
                                            balancebf += opp;
                                            budgettotal += budget;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            toRev += toRevenue;
                                            closeBal += closingbalance;
                                            vsat.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                        }
                                    }


                                    else if (glid == 5161)
                                    {

                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            budgettotaero += budget;
                                            invoicetotaero += invoiceamount;
                                            torevaero += toRevenue;
                                            fromrevaero += fromRevenue;
                                            closingtotaero += closingbalance;
                                            balancebfaero += opp;
                                            fromRev += fromRevenue;
                                            balancebf += opp;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            toRev += toRevenue;
                                            budgettotal += budget;
                                            closeBal += closingbalance;
                                            aero.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                        }
                                    }


                                    else if (glid == 5162)
                                    {
                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            budgettotmar += budget;
                                            invoicetotmar += invoiceamount;
                                            torevmar += toRevenue;
                                            fromrevmar += fromRevenue;
                                            closingtotmar += closingbalance;
                                            balancebfmar += opp;
                                            fromRev += fromRevenue;
                                            budgettotal += budget;
                                            balancebf += opp;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            toRev += toRevenue;
                                            closeBal += closingbalance;
                                            marine.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                        }
                                    }

                                    else if (glid == 5163)
                                    {
                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            budgettottrunk += budget;
                                            invoicetottrunk += invoiceamount;
                                            torevtrunk += toRevenue;
                                            fromrevtrunk += fromRevenue;
                                            closingtottrunk += closingbalance;
                                            balancebftrunk += opp;
                                            fromRev += fromRevenue;
                                            balancebf += opp;
                                            budgettotal += budget;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            toRev += toRevenue;
                                            closeBal += closingbalance;
                                            trunking.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                        }
                                    }


                                    else if (glid == 5164)
                                    {
                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            budgettotother += budget;
                                            invoicetotother += invoiceamount;
                                            torevother += toRevenue;
                                            fromrevother += fromRevenue;
                                            closingtotother += closingbalance;
                                            balancebfother += opp;
                                            fromRev += fromRevenue;
                                            balancebf += opp;
                                            budgettotal += budget;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            toRev += toRevenue;
                                            closeBal += closingbalance;
                                            other.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                        }
                                    }
                                    else
                                    {
                                        table.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(openingbalance, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }
                            }

                            //drop here 

                            else if (ValidityS.Month == (defered.Month - 1) && ValidityS.Day > 15 && ValidityS.Year == defered.Year)
                            {
                                if (Difference >= 0 && trial2 >= 0)
                                {
                                    openingbalance = 0;
                                    toRevenue = invoiceamount * 1 / validity;
                                    //changed
                                    fromRevenue = 0;
                                    closingbalance = invoiceamount - toRevenue;
                                    invoicestat = "No";


                                    //if (!IsEmpty(opset))
                                    //{
                                    //    opp = opp;
                                    //}
                                    //else
                                    //    opp = 0;

                                    opp = invoiceamount;

                                    if (Difference > 1 && IsEmpty(opset))
                                    {
                                        toRevenue = toRevenue * Difference;
                                    }

                                }
                                if (glid == 5156)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotcell += budget;
                                        invoicetotcell += invoiceamount;
                                        torevcell += toRevenue;
                                        fromremcell += fromRevenue;
                                        closingtotcell += closingbalance;
                                        balancebfcell += opp;
                                        fromRev += fromRevenue;
                                        balancebf += opp;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        budgettotal += budget;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        cell.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5157)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotbb += budget;
                                        invoicetotbb += invoiceamount;
                                        torevbb += toRevenue;
                                        fromrembb += fromRevenue;
                                        closingtotbb += closingbalance;
                                        balancebfbb += opp;
                                        fromRev += fromRevenue;
                                        balancebf += opp;
                                        budgettotal += budget;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        bbrand.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5158)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotmic += budget;
                                        invoicetotmicro += invoiceamount;
                                        torevmicro += toRevenue;
                                        fromrevmicro += fromRevenue;
                                        closingtotmicro += closingbalance;
                                        balancebfmicro += opp;
                                        fromRev += fromRevenue;
                                        balancebf += opp;
                                        budgettotal += budget;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        micro.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5159)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotds += budget;
                                        invoicetotds += invoiceamount;
                                        torevds += toRevenue;
                                        fromrevds += fromRevenue;
                                        closingtotds += closingbalance;
                                        balancebfds += opp;
                                        fromRev += fromRevenue;
                                        balancebf += opp;
                                        budgettotal += budget;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        dServices.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }


                                else if (glid == 5160)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotvsat += budget;
                                        invoicetotvsat += invoiceamount;
                                        torevvsat += toRevenue;
                                        fromrevvsat += fromRevenue;
                                        closingtotvsat += closingbalance;
                                        balancebfvsat += opp;
                                        fromRev += fromRevenue;
                                        balancebf += opp;
                                        budgettotal += budget;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        vsat.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }


                                else if (glid == 5161)
                                {

                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotaero += budget;
                                        invoicetotaero += invoiceamount;
                                        torevaero += toRevenue;
                                        fromrevaero += fromRevenue;
                                        closingtotaero += closingbalance;
                                        balancebfaero += opp;
                                        fromRev += fromRevenue;
                                        balancebf += opp;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        budgettotal += budget;
                                        closeBal += closingbalance;
                                        aero.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }


                                else if (glid == 5162)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotmar += budget;
                                        invoicetotmar += invoiceamount;
                                        torevmar += toRevenue;
                                        fromrevmar += fromRevenue;
                                        closingtotmar += closingbalance;
                                        balancebfmar += opp;
                                        fromRev += fromRevenue;
                                        budgettotal += budget;
                                        balancebf += opp;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        marine.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5163)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettottrunk += budget;
                                        invoicetottrunk += invoiceamount;
                                        torevtrunk += toRevenue;
                                        fromrevtrunk += fromRevenue;
                                        closingtottrunk += closingbalance;
                                        balancebftrunk += opp;
                                        fromRev += fromRevenue;
                                        balancebf += opp;
                                        budgettotal += budget;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        trunking.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }


                                else if (glid == 5164)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotother += budget;
                                        invoicetotother += invoiceamount;
                                        torevother += toRevenue;
                                        fromrevother += fromRevenue;
                                        closingtotother += closingbalance;
                                        balancebfother += opp;
                                        fromRev += fromRevenue;
                                        balancebf += opp;
                                        budgettotal += budget;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        other.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }
                                else
                                {
                                    table.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                }
                            }

                            else
                                if (ValidityS.Month == defered.Month && ValidityF.Year != defered.Year && ValidityS.Day <= 15 && ValidityS.Year != defered.Year)
                            {

                                if (Difference >= 0 && trial2 >= 0)
                                {

                                    Difference = Difference + 1;
                                    trial2 = validity - Difference;
                                    toRevenue = invoiceamount * 1 / validity;

                                    decimal amountalreadypaid = toRevenue * (Difference - 1);
                                    openingbalance = invoiceamount - amountalreadypaid;
                                    fromRevenue = 0;
                                    closingbalance = invoiceamount - (amountalreadypaid + toRevenue);

                                    invoicestat = "No";

                                    if (!IsEmpty(opset))
                                    {
                                        opp = opp;
                                    }
                                    else
                                        opp = 0;

                                    if (Difference > 1 && IsEmpty(opset))
                                    {
                                        toRevenue = toRevenue * Difference;
                                        fromRevenue = invoiceamount;
                                    }
                                }
                                if (glid == 5156)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotcell += budget;
                                        invoicetotcell += invoiceamount;
                                        torevcell += toRevenue;
                                        fromremcell += fromRevenue;
                                        closingtotcell += closingbalance;
                                        balancebfcell += opp;
                                        fromRev += fromRevenue;
                                        balancebf += opp;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        budgettotal += budget;
                                        closeBal += closingbalance;
                                        cell.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5157)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotbb += budget;
                                        invoicetotbb += invoiceamount;
                                        torevbb += toRevenue;
                                        fromrembb += fromRevenue;
                                        closingtotbb += closingbalance;
                                        balancebfbb += opp;
                                        fromRev += fromRevenue;
                                        balancebf += opp;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        budgettotal += budget;
                                        closeBal += closingbalance;
                                        bbrand.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }
                                else if (glid == 5158)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotmic += budget;
                                        invoicetotmicro += invoiceamount;
                                        torevmicro += toRevenue;
                                        fromrevmicro += fromRevenue;
                                        closingtotmicro += closingbalance;
                                        balancebfmicro += opp;
                                        fromRev += fromRevenue;
                                        balancebf += opp;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        budgettotal += budget;
                                        closeBal += closingbalance;
                                        micro.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5159)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotds += budget;
                                        invoicetotds += invoiceamount;
                                        torevds += toRevenue;
                                        fromrevds += fromRevenue;
                                        closingtotds += closingbalance;
                                        balancebfds += opp;
                                        fromRev += fromRevenue;
                                        balancebf += opp;
                                        budgettotal += budget;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        dServices.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5160)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotvsat += budget;
                                        invoicetotvsat += invoiceamount;
                                        torevvsat += toRevenue;
                                        fromrevvsat += fromRevenue;
                                        closingtotvsat += closingbalance;
                                        balancebfvsat += opp;
                                        fromRev += fromRevenue;
                                        balancebf += opp;
                                        budgettotal += budget;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        vsat.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5161)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotaero += budget;
                                        invoicetotaero += invoiceamount;
                                        torevaero += toRevenue;
                                        fromrevaero += fromRevenue;
                                        closingtotaero += closingbalance;
                                        balancebfaero += opp;
                                        fromRev += fromRevenue;
                                        balancebf += opp;
                                        budgettotal += budget;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        aero.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5162)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotmar += budget;
                                        invoicetotmar += invoiceamount;
                                        torevmar += toRevenue;
                                        fromrevmar += fromRevenue;
                                        closingtotmar += closingbalance;
                                        balancebfmar += opp;
                                        fromRev += fromRevenue;
                                        balancebf += opp;
                                        budgettotal += budget;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        marine.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5163)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettottrunk += budget;
                                        invoicetottrunk += invoiceamount;
                                        torevtrunk += toRevenue;
                                        fromrevtrunk += fromRevenue;
                                        closingtottrunk += closingbalance;
                                        balancebftrunk += opp;
                                        fromRev += fromRevenue;
                                        balancebf += opp;
                                        budgettotal += budget;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        trunking.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5164)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotother += budget;
                                        invoicetotother += invoiceamount;
                                        torevother += toRevenue;
                                        fromrevother += fromRevenue;
                                        closingtotother += closingbalance;
                                        balancebfother += opp;
                                        fromRev += fromRevenue;
                                        budgettotal += budget;
                                        balancebf += opp;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        other.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }
                                else
                                {
                                    table.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                }
                            }

                            //Right here

                            else if (ValidityS.Month == defered.Month && ValidityF.Year != defered.Year && ValidityS.Day > 15 && ValidityS.Year != defered.Year)
                            {
                                if (Difference >= 0 && trial2 >= 0)
                                {
                                    toRevenue = invoiceamount * 1 / validity;

                                    decimal amountalreadypaid = toRevenue * (Difference - 1);
                                    openingbalance = invoiceamount - amountalreadypaid;
                                    fromRevenue = 0;
                                    closingbalance = invoiceamount - (amountalreadypaid + toRevenue);

                                    invoicestat = "No";


                                    if (!IsEmpty(opset))
                                    {
                                        opp = opp;

                                    }
                                    else
                                        opp = 0;



                                    if (Difference > 1 && IsEmpty(opset))
                                    {
                                        toRevenue = toRevenue * Difference;
                                        fromRevenue = invoiceamount;
                                    }
                                }
                                if (glid == 5156)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotcell += budget;
                                        invoicetotcell += invoiceamount;
                                        torevcell += toRevenue;
                                        fromremcell += fromRevenue;
                                        closingtotcell += closingbalance;
                                        balancebfcell += opp;
                                        fromRev += fromRevenue;
                                        balancebf += opp;
                                        budgettotal += budget;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        cell.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5157)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotbb += budget;
                                        invoicetotbb += invoiceamount;
                                        torevbb += toRevenue;
                                        fromrembb += fromRevenue;
                                        closingtotbb += closingbalance;
                                        balancebfbb += opp;
                                        fromRev += fromRevenue;
                                        balancebf += opp;
                                        budgettotal += budget;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        bbrand.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }
                                else if (glid == 5158)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotmic += budget;
                                        invoicetotmicro += invoiceamount;
                                        torevmicro += toRevenue;
                                        fromrevmicro += fromRevenue;
                                        closingtotmicro += closingbalance;
                                        balancebfmicro += opp;
                                        fromRev += fromRevenue;
                                        balancebf += opp;
                                        budgettotal += budget;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        micro.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5159)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotds += budget;
                                        invoicetotds += invoiceamount;
                                        torevds += toRevenue;
                                        fromrevds += fromRevenue;
                                        closingtotds += closingbalance;
                                        balancebfds += opp;
                                        fromRev += fromRevenue;
                                        budgettotal += budget;
                                        balancebf += opp;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        dServices.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5160)
                                {

                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotvsat += budget;
                                        invoicetotvsat += invoiceamount;
                                        torevvsat += toRevenue;
                                        fromrevvsat += fromRevenue;
                                        closingtotvsat += closingbalance;
                                        balancebfvsat += opp;
                                        fromRev += fromRevenue;
                                        budgettotal += budget;
                                        balancebf += opp;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        vsat.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5161)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotaero += budget;
                                        invoicetotaero += invoiceamount;
                                        torevaero += toRevenue;
                                        fromrevaero += fromRevenue;
                                        closingtotaero += closingbalance;
                                        balancebfaero += opp;
                                        fromRev += fromRevenue;
                                        budgettotal += budget;
                                        balancebf += opp;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;

                                        aero.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5162)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotmar += budget;
                                        invoicetotmar += invoiceamount;
                                        torevmar += toRevenue;
                                        fromrevmar += fromRevenue;
                                        closingtotmar += closingbalance;
                                        balancebfmar += opp;
                                        fromRev += fromRevenue;
                                        budgettotal += budget;
                                        balancebf += opp;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        marine.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5163)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettottrunk += budget;
                                        invoicetottrunk += invoiceamount;
                                        torevtrunk += toRevenue;
                                        fromrevtrunk += fromRevenue;
                                        closingtottrunk += closingbalance;
                                        balancebftrunk += opp;
                                        fromRev += fromRevenue;
                                        budgettotal += budget;
                                        balancebf += opp;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        trunking.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }

                                else if (glid == 5164)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        budgettotother += budget;
                                        invoicetotother += invoiceamount;
                                        torevother += toRevenue;
                                        fromrevother += fromRevenue;
                                        closingtotother += closingbalance;
                                        balancebfother += opp;
                                        fromRev += fromRevenue;
                                        budgettotal += budget;
                                        balancebf += opp;
                                        invoiceTotalForYes += invoiceamount;
                                        invoiceTotal += invoiceamount;
                                        toRev += toRevenue;
                                        closeBal += closingbalance;
                                        other.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }
                                else
                                {
                                    table.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), Math.Round(budget, 2), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                }

                            }
                            else
                            {

                                if (ValidityS.Month != defered.Month && ValidityS.Day <= 15)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {

                                        Difference = Difference + 1;
                                        trial2 = validity - Difference;
                                        toRevenue = invoiceamount * 1 / validity;

                                        decimal amountalreadypaid = toRevenue * (Difference - 1);
                                        openingbalance = invoiceamount - amountalreadypaid;
                                        fromRevenue = 0;
                                        closingbalance = invoiceamount - (amountalreadypaid + toRevenue);

                                        invoicestat = "No";

                                        if (!IsEmpty(opset))
                                        {
                                            opp = opp;
                                        }
                                        else
                                            opp = 0;

                                        if (Difference > 1 && IsEmpty(opset))
                                        {
                                            toRevenue = toRevenue * Difference;
                                            fromRevenue = invoiceamount;
                                        }
                                    }
                                    if (glid == 5156)
                                    {
                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            budgettotcell += budget;
                                            invoicetotcell += invoiceamount;
                                            torevcell += toRevenue;
                                            fromremcell += fromRevenue;
                                            closingtotcell += closingbalance;
                                            balancebfcell += opp;
                                            fromRev += fromRevenue;
                                            balancebf += opp;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            toRev += toRevenue;
                                            budgettotal += budget;
                                            closeBal += closingbalance;
                                            cell.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                        }
                                    }

                                    else if (glid == 5157)
                                    {
                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            budgettotbb += budget;
                                            invoicetotbb += invoiceamount;
                                            torevbb += toRevenue;
                                            fromrembb += fromRevenue;
                                            closingtotbb += closingbalance;
                                            balancebfbb += opp;
                                            fromRev += fromRevenue;
                                            balancebf += opp;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            toRev += toRevenue;
                                            budgettotal += budget;
                                            closeBal += closingbalance;
                                            bbrand.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                        }
                                    }
                                    else if (glid == 5158)
                                    {
                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            budgettotmic += budget;
                                            invoicetotmicro += invoiceamount;
                                            torevmicro += toRevenue;
                                            fromrevmicro += fromRevenue;
                                            closingtotmicro += closingbalance;
                                            balancebfmicro += opp;
                                            fromRev += fromRevenue;
                                            balancebf += opp;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            toRev += toRevenue;
                                            budgettotal += budget;
                                            closeBal += closingbalance;
                                            micro.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                        }
                                    }

                                    else if (glid == 5159)
                                    {
                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            budgettotds += budget;
                                            invoicetotds += invoiceamount;
                                            torevds += toRevenue;
                                            fromrevds += fromRevenue;
                                            closingtotds += closingbalance;
                                            balancebfds += opp;
                                            fromRev += fromRevenue;
                                            balancebf += opp;
                                            budgettotal += budget;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            toRev += toRevenue;
                                            closeBal += closingbalance;
                                            dServices.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                        }
                                    }

                                    else if (glid == 5160)
                                    {
                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            budgettotvsat += budget;
                                            invoicetotvsat += invoiceamount;
                                            torevvsat += toRevenue;
                                            fromrevvsat += fromRevenue;
                                            closingtotvsat += closingbalance;
                                            balancebfvsat += opp;
                                            fromRev += fromRevenue;
                                            balancebf += opp;
                                            budgettotal += budget;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            toRev += toRevenue;
                                            closeBal += closingbalance;
                                            vsat.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                        }
                                    }

                                    else if (glid == 5161)
                                    {
                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            budgettotaero += budget;
                                            invoicetotaero += invoiceamount;
                                            torevaero += toRevenue;
                                            fromrevaero += fromRevenue;
                                            closingtotaero += closingbalance;
                                            balancebfaero += opp;
                                            fromRev += fromRevenue;
                                            balancebf += opp;
                                            budgettotal += budget;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            toRev += toRevenue;
                                            closeBal += closingbalance;
                                            aero.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                        }
                                    }

                                    else if (glid == 5162)
                                    {
                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            budgettotmar += budget;
                                            invoicetotmar += invoiceamount;
                                            torevmar += toRevenue;
                                            fromrevmar += fromRevenue;
                                            closingtotmar += closingbalance;
                                            balancebfmar += opp;
                                            fromRev += fromRevenue;
                                            balancebf += opp;
                                            budgettotal += budget;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            toRev += toRevenue;
                                            closeBal += closingbalance;
                                            marine.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                        }
                                    }

                                    else if (glid == 5163)
                                    {
                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            budgettottrunk += budget;
                                            invoicetottrunk += invoiceamount;
                                            torevtrunk += toRevenue;
                                            fromrevtrunk += fromRevenue;
                                            closingtottrunk += closingbalance;
                                            balancebftrunk += opp;
                                            fromRev += fromRevenue;
                                            balancebf += opp;
                                            budgettotal += budget;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            toRev += toRevenue;
                                            closeBal += closingbalance;
                                            trunking.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                        }
                                    }

                                    else if (glid == 5164)
                                    {
                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            budgettotother += budget;
                                            invoicetotother += invoiceamount;
                                            torevother += toRevenue;
                                            fromrevother += fromRevenue;
                                            closingtotother += closingbalance;
                                            balancebfother += opp;
                                            fromRev += fromRevenue;
                                            budgettotal += budget;
                                            balancebf += opp;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            toRev += toRevenue;
                                            closeBal += closingbalance;
                                            other.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                        }
                                    }
                                    else
                                    {
                                        table.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }

                                }

                                else if (ValidityS.Day > 15 && trial2 == 0)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        toRevenue = invoiceamount * 1 / validity;

                                        decimal amountalreadypaid = toRevenue * (Difference - 1);
                                        openingbalance = invoiceamount - amountalreadypaid;
                                        fromRevenue = 0;
                                        closingbalance = invoiceamount - (amountalreadypaid + toRevenue);

                                        invoicestat = "No";


                                        if (!IsEmpty(opset))
                                        {
                                            opp = opp;

                                        }
                                        else
                                            opp = 0;



                                        if (Difference > 1 && IsEmpty(opset))
                                        {
                                            toRevenue = toRevenue * Difference;
                                            fromRevenue = invoiceamount;
                                        }
                                    }
                                    if (glid == 5156)
                                    {
                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            budgettotcell += budget;
                                            invoicetotcell += invoiceamount;
                                            torevcell += toRevenue;
                                            fromremcell += fromRevenue;
                                            closingtotcell += closingbalance;
                                            balancebfcell += opp;
                                            fromRev += fromRevenue;
                                            balancebf += opp;
                                            budgettotal += budget;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            toRev += toRevenue;
                                            closeBal += closingbalance;
                                            cell.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                        }
                                    }

                                    else if (glid == 5157)
                                    {
                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            budgettotbb += budget;
                                            invoicetotbb += invoiceamount;
                                            torevbb += toRevenue;
                                            fromrembb += fromRevenue;
                                            closingtotbb += closingbalance;
                                            balancebfbb += opp;
                                            fromRev += fromRevenue;
                                            balancebf += opp;
                                            budgettotal += budget;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            toRev += toRevenue;
                                            closeBal += closingbalance;
                                            bbrand.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                        }
                                    }
                                    else if (glid == 5158)
                                    {
                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            budgettotmic += budget;
                                            invoicetotmicro += invoiceamount;
                                            torevmicro += toRevenue;
                                            fromrevmicro += fromRevenue;
                                            closingtotmicro += closingbalance;
                                            balancebfmicro += opp;
                                            fromRev += fromRevenue;
                                            balancebf += opp;
                                            budgettotal += budget;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            toRev += toRevenue;
                                            closeBal += closingbalance;
                                            micro.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                        }
                                    }

                                    else if (glid == 5159)
                                    {
                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            budgettotds += budget;
                                            invoicetotds += invoiceamount;
                                            torevds += toRevenue;
                                            fromrevds += fromRevenue;
                                            closingtotds += closingbalance;
                                            balancebfds += opp;
                                            fromRev += fromRevenue;
                                            budgettotal += budget;
                                            balancebf += opp;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            toRev += toRevenue;
                                            closeBal += closingbalance;
                                            dServices.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                        }
                                    }

                                    else if (glid == 5160)
                                    {

                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            budgettotvsat += budget;
                                            invoicetotvsat += invoiceamount;
                                            torevvsat += toRevenue;
                                            fromrevvsat += fromRevenue;
                                            closingtotvsat += closingbalance;
                                            balancebfvsat += opp;
                                            fromRev += fromRevenue;
                                            budgettotal += budget;
                                            balancebf += opp;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            toRev += toRevenue;
                                            closeBal += closingbalance;
                                            vsat.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                        }
                                    }

                                    else if (glid == 5161)
                                    {
                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            budgettotaero += budget;
                                            invoicetotaero += invoiceamount;
                                            torevaero += toRevenue;
                                            fromrevaero += fromRevenue;
                                            closingtotaero += closingbalance;
                                            balancebfaero += opp;
                                            fromRev += fromRevenue;
                                            budgettotal += budget;
                                            balancebf += opp;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            toRev += toRevenue;
                                            closeBal += closingbalance;

                                            aero.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                        }
                                    }

                                    else if (glid == 5162)
                                    {
                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            budgettotmar += budget;
                                            invoicetotmar += invoiceamount;
                                            torevmar += toRevenue;
                                            fromrevmar += fromRevenue;
                                            closingtotmar += closingbalance;
                                            balancebfmar += opp;
                                            fromRev += fromRevenue;
                                            budgettotal += budget;
                                            balancebf += opp;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            toRev += toRevenue;
                                            closeBal += closingbalance;
                                            marine.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                        }
                                    }

                                    else if (glid == 5163)
                                    {
                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            budgettottrunk += budget;
                                            invoicetottrunk += invoiceamount;
                                            torevtrunk += toRevenue;
                                            fromrevtrunk += fromRevenue;
                                            closingtottrunk += closingbalance;
                                            balancebftrunk += opp;
                                            fromRev += fromRevenue;
                                            budgettotal += budget;
                                            balancebf += opp;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            toRev += toRevenue;
                                            closeBal += closingbalance;
                                            trunking.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                        }
                                    }

                                    else if (glid == 5164)
                                    {
                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            budgettotother += budget;
                                            invoicetotother += invoiceamount;
                                            torevother += toRevenue;
                                            fromrevother += fromRevenue;
                                            closingtotother += closingbalance;
                                            balancebfother += opp;
                                            fromRev += fromRevenue;
                                            budgettotal += budget;
                                            balancebf += opp;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            toRev += toRevenue;
                                            closeBal += closingbalance;
                                            other.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                        }
                                    }
                                    else
                                    {
                                        table.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }
                                else if (ValidityS.Month != defered.Month && ValidityS.Day > 15)
                                {
                                    if (Difference >= 0 && trial2 >= 0)
                                    {
                                        toRevenue = invoiceamount * 1 / validity;
                                        decimal amountalreadypaid = toRevenue * (Difference - 1);
                                        openingbalance = invoiceamount - amountalreadypaid;
                                        fromRevenue = 0;
                                        closingbalance = invoiceamount - (amountalreadypaid + toRevenue);
                                        invoicestat = "No";

                                        if (!IsEmpty(opset))
                                        {
                                            opp = opp;
                                        }
                                        else
                                            opp = 0;
                                        //  balancebf += opp;

                                        if (Difference > 1 && IsEmpty(opset))
                                        {
                                            toRevenue = toRevenue * Difference;
                                            fromRevenue = invoiceamount;
                                        }
                                    }
                                    if (glid == 5156)
                                    {
                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            budgettotcell += budget;
                                            invoicetotcell += invoiceamount;
                                            torevcell += toRevenue;
                                            fromremcell += fromRevenue;
                                            closingtotcell += closingbalance;
                                            balancebfcell += opp;
                                            fromRev += fromRevenue;
                                            budgettotal += budget;
                                            balancebf += opp;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            toRev += toRevenue;
                                            closeBal += closingbalance;
                                            cell.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                        }
                                    }

                                    else if (glid == 5157)
                                    {
                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            budgettotbb += budget;
                                            invoicetotbb += invoiceamount;
                                            torevbb += toRevenue;
                                            fromrembb += fromRevenue;
                                            closingtotbb += closingbalance;
                                            balancebfbb += opp;
                                            fromRev += fromRevenue;
                                            budgettotal += budget;
                                            balancebf += opp;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            toRev += toRevenue;
                                            closeBal += closingbalance;
                                            bbrand.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                        }
                                    }
                                    else if (glid == 5158)
                                    {
                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            budgettotmic += budget;
                                            invoicetotmicro += invoiceamount;
                                            torevmicro += toRevenue;
                                            fromrevmicro += fromRevenue;
                                            closingtotmicro += closingbalance;
                                            balancebfmicro += opp;
                                            fromRev += fromRevenue;
                                            budgettotal += budget;
                                            balancebf += opp;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            toRev += toRevenue;
                                            closeBal += closingbalance;
                                            micro.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                        }
                                    }

                                    else if (glid == 5159)
                                    {
                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            budgettotds += budget;
                                            invoicetotds += invoiceamount;
                                            torevds += toRevenue;
                                            fromrevds += fromRevenue;
                                            closingtotds += closingbalance;
                                            balancebfds += opp;
                                            fromRev += fromRevenue;
                                            budgettotal += budget;
                                            balancebf += opp;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            toRev += toRevenue;
                                            closeBal += closingbalance;
                                            dServices.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                        }
                                    }

                                    else if (glid == 5160)
                                    {
                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            budgettotvsat += budget;
                                            invoicetotvsat += invoiceamount;
                                            torevvsat += toRevenue;
                                            fromrevvsat += fromRevenue;
                                            closingtotvsat += closingbalance;
                                            balancebfvsat += opp;
                                            fromRev += fromRevenue;
                                            budgettotal += budget;
                                            balancebf += opp;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            toRev += toRevenue;
                                            closeBal += closingbalance;
                                            vsat.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                        }
                                    }

                                    else if (glid == 5161)
                                    {
                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            budgettotaero += budget;
                                            invoicetotaero += invoiceamount;
                                            torevaero += toRevenue;
                                            fromrevaero += fromRevenue;
                                            closingtotaero += closingbalance;
                                            balancebfaero += opp;
                                            fromRev += fromRevenue;
                                            budgettotal += budget;
                                            balancebf += opp;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            toRev += toRevenue;
                                            closeBal += closingbalance;
                                            aero.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                        }
                                    }

                                    else if (glid == 5162)
                                    {
                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            budgettotmar += budget;
                                            invoicetotmar += invoiceamount;
                                            torevmar += toRevenue;
                                            fromrevmar += fromRevenue;
                                            closingtotmar += closingbalance;
                                            balancebfmar += opp;
                                            budgettotal += budget;
                                            fromRev += fromRevenue;
                                            balancebf += opp;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            toRev += toRevenue;
                                            closeBal += closingbalance;
                                            marine.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                        }
                                    }

                                    else if (glid == 5163)
                                    {
                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            budgettottrunk += budget;
                                            invoicetottrunk += invoiceamount;
                                            torevtrunk += toRevenue;
                                            fromrevtrunk += fromRevenue;
                                            closingtottrunk += closingbalance;
                                            balancebftrunk += opp;
                                            fromRev += fromRevenue;
                                            budgettotal += budget;
                                            balancebf += opp;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            toRev += toRevenue;
                                            closeBal += closingbalance;
                                            trunking.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                        }
                                    }

                                    else if (glid == 5164)
                                    {
                                        if (Difference >= 0 && trial2 >= 0)
                                        {
                                            budgettotother += budget;
                                            invoicetotother += invoiceamount;
                                            torevother += toRevenue;
                                            fromrevother += fromRevenue;
                                            closingtotother += closingbalance;
                                            balancebfother += opp;
                                            budgettotal += budget;
                                            fromRev += fromRevenue;
                                            balancebf += opp;
                                            invoiceTotalForYes += invoiceamount;
                                            invoiceTotal += invoiceamount;
                                            toRev += toRevenue;
                                            closeBal += closingbalance;
                                            other.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), formatMoney(Math.Round(invoiceamount, 2)), invoicestat, formatMoney(Math.Round(opp, 2)), formatMoney(Math.Round(fromRevenue, 2)), formatMoney(Math.Round(-toRevenue, 2)), formatMoney(Math.Round(closingbalance, 2)), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                        }
                                    }
                                    else
                                    {
                                        table.Rows.Add(ccnum, clientCompany, invoiceid.ToString(), formatMoney(Math.Round(budget, 2)), Math.Round(invoiceamount, 2), invoicestat, Math.Round(opp, 2), Math.Round(fromRevenue, 2), Math.Round(-toRevenue, 2), Math.Round(closingbalance, 2), validity, Difference, trial2/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, ValiditySS, ValidityFF);
                                    }
                                }
                            }

                            if (Difference > 0 || description == "Modification" && Difference == 0 || Difference == 0 && ValidityS.Month == defered.Month && ValidityS.Year == defered.Year && ValidityS.Day > 15)
                            {

                                DataSet opstatus = new DataSet();
                                opstatus = _000.App_Code.BusinessClass.GetOpeningBalStat(ccnum, invoiceid, defered);
                               
                                    if (IsEmpty(opstatus))
                                    {
                                        _000.App_Code.BusinessClass.InsertOpBalNew(ccnum, closingbalance, defered, invoiceid, ValidityS, ValidityF);
                                    }
                                    else
                                    {
                                        _000.App_Code.BusinessClass.UpdateOpBalNew(ccnum, closingbalance, defered, invoiceid, ValidityS, ValidityF);
                                    }
                            }
                        }
                   

                
               
                cell.Rows.Add("Sub Total", " ", " ", formatMoney(Math.Round(budgettotcell, 2)), formatMoney(Math.Round(invoicetotcell, 2)), " ", formatMoney(Math.Round(balancebfcell, 2)), formatMoney(Math.Round(fromremcell, 2)), formatMoney(Math.Round(-torevcell, 2)), formatMoney(Math.Round(closingtotcell, 2)), 0, 0, 0/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, DateTime.Now.ToString("MM/dd/yyyy"), DateTime.Now.ToString("MM/dd/yyyy"));
                bbrand.Rows.Add("Sub Total", " ", " ", formatMoney(Math.Round(budgettotbb, 2)), formatMoney(Math.Round(invoicetotbb, 2)), " ", formatMoney(Math.Round(balancebfbb, 2)), formatMoney(Math.Round(fromrembb, 2)), formatMoney(Math.Round(-torevbb, 2)), formatMoney(Math.Round(closingtotbb, 2)), 0, 0, 0/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, DateTime.Now.ToString("MM/dd/yyyy"), DateTime.Now.ToString("MM/dd/yyyy"));
                micro.Rows.Add("Sub Total", " ", " ", formatMoney(Math.Round(budgettotmic, 2)), formatMoney(Math.Round(invoicetotmicro, 2)), " ", formatMoney(Math.Round(balancebfmicro, 2)), formatMoney(Math.Round(fromrevmicro, 2)), formatMoney(Math.Round(-torevmicro, 2)), formatMoney(Math.Round(closingtotmicro, 2)), 0, 0, 0/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, DateTime.Now.ToString("MM/dd/yyyy"), DateTime.Now.ToString("MM/dd/yyyy"));
                vsat.Rows.Add("Sub Total", " ", " ", formatMoney(Math.Round(budgettotvsat, 2)), formatMoney(Math.Round(invoicetotvsat, 2)), " ", formatMoney(Math.Round(balancebfvsat, 2)), formatMoney(Math.Round(fromrevvsat, 2)), formatMoney(Math.Round(-torevvsat, 2)), formatMoney(Math.Round(closingtotvsat, 2)), 0, 0, 0/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, DateTime.Now.ToString("MM/dd/yyyy"), DateTime.Now.ToString("MM/dd/yyyy"));
                aero.Rows.Add("Sub Total", " ", " ", formatMoney(Math.Round(budgettotaero, 2)), formatMoney(Math.Round(invoicetotaero, 2)), " ", formatMoney(Math.Round(balancebfaero, 2)), formatMoney(Math.Round(fromrevaero, 2)), formatMoney(Math.Round(-torevaero, 2)), formatMoney(Math.Round(closingtotaero, 2)), 0, 0, 0/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, DateTime.Now.ToString("MM/dd/yyyy"), DateTime.Now.ToString("MM/dd/yyyy"));
                marine.Rows.Add("Sub Total", " ", " ", formatMoney(Math.Round(budgettotmar, 2)), formatMoney(Math.Round(invoicetotmar, 2)), " ", formatMoney(Math.Round(balancebfmar, 2)), formatMoney(Math.Round(fromrevmar, 2)), formatMoney(Math.Round(-torevmar, 2)), formatMoney(Math.Round(closingtotmar, 2)), 0, 0, 0/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, DateTime.Now.ToString("MM/dd/yyyy"), DateTime.Now.ToString("MM/dd/yyyy"));
                dServices.Rows.Add("Sub Total", " ", " ", formatMoney(Math.Round(budgettotds, 2)), formatMoney(Math.Round(invoicetotds, 2)), " ", formatMoney(Math.Round(balancebfds, 2)), formatMoney(Math.Round(fromrevds, 2)), formatMoney(Math.Round(-torevds, 2)), formatMoney(Math.Round(closingtotds, 2)), 0, 0, 0/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, DateTime.Now.ToString("MM/dd/yyyy"), DateTime.Now.ToString("MM/dd/yyyy"));
                trunking.Rows.Add("Sub Total", " ", " ", formatMoney(Math.Round(budgettottrunk, 2)), formatMoney(Math.Round(invoicetottrunk, 2)), " ", formatMoney(Math.Round(balancebftrunk, 2)), formatMoney(Math.Round(fromrevtrunk, 2)), formatMoney(Math.Round(-torevtrunk, 2)), formatMoney(Math.Round(closingtottrunk, 2)), 0, 0, 0/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, DateTime.Now.ToString("MM/dd/yyyy"), DateTime.Now.ToString("MM/dd/yyyy"));
                other.Rows.Add("Sub Total", " ", " ", formatMoney(Math.Round(budgettotother, 2)), formatMoney(Math.Round(invoicetotother, 2)), " ", formatMoney(Math.Round(balancebfother, 2)), formatMoney(Math.Round(fromrevother, 2)), formatMoney(Math.Round(-torevother, 2)), formatMoney(Math.Round(closingtotother, 2)), 0, 0, 0/* ((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, DateTime.Now.ToString("MM/dd/yyyy"), DateTime.Now.ToString("MM/dd/yyyy"));

                table.Rows.Add("Total", " ", " ", formatMoney(Math.Round(budgettotal, 2)), formatMoney(Math.Round(invoiceTotal, 2)), " ", 0, formatMoney(Math.Round(fromRev, 2)), formatMoney(Math.Round(-toRev, 2)), 0, 0, 0, 0/*((DateTime.Now.Year - ValidityS.Year) * 12) + validity - Difference*/, DateTime.Now, DateTime.Now);
                

                DataSet ds = new DataSet();

                dsi.Tables.Add(cell);
                dsi.Tables.Add(vsat);
                dsi.Tables.Add(marine);
                dsi.Tables.Add(dServices);
                dsi.Tables.Add(table);
                dsi.Tables.Add(aero);
                dsi.Tables.Add(trunking);
                dsi.Tables.Add(other);
                dsi.Tables.Add(bbrand);
                dsi.Tables.Add(micro);

                connection1.Close();
                connection2.Close();
            }
            catch (Exception ex)
            {
                lblError.Text = "Could not Establish Connection to Database";
            }
            return dsi;
        }



        public void CreateDeferredTotals()
        {
            int year = 0;

            if (DropDownList1.SelectedValue == "2016")
            {
                year = 2016;
            }
            else if (DropDownList1.SelectedValue == "2017")
            {
                year = 2017;
            }

            else if (DropDownList1.SelectedValue == "2018")
            {
                year = 2018;
            }

            else if (DropDownList1.SelectedValue == "2019")
            {
                year = 2019;
            }

            else if (DropDownList1.SelectedValue == "2020")
            {
                year = 2020;
            }

            string month = "";

            if (ddl1.SelectedValue == "1")
            {
                month = "January";
            }

            else if (ddl1.SelectedValue == "2")
            {
                month = "February";
            }

            else if (ddl1.SelectedValue == "3")
            {
                month = "March";
            }

            else if (ddl1.SelectedValue == "4")
            {
                month = "April";
            }

            else if (ddl1.SelectedValue == "5")
            {
                month = "May";
            }

            else if (ddl1.SelectedValue == "6")
            {
                month = "June";
            }

            else if (ddl1.SelectedValue == "7")
            {
                month = "July";
            }

            else if (ddl1.SelectedValue == "8")
            {
                month = "August";
            }

            else if (ddl1.SelectedValue == "9")
            {
                month = "September";
            }

            else if (ddl1.SelectedValue == "10")
            {
                month = "October";
            }

            else if (ddl1.SelectedValue == "11")
            {
                month = "November";
            }

            else if (ddl1.SelectedValue == "12")
            {
                month = "December";
            }




            var Testpath = @"C:\\Users\\asms-accpac-1\\Documents\\DefferedincomeReport_SectionSubTotalsFor" + month + " " + year + ".pdf";
            Document doc = new Document(iTextSharp.text.PageSize.A4_LANDSCAPE, 0, 0, 20, 0);
            try
            {
                PdfWriter defffered = PdfWriter.GetInstance(doc, new FileStream(Testpath, FileMode.OpenOrCreate));

            }

            catch (Exception ex)
            {
                lblError.Text = "";
            }
            doc.Open();
            Paragraph space = new Paragraph("\n");


            var imagePath = Server.MapPath("~/spec.jpg");
            iTextSharp.text.Image PNG = iTextSharp.text.Image.GetInstance(imagePath);
            PNG.ScaleToFit(100f, 100f);
            PNG.Alignment = 1;
            Paragraph paragraph4 = new Paragraph(new Phrase("Spectrum Management Authority Deferred Income Sections Report For the Month of " + month + " " + year, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            paragraph4.Alignment = 1;
            doc.Add(paragraph4);
            doc.Add(PNG);


            Paragraph paragraphcell = new Paragraph(new Phrase("Summary of Revenue Totals", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));




            PdfPTable tcell = new PdfPTable(7);
            tcell.AddCell(new Phrase("Category", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase("Budget Total", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase("Invoice Total ", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase("Balance B/FWD ", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase("From Revenue", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase("To Revenue", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase("Balance C/FWD", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));


            tcell.AddCell(new Phrase("Cellular", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase(formatMoney(Math.Round(budgettotcell, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase(formatMoney(Math.Round(invoicetotcell, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            tcell.AddCell(new Phrase(formatMoney(Math.Round(balancebfcell, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase(formatMoney(Math.Round(fromremcell, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase(formatMoney(Math.Round(torevcell, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase(formatMoney(Math.Round(closingtotcell, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));



            tcell.AddCell(new Phrase("P/R Commercial(Broadband)", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));



            tcell.AddCell(new Phrase(formatMoney(Math.Round(budgettotbb, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase(formatMoney(Math.Round(invoicetotbb, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            tcell.AddCell(new Phrase(formatMoney(Math.Round(balancebfbb, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase(formatMoney(Math.Round(fromrembb, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase(formatMoney(Math.Round(-torevbb, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase(formatMoney(Math.Round(closingtotbb, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));






            tcell.AddCell(new Phrase("P/R Commercial (Microwave)", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));



            tcell.AddCell(new Phrase(formatMoney(Math.Round(budgettotmic, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase(formatMoney(Math.Round(invoicetotmicro, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            tcell.AddCell(new Phrase(formatMoney(Math.Round(balancebfmicro, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase(formatMoney(Math.Round(fromrevmicro, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase(formatMoney(Math.Round(-torevmicro, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase(formatMoney(Math.Round(closingtotmicro, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));



            tcell.AddCell(new Phrase("Vsat", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));



            tcell.AddCell(new Phrase(formatMoney(Math.Round(budgettotvsat, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase(formatMoney(Math.Round(invoicetotvsat, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            tcell.AddCell(new Phrase(formatMoney(Math.Round(balancebfvsat, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase(formatMoney(Math.Round(fromrevvsat, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase(formatMoney(Math.Round(-torevvsat, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase(formatMoney(Math.Round(closingtotvsat, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));




            tcell.AddCell(new Phrase("P/R - Marine", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));



            tcell.AddCell(new Phrase(formatMoney(Math.Round(budgettotmar, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase(formatMoney(Math.Round(invoicetotmar, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            tcell.AddCell(new Phrase(formatMoney(Math.Round(balancebfmar, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase(formatMoney(Math.Round(fromrevmar, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase(formatMoney(Math.Round(-torevmar, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase(formatMoney(Math.Round(closingtotmar, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));


            tcell.AddCell(new Phrase("P/R Commercial (Data & Services)", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));


            tcell.AddCell(new Phrase(formatMoney(Math.Round(budgettotds, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase(formatMoney(Math.Round(invoicetotds, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            tcell.AddCell(new Phrase(formatMoney(Math.Round(balancebfds, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase(formatMoney(Math.Round(fromrevds, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase(formatMoney(Math.Round(-torevds, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase(formatMoney(Math.Round(closingtotds, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));





            tcell.AddCell(new Phrase("P/R - Aerounautical", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));


            tcell.AddCell(new Phrase(formatMoney(Math.Round(budgettotaero, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase(formatMoney(Math.Round(invoicetotaero, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            tcell.AddCell(new Phrase(formatMoney(Math.Round(balancebfaero, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase(formatMoney(Math.Round(fromrevaero, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase(formatMoney(Math.Round(-torevaero, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase(formatMoney(Math.Round(closingtotaero, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));




            tcell.AddCell(new Phrase("P/R - Trunking", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase(formatMoney(Math.Round(budgettottrunk, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase(formatMoney(Math.Round(invoicetottrunk, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            tcell.AddCell(new Phrase(formatMoney(Math.Round(balancebftrunk, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase(formatMoney(Math.Round(fromrevtrunk, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase(formatMoney(Math.Round(-torevtrunk, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase(formatMoney(Math.Round(closingtottrunk, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));




            tcell.AddCell(new Phrase("Other P/R Non Commercial", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            tcell.AddCell(new Phrase(formatMoney(Math.Round(budgettotother, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase(formatMoney(Math.Round(invoicetotother, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            tcell.AddCell(new Phrase(formatMoney(Math.Round(balancebfother, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase(formatMoney(Math.Round(fromrevother, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase(formatMoney(Math.Round(-torevother, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase(formatMoney(Math.Round(closingtotother, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));


            tcell.AddCell(new Phrase("Sum Total", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            decimal finBudgetTotal = 0, finInvoiceTotal = 0, finBalanceBf = 0, finFromRev = 0, finToRev = 0, finClosingBalance = 0;
            finBudgetTotal = budgettotother + budgettottrunk + budgettotvsat + budgettotmic + budgettotmar + budgettotcell + budgettotbb + budgettotaero + budgettotds;
            finInvoiceTotal = invoicetotaero + invoicetotbb + invoicetotcell + invoicetotds + invoicetotmar + invoicetotmicro + invoicetotother + invoicetottrunk + invoicetotvsat;
            finBalanceBf = balancebfaero + balancebfbb + balancebfcell + balancebfmar + balancebfmicro + balancebfother + balancebftrunk + balancebfvsat + balancebfds;
            finFromRev = fromrembb + fromremcell + fromrevaero + fromrevmar + fromrevmicro + fromrevds + fromrevother + fromrevtrunk + fromrevvsat;
            finToRev = torevaero + torevbb + torevcell + torevds + torevmar + torevmicro + torevother + torevtrunk + torevvsat;
            finClosingBalance = closingtotaero + closingtotbb + closingtotcell + closingtotds + closingtotmar + closingtotmicro + closingtotother + closingtottrunk + closingtotvsat;

            tcell.AddCell(new Phrase(formatMoney(Math.Round(finBudgetTotal, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase(formatMoney(Math.Round(finInvoiceTotal, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
            tcell.AddCell(new Phrase(formatMoney(Math.Round(finBalanceBf, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase(formatMoney(Math.Round(finFromRev, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase(formatMoney(Math.Round(-finToRev, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));

            tcell.AddCell(new Phrase(formatMoney(Math.Round(finClosingBalance, 2)), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));


            paragraphcell.SpacingAfter = 7; paragraphcell.Alignment = 1;
            doc.Add(paragraphcell);
            doc.Add(space);
            doc.Add(tcell); ;
            doc.Add(space);




            doc.Close();



            //Process pp = new Process();
            //pp.StartInfo.UseShellExecute = true;

            //pp.StartInfo.FileName = Testpath;
            //pp.Start();
        }

        bool IsEmpty(DataSet dataSet)
        {
            foreach (DataTable table in dataSet.Tables)
                if (table.Rows.Count != 0) return false;

            return true;
        }


        protected void Button1_Click(object sender, EventArgs e)
        {
            getdetails1();

        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            ddlccnum.DataBind();
        }



        protected void btn_Export_Click(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();

            IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().
                                              Select(column => column.ColumnName);
            sb.AppendLine(string.Join(", ", columnNames));

            foreach (DataRow row in dt.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                sb.AppendLine(string.Join(",", fields));
            }

            var Testpath1 = Server.MapPath("~/Excel.csv");
            File.WriteAllText(Testpath1, sb.ToString());

        }

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var error = "$('.message').slideDown(400, 'swing', function() { });";

            try
            {

                int i = _000.App_Code.BusinessClass.UpdateBudgetInfo(Convert.ToInt32(ddlccnum.SelectedItem.Text), Convert.ToDecimal(txtBudget.Text));
                if (i > 0)
                {
                    lblError.ForeColor = System.Drawing.ColorTranslator.FromHtml("#007f00");
                    lblError.Text = "Budget Succesfully Updated";
                    ClientScript.RegisterStartupScript(this.GetType(), "err", error, true);

                }
                else
                {

                    lblError.ForeColor = System.Drawing.ColorTranslator.FromHtml("#FA7B7B");
                    lblError.Text = "Please Check The Information you entered and try again";
                    ClientScript.RegisterStartupScript(this.GetType(), "err", error, true);
                }

            }

            catch (Exception ex)
            {
                lblError.ForeColor = System.Drawing.ColorTranslator.FromHtml("#FA7B7B");
                lblError.Text = "Please Check The Information you entered and try again";
                ClientScript.RegisterStartupScript(this.GetType(), "err", error, true);
            }
        }

        string formatMoney(decimal inputs)
        {
            string input = Convert.ToString(inputs);
            string neg = " ";
            if(input[0] == '-')
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
            if(neg[0] == '-')
            return "("+formatted+")";
            else return formatted;

        }

        protected void btnDeferred_Click(object sender, EventArgs e)
        {

            int year = 0;

            if (DropDownList1.SelectedValue == "2016")
            {
                year = 2016;
            }
            else if (DropDownList1.SelectedValue == "2017")
            {
                year = 2017;
            }

            else if (DropDownList1.SelectedValue == "2018")
            {
                year = 2018;
            }

            else if (DropDownList1.SelectedValue == "2019")
            {
                year = 2019;
            }

            else if (DropDownList1.SelectedValue == "2020")
            {
                year = 2020;
            }

            string month = "";

            if (ddl1.SelectedValue == "1")
            {
                month = "January";
            }

            else if (ddl1.SelectedValue == "2")
            {
                month = "February";
            }

            else if (ddl1.SelectedValue == "3")
            {
                month = "March";
            }

            else if (ddl1.SelectedValue == "4")
            {
                month = "April";
            }

            else if (ddl1.SelectedValue == "5")
            {
                month = "May";
            }

            else if (ddl1.SelectedValue == "6")
            {
                month = "June";
            }

            else if (ddl1.SelectedValue == "7")
            {
                month = "July";
            }

            else if (ddl1.SelectedValue == "8")
            {
                month = "August";
            }

            else if (ddl1.SelectedValue == "9")
            {
                month = "September";
            }

            else if (ddl1.SelectedValue == "10")
            {
                month = "October";
            }

            else if (ddl1.SelectedValue == "11")
            {
                month = "November";
            }

            else if (ddl1.SelectedValue == "12")
            {
                month = "December";
            }

            var Testpath = @"C:\\Users\\asms-accpac-1\\Documents\\" + "DefferedincomeReportFor" + month + " " + year + ".pdf";

            try
            {


                WebClient User = new WebClient();
                Byte[] FileBuffer = User.DownloadData(Testpath);
                if (FileBuffer != null)
                {
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-length", FileBuffer.Length.ToString());
                    Response.BinaryWrite(FileBuffer);
                }
            }
            catch (Exception ex)
            {

                var error = "$('.message').slideDown(400, 'swing', function() { });";
                lblError.ForeColor = System.Drawing.ColorTranslator.FromHtml("#FA7B7B");
                lblError.Text = "Report Does Not Exist For The Selected Month";
                ClientScript.RegisterStartupScript(this.GetType(), "err", error, true);
            }
        }

        protected void btnDeferred_Totals(object sender, EventArgs e)
        {
            int year = 0;

            if (DropDownList1.SelectedValue == "2016")
            {
                year = 2016;
            }
            else if (DropDownList1.SelectedValue == "2017")
            {
                year = 2017;
            }

            else if (DropDownList1.SelectedValue == "2018")
            {
                year = 2018;
            }

            else if (DropDownList1.SelectedValue == "2019")
            {
                year = 2019;
            }

            else if (DropDownList1.SelectedValue == "2020")
            {
                year = 2020;
            }

            string month = "";

            if (ddl1.SelectedValue == "1")
            {
                month = "January";
            }

            else if (ddl1.SelectedValue == "2")
            {
                month = "February";
            }

            else if (ddl1.SelectedValue == "3")
            {
                month = "March";
            }

            else if (ddl1.SelectedValue == "4")
            {
                month = "April";
            }

            else if (ddl1.SelectedValue == "5")
            {
                month = "May";
            }

            else if (ddl1.SelectedValue == "6")
            {
                month = "June";
            }

            else if (ddl1.SelectedValue == "7")
            {
                month = "July";
            }

            else if (ddl1.SelectedValue == "8")
            {
                month = "August";
            }

            else if (ddl1.SelectedValue == "9")
            {
                month = "September";
            }

            else if (ddl1.SelectedValue == "10")
            {
                month = "October";
            }

            else if (ddl1.SelectedValue == "11")
            {
                month = "November";
            }

            else if (ddl1.SelectedValue == "12")
            {
                month = "December";
            }
            try
            {
                var Testpath = @"C:\\Users\\asms-accpac-1\\Documents\\DefferedincomeReport_SectionSubTotalsFor" + month + " " + year + ".pdf";
                WebClient User = new WebClient();
                Byte[] FileBuffer = User.DownloadData(Testpath);
                if (FileBuffer != null)
                {
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-length", FileBuffer.Length.ToString());
                    Response.BinaryWrite(FileBuffer);
                }
            }
            catch (Exception ex)
            {

                var error = "$('.message').slideDown(400, 'swing', function() { });";
                lblError.ForeColor = System.Drawing.ColorTranslator.FromHtml("#FA7B7B");
                lblError.Text = "Report Does Not Exist For The Selected Month";
                ClientScript.RegisterStartupScript(this.GetType(), "err", error, true);
            }
        }

        protected void download()
        {
            int year = 0;

            if (DropDownList1.SelectedValue == "2016")
            {
                year = 2016;
            }
            else if (DropDownList1.SelectedValue == "2017")
            {
                year = 2017;
            }

            else if (DropDownList1.SelectedValue == "2018")
            {
                year = 2018;
            }

            else if (DropDownList1.SelectedValue == "2019")
            {
                year = 2019;
            }

            else if (DropDownList1.SelectedValue == "2020")
            {
                year = 2020;
            }

            string month = "";

            if (ddl1.SelectedValue == "1")
            {
                month = "January";
            }

            else if (ddl1.SelectedValue == "2")
            {
                month = "February";
            }

            else if (ddl1.SelectedValue == "3")
            {
                month = "March";
            }

            else if (ddl1.SelectedValue == "4")
            {
                month = "April";
            }

            else if (ddl1.SelectedValue == "5")
            {
                month = "May";
            }

            else if (ddl1.SelectedValue == "6")
            {
                month = "June";
            }

            else if (ddl1.SelectedValue == "7")
            {
                month = "July";
            }

            else if (ddl1.SelectedValue == "8")
            {
                month = "August";
            }

            else if (ddl1.SelectedValue == "9")
            {
                month = "September";
            }

            else if (ddl1.SelectedValue == "10")
            {
                month = "October";
            }

            else if (ddl1.SelectedValue == "11")
            {
                month = "November";
            }

            else if (ddl1.SelectedValue == "12")
            {
                month = "December";
            }
            var fileName = @"\DefferedincomeReportFor" + month + " " + year + ".pdf";

            // string doc = MapPath("C:/Users/asms-accpac-1/Documents/");


            // string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            //StreamWriter output = new StreamWriter(mydocpath + @"\" + "outputInv" + ".txt");
            var mydocpath = "C:\\Users\\asms-accpac-1\\Documents";
            var Testpath = "~/" + mydocpath + fileName;

            var TestPath = mydocpath;


            byte[] Content = File.ReadAllBytes(@TestPath);
            Response.ContentType = "text /pdf";
            Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
            Response.BufferOutput = true; ;
            Response.OutputStream.Write(Content, 0, Content.Length);
            Response.End();

            //Downloader.Download(fileName, Testpath);
        }

        public int CheckMonths(DateTime sdate, DateTime edate)
        {

            TimeSpan ts = edate - sdate;

            int tss = Convert.ToInt32(ts.Days);

            int months = tss / 30;
            // if (months == 2) months = 3;
            return months;
        }

        public int CheckMonthsModification(DateTime sdate, DateTime edate)
        {

            int months = ((edate.Year - sdate.Year) * 12) + edate.Month - sdate.Month;


            return months;
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/MenuGrid.aspx");
        }

        protected void btntry_Click(object sender, EventArgs e)
        {
            download();
        }
    }
}




