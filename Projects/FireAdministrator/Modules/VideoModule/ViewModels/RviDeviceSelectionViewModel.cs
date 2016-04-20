using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace VideoModule.ViewModels
{
	public class RviDeviceSelectionViewModel : SaveCancelDialogViewModel
	{
		public RviDeviceSelectionViewModel(List<RviServer> rviServers)
		{
			Title = "Устройства";
			RviServers = rviServers;
			Cameras = new List<RviDeviceViewModel>();
			Devices = new ObservableCollection<RviDeviceViewModel>();
			BuildTree();
		}

		public ObservableCollection<RviDeviceViewModel> Devices { get; private set; }
		public List<RviServer> RviServers { get; private set; }
		List<RviDeviceViewModel> Cameras { get; set; }
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
			RewriteRviServers();
			return true;
		}
		void RewriteRviServers()
		{
			var newRviServers = new List<RviServer>();
			foreach (var rviServer in RviServers)
			{
				var newRviDevices = new List<RviDevice>();
				foreach (var rviDevice in rviServer.RviDevices)
				{
					var newCameras = new List<Camera>();
					foreach (var camera in rviDevice.Cameras)
					{
						var cameraViewModel = Cameras.FirstOrDefault(x => x.CameraUid == camera.UID);
						if (cameraViewModel.IsChecked)
						{
							newCameras.Add(camera);
						}
					}
					if (newCameras.Count != 0)
					{
						var newRviDevice = new RviDevice { Uid = rviDevice.Uid, Ip = rviDevice.Ip, Name = rviDevice.Name, Cameras = newCameras };
						newRviDevices.Add(rviDevice);
					}
				}
				if (newRviDevices.Count != 0)
				{
					var newRviServer = new RviServer { Ip = rviServer.Ip, Port = rviServer.Port, Protocol = rviServer.Protocol, Url = rviServer.Url, RviDevices = newRviDevices };
					newRviServers.Add(newRviServer);
				}
				RviServers = newRviServers;
			}
		}
	}
}