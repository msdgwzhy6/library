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
    public class DataManager
    {
        private String connectionString =
        "Data Source=LAIZF-PC\\LAIZF;Initial Catalog=LibrarySeatsManagement;Integrated Security=True";

        virtual protected String GetConnectionString()
        {
            return connectionString;
        }

        virtual protected SqlDataAdapter CreateDataAdapter(String connectionString, String commandText)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandText = commandText;
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
            return dataAdapter;
        }
    }
}
