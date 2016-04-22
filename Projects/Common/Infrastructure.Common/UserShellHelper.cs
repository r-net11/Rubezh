using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using Common;
using FiresecAPI.Enums;
using Microsoft.Win32;

namespace Infrastructure.Common
{
	public class UserShellHelper
	{
		#region <Константы>

		private const string MonitorShell = "StrazhMonitor.exe";
		private const string LayoutsShell = "StrazhMonitor.Layout.exe";
		private const string DefaultShell = "explorer.exe";

		private const string ShellRegistryKey = @"Software\Microsoft\Windows NT\CurrentVersion\Winlogon";
		private const string ShellRegistryName = "Shell";

		private const string PoliciesSystemRegistryKey = @"Software\Microsoft\Windows\CurrentVersion\Policies\System";
		private const string DisableTaskMgrRegistryName = "DisableTaskMgr";

		#endregion </Константы>

		#region <Управление системой>

		private static bool SetOrDeleteKey(bool enable, string key, string name, object value, string errorMessage = null)
		{
			try
			{
				var regKey = Registry.CurrentUser.CreateSubKey(key);
				if (regKey == null)
				{
					Logger.Info(string.Format("Ключ реестра '{0}' не существует.", key));
					return false;
				}
				if (enable)
				{
					Logger.Info(string.Format("Для ветки рееста '{0}' параметру '{1}' устанавливаем значение '{2}'", key, name, value));
					regKey.SetValue(name, value);
				}
				else
				{
					Logger.Info(string.Format("Для ветки рееста '{0}' удаляем параметр '{1}'", key, name));
					regKey.DeleteValue(name);
				}
				regKey.Close();
				return true;
			}
			catch (Exception e)
			{
				throw new Exception(errorMessage ?? string.Format("Ошибка при настройке реестра '{0}'", key), e);
			}
			return false;
		}

		public static ShellType GetShell()
		{
			Logger.Info("Запущена процедура определения типа рабочего стола для текущего пользователя Windows");
			var key = Registry.CurrentUser.OpenSubKey(ShellRegistryKey);
			if (key == null)
			{
				throw new Exception(string.Format("Ошибка при обращении к ключу реестра '{0}'", ShellRegistryKey));
			}
			var value = key.GetValue(ShellRegistryName);
			if (value == null)
			{
				Logger.Info(string.Format("Тип оболочки рабочего стола для текущего пользователя Windows установлен в '{0}'", ShellType.Default));
				return ShellType.Default;
			}
			switch (Path.GetFileName(value.ToString()))
			{
				case MonitorShell:
					Logger.Info(string.Format("Тип оболочки рабочего стола для текущего пользователя Windows установлен в '{0}'", ShellType.Monitor));
					return ShellType.Monitor;
				case LayoutsShell:
					Logger.Info(string.Format("Тип оболочки рабочего стола для текущего пользователя Windows установлен в '{0}'", ShellType.Layouts));
					return ShellType.Layouts;
				default:
					Logger.Info(string.Format("Тип оболочки рабочего стола для текущего пользователя Windows установлен в '{0}'", ShellType.Default));
					return ShellType.Default;
			}
		}

		public static bool SetShell(ShellType shellType)
		{
			if (GetShell() == shellType)
			{
				Logger.Info(string.Format("Текущий тип оболочки рабочего стола соответствует требуемому '{0}'. Не меняем его", shellType));
				return true;
			}

			var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var shellName = string.Empty;
			switch (shellType)
			{
				case ShellType.Monitor:
					shellName = MonitorShell;
					break;
				case ShellType.Layouts:
					shellName = LayoutsShell;
					break;
			}
			var cmd = shellType == ShellType.Default 
				? string.Empty
				: string.Format(@"{0}\{1}", assemblyPath, shellName);
			return SetOrDeleteKey(shellType != ShellType.Default, ShellRegistryKey, ShellRegistryName, cmd, "Ошибка при установке типа оболочки рабочего стола");
		}

		public static void DisableTaskManager(bool disable)
		{
			Logger.Info(string.Format("Запретить для данной учетной записи Windows доступ к диспетчеру задач? {0}", disable));
			SetOrDeleteKey(disable, PoliciesSystemRegistryKey, DisableTaskMgrRegistryName, "1", "Ошибка при отключении диспетчера задач");
		}

		#endregion </Управление системой>

		#region <Работа с пользователями и правами>

		public static bool IsUserAdministrator()
		{
			bool isAdmin;
			try
			{
				var user = WindowsIdentity.GetCurrent();
				if (user == null)
				{
					return false;
				}
				var principal = new WindowsPrincipal(user);
				isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
			}
			catch
			{
				isAdmin = false;
			}
			return isAdmin;
		}

		#endregion </Работа с пользователями и правами>

		#region Вспомогательные

		public static void Reboot()
		{
			Process.Start("shutdown", "/r /f /t 0");
		}

		public static void Shutdown()
		{
			Process.Start("shutdown", "/s /f /t 0");
		}

		public static void Logoff()
		{
			Process.Start("shutdown", "/l");
		}

		#endregion
	}
}