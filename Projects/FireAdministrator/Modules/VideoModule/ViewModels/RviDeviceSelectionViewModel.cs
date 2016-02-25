using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.Models;
using RubezhClient;
using RviClient;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace VideoModule.ViewModels
{
	public class RviDeviceSelectionViewModel : SaveCancelDialogViewModel
	{
		public RviDeviceSelectionViewModel()
		{
			Title = "Устройства";
			RviServers = new List<RviServer>();
			Cameras = new List<RviDeviceViewModel>();
			Devices = new ObservableCollection<RviDeviceViewModel>();
			GetRviConfiguration();
			BuildTree();
		}

		public ObservableCollection<RviDeviceViewModel> Devices { get; private set; }
		public List<RviServer> RviServers { get; private set; }
		List<RviDeviceViewModel> Cameras { get; set; }
		void GetRviConfiguration()
		{
			var rviSettings = ClientManager.SystemConfiguration.RviSettings;
			WaitHelper.Execute(() =>
			{
				RviServers = RviClientHelper.GetServers(rviSettings.Url, rviSettings.Login, rviSettings.Password, ClientManager.SystemConfiguration.Cameras);
			});
			if (RviServers.Count == 0)
			{
				MessageBoxService.ShowWarning(string.Format("Не удалось подключиться к серверу {0}:{1}", rviSettings.Ip, rviSettings.Port));
				return;
			}
			else
			{
				var notConnectedRviServers = RviServers.Where(x => x.Status == RviStatus.ConnectionLost);
				if (notConnectedRviServers.Count() > 0)
				{
					var message = new StringBuilder("Не удалось подключиться к следующим серверам из конфигурации:\n");
					foreach (var notConnectedRviServer in notConnectedRviServers)
					{
						message.Append(string.Format("{0}:{1}\n", notConnectedRviServer.Ip, notConnectedRviServer.Port));
					}
					MessageBoxService.ShowWarning(message.ToString());
					return;
				}
			}
		}
		void BuildTree()
		{
			foreach (var rviServer in RviServers)
			{
				var serverViewModel = new RviDeviceViewModel(rviServer);
				Devices.Add(serverViewModel);
				foreach (var device in rviServer.RviDevices)
				{
					var deviceViewModel = new RviDeviceViewModel(device);
					serverViewModel.AddChild(deviceViewModel);
					foreach (var camera in device.Cameras)
					{
						var cameraViewModel = new RviDeviceViewModel(camera);
						deviceViewModel.AddChild(cameraViewModel);
						Cameras.Add(cameraViewModel);
					}
				}
			}
		}
		protected override bool Save()
		{
			foreach (var device in Cameras)
			{
				device.Camera.IsAddedInConfiguration = device.IsChecked;
			}
			return true;
		}
	}
}
