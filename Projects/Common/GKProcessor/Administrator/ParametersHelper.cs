using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecAPI;
using FiresecAPI.GK;

namespace GKProcessor
{
	public static class ParametersHelper
	{
		public static string SetSingleParameter(XBase xBase, List<byte> parameterBytes)
		{
			try
			{
				var commonDatabase = GetCommonDatabase(xBase);
				if (commonDatabase != null)
				{
					var descriptor = commonDatabase.Descriptors.FirstOrDefault(x => x.XBase.BaseUID == xBase.BaseUID);
					if (descriptor != null)
					{
						return SetDeviceParameters(commonDatabase, descriptor, parameterBytes);
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "ParametersHelper.SetSingleParameter");
			}
			return null;
		}

		public static OperationResult<List<XProperty>> GetSingleParameter(XBase xBase)
		{
			try
			{
				var commonDatabase = GetCommonDatabase(xBase);
				if (commonDatabase != null)
				{
					var descriptor = commonDatabase.Descriptors.FirstOrDefault(x => x.XBase.BaseUID == xBase.BaseUID);
					if (descriptor != null)
					{
						var result = GetDeviceParameters(commonDatabase, descriptor);
						return result;
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "ParametersHelper.GetSingleParameter");
			}
			return new OperationResult<List<XProperty>>("Непредвиденная ошибка");
		}

		static OperationResult<List<XProperty>> GetDeviceParameters(CommonDatabase commonDatabase, BaseDescriptor descriptor)
		{
			var properties = new List<XProperty>();

			var no = descriptor.GetDescriptorNo();
			var sendResult = SendManager.Send(commonDatabase.RootDevice, 2, 9, ushort.MaxValue, BytesHelper.ShortToBytes(no));
			if (sendResult.HasError)
			{
				return new OperationResult<List<XProperty>>(sendResult.Error);
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
						if (property != null)
						{
							property.Value = paramValue;
							property.DriverProperty = driverProperty;
							if (property.DriverProperty.DriverPropertyType == XDriverPropertyTypeEnum.BoolType)
								property.Value = (ushort)(property.Value > 0 ? 1 : 0);

							properties.Add(property);
						}
					}
					else
						return new OperationResult<List<XProperty>>("Неизвестный номер параметра");
				}
			}
			if (descriptor.Direction != null && binProperties.Count >= 3)
			{
				properties.Add(new XProperty() { Value = binProperties[0].Value });
				properties.Add(new XProperty() { Value = binProperties[1].Value });
				properties.Add(new XProperty() { Value = binProperties[2].Value });
			}
			return new OperationResult<List<XProperty>>() { Result = properties };
		}
		static string SetDeviceParameters(CommonDatabase commonDatabase, BaseDescriptor descriptor, List<byte> parameterBytes)
		{
			if (descriptor.Device != null)
			{
				foreach (var property in descriptor.Device.Properties)
				{
					var driverProperty = descriptor.Device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
					if (driverProperty != null)
					{
						double minValue = driverProperty.Min;
						double maxValue = driverProperty.Max;
						double value = property.Value;

						if (driverProperty.Multiplier != 0)
						{
							minValue /= driverProperty.Multiplier;
							maxValue /= driverProperty.Multiplier;
							value /= driverProperty.Multiplier;
						}

						if (minValue != 0)
							if (value < minValue)
								return "Параметр " + driverProperty.Caption + " должен быть больше " + minValue.ToString();

						if (maxValue != 0)
							if (value > maxValue)
								return "Параметр " + driverProperty.Caption + " должен быть меньше " + maxValue.ToString();
					}
				}
			}

			if (descriptor.Parameters.Count > 0)
			{
				var rootDevice = commonDatabase.RootDevice;
				var no = descriptor.GetDescriptorNo();
				var bytes = new List<byte>();
				bytes.AddRange(BytesHelper.ShortToBytes(no));
				//bytes.AddRange(descriptor.Parameters);
				bytes.AddRange(parameterBytes);
				var sendResult = SendManager.Send(rootDevice, (ushort)bytes.Count, 10, 0, bytes);
				if (sendResult.HasError)
					return sendResult.Error;
			}
			return null;
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

		public static BaseDescriptor GetBaseDescriptor(XBase xBase)
		{
			var commonDatabase = GetCommonDatabase(xBase);
			if (commonDatabase != null)
			{
				return commonDatabase.Descriptors.FirstOrDefault(x => x.XBase.BaseUID == xBase.BaseUID);
			}
			return null;
		}
	}
}