using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;
using Common.GK;
using System.Threading;

namespace GKProcessor
{
	public static class ParameterUpdateHelper
	{
		public static event Action<AUParameterValue> NewAUParameterValue;
		static void OnNewAUParameterValue(AUParameterValue auParameterValue)
		{
			if (NewAUParameterValue != null)
				NewAUParameterValue(auParameterValue);
		}

		public static void UpdateDevice(XDevice device)
		{
			if (device.KauDatabaseParent != null && device.KauDatabaseParent.Driver.DriverType == XDriverType.KAU)
			{
				foreach (var auParameter in device.Driver.AUParameters)
				{
					var bytes = new List<byte>();
					var databaseNo = device.GetDatabaseNo(DatabaseType.Kau);
					bytes.Add((byte)device.Driver.DriverTypeNo);
					bytes.Add(device.IntAddress);
					bytes.Add((byte)(device.ShleifNo - 1));
					bytes.Add(auParameter.No);
					var result = SendManager.Send(device.KauDatabaseParent, 4, 131, 2, bytes);
					if (!result.HasError)
					{
						for (int i = 0; i < 100; i++)
						{
							var no = device.GetDatabaseNo(DatabaseType.Gk);
							result = SendManager.Send(device.GkDatabaseParent, 2, 12, 68, BytesHelper.ShortToBytes(no));
							if (result.Bytes.Count > 0)
							{
								var resievedParameterNo = result.Bytes[63];
								if (resievedParameterNo == auParameter.No)
								{
									var parameterValue = BytesHelper.SubstructShort(result.Bytes, 64);
									var stringValue = parameterValue.ToString();
									if (auParameter.Name == "Дата последнего обслуживания")
									{
										stringValue = (parameterValue / 256).ToString() + "." + (parameterValue % 256).ToString();
									}
									if ((device.Driver.DriverType == XDriverType.Valve || device.Driver.DriverType == XDriverType.Pump)
										&& auParameter.Name == "Режим работы")
									{
										stringValue = "Неизвестно";
										switch (parameterValue & 3)
										{
											case 0:
												stringValue = "Автоматический";
												break;

											case 1:
												stringValue = "Ручной";
												break;

											case 2:
												stringValue = "Отключено";
												break;
										}
									}
									var auParameterValue = new AUParameterValue()
									{
										Device = device,
										DriverParameter = auParameter,
										Name = auParameter.Name,
										Value = parameterValue,
										StringValue = stringValue
									};
									OnNewAUParameterValue(auParameterValue);

									break;
								}
								Thread.Sleep(100);
							}
						}
					}
				}
			}
			else if (device.KauDatabaseParent != null && device.KauDatabaseParent.Driver.DriverType == XDriverType.RSR2_KAU)
			{
				var no = device.GetDatabaseNo(DatabaseType.Gk);
				var result = SendManager.Send(device.GkDatabaseParent, 2, 12, 68, BytesHelper.ShortToBytes(no));
				if (!result.HasError)
				{
					if (result.Bytes.Count > 0)
					{
						for (int i = 0; i < device.Driver.AUParameters.Count; i++)
						{
							var auParameter = device.Driver.AUParameters[i];
							var parameterValue = BytesHelper.SubstructShort(result.Bytes, 48 + i * 2);
							var stringValue = parameterValue.ToString();
							if (auParameter.Name == "Дата последнего обслуживания")
							{
								stringValue = (parameterValue / 256).ToString() + "." + (parameterValue % 256).ToString();
							}

							var auParameterValue = new AUParameterValue()
							{
								Device = device,
								DriverParameter = auParameter,
								Name = auParameter.Name,
								Value = parameterValue,
								StringValue = stringValue
							};
							OnNewAUParameterValue(auParameterValue);
						}
					}
				}
			}
		}
	}
}