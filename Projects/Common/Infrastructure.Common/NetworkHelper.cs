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
		/// <summary>
		/// Возвращает список IP-адресов (IPv4) хоста, включая localhost
		/// </summary>
		/// <returns>Список IP-адресов (IPv4) хоста, включая localhost</returns>
		public static List<string> GetHostIpAddresses()
		{
			try
			{
				var hostName = Dns.GetHostName();
				var ipEntry = Dns.GetHostEntry(hostName);
				var ipAdresses = ipEntry.AddressList.Where(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
					.Select(x => x.ToString())
					.ToList();
				ipAdresses.Insert(0, "localhost");
				return ipAdresses;
			}
			catch (Exception e)
			{
				Logger.Error(e, "NetworkHelper.GetHostIpAddresses");
				return new List<string>() {"localhost"};
			}
		}
	}
}
