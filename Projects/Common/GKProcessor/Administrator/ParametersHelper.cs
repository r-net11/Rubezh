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

		public static void GetAllParameters()
		{
			DatabaseManager.Convert();
			foreach (var kauDatabase in DatabaseManager.KauDatabases)
			{
				LoadingService.Show("Запрос параметров", kauDatabase.Descriptors.Count);
				try
				{
					foreach (var descriptor in kauDatabase.Descriptors)
					{
						if (descriptor.Device != null)
						{
							var result = GetDeviceParameters(kauDatabase, descriptor);
							if (!result)
							{
								MessageBoxService.ShowError("Ошибка при чтении параметра устройства " + descriptor.Device.PresentationDriverAndAddress);
								return;
							}
						}
					}
				}
				catch (Exception e)
				{
					Logger.Error(e, "ParametersHelper.GetParametersFromDB");
				}
				finally
				{
					LoadingService.Close();
				}
			}
		}

		public static void SetAllParameters()
		{
			DatabaseManager.Convert();
			foreach (var kauDatabase in DatabaseManager.KauDatabases)
			{
				LoadingService.Show("Запись параметров", kauDatabase.Descriptors.Count);
				try
				{
					foreach (var descriptor in kauDatabase.Descriptors)
					{
						if (descriptor.Device != null)
						{
							var result = SetDeviceParameters(kauDatabase, descriptor);
							if (!string.IsNullOrEmpty(result))
							{
								MessageBoxService.ShowError("Ошибка при записи параметра устройства " + descriptor.Device.PresentationDriverAndAddress + "\n" + result);
								return;
							}
						}
					}
				}
				catch (Exception e)
				{
					Logger.Error(e, "ParametersHelper.SetParametersToDB");
				}
				finally
				{
					LoadingService.Close();
				}
			}
		}

		public static void SetSingleParameter(XDevice device)
		{
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
							ErrorLog += "\n" + device.PresentationDriverAndAddress;
						}
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "ParametersHelper.SetSingleParameter");
			}
		}

		public static string ErrorLog { get; set; }

		public static void GetSingleParameter(XDevice device)
		{
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
							ErrorLog += "\n" + device.PresentationDriverAndAddress;
						}
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "ParametersHelper.GetSingleParameter");
			}
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
						var paramValue = (ushort)binProperty.Value;
						if (driverProperty.IsLowByte)
						{
							paramValue = (ushort)(paramValue << 8);
							paramValue = (ushort)(paramValue >> 8);
						}
						if (driverProperty.IsHieghByte)
						{
							paramValue = (ushort)(paramValue / 256);
						}
						paramValue = (ushort)(paramValue >> driverProperty.Offset);
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
							        Value = paramValue
							    });
						}
						if (property != null)
						{
							property.Value = paramValue;
						}
					}
					else
					{
						return false;
					}
				}
			}
			descriptor.Device.OnAUParametersChanged();
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

						if (driverProperty.Multiplier != 0)
						{
							property.Value = (ushort)(property.Value * driverProperty.Multiplier);
						}
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

		public static void SetSingleDirectionParameter(XDirection direction)
		{
			DatabaseManager.Convert();
			try
			{
				var commonDatabase = GetCommonDatabase(direction);
				if (commonDatabase != null)
				{
					var descriptor = commonDatabase.Descriptors.FirstOrDefault(x => x.Direction == direction);
					if (descriptor != null)
					{
						var result = SetDirectionParameters(commonDatabase, descriptor);
						if (!string.IsNullOrEmpty(result))
						{
							MessageBoxService.ShowError("Ошибка при записи параметра направления " + direction.PresentationName + "\n" + result);
						}
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "ParametersHelper.SetDirectionParameters");
			}
		}

		static string SetDirectionParameters(CommonDatabase commonDatabase, BaseDescriptor descriptor)
		{
			if (descriptor.Device != null)
			{
				foreach (var property in descriptor.Device.DeviceProperties)
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

						if (driverProperty.Multiplier != 0)
						{
							property.Value = (ushort)(property.Value * driverProperty.Multiplier);
						}
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

		public static void GetSingleDirectionParameter(XDirection direction)
		{
			DatabaseManager.Convert();
			try
			{
				var commonDatabase = GetCommonDatabase(direction);
				if (commonDatabase != null)
				{
					var descriptor = commonDatabase.Descriptors.FirstOrDefault(x => x.Direction == direction);
					if (descriptor != null)
					{
						var result = GetDirectionParameters(commonDatabase, descriptor, direction);
						if (!result)
						{
							ErrorLog += "\n" + direction.PresentationName;
						}
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "ParametersHelper.GetSingleParameter");
			}
		}

		static bool GetDirectionParameters(CommonDatabase commonDatabase, BaseDescriptor descriptor, XDirection direction)
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
						var paramValue = (ushort)binProperty.Value;
						if (driverProperty.IsLowByte)
						{
							paramValue = (ushort)(paramValue << 8);
							paramValue = (ushort)(paramValue >> 8);
						}
						if (driverProperty.IsHieghByte)
						{
							paramValue = (ushort)(paramValue / 256);
						}
						paramValue = (ushort)(paramValue >> driverProperty.Offset);
						if (driverProperty.Mask != 0)
						{
							paramValue = (byte)(paramValue & driverProperty.Mask);
						}
						if (driverProperty.Multiplier != 0)
						{
							paramValue = (ushort)((double)paramValue / driverProperty.Multiplier);
						}
						var property = descriptor.Device.DeviceProperties.FirstOrDefault(x => x.Name == driverProperty.Name);
						if (property != null)
						{
							property.Value = paramValue;
						}
					}
					else
					{
						return false;
					}
				}
			}
			AllParametersChanged(binProperties[0].Value, binProperties[1].Value, binProperties[2].Value);
			return true;
		}

		static CommonDatabase GetCommonDatabase(XBase xBase)
		{
			CommonDatabase commonDatabase = null;
			if (xBase.KauDatabaseParent != null)
			{
				commonDatabase = DatabaseManager.KauDatabases.FirstOrDefault(x => x.RootDevice == xBase.KauDatabaseParent);
			}
			else if (xBase.GkDatabaseParent != null)
			{
				commonDatabase = DatabaseManager.GkDatabases.FirstOrDefault(x => x.RootDevice == xBase.GkDatabaseParent);
			}
			return commonDatabase;
		}
	}
}