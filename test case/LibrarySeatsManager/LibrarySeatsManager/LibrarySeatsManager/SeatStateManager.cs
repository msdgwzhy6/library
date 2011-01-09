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
using NUnit.Framework;
namespace LibrarySeatsManager
{
    public class SeatStateManager:DataManager
    {
        String commandText = "Select Row, Col, States from SeatsInformation";

        private SqlDataAdapter GetDataAdapter()
        {
            SqlDataAdapter dataAdapter = CreateDataAdapter(GetConnectionString(), commandText);
            SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);
            return dataAdapter;
        }

        private DataTable GetDataTable()
        {
            DataTable seatInformationTable = new DataTable();
            SqlDataAdapter dataAdapter = GetDataAdapter();
            dataAdapter.Fill(seatInformationTable);
            return seatInformationTable;
        }

        public Boolean UpdateSeatsStates(int row, int column, String state)
        {
            DataTable seatsStatesTable = GetDataTable();
            SqlDataAdapter seatsStatesAdapter = GetDataAdapter();
            String expression = CreateSelectSeatExpression(row, column);
            DataRow[] updateRows = seatsStatesTable.Select(expression);
            foreach (DataRow updateRow in updateRows)
            {
                updateRow["States"] = state;
            }
            seatsStatesAdapter.Update(seatsStatesTable);
            return true;
        }

        private static String CreateSelectSeatExpression(int row, int column)
        {
            String expression = "Row = '";
            expression += row.ToString();
            expression += "' AND Col = '";
            expression += column.ToString();
            expression += "'";
            return expression;
        }

        public DataTable QuerySeatsStates()
        {   
            return GetDataTable();
        }

        public Boolean ClearOccupiedSeat(int row, int column)
        {
            DataTable seatsStatesTable = GetDataTable();
            String expression = CreateSelectSeatExpression(row, column);
            DataRow[] modifiedRows = seatsStatesTable.Select(expression);
            return ModifyOccupiedSeatState(seatsStatesTable, modifiedRows);
        }

        private Boolean ModifyOccupiedSeatState(DataTable seatsStatesTable, DataRow[] modifiedRows)
        {
            foreach (DataRow modifiedRow in modifiedRows)
            {
                if (modifiedRow["States"].Equals("Occupied"))
                {
                    modifiedRow["States"] = "Empty";
                    GetDataAdapter().Update(seatsStatesTable);
                    return true;
                }
            }
            return false;
        }
    }
}
