using System.Net;

namespace Common
{
    public static class MachineNameHelper
    {
        public static string GetMachineName()
        {
            string domainName = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
            string hostName = Dns.GetHostName();
            string machineName = "";
            if (!hostName.Contains(domainName))
                machineName = hostName + "." + domainName;
            else
                machineName = hostName;

            return machineName;
        }
    }
}
