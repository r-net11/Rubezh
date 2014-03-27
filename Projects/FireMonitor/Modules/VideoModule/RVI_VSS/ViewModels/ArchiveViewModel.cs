using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Entities.DeviceOriented;
using Infrastructure.Common;
using Infrastructure.Common.Video.RVI_VSS;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Practices.Prism;
using Device = Entities.DeviceOriented.Device;

namespace VideoModule.RVI_VSS.ViewModels
{
	public class ArchiveViewModel : SaveCancelDialogViewModel
	{
		public ArchiveViewModel()
		{
			Title = "Архив видеозаписей на сервере";
			InitiallizeDevices();
			StartTime = DateTime.Now - TimeSpan.FromHours(1);
			EndTime = DateTime.Now;
			ArchivePlayer = new CellPlayerWrap();
			SearchCommand = new RelayCommand(OnSearch, CanSearch);
		}

		void InitiallizeDevices()
		{
			Devices = new ObservableCollection<DeviceViewModel>();
			foreach (var device in new List<Device>(SystemPerimeter.Instance.Devices))
			{
				var deviceViewModel = new DeviceViewModel(device);
				Devices.Add(deviceViewModel);
			}
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
				OnPropertyChanged(()=>StartTime);
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

		private ObservableCollection<DeviceViewModel> _devices;
		public ObservableCollection<DeviceViewModel> Devices
		{
			get { return _devices; }
			set
			{
				_devices = value;
				OnPropertyChanged(() => Devices);
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
			//Records = new ObservableCollection<PlayBackDeviceRecord>(SelectedDevice.SelectedChannel.QueryRecordFiles(StartTime, EndTime));
			Records = new ObservableCollection<PlayBackDeviceRecord>();
			for (var time = StartTime; time < EndTime; time += TimeSpan.FromDays(1))
			{
				var records = SelectedDevice.SelectedChannel.QueryRecordFiles(time, time + TimeSpan.FromDays(1));
				Records.AddRange(records);
			}
		}

		bool CanSearch()
		{
			return ((SelectedDevice != null) && (SelectedDevice.SelectedChannel != null));
		}
	}
}