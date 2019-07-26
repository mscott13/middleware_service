using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace Interface
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        string uname = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["uname"] != null)
            {
                uname = Session["uname"].ToString();
            }
        }

        protected void btnLogIn_Click(object sender, EventArgs e)
        {
            var error = "$('.message').slideDown(400, 'swing', function() { });";
            // string username = Session["uname"].ToString();
            if (txtPsw.Text == txtCPsw.Text)
            {
                try
                {
                    MembershipUser mu = Membership.GetUser(uname);
                    mu.ChangePassword(mu.ResetPassword(), txtPsw.Text);
                    lblError.ForeColor = System.Drawing.ColorTranslator.FromHtml("#007f00");
                    lblError.Text = "Password Successfully Changed";
                    ClientScript.RegisterStartupScript(this.GetType(), "err", error, true);
                }

                catch (Exception ex)
                {
                    lblError.ForeColor = System.Drawing.ColorTranslator.FromHtml("#FA7B7B");
                    lblError.Text = ex.Message;
                    ClientScript.RegisterStartupScript(this.GetType(), "err", error, true);
                }
            }
            else {
                lblError.ForeColor = System.Drawing.ColorTranslator.FromHtml("#FA7B7B");
                lblError.Text = "Passwords Do Not Match";
                ClientScript.RegisterStartupScript(this.GetType(), "err", error, true);
            }
        }
    }
}