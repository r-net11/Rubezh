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
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Windows;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout.Serialization;
using LayoutModel = FiresecAPI.Models.Layouts.Layout;
using FiresecAPI.AutomationCallback;
using Infrastructure.Common.Windows.ViewModels;

namespace FireMonitor.Layout.ViewModels
{
    public class MonitorLayoutShellViewModel : MonitorShellViewModel
    {
        private AutoActivationViewModel _autoActivationViewModel;
        private SoundViewModel _soundViewModel;

        public MonitorLayoutShellViewModel(FiresecAPI.Models.Layouts.Layout layout)
            : base("Monitor.Layout")
        {
            Layout = layout;
            LayoutContainer = new LayoutContainer(this, layout);
            LayoutContainer.LayoutChanging += LayoutChanging;
            ChangeUserCommand = new RelayCommand(OnChangeUser, CanChangeUser);
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
            RibbonContent.Items[2][0].ImageSource = _autoActivationViewModel.IsAutoActivation ? "/Controls;component/Images/BWindowNormal.png" : "/Controls;component/Images/windowCross.png";
            RibbonContent.Items[2][0].ToolTip = _autoActivationViewModel.IsAutoActivation ? "Автоматическая активация ВКЛючена" : "Автоматическая активация ВЫКЛючена";
            RibbonContent.Items[2][0].Text = _autoActivationViewModel.IsAutoActivation ? "Выключить автоактивицию" : "Включить автоактивацию";
            RibbonContent.Items[2][1].ImageSource = _autoActivationViewModel.IsPlansAutoActivation ? "/Controls;component/Images/BMapOn.png" : "/Controls;component/Images/BMapOff.png";
            RibbonContent.Items[2][1].ToolTip = _autoActivationViewModel.IsPlansAutoActivation ? "Автоматическая активация планов ВКЛючена" : "Автоматическая активация планов ВЫКЛючена";
            RibbonContent.Items[2][1].Text = _autoActivationViewModel.IsPlansAutoActivation ? "Выключить автоактивицию плана" : "Включить автоактивацию плана";
            RibbonContent.Items[2][2].ImageSource = _soundViewModel.IsSoundOn ? "/Controls;component/Images/BSound.png" : "/Controls;component/Images/BMute.png";
            RibbonContent.Items[2][2].ToolTip = _soundViewModel.IsSoundOn ? "Звук включен" : "Звук выключен";
            RibbonContent.Items[2][2].Text = _soundViewModel.IsSoundOn ? "Выключить звук" : "Включить звук";
        }
        private void AddRibbonItem()
        {
            RibbonContent.Items.Add(new RibbonMenuItemViewModel("Сменить пользователя", ChangeUserCommand, "/Controls;component/Images/BUser.png"));

            var ip = ConnectionSettingsManager.IsRemote ? null : FiresecManager.GetIP();
            var layouts = FiresecManager.LayoutsConfiguration.Layouts.Where(layout => layout.Users.Contains(FiresecManager.CurrentUser.UID) && (ip == null || layout.HostNameOrAddressList.Contains(ip))).OrderBy(item => item.Caption);
            RibbonContent.Items.Add(new RibbonMenuItemViewModel("Сменить шаблон", new ObservableCollection<RibbonMenuItemViewModel>(layouts.Select(item => new RibbonMenuItemViewModel(item.Caption, ChangeLayoutCommand, item, "/Controls;component/Images/BLayouts.png", item.Description))), "/Controls;component/Images/BLayouts.png"));

            _autoActivationViewModel = new AutoActivationViewModel();
            _soundViewModel = new SoundViewModel();
            RibbonContent.Items.Add(new RibbonMenuItemViewModel("Автоактивиция", new ObservableCollection<RibbonMenuItemViewModel>()
			{
				new RibbonMenuItemViewModel(string.Empty, _autoActivationViewModel.ChangeAutoActivationCommand),
				new RibbonMenuItemViewModel(string.Empty, _autoActivationViewModel.ChangePlansAutoActivationCommand),
				new RibbonMenuItemViewModel(string.Empty, _soundViewModel.PlaySoundCommand) { IsNewGroup = true },
			}, "/Controls;component/Images/BConfig.png"));
            if (AllowClose)
                RibbonContent.Items.Add(new RibbonMenuItemViewModel("Выход", ApplicationCloseCommand, "/Controls;component/Images/BExit.png") { Order = int.MaxValue });
        }

        public RelayCommand ChangeUserCommand { get; private set; }
        private void OnChangeUser()
        {
            ApplicationService.ShutDown();
            Process.Start(Application.ResourceAssembly.Location);
        }
        private bool CanChangeUser()
        {
            return FiresecManager.CheckPermission(PermissionType.Oper_Logout);
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
                var visuaPropertyData = (VisualPropertyData)automationCallbackResult.Data;
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