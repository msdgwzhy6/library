using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Data.SqlClient;

namespace LibrarySeatsManager
{
    public class RoleManager:DataManager
    {
        private DataTable GetDataTable()
        {
            DataTable roleInformationTable = new DataTable();
            String commandText = "Select Roles from RoleInformation";
            SqlDataAdapter roleInformationAdapter = CreateDataAdapter(GetConnectionString(), commandText);
            roleInformationAdapter.Fill(roleInformationTable);
            return roleInformationTable;
        }
     
    }
}
