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
    public partial class HomePage : System.Web.UI.Page
    {
        SeatStateManager seat = new SeatStateManager();
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private void getData() 
        {
            DataTable dt = new DataTable();
            dt = seat.QuerySeatsStates();
            this.SeatGrid.DataSource = dt;
            this.SeatGrid.DataBind();           
        }

        protected void Query_Click(object sender, EventArgs e)
        {
            getData();
        }

        protected void Authorize_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Authorize.aspx");
        }

        protected void Exit_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Default.aspx");
        }

        protected void Clear_Click(object sender, EventArgs e)
        {

        }
    }
}
