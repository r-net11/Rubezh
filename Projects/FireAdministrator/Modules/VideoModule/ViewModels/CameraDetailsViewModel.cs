using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System;
using FiresecClient;
using System.Collections.Generic;
using XFiresecAPI;
using Infrastructure.Common.Video.RVI_VSS;

namespace VideoModule.ViewModels
{
	public class CameraDetailsViewModel : SaveCancelDialogViewModel
	{
		public Camera Camera { get; private set; }
		readonly CellPlayerWrap _cellPlayerWrap;

		public CameraDetailsViewModel(Camera camera)
		{
			ShowZonesCommand = new RelayCommand(OnShowZones);
			ShowCommand = new RelayCommand(OnShow);

			if (camera.Address =="")
			{
				Title = "Создание новой камеры";
				Camera = new Camera();
				Camera.Name = "Камера";
			}
			else
			{
				Camera = camera;
				Title = "Свойства камеры: " + Camera.PresentationName;
			}
			_cellPlayerWrap = new CellPlayerWrap();
			Initialize();
			CopyProperties();
		}

		void Initialize()
		{
			StateClasses = new List<XStateClass>();
			StateClasses.Add(XStateClass.Fire1);
			StateClasses.Add(XStateClass.Fire2);
			StateClasses.Add(XStateClass.Attention);
			StateClasses.Add(XStateClass.Ignore);
		}

		void CopyProperties()
		{
			Name = Camera.Name;
			Left = Camera.Left;
			Top = Camera.Top;
			Width = Camera.Width;
			Height = Camera.Height;
			IgnoreMoveResize = Camera.IgnoreMoveResize;
			SelectedStateClass = Camera.StateClass;
			ChannelNumber = Camera.ChannelNumber + 1;
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		int _channelNumber;
		public int ChannelNumber
		{
			get { return _channelNumber; }
			set
			{
				_channelNumber = value;
				OnPropertyChanged(() => ChannelNumber);
			}
		}

		int _left;
		public int Left
		{
			get { return _left; }
			set
			{
				_left = value;
				OnPropertyChanged(() => Left);
			}
		}

		int _top;
		public int Top
		{
			get { return _top; }
			set
			{
				_top = value;
				OnPropertyChanged(() => Top);
			}
		}

		int _width;
		public int Width
		{
			get { return _width; }
			set
			{
				_width = value;
				OnPropertyChanged(() => Width);
			}
		}

		int _height;
		public int Height
		{
			get { return _height; }
			set
			{
				_height = value;
				OnPropertyChanged(() => Height);
			}
		}
		
		bool _ignoreMoveResize;
		public bool IgnoreMoveResize
		{
			get { return _ignoreMoveResize; }
			set
			{
				_ignoreMoveResize = value;
				OnPropertyChanged(() => IgnoreMoveResize);
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
				OnPropertyChanged(() => SelectedStateClass);
			}
		}
		
		public RelayCommand ShowZonesCommand { get; private set; }
		void OnShowZones()
		{
			var zonesSelectationViewModel = new ZonesSelectationViewModel(Camera.ZoneUIDs);
			if (DialogService.ShowModalWindow(zonesSelectationViewModel))
			{
				Camera.ZoneUIDs = zonesSelectationViewModel.Zones;
				OnPropertyChanged("PresentationZones");
			}
		}

		public string PresentationZones
		{
			get
			{
				var zones =
					Camera.ZoneUIDs.Select(zoneUID => XManager.Zones.FirstOrDefault(x => x.BaseUID == zoneUID))
						.Where(zone => zone != null)
						.ToList();
				var presentationZones = XManager.GetCommaSeparatedObjects(new List<INamedBase>(zones));
				return presentationZones;
			}
		}

		public RelayCommand ShowCommand { get; private set; }
		void OnShow()
		{
			try
			{
				var title = Name + " " + ChannelNumber;
				var previewViewModel = new PreviewViewModel(title, _cellPlayerWrap);
				_cellPlayerWrap.Connect(Camera.Parent.Address, Camera.Parent.Port, Camera.Parent.Login, Camera.Parent.Password);
				_cellPlayerWrap.Start(ChannelNumber - 1);
				DialogService.ShowModalWindow(previewViewModel);
				_cellPlayerWrap.Stop();
			}
			catch (Exception e)
			{
				MessageBoxService.ShowWarning(e.Message);
			}
		}

		protected override bool Save()
		{
			Camera.Name = Name;
			Camera.ChannelNumber = ChannelNumber - 1;
			Camera.Left = Left;
			Camera.Top = Top;
			Camera.Width = Width;
			Camera.Height = Height;
			Camera.IgnoreMoveResize = IgnoreMoveResize;
			Camera.StateClass = SelectedStateClass;
			_cellPlayerWrap.Stop();
			return base.Save();
		}
	}
}