using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GullyInsurance.Policy.Domain.Events;

namespace GullyInsurance.Policy.Domain.Model
{
    public class AutoPolicy
    {
        public Guid PolicyId { get; set; }
        
        internal void HandleBind(BindEvent bind)
        {
            Console.WriteLine("Tthe Policy is bound");
        }
    }
}
