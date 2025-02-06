using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Rowles.PIA.Managed.Enums;

namespace Rowles.PIA.Managed
{
    public class ManagedPIA
    {
        // location of piactl.exe
        string m_VPNEXEPATH;
        public string VPNExecutablePath => m_VPNEXEPATH;
        public bool CanFindVPNExecutable => File.Exists(m_VPNEXEPATH); // Checks if we can find the VPN file

        public ManagedPIA(string vpnexe = "C:\\Program Files\\Private Internet Access\\piactl.exe")
        {
            if (string.IsNullOrEmpty(vpnexe))
                throw new FileNotFoundException("piactl.exe file not found.");

            m_VPNEXEPATH = vpnexe.ToLower();
        }

        // Queries the PIA Service (piactl.exe)
        async Task<string> QueryPIAAsync(string command)
        {
            if (!CanFindVPNExecutable)
            {
                throw new FileNotFoundException("piactl.exe file not found.");
            }

            Process process = new Process();
            process.StartInfo.FileName = m_VPNEXEPATH;
            process.StartInfo.Arguments = command;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            await process.WaitForExitAsync();
            string std_out = await process.StandardOutput.ReadToEndAsync();
            return std_out;
        }

        #region VPN Connection

        public async Task Connect() => await QueryPIAAsync("connect");
        public async Task Disconnect() => await QueryPIAAsync("disconnect");

        public async Task<ConnectionState> GetConnectionStateAsync()
        {
            var proc_result = await QueryPIAAsync("get connectionstate");
            try
            {
                return (ConnectionState)Enum.Parse(typeof(ConnectionState), proc_result);
            }
            catch { return ConnectionState.Unknown; }
        }

        public async Task<string> GetPublicIPAddress()
        {
            return await QueryPIAAsync("get pubip");
        }

        public async Task<string> GetVPNIPAddress()
        {
            return await QueryPIAAsync("get vpnip");
        }
        #endregion

        #region Authentication

        public async Task<bool> AttemptLoginAsync(string credentials_file = "./credentials.txt")
        {
            if (!File.Exists(credentials_file))
                throw new FileNotFoundException($"{credentials_file} missing");

            try
            {
                var result = await QueryPIAAsync("login");
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> AttemptLogoutAsync()
        {
            try
            {
                await QueryPIAAsync("logout");
                return true;
            }
            catch { return false; }
        }

        #endregion

        #region VPN Region
        public async Task<string[]> GetRegionsAsync()
        {
            var proc_result = await QueryPIAAsync("get regions");
            return proc_result.Split('\n');
        }
        public async Task<int> GetRegionsCountAsync()
        {
            var proc_result = await GetRegionsAsync();
            return proc_result.Length;
        }

        public async Task<string> GetCurrentRegionAsync()
        {
            return await QueryPIAAsync("get region");
        }

        public async Task<bool> SetRegion(string region = "auto")
        {
            try
            {
                await QueryPIAAsync($"set region {region}");
                return true;
            }
            catch { return false; }
        }

        #endregion
    }
}
