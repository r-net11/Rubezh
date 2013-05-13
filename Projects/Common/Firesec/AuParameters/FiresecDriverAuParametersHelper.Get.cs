using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using FiresecAPI;
using FiresecAPI.Models;
using Common;

namespace Firesec
{
	public static partial class FiresecDriverAuParametersHelper
	{
		public static FiresecSerializedClient FiresecSerializedClient { get; set; }
		static List<DevicePropertyRequest> DevicePropertyRequests = new List<DevicePropertyRequest>();
		static Thread AUParametersThread;
		static AutoResetEvent StopEvent;

		static FiresecDriverAuParametersHelper()
		{
			Dispatcher.CurrentDispatcher.ShutdownStarted += (s, e) =>
			{
				if (StopEvent != null)
				{
					StopEvent.Set();
				}
				if (AUParametersThread != null)
				{
					AUParametersThread.Join(TimeSpan.FromSeconds(1));
				}
			};
		}

		public static OperationResult<bool> BeginGetConfigurationParameters(Device device)
		{
			var devicePropertyRequest = new DevicePropertyRequest(device);
			foreach (var propertyNo in devicePropertyRequest.PropertyNos)
			{
				int requestId = 0;
				var result = FiresecSerializedClient.ExecuteRuntimeDeviceMethod(device.PlaceInTree, "Device$ReadSimpleParam", propertyNo.ToString(), ref requestId);
				if (result.HasError)
				{
					return new OperationResult<bool>(result.Error);
				}
				Trace.WriteLine(requestId.ToString() + " " + device.PlaceInTree);
				devicePropertyRequest.RequestIds.Add(requestId);
			}
			DevicePropertyRequests.Add(devicePropertyRequest);

			if (AUParametersThread == null)
			{
				StopEvent = new AutoResetEvent(false);
				AUParametersThread = new Thread(AUParametersThreadRun);
				AUParametersThread.Start();
			}
			return new OperationResult<bool>() { Result = true };
		}

		static void AUParametersThreadRun()
		{
            while (DevicePropertyRequests.Count > 0)
            {
                try
                {
                    DevicePropertyRequests.RemoveAll(x => x.IsDeleting);
                    var devicePropertyRequests = DevicePropertyRequests.ToList();
                    Trace.WriteLine("devicePropertyRequests.Count = " + devicePropertyRequests.Count().ToString());

                    int stateConfigQueriesRequestId = 0;
                    var result = FiresecSerializedClient.ExecuteRuntimeDeviceMethod("", "StateConfigQueries", null, ref stateConfigQueriesRequestId);
                    if (result == null || result.HasError || result.Result == null)
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(1));
                        continue;
                    }

                    Firesec.Models.DeviceCustomFunctions.requests requests = SerializerHelper.Deserialize<Firesec.Models.DeviceCustomFunctions.requests>(result.Result);
                    if (requests != null && requests.request.Count() > 0)
                    {
                        foreach (var request in requests.request)
                        {
                            foreach (var devicePropertyRequest in devicePropertyRequests)
                            {
                                if (devicePropertyRequest.RequestIds.Contains(request.id))
                                {
                                    devicePropertyRequest.RequestIds.Remove(request.id);
                                    int propertyNo = request.param.FirstOrDefault(x => x.name == "ParamNo").value;
                                    int propertyValue = request.param.FirstOrDefault(x => x.name == "ParamValue").value;
                                    if (propertyNo == 0x87)
                                    {
                                        ;
                                    }
                                    foreach (var driverProperty in devicePropertyRequest.Device.Driver.Properties.FindAll(x => x.No == propertyNo))
                                    {
                                        if (devicePropertyRequest.Properties.FirstOrDefault(x => x.Name == driverProperty.Name) == null)
                                        {
                                            
                                            devicePropertyRequest.Properties.Add(CreateProperty(propertyValue, driverProperty));
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
                                        var property = devicePropertyRequest.Device.Properties.FirstOrDefault(x => x.Name == resultProperty.Name);
                                        if (property == null)
                                        {
                                            property = new Property()
                                            {
                                                Name = resultProperty.Name
                                            };
                                            devicePropertyRequest.Device.Properties.Add(property);
                                        }
                                        property.Value = resultProperty.Value;
                                    }
                                    devicePropertyRequest.Device.OnAUParametersChanged();
                                }
                            }
                        }
                    }
                    if (StopEvent.WaitOne(1000))
                        break;
                }
                catch (Exception e)
                {
                    Logger.Error(e, "FiresecDriverAuParametersHelper.AUParametersThreadRun.while");
                }
            }
			AUParametersThread = null;
		}

		static Property CreateProperty(int paramValue, DriverProperty driverProperty)
		{
			var offsetParamValue = paramValue;

			var highByteValue = paramValue / 256;
			var lowByteValue = paramValue - highByteValue * 256;

			if (driverProperty.HighByte)
				offsetParamValue = highByteValue;
			else if (driverProperty.LargeValue)
				offsetParamValue = paramValue;
			else
				offsetParamValue = lowByteValue;

			if (driverProperty.Caption == "Проигрываемое сообщение")
			{
				return MRO2Helper.GetMessageNumber(offsetParamValue);
			}

			if (driverProperty.MinBit > 0)
			{
				byte byteOffsetParamValue = (byte)offsetParamValue;
				byteOffsetParamValue = (byte)(byteOffsetParamValue >> driverProperty.MinBit);
				byteOffsetParamValue = (byte)(byteOffsetParamValue << driverProperty.MinBit);
				offsetParamValue = byteOffsetParamValue;
			}

			if (driverProperty.MaxBit > 0)
			{
				byte byteOffsetParamValue = (byte)offsetParamValue;
				byteOffsetParamValue = (byte)(byteOffsetParamValue << 8 - driverProperty.MaxBit);
				byteOffsetParamValue = (byte)(byteOffsetParamValue >> 8 - driverProperty.MaxBit);
				offsetParamValue = byteOffsetParamValue;
			}

			if (driverProperty.BitOffset > 0)
			{
				offsetParamValue = offsetParamValue >> driverProperty.BitOffset;
			}

			if (driverProperty.Caption == "Задержка включения МРО, с")
			{
				offsetParamValue = offsetParamValue * 5;
			}

			var property = new Property()
			{
				Name = driverProperty.Name,
				Value = offsetParamValue.ToString()
			};

			return property;
		}
	}
}