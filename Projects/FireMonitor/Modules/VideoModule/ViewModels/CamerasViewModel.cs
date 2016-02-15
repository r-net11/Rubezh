using Infrastructure.Common.Windows.ViewModels;
using RubezhClient;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace VideoModule.ViewModels
{
	public class CamerasViewModel : ViewPartViewModel, ISelectable<Guid>
	{
		public static CamerasViewModel Current { get; private set; }
		public CamerasViewModel()
		{
			Current = this;
			Initialize();
		}

		public void Initialize()
		{
			Cameras = new ObservableCollection<CameraViewModel>();
			BuildTree();
			SelectedCamera = Cameras.FirstOrDefault();
		}

		public ObservableCollection<CameraViewModel> Cameras { get; private set; }

		CameraViewModel _selectedCamera;
		public CameraViewModel SelectedCamera
		{
			get { return _selectedCamera; }
			set
			{
				_selectedCamera = value;
				if (value != null)
					value.ExpandToThis();
				OnPropertyChanged(() => SelectedCamera);
			}
		}
		public void Select(Guid cameraUID)
		{
			if (cameraUID != Guid.Empty)
				SelectedCamera = Cameras.FirstOrDefault(x => x.Camera.UID == cameraUID);
		}
		void BuildTree()
		{
			var servers = ClientManager.SystemConfiguration.RviServers;
			foreach (var server in servers)
			{
				var serverViewModel = new CameraViewModel(server);
				foreach (var device in server.RviDevices)
				{
					var deviceViewModel = new CameraViewModel(device);
					foreach (var channel in device.RviChannels)
					{
						var channelViewModel = new CameraViewModel(channel.Name);
						foreach (var stream in channel.Cameras)
						{
							if (stream.IsAddedInConfiguration)
							{
								var streamViewModel = new CameraViewModel(stream.Name, stream);
								if (!channelViewModel.Children.Contains(streamViewModel))
								{
									channelViewModel.AddChild(streamViewModel);
									if (!deviceViewModel.Children.Contains(channelViewModel))
									{
										deviceViewModel.AddChild(channelViewModel);
										if (!serverViewModel.Children.Contains(deviceViewModel))
										{
											serverViewModel.AddChild(deviceViewModel);
											if (!Cameras.Contains(serverViewModel))
											{
												Cameras.Add(serverViewModel);
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}
}