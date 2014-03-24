using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System.Windows;

namespace UI
{
	public class MainViewModel : BaseViewModel
	{
		public BootstrapperApplication Bootstrapper { get; private set; }

		public MainViewModel(BootstrapperApplication bootstrapper)
		{
			InstallCommand = new RelayCommand(OnInstall, () => InstallEnabled == true);
			UninstallCommand = new RelayCommand(OnUninstall, () => UninstallEnabled == true);
			ExitCommand = new RelayCommand(OnExit);

			Bootstrapper = bootstrapper;
			Bootstrapper.ApplyComplete += this.OnApplyComplete;
			Bootstrapper.DetectPackageComplete += this.OnDetectPackageComplete;
			Bootstrapper.PlanComplete += this.OnPlanComplete;

			IsThinking = false;
		}

		private bool _isSQLEnabled;
		public bool IsSQLEnabled
		{
			get { return _isSQLEnabled; }
			set
			{
				_isSQLEnabled = value;
				OnPropertyChanged("IsSQLEnabled");
			}
		}

		private bool _isFiresecEnabled;
		public bool IsFiresecEnabled
		{
			get { return _isFiresecEnabled; }
			set
			{
				_isFiresecEnabled = value;
				OnPropertyChanged("IsFiresecEnabled");
			}
		}

		private bool _installEnabled;
		public bool InstallEnabled
		{
			get { return _installEnabled; }
			set
			{
				_installEnabled = value;
				OnPropertyChanged("InstallEnabled");
			}
		}

		private bool _uninstallEnabled;
		public bool UninstallEnabled
		{
			get { return _uninstallEnabled; }
			set
			{
				_uninstallEnabled = value;
				OnPropertyChanged("UninstallEnabled");
			}
		}

		private bool _isThinking;
		public bool IsThinking
		{
			get { return _isThinking; }
			set
			{
				_isThinking = value;
				OnPropertyChanged("IsThinking");
			}
		}

		public RelayCommand InstallCommand { get; private set; }
		private void OnInstall()
		{
			try
			{
				MessageBox.Show("OnInstall");
				var x = Bootstrapper.Engine.StringVariables["Checkbox1"];
				MessageBox.Show(x);
				x = "0";

				var y = Bootstrapper.Engine.StringVariables["Checkbox1"];
				MessageBox.Show(y);

				var x2 = Bootstrapper.Engine.StringVariables["Checkbox2"];
				MessageBox.Show(x2);
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
			}

			IsThinking = true;
			Bootstrapper.Engine.Plan(LaunchAction.Install);
		}

		public RelayCommand UninstallCommand { get; private set; }
		private void OnUninstall()
		{
			IsThinking = true;
			Bootstrapper.Engine.Plan(LaunchAction.Uninstall);
		}

		public RelayCommand ExitCommand { get; private set; }
		private void OnExit()
		{
			UIBootstrapper.BootstrapperDispatcher.InvokeShutdown();
		}

		/// <summary>
		/// Method that gets invoked when the Bootstrapper ApplyComplete event is fired.
		/// This is called after a bundle installation has completed. Make sure we updated the view.
		/// </summary>
		private void OnApplyComplete(object sender, ApplyCompleteEventArgs e)
		{
			IsThinking = false;
			InstallEnabled = false;
			UninstallEnabled = false;
		}

		/// <summary>
		/// Method that gets invoked when the Bootstrapper DetectPackageComplete event is fired.
		/// Checks the PackageId and sets the installation scenario. The PackageId is the ID
		/// specified in one of the package elements (msipackage, exepackage, msppackage,
		/// msupackage) in the WiX bundle.
		/// </summary>
		private void OnDetectPackageComplete(object sender, DetectPackageCompleteEventArgs e)
		{
			if (e.PackageId == "Firesec2Installer")
			{
				if (e.State == PackageState.Absent)
				{
					InstallEnabled = true;
					IsFiresecEnabled = true;
				}
				else if (e.State == PackageState.Present)
				{
					UninstallEnabled = true;
					IsFiresecEnabled = false;
				}
			}
			if (e.PackageId == "Firesec2Installer")
			{
				if (e.State == PackageState.Absent)
				{
					InstallEnabled = true;
					IsSQLEnabled = true;
				}
				else if (e.State == PackageState.Present)
				{
					UninstallEnabled = true;
					IsSQLEnabled = false;
				}
			}

			InstallEnabled = true;
			UninstallEnabled = true;
		}

		/// <summary>
		/// Method that gets invoked when the Bootstrapper PlanComplete event is fired.
		/// If the planning was successful, it instructs the Bootstrapper Engine to 
		/// install the packages.
		/// </summary>
		private void OnPlanComplete(object sender, PlanCompleteEventArgs e)
		{
			if (e.Status >= 0)
				Bootstrapper.Engine.Apply(System.IntPtr.Zero);
		}		
	}
}