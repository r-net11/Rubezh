using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
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

		/// <summary>
		/// Определяет принадлежность двух ip-адресов к одной подсети
		/// </summary>
		/// <param name="ip1">ip-адрес 1</param>
		/// <param name="mask1">маска для ip-адрес 1</param>
		/// <param name="ip2">ip-адрес 2</param>
		/// <param name="mask2">маска для ip-адрес 2</param>
		/// <returns>true, если ip-адреса принадлежат одной подсети,
		/// false - в противном случае</returns>
		public static bool IsSubnetEqual(string ip1, string mask1, string ip2, string mask2)
		{
			var ip1Address = IPAddress.Parse(ip1).Address;
			var mask1Address = IPAddress.Parse(mask1).Address;
			ip1Address &= mask1Address;

			var ip2Address = IPAddress.Parse(ip2).Address;
			var mask2Address = IPAddress.Parse(mask2).Address;
			ip2Address &= mask2Address;

			return ip1Address == ip2Address;
		}

		/// <summary>
		/// Возвращает список информации по ip4-адресам для всех сетевых адаптеров
		/// </summary>
		/// <returns>Информация по ip4-адресам для всех сетевых адаптеров</returns>
		public static List<UnicastIPAddressInformation> GetIp4NetworkInterfacesInfo()
		{
			var result = new List<UnicastIPAddressInformation>();

			var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces().Where(x => x.NetworkInterfaceType == NetworkInterfaceType.Ethernet).ToList();
			foreach (var networkInterface in networkInterfaces)
			{
				var props = networkInterface.GetIPProperties().UnicastAddresses.Where(x => x.Address.AddressFamily == AddressFamily.InterNetwork).ToList();
				result.AddRange(props);
			}
			return result;
		}
	}
}
