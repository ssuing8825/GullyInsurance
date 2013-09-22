using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GullyInsurance.Policy.Domain.Events;

namespace GullyInsurance.Policy.Domain.Model
{
    public class AutoPolicy
    {
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
            Console.WriteLine("Tthe Policy is bound");
            //Set internal State. 
            Status = PolicyStatus.Active;
            EffectiveDate = bind.EffectiveDate;
        }

        public void HandleBeginNewPolicyQuoteEvent(BeginNewPolicyQuoteEvent ev)
        {
            Status = PolicyStatus.NewQuote;
            PolicyId = ev.PolicyId;
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
