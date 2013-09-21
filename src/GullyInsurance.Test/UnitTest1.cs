using System;
using GullyInsurance.Policy.Domain;
using GullyInsurance.Policy.Domain.Events;
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
            var q = new AutoPolicy { PolicyId = Guid.NewGuid() };
            var d = new BindEvent(q);
            i.HandleBindEvent(d);

            var vehicleEvent = new InsureVehicleEvent() { Make = "Honda", VIN = "asdfadsf", PolicyId = q.PolicyId };
            i.HandleInsuredVehicleEvent(vehicleEvent);


            //d.Name = "Joe";
            //i.HandleBindEvent(d);
        }
    }
}
