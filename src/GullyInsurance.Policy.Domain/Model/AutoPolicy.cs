using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GullyInsurance.Policy.Domain.Events;
using GullyInsurance.Policy.Domain.ExternalSystems;

namespace GullyInsurance.Policy.Domain.Model
{
    public class AutoPolicy
    {
        private IBilling billing;

        public Guid PolicyId { get; set; }
        public List<Vehicle> Vehicles { get; set; }

        public PolicyStatus Status { get; set; }
        public DateTime? EffectiveDate { get; set; }

        public AutoPolicy()
        {
            Vehicles = new List<Vehicle>();
        }
        internal void HandleBind(BindEvent bind)
        {
            Console.WriteLine("The Policy is bound");
            //Set internal State. 
            Status = PolicyStatus.Active;
            EffectiveDate = bind.EffectiveDate;
        }

        public void HandleBeginNewPolicyQuoteEvent(BeginNewPolicyQuoteEvent ev)
        {
            Status = PolicyStatus.NewQuote;
            PolicyId = ev.PolicyId;
            Console.WriteLine("The Policy is New");
            //Call the billing process. The Domain logic doesn't care about the context of the running of events: that's the gateways problem.
            //This could be the event or the domain object or anything
            //Actually if this was in a SAGA we just might not set the command to the gateway.
            Registry.BillingGateway().Notify(this);

        }

        internal void HandleAddVehicleToQuoteEvent(AddVehicleToQuoteEvent ev)
        {

            this.Vehicles.Add(ev.VehicleToAdd);
            ev.VehicleToAdd.HandleAddToQuoteEvent(ev);
        }
    }
    public class Vehicle
    {
        public Guid VehicleId { get; set; }
        public string Vin { get; set; }
        public string PerformanceCode { get; set; }

        internal void HandleAddToQuoteEvent(AddVehicleToQuoteEvent ev)
        {
            Console.WriteLine("HandleAddToQuoteEvent");
            
            //Set the performanceCode this is something consumers shouldn't need to konw.
            PerformanceCode = "asdfadsf";
        }
    }

    public enum PolicyStatus
    {
        NewQuote,
        Active,

    }
}
