using System;
using System.Windows;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;

namespace UI
{
	public class MainViewModel : BaseViewModel
	{
		public BootstrapperApplication Bootstrapper { get; set; }

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

		bool _isSQLEnabled;
		public bool IsSQLEnabled
		{
			get { return _isSQLEnabled; }
			set
			{
				_isSQLEnabled = value;
				OnPropertyChanged(() => IsSQLEnabled);
			}
		}

		bool _isInstallSDK;
		public bool IsInstallSDK
		{
			get { return _isInstallSDK; }
			set
			{
				_isInstallSDK = value;
				OnPropertyChanged(() => IsInstallSDK);
			}
		}

		//bool _isInstallOPC;
		//public bool IsInstallOPC
		//{
		//    get { return _isInstallOPC; }
		//    set
		//    {
		//        _isInstallOPC = value;
		//        OnPropertyChanged(() => IsInstallOPC);
		//    }
		//}

		//bool _isInstallGK;
		//public bool IsInstallGK
		//{
		//    get { return _isInstallGK; }
		//    set
		//    {
		//        _isInstallGK = value;
		//        OnPropertyChanged(() => IsInstallGK);
		//    }
		//}

		//bool _isInstallVideo;
		//public bool IsInstallVideo
		//{
		//    get { return _isInstallVideo; }
		//    set
		//    {
		//        _isInstallVideo = value;
		//        OnPropertyChanged(() => IsInstallVideo);
		//    }
		//}
		
		//bool _isInstallFiresec2;
		//public bool IsInstallFiresec2
		//{
		//    get { return _isInstallFiresec2; }
		//    set
		//    {
		//        _isInstallFiresec2 = value;
		//        OnPropertyChanged(() => IsInstallFiresec2);
		//    }
		//}

		bool _isFiresecEnabled;
		public bool IsFiresecEnabled
		{
			get { return _isFiresecEnabled; }
			set
			{
				_isFiresecEnabled = value;
				OnPropertyChanged("IsFiresecEnabled");
			}
		}

		bool _installEnabled;
		public bool InstallEnabled
		{
			get { return _installEnabled; }
			set
			{
				_installEnabled = value;
				OnPropertyChanged("InstallEnabled");
			}
		}

		bool _uninstallEnabled;
		public bool UninstallEnabled
		{
			get { return _uninstallEnabled; }
			set
			{
				_uninstallEnabled = value;
				OnPropertyChanged("UninstallEnabled");
			}
		}

		bool _isThinking;
		public bool IsThinking
		{
			get { return _isThinking; }
			set
			{
				_isThinking = value;
				OnPropertyChanged("IsThinking");
			}
		}
		
		public RelayCommand InstallCommand { get; set; }
		void OnInstall()
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
				if (IsInstallSDK)
					Bootstrapper.Engine.StringVariables["InstallSDK"] = "1";
				//if (IsInstallOPC)
				//    Bootstrapper.Engine.StringVariables["InstallOPC"] = "1";
				//if (IsInstallGK)
				//    Bootstrapper.Engine.StringVariables["InstallGK"] = "1";
				//if (IsInstallSKD)
				//    Bootstrapper.Engine.StringVariables["InstallSKD"] = "1";
				//if (IsInstallVideo)
				//    Bootstrapper.Engine.StringVariables["InstallVideo"] = "1";
				//if (IsInstallFiresec2)
				//    Bootstrapper.Engine.StringVariables["InstallFiresec2"] = "1";
			}
			catch (System.Exception e)
			{
				MessageBox.Show(e.Message);	
			}
			
			IsThinking = true;
			Bootstrapper.Engine.Plan(LaunchAction.Install);
		}

		public RelayCommand UninstallCommand { get; set; }
		void OnUninstall()
		{
			IsThinking = true;
			Bootstrapper.Engine.Plan(LaunchAction.Uninstall);
		}

		public RelayCommand ExitCommand { get; set; }
		void OnExit()
		{
			UIBootstrapper.BootstrapperDispatcher.InvokeShutdown();
		}

		void OnApplyComplete(object sender, ApplyCompleteEventArgs e)
		{
			IsThinking = false;
			InstallEnabled = false;
			UninstallEnabled = false;
		}

		void OnDetectPackageComplete(object sender, DetectPackageCompleteEventArgs e)
		{
			InstallEnabled = true;
			UninstallEnabled = true;
		}

		void OnPlanComplete(object sender, PlanCompleteEventArgs e)
		{
			if (e.Status >= 0)
				Bootstrapper.Engine.Apply(System.IntPtr.Zero);
		}		
	}
}