using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Common;
using Common.GK;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Common.Windows;
using XFiresecAPI;

namespace GKModule
{
	public static class ParametersHelper
	{
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
								MessageBoxService.ShowError("Ошибка при чтении параметра устройства " + binaryObject.Device.ShortPresentationAddressAndDriver);
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
			ServiceFactory.SaveService.GKChanged = true;
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
							if (!result)
							{
								MessageBoxService.ShowError("Ошибка при чтении параметра устройства " + binaryObject.Device.ShortPresentationAddressAndDriver);
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
			DatabaseManager.Convert();
			LoadingService.Show("Запись параметров", 1);
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
						if (!result)
						{
							MessageBoxService.ShowError("Ошибка при записи параметра устройства " + device.ShortPresentationAddressAndDriver);
						}
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "ParametersHelper.SetSingleParameter");
			}
			finally
			{
				LoadingService.Close();
			}
		}

		public static void GetSingleParameter(XDevice device)
		{
			DatabaseManager.Convert();
			LoadingService.Show("Запрос параметров", 1);
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
							MessageBoxService.ShowError("Ошибка при получении параметра устройства " + device.ShortPresentationAddressAndDriver);
						}
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "ParametersHelper.SetSingleParameter");
			}
			finally
			{
				LoadingService.Close();
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
							paramValue = (ushort)(paramValue >> 8);
							paramValue = (ushort)(paramValue << 8);
						}
						paramValue = (ushort)(paramValue >> driverProperty.Offset);
						paramValue = (byte)(paramValue & driverProperty.Mask);
						if (driverProperty.Multiplier != 0)
						{
							paramValue = (ushort)((double)paramValue / driverProperty.Multiplier);
						}
						var property = binaryObject.Device.Properties.FirstOrDefault(x => x.Name == driverProperty.Name);
						if (property != null)
						{
							if (property.Value != paramValue)
								property.Value = paramValue;
						}
					}
				}
			}
			var deviceViewModel = DevicesViewModel.Current.AllDevices.FirstOrDefault(x => x.Device.UID == binaryObject.Device.UID);
			if (deviceViewModel != null)
			{
				deviceViewModel.UpdateProperties();
			}
			return true;
		}

		static bool SetDeviceParameters(CommonDatabase commonDatabase, BinaryObjectBase binaryObject)
		{
			if (binaryObject.Device != null)
			{
				foreach (var property in binaryObject.Device.Properties)
				{
					var driverProperty = binaryObject.Device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
					if (driverProperty != null)
					{
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
				return !sendResult.HasError;
			}
			return true;
		}
	}

	class BinProperty
	{
		public byte ParamNo { get; set; }
		public ushort ParamValue { get; set; }
	}
}