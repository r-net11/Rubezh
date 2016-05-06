using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Common;
using FireMonitor.Layout.ViewModels;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Shell = FireMonitor;

namespace FireMonitor.Layout
{
	internal class Bootstrapper : Shell.Bootstrapper
	{
		private Guid? _layoutID;
		private StrazhAPI.Models.Layouts.Layout _layout;
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
			ServiceFactory.Events.GetEvent<UserChangedEvent>().Unsubscribe(OnUserChanged);
			ServiceFactory.Events.GetEvent<UserChangedEvent>().Subscribe(OnUserChanged);
			return base.Run();
		}
		protected override ShellViewModel CreateShell()
		{
			_monitorLayoutShellViewModel = new MonitorLayoutShellViewModel(_layout);
			return _layout == null ? base.CreateShell() : _monitorLayoutShellViewModel;
		}

		private StrazhAPI.Models.Layouts.Layout SelectLayout(List<StrazhAPI.Models.Layouts.Layout> layouts)
		{
			layouts.Sort((x, y) => string.Compare(x.Caption, y.Caption));
			Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
			var viewModel = new SelectLayoutViewModel(layouts);
			var isSelected = DialogService.ShowModalWindow(viewModel);
			Application.Current.ShutdownMode = ShutdownMode.OnLastWindowClose;
			return isSelected ? viewModel.SelectedLayout : null;
		}
		private void UpdateLayout()
		{
			try
			{
				_layoutID = null;
				var result = GetLayout();
				if (result && _monitorLayoutShellViewModel != null)
					_monitorLayoutShellViewModel.LayoutContainer.UpdateLayout(_layout);
				else
					ApplicationService.ShutDown();
			}
			catch (Exception e)
			{
				Logger.Error(e, "FireMonitor.Layout.Bootstrapper.OnConfigurationChanged");
			}
		}

		protected override string GetRestartCommandLineArguments()
		{
			var args = base.GetRestartCommandLineArguments();
			if (args.Length > 0)
				args += " ";
			return args + "layout='" + _layout.UID.ToString() + "'";
		}
		public override void InitializeCommandLineArguments(string[] args)
		{
			_layoutID = null;
			base.InitializeCommandLineArguments(args);
			if (args != null)
				foreach (var arg in args)
					if (arg.StartsWith("layout='") && arg.EndsWith("'"))
					{
						var layout = arg.Replace("layout='", "");
						layout = layout.Replace("'", "");
						Guid layoutID;
						if (Guid.TryParse(layout, out layoutID))
							_layoutID = layoutID;
					}
		}

		private void OnUserChanged(UserChangedEventArgs userChangedEventArgs)
		{
			UpdateLayout();
		}

		private bool GetLayout()
		{
			_layout = null;
			var ip = ConnectionSettingsManager.IsRemote ? FiresecManager.GetIP() : null;
			var layouts = FiresecManager.LayoutsConfiguration.Layouts.Where(layout => layout.Users.Contains(FiresecManager.CurrentUser.UID) && (ip == null || layout.HostNameOrAddressList.Count == 0 || layout.HostNameOrAddressList.Contains(ip))).ToList();
			if (layouts.Count > 0)
			{
				if (_layoutID.HasValue)
					_layout = layouts.FirstOrDefault(item => item.UID == _layoutID.Value);

				if (_layout == null && layouts.Count == 1)
					_layout = layouts[0];

				if (_layout == null)
				{
					ServiceFactory.ResourceService.AddResource(new ResourceDescription(typeof(Bootstrapper).Assembly, "DataTemplates/Dictionary.xaml"));
					_layout = SelectLayout(layouts);
				}

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