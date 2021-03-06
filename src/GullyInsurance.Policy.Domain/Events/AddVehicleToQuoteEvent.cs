﻿using System;
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
            : base(DateTime.Now)
        {
            Policy = policy;
            this.PolicyId = Policy.PolicyId;
            VehicleToAdd = vehicle;
        }

        internal override void Process()
        {
            Console.WriteLine("Adding Vehicle to Policy");
            Policy.HandleAddVehicleToQuoteEvent(this);
        }
        internal override void Reverse()
        {
            throw new NotImplementedException();
        }
    }
}
