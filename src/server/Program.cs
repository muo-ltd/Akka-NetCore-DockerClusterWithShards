using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Loader;
using System.Threading;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Akka.Configuration;

namespace Server
{
    class Program
    {
        public static ActorSystem _clusterSystem;
        static void Main(string[] args)
        {
            var hostname = Dns.GetHostName();
            Console.WriteLine($"Hostname: {hostname}");

            var hostIp = GetHostIPAddress(hostname);
            Console.WriteLine($"HostIP: {hostIp}");

            var hcon = @"akka {
                                debug.unhandled = on
                                stdout-loglevel = DEBUG
                                loglevel = DEBUG
                                
                                actor {
                                    provider = ""Akka.Cluster.ClusterActorRefProvider, Akka.Cluster"" 

                                    serializers {
                                        hyperion = ""Akka.Serialization.HyperionSerializer, Akka.Serialization.Hyperion""
                                    }
        
                                    serialization-bindings {
                                        ""System.Object"" = hyperion
                                    }
                                }

                                remote {
                                    log-remote-lifecycle-events = DEBUG
                                    dot-netty.tcp {
                                        port = 4053
                                        hostname = {hostIP}
                                    }
                                }

                                cluster {
                                    allow-weakly-up-members = off
                                    seed-nodes = [""{clusterAddresses}""]
                                    roles = [server,sharding]

                                    sharding {
                                        role = sharding
                                        remember-entities = false
                                    }
                                }
                            }";

            hcon = hcon.Replace("{hostIP}", hostIp);
            hcon = hcon.Replace("{clusterAddresses}", $"akka.tcp://testsystem@{hostIp}:4053");

            Console.WriteLine($"Seed Address:{hostIp}");

            _clusterSystem = ActorSystem.Create("testsystem", ConfigurationFactory.ParseString(hcon));

            var clusterSharding = ClusterSharding.Get(_clusterSystem);

            clusterSharding.Start(
                typeName: "testregion",
                entityProps: Props.Create<TestEntity>(),
                settings: ClusterShardingSettings.Create(_clusterSystem),
                messageExtractor: new MessageExtractor());

            AssemblyLoadContext.Default.Unloading += (obj) => 
            {
                LeaveCluster();
            };

            Console.CancelKeyPress += (s, ev) => 
            {
                LeaveCluster();
            };

            new AutoResetEvent(false).WaitOne();
        }

        public static string GetHostIPAddress(string hostName)
        {
            string hostIP = string.Empty;
            var ipAddressEntries = Dns.GetHostAddresses(hostName);
            hostIP = ipAddressEntries.Where(x => x.AddressFamily == AddressFamily.InterNetwork).First().ToString();
            return hostIP;
        }

        private static void LeaveCluster()
        {
            Console.WriteLine("Leaving cluster");
            CoordinatedShutdown.Get(_clusterSystem).Run().Wait(TimeSpan.FromSeconds(60));
        }
    }
}
