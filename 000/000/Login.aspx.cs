using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace _000
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
         
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                if (Membership.ValidateUser(txtUsr.Text, txtPsw.Text))
                {

                    FormsAuthentication.RedirectFromLoginPage(txtUsr.Text, false);
                    Response.Redirect("Default.aspx");

                }
                else
                {
                    lblError.ForeColor = System.Drawing.ColorTranslator.FromHtml("#FA7B7B");
                    lblError.Text = "Invalid login";

                }
            }
            catch(Exception ex)
            {
                lblError.ForeColor = System.Drawing.ColorTranslator.FromHtml("#FA7B7B");
                lblError.Text = ex.Message;
            }
           
        }
    }
}