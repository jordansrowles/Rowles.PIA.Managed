using Rowles.PIA.Managed;

namespace Rowles.PIA.Managed.Tests.Repl
{
    internal class Program
    {

        static async Task Main(string[] args)
        {
            ManagedPIA vpnService = new("C:\\Program Files\\Private Internet Access\\piactl.exe");
            await vpnService.Connect();
            Thread.Sleep(1000);

            Console.WriteLine($"FOUND VPN FILE: \t{vpnService.CanFindVPNExecutable} ({vpnService.VPNExecutablePath})");
            Console.WriteLine($"CONNECTION STATE: \t{await vpnService.GetConnectionStateAsync()}");
            Console.WriteLine($"CURRENT REGION: \t{await vpnService.GetCurrentRegionAsync()}");
            Console.WriteLine($"AVL. REGIONS: \t{await vpnService.GetRegionsCountAsync()}");
            Console.WriteLine($"PUBLIC IP: \t{await vpnService.GetPublicIPAddress()}");
            Console.WriteLine($"VPN IP: \t{await vpnService.GetVPNIPAddress()}");

            await vpnService.Disconnect();


            //await vpnService.SetRegion("china");
            //Console.WriteLine($"CURRENT REGION: \t{await vpnService.GetCurrentRegionAsync()}");



            Console.ReadLine();
        }
    }
}
