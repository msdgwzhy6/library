using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NUnit.Framework;
using library2;

namespace LibrarySeatsManager
{
    [TestFixture]
    public class SiftFeatTest
    {
        [Test]
         public void FindImageTest()
        {
            SiftFeat target = new SiftFeat(); 
            int row = 0; 
            int col = 0; 
            bool expected = true; 
            bool actual;
            actual = target.FindImage(row, col);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SiftFeaturesTest()
        {
            SiftFeat target = new SiftFeat(); 
            target.SiftFeatures();
        }
    }
}