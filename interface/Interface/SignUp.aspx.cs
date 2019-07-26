using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace Interface
{
    public partial class SignUp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnLogIn_Click(object sender, EventArgs e)
        {
            Integration intlink = new Integration();
            var msg = "$('.message').slideDown(400, 'swing', function() { });";
            var action = "$('.overlay').addClass('activate');";
            var action2 = "$('.signUp-form').addClass('loginactive');";

            if (txtPsw.Text == txtPsw2.Text)
            {
                try
                {
                    MembershipUser newuser = Membership.CreateUser(txtUsr.Text, txtPsw.Text);
                    Roles.AddUserToRole(txtUsr.Text, "Regular");
                    lblError.ForeColor = System.Drawing.ColorTranslator.FromHtml("#87CDA3");
                    lblError.Text = "Account Created";
                    intlink.Log("Account created for user: " + txtUsr.Text);

                    ClientScript.RegisterStartupScript(this.GetType(), "err", msg, true);
                    ClientScript.RegisterStartupScript(this.GetType(), "action", action, true);
                    ClientScript.RegisterStartupScript(this.GetType(), "action2", action2, true);
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

        protected void Button1_Click(object sender, EventArgs e)
        {
            Integration intlink = new Integration();
            var action = "$('.overlay').addClass('activate');";
            var action2 = "$('.signUp-form').addClass('loginactive');";
            var error = "$('.message').slideDown(400, 'swing', function() { });";
            try
            {
                if (Membership.ValidateUser(txtUname.Text, txtPsw_.Text))
                {
                    if (Roles.IsUserInRole(txtUname.Text, "Admin"))
                    {
                        intlink.Log("User: " + txtUname.Text + " logged in");
                        Session["Admin"] = txtUname.Text;
                        Session.Timeout = 1800;
                        FormsAuthentication.RedirectFromLoginPage(txtUname.Text, false);
                        Response.Redirect("MenuGrid.aspx");
                    }

                    else if (Roles.IsUserInRole(txtUname.Text, "Regular"))
                    {
                        intlink.Log("User: " + txtUname.Text + " logged in");
                        Session["uname"] = txtUname.Text;
                        Session.Timeout = 1800;
                        FormsAuthentication.RedirectFromLoginPage(txtUname.Text, false);
                        Response.Redirect("MenuGrid.aspx");
                    }

                }
                else
                {

                    lblError.ForeColor = System.Drawing.ColorTranslator.FromHtml("#FA7B7B");
                    lblError.Text = "Invalid login";
                    ClientScript.RegisterStartupScript(this.GetType(), "err1", error, true);
                    ClientScript.RegisterStartupScript(this.GetType(), "action", action, true);
                    ClientScript.RegisterStartupScript(this.GetType(), "action2", action2, true);
                }
            }
            catch (Exception ex)
            {
                lblError.ForeColor = System.Drawing.ColorTranslator.FromHtml("#FA7B7B");
                lblError.Text = ex.Message;
                ClientScript.RegisterStartupScript(this.GetType(), "err1", error, true);
            }
        }
    }
}