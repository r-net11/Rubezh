using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using GKProcessor;
using Infrastructure.Common.Windows;
using XFiresecAPI;

namespace GKProcessor
{
	public static class ParametersHelper
	{
		public static event Action<ushort, ushort, ushort> AllParametersChanged;
		
		public static string SetSingleParameter(XDevice device)
		{
			var errorLog = "";
			try
			{
				var commonDatabase = GetCommonDatabase(device);
				if (commonDatabase != null)
				{
					var descriptor = commonDatabase.Descriptors.FirstOrDefault(x => x.Device == device);
					if (descriptor != null)
					{
						var result = SetDeviceParameters(commonDatabase, descriptor);
						if (result != null)
						{
							errorLog = "Ошибка";
						}
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "ParametersHelper.SetSingleParameter");
			}
			return errorLog;
		}

		public static string GetSingleParameter(XDevice device)
		{
			var errorLog = "";
			try
			{
				var commonDatabase = GetCommonDatabase(device);
				if (commonDatabase != null)
				{
					var descriptor = commonDatabase.Descriptors.FirstOrDefault(x => x.Device == device);
					if (descriptor != null)
					{
						var result = GetDeviceParameters(commonDatabase, descriptor);
						if (!result)
						{
							errorLog = "Ошибка";
						}
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "ParametersHelper.GetSingleParameter");
			}
			return errorLog;
		}

		static bool GetDeviceParameters(CommonDatabase commonDatabase, BaseDescriptor descriptor)
		{
			var no = descriptor.GetDescriptorNo();
			LoadingService.DoStep("Запрос параметров объекта " + no);
			var sendResult = SendManager.Send(commonDatabase.RootDevice, 2, 9, ushort.MaxValue, BytesHelper.ShortToBytes(no));
			if (sendResult.HasError)
			{
				return false;
			}

			var binProperties = new List<BinProperty>();
			for (int i = 0; i < sendResult.Bytes.Count / 4; i++)
			{
				byte paramNo = sendResult.Bytes[i * 4];
				ushort paramValue = BytesHelper.SubstructShort(sendResult.Bytes, i * 4 + 1);
				var binProperty = new BinProperty()
				{
					No = paramNo,
					Value = paramValue
				};
				binProperties.Add(binProperty);
			}

			if (descriptor.Device != null)
			{
				foreach (var driverProperty in descriptor.Device.Driver.Properties)
				{
					if (!driverProperty.IsAUParameter)
						continue;

					var binProperty = binProperties.FirstOrDefault(x => x.No == driverProperty.No);
					if (binProperty != null)
					{
						var paramValue = binProperty.Value;
						if (driverProperty.IsLowByte)
						{
							paramValue = (ushort)(paramValue << 8);
							paramValue = (ushort)(paramValue >> 8);
						}
						if (driverProperty.IsHieghByte)
						{
							paramValue = (ushort)(paramValue / 256);
						}
						if (driverProperty.Mask != 0)
						{
							paramValue = (byte)(paramValue & driverProperty.Mask);
						}
						if (driverProperty.Multiplier != 0)
						{
							paramValue = (ushort)((double)paramValue / driverProperty.Multiplier);
						}
						var property = descriptor.Device.DeviceProperties.FirstOrDefault(x => x.Name == driverProperty.Name);
						if (property == null)
						{
							var systemProperty = descriptor.Device.Properties.FirstOrDefault(x => x.Name == driverProperty.Name);
							descriptor.Device.DeviceProperties.Add(new XProperty()
							{
							    DriverProperty = systemProperty.DriverProperty,
							    Name = systemProperty.Name,
							    Value = paramValue,
							});
						}
						if (property != null && property.Value != paramValue)
						{
							property.Value = paramValue;
							property.DriverProperty = driverProperty;
							if (property.DriverProperty.DriverPropertyType == XDriverPropertyTypeEnum.BoolType)
								property.Value = (ushort)(property.Value > 0 ? 1 : 0);
							descriptor.Device.OnAUParametersChanged();
						}
					}
					else
						return false;
				}
			}
			AllParametersChanged(binProperties[0].Value, binProperties[1].Value, binProperties[2].Value);
			return true;
		}
		static string SetDeviceParameters(CommonDatabase commonDatabase, BaseDescriptor descriptor)
		{
			if (descriptor.Device != null)
			{
				foreach (var property in descriptor.Device.Properties)
				{
					var driverProperty = descriptor.Device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
					if (driverProperty != null)
					{
						if (driverProperty.Min != 0)
							if (property.Value < driverProperty.Min)
								return "Парметр " + driverProperty.Caption + " должен быть больше " + driverProperty.Min.ToString();

						if (driverProperty.Max != 0)
							if (property.Value > driverProperty.Max)
								return "Парметр " + driverProperty.Caption + " должен быть меньше " + driverProperty.Max.ToString();
					}
				}
			}

			if (descriptor.Parameters.Count > 0)
			{
				var rootDevice = commonDatabase.RootDevice;
				var no = descriptor.GetDescriptorNo();
				var bytes = new List<byte>();
				bytes.AddRange(BytesHelper.ShortToBytes(no));
				bytes.AddRange(descriptor.Parameters);
				LoadingService.DoStep("Запись параметров объекта " + no);
				var sendResult = SendManager.Send(rootDevice, (ushort)bytes.Count, 10, 0, bytes);
				if (sendResult.HasError)
					return sendResult.Error;
			}
			return null;
		}

		public static string SetSingleDirectionParameter(XDirection direction)
		{
			var errorLog = "";
			DescriptorsManager.Create();
			try
			{
				var commonDatabase = GetCommonDatabase(direction);
				if (commonDatabase != null)
				{
					var descriptor = commonDatabase.Descriptors.FirstOrDefault(x => x.Direction == direction);
					if (descriptor != null)
					{
						var result = SetDeviceParameters(commonDatabase, descriptor);
						if (!string.IsNullOrEmpty(result))
						{
							errorLog = "Ошибка при записи параметра направления " + direction.PresentationName;
						}
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "ParametersHelper.SetDirectionParameters");
			}
			return errorLog;
		}

		public static string GetSingleDirectionParameter(XDirection direction)
		{
			var errorLog = "";
			DescriptorsManager.Create();
			try
			{
				var commonDatabase = GetCommonDatabase(direction);
				if (commonDatabase != null)
				{
					var descriptor = commonDatabase.Descriptors.FirstOrDefault(x => x.Direction == direction);
					if (descriptor != null)
					{
						var result = GetDeviceParameters(commonDatabase, descriptor);
						if (!result)
						{
							errorLog = "Ошибка";
						}
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "ParametersHelper.GetSingleParameter");
			}
			return errorLog;
		}

		static CommonDatabase GetCommonDatabase(XBase xBase)
		{
			CommonDatabase commonDatabase = null;
			if (xBase.KauDatabaseParent != null)
			{
				commonDatabase = DescriptorsManager.KauDatabases.FirstOrDefault(x => x.RootDevice == xBase.KauDatabaseParent);
			}
			else if (xBase.GkDatabaseParent != null)
			{
				commonDatabase = DescriptorsManager.GkDatabases.FirstOrDefault(x => x.RootDevice == xBase.GkDatabaseParent);
			}
			return commonDatabase;
		}
	}
}