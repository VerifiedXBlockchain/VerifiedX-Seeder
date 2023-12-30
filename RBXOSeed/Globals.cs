using RBXOSeed.Models;
using System.Collections.Concurrent;

namespace RBXOSeed
{
    public class Globals
    {
        public static CommandArguments CommandArguments = new CommandArguments();
        public static string? SeedName { get; set; }
        public static int PortToCheck = 3338;
        public static ConcurrentQueue<(string, bool)> NodeQueue = new ConcurrentQueue<(string, bool)>();
        public static ConcurrentBag<string> NodeBag = new ConcurrentBag<string>();
    }
}
