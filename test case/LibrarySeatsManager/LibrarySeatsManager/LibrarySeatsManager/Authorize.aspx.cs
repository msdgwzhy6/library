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
    public partial class AuthorizePage : System.Web.UI.Page
    {
        String userName;
        UserManager user = new UserManager();
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Authorize_Click(object sender, EventArgs e)
        {
            userName = this.UsernameText.Text.ToString();
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Home.aspx");
        }

     
    }
}
