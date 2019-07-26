using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interface
{
    public partial class RegisterAdmin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Admin"] == null)
            {
                Response.Redirect("~/MenuGrid.aspx");
            }
        }

        protected void btnLogIn_Click(object sender, EventArgs e)
        {
            var msg = "$('.message').slideDown(400, 'swing', function() { });";
         

            if (txtPsw.Text == txtPsw2.Text)
            {
                try
                {
                    MembershipUser newuser = Membership.CreateUser(txtUsr.Text, txtPsw.Text);
                    Roles.AddUserToRole(txtUsr.Text, "Admin");
                    lblError.ForeColor = System.Drawing.ColorTranslator.FromHtml("#87CDA3");
                    lblError.Text = "Account Created";

                    ClientScript.RegisterStartupScript(this.GetType(), "err", msg, true);
       
                }
                catch (Exception ex)
                {
                    lblError.Text = ex.Message;
                    lblError.ForeColor = System.Drawing.ColorTranslator.FromHtml("#FA7B7B");
                    ClientScript.RegisterStartupScript(this.GetType(), "err", msg, true);
                    return;
                }

            }
            else
            {
                lblError.Text = "Passwords do not Match";
                lblError.ForeColor = System.Drawing.ColorTranslator.FromHtml("#FA7B7B");
                ClientScript.RegisterStartupScript(this.GetType(), "err", msg, true);
            }
        }
    }
}