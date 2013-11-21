using System.Collections.Generic;
using System.Linq;
using GKProcessor;
using FiresecAPI;
using XFiresecAPI;
using Infrastructure.Common.Windows;
using Infrastructure;

namespace GKModule
{
	public static class ObjectCommandSendHelper
	{
		public static void ExecuteDeviceCommand(XDevice device, XStateBit stateType)
		{
			Watcher.SendControlCommand(device, stateType);
			JournaActionlHelper.Add("Команда оператора", stateType.ToDescription(), XStateClass.Info, device);
		}

		public static void Reset(XBase xBase)
		{
			Watcher.SendControlCommand(xBase, XStateBit.Reset);
			JournaActionlHelper.Add("Команда оператора", "Сброс", XStateClass.Info, xBase);
		}

		public static void ResetFire1(XZone zone)
		{
			Watcher.SendControlCommand(zone, 0x02);
			JournaActionlHelper.Add("Команда оператора", "Сброс", XStateClass.Info, zone);
		}

		public static void ResetFire2(XZone zone)
		{
			Watcher.SendControlCommand(zone, 0x03);
			JournaActionlHelper.Add("Команда оператора", "Сброс", XStateClass.Info, zone);
		}

		public static void SetAutomaticRegime(XBase xBase)
		{
			Watcher.SendControlCommand(xBase, XStateBit.SetRegime_Automatic);
			JournaActionlHelper.Add("Команда оператора", "Перевод в автоматический режим", XStateClass.Info, xBase);
		}

		public static void SetManualRegime(XBase xBase)
		{
			Watcher.SendControlCommand(xBase, XStateBit.SetRegime_Manual);
			JournaActionlHelper.Add("Команда оператора", "Перевод в ручной режим", XStateClass.Info, xBase);
		}

		public static void SetIgnoreRegime(XBase xBase)
		{
			Watcher.SendControlCommand(xBase, XStateBit.SetRegime_Off);
			JournaActionlHelper.Add("Команда оператора", "Перевод в ручной режим", XStateClass.Info, xBase);
		}

		public static void TurnOn(XBase xBase)
		{
			Watcher.SendControlCommand(xBase, XStateBit.TurnOn_InManual);
			JournaActionlHelper.Add("Команда оператора", "Включить", XStateClass.Info, xBase);
		}

		public static void TurnOnNow(XBase xBase)
		{
			Watcher.SendControlCommand(xBase, XStateBit.TurnOnNow_InManual);
			JournaActionlHelper.Add("Команда оператора", "Включить немедленно", XStateClass.Info, xBase);
		}

		public static void TurnOff(XBase xBase)
		{
			Watcher.SendControlCommand(xBase, XStateBit.TurnOff_InManual);
			JournaActionlHelper.Add("Команда оператора", "Выключить", XStateClass.Info, xBase);
		}

		public static void TurnOffNow(XBase xBase)
		{
			Watcher.SendControlCommand(xBase, XStateBit.TurnOffNow_InManual);
			JournaActionlHelper.Add("Команда оператора", "Выключить немедленно", XStateClass.Info, xBase);
		}

		public static void Stop(XBase xBase)
		{
			Watcher.SendControlCommand(xBase, XStateBit.Stop_InManual);
			JournaActionlHelper.Add("Команда оператора", "Остановка пуска", XStateClass.Info, xBase);
		}
	}
}