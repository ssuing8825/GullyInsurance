using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GullyInsurance.Policy.Domain.Events
{
    public class CancelEvent
    {
        public Guid PolicyId { get; set; }
        public string Name { get; set; }

    }
}
