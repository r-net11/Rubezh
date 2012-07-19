using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GKModule.Database;
using System.Diagnostics;
using GKModule.ViewModels;
using Infrastructure.Common.Windows;
using Infrastructure;

namespace GKModule
{
	public static class ParametersHelper
	{
		public static void GetParameters()
		{
			DatabaseProcessor.Convert();

			foreach (var gkDatabase in DatabaseProcessor.DatabaseCollection.GkDatabases)
			{
				LoadingService.Show("Запрос параметров", gkDatabase.BinaryObjects.Count);

				foreach (var binaryObject in gkDatabase.BinaryObjects)
				{
					var rootDevice = gkDatabase.RootDevice;
					var no = binaryObject.GetNo();
					LoadingService.DoStep("Запрос параметров объекта " + no);
					var bytes = SendManager.Send(rootDevice, 2, 9, -1, BytesHelper.ShortToBytes(no));

					if (bytes != null)
					{
						for (int i = 0; i < bytes.Count / 4; i++)
						{
							byte paramNo = bytes[i];
							short paramValue = BytesHelper.SubstructShort(bytes, i + 1);

							if (binaryObject.Device != null)
							{
								var driverProperty = binaryObject.Device.Driver.Properties.FirstOrDefault(x => x.No == paramNo);
								if (driverProperty != null)
								{
									var property = binaryObject.Device.Properties.FirstOrDefault(x => x.Name == driverProperty.Name);
									if (property != null)
									{
										property.Value = paramValue;
									}
								}
								var deviceViewModel = DevicesViewModel.Current.Devices.FirstOrDefault(x=>x.Device.UID == binaryObject.Device.UID);
								if (deviceViewModel != null)
								{
									deviceViewModel.UpdateProperties();
								}
							}
						}
					}
				}
				LoadingService.Close();
			}

			ServiceFactory.SaveService.XDevicesChanged = true;
		}

		public static void SetParameters()
		{
			DatabaseProcessor.Convert();

			foreach (var gkDatabase in DatabaseProcessor.DatabaseCollection.GkDatabases)
			{
				LoadingService.Show("Запись параметров", gkDatabase.BinaryObjects.Count);

				foreach (var binaryObject in gkDatabase.BinaryObjects)
				{
					if (binaryObject.Device != null)
					{
						if (binaryObject.Parameters.Count > 0)
						{
							var rootDevice = gkDatabase.RootDevice;
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
}