using System.Collections.Generic;
using System.Linq;
using Common.GK;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Common.Windows;

namespace GKModule
{
	public static class ParametersHelper
	{
		public static void GetParameters()
		{
			DatabaseManager.Convert();
			foreach (var gkDatabase in DatabaseManager.GkDatabases)
			{
				GetParametersFromDB(gkDatabase);
			}
			ServiceFactory.SaveService.XDevicesChanged = true;
		}

		static void GetParametersFromDB(CommonDatabase commonDatabase)
		{
			LoadingService.Show("Запрос параметров", commonDatabase.BinaryObjects.Count);

			foreach (var binaryObject in commonDatabase.BinaryObjects)
			{
				var rootDevice = commonDatabase.RootDevice;
				var no = binaryObject.GetNo();
				LoadingService.DoStep("Запрос параметров объекта " + no);
				var sendResult = SendManager.Send(rootDevice, 2, 9, -1, BytesHelper.ShortToBytes(no));

				if (sendResult.HasError == false)
				{
					for (int i = 0; i < sendResult.Bytes.Count / 4; i++)
					{
						byte paramNo = sendResult.Bytes[i * 4];
						short paramValue = BytesHelper.SubstructShort(sendResult.Bytes, i * 4 + 1);

						if (binaryObject.Device != null)
						{
							var driverProperty = binaryObject.Device.Driver.Properties.FirstOrDefault(x => x.No == paramNo);
							if (driverProperty != null)
							{
								var property = binaryObject.Device.Properties.FirstOrDefault(x => x.Name == driverProperty.Name);
								if (property != null)
								{
									if (property.Value != paramValue)
										property.Value = paramValue;
								}
								else
									MessageBoxService.Show("Не найдено свойство устройства");
							}
						}
					}
				}
				//if (hasChangedProperties)
				{
					var deviceViewModel = DevicesViewModel.Current.Devices.FirstOrDefault(x => x.Device.UID == binaryObject.Device.UID);
					if (deviceViewModel != null)
					{
						deviceViewModel.UpdateProperties();
					}
				}
			}
			LoadingService.Close();
		}

		public static void SetParameters()
		{
			DatabaseManager.Convert();

			foreach (var gkDatabase in DatabaseManager.GkDatabases)
			{
				SetParametersToDB(gkDatabase);
			}
			foreach (var kauDatabase in DatabaseManager.KauDatabases)
			{
				SetParametersToDB(kauDatabase);
			}
		}

		static void SetParametersToDB(CommonDatabase commonDatabase)
		{
			LoadingService.Show("Запись параметров", commonDatabase.BinaryObjects.Count);

			foreach (var binaryObject in commonDatabase.BinaryObjects)
			{
				if (binaryObject.Device != null)
				{
					if (binaryObject.Parameters.Count > 0)
					{
						var rootDevice = commonDatabase.RootDevice;
						var no = binaryObject.GetNo();
						var bytes = new List<byte>();
						bytes.AddRange(BytesHelper.ShortToBytes(no));
						bytes.AddRange(binaryObject.Parameters);
						LoadingService.DoStep("Запись параметров объекта " + no);
						SendManager.Send(rootDevice, (short)bytes.Count, 10, 0, bytes);
					}
				}
			}

			LoadingService.Close();
		}
	}
}