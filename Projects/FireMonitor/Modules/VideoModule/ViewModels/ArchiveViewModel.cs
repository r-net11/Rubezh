using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Entities.DeviceOriented;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Video.RVI_VSS;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Practices.Prism;

namespace VideoModule.ViewModels
{
	public class ArchiveViewModel : DialogViewModel
	{
		public ArchiveViewModel()
		{
			Title = "Архив видеозаписей на сервере";
			CellPlayerWrap = new CellPlayerWrap();
			StartTime = DateTime.Now - TimeSpan.FromHours(1);
			EndTime = DateTime.Now;
			ArchivePlayer = new CellPlayerWrap();
			SearchCommand = new RelayCommand(OnSearch, CanSearch);
			StartCommand = new RelayCommand(OnStart);
			PauseCommand = new RelayCommand(OnPause);
			StopCommand = new RelayCommand(OnStop);
			FastCommand = new RelayCommand(OnFast);
			SlowCommand = new RelayCommand(OnSlow);
			InitializeCameras();
		}

		public CellPlayerWrap CellPlayerWrap { get; private set; }

		private bool _isStarted;
		public bool IsStarted
		{
			get { return _isStarted; }
			set
			{
				_isStarted = value;
				OnPropertyChanged(() => IsStarted);
			}
		}

		private bool _isPaused;
		public bool IsPaused
		{
			get { return _isPaused; }
			set
			{
				_isPaused = value;
				OnPropertyChanged(() => IsPaused);
			}
		}

		public RelayCommand StartCommand { get; private set; }
		void OnStart()
		{
			if (SelectedRecord == null)
			{
				MessageBoxService.ShowError("Выберите файл для воспроизведения", "Сообщение");
				return;
			}
			try
			{
				if (StartedRecord != null)
					CellPlayerWrap.Stop(StartedRecord);
				CellPlayerWrap.Start(SelectedRecord);
				StartedRecord = SelectedRecord;
				IsStarted = true;
				IsPaused = false;
			}
			catch { }
		}

		public RelayCommand PauseCommand { get; private set; }
		void OnPause()
		{
			if (StartedRecord != null)
			{
				try
				{
					if (CellPlayerWrap.Pause(StartedRecord, !IsPaused))
						IsPaused = !IsPaused;
				}
				catch { }
			}
		}

		public RelayCommand StopCommand { get; private set; }
		void OnStop()
		{
			if (StartedRecord != null)
			{
				try
				{
					CellPlayerWrap.Stop(StartedRecord);
					IsStarted = false;
					IsPaused = false;
				}
				catch { }
			}
		}

		public RelayCommand FastCommand { get; private set; }
		void OnFast()
		{
			if (StartedRecord != null)
			{
				try
				{
					CellPlayerWrap.Fast(StartedRecord);
				}
				catch { }
			}
		}

		public RelayCommand SlowCommand { get; private set; }
		void OnSlow()
		{
			if (StartedRecord != null)
			{
				try
				{
					CellPlayerWrap.Slow(StartedRecord);
				}
				catch { }
			}
		}

		public void InitializeCameras()
		{
			Cameras = new ObservableCollection<CameraViewModel>();
			foreach (var camera in FiresecManager.SystemConfiguration.Cameras)
			{
				var cameraViewModel = new CameraViewModel(camera, CellPlayerWrap);
				cameraViewModel.Connect();
				Cameras.Add(cameraViewModel);
			}
			SelectedCamera = Cameras.FirstOrDefault();
		}

		private CellPlayerWrap _archivePlayer { get; set; }
		public CellPlayerWrap ArchivePlayer
		{
			get { return _archivePlayer; }
			set
			{
				_archivePlayer = value;
				OnPropertyChanged(() => _archivePlayer);
			}
		}

		private DateTime _startTime;
		public DateTime StartTime
		{
			get { return _startTime; }
			set
			{
				_startTime = value;
				OnPropertyChanged(() => StartTime);
			}
		}

		private DateTime _endTime;
		public DateTime EndTime
		{
			get { return _endTime; }
			set
			{
				_endTime = value;
				OnPropertyChanged(() => EndTime);
			}
		}

		private ObservableCollection<CameraViewModel> _cameras;
		public ObservableCollection<CameraViewModel> Cameras
		{
			get { return _cameras; }
			set
			{
				_cameras = value;
				OnPropertyChanged(() => Cameras);
			}
		}

		private CameraViewModel _selectedCamera;
		public CameraViewModel SelectedCamera
		{
			get { return _selectedCamera; }
			set
			{
				_selectedCamera = value;
				OnPropertyChanged(() => SelectedCamera);
			}
		}

		CameraViewModel _selectedChannel;
		public CameraViewModel SelectedChannel
		{
			get { return _selectedChannel; }
			set
			{
				_selectedChannel = value;
				OnPropertyChanged(() => SelectedChannel);
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

		public PlayBackDeviceRecord StartedRecord { get; private set; }

		public RelayCommand SearchCommand { get; private set; }
		void OnSearch()
		{
			var camera = new Camera();
			if (SelectedCamera.IsDvr)
			{
				if (SelectedChannel != null)
					camera = SelectedChannel.Camera;
			}
			else
				camera = SelectedCamera.Camera;
			var cellPlayerWrap = new CellPlayerWrap();
			var channel = cellPlayerWrap.Connect(camera.Address, camera.Port, camera.Login, camera.Password).FirstOrDefault
				(x => x.ChannelNumber == camera.ChannelNumber);
			if (channel == null)
				return;
			Records = new ObservableCollection<PlayBackDeviceRecord>();
			for (var time = StartTime; new DateTime(time.Year, time.Month, time.Day) < EndTime; time += TimeSpan.FromDays(1))
			{
				var dayStart = new DateTime(time.Year, time.Month, time.Day);
				var dayEnd = new DateTime(time.Year, time.Month, time.Day, 23, 59, 59);
				var startTime = dayStart;
				if (startTime < StartTime)
					startTime = StartTime;
				var endTime = dayEnd;
				if (endTime > EndTime)
					endTime = EndTime;
				var records = channel.QueryRecordFiles(startTime, endTime);
				Trace.WriteLine(startTime + "  -  " + endTime);
				Records.AddRange(records);
			}
		}

		bool CanSearch()
		{
			if (SelectedCamera == null)
				return false;
			if (SelectedCamera.IsDvr)
				if (SelectedChannel == null)
					return false;
			return true;
		}
	}
}