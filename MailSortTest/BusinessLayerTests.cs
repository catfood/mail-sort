using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MailSortTest
{
    [TestClass]
    public class BusinessLayerTests
    {
        [TestMethod]
        public void TestCreateBusinessLayer()
        {
            var bl = new MailSortBL.RulesData();
            var rules = bl.Load("rules.db");
            bl.Save("delete-rules.db", rules);
        }
    }
}
