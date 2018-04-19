using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Loader;
using System.Threading;
using Akka.Actor;
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
                                actor {
                                    provider = remote
                                }

                                remote {
                                    log-remote-lifecycle-events = DEBUG
                                    dot-netty.tcp {
                                        port = 4053
                                        hostname = {hostIP}
                                        enable-pooling = false
                                    }
                                }
                            }";

            hcon = hcon.Replace("{hostIP}", hostIp);

            Console.WriteLine($"Seed Address:{hostIp}");

            _clusterSystem = ActorSystem.Create("testsystem", ConfigurationFactory.ParseString(hcon));

            _clusterSystem.ActorOf(TestEntity.Props(), "testactor");

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
