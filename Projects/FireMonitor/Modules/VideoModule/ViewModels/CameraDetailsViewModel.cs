using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrastructure.PlanLink.ViewModels;
using Infrustructure.Plans.Events;
using RubezhAPI.Journal;
using RubezhAPI.Models;
using RubezhClient;
using RviClient;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;

namespace VideoModule.ViewModels
{
	public class CameraDetailsViewModel : DialogViewModel, IWindowIdentity
	{
		public Camera Camera { get; private set; }
		public int Width { get; private set; }
		public int Height { get; private set; }
		public int MarginLeft { get; private set; }
		public int MarginTop { get; private set; }
		public bool IsRecordOnline { get; private set; }
		public bool IsOnGuard { get; private set; }
		public RviStatus Status { get; private set; }
		public bool IsConnected { get { return Status == RviStatus.Connected; } }
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
				PlanLinks = new PlanLinksViewModel(Camera.PlanElementUIDs);

			}
		}
		void OnCameraStatusChanged()
		{
			if (!IsConnected && Camera.Status == RviStatus.Connected)
				OnPlay();
			else if (IsConnected && Camera.Status != RviStatus.Connected)
				OnStop();
			IsRecordOnline = Camera.IsRecordOnline;
			IsOnGuard = Camera.IsOnGuard;
			Status = Camera.Status;
			OnPropertyChanged(() => IsRecordOnline);
			OnPropertyChanged(() => IsOnGuard);
			OnPropertyChanged(() => Status);
			OnPropertyChanged(() => IsConnected);
		}

		public RelayCommand ShowCommand { get; private set; }
		void OnShow()
		{
			ServiceFactory.Events.GetEvent<ShowCameraEvent>().Publish(Camera.UID);
		}

		//public ObservableCollection<PlanViewModel> PlanNames
		//{
		//	get
		//	{
		//		var planes = ClientManager.PlansConfiguration.AllPlans.Where(item => item.ElementExtensions.OfType<ElementCamera>().Any(element => element.CameraUID == Camera.UID));
		//		var planViewModels = new ObservableCollection<PlanViewModel>();
		//		foreach (var plan in planes)
		//		{
		//			planViewModels.Add(new PlanViewModel(plan, Camera));
		//		}
		//		return planViewModels;
		//	}
		//}

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
			return IsConnected && !Camera.IsOnGuard;
		}
		public RelayCommand AlarmDisableChannelCommand { get; private set; }
		void OnAlarmDisableChannel()
		{
			RviClientHelper.AlarmDisableChannel(ClientManager.SystemConfiguration.RviSettings, Camera);
		}
		bool CanAlarmDisableChannel()
		{
			return IsConnected && Camera.IsOnGuard;
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

		//public class PlanViewModel : BaseViewModel
		//{
		//	Plan Plan;
		//	Camera Camera;

		//	public string Name
		//	{
		//		get { return Plan.Caption; }
		//	}

		//	//public PlanViewModel(Plan plan, Camera camera)
		//	//{
		//	//	Plan = plan;
		//	//	Camera = camera;
		//	//	ShowOnPlanCommand = new RelayCommand(OnShowOnPlan);
		//	//}

		//	public RelayCommand ShowOnPlanCommand { get; private set; }

		//	void OnShowOnPlan()
		//	{
		//		ShowCamera(Camera, Plan);
		//	}

		//	public static void ShowCamera(Camera camera, Plan plan)
		//	{
		//		var element = plan == null ? null : plan.ElementExtensions.OfType<ElementCamera>().FirstOrDefault(item => item.CameraUID == camera.UID);
		//		if (plan == null || element == null)
		//			ShowCamera(camera);
		//		else
		//			ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
		//	}

		//	public static void ShowCamera(Camera camera)
		//	{
		//		ServiceFactoryBase.Events.GetEvent<ShowCameraOnPlanEvent>().Publish(camera);
		//	}
		//}
	}
}