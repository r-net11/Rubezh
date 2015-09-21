using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Common;

namespace Infrastructure.Common
{
	public static class NetworkHelper
	{
		public const string Localhost = "localhost";
		public const string LocalhostIp = "127.0.0.1";

		public static bool IsLocalAddress(string address)
		{
			return String.Equals(address, Localhost, StringComparison.InvariantCultureIgnoreCase) ||
			       String.Equals(address, LocalhostIp, StringComparison.InvariantCultureIgnoreCase);
		}

		/// <summary>
		/// Возвращает список IP-адресов (IPv4) хоста
		/// </summary>
		/// <returns>Список IP-адресов (IPv4) хоста</returns>
		public static List<string> GetHostIpAddresses()
		{
			try
			{
				var hostName = Dns.GetHostName();
				var ipEntry = Dns.GetHostEntry(hostName);
				return ipEntry.AddressList.Where(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
					.Select(x => x.ToString())
					.ToList();
			}
			catch (Exception e)
			{
				Logger.Error(e, "NetworkHelper.GetHostIpAddresses");
				return new List<string>();
			}
		}
	}
}
