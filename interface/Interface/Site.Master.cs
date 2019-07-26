using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using ACCPAC.Advantage;
using System.Data.SqlClient;
using System.Configuration;
using System.Threading;
namespace _000
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           
        }




        protected void btnLogin_Click(object sender, EventArgs e)
        {
            Response.Redirect("/Login.aspx");
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
        
            FormsAuthentication.SignOut();
            Response.Redirect("Login.aspx");
            
        }
    }
}