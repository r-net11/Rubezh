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
			TestCommand = new RelayCommand(OnTest);
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
					Address = "192.168.0.1"
				};
			}

			CopyProperties();
		}

		void CopyProperties()
		{
			Name = Camera.Name;
			Address = Camera.Address;
			Left = Camera.Left;
			Top = Camera.Top;
			Width = Camera.Width;
			Height = Camera.Height;
			IgnoreMoveResize = Camera.IgnoreMoveResize;
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

		int _left;
		public int Left
		{
			get { return _left; }
			set
			{
				_left = value;
				OnPropertyChanged("Left");
			}
		}

		int _top;
		public int Top
		{
			get { return _top; }
			set
			{
				_top = value;
				OnPropertyChanged("Top");
			}
		}

		int _width;
		public int Width
		{
			get { return _width; }
			set
			{
				_width = value;
				OnPropertyChanged("Width");
			}
		}

		int _height;
		public int Height
		{
			get { return _height; }
			set
			{
				_height = value;
				OnPropertyChanged("Height");
			}
		}

		bool _ignoreMoveResize;
		public bool IgnoreMoveResize
		{
			get { return _ignoreMoveResize; }
			set
			{
				_ignoreMoveResize = value;
				OnPropertyChanged("IgnoreMoveResize");
			}
		}

		public string PresenrationZones
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
				OnPropertyChanged("PresenrationZones");
			}
		}

		public RelayCommand TestCommand { get; private set; }
		void OnTest()
		{
			var camera = new FiresecAPI.Models.Camera()
			{
				Address = Address,
				Left = Left,
				Top = Top,
				Width = Width,
				Height = Height,
				IgnoreMoveResize = IgnoreMoveResize
			};
			VideoService.ShowModal(camera); //"172.16.7.202"
			Left = camera.Left;
			Top = camera.Top;
			Width = camera.Width;
			Height = camera.Height;
		}

		protected override bool Save()
		{
			Camera.Name = Name;
			Camera.Address = Address;
			Camera.Left = Left;
			Camera.Top = Top;
			Camera.Width = Width;
			Camera.Height = Height;
			Camera.StateClass = SelectedStateClass;
			Camera.ZoneUIDs = Zones.ToList();
			Camera.IgnoreMoveResize = IgnoreMoveResize;
			return base.Save();
		}
	}
}