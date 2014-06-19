using System;
using System.Windows;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;

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

			try
			{
				IsSQLEnabled = (Bootstrapper.Engine.StringVariables["InstallSQL"] == "1" || 
					Bootstrapper.Engine.StringVariables["InstallSQL_64"] == "1");
			}
			catch (System.Exception e)
			{
				MessageBox.Show(e.Message);	
			}
			
			IsThinking = false;
		}

		private bool _isSQLEnabled;
		public bool IsSQLEnabled
		{
			get { return _isSQLEnabled; }
			set
			{
				_isSQLEnabled = value;
				OnPropertyChanged(() => IsSQLEnabled);
			}
		}

		private bool _isInstallHasp;
		public bool IsInstallHasp
		{
			get { return _isInstallHasp; }
			set
			{
				_isInstallHasp = value;
				OnPropertyChanged(() => IsInstallHasp);
			}
		}

		private bool _isInstallOPC;
		public bool IsInstallOPC
		{
			get { return _isInstallOPC; }
			set
			{
				_isInstallOPC = value;
				OnPropertyChanged(() => IsInstallOPC);
			}
		}

		private bool _isInstallSDK;
		public bool IsInstallSDK
		{
			get { return _isInstallSDK; }
			set
			{
				_isInstallSDK = value;
				OnPropertyChanged(() => IsInstallSDK);
			}
		}

		private bool _isInstallGK;
		public bool IsInstallGK
		{
			get { return _isInstallGK; }
			set
			{
				_isInstallGK = value;
				OnPropertyChanged(() => IsInstallGK);
			}
		}

		private bool _isInstallSKD;
		public bool IsInstallSKD
		{
			get { return _isInstallSKD; }
			set
			{
				_isInstallSKD = value;
				OnPropertyChanged(() => IsInstallSKD);
			}
		}

		private bool _isInstallVideo;
		public bool IsInstallVideo
		{
			get { return _isInstallVideo; }
			set
			{
				_isInstallVideo = value;
				OnPropertyChanged(() => IsInstallVideo);
			}
		}
		
		private bool _isInstallFiresec2;
		public bool IsInstallFiresec2
		{
			get { return _isInstallFiresec2; }
			set
			{
				_isInstallFiresec2 = value;
				OnPropertyChanged(() => IsInstallFiresec2);
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
				if (IsSQLEnabled)
				{
					if (Environment.Is64BitOperatingSystem)
						Bootstrapper.Engine.StringVariables["InstallSQL_64"] = "1";
					else
						Bootstrapper.Engine.StringVariables["InstallSQL"] = "1";
				}
				if (IsInstallHasp)
					Bootstrapper.Engine.StringVariables["InstallHasp"] = "1";
				if (IsInstallOPC)
					Bootstrapper.Engine.StringVariables["InstallOPC"] = "1";
				if (IsInstallSDK)
					Bootstrapper.Engine.StringVariables["InstallSDK"] = "1";
				if (IsInstallGK)
					Bootstrapper.Engine.StringVariables["InstallGK"] = "1";
				if (IsInstallSKD)
					Bootstrapper.Engine.StringVariables["InstallSKD"] = "1";
				if (IsInstallVideo)
					Bootstrapper.Engine.StringVariables["InstallVideo"] = "1";
				if (IsInstallFiresec2)
					Bootstrapper.Engine.StringVariables["InstallFiresec2"] = "1";
			}
			catch (System.Exception e)
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