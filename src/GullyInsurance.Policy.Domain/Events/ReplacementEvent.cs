using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GullyInsurance.Policy.Domain.Events
{
    public class ReplacementEvent : DomainEvent
    {
        private DomainEvent original;
        private DomainEvent replacement;
        public DomainEvent Original { get { return original; } }
        public DomainEvent Replacement { get { return replacement; } }

        public ReplacementEvent(DomainEvent oldEvent, DomainEvent replacement)
            : base(oldEvent.Occurred)
        {
            this.original = oldEvent;
            this.replacement = replacement;
        }

        internal override void Process()
        {
            throw new Exception("Replacements should not be processed directly");
        }
        internal override void Reverse()
        {
            throw new Exception("Cannot reverse replacements");
        }
    }




}
