using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GullyInsurance.Policy.Domain.Model;

namespace GullyInsurance.Policy.Domain.Events
{
    public class AddVehicleToQuoteEvent : DomainEvent, IContainPolicy
    {
        public AutoPolicy Policy { get; set; }
        public Vehicle VehicleToAdd { get; set; }

        public AddVehicleToQuoteEvent(AutoPolicy policy, Vehicle vehicle)
        {
            Policy = policy;
            this.PolicyId = Policy.PolicyId;
            VehicleToAdd = vehicle;
        }

        public override void Process()
        {
            Policy.HandleAddVehicleToQuoteEvent(this);
        }
    }
}
