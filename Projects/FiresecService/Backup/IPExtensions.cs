using System.Net;

namespace Common
{
	/// <summary>
	/// Extension класс, который содержит методы для работы с классом System.Net.IPAddress
	/// <see cref="http://stackoverflow.com/questions/13630984/compare-ip-address-if-it-is-lower-than-the-other-one"/>
	/// </summary>
	public static class IPExtensions
	{
		/// <summary>
		/// Конвертирует экземпляр класса IPAddress в Integer32.
		/// </summary>
		/// <param name="ip">Экземпляр класса IPAddress.</param>
		/// <returns>Возвращает соответствующее значение Integer32.</returns>
		private static int ToInteger(this IPAddress ip)
		{
			var bytes = ip.GetAddressBytes();
			var result = bytes[0] << 24 | bytes[1] << 16 | bytes[2] << 8 | bytes[3];

			return result;
		}

		//returns 0 if equal
		//returns 1 if ip1 > ip2
		//returns -1 if ip1 < ip2
		/// <summary>
		/// Класс, реализующий сравнение двух экземпляров класса IPAddress
		/// </summary>
		/// <param name="IP1">Экземпляр класса IPAddress, который сравнивается</param>
		/// <param name="IP2">Экземпляр класса IPAddress, с которым производится сравнение</param>
		/// <returns>
		/// Возвращает 0, если одинаковые
		/// Возвращает 1, если IP1 > IP2
		/// Возвращает -1, если IP1 < IP2
		/// </returns>
		public static int Compare(this IPAddress IP1, IPAddress IP2)
		{
			var ip1 = IP1.ToInteger();
			var ip2 = IP2.ToInteger();
			return (((ip1 - ip2) >> 0x1F) | (int)((uint)(-(ip1 - ip2)) >> 0x1F));
		}
	}
}
