using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace _000
{
    public partial class UpdateBudget : System.Web.UI.Page
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

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try {

               int i= _000.App_Code.BusinessClass.UpdateBudgetInfo(Convert.ToInt32(txtCcnum.Text), Convert.ToDecimal(txtBudget.Text));
                if(i>0)
                {
                    lblError.ForeColor = System.Drawing.ColorTranslator.FromHtml("#007f00");
                    lblError.Text = "Budget Succesfully Updated";

                }
                else
                {

                    lblError.ForeColor = System.Drawing.ColorTranslator.FromHtml("#FA7B7B");
                    lblError.Text = "Please Check The Information you entered and try again";
                }

            }

            catch (Exception ex)
            {
                lblError.ForeColor = System.Drawing.ColorTranslator.FromHtml("#FA7B7B");
                lblError.Text = "Please Check The Information you entered and try again";
            }
        }
    }
}