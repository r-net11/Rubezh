using System.Threading;
using FiresecAPI.Models;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Diagnostics;

namespace ServerFS2.Monitoring
{
	public class CommandExecutor
	{
		AutoResetEvent autoResetEvent = new AutoResetEvent(false);
		Device Device;
		string CommandName;
		bool IsReady = false;

		public CommandExecutor(Device device, string commandName)
		{
			Device = device;
			CommandName = commandName;
			Device.DeviceState.StateChanged += DeviceState_StateChanged;
			MonitoringManager.AddCommand(Device, CommandName);
			return;
			for (int i = 0; i < 10; i++)
			{
				if (IsReady)
					return;
				Thread.Sleep(TimeSpan.FromSeconds(1));
			}
			//autoResetEvent.WaitOne(TimeSpan.FromSeconds(50));
		}

		void DeviceState_StateChanged()
		{
			Trace.WriteLine("CommandExecutor DeviceState_StateChanged");
			var tableNo = MetadataHelper.GetDeviceTableNo(Device);
			var deviceId = MetadataHelper.GetIdByUid(Device.DriverUID);
			var devicePropInfo = MetadataHelper.Metadata.devicePropInfos.FirstOrDefault(x => (x.tableType == tableNo) && (x.name == CommandName));
			if (devicePropInfo.off == "0" && Device.DeviceState.States.Any(x => x.DriverState.Code == devicePropInfo.expectedState))
			{
				Trace.WriteLine("CommandExecutor Ready");
				IsReady = true;
				autoResetEvent.Set();
			}
			else if (devicePropInfo.off == "1" && !Device.DeviceState.States.Any(x => x.DriverState.Code == devicePropInfo.expectedState))
			{
				IsReady = true;
				autoResetEvent.Set();
			}
			Device.DeviceState.StateChanged -= DeviceState_StateChanged;
		}
	}
}