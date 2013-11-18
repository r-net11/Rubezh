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
            if (Watcher.SendControlCommand(device, stateType))
			{
				JournaActionlHelper.Add("Команда оператора", stateType.ToDescription(), XStateClass.Info, device);
			}
		}

		public static void Reset(XBase xBase)
		{
            if (Watcher.SendControlCommand(xBase, XStateBit.Reset))
			{
				JournaActionlHelper.Add("Команда оператора", "Сброс", XStateClass.Info, xBase);
			}
		}

		public static void ResetFire1(XZone zone, bool mustValidatePassword = true)
		{
            if (Watcher.SendControlCommand(zone, 0x02, mustValidatePassword))
			{
				JournaActionlHelper.Add("Команда оператора", "Сброс", XStateClass.Info, zone);
			}
		}

		public static void ResetFire2(XZone zone, bool mustValidatePassword = true)
		{
            if (Watcher.SendControlCommand(zone, 0x03, mustValidatePassword))
			{
				JournaActionlHelper.Add("Команда оператора", "Сброс", XStateClass.Info, zone);
			}
		}

		public static void SetAutomaticRegime(XBase xBase, bool mustValidatePassword = true)
		{
            if (Watcher.SendControlCommand(xBase, XStateBit.SetRegime_Automatic, mustValidatePassword))
			{
				JournaActionlHelper.Add("Команда оператора", "Перевод в автоматический режим", XStateClass.Info, xBase);
			}
		}

		public static void SetManualRegime(XBase xBase)
		{
            if (Watcher.SendControlCommand(xBase, XStateBit.SetRegime_Manual))
			{
				JournaActionlHelper.Add("Команда оператора", "Перевод в ручной режим", XStateClass.Info, xBase);
			}
		}

		public static void SetIgnoreRegime(XBase xBase, bool mustValidatePassword = true)
		{
            if (Watcher.SendControlCommand(xBase, XStateBit.SetRegime_Off, mustValidatePassword))
			{
				JournaActionlHelper.Add("Команда оператора", "Перевод в ручной режим", XStateClass.Info, xBase);
			}
		}

		public static void TurnOn(XBase xBase)
		{
            if (Watcher.SendControlCommand(xBase, XStateBit.TurnOn_InManual))
			{
				JournaActionlHelper.Add("Команда оператора", "Включить", XStateClass.Info, xBase);
			}
		}

		public static void TurnOnNow(XBase xBase)
		{
            if (Watcher.SendControlCommand(xBase, XStateBit.TurnOnNow_InManual))
			{
				JournaActionlHelper.Add("Команда оператора", "Включить немедленно", XStateClass.Info, xBase);
			}
		}

		public static void TurnOff(XBase xBase)
		{
            if (Watcher.SendControlCommand(xBase, XStateBit.TurnOff_InManual))
			{
				JournaActionlHelper.Add("Команда оператора", "Выключить", XStateClass.Info, xBase);
			}
		}

		public static void TurnOffNow(XBase xBase)
		{
			if (Watcher.SendControlCommand(xBase, XStateBit.TurnOffNow_InManual))
			{
				JournaActionlHelper.Add("Команда оператора", "Выключить немедленно", XStateClass.Info, xBase);
			}
		}

		public static void Stop(XBase xBase)
		{
			if (Watcher.SendControlCommand(xBase, XStateBit.Stop_InManual))
			{
				JournaActionlHelper.Add("Команда оператора", "Остановка пуска", XStateClass.Info, xBase);
			}
		}

        //static bool SendControlCommand(XBase xBase, XStateBit stateType, bool mustValidatePassword = true)
        //{
        //    return Watcher.SendControlCommand(xBase, stateType, mustValidatePassword);
        //    //var code = 0x80 + (int)stateType;
        //    //return SendControlCommand(xBase, (byte)code, mustValidatePassword);
        //}

        //static bool SendControlCommand(XBase xBase, byte code, bool mustValidatePassword = true)
        //{
        //    var bytes = new List<byte>();
        //    var databaseNo = xBase.GKDescriptorNo;
        //    bytes.AddRange(BytesHelper.ShortToBytes(databaseNo));
        //    bytes.Add(code);

        //    var result = true;
        //    if (mustValidatePassword)
        //    {
        //        result = ServiceFactory.SecurityService != null && ServiceFactory.SecurityService.Validate();
        //    }
        //    if (result)
        //    {
        //        WatcherManager.Send(OnCompleted, SendPriority.Normal, xBase.GkDatabaseParent, 3, 13, 0, bytes);
        //    }
        //    return result;
        //}

        //public static void SendControlCommandMRO(XBase xBase, byte code, byte code2)
        //{
        //    var bytes = new List<byte>();
        //    var databaseNo = xBase.GKDescriptorNo;
        //    bytes.AddRange(BytesHelper.ShortToBytes(databaseNo));
        //    bytes.Add(code);
        //    bytes.Add(code2);

        //    WatcherManager.Send(OnCompleted, SendPriority.Normal, xBase.GkDatabaseParent, 3, 13, 0, bytes);
        //}

        //static void OnCompleted(SendResult sendResult)
        //{
        //    if (sendResult.HasError)
        //    {
        //        ApplicationService.BeginInvoke(() =>
        //        {
        //            MessageBoxService.ShowError("Ошибка при выполнении операции");
        //        });
        //    }
        //}
	}
}