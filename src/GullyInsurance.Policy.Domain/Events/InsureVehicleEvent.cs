using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GullyInsurance.Policy.Domain.Events
{
    public class InsureVehicleEvent : DomainEvent
    {
        public string Make { get; set; }
        public string VIN { get; set; }

        public override void Process()
        {
           Console.WriteLine("Insuring a Vehicle");
        }
    }
}
