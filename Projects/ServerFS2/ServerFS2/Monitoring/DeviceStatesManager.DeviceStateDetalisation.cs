using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using ServerFS2.Service;
using System.Diagnostics;
using Common;
using System.Collections;
using FiresecAPI;

namespace ServerFS2.Monitoring
{
	public partial class DeviceStatesManager
	{
		void UpdateDeviceStateDetalisation(Device device)
		{
			//foreach (var deviceTable in MetadataHelper.Metadata.deviceTables)
			//{
			//    if (deviceTable.detalization != null)
			//    {
			//        foreach (var detalization in deviceTable.detalization)
			//        {
			//            if (detalization.source != null)
			//                Trace.WriteLine(detalization.source);
			//        }
			//    }
			//}

			try
			{
				if (device.Driver.DriverType == DriverType.MPT)
				{
					;
				}
				var additionalparameters = new List<Parameter>();

				var deviceTable = MetadataHelper.GetMetadataDeviceTable(device);
				if (deviceTable != null && deviceTable.detalization != null)
				{
					foreach (var metadataDetalization in deviceTable.detalization)
					{
						var stateType = (StateType)Int32.Parse(metadataDetalization.@class);
						var parameterName = "";
						switch (stateType)
						{
							case StateType.Norm:
								parameterName = "OtherMessage";
								break;

							case StateType.Failure:
								parameterName = "FailureType";
								break;

							case StateType.Fire:
								parameterName = "AlarmReason";
								break;
						}

						if (device.DeviceState.StateType == stateType)
						{
							var rawParameterValue = 0;
							var rawParameterIndex = -1;
							if (metadataDetalization.source == null)
							{
								if (metadataDetalization.stateByte != null)
								{
									switch (metadataDetalization.stateByte)
									{
										case "high":
											if (device.StateWordBytes.Count > 0)
												rawParameterValue = device.StateWordBytes[0];
											break;
									}
								}
								else
								{
									rawParameterIndex = 1;
								}
							}
							else
							{
								var splittedSources = metadataDetalization.source.Split('_');
								var source = splittedSources.Last();
								rawParameterIndex = GetRawParameterIndex(source);
							}
							if (rawParameterIndex != -1)
							{
								if (device.RawParametersBytes != null && device.RawParametersBytes.Count > rawParameterIndex)
								{
									rawParameterValue = device.RawParametersBytes[rawParameterIndex];
								}
							}

							var statusBytesArray = new byte[] { (byte)rawParameterValue };
							var bitArray = new BitArray(statusBytesArray);

							var metadataDictionary = MetadataHelper.Metadata.dictionary.FirstOrDefault(x => x.name == metadataDetalization.dictionary);
							if (metadataDictionary != null)
							{
								foreach (var matadataBit in metadataDictionary.bit)
								{
									var isMatch = false;
									if (matadataBit.no.Contains("-"))
									{
										var stringBits = matadataBit.no.Split('-');
										if (stringBits.Count() == 2)
										{
											var startBit = Int32.Parse(stringBits[0]);
											var endBit = Int32.Parse(stringBits[1]);
											var maskedParameterValue = rawParameterValue & ((1 << startBit) + (1 << endBit));
											if (maskedParameterValue == Int32.Parse(matadataBit.val))
											{
												isMatch = true;
											}
										}
									}
									else
									{
										var bitNo = Int32.Parse(matadataBit.no);
										if (bitArray[bitNo])
										{
											isMatch = true;
										}
									}
									if (isMatch)
									{
										var additionalparameter = new Parameter()
										{
											Name = parameterName,
											Value = matadataBit.value,
											Visible = true,
										};
										var driverParameter = device.Driver.Parameters.FirstOrDefault(x => x.Name == parameterName);
										if (driverParameter != null)
										{
											additionalparameter.Caption = driverParameter.Caption;
										}
										additionalparameters.Add(additionalparameter);
									}
								}
							}
						}
					}
				}

				foreach (var parameter in device.DeviceState.Parameters)
				{
					if (parameter.Name == "OtherMessage" || parameter.Name == "FailureType" || parameter.Name == "AlarmReason")
					{
						var additionalparameter = additionalparameters.FirstOrDefault(x => x.Value == parameter.Value);
						if (additionalparameter != null)
						{
							if (parameter.Value != additionalparameter.Value)
							{
								parameter.Value = additionalparameter.Value;
								HasChanges = true;
							}
						}
						else
						{
							parameter.IsDeleting = true;
							HasChanges = true;
						}
					}
				}
				foreach (var additionalparameter in additionalparameters)
				{
					var parameter = device.DeviceState.Parameters.FirstOrDefault(x => x.Value == additionalparameter.Value);
					if (parameter == null)
					{
						device.DeviceState.Parameters.Add(additionalparameter);
						HasChanges = true;
					}
				}
				device.DeviceState.Parameters.RemoveAll(x => x.IsDeleting);
			}
			catch (Exception e)
			{
				Logger.Error(e, "DeviceStatesManager.UpdateExtraDeviceState");
			}
		}

		public int GetRawParameterIndex(string value)
		{
			switch (value)
			{
				case "0x80H":
					return 0;

				case "0x80L":
					return 1;

				case "0x81H":
					return 2;

				case "0x81L":
					return 3;

				case "0x82":
					return 2;

				case "0x83":
					return 3;

				case "0x84":
					return 4;

				case "0x85":
					return 5;

				case "0x83H":
					return -1;

				case "0x83L":
					return -1;

				case "0x88H":
					return -1;

				case "0x88L":
					return -1;
			}
			return -1;
		}
	}
}