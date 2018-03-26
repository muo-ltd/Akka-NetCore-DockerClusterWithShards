using System;
using Akka.Actor;
using Akka.Event;

namespace Client
{
    public partial class ClientActor :  ReceiveActor
    {
        public class DummyRequest {}
        public class DummyResponse {} 

        private readonly ILoggingAdapter _log = Context.GetLogger();
        private long _totalChars;
        private int _totalMessages;
        private string _remoteAddress;

        private IActorRef _origonalActor; 
        public ClientActor(string remoteAddress)
        {
            _remoteAddress = remoteAddress;

            Receive<DummyRequest>(x => Handle(x));
            Receive<Server.TestEntity.StreamedDataResponse>(x => Handle(x));
            Receive<Server.TestEntity.StreamedDataComplete>(x => Handle(x));
        }

        public void Handle(DummyRequest req)
        {
            _origonalActor = Sender;

            var remoteActor = Context.ActorSelection($"akka.tcp://testsystem@{_remoteAddress}:4053/user/testactor");

            var message = new Server.TestEntity.StreamedDataRequest(1000, 100000);

            remoteActor.Tell(message);

            _log.Info("Handled DummyRequest");
        }

        public void Handle(Server.TestEntity.StreamedDataResponse res)
        {
            _totalChars += res.SomeData.Length;
            _totalMessages++;
            _log.Info($"Received {res.SomeData.Length} chars from {_totalMessages} messages");
        }

        public void Handle(Server.TestEntity.StreamedDataComplete res)
        {
            _log.Info($"Written {_totalChars}");
            _origonalActor.Tell(new DummyResponse());
        }

        public static Props Props(string remoteAddress)
        {
            return Akka.Actor.Props.Create(() => new ClientActor(remoteAddress));
        }

    }
}