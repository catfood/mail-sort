using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;

namespace MailSortTest
{
    [TestClass]
    public class RulesControllerTests
    {
        [TestMethod]
        public void RulesControllerIsOK()
        {
            var controller = new MailSortAPI.Controllers.RulesController();
            Assert.AreEqual(typeof(OkResult), controller.Get().Result.GetType());
        }
    }
}
