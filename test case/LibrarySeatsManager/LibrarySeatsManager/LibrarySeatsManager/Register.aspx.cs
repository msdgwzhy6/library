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
    public partial class RegisterPage : System.Web.UI.Page
    {
        String userName;
        String password;
        UserManager user = new UserManager();
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Register_Click(object sender, EventArgs e)
        {
            userName = this.UsernameText.Text.ToString();
            password = this.PasswordText.Text.ToString();
            if (user.AddUser(userName, password, "Student"))
            {
                this.RegisterStartupScript("", "<script language=javascript>alert('注册成功！')</script>");
            }
            else
            {
                this.RegisterStartupScript("", "<script language=javascript>alert('注册失败！')</script>");
            }
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Default.aspx");
        }
    }
}
