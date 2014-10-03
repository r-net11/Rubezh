using System;
using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class DirectionsViewModel : ViewPartViewModel, ISelectable<Guid>
	{
		public void Initialize()
		{
			Directions = new List<DirectionViewModel>();
			foreach (var direction in GKManager.Directions)
			{
				if (direction.InputZones.Count + direction.InputDevices.Count > 0)
				{
					var directionViewModel = new DirectionViewModel(direction);
					Directions.Add(directionViewModel);
				}
			}
			SelectedDirection = Directions.FirstOrDefault();
		}

		List<DirectionViewModel> _direction;
		public List<DirectionViewModel> Directions
		{
			get { return _direction; }
			set
			{
				_direction = value;
				OnPropertyChanged(() => Directions);
			}
		}

		DirectionViewModel _selectedDirection;
		public DirectionViewModel SelectedDirection
		{
			get { return _selectedDirection; }
			set
			{
				_selectedDirection = value;
				InitializeInputOutputObjects();
				OnPropertyChanged(() => SelectedDirection);
			}
		}

		public void Select(Guid directionUID)
		{
			if (directionUID != Guid.Empty)
			{
				SelectedDirection = Directions.FirstOrDefault(x => x.Direction.UID == directionUID);
			}
			InitializeInputOutputObjects();
		}

		public List<DirectionDeviceViewModel> InputDevices { get; private set; }
		public List<DirectionZoneViewModel> InputZones { get; private set; }
		public List<DeviceViewModel> OutputDevices { get; private set; }

		void InitializeInputOutputObjects()
		{
			if (SelectedDirection == null)
				return;

			InputDevices = new List<DirectionDeviceViewModel>();
			foreach (var inputDevice in SelectedDirection.Direction.DirectionDevices)
			{
				var directionDeviceViewModel = new DirectionDeviceViewModel(inputDevice);
				InputDevices.Add(directionDeviceViewModel);
			}
			OnPropertyChanged(() => InputDevices);

			InputZones = new List<DirectionZoneViewModel>();
			foreach (var directionZone in SelectedDirection.Direction.DirectionZones)
			{
				if (directionZone.Zone.State != null)
				{
					var directionZoneViewModel = new DirectionZoneViewModel(directionZone.Zone);
					directionZoneViewModel.StateType = directionZone.StateBit;
					InputZones.Add(directionZoneViewModel);
				}
			}
			OnPropertyChanged(() => InputZones);

			OutputDevices = new List<DeviceViewModel>();
			foreach (var outputDevice in SelectedDirection.Direction.OutputDevices)
			{
				var deviceViewModel = DevicesViewModel.Current.AllDevices.FirstOrDefault(x => x.Device == outputDevice);
				OutputDevices.Add(deviceViewModel);
			}
			OnPropertyChanged(() => OutputDevices);
		}

		DeviceViewModel _selectedOutputDevice;
		public DeviceViewModel SelectedOutputDevice
		{
			get { return _selectedOutputDevice; }
			set
			{
				_selectedOutputDevice = value;
				OnPropertyChanged(() => SelectedOutputDevice);
			}
		}

		DeviceViewModel _selectedInputDevice;
		public DeviceViewModel SelectedInputDevice
		{
			get { return _selectedInputDevice; }
			set
			{
				_selectedInputDevice = value;
				OnPropertyChanged(() => SelectedInputDevice);
			}
		}
	}
}