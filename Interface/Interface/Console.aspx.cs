using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace Interface
{
    public partial class Console : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Admin"] == null)
            {
                Response.Redirect("~/MenuGrid.aspx");
            }
            else
            {
                DataSet ds = _000.App_Code.BusinessClass.RetrieveUsers();
                grdUsers.DataSource = ds;
                grdUsers.DataBind();
            }
           
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/MenuGrid.aspx");
        }
    }
}
