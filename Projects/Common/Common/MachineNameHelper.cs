using System.Net;

namespace Common
{
	public static class MachineNameHelper
	{
		public static string GetMachineName()
		{
			string domainName = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
			string hostName = Dns.GetHostName();
			if (!hostName.Contains(domainName))
				return hostName + "." + domainName;
			else
				return hostName;
		}
	}
}