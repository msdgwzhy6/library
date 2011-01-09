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
    public partial class WebForm1 : System.Web.UI.Page
    {
        String UserName;
        String Password;
        String CheckUserName;
        UserManager user = new UserManager();
        SeatStateManager seat = new SeatStateManager();

        protected void Page_Load(object sender, EventArgs e)
        {

        }
        //login button
        protected void Button2_Click(object sender, EventArgs e)
        {
            UserName = TextBox1.Text.ToString();
            Password = TextBox2.Text.ToString();
            CheckUserName = TextBox4.Text.ToString();

            //UserManager user = new UserManager();
            if (user.ValidateUser(UserName, Password))
            {
                Panel2.Visible = false;
                Panel1.Visible = true;
                //是否为管理员
                if(user.IsAuthorize(UserName,Password))
                {
                    Button11.Visible = true;
                    Button12.Visible = true;
                }
            }
            else
            {
                this.RegisterStartupScript("", "<script language=javascript>alert('用户名或密码错误！')</script>"); 
            }            
        }
        //register button
        protected void Button1_Click(object sender, EventArgs e)
        {
            UserName = TextBox1.Text.ToString();
            Password = TextBox2.Text.ToString();
            CheckUserName = TextBox4.Text.ToString();

            if (user.AddUser(UserName, Password, "Student"))
            {
                this.RegisterStartupScript("", "<script language=javascript>alert('注册成功！')</script>");
            }
            else
            {
                this.RegisterStartupScript("", "<script language=javascript>alert('注册失败！')</script>");
            }
        }
        //exit
        protected void Button13_Click(object sender, EventArgs e)
        {
            Panel1.Visible = false;
            Panel2.Visible = true;
        }
        //authorize
        protected void Button11_Click(object sender, EventArgs e)
        {
            Panel1.Visible = false;
            Panel3.Visible = true;
        }
        //authorize confirm
        protected void Button15_Click(object sender, EventArgs e)
        {

            Panel1.Visible = true;
            Panel3.Visible = false;
        }
        //authorize cancel
        protected void Button16_Click(object sender, EventArgs e)
        {
            Panel1.Visible = true;
            Panel3.Visible = false;
        }

        //query seats
        protected void Button9_Click(object sender, EventArgs e)
        {
            
        }
        //update seats' state
        protected void Button10_Click(object sender, EventArgs e)
        {

        }
        //Clear Occupied Seat
        protected void Button12_Click(object sender, EventArgs e)
        {
        }

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }      
    }
}
