using System;
using System.Diagnostics;
using System.Text;

namespace Common
{
	public static class SystemInfo
	{
		public static string GetString()
		{
			StringBuilder sb = new StringBuilder(string.Empty);
			sb.AppendLine("System information");
			try
			{
				var process = Process.GetCurrentProcess();
				sb.AppendFormat("Process [{0}]:\t{1} x{2}\n", process.Id, process.ProcessName, GetBitCount(Environment.Is64BitProcess));
				sb.AppendFormat("Operation System:\t{0} {1} Bit Operating System\n", Environment.OSVersion, GetBitCount(Environment.Is64BitOperatingSystem));
				sb.AppendFormat("ComputerName:\t\t{0}\n", Environment.MachineName);
				sb.AppendFormat("UserDomainName:\t\t{0}\n", Environment.UserDomainName);
				sb.AppendFormat("UserName:\t\t{0}\n", Environment.UserName);
				sb.AppendFormat("Base Directory:\t\t{0}\n", AppDomain.CurrentDomain.BaseDirectory);
				sb.AppendFormat("SystemDirectory:\t{0}\n", Environment.SystemDirectory);
				sb.AppendFormat("ProcessorCount:\t\t{0}\n", Environment.ProcessorCount);
				sb.AppendFormat("SystemPageSize:\t\t{0}\n", Environment.SystemPageSize);
				sb.AppendFormat(".Net Framework:\t\t{0}", Environment.Version);
			}
			catch (Exception ex)
			{
				sb.Append(ex.ToString());
			}
			return sb.ToString();
		}
		private static int GetBitCount(bool is64)
		{
			return is64 ? 64 : 86;
		}
	}
}