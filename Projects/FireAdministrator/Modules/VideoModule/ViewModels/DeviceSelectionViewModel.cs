using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.Models;
using RubezhClient;
using RviClient;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace VideoModule.ViewModels
{
	public class DeviceSelectionViewModel : SaveCancelDialogViewModel
	{
		public DeviceSelectionViewModel()
		{
			Title = "Устройства";
			RviServers = new List<RviServer>();
			Cameras = new List<DeviceViewModel>();
			Devices = new ObservableCollection<DeviceViewModel>();
			GetRviConfiguration();
			BuildTree();
		}

		public ObservableCollection<DeviceViewModel> Devices { get; private set; }
		public List<RviServer> RviServers { get; private set; }
		List<DeviceViewModel> Cameras { get; set; }
		protected override bool Save()
		{
			foreach (var device in Cameras)
			{
				device.Camera.IsAddedInConfiguration = device.IsChecked;
			}
			return true;
		}
		public List<Camera> GetCameras()
		{
			return Cameras.Select(x => x.Camera).ToList();
		}
		void GetRviConfiguration()
		{
			WaitHelper.Execute(() =>
			{
				var rviSettings = ClientManager.SystemConfiguration.RviSettings;
				try
				{
					RviServers = RviClientHelper.GetServers(rviSettings.Url, rviSettings.Login, rviSettings.Password, ClientManager.SystemConfiguration.Cameras);
				}
				catch
				{
					MessageBoxService.ShowWarning("Возникла ошибка при получении списка устройств");
					return;
				}
			});
		}
		void BuildTree()
		{
			foreach (var rviServer in RviServers)
			{
				var serverViewModel = new DeviceViewModel(rviServer);
				Devices.Add(serverViewModel);
				foreach (var device in rviServer.RviDevices)
				{
					var deviceViewModel = new DeviceViewModel(device);
					serverViewModel.AddChild(deviceViewModel);
					foreach (var channel in device.RviChannels)
					{
						var channelViewModel = new DeviceViewModel(channel);
						deviceViewModel.AddChild(channelViewModel);
						foreach (var camera in channel.Cameras)
						{
							var cameraViewModel = new DeviceViewModel(camera);
							channelViewModel.AddChild(cameraViewModel);
							Cameras.Add(cameraViewModel);
						}
					}
				}
			}
		}
	}
}