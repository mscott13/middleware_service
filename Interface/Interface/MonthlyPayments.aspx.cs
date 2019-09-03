using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace _000
{
    public partial class MonthlyPayments : System.Web.UI.Page
    {
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

            protected void GetPayments(object sender, EventArgs e)
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

                

               
                DataSet df = _000.App_Code.BusinessClass.GetNumberPaymentsFormonth(month, year);
                DataSet de = _000.App_Code.BusinessClass.GetPaymentsTotalFormonth(month, year);
                DataSet dspec = _000.App_Code.BusinessClass.GetSpecPaymentsTotalFormonth(month, year);
                DataSet dreg = _000.App_Code.BusinessClass.GetRegPaymentsTotalFormonth(month, year);
                DataSet dtype = _000.App_Code.BusinessClass.GetTypeApprovalPaymentsTotalFormonth(month, year);
                DataSet dspecc = _000.App_Code.BusinessClass.GetSpecPaymentsForMonth(month, year);
                DataSet dregg = _000.App_Code.BusinessClass.GetRegPaymentsForMonth(month, year);
                DataSet dtypee = _000.App_Code.BusinessClass.GetTypeApprovalPaymentsForMonth(month, year);

                lblOverview.Text = "Overview";
                decimal result;
                DataRow dff = de.Tables[0].Rows[0];
                lblOverviewdesc1.Text = "Total Payment Amount for the month: ";
                if (decimal.TryParse(dff["PaymentTotal"].ToString(), out result))

                    lbltotinv.Text = "$" + formatMoney(result);
                else lbltotinv.Text = "$" + 0;

                decimal resultt;
                DataRow dspecs = dspec.Tables[0].Rows[0];
                lblspecdesc.Text = "Total Spectrum Payment Amount for the month: ";
                if (decimal.TryParse(dspecs["PaymentTotal"].ToString(), out resultt))

                    lblspecamt.Text = "$" + formatMoney(resultt);
                else lblspecamt.Text = "$" + 0;

                decimal resulttt;
                DataRow dregs = dreg.Tables[0].Rows[0];
                lblregtotdesc.Text = "Total Regulatory Payment Amount for the month: ";
                if (decimal.TryParse(dregs["PaymentTotal"].ToString(), out resulttt))

                    lblregamt.Text = "$" + formatMoney(resulttt);
                else lblregamt.Text = "$" + 0;

                decimal resultttt;
                DataRow dtypes = dtype.Tables[0].Rows[0];
                lblTADesc.Text = "Total Type Approval Payment Amount for the month: ";
                if (decimal.TryParse(dtypes["PaymentTotal"].ToString(), out resultttt))

                    lblTAamt.Text = "$" + formatMoney(resultttt);
                else lblTAamt.Text = "$" + 0;



                DataRow dr = df.Tables[0].Rows[0];
                lblOverviewdesc2.Text = "Total Number of Payments for the month: ";
                lblnuminvoices.Text = dr.ItemArray.GetValue(0).ToString();
                DataSet ds = _000.App_Code.BusinessClass.GetPaymentsForMonth(month, year);
                GridView1.DataSource = dregg;
                GridView1.DataBind();
                GridView2.DataSource = dspecc;
                GridView2.DataBind();
                GridView3.DataSource = dtypee;
                GridView3.DataBind();
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
