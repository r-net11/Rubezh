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
		void UpdateExtraDeviceState(Device device)
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
							var rawParameterIndex = -1;
							if (metadataDetalization.source == null)
								rawParameterIndex = 1;
							else
							{
								var splittedSources = metadataDetalization.source.Split('_');
								var source = splittedSources.Last();
								switch (source)
								{
									case "0x80H":
										rawParameterIndex = 0;
										break;

									case "0x80L":
										rawParameterIndex = 1;
										break;

									case "0x81H":
										rawParameterIndex = 2;
										break;

									case "0x81L":
										rawParameterIndex = 3;
										break;

									case "0x82":
										rawParameterIndex = 2;
										break;

									case "0x83":
										rawParameterIndex = 3;
										break;

									case "0x84":
										rawParameterIndex = 4;
										break;

									case "0x85":
										rawParameterIndex = 5;
										break;

									case "0x83H":
										rawParameterIndex = -1;
										break;

									case "0x83L":
										rawParameterIndex = -1;
										break;

									case "0x88H":
										rawParameterIndex = -1;
										break;

									case "0x88L":
										rawParameterIndex = -1;
										break;
								}
							}
							if (rawParameterIndex != -1)
							{
								if (device.RawParametersBytes != null && device.RawParametersBytes.Count > rawParameterIndex)
								{
									var rawParameterValue = device.RawParametersBytes[rawParameterIndex];
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
													if(maskedParameterValue == Int32.Parse(matadataBit.val))
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
													Visible = true
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
	}
}