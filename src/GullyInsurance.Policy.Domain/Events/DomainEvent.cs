using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace GullyInsurance.Policy.Domain.Events
{
    [BsonKnownTypes(typeof(InsureVehicleEvent), typeof(BindEvent))]
    public class DomainEvent
    {
        public DateTime Recorded { get; set; }
        public DateTime Occured { get; set; }
        public Guid PolicyId { get; set; }

        protected DomainEvent()
        {
        }

        protected DomainEvent(DateTime recorded)
        {
            Recorded = recorded;
            Occured = DateTime.Now;
        }

        public virtual void Process() { }
    }
}
