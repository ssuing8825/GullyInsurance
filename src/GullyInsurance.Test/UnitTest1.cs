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
        public void CreateAQuoteAddVehicleThenGetOutAgain()
        {
            //The AmendService is an event processor. This might be a generic componenet.
            var i = new AmendService();

            //This will be the PolicyId and the Stream is based on this Id
            var newEventStreamGuid = Guid.NewGuid();

            //The first step in the life of a policy is to be new. It should write the send billing message
            var q = new BeginNewPolicyQuoteEvent(newEventStreamGuid);
            i.ProcessEventUsingNewStream(q);

            //Now we are going to add a vehicle using that event since a policy with no vehicle makes no sense.
            var vehicleId = Guid.NewGuid();
            var av = new AddVehicleToQuoteEvent(q.Policy, new Vehicle() { VehicleId = vehicleId, Vin = "213" });
            i.ProcessEventUsingExistingStream(av);

            //Send the bind event since we are ok to move forward.
            i.ProcessEventUsingExistingStream(new BindEvent(av.Policy));

            Console.WriteLine("Finished adding new events");
            //At this point we have a new quote with one vehicle.
            //There is no policy, just a quote and it is very early on in the life of the policy

            //Let's get it our and take a look at it.
            var newP = i.GetCurrentPolicy(newEventStreamGuid);
            Assert.AreEqual(1, newP.Vehicles.Count);
            i.CreateSnapShot(newP, 2);
            Console.WriteLine("Pulled out events and saw policy");

            newP = i.GetPolicy(newEventStreamGuid, 1);
            Assert.AreEqual(0, newP.Vehicles.Count);

            //Now let's insert a resp

        }
    }
}
