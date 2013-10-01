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

        public BeginNewPolicyQuoteEvent(Guid policyId):base(DateTime.Now)
        {
            PolicyId = policyId;
            Policy = new AutoPolicy();
         }

        internal override void Process()
        {
            Policy.HandleBeginNewPolicyQuoteEvent(this);
        }
        internal override void Reverse()
        {
            throw new NotImplementedException();
        }
    }
}
