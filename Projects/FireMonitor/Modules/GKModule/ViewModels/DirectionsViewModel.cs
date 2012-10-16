using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class DirectionsViewModel : ViewPartViewModel, ISelectable<Guid>
	{
		public DirectionsViewModel()
		{
		}

		public void Initialize()
		{
			Directions = (from XDirection direction in XManager.DeviceConfiguration.Directions
					 orderby direction.No
					 select new DirectionViewModel(direction.DirectionState)).ToList();

			SelectedDirection = Directions.FirstOrDefault();
		}

		List<DirectionViewModel> _direction;
		public List<DirectionViewModel> Directions
		{
			get { return _direction; }
			set
			{
				_direction = value;
				OnPropertyChanged("Directions");
			}
		}

		DirectionViewModel _selectedDirection;
		public DirectionViewModel SelectedDirection
		{
			get { return _selectedDirection; }
			set
			{
				_selectedDirection = value;
				InitializeDevices();
				OnPropertyChanged("SelectedDirection");
			}
		}

		public void Select(Guid directionUID)
		{
			if (directionUID != Guid.Empty)
			{
				SelectedDirection = Directions.FirstOrDefault(x => x.Direction.UID == directionUID);
			}
			InitializeDevices();
		}

		public ObservableCollection<DeviceViewModel> Devices { get; private set; }

		void InitializeDevices()
		{
			if (SelectedDirection == null)
				return;

			var devices = new HashSet<XDevice>();

			var directionDevices = from XDevice directionDevice in SelectedDirection.Direction.InputDevices select directionDevice;
			foreach (var directionDevice in directionDevices)
			{
				directionDevice.AllParents.ForEach(x => { devices.Add(x); });
				devices.Add(directionDevice);
			}

			Devices = new ObservableCollection<DeviceViewModel>();
			foreach (var device in devices)
			{
				Devices.Add(new DeviceViewModel(device, Devices)
				{
					IsExpanded = true,
					IsBold = directionDevices.Contains(device)
				});
			}

			foreach (var device in Devices)
			{
				if (device.Device.Parent != null)
				{
					var parent = Devices.FirstOrDefault(x => x.Device.UID == device.Device.Parent.UID);
					device.Parent = parent;
					parent.Children.Add(device);
				}
			}

			OnPropertyChanged("Devices");
		}
	}
}