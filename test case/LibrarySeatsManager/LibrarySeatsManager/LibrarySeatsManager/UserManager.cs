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
    public class UserManager:DataManager
    {
        private String commandText = "Select Account, Password, Role from UserInformation";

        private SqlDataAdapter GetDataAdapter()
        {
            SqlDataAdapter dataAdapter = CreateDataAdapter(GetConnectionString(), commandText);
            SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);
            return dataAdapter;
        }

        private DataTable GetDataTable()
        {
            DataTable userInformationTable = new DataTable();
            GetDataAdapter().Fill(userInformationTable);
            return userInformationTable;
        }

        public Boolean ValidateUser(String account, String password)
        {
            DataTable userInformationTable = GetDataTable();
            if (!CheckIfUserExist(account, userInformationTable))
            {
                return false;
            }
            String expression = CreateCheckPasswordExpression(account, password);
            return CheckIfPasswordRight(userInformationTable, expression);
        }

        private Boolean CheckIfUserExist(String account, DataTable userInformationTable)
        {
            String expression = CreateCheakUserExistExpression(account);
            DataRow[] accounts = userInformationTable.Select(expression);

            foreach (DataRow validatedAccount in accounts)
            {
                return true;
            }
            return false;
        }

        private String CreateCheakUserExistExpression(String account)
        {
            String expression = "Account = '";
            expression += account;
            expression += "'";
            return expression;
        }

        private Boolean CheckIfPasswordRight(DataTable userInformationTable, String expression)
        {
            DataRow[] accounts = userInformationTable.Select(expression);

            foreach (DataRow validatedAccount in accounts)
            {
                return true;
            }
            return false;
        }

        private String CreateCheckPasswordExpression(String account, String password)
        {
            String expression = "Account = '";
            expression += account;
            expression += "' and Password = '";
            expression += password;
            expression += "'";
            return expression;
        }

        public Boolean AddUser(String account, String password, String role)
        {
            DataTable userInformationTable = GetDataTable();

            if (CheckIfUserExist(account, userInformationTable))
            {
                return false;
            }
            
            DataRow newRow = CreateNewRow(account, password, role, userInformationTable);
            userInformationTable.Rows.Add(newRow);
            return InsertNewRow(userInformationTable);
        }

        private Boolean InsertNewRow(DataTable userInformationTable)
        {
            SqlDataAdapter userInformationAdapter = GetDataAdapter();
            userInformationAdapter.Update(userInformationTable);
            return true;
        }

        private DataRow CreateNewRow(String account, String password, String role, DataTable userInformationTable)
        {
            DataRow newRow = userInformationTable.NewRow();
            newRow["Account"] = account;
            newRow["Password"] = password;
            newRow["Role"] = role;
            return newRow;
        }

        public DataTable GetAllUsers()
        {
            DataTable userInformationTable = GetDataTable();
            userInformationTable.Columns.RemoveAt(1);
            return userInformationTable;
        }

        public Boolean RemoveUser(String account)
        {
            SqlDataAdapter userInformationAdapter = GetDataAdapter();
            DataTable userInformationTable = GetDataTable();
            if (!CheckIfUserExist(account, userInformationTable))
            {
                return false;
            }
            String expression = CreateRemoveUserExpression(account);
            DataRow[] removedRow = userInformationTable.Select(expression);
            return RemoveRow(userInformationAdapter, userInformationTable, removedRow);
        }

        private Boolean RemoveRow(SqlDataAdapter userInformationAdapter, DataTable userInformationTable, DataRow[] removedRow)
        {
            foreach (DataRow row in removedRow)
            {
                row.Delete();
                userInformationAdapter.Update(userInformationTable);
                return true;
            }
            return false;
        }

        private String CreateRemoveUserExpression(String account)
        {
            String expression = "Account = '";
            expression += account;
            expression += "'";
            return expression;
        }

        public Boolean IsAuthorize(String account, String password)
        {
            DataTable userInformationTable = GetDataTable();
            String expression = CreateIsAuthorizeExpression(account, password);
            DataRow[] accounts = userInformationTable.Select(expression);
            foreach (DataRow validatedAccount in accounts)
            {
                return true;
            }
            return false;
        }

        private String CreateIsAuthorizeExpression(String account, String password)
        {
            String expression = "Account = '";
            expression += account;
            expression += "' and Password = '";
            expression += password;
            expression += "' and Role = 'Administrator'";
            return expression;
        }

    }
}
