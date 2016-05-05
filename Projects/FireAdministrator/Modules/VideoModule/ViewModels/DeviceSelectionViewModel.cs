using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using RviClient;
using Infrastructure.Common.Windows;
using StrazhAPI.Models;
using RviCommonClient;

namespace VideoModule.ViewModels
{
	public class DeviceSelectionViewModel : SaveCancelDialogViewModel
	{
		public DeviceSelectionViewModel()
		{
			Title = "Устройства";
			Devices = new ObservableCollection<DeviceViewModel>();

			List<IRviDevice> devices = null;
			try
			{
				devices = RviClientHelper.GetDevices(FiresecManager.SystemConfiguration);
			}
			catch
			{
				MessageBoxService.ShowWarning("Возникла ошибка при получении списка устройств");
				return;
			}
			foreach (var device in devices)
			{
				foreach (var channel in device.Channels)
				{
					foreach (var stream in channel.Streams)
					{
						var existingCamera = FiresecManager.SystemConfiguration.Cameras.FirstOrDefault(x => x.RviDeviceUID == device.Guid && x.RviChannelNo == channel.Number && x.StreamNo == stream.Number);
						var deviceViewModel = new DeviceViewModel(device, channel, stream.Number, existingCamera == null);
						Devices.Add(deviceViewModel);
					}
				}
			}
		}

		public ObservableCollection<DeviceViewModel> Devices { get; private set; }

		protected override bool Save()
		{
			return true;
		}

		public List<Camera> GetCameras()
		{
			var cameras = new List<Camera>();
			foreach (var device in Devices)
			{
				if (device.IsChecked)
				{
					var stream = device.Channel.Streams[device.StreamNo];
					if (stream != null)
					{
						var camera = new Camera();
						camera.Name = device.DeviceName;
						camera.StreamNo = device.StreamNo;
						camera.Ip = device.Device.Ip;
						camera.RviDeviceUID = device.Device.Guid;
						camera.RviChannelNo = device.Channel.Number;
						camera.RviRTSP = stream.Rtsp;
						camera.RviChannelName = device.Channel.Name;
						camera.CountPresets = device.Channel.CountPresets;
						camera.CountTemplateBypass = device.Channel.CountTemplateBypass;
						camera.CountTemplatesAutoscan = device.Channel.CountTemplatesAutoscan;
						cameras.Add(camera);
					}
				}
			}
			return cameras;
		}
	}
}