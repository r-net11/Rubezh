using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using RviClient.RVIServiceReference;
using RviClient;

namespace VideoModule.ViewModels
{
	public class DeviceSelectionViewModel : SaveCancelDialogViewModel
	{
		public DeviceSelectionViewModel()
		{
			Title = "Устройства";
			Devices = new ObservableCollection<DeviceViewModel>();

			var devices = RviClientHelper.GetDevices(FiresecManager.SystemConfiguration);
			foreach (var device in devices)
			{
				foreach (var channel in device.Channels)
				{
					foreach (var stream in channel.Streams)
					{
						var deviceViewModel = new DeviceViewModel(device, channel, stream.Number);
						var camera = FiresecManager.SystemConfiguration.Cameras.FirstOrDefault(x => x.RviDeviceUID == device.Guid && x.RviChannelNo == channel.Number && x.StreamNo == stream.Number);
						if (camera != null)
						{
							deviceViewModel.IsChecked = true;
						}
						Devices.Add(deviceViewModel);
					}
				}
			}
		}

		public ObservableCollection<DeviceViewModel> Devices { get; private set; }

		protected override bool Save()
		{
			var cameras = new List<FiresecAPI.Models.Camera>();
			foreach (var device in Devices)
			{
				if (device.IsChecked)
				{
					var camera = FiresecManager.SystemConfiguration.Cameras.FirstOrDefault(x => x.RviDeviceUID == device.Device.Guid && x.RviChannelNo == device.Channel.Number && x.StreamNo == device.StreamNo);
					if (camera == null)
						camera = new FiresecAPI.Models.Camera();
					if (device.Channel.Streams.Count() < device.StreamNo)
						return true;
					var stream = device.Channel.Streams[device.StreamNo - 1];
					if (stream != null)
					{
						camera.Name = device.DeviceName;
						camera.StreamNo = device.StreamNo;
						camera.Ip = device.Device.Ip;
						camera.RviDeviceUID = device.Device.Guid;
						camera.RviChannelNo = device.Channel.Number;
						camera.RviRTSP = stream.Rtsp;
						camera.CountPresets = device.Channel.CountPresets;
						camera.CountTemplateBypass = device.Channel.CountTemplateBypass;
						camera.CountTemplatesAutoscan = device.Channel.CountTemplatesAutoscan;
					}

					if (!cameras.Contains(camera))
						cameras.Add(camera);
				}
			}
			FiresecManager.SystemConfiguration.Cameras = cameras;
			ServiceFactory.SaveService.CamerasChanged = true;
			return true;
		}
	}
}
