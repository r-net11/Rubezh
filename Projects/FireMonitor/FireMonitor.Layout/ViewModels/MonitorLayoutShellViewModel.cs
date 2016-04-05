using FireMonitor.ViewModels;
using Infrastructure;
using Infrastructure.Automation;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using RubezhAPI.AutomationCallback;
using RubezhAPI.Models;
using RubezhClient;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using LayoutModel = RubezhAPI.Models.Layouts.Layout;

namespace FireMonitor.Layout.ViewModels
{
	public class MonitorLayoutShellViewModel : MonitorShellViewModel
	{
		RibbonMenuItemViewModel autoActivationItems;
		private AutoActivationViewModel _autoActivationViewModel;
		private SoundViewModel _soundViewModel;

		public MonitorLayoutShellViewModel(RubezhAPI.Models.Layouts.Layout layout)
			: base()
		{
			Layout = layout;
			LayoutContainer = new LayoutContainer(this, layout);
			LayoutContainer.LayoutChanging += LayoutChanging;
			ChangeUserCommand = new RelayCommand(OnChangeUser, CanChangeUser);
			ChangeLayoutCommand = new RelayCommand<LayoutModel>(OnChangeLayout, CanChangeLayout);
			Icon = @"..\Monitor.Layout.ico";
			ListenAutomationEvents();
		}

		public LayoutContainer LayoutContainer { get; private set; }
		private void LayoutChanging(object sender, EventArgs e)
		{
			Layout = LayoutContainer.Layout;
			UpdateRibbon();
		}

		private void UpdateRibbon()
		{
			if (Layout.IsRibbonEnabled)
			{
				RibbonContent = new RibbonMenuViewModel();
				var ribbonViewModel = new RibbonViewModel()
				{
					Content = RibbonContent,
				};
				ribbonViewModel.PopupOpened += (s, e) => UpdateRibbonItems();
				ribbonViewModel.LogoSource = "rubezhLogo";
				HeaderMenu = ribbonViewModel;
				AddRibbonItem();
				AllowLogoIcon = false;
			}
			else
			{
				HeaderMenu = null;
				AllowLogoIcon = true;
			}
			RibbonVisible = false;
		}
		private void UpdateRibbonItems()
		{
			autoActivationItems[0].ImageSource = _autoActivationViewModel.IsAutoActivation ? "BWindowNormal" : "BWindowCross";
			autoActivationItems[0].ToolTip = _autoActivationViewModel.IsAutoActivation ? "Автоматическая активация ВКЛючена" : "Автоматическая активация ВЫКЛючена";
			autoActivationItems[0].Text = _autoActivationViewModel.IsAutoActivation ? "Выключить автоактивицию" : "Включить автоактивацию";
			autoActivationItems[1].ImageSource = _autoActivationViewModel.IsPlansAutoActivation ? "BMap" : "BMapOff";
			autoActivationItems[1].ToolTip = _autoActivationViewModel.IsPlansAutoActivation ? "Автоматическая активация планов ВКЛючена" : "Автоматическая активация планов ВЫКЛючена";
			autoActivationItems[1].Text = _autoActivationViewModel.IsPlansAutoActivation ? "Выключить автоактивицию плана" : "Включить автоактивацию плана";
			autoActivationItems[2].ImageSource = _soundViewModel.IsSoundOn ? "BSound" : "BMute";
			autoActivationItems[2].ToolTip = _soundViewModel.IsSoundOn ? "Звук включен" : "Звук выключен";
			autoActivationItems[2].Text = _soundViewModel.IsSoundOn ? "Выключить звук" : "Включить звук";
		}
		private void AddRibbonItem()
		{
			RibbonContent.Items.Add(new RibbonMenuItemViewModel("Сменить пользователя", ChangeUserCommand, "BUser") { Order = 0 });

			var ip = ConnectionSettingsManager.IsRemote ? null : ClientManager.GetIP();
			var layouts = ClientManager.LayoutsConfiguration.Layouts.Where(layout =>
				layout.Users.Contains(ClientManager.CurrentUser.UID) &&
				(ip == null || layout.HostNameOrAddressList.Contains(ip))).OrderBy(item => item.Caption);
			RibbonContent.Items.Add(new RibbonMenuItemViewModel("Сменить шаблон",
				new ObservableCollection<RibbonMenuItemViewModel>(layouts.Select(item => new RibbonMenuItemViewModel(item.Caption, ChangeLayoutCommand, item, "BLayouts", item.Description))),
				"BLayouts") { Order = 1 });

			_autoActivationViewModel = new AutoActivationViewModel();
			_soundViewModel = new SoundViewModel();
			autoActivationItems = new RibbonMenuItemViewModel("Автоактивация", new ObservableCollection<RibbonMenuItemViewModel>()
			{
				new RibbonMenuItemViewModel(string.Empty, _autoActivationViewModel.ChangeAutoActivationCommand),
				new RibbonMenuItemViewModel(string.Empty, _autoActivationViewModel.ChangePlansAutoActivationCommand),
				new RibbonMenuItemViewModel(string.Empty, _soundViewModel.PlaySoundCommand) { IsNewGroup = true },
			}, "BConfig") { Order = 2 };
			RibbonContent.Items.Add(autoActivationItems);

			if (AllowClose)
				RibbonContent.Items.Add(new RibbonMenuItemViewModel("Выход", ApplicationCloseCommand, "BExit") { Order = int.MaxValue });
		}

		public RelayCommand ChangeUserCommand { get; private set; }
		void OnChangeUser()
		{
			ApplicationService.ShutDown();
			Process.Start(Application.ResourceAssembly.Location);
		}
		bool CanChangeUser()
		{
			return ClientManager.CheckPermission(PermissionType.Oper_Logout);
		}

		public RelayCommand<LayoutModel> ChangeLayoutCommand { get; private set; }
		void OnChangeLayout(LayoutModel layout)
		{
			if (ClientManager.FiresecService.LayoutChanged(FiresecServiceFactory.UID, layout == null ? Guid.Empty : layout.UID))
			{
				ShowOnPlanHelper.LayoutUID = layout == null ? Guid.Empty : layout.UID;
				ApplicationService.CloseAllWindows();
				LayoutContainer.UpdateLayout(layout);
			}
			else
				MessageBoxService.ShowError("Не удалось сменить шаблон. Возможно, отсутствует связь с сервером.");
		}
		bool CanChangeLayout(LayoutModel layout)
		{
			return layout != Layout;
		}

		public void ListenAutomationEvents()
		{
			SafeFiresecService.AutomationEvent -= OnAutomationCallback;
			SafeFiresecService.AutomationEvent += OnAutomationCallback;
		}
		void OnAutomationCallback(AutomationCallbackResult automationCallbackResult)
		{
			if (automationCallbackResult.AutomationCallbackType == AutomationCallbackType.GetVisualProperty || automationCallbackResult.AutomationCallbackType == AutomationCallbackType.SetVisualProperty
				&& (AutomationHelper.CheckLayoutFilter(automationCallbackResult, Layout == null ? null : (Guid?)Layout.UID)))
			{
				var visuaPropertyData = (VisualPropertyCallbackData)automationCallbackResult.Data;
				var layoutPart = LayoutContainer.LayoutParts.FirstOrDefault(item => item.UID == visuaPropertyData.LayoutPart);
				if (layoutPart != null)
				{
					var sendResponse = false;
					object value = null;
					ApplicationService.Invoke(() =>
					{
						switch (automationCallbackResult.AutomationCallbackType)
						{
							case AutomationCallbackType.GetVisualProperty:
								value = layoutPart.GetProperty(visuaPropertyData.Property);
								sendResponse = true;
								break;
							case AutomationCallbackType.SetVisualProperty:
								layoutPart.SetProperty(visuaPropertyData.Property, visuaPropertyData.Value);
								break;
						}
					});
					if (sendResponse)
						ClientManager.FiresecService.ProcedureCallbackResponse(automationCallbackResult.CallbackUID, value);
				}
			}
		}
	}
}