using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace FiresecUX.ViewModels
{
	public class FiresecUXViewModel : BaseViewModel
	{
		InstallationState _state;
		public InstallationState State
		{
			get { return _state;}
			set
			{
				if (_state != value)
				{
					_state = value;

					// Notify all the properties derived from the state that the state changed.
					base.OnPropertyChanged("State");
					base.OnPropertyChanged("CancelEnabled");
				}
			}
		}

		public InstallationState PreApplyState { get; set; }

		bool _canceled;
		public bool Canceled
		{
			get { return _canceled; }
			set
			{
				_canceled = value;
				OnPropertyChanged("Canceled");
			}
		}

		public FiresecUXViewModel()
		{
			InstallationViewModel = new InstallationViewModel(this);
		}

		public InstallationViewModel InstallationViewModel { get; private set; }
	}
}
