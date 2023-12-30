using System.Net.Sockets;

namespace RBXOSeed.Utility
{
    public static class IPUtility
    {
        public static string GetIPAddress(HttpContext httpContext)
        {
            var Request = httpContext.Request;

            var cfConnect = Request.Headers["CF-CONNECTING-IP"];

            if (cfConnect.ToString().Contains(":"))
            {

            }

            if (cfConnect != "")
                return cfConnect.ToString();

            var ipAddress = httpContext.Connection.RemoteIpAddress.ToString();

            if (!string.IsNullOrEmpty(ipAddress)) return ipAddress;

            return "IP Not Found";
        }

        public static bool IsPortOpen(string host, int port)
        {
            try
            {
                using (var client = new TcpClient())
                {
                    client.Connect(host, port);
                    return true;
                }
            }
            catch (SocketException)
            {
                return false;
            }
        }
    }
}
