using System;
using GullyInsurance.Policy.Domain.Model;

namespace GullyInsurance.Policy.Domain.Events
{
    public class BindEvent : DomainEvent
    {
        public string Name { get; set; }
        internal AutoPolicy Policy { get; set; }

        public BindEvent(AutoPolicy policy) : base(DateTime.Now)
        {
            Policy = policy;
            this.PolicyId = policy.PolicyId;
        }

        public override void Process()
        {
            Policy.HandleBind(this);
        }
    }
}
