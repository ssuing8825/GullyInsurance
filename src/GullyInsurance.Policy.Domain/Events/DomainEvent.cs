using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace GullyInsurance.Policy.Domain.Events
{
    [BsonKnownTypes(typeof(InsureVehicleEvent), typeof(BindEvent), typeof(AddVehicleToQuoteEvent), typeof(BeginNewPolicyQuoteEvent))]
    public abstract class DomainEvent
    {
        public DateTime Recorded { get; set; }
        public DateTime Occurred { get; set; }
        public Guid PolicyId { get; set; }

        public bool ShouldIgnoreOnReplay
        {
            get { return false; }
        }

        protected DomainEvent()
        {
        }
        public bool IsConsequenceOf(DomainEvent other)
        {
            return (!ShouldIgnoreOnReplay && this.After(other));
        }
        internal bool After(DomainEvent ev)
        {
            return this.Occurred < ev.Occurred;
        }



        protected DomainEvent(DateTime recorded)
        {
            Recorded = recorded;
            Occurred = DateTime.Now;
        }

        internal abstract void Process();
        internal abstract void Reverse();
    }
}
