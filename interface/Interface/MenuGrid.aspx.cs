using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interface
{
    public partial class MenuGrid : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
     
            if (Session["Admin"] == null)
            {
                if(Session["uname"] == null)
                {
                    Response.Redirect("~/login.aspx");
                }
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.RemoveAll();
            Response.Redirect("~/login.aspx");
        }
    }
}