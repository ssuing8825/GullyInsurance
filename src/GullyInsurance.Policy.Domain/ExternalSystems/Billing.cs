using System;
using GullyInsurance.Policy.Domain.Model;

namespace GullyInsurance.Policy.Domain.ExternalSystems
{
    public interface IBilling
    {
        AmendService Service { get; set; }
        void Notify(AutoPolicy policy);
    }

    public class Billing : IBilling
    {
        public AmendService Service { get; set; }

        public void Notify(AutoPolicy policy)
        {
            if (Service.IsActive)
                 Console.WriteLine("Sending Billing Message");
        }
    }
}
