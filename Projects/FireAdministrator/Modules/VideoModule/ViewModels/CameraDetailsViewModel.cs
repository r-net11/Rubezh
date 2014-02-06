using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace VideoModule.ViewModels
{
	public class CameraDetailsViewModel : SaveCancelDialogViewModel
	{
		public Camera Camera { get; private set; }
		public List<Guid> Zones { get; set; }

		public CameraDetailsViewModel(Camera camera = null)
		{
			ShowZonesCommand = new RelayCommand(OnShowZones);
			StateClasses = new List<XStateClass>();
			StateClasses.Add(XStateClass.Fire1);
			StateClasses.Add(XStateClass.Fire2);
			StateClasses.Add(XStateClass.Attention);
			StateClasses.Add(XStateClass.Ignore);

			if (camera != null)
			{
				Title = "Редактировать камеру";
				Camera = camera;
			}
			else
			{
				Title = "Создать камеру";
				Camera = new Camera()
				{
					Name = "Новая камера",
					Address = "172.16.7.88"
				};
			}

			CopyProperties();
		}

		void CopyProperties()
		{
			Name = Camera.Name;
			Address = Camera.Address;
			SelectedStateClass = Camera.StateClass;
			if (Camera.ZoneUIDs == null)
				Camera.ZoneUIDs = new List<Guid>();
			Zones = Camera.ZoneUIDs.ToList();
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged("Name");
			}
		}

		string _address;
		public string Address
		{
			get { return _address; }
			set
			{
				_address = value;
				OnPropertyChanged("Address");
			}
		}

		public string PresentationZones
		{
			get
			{
				var zones = new List<XZone>();
				foreach (var zoneUID in Zones)
				{
					var zone = XManager.Zones.FirstOrDefault(x => x.UID == zoneUID);
					if (zone != null)
						zones.Add(zone);
				}
				var presentationZones = XManager.GetCommaSeparatedZones(zones);
				return presentationZones;
			}
		}

		public List<XStateClass> StateClasses { get; private set; }

		XStateClass _selectedStateClass;
		public XStateClass SelectedStateClass
		{
			get { return _selectedStateClass; }
			set
			{
				_selectedStateClass = value;
				OnPropertyChanged("SelectedStateClass");
			}
		}

		public RelayCommand ShowZonesCommand { get; private set; }
		void OnShowZones()
		{
			var zonesSelectationViewModel = new ZonesSelectationViewModel(Zones);
			if (DialogService.ShowModalWindow(zonesSelectationViewModel))
			{
				Zones = zonesSelectationViewModel.Zones;
				OnPropertyChanged("PresentationZones");
			}
		}

		protected override bool Save()
		{
			Camera.Name = Name;
			Camera.Address = Address;
			Camera.StateClass = SelectedStateClass;
			Camera.ZoneUIDs = Zones.ToList();
			return base.Save();
		}
	}
}