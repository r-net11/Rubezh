using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using FireMonitor.ViewModels;
using StrazhAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Windows;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout.Serialization;
using LayoutModel = StrazhAPI.Models.Layouts.Layout;
using StrazhAPI.AutomationCallback;
using Infrastructure.Common.Windows.ViewModels;

namespace FireMonitor.Layout.ViewModels
{
	public class MonitorLayoutShellViewModel : MonitorShellViewModel
	{
		private SoundViewModel _soundViewModel;

		public MonitorLayoutShellViewModel(StrazhAPI.Models.Layouts.Layout layout)
			: base(ClientType.Monitor)
		{
			Layout = layout;
			LayoutContainer = new LayoutContainer(this, layout);
			LayoutContainer.LayoutChanging += LayoutChanging;
			ChangeLayoutCommand = new RelayCommand<LayoutModel>(OnChangeLayout, CanChangeLayout);
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
				ribbonViewModel.LogoSource = "Logo";
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
			RibbonContent.Items[2][2].ImageSource = _soundViewModel.IsSoundOn ? "BSound" : "BMute";
			RibbonContent.Items[2][2].ToolTip = _soundViewModel.IsSoundOn ? "Звук включен" : "Звук выключен";
			RibbonContent.Items[2][2].Text = _soundViewModel.IsSoundOn ? "Выключить звук" : "Включить звук";
		}
		private void AddRibbonItem()
		{
			var ip = ConnectionSettingsManager.IsRemote ? null : FiresecManager.GetIP();
			var layouts = FiresecManager.LayoutsConfiguration.Layouts.Where(layout => layout.Users.Contains(FiresecManager.CurrentUser.UID) && (ip == null || layout.HostNameOrAddressList.Contains(ip))).OrderBy(item => item.Caption);
			RibbonContent.Items.Add(new RibbonMenuItemViewModel("Сменить шаблон", new ObservableCollection<RibbonMenuItemViewModel>(layouts.Select(item => new RibbonMenuItemViewModel(item.Caption, ChangeLayoutCommand, item, "BLayouts", item.Description))), "BLayouts"));

			_soundViewModel = new SoundViewModel();
			RibbonContent.Items.Add(new RibbonMenuItemViewModel("Автоактивиция", new ObservableCollection<RibbonMenuItemViewModel>()
			{
				new RibbonMenuItemViewModel(string.Empty, _soundViewModel.PlaySoundCommand) { IsNewGroup = true },
			}, "BConfig"));
			if (AllowClose)
				RibbonContent.Items.Add(new RibbonMenuItemViewModel("Выход", ApplicationCloseCommand, "BExit") { Order = int.MaxValue });
		}

		public RelayCommand<LayoutModel> ChangeLayoutCommand { get; private set; }
		private void OnChangeLayout(LayoutModel layout)
		{
			ApplicationService.CloseAllWindows();
			LayoutContainer.UpdateLayout(layout);
		}
		private bool CanChangeLayout(LayoutModel layout)
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
						FiresecManager.FiresecService.ProcedureCallbackResponse(automationCallbackResult.CallbackUID, value);
				}
			}
			else if (automationCallbackResult.AutomationCallbackType == AutomationCallbackType.Dialog)
				LayoutDialogViewModel.Show((DialogCallbackData)automationCallbackResult.Data);
		}
	}
}