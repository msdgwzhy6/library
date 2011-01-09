using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NUnit.Framework;
using library2;

namespace LibrarySeatsManager
{
    public class MatchTest
    {
        public void FindImageTest()
        {
            Match target = new Match(); 
            int row = 0; 
            int col = 0; 
            bool expected = true; 
            bool actual;
            actual = target.FindImage(row, col);
            Assert.AreEqual(expected, actual);
        }

        public void MatchFeaturesTest()
        {
            Match target = new Match();
            int expected = 1;
            int actual;
            actual = target.MatchFeatures();
            Assert.AreEqual(expected, actual);
        }
    }
}