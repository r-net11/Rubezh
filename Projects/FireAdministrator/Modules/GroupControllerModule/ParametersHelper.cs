using System.Collections.Generic;
using System.Linq;
using Commom.GK;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Common.Windows;

namespace GKModule
{
	public static class ParametersHelper
	{
		public static void GetParameters()
		{
			DatabaseProcessor.Convert();
			foreach (var gkDatabase in DatabaseProcessor.DatabaseCollection.GkDatabases)
			{
				GetParametersFromDB(gkDatabase);
			}
			foreach (var kauDatabase in DatabaseProcessor.DatabaseCollection.KauDatabases)
			{
				GetParametersFromDB(kauDatabase);
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
				var bytes = SendManager.Send(rootDevice, 2, 9, -1, BytesHelper.ShortToBytes(no));

				bool hasChangedProperties = false;
				if (bytes != null)
				{
					for (int i = 0; i < bytes.Count / 4; i++)
					{
						byte paramNo = bytes[i*4];
						short paramValue = BytesHelper.SubstructShort(bytes, i*4 + 1);

						if (binaryObject.Device != null)
						{
							var driverProperty = binaryObject.Device.Driver.Properties.FirstOrDefault(x => x.No == paramNo);
							if (driverProperty != null)
							{
								var property = binaryObject.Device.Properties.FirstOrDefault(x => x.Name == driverProperty.Name);
								if (property != null)
								{
									if (property.Value != paramValue)
									{
										hasChangedProperties = true;
										property.Value = paramValue;
									}
								}
								else
								{
									MessageBoxService.Show("Не найдено свойство устройства");
								}
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
			DatabaseProcessor.Convert();

			foreach (var gkDatabase in DatabaseProcessor.DatabaseCollection.GkDatabases)
			{
				SetParametersToDB(gkDatabase);
			}
			foreach (var kauDatabase in DatabaseProcessor.DatabaseCollection.KauDatabases)
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