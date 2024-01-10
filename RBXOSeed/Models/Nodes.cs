using RBXOSeed.Data;

namespace RBXOSeed.Models
{
    public class Nodes : AuditFields
    {
        public string NodeIP { get; set; }
        public DateTime LastPolled { get; set; }
        public long CallOuts { get; set; }
        public bool IsPortOpen { get; set; }
        public bool IsValidator { get; set; }
        public bool IsActive { get; set; }
        public int FailureCount { get; set; }
        public DateTime LastActiveDate { get; set; }

        public static LiteDB.ILiteCollection<Nodes>? GetAll()
        {
            try
            {
                var nodes = DbContext.DB.GetCollection<Nodes>(DbContext.RSRV_NODES);
                return nodes;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string SaveNode(Nodes node)
        {
            var nodes = GetAll();
            if (nodes == null)
            {
            }
            else
            {
                var nodeRec = nodes.FindOne(x => x.NodeIP == node.NodeIP);
                if (nodeRec != null)
                {
                    nodes.UpdateSafe(node);
                }
                else
                {
                    nodes.InsertSafe(node);
                }
            }

            return "Error Saving Nodes";
        }

        public static void DeleteNode(string nodeIP)
        {
            var nodes = GetAll();
            if (nodes != null)
            {
                nodes.DeleteManySafe(x => x.NodeIP == nodeIP);
            }
        }
    }
}
