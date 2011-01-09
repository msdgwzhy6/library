using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NUnit.Framework;
using System.Data.SqlClient;
using System.Data;

namespace LibrarySeatsManager
{
    [TestFixture]
    public class SeatStateManagerTest
    {
        [Test]
        public void TestQuerySeatsStates()
        {
            SeatStateManager target = new SeatStateManager();
            DataTable actual;
            actual = target.QuerySeatsStates();
            Assert.IsNotNull(actual);
        }

        [Test]
        public void TestUpdateSeatsStates()
        {
            SeatStateManager target = new SeatStateManager();
            int row = 1;
            int column = 1;
            Boolean actual = true;
            Boolean expected;
            target.ClearOccupiedSeat(row, column);
            target.UpdateSeatsStates(row, column, "Occupied");
            expected = target.ClearOccupiedSeat(row, column);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestClearOccupiedSeat()
        {
            SeatStateManager target = new SeatStateManager();
            int row = 1;
            int column = 1;
            bool expected = true;
            bool actual;
            target.UpdateSeatsStates(row, column, "Occupied");
            actual = target.ClearOccupiedSeat(row, column);
            Assert.AreEqual(expected, actual);
        }
    }
}