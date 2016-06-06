using System;
using Infrastructure.Common.Windows.ViewModels;
using Localization.Security.ViewModels;

namespace SecurityModule.ViewModels
{
	public class RemoteMachineViewModel : SaveCancelDialogViewModel
	{
		public RemoteMachineViewModel()
		{
			Title = CommonViewModels.RemoteMachine_Title;
		}

		private string _hostname;
		public string Hostname
		{
			get { return _hostname; }
			set
			{
				if (_hostname == value)
					return;
				_hostname = value;
				OnPropertyChanged(() => Hostname);
			}
		}

		private string _address;
		public string Address
		{
			get { return _address; }
			set
			{
				if (_address == value)
					return;
				_address = value;
				OnPropertyChanged(() => Address);
			}
		}

		public string HostNameOrAddress
		{
			get { return (PcIdentificationMode == PcIdentificationMode.Hostname) ? Hostname : Address; }
		}

		private PcIdentificationMode _pcIdentificationMode;
		public PcIdentificationMode PcIdentificationMode
		{
			get { return _pcIdentificationMode; }
			set
			{
				if (_pcIdentificationMode == value)
					return;
				_pcIdentificationMode = value;
				switch (value)
				{
					case PcIdentificationMode.Hostname:
						Address = String.Empty;
						break;
					case PcIdentificationMode.IpAddress:
						Hostname = String.Empty;
						break;
				}
				OnPropertyChanged(() => PcIdentificationMode);
			}
		}
	}

	public enum PcIdentificationMode
	{
		Hostname = 0,
		IpAddress = 1
	}
}