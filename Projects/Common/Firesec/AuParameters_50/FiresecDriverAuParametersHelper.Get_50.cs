using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using Common;
using FiresecAPI;
using FiresecAPI.Models;

namespace Firesec_50
{
	public static partial class FiresecDriverAuParametersHelper
	{
		public static event Action<string, int> Progress;
		static void OnPropgress(string value, int percentsCompleted)
		{
			if (Progress != null)
				Progress(value, percentsCompleted);
		}

		public static Firesec.FiresecSerializedClient FiresecSerializedClient { get; set; }
		static List<DevicePropertyRequest> DevicePropertyRequests = new List<DevicePropertyRequest>();
		static Thread AUParametersThread;
		static AutoResetEvent StopEvent;

		static FiresecDriverAuParametersHelper()
		{
			Dispatcher.CurrentDispatcher.ShutdownStarted += (s, e) =>
			{
				StopAUParametersThread();
			};
		}

		static void StopAUParametersThread()
		{
			if (StopEvent != null)
			{
				StopEvent.Set();
			}
			if (AUParametersThread != null)
			{
				AUParametersThread.Join(TimeSpan.FromSeconds(5));
			}
			AUParametersThread = null;
		}

		public static void BeginGetAuParameters(List<Device> devices)
		{
			StopAUParametersThread();
			StopEvent = new AutoResetEvent(false);
			AUParametersThread = new Thread(() => { GetAuParameters(devices); });
			AUParametersThread.Start();
		}

        static void GetAuParameters(List<Device> devices)
        {
            string errorMessage = "";
            for (int i = 0; i < devices.Count; i++)
            {
                var device = devices[i];
                OnPropgress("Чтение параметров устройства " + device.DottedPresentationNameAndAddress, (i * 100) / devices.Count);
                var addedDevicePropertyRequest = new DevicePropertyRequest(device);
                foreach (var propertyNo in addedDevicePropertyRequest.PropertyNos)
                {
                    int requestId = 0;
                    var result = FiresecSerializedClient.ExecuteRuntimeDeviceMethod(device.PlaceInTree, "Device$ReadSimpleParam", propertyNo.ToString(), ref requestId);
                    if (result.HasError)
                    {
                        AUParametersThread = null;
                        return;
                    }
                    var requestInfo = new Firesec_50.DevicePropertyRequest.RequestInfo()
                    {
                        ParamNo = propertyNo,
                        RequestId = requestId
                    };
                    addedDevicePropertyRequest.RequestIds.Add(requestInfo);
                }
                DevicePropertyRequests.Add(addedDevicePropertyRequest);

                try
                {
                    var lastActivityTime = DateTime.Now;
                    while (DevicePropertyRequests.Count > 0)
                    {
                        DevicePropertyRequests.RemoveAll(x => x.IsDeleting);
                        var devicePropertyRequests = DevicePropertyRequests.ToList();

                        int stateConfigQueriesRequestId = 0;
                        var result = FiresecSerializedClient.ExecuteRuntimeDeviceMethod("", "StateConfigQueries", null, ref stateConfigQueriesRequestId);
                        if (result == null || result.HasError || result.Result == null)
                        {
                            Thread.Sleep(TimeSpan.FromSeconds(1));
                            continue;
                        }

                        var stringResult = result.Result;
                        stringResult = ClearString(stringResult);

                        Firesec.Models.DeviceCustomFunctions_50.Requests requests = Firesec.SerializerHelper.Deserialize<Firesec.Models.DeviceCustomFunctions_50.Requests>(stringResult);
                        foreach (var request in requests.Request)
                        {
                            foreach (var devicePropertyRequest in devicePropertyRequests)
                            {
                                if (devicePropertyRequest.RequestIds.Any(x => x.RequestId == request.ID))
                                {
                                    if (request.State != "5")
                                        continue;

                                    var resultString = request.resultString;
                                    if (resultString == null)
                                        continue;

                                    var requestInfo = devicePropertyRequest.RequestIds.FirstOrDefault(x => x.RequestId == request.ID);
                                    if (requestInfo == null)
                                        continue;
                                    devicePropertyRequest.RequestIds.RemoveAll(x => x.RequestId == request.ID);
                                    int propertyNo = requestInfo.ParamNo;
                                    int propertyValue = 0;
                                    try
                                    {
                                        if (!string.IsNullOrEmpty(resultString))
                                            propertyValue = int.Parse(resultString);
                                    }
                                    catch { ;}

                                    foreach (var driverProperty in devicePropertyRequest.Device.Driver.Properties.FindAll(x => x.No == propertyNo))
                                    {
                                        if (devicePropertyRequest.Properties.FirstOrDefault(x => x.Name == driverProperty.Name) == null)
                                        {
                                            devicePropertyRequest.Properties.Add(CreateProperty(propertyValue, driverProperty));
                                            OnPropgress("Чтение параметров устройства " + device.DottedPresentationNameAndAddress + " (" + driverProperty.Caption + ")", (i * 100) / devices.Count);
                                            lastActivityTime = DateTime.Now;
                                        }
                                    }
                                }
                            }

                            foreach (var devicePropertyRequest in devicePropertyRequests)
                            {
                                if (devicePropertyRequest.RequestIds.Count == 0)
                                {
                                    foreach (var resultProperty in devicePropertyRequest.Properties)
                                    {
                                        var property = devicePropertyRequest.Device.DeviceAUProperties.FirstOrDefault(x => x.Name == resultProperty.Name);
                                        if (property == null)
                                        {
                                            property = new Property()
                                            {
                                                Name = resultProperty.Name
                                            };
                                            devicePropertyRequest.Device.DeviceAUProperties.Add(property);
                                        }
                                        property.Value = resultProperty.Value;
                                    }
                                    devicePropertyRequest.Device.OnAUParametersChanged();
                                }
                            }
                        }
                        if ((DateTime.Now - lastActivityTime).Seconds > 30)
                        {
                            errorMessage += device.DottedPresentationNameAndAddress + " ,";
                                break;
                        }
                        if (StopEvent.WaitOne(1000))
                            break;
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e, "FiresecDriverAuParametersHelper.AUParametersThreadRun");
                }
            }
            if (errorMessage != "")
            {
                errorMessage = "Ошибка при чтении устройств " + errorMessage;
                if (errorMessage.EndsWith(" ,"))
                    errorMessage = errorMessage.Remove(errorMessage.Length - 2, 2);
                OnPropgress(errorMessage, 0);
            }
            else
            {
                OnPropgress("Готово", 0);
            }
            AUParametersThread = null;
        }

		static Property CreateProperty(int paramValue, DriverProperty driverProperty)
		{
			var realParamValue = paramValue;

			var highByteValue = paramValue / 256;
			var lowByteValue = paramValue - highByteValue * 256;

			if (driverProperty.HighByte)
				realParamValue = highByteValue;
			else if (driverProperty.LargeValue)
				realParamValue = paramValue;
			else
				realParamValue = lowByteValue;

			if (driverProperty.Caption == "Проигрываемое сообщение")
			{
				return MRO2Helper.GetMessageNumber(realParamValue);
			}

			if (driverProperty.MinBit > 0)
			{
				byte byteOffsetParamValue = (byte)realParamValue;
				byteOffsetParamValue = (byte)(byteOffsetParamValue >> driverProperty.MinBit);
				byteOffsetParamValue = (byte)(byteOffsetParamValue << driverProperty.MinBit);
				realParamValue = byteOffsetParamValue;
			}

			if (driverProperty.MaxBit > 0)
			{
				byte byteOffsetParamValue = (byte)realParamValue;
				byteOffsetParamValue = (byte)(byteOffsetParamValue << 8 - driverProperty.MaxBit);
				byteOffsetParamValue = (byte)(byteOffsetParamValue >> 8 - driverProperty.MaxBit);
				realParamValue = byteOffsetParamValue;
			}

			if (driverProperty.BitOffset > 0)
			{
				realParamValue = realParamValue >> driverProperty.BitOffset;
			}

			if (driverProperty.Caption == "Задержка включения МРО, с")
			{
				realParamValue = realParamValue * 5;
			}

			if (driverProperty.Multiplier > 0)
			{
				realParamValue = (int)(realParamValue / driverProperty.Multiplier);
			}

			var property = new Property()
			{
				Name = driverProperty.Name,
				Value = realParamValue.ToString()
			};

			return property;
		}

		static string ClearString(string value)
		{
			for (int a = 0; a < 100; a++)
			{
				int startIndex = -1;
				int cdataLength = -1;
				var length = value.Length;
				for (int i = 0; i < value.Length; i++)
				{
					if (i < length - 9)
					{
						if (value.Substring(i, 9) == "<![CDATA[")
						{
							var cdataEndIndex = value.Substring(i + 9).IndexOf("]");
							if (cdataEndIndex > 10)
							{
								startIndex = i + 9;
								cdataLength = cdataEndIndex;
								break;
							}
						}
					}
				}
				if (startIndex != -1)
				{
					value = value.Remove(startIndex, cdataLength);
				}
				else
				{
					break;
				}
			}
			return value;
		}
	}
}