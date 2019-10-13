using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using MailSort.Rules;

namespace MailSortTest
{
    [TestClass]
    public class RulesControllerTests
    {
        private MailSortAPI.Controllers.RulesController controller;

        [TestInitialize]
        public void Startup()
        {
            var bl = new MailSortBL.RulesData();
            controller = new MailSortAPI.Controllers.RulesController(bl);
        }

        [TestMethod]
        public void RulesControllerIsOK()
        {
            Assert.IsInstanceOfType(controller.Get(),typeof(MailDTO.RulesetDTO));
        }

        [TestMethod]
        public void RulesControllerCallsBL()
        {
            var x = controller.Get();
        }
    }
}
