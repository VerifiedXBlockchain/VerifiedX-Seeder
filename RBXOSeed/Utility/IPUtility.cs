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
            using (TcpClient tcpClient = new TcpClient())
            {
                try
                {
                    var result = tcpClient.BeginConnect(host, port, null, null);
                    var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1));
                    if (!success)
                    {
                        throw new SocketException();
                    }
                    return true;
                }
                catch (SocketException)
                {
                    return false;
                }
            }
        }
    }
}
