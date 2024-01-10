using Newtonsoft.Json;
using RBXOSeed.Models;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text.Json.Nodes;

namespace RBXOSeed.Services
{
    public class StartupServices
    {
        public static async Task RunArgs()
        {
            if(Globals.CommandArguments.SeedNodes)
                await SeedNodeData();

            if (Globals.CommandArguments.UseSelfSignedCertificate)
                GetSelfSignedCertificate();
        }

        public static async Task SetGlobals(WebApplication? app)
        {
            if (app == null)
                return;
            try
            {
                string? myValue = app.Configuration.GetSection("GlobalSettings")["SeedName"];
                if(myValue != null)
                    Globals.SeedName = myValue;

                int? portToCheck = app.Configuration.GetValue<int>("GlobalSettings:PortToCheck");
                if (portToCheck != null)
                    Globals.PortToCheck = portToCheck.Value;
            }
            catch (Exception ex)
            {

            }
            
        }

        private static async Task SeedNodeData()
        {
            try
            {
                var nodeCheck = Nodes.GetAll().Query().Exists();
                if (!nodeCheck)
                {
                    var nodesData = await File.ReadAllTextAsync(@"..\RBXOSeed\NodeSeedData\nodes.txt");
                    var nodesArray = nodesData.Split(',');
                    
                    foreach (var node in nodesArray)
                    {
                        Nodes nNode = new Nodes
                        {
                            CallOuts = 0,
                            IsActive = true,
                            IsPortOpen = true,
                            LastActiveDate = DateTime.UtcNow,
                            LastPolled = DateTime.UtcNow,
                            IsValidator = false,
                            NodeIP = node.Replace(" ", ""),
                            FailureCount = 0,

                        };
                        Nodes.SaveNode(nNode);
                    }
                }
            }
            catch { }
        }

        private static X509Certificate2 GetSelfSignedCertificate()
        {
            var password = Guid.NewGuid().ToString();
            var commonName = "RBXSelfSignedCertAPI";
            var rsaKeySize = 2048;
            var years = 100;
            var hashAlgorithm = HashAlgorithmName.SHA256;

            using (var rsa = RSA.Create(rsaKeySize))
            {
                var request = new CertificateRequest($"cn={commonName}", rsa, hashAlgorithm, RSASignaturePadding.Pkcs1);

                request.CertificateExtensions.Add(
                  new X509KeyUsageExtension(X509KeyUsageFlags.DataEncipherment | X509KeyUsageFlags.KeyEncipherment | X509KeyUsageFlags.DigitalSignature, false)
                );
                request.CertificateExtensions.Add(
                  new X509EnhancedKeyUsageExtension(
                    new OidCollection { new Oid("1.3.6.1.5.5.7.3.1") }, false)
                );

                var certificate = request.CreateSelfSigned(DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddYears(years));
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    certificate.FriendlyName = commonName;

                // Return the PFX exported version that contains the key
                return new X509Certificate2(certificate.Export(X509ContentType.Pfx, password), password, X509KeyStorageFlags.MachineKeySet);
            }
        }
    }
}
