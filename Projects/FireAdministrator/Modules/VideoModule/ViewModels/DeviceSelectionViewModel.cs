using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
using Localization.Video.Errors;
using Localization.Video.ViewModels;
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
			Title = CommonViewModels.Devices;
			Devices = new ObservableCollection<DeviceViewModel>();

			List<IRviDevice> devices = null;
			try
			{
				devices = RviClientHelper.GetDevices(FiresecManager.SystemConfiguration);
			}
			catch
			{
				MessageBoxService.ShowWarning(CommonErrors.GetListDevice_Error);
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
			foreach (var device in Devices.Where(x => x.IsChecked))
			{
				var stream = device.Channel.Streams.FirstOrDefault(x => x.Number == device.StreamNo);
				if (stream != null)
				{
					var camera = new Camera
					{
						Name = device.DeviceName,
						StreamNo = device.StreamNo,
						Ip = device.Device.Ip,
						RviDeviceUID = device.Device.Guid,
						RviChannelNo = device.Channel.Number,
						RviRTSP = stream.Rtsp,
						RviChannelName = device.Channel.Name,
						CountPresets = device.Channel.CountPresets,
						CountTemplateBypass = device.Channel.CountTemplateBypass,
						CountTemplatesAutoscan = device.Channel.CountTemplatesAutoscan
					};
					cameras.Add(camera);
				}
			}
			return cameras;
		}
	}
}