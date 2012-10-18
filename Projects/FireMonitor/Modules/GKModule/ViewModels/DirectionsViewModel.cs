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
				InitializeInputOutputObjects();
				OnPropertyChanged("SelectedDirection");
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

        //List<XDevice> _inputDevices;
        public List<DeviceViewModel> InputDevices { get; private set; }
        //{
        //    get { return _inputDevices; }
        //    set
        //    {
        //        _inputDevices = value;
        //        OnPropertyChanged("InputDevices");
        //    }
        //}

        //List<XZone> _inputZones;
        public List<ZoneViewModel> InputZones{get;private set;}
        //{
        //    get { return _inputZones; }
        //    set
        //    {
        //        _inputZones = value;
        //        OnPropertyChanged("InputZones");
        //    }
        //}

        //List<XDevice> _outputDevices;
        public List<DeviceViewModel> OutputDevices{get;private set;}
        //{
        //    get { return _outputDevices; }
        //    set
        //    {
        //        _outputDevices = value;
        //        OnPropertyChanged("OutputDevices");
        //    }
        //}

		void InitializeInputOutputObjects()
		{
			if (SelectedDirection == null)
				return;

            InputDevices = new List<DeviceViewModel>();
            foreach (var inputDevice in SelectedDirection.Direction.InputDevices)
            {
                var deviceViewModel = DevicesViewModel.Current.AllDevices.FirstOrDefault(x=>x.Device == inputDevice);
                InputDevices.Add(deviceViewModel);
            }
            OnPropertyChanged("InputDevices");

            InputZones = new List<ZoneViewModel>();
            foreach (var inputZone in SelectedDirection.Direction.InputZones)
            {
                var zoneViewModel = ZonesViewModel.Current.Zones.FirstOrDefault(x => x.Zone == inputZone);
                InputZones.Add(zoneViewModel);
            }
            OnPropertyChanged("InputZones");

            OutputDevices = new List<DeviceViewModel>();
            foreach (var outputDevice in SelectedDirection.Direction.OutputDevices)
            {
                var deviceViewModel = DevicesViewModel.Current.AllDevices.FirstOrDefault(x => x.Device == outputDevice);
                OutputDevices.Add(deviceViewModel);
            }
            OnPropertyChanged("OutputDevices");
		}
	}
}