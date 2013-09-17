using System.Collections.Generic;
using Common.GK;
using FiresecAPI;
using XFiresecAPI;
using Infrastructure.Common.Windows;
using Infrastructure;
using Infrastructure.Common.Services;

namespace GKProcessor
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

		public static void SetAutomaticRegimeForDevice(XDevice device)
		{
			if (SendControlCommand(device, XStateBit.SetRegime_Automatic))
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

		public static void SetIgnoreRegimeForDevice(XDevice device)
		{
			if (ObjectCommandSendHelper.SendControlCommand(device, XStateBit.SetRegime_Off))
			{
				JournaActionlHelper.Add("Команда оператора", "Перевод в ручной режим", XStateClass.Info, device);
			}
		}

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

		static bool SendControlCommand(XBinaryBase binaryBase, XStateBit stateType)
		{
			var code = 0x80 + (int)stateType;
			return SendControlCommand(binaryBase, (byte)code);
		}

		static bool SendControlCommand(XBinaryBase binaryBase, byte code, bool mustValidatePassword = true)
		{
			var bytes = new List<byte>();
			var databaseNo = binaryBase.GetDatabaseNo(DatabaseType.Gk);
			bytes.AddRange(BytesHelper.ShortToBytes(databaseNo));
			bytes.Add(code);

			var result = true;
			if (mustValidatePassword)
			{
				result = ServiceFactoryBase.SecurityService.Validate();
			}
			if (result)
			{
				WatcherManager.Send(OnCompleted, SendPriority.Normal, binaryBase.GkDatabaseParent, 3, 13, 0, bytes);
			}
			return result;
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