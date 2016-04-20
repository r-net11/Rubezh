using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrastructure.PlanLink.ViewModels;
using RubezhAPI.Journal;
using RubezhAPI.Models;
using RubezhClient;
using RviClient;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows.Input;

namespace VideoModule.ViewModels
{
	public class CameraDetailsViewModel : DialogViewModel, IWindowIdentity
	{
		public Camera Camera { get; private set; }
		public int Width { get; private set; }
		public int Height { get; private set; }
		public int MarginLeft { get; private set; }
		public int MarginTop { get; private set; }
		public PlanLinksViewModel PlanLinks { get; private set; }

		public CameraDetailsViewModel(Camera camera)
		{
			ShowCommand = new RelayCommand(OnShow);
			SetPtzPresetCommand = new RelayCommand(OnSetPtzPreset, CanSetPtzPreset);
			AlarmSetChannelCommand = new RelayCommand(OnAlarmSetChannel, CanAlarmSetChannel);
			AlarmDisableChannelCommand = new RelayCommand(OnAlarmDisableChannel, CanAlarmDisableChannel);
			Camera = camera;
			Title = Camera.PresentationName;
			Camera.StatusChanged += OnCameraStatusChanged;

			Presets = new ObservableCollection<int>();
			for (int i = 0; i < camera.CountPresets; i++)
			{
				Presets.Add(i + 1);
			}
			SelectedPreset = Presets.FirstOrDefault();

			if (Camera != null)
			{
				Width = Camera.ShowDetailsWidth;
				Height = Camera.ShowDetailsHeight;
				MarginLeft = Camera.ShowDetailsMarginLeft;
				MarginTop = Camera.ShowDetailsMarginTop;
				IsRecordOnline = Camera.IsRecordOnline;
				IsOnGuard = Camera.IsOnGuard;
				Status = Camera.Status;
				PlanLinks = new PlanLinksViewModel(Camera);
			}
		}
		RviStatus _status;
		public RviStatus Status
		{
			get { return _status; }
			set
			{
				_status = value;
				IsConnected = value == RviStatus.Connected;
				OnPropertyChanged(() => Status);
			}
		}
		bool _isOnGuard;
		public bool IsOnGuard
		{
			get { return _isOnGuard; }
			set
			{
				_isOnGuard = value;
				OnPropertyChanged(() => IsOnGuard);
			}
		}
		bool _isRecordOnline;
		public bool IsRecordOnline
		{
			get { return _isRecordOnline; }
			set
			{
				_isRecordOnline = value;
				OnPropertyChanged(() => IsRecordOnline);
			}
		}
		bool _isConnected;
		public bool IsConnected
		{
			get { return _isConnected; }
			set
			{
				_isConnected = value;
				OnPropertyChanged(() => IsConnected);
			}
		}
		void OnCameraStatusChanged()
		{
			var oldStatus = Status;
			IsRecordOnline = Camera.IsRecordOnline;
			IsOnGuard = Camera.IsOnGuard;
			Status = Camera.Status;
			if (oldStatus != RviStatus.Connected && Camera.Status == RviStatus.Connected)
				OnPlay();
			else if (oldStatus == RviStatus.Connected && Camera.Status != RviStatus.Connected)
				OnStop();
			CommandManager.InvalidateRequerySuggested();
		}

		public RelayCommand ShowCommand { get; private set; }
		void OnShow()
		{
			ServiceFactory.Events.GetEvent<ShowCameraEvent>().Publish(Camera.UID);
		}

		public string Guid
		{
			get { return Camera.UID.ToString(); }
		}

		public ObservableCollection<int> Presets { get; private set; }

		int _selectedPreset;
		public int SelectedPreset
		{
			get { return _selectedPreset; }
			set
			{
				_selectedPreset = value;
				OnPropertyChanged(() => SelectedPreset);
			}
		}

		public RelayCommand SetPtzPresetCommand { get; private set; }
		void OnSetPtzPreset()
		{
			try
			{
				//RviClient.RviClientHelper.SetPtzPreset(ClientManager.SystemConfiguration, Camera, SelectedPreset - 1);
				RviClient.RviClientHelper.SetPtzPreset(ClientManager.SystemConfiguration.RviSettings, Camera, SelectedPreset);

				var journalItem = new JournalItem()
				{
					SystemDateTime = DateTime.Now,
					JournalEventNameType = JournalEventNameType.Перевод_в_предустановку,
					JournalEventDescriptionType = JournalEventDescriptionType.NULL,
					DescriptionText = SelectedPreset.ToString(),
					JournalSubsystemType = JournalSubsystemType.Video,
					JournalObjectType = JournalObjectType.Camera,
					ObjectUID = Camera.UID,
					ObjectName = Camera.PresentationName,
				};
				ClientManager.FiresecService.AddJournalItem(journalItem);
			}
			catch
			{
				MessageBoxService.ShowWarning("Возникла ошибка при переводе в предустановку");
			}
		}
		bool CanSetPtzPreset()
		{
			return IsConnected && Presets.Count > 0;
		}
		public bool IsSetPtzPreset
		{
			get { return Presets.Count > 0; }
		}

		public RelayCommand AlarmSetChannelCommand { get; private set; }
		void OnAlarmSetChannel()
		{
			RviClientHelper.AlarmSetChannel(ClientManager.SystemConfiguration.RviSettings, Camera);
		}
		bool CanAlarmSetChannel()
		{
			return IsConnected && !IsOnGuard;
		}
		public RelayCommand AlarmDisableChannelCommand { get; private set; }
		void OnAlarmDisableChannel()
		{
			RviClientHelper.AlarmDisableChannel(ClientManager.SystemConfiguration.RviSettings, Camera);
		}
		bool CanAlarmDisableChannel()
		{
			return IsConnected && IsOnGuard;
		}
		public bool PrepareToTranslation(out IPEndPoint ipEndPoint, out int vendorId)
		{
			return RviClientHelper.PrepareToTranslation(ClientManager.SystemConfiguration.RviSettings, Camera.SelectedRviStream, out ipEndPoint, out vendorId);
		}
		public EventHandler Play;
		void OnPlay()
		{
			if (Play != null)
				Play(this, EventArgs.Empty);
		}
		public EventHandler Stop;
		void OnStop()
		{
			if (Stop != null)
				Stop(this, EventArgs.Empty);
		}
	}
}