using System.Text;
using Akka.Actor;
using Akka.Event;

namespace Server 
{
    public partial class TestEntity :  ReceiveActor
    {
        private readonly ILoggingAdapter _log = Context.GetLogger();
        public class StreamedDataRequest 
        {
            public StreamedDataRequest(int noChars, int noInstances)
            {
                NoChars = noChars;
                NoInstances = noInstances;
            }
            public int NoChars { get; }
            public int NoInstances { get; }
        } 

        public class StreamedDataResponse 
        {
            public string SomeData { get; set; }
        }

        public class StreamedDataComplete
        {
        }

        public TestEntity()
        {
            _log.Info("TestEntity Created");
            Receive<StreamedDataRequest>(x => Handle(x));
        }

        private void Handle(StreamedDataRequest req)
        {
            _log.Info($"Handling StreamedDataRequest");
            
            StringBuilder builder = new StringBuilder(req.NoChars);
            for(int i = 0; i < req.NoChars; i++)
            {
                builder.Append("1");
            }

            for(int i = 0; i < req.NoInstances; i++)
            {
                Sender.Tell(new StreamedDataResponse() { SomeData = builder.ToString()});
            }

            Sender.Tell(new StreamedDataComplete());

            _log.Info($"Sent {req.NoInstances} with {req.NoChars} in each");
        }
    }
}