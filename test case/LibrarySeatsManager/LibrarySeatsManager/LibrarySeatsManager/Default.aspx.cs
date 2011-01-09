using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace LibrarySeatsManager
{
    public partial class LoginPage : System.Web.UI.Page
    {
        String userName;
        String password;
        UserManager user = new UserManager();
        
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Login_Click(object sender, EventArgs e)
        {
            userName = this.UsernameText.Text.ToString();
            password = this.PasswordText.Text.ToString();
            if (user.ValidateUser(userName, password))
            {
                Response.Redirect("~/Home.aspx");
            }
            else
            {
                this.RegisterStartupScript("", "<script language=javascript>alert('登录失败！')</script>");
            }  
        }

        protected void Register_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Register.aspx");
        }
    }
}
