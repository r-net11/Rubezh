using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using FireMonitor.ViewModels;
using RubezhAPI.Models;
using RubezhClient;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using LayoutModel = RubezhAPI.Models.Layouts.Layout;
using RubezhAPI.AutomationCallback;

namespace FireMonitor.Layout.ViewModels
{
	public class MonitorLayoutShellViewModel : MonitorShellViewModel
	{
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
			RibbonContent.Items[2][0].ImageSource = _autoActivationViewModel.IsAutoActivation ? "BWindowNormal" : "BWindowCross";
			RibbonContent.Items[2][0].ToolTip = _autoActivationViewModel.IsAutoActivation ? "Автоматическая активация ВКЛючена" : "Автоматическая активация ВЫКЛючена";
			RibbonContent.Items[2][0].Text = _autoActivationViewModel.IsAutoActivation ? "Выключить автоактивицию" : "Включить автоактивацию";
			RibbonContent.Items[2][1].ImageSource = _autoActivationViewModel.IsPlansAutoActivation ? "BMap" : "BMapOff";
			RibbonContent.Items[2][1].ToolTip = _autoActivationViewModel.IsPlansAutoActivation ? "Автоматическая активация планов ВКЛючена" : "Автоматическая активация планов ВЫКЛючена";
			RibbonContent.Items[2][1].Text = _autoActivationViewModel.IsPlansAutoActivation ? "Выключить автоактивицию плана" : "Включить автоактивацию плана";
			RibbonContent.Items[2][2].ImageSource = _soundViewModel.IsSoundOn ? "BSound" : "BMute";
			RibbonContent.Items[2][2].ToolTip = _soundViewModel.IsSoundOn ? "Звук включен" : "Звук выключен";
			RibbonContent.Items[2][2].Text = _soundViewModel.IsSoundOn ? "Выключить звук" : "Включить звук";
		}
		private void AddRibbonItem()
		{
			RibbonContent.Items.Add(new RibbonMenuItemViewModel("Сменить пользователя", ChangeUserCommand, "BUser"));

			var ip = ConnectionSettingsManager.IsRemote ? null : ClientManager.GetIP();
			var layouts = ClientManager.LayoutsConfiguration.Layouts.Where(layout => 
				layout.Users.Contains(ClientManager.CurrentUser.UID) && 
				(ip == null || layout.HostNameOrAddressList.Contains(ip)) &&
				Bootstrapper.CheckLicense(layout)).OrderBy(item => item.Caption);
			RibbonContent.Items.Add(new RibbonMenuItemViewModel("Сменить шаблон", new ObservableCollection<RibbonMenuItemViewModel>(layouts.Select(item => new RibbonMenuItemViewModel(item.Caption, ChangeLayoutCommand, item, "BLayouts", item.Description))), "BLayouts"));

			_autoActivationViewModel = new AutoActivationViewModel();
			_soundViewModel = new SoundViewModel();
			RibbonContent.Items.Add(new RibbonMenuItemViewModel("Автоактивиция", new ObservableCollection<RibbonMenuItemViewModel>()
			{
				new RibbonMenuItemViewModel(string.Empty, _autoActivationViewModel.ChangeAutoActivationCommand),
				new RibbonMenuItemViewModel(string.Empty, _autoActivationViewModel.ChangePlansAutoActivationCommand),
				new RibbonMenuItemViewModel(string.Empty, _soundViewModel.PlaySoundCommand) { IsNewGroup = true },
			}, "BConfig"));
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
			ApplicationService.CloseAllWindows();
			LayoutContainer.UpdateLayout(layout);
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
		private void OnAutomationCallback(AutomationCallbackResult automationCallbackResult)
		{
			if (automationCallbackResult.AutomationCallbackType == AutomationCallbackType.GetVisualProperty || automationCallbackResult.AutomationCallbackType == AutomationCallbackType.SetVisualProperty)
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
			else if (automationCallbackResult.AutomationCallbackType == AutomationCallbackType.Dialog)
				LayoutDialogViewModel.Show((DialogCallbackData)automationCallbackResult.Data);
		}
	}
}