using System;
using GullyInsurance.Policy.Domain;
using GullyInsurance.Policy.Domain.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GullyInsurance.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var i = new AmendService();
            i.RunAmend(new InsureVehicle(){PolicyId = Guid.NewGuid()});

        }
    }
}
