using Microsoft.Win32;

namespace Infrastructure.Common.RegistryHelper
{
	public class RegistryHelper
	{
		public static void Integrate()
		{
			//ОЗ вместо explorer.exe
			RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon", true);
			key.SetValue("Shell", "C:\\Program Files\\Rubezh\\FireMonitor\\FireMonitor.exe");

			//Выключает WinKey
			key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Keyboard Layout", true);
			key.SetValue("Scancode Map", "hex:00,00,00,00,00,00,00,00,03,00,00,00,00,00,5B,E0,00,00,5C,E0,00,00,00,00");

			//Отключить Usb
			key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\USBSTOR", true);
			key.SetValue("Type", "dword:00000001");
			key.SetValue("Start", "dword:00000004");
			key.SetValue("ErrorControl", "dword:00000001");
			key.SetValue("ImagePath", "hex(2):73,00,79,00,73,00,74,00,65,00,6d,00,33,00,32,00,5c,00,44,00,\\52,00,49,00,56,00,45,00,52,00,53,00,5c,00,55,00,53,00,42,00,53,00,54,00,4f,\\00,52,00,2e,00,53,00,59,00,53,00,00,00");
			key.SetValue("DisplayName", "Драйвер запоминающих устройств для USB");
			key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\USBSTOR\\Security", true);
			key.SetValue("Security", "hex:01,00,14,80,90,00,00,00,9c,00,00,00,14,00,00,00,30,00,00,00,02,\\00,1c,00,01,00,00,00,02,80,14,00,ff,01,0f,00,01,01,00,00,00,00,00,01,00,00,\\00,00,02,00,60,00,04,00,00,00,00,00,14,00,fd,01,02,00,01,01,00,00,00,00,00,\\05,12,00,00,00,00,00,18,00,ff,01,0f,00,01,02,00,00,00,00,00,05,20,00,00,00,\\20,02,00,00,00,00,14,00,8d,01,02,00,01,01,00,00,00,00,00,05,0b,00,00,00,00,\\00,18,00,fd,01,02,00,01,02,00,00,00,00,00,05,20,00,00,00,23,02,00,00,01,01,\\00,00,00,00,00,05,12,00,00,00,01,01,00,00,00,00,00,05,12,00,00,00");
			key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\USBSTOR\\Enum", true);
			key.SetValue("Count", "dword:00000000");
			key.SetValue("NextInstance", "dword:00000000");

			//Не тестил
			//Выключить Cdrom
			key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\Cdrom", true);
			key.SetValue("Start", "dword:00000004");

			//Не тестил
			//Выключить Cdrom
			key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\Flpydisk", true);
			key.SetValue("Start", "dword:00000004");
		}

		public static void Desintegrate()
		{
			//Возвращает explorer.exe
			RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon", true);
			key.SetValue("Shell", "explorer.exe");

			//Включает TaskManager
			key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", true);
			key.SetValue("DisableTaskMgr", "dword:00000000");

			//Включает WinKey
			key = Registry.LocalMachine.OpenSubKey("CurrentControlSet\\Control\\Keyboard Layout", true);
			key.SetValue("Scancode Map", "-");

			//Включить Usb
			key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\USBSTOR", true);
			key.SetValue("Type", "dword:00000001");
			key.SetValue("Start", "dword:00000003");
			key.SetValue("ErrorControl", "dword:00000001");
			key.SetValue("ImagePath", "hex(2):73,00,79,00,73,00,74,00,65,00,6d,00,33,00,32,00,5c,00,44,00,\\52,00,49,00,56,00,45,00,52,00,53,00,5c,00,55,00,53,00,42,00,53,00,54,00,4f,\\00,52,00,2e,00,53,00,59,00,53,00,00,00");
			key.SetValue("DisplayName", "Драйвер запоминающих устройств для USB");
			key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\USBSTOR\\Security", true);
			key.SetValue("Security", "ex:01,00,14,80,90,00,00,00,9c,00,00,00,14,00,00,00,30,00,00,00,02,\\00,1c,00,01,00,00,00,02,80,14,00,ff,01,0f,00,01,01,00,00,00,00,00,01,00,00,\\00,00,02,00,60,00,04,00,00,00,00,00,14,00,fd,01,02,00,01,01,00,00,00,00,00,\\05,12,00,00,00,00,00,18,00,ff,01,0f,00,01,02,00,00,00,00,00,05,20,00,00,00,\\20,02,00,00,00,00,14,00,8d,01,02,00,01,01,00,00,00,00,00,05,0b,00,00,00,00,\\00,18,00,fd,01,02,00,01,02,00,00,00,00,00,05,20,00,00,00,23,02,00,00,01,01,\\00,00,00,00,00,05,12,00,00,00,01,01,00,00,00,00,00,05,12,00,00,00");
			key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\USBSTOR\\Enum", true);
			key.SetValue("Count", "dword:00000001");
			key.SetValue("NextInstance", "dword:00000001");
			key.SetValue("0", "USB\\Vid_1005&Pid_b113\\0D91018070E3595A");

			//Не тестил
			//Включить Cdrom
			key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\Cdrom", true);
			key.SetValue("Start", "dword:00000001");

			//Не тестил
			//Включить Cdrom
			key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\Flpydisk", true);
			key.SetValue("Start", "dword:00000001");
		}
	}
}