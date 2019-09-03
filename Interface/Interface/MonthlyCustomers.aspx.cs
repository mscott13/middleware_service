using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace _000
{
    public partial class MonthlyCustomers : System.Web.UI.Page
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


        protected void GetCustomers(object sender, EventArgs e)
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

                DataSet df = _000.App_Code.BusinessClass.GetNumberCustomersFormonth(month, year);


                DataRow dr = df.Tables[0].Rows[0];
                lblTotal.Text = "Total Number of Customers for the month: ";
                lbltot.Text = dr.ItemArray.GetValue(0).ToString();
                DataSet ds = _000.App_Code.BusinessClass.GetCustomersFormonth(month, year);
                GridView1.DataSource = ds;
                GridView1.DataBind();
            }

            catch(Exception ex)
            {
                
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/MenuGrid.aspx");
        }
    }
}