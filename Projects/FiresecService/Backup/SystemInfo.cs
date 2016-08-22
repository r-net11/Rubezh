using System;
using System.Diagnostics;
using System.Text;

namespace Common
{
	/// <summary>
	/// Представляет информацию по параметрам системного окружения для текущего процесса
	/// </summary>
	public static class SystemInfo
	{
		/// <summary>
		/// Получает параметры системного окружения для текущего процесса
		/// </summary>
		/// <returns>Строка, содержащая значения параметов системного окружения для текущего процесса</returns>
		public static string GetString()
		{
			var stringBuilder = new StringBuilder(string.Empty);
			stringBuilder.AppendLine("System information");
			try
			{
				var process = Process.GetCurrentProcess();
				stringBuilder.AppendFormat("Process [{0}]:\t{1} x{2}\n", process.Id, process.ProcessName, GetBitCount(Environment.Is64BitProcess));
				stringBuilder.AppendFormat("Operation System:\t{0} {1} Bit Operating System\n", Environment.OSVersion, GetBitCount(Environment.Is64BitOperatingSystem));
				stringBuilder.AppendFormat("ComputerName:\t\t{0}\n", Environment.MachineName);
				stringBuilder.AppendFormat("UserDomainName:\t\t{0}\n", Environment.UserDomainName);
				stringBuilder.AppendFormat("UserName:\t\t{0}\n", Environment.UserName);
				stringBuilder.AppendFormat("Base Directory:\t\t{0}\n", AppDomain.CurrentDomain.BaseDirectory);
				stringBuilder.AppendFormat("SystemDirectory:\t{0}\n", Environment.SystemDirectory);
				stringBuilder.AppendFormat("ProcessorCount:\t\t{0}\n", Environment.ProcessorCount);
				stringBuilder.AppendFormat("SystemPageSize:\t\t{0}\n", Environment.SystemPageSize);
				stringBuilder.AppendFormat(".Net Framework:\t\t{0}", Environment.Version);
			}
			catch (Exception ex)
			{
				stringBuilder.Append(ex.ToString());
			}
			return stringBuilder.ToString();
		}

		/// <summary>
		/// Определяет разрядность операционной системы
		/// </summary>
		/// <param name="is64">Текущая операционная система 64-х разрядная?</param>
		/// <returns>Целое число, представляющее разрядность операционной системы</returns>
		private static int GetBitCount(bool is64)
		{
			return is64 ? 64 : 86;
		}
	}
}