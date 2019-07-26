using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace Interface
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["uname"] != null || Session["Admin"]!= null)
            {
                Response.Redirect("~/MenuGrid.aspx");
            }
        }

        protected void btnLogIn_Click(object sender, EventArgs e)
        {
            Integration intlink = new Integration();
            var error = "$('.message').slideDown(400, 'swing', function() { });";
            try
            {
                if (Membership.ValidateUser(txtUname.Text, txtPsw.Text))
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
                    ClientScript.RegisterStartupScript(this.GetType(), "err", error, true);
                }
            }catch(Exception ex)
            {
                lblError.ForeColor = System.Drawing.ColorTranslator.FromHtml("#FA7B7B");
                lblError.Text = ex.Message;
                ClientScript.RegisterStartupScript(this.GetType(), "err", error, true);
            }
        }
    }
}