﻿using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Events;

namespace VideoModule.ViewModels
{
	public class CameraDetailsViewModel : DialogViewModel, IWindowIdentity
	{
		public Camera Camera { get; private set; }
		public string RviRTSP { get; private set; }

		public CameraDetailsViewModel(Camera camera)
		{
			ShowCommand = new RelayCommand(OnShow);
			SetPtzPresetCommand = new RelayCommand(OnSetPtzPreset, CanSetPtzPreset);
			Camera = camera;
			Title = Camera.PresentationName;

			Presets = new ObservableCollection<int>();
			for (int i = 0; i < camera.CountPresets; i++)
			{
				Presets.Add(i + 1);
			}
			SelectedPreset = Presets.FirstOrDefault();

			if (Camera != null)
				RviRTSP = Camera.RviRTSP;
		}

		public RelayCommand ShowCommand { get; private set; }
		void OnShow()
		{
			ServiceFactory.Events.GetEvent<ShowCameraEvent>().Publish(Camera.UID);
		}

		public ObservableCollection<PlanViewModel> PlanNames
		{
			get
			{
				var planes = FiresecManager.PlansConfiguration.AllPlans.Where(item => item.ElementExtensions.OfType<ElementCamera>().Any(element => element.CameraUID == Camera.UID));
				var planViewModels = new ObservableCollection<PlanViewModel>();
				foreach (var plan in planes)
				{
					planViewModels.Add(new PlanViewModel(plan, Camera));
				}
				return planViewModels;
			}
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
				RviClient.RviClientHelper.SetPtzPreset(FiresecManager.SystemConfiguration, Camera, SelectedPreset - 1);
			}
			catch
			{
				MessageBoxService.ShowWarning("Возникла ошибка при переводе в предустановку");
			}
		}

		bool CanSetPtzPreset()
		{
			return Presets.Count > 0;
		}

		public bool IsSetPtzPreset
		{
			get { return CanSetPtzPreset(); }
		}

		public class PlanViewModel : BaseViewModel
		{
			Plan Plan;
			Camera Camera;

			public string Name
			{
				get { return Plan.Caption; }
			}

			public PlanViewModel(Plan plan, Camera camera)
			{
				Plan = plan;
				Camera = camera;
				ShowOnPlanCommand = new RelayCommand(OnShowOnPlan);
			}

			public RelayCommand ShowOnPlanCommand { get; private set; }

			void OnShowOnPlan()
			{
				ShowCamera(Camera, Plan);
			}

			public static void ShowCamera(Camera camera, Plan plan)
			{
				var element = plan == null ? null : plan.ElementExtensions.OfType<ElementCamera>().FirstOrDefault(item => item.CameraUID == camera.UID);
				if (plan == null || element == null)
					ShowCamera(camera);
				else
					ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
			}

			public static void ShowCamera(Camera camera)
			{
				ServiceFactoryBase.Events.GetEvent<ShowCameraOnPlanEvent>().Publish(camera);
			}
		}
	}
}