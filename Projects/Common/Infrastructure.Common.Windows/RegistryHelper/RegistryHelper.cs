using System;
using Common;
using Microsoft.Win32;

namespace Infrastructure.Common.RegistryHelper
{
	public class RegistryHelper
	{
		public static void Integrate()
		{
			RegistryKey key;

			//Включает TaskManager
			try
			{
				key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", true);
				key.SetValue("DisableTaskMgr", 1, RegistryValueKind.DWord);
			}
			catch (Exception e)
			{
				Logger.Error(e, "RegistryHelper.Integrate 1");
			}

			//Выключает WinKey
			try
			{
				key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Keyboard Layout", true);
				key.SetValue("Scancode Map",
					new byte[] { 00, 00, 00, 00, 00, 00, 00, 00, 03, 00, 00, 00, 00, 00, 0x5b, 0xe0, 00, 00, 0x5c, 0xe0, 00, 00, 00, 00 },
					RegistryValueKind.Binary);
			}
			catch (Exception e)
			{
				Logger.Error(e, "RegistryHelper.Integrate 2");
			}

			//Выключить Cdrom
			try
			{
				key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\Cdrom", true);
				key.SetValue("Start", "dword:00000004");
			}
			catch (Exception e)
			{
				Logger.Error(e, "RegistryHelper.Integrate 3");
			}

			//Выключить floppy
			try
			{
				key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\Flpydisk", true);
				key.SetValue("Start", "dword:00000004");
			}
			catch (Exception e)
			{
				Logger.Error(e, "RegistryHelper.Integrate 4");
			}

			return;
			//Отключить Usb
			try
			{
				key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\USBSTOR", true);
				key.SetValue("Type", 1, RegistryValueKind.DWord);
				key.SetValue("Start", 4, RegistryValueKind.DWord);
				key.SetValue("ErrorControl", 1, RegistryValueKind.DWord);
				key.SetValue("ImagePath",
					new byte[] { 0x73, 00, 0x79, 00, 0x73, 00, 0x74, 00, 0x65, 00, 0x6d, 00, 0x33, 00, 0x32, 00, 0x5c, 00, 0x44, 00, 0x52, 00, 0x49, 00, 0x56, 00, 0x45, 00, 0x52, 00, 0x53, 00, 0x5c, 00, 0x55, 00, 0x53, 00, 0x42, 00, 0x53, 00, 0x54, 00, 0x4f, 00, 0x52, 00, 0x2e, 00, 0x53, 00, 0x59, 00, 0x53, 00, 00, 00 },
					RegistryValueKind.Binary);
				key.SetValue("DisplayName", "Драйвер запоминающих устройств для USB");
				key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\USBSTOR\\Security", true);
				key.SetValue("Security",
					new byte[] { 0x01, 00, 0x14, 0x80, 0x90, 00, 00, 00, 0x9c, 00, 00, 00, 0x14, 00, 00, 00, 0x30, 00, 00, 00, 0x02, 00, 0x1c, 00, 0x01, 00, 00, 00, 0x02, 0x80, 0x14, 00, 0xff, 0x01, 0x0f, 00, 0x01, 0x01, 00, 00, 00, 00, 00, 0x01, 00, 00, 00, 00, 0x02, 00, 0x60, 00, 0x04, 00, 00, 00, 00, 00, 0x14, 00, 0xfd, 0x01, 0x02, 00, 0x01, 0x01, 00, 00, 00, 00, 00, 0x05, 0x12, 00, 00, 00, 00, 00, 0x18, 00, 0xff, 0x01, 0x0f, 00, 0x01, 0x02, 00, 00, 00, 00, 00, 0x05, 0x20, 00, 00, 00, 0x20, 0x02, 00, 00, 00, 00, 0x14, 00, 0x8d, 0x01, 0x02, 00, 0x01, 0x01, 00, 00, 00, 00, 00, 0x05, 0x0b, 00, 00, 00, 00, 00, 0x18, 00, 0xfd, 0x01, 0x02, 00, 0x01, 0x02, 00, 00, 00, 00, 00, 0x05, 0x20, 00, 00, 00, 0x23, 0x02, 00, 00, 0x01, 0x01, 00, 00, 00, 00, 00, 0x05, 0x12, 00, 00, 00, 0x01, 0x01, 00, 00, 00, 00, 00, 0x05, 0x12, 00, 00, 00 },
					RegistryValueKind.Binary);
				key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\USBSTOR\\Enum", true);
				if (key != null)
				{
					key.SetValue("Count", 0, RegistryValueKind.DWord);
					key.SetValue("NextInstance", 0, RegistryValueKind.DWord);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "RegistryHelper.Desintegrate 5");
			}
		}

		public static void Desintegrate()
		{
			RegistryKey key;

			//Включает TaskManager
			try
			{
				key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", true);
				key.SetValue("DisableTaskMgr", 0, RegistryValueKind.DWord);
			}
			catch (Exception e)
			{
				Logger.Error(e, "RegistryHelper.Desintegrate 1");
			}

			//Включает WinKey
			try
			{
				key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Keyboard Layout", true);
				key.DeleteValue("Scancode Map");
			}
			catch (Exception e)
			{
				Logger.Error(e, "RegistryHelper.Desintegrate 2");
			}

			//Включить Cdrom
			try
			{
				key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\Cdrom", true);
				key.SetValue("Start", 1, RegistryValueKind.DWord);
			}
			catch (Exception e)
			{
				Logger.Error(e, "RegistryHelper.Desintegrate 3");
			}

			//Включить Floppy
			try
			{
				key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\Flpydisk", true);
				key.SetValue("Start", 1, RegistryValueKind.DWord);
			}
			catch (Exception e)
			{
				Logger.Error(e, "RegistryHelper.Desintegrate 4");
			}

			return;
			//Включить Usb
			try
			{
				key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\USBSTOR", true);
				key.SetValue("Type", 1, RegistryValueKind.DWord);
				key.SetValue("Start", 3, RegistryValueKind.DWord);
				key.SetValue("ErrorControl", 1, RegistryValueKind.DWord);
				key.SetValue("ImagePath",
					new byte[] { 0x73, 00, 0x79, 00, 0x73, 00, 0x74, 00, 0x65, 00, 0x6d, 00, 0x33, 00, 0x32, 00, 0x5c, 00, 0x44, 00, 0x52, 00, 0x49, 00, 0x56, 00, 0x45, 00, 0x52, 00, 0x53, 00, 0x5c, 00, 0x55, 00, 0x53, 00, 0x42, 00, 0x53, 00, 0x54, 00, 0x4f, 00, 0x52, 00, 0x2e, 00, 0x53, 00, 0x59, 00, 0x53, 00, 00, 00 },
					RegistryValueKind.Binary);
				key.SetValue("DisplayName", "Драйвер запоминающих устройств для USB");
				key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\USBSTOR\\Security", true);
				key.SetValue("Security", "ex:01,00,14,80,90,00,00,00,9c,00,00,00,14,00,00,00,30,00,00,00,02,\\00,1c,00,01,00,00,00,02,80,14,00,ff,01,0f,00,01,01,00,00,00,00,00,01,00,00,\\00,00,02,00,60,00,04,00,00,00,00,00,14,00,fd,01,02,00,01,01,00,00,00,00,00,\\05,12,00,00,00,00,00,18,00,ff,01,0f,00,01,02,00,00,00,00,00,05,20,00,00,00,\\20,02,00,00,00,00,14,00,8d,01,02,00,01,01,00,00,00,00,00,05,0b,00,00,00,00,\\00,18,00,fd,01,02,00,01,02,00,00,00,00,00,05,20,00,00,00,23,02,00,00,01,01,\\00,00,00,00,00,05,12,00,00,00,01,01,00,00,00,00,00,05,12,00,00,00");
				key.SetValue("ImagePath",
					new byte[] { 0x01, 00, 0x14, 0x80, 0x90, 00, 00, 00, 0x9c, 00, 00, 00, 0x14, 00, 00, 00, 0x30, 00, 00, 00, 0x02, 00, 0x1c, 00, 0x01, 00, 00, 00, 0x02, 0x80, 0x14, 00, 0xff, 0x01, 0x0f, 00, 0x01, 0x01, 00, 00, 00, 00, 00, 0x01, 00, 00, 00, 00, 0x02, 00, 0x60, 00, 0x04, 00, 00, 00, 00, 00, 0x14, 00, 0xfd, 0x01, 0x02, 00, 0x01, 0x01, 00, 00, 00, 00, 00, 0x05, 0x12, 00, 00, 00, 00, 00, 0x18, 00, 0xff, 0x01, 0x0f, 00, 0x01, 0x02, 00, 00, 00, 00, 00, 0x05, 0x20, 00, 00, 00, 0x20, 0x02, 00, 00, 00, 00, 0x14, 00, 0x8d, 0x01, 0x02, 00, 0x01, 0x01, 00, 00, 00, 00, 00, 0x05, 0x0b, 00, 00, 00, 00, 00, 0x18, 00, 0xfd, 0x01, 0x02, 00, 0x01, 0x02, 00, 00, 00, 00, 00, 0x05, 0x20, 00, 00, 00, 0x23, 0x02, 00, 00, 0x01, 0x01, 00, 00, 00, 00, 00, 0x05, 0x12, 00, 00, 00, 0x01, 0x01, 00, 00, 00, 00, 00, 0x05, 0x12, 00, 00, 00 },
					RegistryValueKind.Binary);
				key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\USBSTOR\\Enum", true);
				if (key != null)
				{
					key.SetValue("Count", 1, RegistryValueKind.DWord);
					key.SetValue("NextInstance", 1, RegistryValueKind.DWord);
					key.SetValue("0", "USB\\Vid_1005&Pid_b113\\0D91018070E3595A");
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "RegistryHelper.Desintegrate 5");
			}
		}
	}
}