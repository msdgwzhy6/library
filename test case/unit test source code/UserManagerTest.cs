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
    public class UserManagerTest
    {

        [Test]
        public void TestValidateUserWithExistUser()
        {
            UserManager target = new UserManager();
            string account = "08386130";
            string password = "08386130";
            bool expected = true;
            bool actual;
            actual = target.ValidateUser(account, password);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestValidateUserWithNotExistUser()
        {
            UserManager target = new UserManager();
            string account = "00000000";
            string password = "08386130";
            bool expected = false;
            bool actual;
            actual = target.ValidateUser(account, password);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestValidateUserWithErrorPassword()
        {
            UserManager target = new UserManager();
            string account = "08386130";
            string password = "00000000";
            bool expected = false;
            bool actual;
            actual = target.ValidateUser(account, password);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestAddUserWithExistAccount()
        {
            UserManager target = new UserManager();
            string account = "08386129";
            string password = "08386129";
            string role = "Student";
            bool expected = false;
            bool actual;
            actual = target.AddUser(account, password, role);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestRemoveUserWithNotExistAccount()
        {
            UserManager target = new UserManager();
            string account = "abcdefgh";
            bool expected = false;
            bool actual;
            actual = target.RemoveUser(account);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestIsAuthorizeWithAdministratorAccount()
        {
            UserManager target = new UserManager();
            string account = "08386130";
            string password = "08386130";
            bool expected = true;
            bool actual;
            actual = target.IsAuthorize(account, password);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestIsAuthorizeWithNotStudentAccount()
        {
            UserManager target = new UserManager();
            string account = "08386128";
            string password = "08386128";
            bool expected = false;
            bool actual;
            actual = target.IsAuthorize(account, password);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestGetAllUsersWithExistUsers()
        {
            UserManager target = new UserManager();
            DataTable expected = null;
            DataTable actual;
            actual = target.GetAllUsers();
            Assert.AreNotEqual(expected, actual);
        }
    }
}