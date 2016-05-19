using System;
using System.Linq;
using FiresecBootstrapperApplication.Common;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using Microsoft.Win32;

namespace FiresecBootstrapperApplication
{
	public class MainViewModel : BaseViewModel
	{
		BootstrapperApplication _bootstrapper;
		
		public MainViewModel(BootstrapperApplication bootstrapper)
		{
			_bootstrapper = bootstrapper;
			_bootstrapper.ApplyComplete += this.OnApplyComplete;
			_bootstrapper.DetectPackageComplete += this.OnDetectPackageComplete;
			_bootstrapper.PlanComplete += this.OnPlanComplete;

			InstallCommand = new RelayCommand(OnInstall, CanInstall);
			UninstallCommand = new RelayCommand(OnUninstall, CanUninstall);
			ExitCommand = new RelayCommand(OnExit);
			
			using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32))
			{
				var instanceNames = hklm.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL", false).GetValueNames();
				if (!instanceNames.Any(x => x == "FIRESECINSTANCE"))
				{
					CanInstallSQL = true;
					IsInstallSQL = true;
				}
			}
			
			HeaderText = "Установка ОПС Firesec-2";
			_bootstrapper.Engine.Detect();
		}
		
		bool _isInstallEnabled;
		public bool IsInstallEnabled
		{
			get { return _isInstallEnabled; }
			set
			{
				_isInstallEnabled = value;
				OnPropertyChanged(() => IsInstallEnabled);
			}
		}
		
		bool _isUninstallEnabled;
		public bool IsUninstallEnabled
		{
			get { return _isUninstallEnabled; }
			set
			{
				_isUninstallEnabled = value;
				OnPropertyChanged(() => IsUninstallEnabled);
			}
		}

		bool _isThinking;
		public bool IsThinking
		{
			get { return _isThinking; }
			set
			{
				_isThinking = value;
				if (value)
				{
					IsInstallEnabled = false;
					IsUninstallEnabled = false;
				}
				OnPropertyChanged(()=>IsThinking);
			}
		}

		bool _isInstallSQL;
		public bool IsInstallSQL
		{
			get { return _isInstallSQL; }
			set 
			{
				_isInstallSQL = value;
				OnPropertyChanged(() => IsInstallSQL);
			}
		}


		string _headerText;
		public string HeaderText
		{
			get { return _headerText; }
			set
			{
				_headerText = value;
				OnPropertyChanged(() => HeaderText);
			}
		}

		public bool CanInstallSQL{ get; private set; }

		#region BootstrapperApplicationEventHandlers
		void OnApplyComplete(object sender, ApplyCompleteEventArgs e)
		{
			IsThinking = false;
			IsInstallEnabled = false;
			IsUninstallEnabled = false;
			HeaderText = "Готово";
		}
		void OnDetectPackageComplete(object sender, DetectPackageCompleteEventArgs e)
		{
			if (e.PackageId == "FiresecId_1")
			{
				if (e.State == PackageState.Absent)
					IsInstallEnabled = true;
				else if (e.State == PackageState.Present)
					IsUninstallEnabled = true;
			}
		}
		void OnPlanComplete(object sender, PlanCompleteEventArgs e)
		{
			if (e.Status >= 0)
				_bootstrapper.Engine.Apply(System.IntPtr.Zero);
		}
		#endregion
		
		#region RelayCommands
		public RelayCommand InstallCommand { get; private set; }
		void OnInstall()
		{
			IsThinking = true;
			if (IsInstallSQL)
			{
				if (Environment.Is64BitOperatingSystem)
					_bootstrapper.Engine.StringVariables["InstallSQL_64"] = "1";
				else
					_bootstrapper.Engine.StringVariables["InstallSQL"] = "1";
			}
			HeaderText = "Установка . . .";
			_bootstrapper.Engine.Plan(LaunchAction.Install);
		}
		bool CanInstall()
		{
			return IsInstallEnabled == true;
		}

		public RelayCommand UninstallCommand { get; private set; }
		void OnUninstall()
		{
			IsThinking = true;
			HeaderText = "Удаление . . .";
			_bootstrapper.Engine.Plan(LaunchAction.Uninstall);
		}
		bool CanUninstall()
		{
			return IsUninstallEnabled == true;
		}

		public RelayCommand ExitCommand { get; private set; }
		void OnExit()
		{
			FiresecBootstrapperApplication.BootstrapperDispatcher.InvokeShutdown();
		}
		#endregion
	}
}