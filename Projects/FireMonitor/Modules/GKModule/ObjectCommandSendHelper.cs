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
			if (ObjectCommandSendHelper.SendControlCommand(device, stateType))
			{
				JournaActionlHelper.Add("Команда оператора", stateType.ToDescription(), XStateClass.Info, device);
			}
		}

		public static void ResetDevice(XDevice device)
		{
			if (ObjectCommandSendHelper.SendControlCommand(device, XStateBit.Reset))
			{
				JournaActionlHelper.Add("Команда оператора", "Сброс", XStateClass.Info, device);
			}
		}

		public static void ResetZone(XZone zone)
		{
			if (ObjectCommandSendHelper.SendControlCommand(zone, XStateBit.Reset))
			{
				JournaActionlHelper.Add("Команда оператора", "Сброс", XStateClass.Info, zone);
			}
		}

		public static void ResetFire1(XZone zone, bool mustValidatePassword = true)
		{
			if (ObjectCommandSendHelper.SendControlCommand(zone, 0x02, mustValidatePassword))
			{
				JournaActionlHelper.Add("Команда оператора", "Сброс", XStateClass.Info, zone);
			}
		}

		public static void ResetFire2(XZone zone, bool mustValidatePassword = true)
		{
			if (ObjectCommandSendHelper.SendControlCommand(zone, 0x03, mustValidatePassword))
			{
				JournaActionlHelper.Add("Команда оператора", "Сброс", XStateClass.Info, zone);
			}
		}

		public static void SetAutomaticRegimeForZone(XZone zone)
		{
			if (SendControlCommand(zone, XStateBit.SetRegime_Automatic))
			{
				JournaActionlHelper.Add("Команда оператора", "Перевод в автоматический режим", XStateClass.Info, zone);
			}
		}

		public static void SetIgnoreRegimeForZone(XZone zone)
		{
			if (ObjectCommandSendHelper.SendControlCommand(zone, XStateBit.SetRegime_Off))
			{
				JournaActionlHelper.Add("Команда оператора", "Отключение", XStateClass.Info, zone);
			}
		}

		public static void SetAutomaticRegimeForDevice(XDevice device, bool mustValidatePassword = true)
		{
			if (SendControlCommand(device, XStateBit.SetRegime_Automatic, mustValidatePassword))
			{
				JournaActionlHelper.Add("Команда оператора", "Перевод в автоматический режим", XStateClass.Info, device);
			}
		}

		public static void SetManualRegimeForDevice(XDevice device)
		{
			if (ObjectCommandSendHelper.SendControlCommand(device, XStateBit.SetRegime_Manual))
			{
				JournaActionlHelper.Add("Команда оператора", "Перевод в ручной режим", XStateClass.Info, device);
			}
		}

		public static void SetIgnoreRegimeForDevice(XDevice device, bool mustValidatePassword = true)
		{
			if (ObjectCommandSendHelper.SendControlCommand(device, XStateBit.SetRegime_Off, mustValidatePassword))
			{
				JournaActionlHelper.Add("Команда оператора", "Перевод в ручной режим", XStateClass.Info, device);
			}
		}

		#region Direction
		public static void SetAutomaticRegimeForDirection(XDirection direction)
		{
			if (SendControlCommand(direction, XStateBit.SetRegime_Automatic))
			{
				JournaActionlHelper.Add("Команда оператора", "Перевод в автоматический режим", XStateClass.Info, direction);
			}
		}

		public static void SetManualRegimeForDirection(XDirection direction)
		{
			if (ObjectCommandSendHelper.SendControlCommand(direction, XStateBit.SetRegime_Manual))
			{
				JournaActionlHelper.Add("Команда оператора", "Перевод в ручной режим", XStateClass.Info, direction);
			}
		}

		public static void SetIgnoreRegimeForDirection(XDirection direction)
		{
			if (ObjectCommandSendHelper.SendControlCommand(direction, XStateBit.SetRegime_Off))
			{
				JournaActionlHelper.Add("Команда оператора", "Перевод в ручной режим", XStateClass.Info, direction);
			}
		}

		public static void TurnOnDirection(XDirection direction)
		{
			if (ObjectCommandSendHelper.SendControlCommand(direction, XStateBit.TurnOn_InManual))
			{
				JournaActionlHelper.Add("Команда оператора", "Включить", XStateClass.Info, direction);
			}
		}

		public static void TurnOnNowDirection(XDirection direction)
		{
			if (ObjectCommandSendHelper.SendControlCommand(direction, XStateBit.TurnOnNow_InManual))
			{
				JournaActionlHelper.Add("Команда оператора", "Включить немедленно", XStateClass.Info, direction);
			}
		}

		public static void TurnOffDirection(XDirection direction)
		{
			if (ObjectCommandSendHelper.SendControlCommand(direction, XStateBit.TurnOff_InManual))
			{
				JournaActionlHelper.Add("Команда оператора", "Выключить", XStateClass.Info, direction);
			}
		}
		#endregion

		#region Delay
		public static void SetAutomaticRegimeForDelay(XDelay delay)
		{
			if (SendControlCommand(delay, XStateBit.SetRegime_Automatic))
			{
				JournaActionlHelper.Add("Команда оператора", "Перевод в автоматический режим", XStateClass.Info, delay);
			}
		}

		public static void SetManualRegimeForDelay(XDelay delay)
		{
			if (ObjectCommandSendHelper.SendControlCommand(delay, XStateBit.SetRegime_Manual))
			{
				JournaActionlHelper.Add("Команда оператора", "Перевод в ручной режим", XStateClass.Info, delay);
			}
		}

		public static void SetIgnoreRegimeForDelay(XDelay delay)
		{
			if (ObjectCommandSendHelper.SendControlCommand(delay, XStateBit.SetRegime_Off))
			{
				JournaActionlHelper.Add("Команда оператора", "Перевод в ручной режим", XStateClass.Info, delay);
			}
		}

		public static void TurnOnDelay(XDelay delay)
		{
			if (ObjectCommandSendHelper.SendControlCommand(delay, XStateBit.TurnOn_InManual))
			{
				JournaActionlHelper.Add("Команда оператора", "Включить", XStateClass.Info, delay);
			}
		}

		public static void TurnOnNowDelay(XDelay delay)
		{
			if (ObjectCommandSendHelper.SendControlCommand(delay, XStateBit.TurnOnNow_InManual))
			{
				JournaActionlHelper.Add("Команда оператора", "Включить немедленно", XStateClass.Info, delay);
			}
		}

		public static void TurnOffDelay(XDelay delay)
		{
			if (ObjectCommandSendHelper.SendControlCommand(delay, XStateBit.TurnOff_InManual))
			{
				JournaActionlHelper.Add("Команда оператора", "Выключить", XStateClass.Info, delay);
			}
		}
		#endregion

		public static void TurnOffDeviceAnyway(XDevice device)
		{
			var stateType = XStateBit.TurnOff_InManual;
			if (device.DeviceState.StateBits.Contains(XStateBit.Norm))
				ObjectCommandSendHelper.SendControlCommand(device, XStateBit.TurnOff_InAutomatic);

			if (ObjectCommandSendHelper.SendControlCommand(device, stateType))
			{
				JournaActionlHelper.Add("Команда оператора", "Выключение", XStateClass.Info, device);
			}
		}

		public static void TurnOffDirectionAnyway(XDirection direction)
		{
			var stateType = XStateBit.TurnOff_InManual;
			if (direction.DirectionState.StateBits.Contains(XStateBit.Norm))
				ObjectCommandSendHelper.SendControlCommand(direction, XStateBit.TurnOff_InAutomatic);

			if (ObjectCommandSendHelper.SendControlCommand(direction, stateType))
			{
				JournaActionlHelper.Add("Команда оператора", "Выключение", XStateClass.Info, direction);
			}
		}

		static bool SendControlCommand(XBase xBase, XStateBit stateType, bool mustValidatePassword = true)
		{
			var code = 0x80 + (int)stateType;
			return SendControlCommand(xBase, (byte)code, mustValidatePassword);
		}

		static bool SendControlCommand(XBase xBase, byte code, bool mustValidatePassword = true)
		{
			var bytes = new List<byte>();
			var databaseNo = xBase.GKDescriptorNo;
			bytes.AddRange(BytesHelper.ShortToBytes(databaseNo));
			bytes.Add(code);

			var result = true;
			if (mustValidatePassword)
			{
				result = ServiceFactory.SecurityService != null && ServiceFactory.SecurityService.Validate();
			}
			if (result)
			{
				WatcherManager.Send(OnCompleted, SendPriority.Normal, xBase.GkDatabaseParent, 3, 13, 0, bytes);
			}
			return result;
		}

		public static void SendControlCommandMRO(XBase xBase, byte code, byte code2)
		{
			var bytes = new List<byte>();
			var databaseNo = xBase.GKDescriptorNo;
			bytes.AddRange(BytesHelper.ShortToBytes(databaseNo));
			bytes.Add(code);
			bytes.Add(code2);

			WatcherManager.Send(OnCompleted, SendPriority.Normal, xBase.GkDatabaseParent, 3, 13, 0, bytes);
		}

		static void OnCompleted(SendResult sendResult)
		{
			if (sendResult.HasError)
			{
				ApplicationService.BeginInvoke(() =>
				{
					MessageBoxService.ShowError("Ошибка при выполнении операции");
				});
			}
		}
	}
}