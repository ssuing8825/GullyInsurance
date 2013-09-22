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
            i.ProcessEventUsingNewStream(d);

            var vehicleEvent = new InsureVehicleEvent() { Make = "Honda", VIN = "asdfadsf", PolicyId = q.PolicyId };
            i.ProcessEventUsingExistingStream(vehicleEvent);


            //d.Name = "Joe";
            //i.HandleBindEvent(d);
        }

        [TestMethod]
        public void CreateAQuoteAddVEhicleThenGetOutAgain()
        {
            var i = new AmendService();

            //This will be the PolicyId and the Stream is based on this Id
            var newEventStreamGuid = Guid.NewGuid();
            var q = new BeginNewPolicyQuoteEvent(newEventStreamGuid);
            i.ProcessEventUsingNewStream(q);
            var vehicleId = Guid.NewGuid();
            var av = new AddVehicleToQuoteEvent(q.Policy, new Vehicle() { VehicleId = vehicleId, Vin = "213" });
            i.ProcessEventUsingExistingStream(av);
            i.ProcessEventUsingExistingStream(new BindEvent(av.Policy));


            //At this point we have a new quote with one vehicle.
            //There is no policy, just a quote and it is very early on in the life of the policy

            //Let's get it our and take a look at it.
            var newP = i.GetCurrentPolicy(newEventStreamGuid);
            Console.WriteLine(newP.Vehicles.Count);
            i.CreateSnapShot(newP, 2);

            newP = i.GetPolicy(newEventStreamGuid, 1);
            Console.WriteLine(newP.Vehicles.Count);

        }
    }
}
