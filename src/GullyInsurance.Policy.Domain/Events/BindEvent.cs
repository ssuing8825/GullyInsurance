using System;
using GullyInsurance.Policy.Domain.Model;

namespace GullyInsurance.Policy.Domain.Events
{
    public class BindEvent : DomainEvent, IContainPolicy
    {
        public string Name { get; set; }
        public DateTime EffectiveDate { get; set; }
        public AutoPolicy Policy { get; set; }
        public BindEvent(AutoPolicy policy) : base(DateTime.Now)
        {
            this.PolicyId = policy.PolicyId;
            Policy = policy;
        }

        public override void Process()
        {
            Policy.HandleBind(this);
        }
    }
}
