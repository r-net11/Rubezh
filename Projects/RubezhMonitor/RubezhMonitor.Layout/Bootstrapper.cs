using RubezhMonitor.Layout.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Shell = RubezhMonitor;

namespace RubezhMonitor.Layout
{
	internal sealed class Bootstrapper : Shell.Bootstrapper
	{
		private Guid? _layoutID;
		private RubezhAPI.Models.Layouts.Layout _layout;
		private MonitorLayoutShellViewModel _monitorLayoutShellViewModel;
		public static Bootstrapper BootstrapperCurrent { get; private set; }
		
		public Bootstrapper()
		{
			BootstrapperCurrent = this;
			_layout = null;
		}

		protected override bool Run()
		{
			var result = GetLayout();
			if (!result)
				return false;
			return base.Run();
		}
		protected override ShellViewModel CreateShell()
		{
			_monitorLayoutShellViewModel = new MonitorLayoutShellViewModel(_layout);
			_monitorLayoutShellViewModel.LayoutContainer.LayoutChanged += _LayoutChanging;
			return _layout == null ? base.CreateShell() : _monitorLayoutShellViewModel;
		}

		protected override Guid? GetLayoutUID()
		{
			return _layout == null ? null : (Guid?)_layout.UID;
		}

		private RubezhAPI.Models.Layouts.Layout SelectLayout(List<RubezhAPI.Models.Layouts.Layout> layouts)
		{
			layouts.Sort((x, y) => string.Compare(x.Caption, y.Caption));
			Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
			var viewModel = new SelectLayoutViewModel(layouts);
			var isSelected = DialogService.ShowModalWindow(viewModel);
			Application.Current.ShutdownMode = ShutdownMode.OnLastWindowClose;
			return isSelected ? viewModel.SelectedLayout : null;
		}

		protected override string GetRestartCommandLineArguments(bool IsRestart = false)
		{
			if (!IsRestart)
				return base.GetRestartCommandLineArguments();
			if (IsRestart)
			{
				var args = base.GetRestartCommandLineArguments();
				if (args.Length > 0)
					args += " ";
				return args + "layout='" + _layout.UID.ToString() + "'";
			}
			return null;
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

		private bool GetLayout()
		{
			_layout = null;
			var ip = ConnectionSettingsManager.IsRemote ? ClientManager.GetIP() : null;
			var localHostName = System.Net.Dns.GetHostName();
			var layouts = ClientManager.LayoutsConfiguration.Layouts.Where(layout =>
				layout.Users.Contains(ClientManager.CurrentUser.UID) &&
				(ip == null || layout.HostNameOrAddressList.Count == 0 || layout.HostNameOrAddressList.Contains(ip)) ||
					layout.HostNameOrAddressList.Any(allowedHostName => string.Compare(allowedHostName, localHostName, true) == 0)).ToList();

			if (layouts.Count > 0)
			{
				if (_layoutID.HasValue)
				{
					_layout = layouts.FirstOrDefault(item => item.UID == _layoutID.Value);
					if (_layout == null)
						_layout = layouts.FirstOrDefault();
				}

				if (_layout == null && layouts.Count == 1)
					_layout = layouts[0];

				if (_layout == null)
				{
					ServiceFactory.ResourceService.AddResource(typeof(Bootstrapper).Assembly, "DataTemplates/Dictionary.xaml");
					_layout = SelectLayout(layouts);
				}

				if (_layout == null)
					return false;
				else
				{
					ShowOnPlanHelper.LayoutUID = _layout.UID;
					return ClientManager.RubezhService.LayoutChanged(RubezhServiceFactory.UID, _layout.UID);
				}
			}

			MessageBoxService.ShowWarning("К сожалению, для Вас нет ни одного доступного макета!");
			return false;
		}

		void _LayoutChanging(object sender, EventArgs e)
		{
			_layout = _monitorLayoutShellViewModel.LayoutContainer.Layout;
		}
	}
}