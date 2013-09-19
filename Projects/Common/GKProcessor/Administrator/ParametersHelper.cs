using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.GK;
using Infrastructure;
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
				LoadingService.Show("Запрос параметров", kauDatabase.BinaryObjects.Count);
				try
				{
					foreach (var binaryObject in kauDatabase.BinaryObjects)
					{
						if (binaryObject.Device != null)
						{
							var result = GetDeviceParameters(kauDatabase, binaryObject);
							if (!result)
							{
								MessageBoxService.ShowError("Ошибка при чтении параметра устройства " + binaryObject.Device.PresentationDriverAndAddress);
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
				LoadingService.Show("Запись параметров", kauDatabase.BinaryObjects.Count);
				try
				{
					foreach (var binaryObject in kauDatabase.BinaryObjects)
					{
						if (binaryObject.Device != null)
						{
							var result = SetDeviceParameters(kauDatabase, binaryObject);
							if (!string.IsNullOrEmpty(result))
							{
								MessageBoxService.ShowError("Ошибка при записи параметра устройства " + binaryObject.Device.PresentationDriverAndAddress + "\n" + result);
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
				CommonDatabase commonDatabase = null;
				if (device.KauDatabaseParent != null)
				{
					commonDatabase = DatabaseManager.KauDatabases.FirstOrDefault(x => x.RootDevice == device.KauDatabaseParent);
				}
				else if (device.GkDatabaseParent != null)
				{
					commonDatabase = DatabaseManager.GkDatabases.FirstOrDefault(x => x.RootDevice == device.GkDatabaseParent);
				}
				if (commonDatabase != null)
				{
					var binaryObject = commonDatabase.BinaryObjects.FirstOrDefault(x => x.Device == device);
					if (binaryObject != null)
					{
						var result = SetDeviceParameters(commonDatabase, binaryObject);
						if (!string.IsNullOrEmpty(result))
						{
							MessageBoxService.ShowError("Ошибка при записи параметра устройства " + device.PresentationDriverAndAddress + "\n" + result);
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
				CommonDatabase commonDatabase = null;
				if (device.KauDatabaseParent != null)
				{
					commonDatabase = DatabaseManager.KauDatabases.FirstOrDefault(x => x.RootDevice == device.KauDatabaseParent);
				}
				else if (device.GkDatabaseParent != null)
				{
					commonDatabase = DatabaseManager.GkDatabases.FirstOrDefault(x => x.RootDevice == device.GkDatabaseParent);
				}
				if (commonDatabase != null)
				{
					var binaryObject = commonDatabase.BinaryObjects.FirstOrDefault(x => x.Device == device);
					if (binaryObject != null)
					{
						var result = GetDeviceParameters(commonDatabase, binaryObject);
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

		static bool GetDeviceParameters(CommonDatabase commonDatabase, BinaryObjectBase binaryObject)
		{
			var no = binaryObject.GetNo();
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
					ParamNo = paramNo,
					ParamValue = paramValue
				};
				binProperties.Add(binProperty);
			}

			if (binaryObject.Device != null)
			{
				foreach (var driverProperty in binaryObject.Device.Driver.Properties)
				{
					if (!driverProperty.IsAUParameter)
						continue;

					var binProperty = binProperties.FirstOrDefault(x => x.ParamNo == driverProperty.No);
					if (binProperty != null)
					{
						var paramValue = (ushort)binProperty.ParamValue;
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
						var property = binaryObject.Device.Properties.FirstOrDefault(x => x.Name == driverProperty.Name);
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
			binaryObject.Device.OnAUParametersChanged();
			return true;
		}
		static string SetDeviceParameters(CommonDatabase commonDatabase, BinaryObjectBase binaryObject)
		{
			if (binaryObject.Device != null)
			{
				foreach (var property in binaryObject.Device.Properties)
				{
					var driverProperty = binaryObject.Device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
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

			if (binaryObject.Parameters.Count > 0)
			{
				var rootDevice = commonDatabase.RootDevice;
				var no = binaryObject.GetNo();
				var bytes = new List<byte>();
				bytes.AddRange(BytesHelper.ShortToBytes(no));
				bytes.AddRange(binaryObject.Parameters);
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
				CommonDatabase commonDatabase = null;
				if (direction.KauDatabaseParent != null)
				{
					commonDatabase = DatabaseManager.KauDatabases.FirstOrDefault(x => x.RootDevice == direction.KauDatabaseParent);
				}
				else if (direction.GkDatabaseParent != null)
				{
					commonDatabase = DatabaseManager.GkDatabases.FirstOrDefault(x => x.RootDevice == direction.GkDatabaseParent);
				}
				if (commonDatabase != null)
				{
					var binaryObject = commonDatabase.BinaryObjects.FirstOrDefault(x => x.Direction == direction);
					if (binaryObject != null)
					{
						var result = SetDirectionParameters(commonDatabase, binaryObject);
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
		static string SetDirectionParameters(CommonDatabase commonDatabase, BinaryObjectBase binaryObject)
		{
			if (binaryObject.Device != null)
			{
				foreach (var property in binaryObject.Device.Properties)
				{
					var driverProperty = binaryObject.Device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
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

			if (binaryObject.Parameters.Count > 0)
			{
				var rootDevice = commonDatabase.RootDevice;
				var no = binaryObject.GetNo();
				var bytes = new List<byte>();
				bytes.AddRange(BytesHelper.ShortToBytes(no));
				bytes.AddRange(binaryObject.Parameters);
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
				CommonDatabase commonDatabase = null;
				if (direction.KauDatabaseParent != null)
				{
					commonDatabase = DatabaseManager.KauDatabases.FirstOrDefault(x => x.RootDevice == direction.KauDatabaseParent);
				}
				else if (direction.GkDatabaseParent != null)
				{
					commonDatabase = DatabaseManager.GkDatabases.FirstOrDefault(x => x.RootDevice == direction.GkDatabaseParent);
				}
				if (commonDatabase != null)
				{
					var binaryObject = commonDatabase.BinaryObjects.FirstOrDefault(x => x.Direction == direction);
					if (binaryObject != null)
					{
						var result = GetDirectionParameters(commonDatabase, binaryObject, direction);
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
		static bool GetDirectionParameters(CommonDatabase commonDatabase, BinaryObjectBase binaryObject, XDirection direction)
		{
			var no = binaryObject.GetNo();
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
					ParamNo = paramNo,
					ParamValue = paramValue
				};
				binProperties.Add(binProperty);
			}

			if (binaryObject.Device != null)
			{
				foreach (var driverProperty in binaryObject.Device.Driver.Properties)
				{
					if (!driverProperty.IsAUParameter)
						continue;

					var binProperty = binProperties.FirstOrDefault(x => x.ParamNo == driverProperty.No);
					if (binProperty != null)
					{
						var paramValue = (ushort)binProperty.ParamValue;
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
						var property = binaryObject.Device.Properties.FirstOrDefault(x => x.Name == driverProperty.Name);
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
			AllParametersChanged(binProperties[0].ParamValue, binProperties[1].ParamValue, binProperties[2].ParamValue);
			return true;
		}
	}

	class BinProperty
	{
		public byte ParamNo { get; set; }
		public ushort ParamValue { get; set; }
	}
}