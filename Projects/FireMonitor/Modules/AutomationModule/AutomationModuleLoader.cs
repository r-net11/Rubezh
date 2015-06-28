using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutomationModule.Events;
using AutomationModule.Plans;
using AutomationModule.ViewModels;
using FiresecAPI;
using FiresecAPI.Automation;
using FiresecAPI.AutomationCallback;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecAPI.Models.Layouts;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Client.Layout;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Infrastructure.Models;
using Infrustructure.Plans.Events;
using Microsoft.Practices.Prism.Events;

namespace AutomationModule
{
	public class AutomationModuleLoader : ModuleBase, ILayoutProviderModule
	{
		PlanPresenter _planPresenter;
		ProceduresViewModel ProceduresViewModel;
		NavigationItem _proceduresNavigationItem;

		public override void CreateViewModels()
		{
			_planPresenter = new PlanPresenter();
			ProceduresViewModel = new ProceduresViewModel();
			ProcessShedule();
		}

		void ProcessShedule()
		{
		}

		public override void Initialize()
		{
			_proceduresNavigationItem.IsVisible = FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures.Count > 0;
			ProceduresViewModel.Initialize();
			_planPresenter.Initialize();
			ServiceFactoryBase.Events.GetEvent<RegisterPlanPresenterEvent<Plan, XStateClass>>().Publish(_planPresenter);
		}

		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			_proceduresNavigationItem = new NavigationItem<ShowAutomationEvent, object>(ProceduresViewModel, ModuleType.ToDescription(), "Video1");
			return new List<NavigationItem>
			{
				_proceduresNavigationItem
			};
		}

		public override ModuleType ModuleType
		{
			get { return ModuleType.Automation; }
		}
		public override void RegisterResource()
		{
			base.RegisterResource();
			ServiceFactoryBase.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
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
						FiresecManager.SystemConfiguration.AutomationConfiguration.AutomationSounds.FirstOrDefault(
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
							FiresecManager.FiresecService.ProcedureCallbackResponse(automationCallbackResult.CallbackUID, confirm);
						}
						else
							MessageBoxService.ShowExtended(messageArguments.Message, "Сообщение", messageArguments.IsModalWindow);
					});
					break;
				case AutomationCallbackType.Property:
				{
					var propertyArguments = (PropertyCallBackData) automationCallbackResult.Data;
					var ShowObjectDetailsEvent = new CompositePresentationEvent<Guid>();
					switch (propertyArguments.ObjectType)
					{
						case ObjectType.Device:
					//		var device = GKManager.Devices.FirstOrDefault(x => x.UID == propertyArguments.ObjectUid);
						//	if (device != null)
							//	ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowGKDeviceDetailsEvent>();
							break;

						case ObjectType.SKDDevice:
							var skdDevice = SKDManager.Devices.FirstOrDefault(x => x.UID == propertyArguments.ObjectUid);
							if (skdDevice != null)
								ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowSKDDeviceDetailsEvent>();
							break;

						case ObjectType.SKDZone:
							var skdZone = SKDManager.Zones.FirstOrDefault(x => x.UID == propertyArguments.ObjectUid);
							if (skdZone != null)
								ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowSKDZoneDetailsEvent>();
							break;

						case ObjectType.VideoDevice:
							var videoDevice = FiresecManager.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == propertyArguments.ObjectUid);
							if (videoDevice != null)
								ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowCameraDetailsEvent>();
							break;

						case ObjectType.Door:
							var door = SKDManager.Doors.FirstOrDefault(x => x.UID == propertyArguments.ObjectUid);
							if (door != null)
								ShowObjectDetailsEvent = ServiceFactory.Events.GetEvent<ShowSKDDoorDetailsEvent>();
							break;

					}
					if (ShowObjectDetailsEvent != null)
						ApplicationService.BeginInvoke(() => ShowObjectDetailsEvent.Publish(propertyArguments.ObjectUid));
				}
				break;
			}
		}

		#region ILayoutProviderModule Members
		public IEnumerable<ILayoutPartPresenter> GetLayoutParts()
		{
#if DEBUG
			yield return new LayoutPartPresenter(LayoutPartIdentities.Automation, "Процедураы", "Procedure.png", p => ProceduresViewModel);
#endif
			yield return new LayoutPartPresenter(LayoutPartIdentities.AutomationProcedure, "Процедура", "Procedure.png", p => new LayoutProcedurePartViewModel((LayoutPartProcedureProperties)p));
			yield return new LayoutPartPresenter(LayoutPartIdentities.TextBlock, "Метка", "Text.png", p => new LayoutTextBlockPartViewModel((LayoutPartTextProperties)p));
			yield return new LayoutPartPresenter(LayoutPartIdentities.TextBox, "Текстовое поле", "Text.png", p => new LayoutTextBoxPartViewModel((LayoutPartTextProperties)p));
		}
		#endregion
	}
}