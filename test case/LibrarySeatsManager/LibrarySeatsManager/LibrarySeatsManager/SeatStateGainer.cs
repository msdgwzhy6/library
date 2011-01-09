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
using library2;

namespace LibrarySeatsManager
{
    public class SeatStateGainer
    {
        private Match stateGainer;
        private SeatStateManager stateManager;

        private int GetSeatState(int row, int column)
        {
            stateGainer = new Match();
            if (stateGainer.FindImage(row, column))
            {
                return stateGainer.MatchFeatures();
            }
            return -1;
        }

        public Boolean UpdateSeatState(int row, int column)
        {
            stateManager = new SeatStateManager();
            int stateInt = GetSeatState(row, column);
            if (stateInt == -1)
                 return false;
            String stateString = StateTransferFromIntToString(stateInt);
            return stateManager.UpdateSeatsStates(row, column, stateString);
        }

        private String StateTransferFromIntToString(int state)
        {
            if (state == 1)
            {
                return "Empty";
            }
            else
            {
                return "Using";
            }
        }
    }
}
