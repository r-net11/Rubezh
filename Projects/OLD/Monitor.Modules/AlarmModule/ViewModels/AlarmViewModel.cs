using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Infrastructure.Events;

namespace AlarmModule.ViewModels
{
	public class AlarmViewModel : BaseViewModel
	{
		public Alarm Alarm { get; set; }

		public AlarmViewModel(Alarm alarm)
		{
			Alarm = alarm;

			ResetCommand = new RelayCommand(OnReset, CanReset);
			RemoveFromIgnoreListCommand = new RelayCommand(OnRemoveFromIgnoreList, CanRemoveFromIgnoreList);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			ShowDeviceCommand = new RelayCommand(OnShowDevice, CanShowDevice);
			ShowZoneCommand = new RelayCommand(OnShowZone, CanShowZone);
			ShowInstructionCommand = new RelayCommand(OnShowInstruction, CanShowInstruction);
		}

		public string DeviceName
		{
			get
			{
				if (Alarm.Device != null)
				{
					return Alarm.Device.DottedPresentationAddressAndName;
				}
				return "";
			}
		}

		public string ZoneName
		{
			get
			{
				if (Alarm.Zone != null)
				{
					return "Зона " + Alarm.Zone.PresentationName;
				}
				return "";
			}
		}

		public RelayCommand ResetCommand { get; private set; }
		void OnReset()
		{
			Alarm.Reset();
		}
		public bool CanReset()
		{
			return Alarm.CanReset();
		}

		public RelayCommand RemoveFromIgnoreListCommand { get; private set; }
		void OnRemoveFromIgnoreList()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				Alarm.RemoveFromIgnoreList();
			}
		}
		public bool CanRemoveFromIgnoreList()
		{
			return Alarm.CanRemoveFromIgnoreList();
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			if (Alarm.Device != null)
			{
				ServiceFactory.Events.GetEvent<ShowDeviceOnPlanEvent>().Publish(Alarm.Device.UID);
			}
			if (Alarm.Zone != null)
			{
				ServiceFactory.Events.GetEvent<ShowZoneOnPlanEvent>().Publish(Alarm.Zone.UID);
			}
		}	   
		bool CanShowOnPlan()
		{
			if (Alarm.Device != null)
			{
				return FiresecManager.PlansConfiguration.AllPlans.Any(x => { return x.ElementDevices.Any(y => y.DeviceUID == Alarm.Device.UID); });
			}
			if (Alarm.Zone != null)
			{
				return FiresecManager.PlansConfiguration.AllPlans.Any(x => { return x.ElementPolygonZones.Any(y => y.ZoneUID == Alarm.Zone.UID); }) ||
					FiresecManager.PlansConfiguration.AllPlans.Any(x => { return x.ElementRectangleZones.Any(y => y.ZoneUID == Alarm.Zone.UID); });
			}
			return false;
		}
		public string PlanName
		{
			get
			{
				if (Alarm.Device != null)
				{
					var plan = FiresecManager.PlansConfiguration.AllPlans.FirstOrDefault(x => { return x.ElementDevices.Any(y => y.DeviceUID == Alarm.Device.UID); });
					if(plan != null)
						return plan.Caption;
				}
				if (Alarm.Zone != null)
				{
					var plan = FiresecManager.PlansConfiguration.AllPlans.FirstOrDefault(x => { return x.ElementRectangleZones.Any(y => y.ZoneUID == Alarm.Zone.UID); });
					if (plan != null)
						return plan.Caption;
					plan = FiresecManager.PlansConfiguration.AllPlans.FirstOrDefault(x => { return x.ElementPolygonZones.Any(y => y.ZoneUID == Alarm.Zone.UID); });
					if (plan != null)
						return plan.Caption;
				}
				return null;
			}
		}

		public RelayCommand ShowDeviceCommand { get; private set; }
		void OnShowDevice()
		{
			ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(Alarm.Device.UID);
		}
		bool CanShowDevice()
		{
			return (Alarm.Device != null);
		}

		public RelayCommand ShowZoneCommand { get; private set; }
		void OnShowZone()
		{
			ServiceFactory.Events.GetEvent<ShowZoneEvent>().Publish(Alarm.Zone.UID);
		}
		bool CanShowZone()
		{
			return Alarm.Zone != null;
		}

		public RelayCommand ShowInstructionCommand { get; private set; }
		void OnShowInstruction()
		{
			var instructionViewModel = new InstructionViewModel(Alarm.Device, Alarm.Zone, Alarm.AlarmType);
			DialogService.ShowModalWindow(instructionViewModel);
		}
		bool CanShowInstruction()
		{
			var instructionViewModel = new InstructionViewModel(Alarm.Device, Alarm.Zone, Alarm.AlarmType);
			return instructionViewModel.HasContent;
		}
		public Instruction Instruction
		{
			get
			{
				var instructionViewModel = new InstructionViewModel(Alarm.Device, Alarm.Zone, Alarm.AlarmType);
				return instructionViewModel.Instruction;
			}
		}
	}
}