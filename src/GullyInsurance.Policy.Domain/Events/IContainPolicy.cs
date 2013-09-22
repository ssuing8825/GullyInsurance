using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GullyInsurance.Policy.Domain.Model;

namespace GullyInsurance.Policy.Domain.Events
{
   public interface IContainPolicy
    {
        AutoPolicy Policy { get; set; }
    }
}
