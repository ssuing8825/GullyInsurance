using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
        private static IEventStream stream;

        private static readonly byte[] EncryptionKey = new byte[]
            {
                0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0xa, 0xb, 0xc, 0xd, 0xe, 0xf
            };



        public void ProcessEventUsingNewStream(DomainEvent domainEvent)
        {
            using (var scope = new TransactionScope())
            using (store = WireupEventStore())
            {
                OpenOrCreateStream(domainEvent);
                scope.Complete();
            }
        }
        public void ProcessEventUsingExistingStream(DomainEvent domainEvent)
        {
            using (var scope = new TransactionScope())
            using (store = WireupEventStore())
            {
                AppendToStream(domainEvent);
                scope.Complete();
            }
        }

        public AutoPolicy GetCurrentPolicy(Guid policyId)
        {
            AutoPolicy policy = null;
            using (var scope = new TransactionScope())
            using (store = WireupEventStore())
            {
                using (stream = store.OpenStream(policyId, int.MinValue, int.MaxValue))
                {
                    foreach (EventMessage @event in stream.CommittedEvents)
                    {
                        ((DomainEvent)@event.Body).Process();
                        if (@event.Body is IContainPolicy)
                            policy = ((IContainPolicy)@event.Body).Policy;
                    }
                }
            }
            return policy;
        }

        public AutoPolicy GetPolicy(Guid policyId, int maxRevisionNumber)
        {
            AutoPolicy policy = null;
            using (var scope = new TransactionScope())
            using (store = WireupEventStore())
            {
                using (stream = store.OpenStream(policyId, int.MinValue, maxRevisionNumber))
                {
                    foreach (EventMessage @event in stream.CommittedEvents)
                    {
                        ((DomainEvent)@event.Body).Process();
                        if (@event.Body is IContainPolicy)
                            policy = ((IContainPolicy)@event.Body).Policy;
                    }
                }
            }
            return policy;
        }

        private void AppendToStream(DomainEvent domainEvent)
        {
            using (stream = store.OpenStream(domainEvent.PolicyId, int.MinValue, int.MaxValue))
            {
                stream.Add(new EventMessage { Body = domainEvent });
                stream.CommitChanges(Guid.NewGuid());
            }
        }
        private static void OpenOrCreateStream(DomainEvent policy)
        {
            // we can call CreateStream(StreamId) if we know there isn't going to be any data.
            // or we can call OpenStream(StreamId, 0, int.MaxValue) to read all commits,
            // if no commits exist then it creates a new stream for us.
            using (stream = store.OpenStream(policy.PolicyId, 0, int.MaxValue))
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

        private void DispatchCommit(Commit commit)
        {
            // This is where we'd hook into our messaging infrastructure, such as NServiceBus,
            // MassTransit, WCF, or some other communications infrastructure.
            // This can be a class as well--just implement IDispatchCommits.
            try
            {
                foreach (EventMessage @event in commit.Events)
                {
                    var ev = (DomainEvent)@event.Body;

                    try
                    {
                        if (ev is ReplacementEvent)
                        {
                            ProcessReplacement((ReplacementEvent)ev);
                        }

                        else if (OutOfOrder(ev))
                            ProcessOutOfOrder(ev);
                        else
                        {
                            BasicProcessEvent(ev);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;

                        //ev.ProcessingError = ex;
                        //if (ShouldRethrowExceptions) throw ex;
                    }


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to dispatch {0}", ex);
            }
        }

        private static void ProcessReplacement(ReplacementEvent replacementEvent)
        {
            throw new NotImplementedException();
        }

        private bool OutOfOrder(DomainEvent e)
        {
            if (LogIsEmpty()) return false;
            return (LastEvent().After(e));

        }
        private DomainEvent LastEvent()
        {
            if (LogIsEmpty()) return null;
            return (DomainEvent)stream.CommittedEvents.Last().Body;
        }
        private bool LogIsEmpty()
        {
            return 0 == stream.CommittedEvents.Count;
        }

        private void ProcessOutOfOrder(DomainEvent e)
        {
            RewindTo(e);
            BasicProcessEvent(e);
            ReplayAfter(e);
        }

        private void ReplayAfter(DomainEvent e)
        {
            throw new NotImplementedException();
        }

  
        private void RewindTo(DomainEvent priorEvent)
        {
            IList<> consequences = Consequences(priorEvent);
            for (int i = consequences.Count - 1; i >= 0; i--)
                BasicReverseEvent(((DomainEvent)consequences[i]));
        }
        private List<DomainEvent> Consequences(DomainEvent baseEvent)
        {
            var result = new List<DomainEvent>();

            foreach (var candidate in stream.CommittedEvents)
            {
                if (((DomainEvent)candidate.Body).IsConsequenceOf(baseEvent)) 
                    result.Add((DomainEvent)candidate.Body);
            }
            return result;
        }

        private void BasicProcessEvent(DomainEvent e)
        {
            Console.WriteLine("Message dispatched " + e.PolicyId);
            e.Process();
        }
        private void BasicReverseEvent(DomainEvent e)
        {
            Console.WriteLine("Message Reversed " + e.PolicyId);
            e.Reverse();
        }

        public void CreateSnapShot(AutoPolicy policy, int streamRevision)
        {
            using (var scope = new TransactionScope())
            using (store = WireupEventStore())
            {
                store.Advanced.AddSnapshot(new Snapshot(policy.PolicyId, streamRevision, policy));
                scope.Complete();
            }
        }
    }
}


