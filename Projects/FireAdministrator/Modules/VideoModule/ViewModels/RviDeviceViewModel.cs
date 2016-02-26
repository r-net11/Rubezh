using Infrastructure.Common.TreeList;
using RubezhAPI.Models;
using System;

namespace VideoModule.ViewModels
{
	public class RviDeviceViewModel : TreeNodeViewModel<RviDeviceViewModel>
	{
		public bool IsEnabled { get; private set; }
		public string Name { get; private set; }
		public string Address { get; private set; }
		public Guid CameraUid { get; private set; }
		public Camera Camera { get; private set; }
		public bool IsCamera { get; private set; }
		public RviDeviceViewModel(Camera camera)
		{
			Camera = camera;
			Name = camera.Name;
			CameraUid = camera.UID;
			IsCamera = true;
			IsChecked = camera.IsAddedInConfiguration;
			IsEnabled = !camera.IsAddedInConfiguration;
		}
		public RviDeviceViewModel(RviServer server)
		{
			Name = server.PresentationName;
		}
		public RviDeviceViewModel(RviDevice device)
		{
			Name = device.Name;
			Address = device.Ip;
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