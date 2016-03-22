using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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
			foreach (var device in Cameras)
			{
				device.Camera.IsAddedInConfiguration = device.IsChecked;
			}
			return true;
		}
	}
}