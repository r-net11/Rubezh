using Infrastructure.Common.TreeList;
using RubezhAPI.Models;
using System;

namespace VideoModule.ViewModels
{
	public class DeviceViewModel : TreeNodeViewModel<DeviceViewModel>
	{
		public bool IsEnabled { get; set; }
		public string Name { get; private set; }
		public string Address { get; private set; }
		public Guid CameraUid { get; private set; }
		public Camera Camera { get; private set; }
		public bool IsCamera { get; private set; }
		public DeviceViewModel(Camera camera)
		{
			Camera = camera;
			Name = camera.Name;
			CameraUid = camera.UID;
			IsCamera = true;
			IsChecked = camera.IsAddedInConfiguration;
			IsEnabled = !camera.IsAddedInConfiguration;
		}
		public DeviceViewModel(RviServer server)
		{
			Name = server.Name;
		}
		public DeviceViewModel(RviDevice device)
		{
			Name = device.Name;
			Address = device.Ip;
		}
		public DeviceViewModel(RviChannel channel)
		{
			Name = channel.Name;
		}
		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged(() => IsChecked);
			}
		}
	}
}