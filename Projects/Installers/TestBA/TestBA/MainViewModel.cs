using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;

namespace Examples.Bootstrapper
{

	public class MainViewModel : BaseViewModel
    {
        //constructor
        public MainViewModel(BootstrapperApplication bootstrapper)
        {
            
            this.IsThinking = false;

            this.Bootstrapper = bootstrapper;
            this.Bootstrapper.ApplyComplete += this.OnApplyComplete;
            this.Bootstrapper.DetectPackageComplete += this.OnDetectPackageComplete;
            this.Bootstrapper.PlanComplete += this.OnPlanComplete;

			InstallCommand = new RelayCommand(OnInstall);
			UninstallCommand = new RelayCommand(OnUninstall);
			ExitCommand = new RelayCommand(OnExit);
        }

        #region Properties

        bool installEnabled;
        public bool InstallEnabled
        {
            get { return installEnabled; }
            set
            {
                installEnabled = value;
                OnPropertyChanged("InstallEnabled");
            }
        }

        bool uninstallEnabled;
        public bool UninstallEnabled
        {
            get { return uninstallEnabled; }
            set
            {
                uninstallEnabled = value;
                OnPropertyChanged("UninstallEnabled");
            }
        }

        bool isThinking;
        public bool IsThinking
        {
            get { return isThinking; }
            set
            {
                isThinking = value;
                OnPropertyChanged("IsThinking");
            }
        }

        public BootstrapperApplication Bootstrapper { get; set; }

        #endregion //Properties

        #region Methods
		/// <summary>
        /// Method that gets invoked when the Bootstrapper ApplyComplete event is fired.
        /// This is called after a bundle installation has completed. Make sure we updated the view.
        /// </summary>
        void OnApplyComplete(object sender, ApplyCompleteEventArgs e)
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
        void OnDetectPackageComplete(object sender, DetectPackageCompleteEventArgs e)
        {
            if (e.PackageId == "DummyInstallationPackageId")
            {
                if (e.State == PackageState.Absent)
                    InstallEnabled = true;

                else if (e.State == PackageState.Present)
                    UninstallEnabled = true;
            }
        }

        /// <summary>
        /// Method that gets invoked when the Bootstrapper PlanComplete event is fired.
        /// If the planning was successful, it instructs the Bootstrapper Engine to 
        /// install the packages.
        /// </summary>
        void OnPlanComplete(object sender, PlanCompleteEventArgs e)
        {
            if (e.Status >= 0)
                Bootstrapper.Engine.Apply(System.IntPtr.Zero);
        }

        #endregion //Methods

        #region RelayCommands

		public RelayCommand InstallCommand { get; private set; }
		void OnInstall()
		{
			IsThinking = true;
			Bootstrapper.Engine.Plan(LaunchAction.Install);
		}
		bool CanInstall()
		{
			return InstallEnabled == true;
		}

		public RelayCommand UninstallCommand { get; private set; }
		void OnUninstall()
        {
            IsThinking = true;
            Bootstrapper.Engine.Plan(LaunchAction.Uninstall);
        }
		bool CanUninstall()
		{
			return UninstallEnabled == true;
		}
		
		public RelayCommand ExitCommand;
		void OnExit()
		{
			TestBA.BootstrapperDispatcher.InvokeShutdown();
		}
        
        #endregion //RelayCommands
    }
}