using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace Interface
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnLogIn_Click(object sender, EventArgs e)
        {
            var error = "$('.message').slideDown(400, 'swing', function() { });";

            MembershipUser mu = Membership.GetUser(txtUsr.Text);
            if (mu==null)
            {
                lblError.Text = "User does not exist!";
                lblError.ForeColor = System.Drawing.ColorTranslator.FromHtml("#FA7B7B");
                ClientScript.RegisterStartupScript(this.GetType(), "err", error, true);
            }
            else
            {
                try
                {

                    Membership.DeleteUser(txtUsr.Text);
                    lblError.Text = "User Deleted!";
                    lblError.ForeColor = System.Drawing.ColorTranslator.FromHtml("#007f00");
                    ClientScript.RegisterStartupScript(this.GetType(), "err", error, true);
                }

                catch (Exception ex)
                {

                    lblError.Text = "Error Deleting User!";
                    lblError.ForeColor = System.Drawing.ColorTranslator.FromHtml("#FA7B7B");
                    ClientScript.RegisterStartupScript(this.GetType(), "err", error, true);
                }
            }
           
        }
    }
}