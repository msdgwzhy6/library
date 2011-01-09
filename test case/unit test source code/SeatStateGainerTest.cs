using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NUnit.Framework;

namespace LibrarySeatsManager
{
    [TestFixture]
    public class SeatStateGainerTest
    {
        [Test]
        public void TestUpdateSeatStateWithExistSeat()
        {
            SeatStateGainer target = new SeatStateGainer();
            int row = 0;
            int column = 0;
            bool expected = true;
            bool actual;
            actual = target.UpdateSeatState(row, column);
            Assert.AreEqual(expected, actual);
        }
    }
}