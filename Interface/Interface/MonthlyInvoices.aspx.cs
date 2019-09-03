using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace _000
{
    public partial class MonthlyInvoices : System.Web.UI.Page
    {
        public decimal regTotal = 0;
        public decimal specTotal = 0;
        public decimal typeTotal = 0;

        public int regCount = 0;
        public int specCount = 0;
        public int typeCount = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Admin"] == null)
            {
                if (Session["uname"] == null)
                {
                    Response.Redirect("~/login.aspx");
                }
            }
        }

        protected void GetInvoices(object sender, EventArgs e)
        {
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

            try {

                Interface.ReturnData specData = _000.App_Code.BusinessClass.GetSpecInvoicesFormonth(month, year);
                DataSet dspec = specData.ds;
                specTotal = specData.amount;
                specCount = specData.count;

                Interface.ReturnData regData = _000.App_Code.BusinessClass.GetRegInvoicesFormonth(month, year);
                DataSet ds = regData.ds;
                regTotal = regData.amount;
                regCount = regData.count;

                Interface.ReturnData typeData = _000.App_Code.BusinessClass.GetTypeApprovalInvoicesFormonth(month, year);
                DataSet dtainv = typeData.ds;
                typeTotal = typeData.amount;
                typeCount = typeData.count;

                GridView1.DataSource = ds;
                GridView1.DataBind();
                GridView2.DataSource = dspec;
                GridView2.DataBind();
                GridView3.DataSource = dtainv;
                GridView3.DataBind();


                DataSet dd = _000.App_Code.BusinessClass.GetRegInvoiceTotalFormonth(month, year);
                DataSet dspecs = _000.App_Code.BusinessClass.GetSpecInvoiceTotalFormonth(month, year);
                DataSet de = _000.App_Code.BusinessClass.GetInvoiceTotalFormonth(month, year);
                DataSet dta = _000.App_Code.BusinessClass.GetTypeApprovalInvoiceTotalFormonth(month, year);

                DataSet df = _000.App_Code.BusinessClass.GetNumberInvoicesFormonth(month, year);

                

                lblOverview.Text = "Overview";
                DataRow dr = df.Tables[0].Rows[0];
                lblOverviewdesc1.Text = "Total Number of Invoices for the month: ";
                lblnuminvoices.Text = (regCount+specCount+typeCount).ToString();

                DataRow drr = dd.Tables[0].Rows[0];
                lblinvTotal.Text = "Total Regulatory Invoice Amount for the month: ";
                Decimal result;

                if (decimal.TryParse(regTotal.ToString(), out result))


                    lblinvtot.Text = "$" + formatMoney(regTotal);
                else lblinvtot.Text = "$" + 0;


                DataRow dspecc = dspecs.Tables[0].Rows[0];
                lblspecdesc.Text = "Total Spectrum Invoice Amount for the month: ";
                Decimal resultt;

                if (decimal.TryParse(specTotal.ToString(), out resultt))


                    lblspecamt.Text = "$" + formatMoney(specTotal);
                else lblspecamt.Text = "$" + 0;


                DataRow dtot = de.Tables[0].Rows[0];
                lblOverviewdesc2.Text = "Total Invoice Amount for the month: ";
                Decimal resulttt;
                decimal invoiceMonthTotal = regTotal + specTotal + typeTotal;

                if (decimal.TryParse(invoiceMonthTotal.ToString(), out resulttt))


                    lbltotinv.Text = "$" + formatMoney(invoiceMonthTotal);
                else lbltotinv.Text = "$" + 0;


                DataRow dtype = dta.Tables[0].Rows[0];
                lblTADesc.Text = "Total Type Approval Invoice Amount for the month: ";
                Decimal resultttt;

                if (decimal.TryParse(dtype["InvoiceTotal"].ToString(), out resultttt))


                    lblTAamt.Text = "$" + formatMoney(resultttt);
                else lblTAamt.Text = "$" + 0;


                
            }
            catch(Exception ex)
            {

            }

        }

        string formatMoney(decimal inputs)
        {
            string input = Convert.ToString(inputs);
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

            return formatted;
        }


        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/MenuGrid.aspx");
        }
    }
}