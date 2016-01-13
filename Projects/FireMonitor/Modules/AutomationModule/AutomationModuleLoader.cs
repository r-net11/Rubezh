using AutomationModule.Plans;
using AutomationModule.ViewModels;
using Infrastructure;
using Infrastructure.Automation;
using Infrastructure.Client.Layout;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Infrustructure.Plans.Events;
using Microsoft.Practices.Prism.Events;
using RubezhAPI;
using RubezhAPI.Automation;
using RubezhAPI.AutomationCallback;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhAPI.Models.Layouts;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AutomationModule
{
	public class AutomationModuleLoader : ModuleBase, ILayoutProviderModule
	{
		PlanPresenter _planPresenter;
		NavigationItem _proceduresNavigationItem;

		public override void CreateViewModels()
		{
			_planPresenter = new PlanPresenter();
			ProcessShedule();
		}

		void ProcessShedule()
		{
		}

		public override void Initialize()
		{
			_planPresenter.Initialize();
			ServiceFactoryBase.Events.GetEvent<RegisterPlanPresenterEvent<Plan, XStateClass>>().Publish(_planPresenter);
		}

		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>();
		}
		public override ModuleType ModuleType
		{
			get { return ModuleType.Automation; }
		}
		public override void RegisterResource()
		{
			base.RegisterResource();
			ServiceFactoryBase.ResourceService.AddResource(GetType().Assembly, "DataTemplates/Dictionary.xaml");
		}
		public override void Dispose()
		{
		}

		public override void AfterInitialize()
		{
			SafeFiresecService.AutomationEvent -= OnAutomationCallback;
			SafeFiresecService.AutomationEvent += OnAutomationCallback;
		}

		void OnAutomationCallback(AutomationCallbackResult automationCallbackResult)
		{
			switch (automationCallbackResult.AutomationCallbackType)
			{
				case AutomationCallbackType.Sound:
					var soundArguments = (SoundCallbackData)automationCallbackResult.Data;
					var sound =
						ClientManager.SystemConfiguration.AutomationConfiguration.AutomationSounds.FirstOrDefault(
							x => x.Uid == soundArguments.SoundUID);
					if (sound != null)
						ApplicationService.Invoke(
							() =>
								AlarmPlayerHelper.Play(
									FileHelper.GetSoundFilePath(Path.Combine(ServiceFactoryBase.ContentService.ContentFolder, sound.Uid.ToString())),
									BeeperType.Alarm, false));
					break;
				case AutomationCallbackType.Message:
					var messageArguments = (MessageCallbackData)automationCallbackResult.Data;
					ApplicationService.Invoke(() =>
					{
						if (messageArguments.WithConfirmation)
						{
							var confirm = MessageBoxService.ShowConfirmation(messageArguments.Message, "Сообщение");
							ProcedureExecutionContext.CallbackResponse(FiresecServiceFactory.UID, automationCallbackResult.ContextType, automationCallbackResult.CallbackUID, confirm);
						}
						else
							MessageBoxService.ShowExtended(messageArguments.Message, "Сообщение", messageArguments.IsModalWindow);
					});
					break;
				case AutomationCallbackType.Property:
					{
						var propertyArguments = (PropertyCallBackData)automationCallbackResult.Data;
						var ShowObjectDetailsEvent = new CompositePresentationEvent<Guid>();
						switch (propertyArguments.ObjectType)
						{
							case ObjectType.Device:
								var device = GKManager.Devices.FirstOrDefault(x => x.UID == propertyArguments.ObjectUid);
								if (device != null)
									ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowGKDeviceDetailsEvent>();
								break;

							case ObjectType.Zone:
								var zone = GKManager.Zones.FirstOrDefault(x => x.UID == propertyArguments.ObjectUid);
								if (zone != null)
									ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowGKZoneDetailsEvent>();
								break;

							case ObjectType.Direction:
								var direction = GKManager.Directions.FirstOrDefault(x => x.UID == propertyArguments.ObjectUid);
								if (direction != null)
									ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowGKDirectionDetailsEvent>();
								break;

							case ObjectType.Delay:
								var delay = GKManager.Delays.FirstOrDefault(x => x.UID == propertyArguments.ObjectUid);
								if (delay != null)
									ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowGKDelayDetailsEvent>();
								break;

							case ObjectType.GuardZone:
								var guardZone = GKManager.GuardZones.FirstOrDefault(x => x.UID == propertyArguments.ObjectUid);
								if (guardZone != null)
									ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowGKGuardZoneDetailsEvent>();
								break;

							case ObjectType.VideoDevice:
								var videoDevice = ClientManager.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == propertyArguments.ObjectUid);
								if (videoDevice != null)
									ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowCameraDetailsEvent>();
								break;

							case ObjectType.GKDoor:
								var gkDoor = GKManager.Doors.FirstOrDefault(x => x.UID == propertyArguments.ObjectUid);
								if (gkDoor != null)
									ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowGKDoorDetailsEvent>();
								break;

							case ObjectType.PumpStation:
								var pumpStation = GKManager.PumpStations.FirstOrDefault(x => x.UID == propertyArguments.ObjectUid);
								if (pumpStation != null)
									ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowGKPumpStationDetailsEvent>();
								break;

							case ObjectType.MPT:
								var mpt = GKManager.MPTs.FirstOrDefault(x => x.UID == propertyArguments.ObjectUid);
								if (mpt != null)
									ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowGKMPTDetailsEvent>();
								break;
						}
						if (ShowObjectDetailsEvent != null)
							ApplicationService.BeginInvoke(() => ShowObjectDetailsEvent.Publish(propertyArguments.ObjectUid));
					}
					break;
				case AutomationCallbackType.GetPlanProperty:
					var controlPlanEventArg = new ControlPlanEventArg
					{
						ControlElementType = ControlElementType.Get,
						PlanCallbackData = (PlanCallbackData)automationCallbackResult.Data
					};
					ServiceFactory.Events.GetEvent<ControlPlanEvent>().Publish(controlPlanEventArg);
					ProcedureExecutionContext.CallbackResponse(FiresecServiceFactory.UID, automationCallbackResult.ContextType, automationCallbackResult.CallbackUID, controlPlanEventArg.PlanCallbackData.Value);
					break;
				case AutomationCallbackType.SetPlanProperty:
					controlPlanEventArg = new ControlPlanEventArg
					{
						ControlElementType = ControlElementType.Set,
						PlanCallbackData = (PlanCallbackData)automationCallbackResult.Data
					};
					ServiceFactory.Events.GetEvent<ControlPlanEvent>().Publish(controlPlanEventArg);
					break;
			}
		}

		#region ILayoutProviderModule Members
		public IEnumerable<ILayoutPartPresenter> GetLayoutParts()
		{
			yield return new LayoutPartPresenter(LayoutPartIdentities.AutomationProcedure, "Процедура", "Procedure.png", p => new LayoutProcedurePartViewModel((LayoutPartProcedureProperties)p));
			yield return new LayoutPartPresenter(LayoutPartIdentities.TextBlock, "Метка", "Text.png", p => new LayoutTextBlockPartViewModel((LayoutPartTextProperties)p));
			yield return new LayoutPartPresenter(LayoutPartIdentities.TextBox, "Текстовое поле", "Text.png", p => new LayoutTextBoxPartViewModel((LayoutPartTextProperties)p));
		}
		#endregion
	}
}