using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RBXOSeed.Models;
using RBXOSeed.Utility;
using RBXOSeed.Data;
using System.Xml.Linq;

namespace RBXOSeed.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class V1Controller : ControllerBase
    {
        #region Private Variables
        public class JsonResult
        {
            public bool Success { get; set; }

            public string Reason { get; set; }
        }

        #endregion

        #region Get - Test if Seed node is online
        /// <summary>
        /// Check Status of API
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> Get([FromQuery]string? walletVersion, bool? isVal)
        {
            if (!HttpContext.Response.Headers.ContainsKey("Access-Control-Allow-Origin"))
            {
                HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            }
            var ip = IPUtility.GetIPAddress(HttpContext);
            if (ip == "")
            {
                ip = "localhost";
            }
            else
            {
                if(!Globals.NodeBag.Contains(ip))
                {
                    Globals.NodeQueue.Enqueue((ip, isVal.Value));
                    Globals.NodeBag.Add(ip);
                }
                    
            }
            if(walletVersion != null)
                return JsonConvert.SerializeObject(new { Name = Globals.SeedName != null ? Globals.SeedName : "RBX One Seed Node", Status = "Online", IP = ip}, Formatting.Indented);

            return $"[\"ReserveBlock Seed Node Status\",\"Online\",\"{ip}\"]";
        }

        #endregion

        #region Gets peers to send out
        [HttpGet("GetNodes")]
        public IEnumerable<string> GetNodes()
        {
            if (!HttpContext.Response.Headers.ContainsKey("Access-Control-Allow-Origin"))
            {
                HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            }
            var ip = IPUtility.GetIPAddress(HttpContext);

            var nodes = Nodes.GetAll()?.Query().Where(x => x.NodeIP != ip && x.IsActive == true).ToList();

            if (nodes.Count() == 0)
            {
                return new string[] { "No Nodes" };
            }
            if (nodes.Count() <= 20) //users can connect up to 14 outbound connections
            {
                var nodeList = nodes.Select(x => x.NodeIP);
                return nodeList;
            }
            else  //users can connect up to 14 outbound connections this will get 14 random
            {
                var rnd = new Random();
                var nodeList = nodes.Where(x => x.IsActive == true).Select(x => x.NodeIP).Take(50);
                return nodeList;
            }
        }

        #endregion

        #region Call to Node
        [HttpGet("GetCallToNode")]
        public async Task<bool> GetCallToNode()
        {
            bool output = true;
            if (!HttpContext.Response.Headers.ContainsKey("Access-Control-Allow-Origin"))
            {
                HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            }

            var ip = IPUtility.GetIPAddress(HttpContext);

            var nodeExist = Nodes.GetAll()?.Query().Where(x => x.NodeIP == ip).FirstOrDefault();
            if(nodeExist == null)
            {
                Nodes node = new Nodes {
                    Active = true,
                    CallOuts = 0,
                    CreateDateTimeUtc = DateTime.UtcNow,
                    FailureCount = 0,
                    IsActive = false,
                    IsPortOpen = true,
                    IsValidator= false,
                    LastActiveDate= DateTime.UtcNow,
                    LastPolled = DateTime.UtcNow,
                    ModifiedDateTimeUtc= DateTime.UtcNow,
                    NodeIP= ip,
                };
            }

            return output;
        }
        #endregion
    }
}
