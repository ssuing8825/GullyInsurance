using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GullyInsurance.Policy.Domain.ExternalSystems
{
   public class Registry
   {
       private static Billing billing;

       public Registry(AmendService service)
       {
           billing = new Billing();
           billing.Service = service;
       }

       public static Billing BillingGateway()
       {
           return billing;
       }
    }
}
