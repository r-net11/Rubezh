using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using DeviceControls;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Events;
using XFiresecAPI;

namespace VideoModule.ViewModels
{
	public class CameraDetailsViewModel : DialogViewModel, IWindowIdentity
	{
		public Camera Camera { get; private set; }

		public CameraDetailsViewModel(Camera camera)
		{
			ShowCommand = new RelayCommand(OnShow);
			ShowZoneCommand = new RelayCommand(OnShowZone);
			Camera = camera;
			Title = Camera.Name;
			TopMost = true;
		}

		public Brush CameraPicture
		{
			get { return PictureCacheSource.CameraPicture.GetDefaultBrush(); }
		}

		public RelayCommand ShowCommand { get; private set; }
		private void OnShow()
		{
			ServiceFactory.Events.GetEvent<ShowCameraEvent>().Publish(Camera.UID);
		}

		public RelayCommand ShowZoneCommand { get; private set; }
		void OnShowZone()
		{
			var zoneUid = Camera.ZoneUIDs.FirstOrDefault();
			ServiceFactory.Events.GetEvent<ShowXZoneEvent>().Publish(zoneUid);
		}

		public string PresentationZones
		{
			get
			{
				var zones = new List<XZone>();
				foreach (var zoneUID in Camera.ZoneUIDs.ToList())
				{
					var zone = XManager.Zones.FirstOrDefault(x => x.BaseUID == zoneUID);
					if (zone != null)
						zones.Add(zone);
				}
				var presentationZones = XManager.GetCommaSeparatedObjects(new List<INamedBase>(zones));
				return presentationZones;
			}
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


		public class PlanViewModel : BaseViewModel
		{
			private Plan Plan;
			private Camera Camera;

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

			private void OnShowOnPlan()
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