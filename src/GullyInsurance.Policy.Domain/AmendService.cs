using System;
using System.Transactions;
using GullyInsurance.Policy.Domain.Events;
using GullyInsurance.Policy.Domain.Model;
using NEventStore;
using NEventStore.Dispatcher;
using NEventStore.Persistence.SqlPersistence.SqlDialects;
using NEventStore.Serialization;


namespace GullyInsurance.Policy.Domain
{
    //http://neventstore.org/
    public class AmendService
    {
        private static IStoreEvents store;

        private static readonly byte[] EncryptionKey = new byte[]
            {
                0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0xa, 0xb, 0xc, 0xd, 0xe, 0xf
            };

        public void HandleBindEvent(BindEvent policy)
        {
            using (var scope = new TransactionScope())
            using (store = WireupEventStore())
            {
                OpenOrCreateStream(policy);
                //AppendToStream(policy);
                //TakeSnapshot();
                //LoadFromSnapshotForwardAndAppend();
                scope.Complete();
            }
        }
        public void HandleInsuredVehicleEvent(InsureVehicleEvent domainEvent)
        {
            using (var scope = new TransactionScope())
            using (store = WireupEventStore())
            {
                AppendToStream(domainEvent);
                scope.Complete();
            }
        }
        private void AppendToStream(DomainEvent domainEvent)
        {
            using (IEventStream stream = store.OpenStream(domainEvent.PolicyId, int.MinValue, int.MaxValue))
            {
                stream.Add(new EventMessage { Body = domainEvent });
                stream.CommitChanges(Guid.NewGuid());
            }
        }
        private static void OpenOrCreateStream(BindEvent policy)
        {
            // we can call CreateStream(StreamId) if we know there isn't going to be any data.
            // or we can call OpenStream(StreamId, 0, int.MaxValue) to read all commits,
            // if no commits exist then it creates a new stream for us.
            using (IEventStream stream = store.OpenStream(policy.Policy.PolicyId, 0, int.MaxValue))
            {
                stream.Add(new EventMessage { Body = policy });
                stream.CommitChanges(Guid.NewGuid());
            }
        }
        private IStoreEvents WireupEventStore()
        {
            var t = new DocumentObjectSerializer();

            return Wireup.Init()
                   .LogToOutputWindow()
                   .UsingMongoPersistence(GetConnectionString, t)
                   .EnlistInAmbientTransaction()
                   .HookIntoPipelineUsing(new[] { new AuthorizationPipelineHook() })
                   .UsingSynchronousDispatchScheduler()
                       .DispatchTo(new DelegateMessageDispatcher(DispatchCommit))
                   .Build();
        }

        public string GetConnectionString()
        {

            return "mongodb://localhost/EventSource?safe=true";


        }

        private static void DispatchCommit(Commit commit)
        {
            // This is where we'd hook into our messaging infrastructure, such as NServiceBus,
            // MassTransit, WCF, or some other communications infrastructure.
            // This can be a class as well--just implement IDispatchCommits.
            try
            {
                foreach (EventMessage @event in commit.Events)
                {
                    Console.WriteLine("Message dispatched " + ((DomainEvent)@event.Body).PolicyId);
                    ((DomainEvent)@event.Body).Process();

                }
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to dispatch");
            }
        }
    }


}
