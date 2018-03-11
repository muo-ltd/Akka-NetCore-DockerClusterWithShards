using Akka.Cluster.Sharding;

namespace Server
{
    public sealed class MessageExtractor : IMessageExtractor
    {
        public string EntityId(object message)
        {
            return (message as Envelope)?.EntityId.ToString();
        }

        public string ShardId(object message)
        {
            return (message as Envelope)?.ShardId.ToString();
        }

        public object EntityMessage(object message)
        {
           return (message as Envelope)?.Message;
        }
    }

    public sealed class Envelope
    {
        private const int _NumberOfShards = 100;
        
        public Envelope(int entityId, object message)
        {
            ShardId = entityId % _NumberOfShards;
            EntityId = entityId;
            Message = message;
        }

        public int ShardId { get;}
        public int EntityId { get; }
        public object Message { get; }
    }
}