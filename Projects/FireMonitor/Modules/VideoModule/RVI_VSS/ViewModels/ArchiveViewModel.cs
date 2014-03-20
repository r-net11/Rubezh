using System;
using System.Collections.ObjectModel;
using System.Linq;
using Entities.DeviceOriented;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Video.RVI_VSS;
using Infrastructure.Common.Windows.ViewModels;

namespace VideoModule.RVI_VSS.ViewModels
{
	public class ArchiveViewModel : SaveCancelDialogViewModel
	{
		public ArchiveViewModel()
		{
			Cameras = new ObservableCollection<Camera>(FiresecManager.SystemConfiguration.Cameras);
			DateTimeFrom = DateTime.Now - TimeSpan.FromHours(1);
			DateTimeTo = DateTime.Now;
			ArchivePlayer = new object();
			StartCommand = new RelayCommand(OnStart, CanStart);
			SearchCommand = new RelayCommand(OnSearch, CanSearch);
		}

		private object _archivePlayer { get; set; }
		public object ArchivePlayer
		{
			get { return _archivePlayer; }
			set
			{
				_archivePlayer = value;
				OnPropertyChanged(() => _archivePlayer);
			}
		}

		private DateTime _dateTimeFrom;
		public DateTime DateTimeFrom
		{
			get { return _dateTimeFrom; }
			set
			{
				_dateTimeFrom = new DateTime(value.Ticks - (value.Ticks % TimeSpan.TicksPerHour), value.Kind);
				OnPropertyChanged(() => DateTimeFrom);
			}
		}

		private DateTime _dateTimeTo;
		public DateTime DateTimeTo
		{
			get { return _dateTimeTo; }
			set
			{
				_dateTimeTo = new DateTime(value.Ticks - (value.Ticks % TimeSpan.TicksPerHour), value.Kind);
				OnPropertyChanged(() => DateTimeTo);
			}
		}

		private ObservableCollection<Camera> _cameras;
		public ObservableCollection<Camera> Cameras
		{
			get { return _cameras; }
			set
			{
				_cameras = value;
				OnPropertyChanged(() => Cameras);
			}
		}

		private DeviceViewModel _selectedDevice;
		public DeviceViewModel SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				OnPropertyChanged(() => SelectedDevice);
			}
		}

		private Camera _selectedCamera;
		public Camera SelectedCamera
		{
			get { return _selectedCamera; }
			set
			{
				_selectedCamera = value;
				var perimeter = SystemPerimeter.Instance;
				var deviceSI = new DeviceSearchInfo(_selectedCamera.Address, _selectedCamera.Port);
				var device = perimeter.AddDevice(deviceSI);
				SelectedDevice = new DeviceViewModel(device);
				OnPropertyChanged(() => SelectedCamera);
			}
		}

		private ObservableCollection<PlayBackDeviceRecord> _records;
		public ObservableCollection<PlayBackDeviceRecord> Records
		{
			get { return _records; }
			set
			{
				_records = value;
				OnPropertyChanged(() => Records);
			}
		}

		private PlayBackDeviceRecord _selectedRecord;
		public PlayBackDeviceRecord SelectedRecord
		{
			get { return _selectedRecord; }
			set
			{
				_selectedRecord = value;
				OnPropertyChanged(() => SelectedRecord);
			}
		}

		public RelayCommand SearchCommand { get; private set; }
		void OnSearch()
		{
			Records = new ObservableCollection<PlayBackDeviceRecord>(SelectedDevice.SelectedChannel.QueryRecordFiles(DateTimeFrom, DateTimeTo));
		}

		bool CanSearch()
		{
			return ((SelectedDevice != null) && (SelectedDevice.SelectedChannel != null));
		}

		public RelayCommand StartCommand { get; private set; }
		void OnStart()
		{
			var cellPlayerWrap = new CellPlayerWrap();
			cellPlayerWrap.InitializeCamera(SelectedCamera);
			ArchivePlayer = cellPlayerWrap.DataContext;
			OnPropertyChanged(() => ArchivePlayer);
		}

		bool CanStart()
		{
			return SelectedRecord != null;
		}
	}
}