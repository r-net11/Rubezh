using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using XFiresecAPI;
using Infrastructure.Common.Windows;
using FiresecClient;
using System.Linq;
using System.Collections.ObjectModel;
using Infrustructure.Plans.Elements;

namespace GKModule.ViewModels
{
	public class AlarmViewModel : BaseViewModel
	{
		public Alarm Alarm { get; private set; }

		public AlarmViewModel(Alarm alarm)
		{
			ShowObjectOrPlanCommand = new RelayCommand(OnShowObjectOrPlan);
			ShowObjectCommand = new RelayCommand(OnShowObject);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			ResetCommand = new RelayCommand(OnReset, CanReset);
			ResetIgnoreCommand = new RelayCommand(OnResetIgnore, CanResetIgnore);
			TurnOnAutomaticCommand = new RelayCommand(OnTurnOnAutomatic, CanTurnOnAutomatic);
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties, CanShowProperties);
			ShowInstructionCommand = new RelayCommand(OnShowInstruction, CanShowInstruction);
			Alarm = alarm;
			InitializePlans();
		}

		public string ObjectName
		{
			get
			{
				if (Alarm.Device != null)
                    return Alarm.Device.PresentationDriverAndAddress;
				if (Alarm.Zone != null)
					return Alarm.Zone.PresentationName;
				if (Alarm.Direction != null)
					return Alarm.Direction.PresentationName;
				return null;
			}
		}

		public string ImageSource
		{
			get
			{
				if (Alarm.Device != null)
					return Alarm.Device.Driver.ImageSource;
				if (Alarm.Zone != null)
					return "/Controls;component/Images/zone.png";
				if (Alarm.Direction != null)
					return "/Controls;component/Images/Blue_Direction.png";
				return null;
			}
		}

		public XStateClass ObjectStateClass
		{
			get
			{
				if (Alarm.Device != null)
					return Alarm.Device.DeviceState.StateClass;
				if (Alarm.Zone != null)
					return Alarm.Zone.ZoneState.StateClass;
				if (Alarm.Direction != null)
					return Alarm.Direction.DirectionState.StateClass;
				return XStateClass.Norm;
			}
		}

		public ObservableCollection<PlanLinkViewModel> Plans { get; private set; }

		void InitializePlans()
		{
			Plans = new ObservableCollection<PlanLinkViewModel>();

			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
			{
				ElementBase elementBase;
				if (Alarm.Device != null)
				{
					elementBase = plan.ElementXDevices.FirstOrDefault(x => x.XDeviceUID == Alarm.Device.UID);
					if (elementBase != null)
					{
						var alarmPlanViewModel = new PlanLinkViewModel(plan, elementBase);
						alarmPlanViewModel.Device = Alarm.Device;
						Plans.Add(alarmPlanViewModel);
					}
				}
				if (Alarm.Zone != null)
				{
					elementBase = plan.ElementRectangleXZones.FirstOrDefault(x => x.ZoneUID == Alarm.Zone.UID);
					if (elementBase != null)
					{
						var alarmPlanViewModel = new PlanLinkViewModel(plan, elementBase);
						alarmPlanViewModel.Zone = Alarm.Zone;
						Plans.Add(alarmPlanViewModel);
						continue;
					}

					elementBase = plan.ElementPolygonXZones.FirstOrDefault(x => x.ZoneUID == Alarm.Zone.UID);
					if (elementBase != null)
					{
						var alarmPlanViewModel = new PlanLinkViewModel(plan, elementBase);
						alarmPlanViewModel.Zone = Alarm.Zone;
						Plans.Add(alarmPlanViewModel);
					}
				}
				if (Alarm.Direction != null)
				{
					elementBase = plan.ElementRectangleXDirections.FirstOrDefault(x => x.DirectionUID == Alarm.Direction.UID);
					if (elementBase != null)
					{
						var alarmPlanViewModel = new PlanLinkViewModel(plan, elementBase);
						alarmPlanViewModel.Direction = Alarm.Direction;
						Plans.Add(alarmPlanViewModel);
						continue;
					}

					elementBase = plan.ElementPolygonXDirections.FirstOrDefault(x => x.DirectionUID == Alarm.Direction.UID);
					if (elementBase != null)
					{
						var alarmPlanViewModel = new PlanLinkViewModel(plan, elementBase);
						alarmPlanViewModel.Direction = Alarm.Direction;
						Plans.Add(alarmPlanViewModel);
					}
				}
			}
		}

		public RelayCommand ShowObjectOrPlanCommand { get; private set; }
		void OnShowObjectOrPlan()
		{
			if (CanShowOnPlan())
				OnShowOnPlan();
			else
				OnShowObject();
		}

		public RelayCommand ShowObjectCommand { get; private set; }
		void OnShowObject()
		{
			if (Alarm.Device != null)
			{
				ServiceFactory.Events.GetEvent<ShowXDeviceEvent>().Publish(Alarm.Device.UID);
			}
			if (Alarm.Zone != null)
			{
				ServiceFactory.Events.GetEvent<ShowXZoneEvent>().Publish(Alarm.Zone.UID);
			}
			if (Alarm.Direction != null)
			{
				ServiceFactory.Events.GetEvent<ShowXDirectionEvent>().Publish(Alarm.Direction.UID);
			}
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			if (Alarm.Device != null)
			{
				ShowOnPlanHelper.ShowDevice(Alarm.Device);
			}
			if (Alarm.Zone != null)
			{
				ShowOnPlanHelper.ShowZone(Alarm.Zone);
			}
			if (Alarm.Direction != null)
			{
				ShowOnPlanHelper.ShowDirection(Alarm.Direction);
			}
		}
		bool CanShowOnPlan()
		{
			if (Alarm.Device != null)
			{
				return ShowOnPlanHelper.CanShowDevice(Alarm.Device);
			}
			if (Alarm.Zone != null)
			{
				return ShowOnPlanHelper.CanShowZone(Alarm.Zone);
			}
			if (Alarm.Direction != null)
			{
				return ShowOnPlanHelper.CanShowDirection(Alarm.Direction);
			}
			return false;
		}

		public RelayCommand ResetCommand { get; private set; }
		void OnReset()
		{
			if (Alarm.Zone != null)
			{
				switch (Alarm.AlarmType)
				{
					case XAlarmType.Fire1:
						ObjectCommandSendHelper.ResetFire1(Alarm.Zone);
						break;

					case XAlarmType.Fire2:
						ObjectCommandSendHelper.ResetFire2(Alarm.Zone);
						break;
				}
			}
			if (Alarm.Device != null)
			{
				ObjectCommandSendHelper.Reset(Alarm.Device);
			}
		}
		bool CanReset()
		{
			if (Alarm.Zone != null)
				return (Alarm.AlarmType == XAlarmType.Fire1 || Alarm.AlarmType == XAlarmType.Fire2);
			if (Alarm.Device != null)
				return Alarm.Device.DeviceState.StateClasses.Contains(XStateClass.Fire2) || Alarm.Device.DeviceState.StateClasses.Contains(XStateClass.Fire1);
			return false;
		}
		public bool CanResetCommand
		{
			get { return CanReset(); }
		}

		public RelayCommand ResetIgnoreCommand { get; private set; }
		void OnResetIgnore()
		{
			if (Alarm.Device != null)
			{
				if (Alarm.Device.DeviceState.StateClasses.Contains(XStateClass.Ignore))
				{
					ObjectCommandSendHelper.SetAutomaticRegime(Alarm.Device);
				}
			}

			if (Alarm.Zone != null)
			{
				if (Alarm.Zone.ZoneState.StateClasses.Contains(XStateClass.Ignore))
				{
					ObjectCommandSendHelper.SetAutomaticRegime(Alarm.Zone);
				}
			}

			if (Alarm.Direction != null)
			{
				if (Alarm.Direction.DirectionState.StateClasses.Contains(XStateClass.Ignore))
				{
					ObjectCommandSendHelper.SetAutomaticRegime(Alarm.Direction);
				}
			}
		}
		bool CanResetIgnore()
		{
			if (Alarm.AlarmType != XAlarmType.Ignore)
				return false;

			if (Alarm.Device != null)
			{
				if (Alarm.Device.DeviceState.StateClasses.Contains(XStateClass.Ignore))
					return true;
			}

			if (Alarm.Zone != null)
			{
				if (Alarm.Zone.ZoneState.StateClasses.Contains(XStateClass.Ignore))
					return true;
			}

			if (Alarm.Direction != null)
			{
				if (Alarm.Direction.DirectionState.StateClasses.Contains(XStateClass.Ignore))
					return true;
			}
			return false;
		}
		public bool CanResetIgnoreCommand
		{
			get { return CanResetIgnore(); }
		}

		public RelayCommand TurnOnAutomaticCommand { get; private set; }
		void OnTurnOnAutomatic()
		{
			if (Alarm.Device != null)
			{
				if (!Alarm.Device.DeviceState.StateBits.Contains(XStateBit.Norm))
				{
					ObjectCommandSendHelper.SetAutomaticRegime(Alarm.Device);
				}
			}
			if (Alarm.Direction != null)
			{
				if (!Alarm.Direction.DirectionState.StateBits.Contains(XStateBit.Norm))
				{
					ObjectCommandSendHelper.SetAutomaticRegime(Alarm.Direction);
				}
			}
		}
		bool CanTurnOnAutomatic()
		{
			if (Alarm.AlarmType != XAlarmType.AutoOff)
				return false;

			if (Alarm.Device != null)
			{
				if (!Alarm.Device.Driver.IsControlDevice)
					return false;

				if (!Alarm.Device.DeviceState.StateBits.Contains(XStateBit.Norm))
					return true;
			}
			if (Alarm.Direction != null)
			{
				if (!Alarm.Direction.DirectionState.StateBits.Contains(XStateBit.Norm))
					return true;
			}
			return false;
		}
		public bool CanTurnOnAutomaticCommand
		{
			get { return CanTurnOnAutomatic(); }
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			var showXArchiveEventArgs = new ShowXArchiveEventArgs()
			{
				Device = Alarm.Device,
				Zone = Alarm.Zone,
				Direction = Alarm.Direction
			};
			ServiceFactory.Events.GetEvent<ShowXArchiveEvent>().Publish(showXArchiveEventArgs);
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			if (Alarm.Device != null)
			{
				ServiceFactory.Events.GetEvent<ShowXDeviceDetailsEvent>().Publish(Alarm.Device.UID);
			}
			if (Alarm.Zone != null)
			{
				DialogService.ShowWindow(new ZoneDetailsViewModel(Alarm.Zone));
			}
			if (Alarm.Direction != null)
			{
				DialogService.ShowWindow(new DirectionDetailsViewModel(Alarm.Direction));
			}
		}
		bool CanShowProperties()
		{
			return (Alarm.Device != null) || (Alarm.Direction != null) || (Alarm.Zone != null);
		}

		public RelayCommand ShowInstructionCommand { get; private set; }
		void OnShowInstruction()
		{
			var instructionViewModel = new InstructionViewModel(Alarm.Device, Alarm.Zone, Alarm.Direction, Alarm.AlarmType);
			DialogService.ShowModalWindow(instructionViewModel);
		}
		bool CanShowInstruction()
		{
            var instructionViewModel = new InstructionViewModel(Alarm.Device, Alarm.Zone, Alarm.Direction, Alarm.AlarmType);
			return instructionViewModel.HasContent;
		}
		public bool CanShowInstructionCommand
		{
			get { return CanShowInstruction(); }
		}
		public XInstruction Instruction
		{
			get
			{
                var instructionViewModel = new InstructionViewModel(Alarm.Device, Alarm.Zone, Alarm.Direction, Alarm.AlarmType);
				return instructionViewModel.Instruction;
			}
		}
	}
}