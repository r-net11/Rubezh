using System.Threading;
using FiresecAPI.Models;

namespace ServerFS2.Monitoring
{
	public class CommandExecutor
	{
		const int expiredTime = 10000;
		AutoResetEvent autoResetEvent = new AutoResetEvent(false);
		Device Device;
		string CommandName;
		Thread checkerThread;


		public CommandExecutor(Device device, string commandName)
		{
			Device = device;
			CommandName = commandName;
			//var task = Task.Factory.StartNew(() =>
			//{
			//    Device.DeviceState.StateChanged += DeviceState_StateChanged;
			//});
			MonitoringProcessor.AddCommand(Device, CommandName);
			//autoResetEvent.WaitOne(expiredTime);
		}

		void DeviceState_StateChanged()
		{
			//var tableNo = MetadataHelper.GetDeviceTableNo(Device);
			//var deviceId = MetadataHelper.GetIdByUid(Device.DriverUID);
			//var devicePropInfo = MetadataHelper.Metadata.devicePropInfos.FirstOrDefault(x => (x.tableType == tableNo) && (x.name == CommandName));
			//if (devicePropInfo.off == "0" && Device.DeviceState.States.Any(x => x.DriverState.Id == devicePropInfo.expectedState))
				autoResetEvent.Set();
				Device.DeviceState.StateChanged -= DeviceState_StateChanged;
			//else if (devicePropInfo.off == "1" && Device.DeviceState.States.Any(x => x.DriverState.Id != devicePropInfo.expectedState))
			//        autoResetEvent.Set();
		}

	}
}
