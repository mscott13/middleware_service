using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace Interface
{
    public partial class ResetPassword : System.Web.UI.Page
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

        protected void btnLogIn_Click(object sender, EventArgs e)
        {
            var error = "$('.message').slideDown(400, 'swing', function() { });";

            try
                {
                    MembershipUser mu = Membership.GetUser(txtUsr.Text);
                    mu.ChangePassword(mu.ResetPassword(), "password");
                    lblError.ForeColor = System.Drawing.ColorTranslator.FromHtml("#007f00");
                    lblError.Text = "Password Updated";
                ClientScript.RegisterStartupScript(this.GetType(), "err", error, true);
            }

                catch (Exception ex)
                {
                    lblError.ForeColor = System.Drawing.ColorTranslator.FromHtml("#FA7B7B");
                    lblError.Text = "User does not exist";
                ClientScript.RegisterStartupScript(this.GetType(), "err", error, true);
            }
            }
          
        
    }
}