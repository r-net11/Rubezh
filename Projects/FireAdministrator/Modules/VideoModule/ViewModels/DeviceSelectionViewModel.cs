using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhClient;
using Infrastructure.Common.Windows.ViewModels;
using RviClient.RVIServiceReference;
using RviClient;
using Infrastructure.Common.Windows;
using RubezhAPI.Models;
using Infrastructure.Common;

namespace VideoModule.ViewModels
{
	public class DeviceSelectionViewModel : SaveCancelDialogViewModel
	{
		public DeviceSelectionViewModel()
		{
			Title = "Устройства";
			Devices = GetDevices();
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
		ObservableCollection<DeviceViewModel> GetDevices()
		{
			var result = new ObservableCollection<DeviceViewModel>();
			WaitHelper.Execute(() =>
			{
				List<Device> devices = null;
				try
				{
					devices = RviClientHelper.GetDevices(ClientManager.SystemConfiguration.RviSettings);
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
							var existingCamera = ClientManager.SystemConfiguration.Cameras.FirstOrDefault(x => x.RviDeviceUID == device.Guid && x.RviChannelNo == channel.Number && x.StreamNo == stream.Number);
							var deviceViewModel = new DeviceViewModel(device, channel, stream.Number, existingCamera == null);
							result.Add(deviceViewModel);
						}
					}
				}
			});
			return result;
		}
	}
}