using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace _000
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                MembershipUser newuser = Membership.CreateUser(txtUsr.Text, txtPsw.Text);
                lblError.ForeColor = System.Drawing.ColorTranslator.FromHtml("#87CDA3");
                lblError.Text = "Account Created";
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
                lblError.ForeColor = System.Drawing.ColorTranslator.FromHtml("#FA7B7B");
                return;
            }
          
        }
    }
}