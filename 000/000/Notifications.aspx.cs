using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace _000
{
    public partial class Notifications : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DataTable tbl = new DataTable();
            tbl.Columns.Add("Customer #");
            tbl.Columns.Add("Date");
            tbl.Columns.Add("Status");
            for (int i=0; i<8; i++) { 
           
           

            DataRow row = tbl.NewRow();
            row["Customer #"] = "00009-L";
            row["Date"] = "03/07/2016";
            row["Status"] = "Test";

            tbl.Rows.Add(row);

            }

            grdNotifications.DataSource = tbl;
            grdNotifications.DataBind();

            
        }

        protected void grdNotifications_SelectedIndexChanged(object sender, EventArgs e)
        {
            var s = e;
        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            dateLabel.Text = DateTime.Now.ToString();
            
        }
    }
}