using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GullyInsurance.Policy.Domain.Model;

namespace GullyInsurance.Policy.Domain.Events
{
    public class BeginNewPolicyQuoteEvent : DomainEvent, IContainPolicy
    {
        public AutoPolicy Policy { get; set; }

        public BeginNewPolicyQuoteEvent(Guid policyId)
        {
            PolicyId = policyId;
            Policy = new AutoPolicy();
         }

        public override void Process()
        {
            Policy.HandleBeginNewPolicyQuoteEvent(this);
        }
    }
}
