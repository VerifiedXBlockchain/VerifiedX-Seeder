using RBXOSeed.Data;
using RBXOSeed.Models;
using RBXOSeed.Utility;
using System.Net.Sockets;

namespace RBXOSeed.Services
{
    public class NodeProcessor
    {
        static SemaphoreSlim NodeQueueProcessorLock = new SemaphoreSlim(1, 1);

        static SemaphoreSlim NodePortProcessorLock = new SemaphoreSlim(1, 1);
        public static async Task PopulateNodeBag()
        {
            var nodes = Nodes.GetAll()?
                .FindAll()
                .Select(x => x.NodeIP)
                .ToList();

            if(nodes?.Count() > 0 )
            {
                foreach(var node in nodes) 
                {
                    Globals.NodeBag.Add(node);
                }
            }

            
        }
        public static async Task StartNodeQueue()
        {
            while(true)
            {
                var delay = Task.Delay(10000); // runs every 10 seconds

                await NodeQueueProcessorLock.WaitAsync();

                try
                {
                    await CheckNodeQueue();
                }
                catch (Exception ex)
                {

                }
                finally { NodeQueueProcessorLock.Release(); }

                await delay;
            }
        }

        public static async Task StartNodePortChecks()
        {
            while (true)
            {
                var delay = Task.Delay(new TimeSpan(1,0,0)); // runs every 1 hour.

                await NodePortProcessorLock.WaitAsync();

                try
                {
                    await CheckNodePorts();
                }
                catch (Exception ex)
                {

                }
                finally { NodePortProcessorLock.Release(); }

                await delay;
            }
        }

        private static async Task CheckNodeQueue()
        {
            var nodes = Nodes.GetAll();
            while (Globals.NodeQueue.Count > 0)
            {
                try
                {
                    var dequeued = Globals.NodeQueue.TryDequeue(out var nodeInfo);
                    if (dequeued)
                    {
                        (string ip, bool isVal) = nodeInfo;
                        if (ip != "localhost")
                        {
                            var nodeRec = nodes?.Query().Where(x => x.NodeIP == ip).FirstOrDefault();

                            if (nodeRec == null)
                            {
                                Nodes node = new Nodes
                                {
                                    CallOuts = 0,
                                    IsActive = false,
                                    IsPortOpen = true,
                                    LastActiveDate = DateTime.UtcNow,
                                    LastPolled = DateTime.UtcNow,
                                    IsValidator = isVal,
                                    NodeIP = ip,
                                    FailureCount = 0,
                                };

                                nodes.InsertSafe(node);
                            }
                        }

                    }
                }
                catch { }
            }
        }

        private static async Task CheckNodePorts()
        {
            var nodes = Nodes.GetAll()?
                .Query()
                .Where(x => x.IsActive && x.IsPortOpen && x.FailureCount < 4 && x.LastPolled.AddHours(1) < DateTime.UtcNow)
                .ToList();

            if (nodes?.Count() > 0)
            {
                var coreCount = Environment.ProcessorCount;
                Parallel.ForEach(nodes, new ParallelOptions { MaxDegreeOfParallelism = coreCount == 4 ? 2 : 4 }, (node, loopState) => {
                    var result = IPUtility.IsPortOpen(node.NodeIP, Globals.PortToCheck);
                    if (result)
                    {
                        node.LastActiveDate = DateTime.UtcNow;
                        node.LastPolled = DateTime.UtcNow;
                        node.FailureCount = 0;
                        node.IsActive = true;
                        node.IsPortOpen = true;
                    }
                    else
                    {
                        node.LastPolled = DateTime.UtcNow;
                        node.FailureCount += 1;
                        //if nodes gets over 5 fails it is moved to inactive state
                        if (node.FailureCount > 3)
                        {
                            node.IsActive = false;
                            node.IsPortOpen = false;
                        }
                    }

                    Nodes.GetAll()?.UpdateSafe(node);
                });
            }
        }
    }
}
