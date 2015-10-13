﻿using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using RubezhAPI;
using RubezhAPI.GK;

namespace GKProcessor
{
	public static class ParametersHelper
	{
		public static string SetSingleParameter(GKBase gkBase, List<byte> parameterBytes)
		{
			try
			{
				var commonDatabase = GetCommonDatabase(gkBase);
				if (commonDatabase != null)
				{
					var descriptor = commonDatabase.Descriptors.FirstOrDefault(x => x.GKBase.UID == gkBase.UID);
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

		public static OperationResult<List<GKProperty>> GetSingleParameter(GKBase gkBase)
		{
			try
			{
				var commonDatabase = GetCommonDatabase(gkBase);
				if (commonDatabase != null)
				{
					var descriptor = commonDatabase.Descriptors.FirstOrDefault(x => x.GKBase.UID == gkBase.UID);
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
			return OperationResult<List<GKProperty>>.FromError("Непредвиденная ошибка");
		}

		static OperationResult<List<GKProperty>> GetDeviceParameters(CommonDatabase commonDatabase, BaseDescriptor descriptor)
		{
			var properties = new List<GKProperty>();

			var no = descriptor.GetDescriptorNo();
			var sendResult = SendManager.Send(commonDatabase.RootDevice, 2, 9, ushort.MaxValue, BytesHelper.ShortToBytes(no));
			if (sendResult.HasError)
			{
				return OperationResult<List<GKProperty>>.FromError(sendResult.Error);
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

			var device = descriptor.GKBase as GKDevice;
			if (device != null)
			{
				foreach (var driverProperty in device.Driver.Properties)
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
						var property = device.DeviceProperties.FirstOrDefault(x => x.Name == driverProperty.Name);
						if (property == null)
						{
							var systemProperty = device.Properties.FirstOrDefault(x => x.Name == driverProperty.Name);
							property = new GKProperty()
							{
								DriverProperty = systemProperty.DriverProperty,
								Name = systemProperty.Name,
								Value = paramValue,
							};
							device.DeviceProperties.Add(property);
						}
						if (property != null)
						{
							property.Value = paramValue;
							property.DriverProperty = driverProperty;
							if (property.DriverProperty.DriverPropertyType == GKDriverPropertyTypeEnum.BoolType)
								property.Value = (ushort)(property.Value > 0 ? 1 : 0);

							properties.Add(property);
						}
					}
					else
						return OperationResult<List<GKProperty>>.FromError("Неизвестный номер параметра");
				}
			}
			if ((descriptor.DescriptorType == DescriptorType.Direction || descriptor.DescriptorType == DescriptorType.Delay
				|| descriptor.DescriptorType == DescriptorType.GuardZone || descriptor.DescriptorType == DescriptorType.PumpStation) && binProperties.Count >= 3)
			{
				properties.Add(new GKProperty() { Value = binProperties[0].Value });
				properties.Add(new GKProperty() { Value = binProperties[1].Value });
				properties.Add(new GKProperty() { Value = binProperties[2].Value });
			}
			if ((descriptor.DescriptorType == DescriptorType.Code || descriptor.DescriptorType == DescriptorType.Door) && binProperties.Count >= 2)
			{
				properties.Add(new GKProperty() { Value = binProperties[0].Value });
				properties.Add(new GKProperty() { Value = binProperties[1].Value });
			}
			return new OperationResult<List<GKProperty>>(properties);
		}
		static string SetDeviceParameters(CommonDatabase commonDatabase, BaseDescriptor descriptor, List<byte> parameterBytes)
		{
			var device = descriptor.GKBase as GKDevice;
			if (device != null)
			{
				foreach (var property in device.Properties)
				{
					var driverProperty = device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
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
				bytes.AddRange(parameterBytes);
				var sendResult = SendManager.Send(rootDevice, (ushort)bytes.Count, 10, 0, bytes);
				if (sendResult.HasError)
					return sendResult.Error;
			}
			return null;
		}

		static CommonDatabase GetCommonDatabase(GKBase gkBase)
		{
			CommonDatabase commonDatabase = null;
			if (gkBase.KauDatabaseParent != null)
			{
				commonDatabase = DescriptorsManager.KauDatabases.FirstOrDefault(x => x.RootDevice == gkBase.KauDatabaseParent);
			}
			else if (gkBase.GkDatabaseParent != null)
			{
				commonDatabase = DescriptorsManager.GkDatabases.FirstOrDefault(x => x.RootDevice == gkBase.GkDatabaseParent);
			}
			return commonDatabase;
		}

		public static BaseDescriptor GetBaseDescriptor(GKBase gkBase)
		{
			var commonDatabase = GetCommonDatabase(gkBase);
			if (commonDatabase != null)
			{
				return commonDatabase.Descriptors.FirstOrDefault(x => x.GKBase.UID == gkBase.UID);
			}
			return null;
		}
	}
}