using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GullyInsurance.Policy.Domain.Events
{
    public class RenewEvent
    {
        public Guid PolicyId { get; set; }
        public DateTime RenewDate { get; set; }
    }
}
