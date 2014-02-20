using System.Collections.Generic;
using System.Linq;
using System.Windows;
using FireMonitor.Layout.ViewModels;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Shell = FireMonitor;
using System;
using Common;

namespace FireMonitor.Layout
{
	internal class Bootstrapper : Shell.Bootstrapper
	{
		private FiresecAPI.Models.Layouts.Layout _layout;
		private MonitorLayoutShellViewModel _monitorLayoutShellViewModel;

		public Bootstrapper()
		{
			_layout = null;
		}

		protected override bool Run()
		{
			var result = GetLayout();
			if (!result)
				return false;
			SafeFiresecService.ConfigurationChangedEvent += () => { ApplicationService.Invoke(OnConfigurationChanged); };
			return base.Run();
		}
		protected override ShellViewModel CreateShell()
		{
			_monitorLayoutShellViewModel = new MonitorLayoutShellViewModel(_layout);
			return _layout == null ? base.CreateShell() : _monitorLayoutShellViewModel;
		}

		private FiresecAPI.Models.Layouts.Layout SelectLayout(List<FiresecAPI.Models.Layouts.Layout> layouts)
		{
			layouts.Sort((x, y) => string.Compare(x.Caption, y.Caption));
			Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
			var viewModel = new SelectLayoutViewModel(layouts);
			DialogService.ShowModalWindow(viewModel);
			Application.Current.ShutdownMode = ShutdownMode.OnLastWindowClose;
			return viewModel.SelectedLayout;
		}

		private void OnConfigurationChanged()
		{
			try
			{
				var result = GetLayout();
				if (result && _monitorLayoutShellViewModel != null)
					_monitorLayoutShellViewModel.UpdateLayout(_layout);
			}
			catch (Exception e)
			{
				Logger.Error(e, "FireMonitor.Layout.Bootstrapper.OnConfigurationChanged");
			}
		}
		private bool GetLayout()
		{
			_layout = null;
			var ip = ConnectionSettingsManager.IsRemote ? null : FiresecManager.GetIP();
			var layouts = FiresecManager.LayoutsConfiguration.Layouts.Where(layout => layout.Users.Contains(FiresecManager.CurrentUser.UID) && (ip == null || layout.IPs.Contains(ip))).ToList();
			if (layouts.Count > 0)
			{
				ServiceFactory.ResourceService.AddResource(new ResourceDescription(typeof(Bootstrapper).Assembly, "DataTemplates/Dictionary.xaml"));
				_layout = layouts.Count == 1 ? layouts[0] : SelectLayout(layouts);
				if (_layout == null)
					return false;
			}
			if (_layout == null)
			{
				MessageBoxService.ShowWarning("К сожалению, для Вас нет ни одного доступного макета!");
				return false;
			}
			return true;
		}
	}
}