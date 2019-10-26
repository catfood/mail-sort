using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using MailSort.Rules;
using Moq;

namespace MailSortTest
{
    [TestClass]
    public class RulesControllerTests
    {
        private MailSortAPI.Controllers.RulesController controller;
        private Mock<MailSortBL.IRulesData> rulesMock;

        [TestInitialize]
        public void Startup()
        {
            var expectedRulesDTO = new MailDTO.RulesetDTO
            {
                RulesetData = "Ruleset Data"
            };
            rulesMock = new Mock<MailSortBL.IRulesData>();
            rulesMock.Setup(foo => foo.Load("rules.db")).Returns(expectedRulesDTO);
            controller = new MailSortAPI.Controllers.RulesController(rulesMock.Object);
        }

        [TestMethod]
        public void RulesControllerIsOK()
        {
            var getResult = controller.Get();
            Assert.IsInstanceOfType(getResult,typeof(MailDTO.RulesetDTO));
        }

        [TestMethod]
        public void RulesControllerCallsBL()
        {
            var x = controller.Get();
            rulesMock.Verify(foo => foo.Load("rules.db"), Times.AtLeastOnce());
        }
    }
}
