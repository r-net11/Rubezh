using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;

namespace SKDModule.ViewModels
{
	public class ReaderDetailsViewModel : SaveCancelDialogViewModel
	{
		public SKDDevice Device { get; private set; }
		public SKDReaderDUPropertyViewModel DUControl { get; private set; }
		public SKDReaderDUPropertyViewModel DUConversation { get; private set; }

		public ReaderDetailsViewModel(SKDDevice device)
		{
			Device = device;
			Title = "Свойства считывателя " + Device.PresentationName;
			if (Device.SKDReaderProperty == null)
				Device.SKDReaderProperty = new SKDReaderProperty();

			DUControl = new SKDReaderDUPropertyViewModel(Device.SKDReaderProperty.DUControl);
			DUConversation = new SKDReaderDUPropertyViewModel(Device.SKDReaderProperty.DUConversation);
			VerificationTime = Device.SKDReaderProperty.VerificationTime;
		}

		int _verificationTime;
		public int VerificationTime
		{
			get { return _verificationTime; }
			set
			{
				_verificationTime = value;
				OnPropertyChanged("VerificationTime");
			}
		}

		protected override bool Save()
		{
			DUControl.Save();
			DUConversation.Save();
			Device.SKDReaderProperty.VerificationTime = VerificationTime;
			return true;
		}
	}

	public class SKDReaderDUPropertyViewModel : BaseViewModel
	{
		public SKDReaderDUProperty SKDReaderDUProperty { get; private set; }

		public SKDReaderDUPropertyViewModel(SKDReaderDUProperty skdReaderDUProperty)
		{
			SKDReaderDUProperty = skdReaderDUProperty;

			IsDU = SKDReaderDUProperty.IsDU;
			IsEmployeeDU = SKDReaderDUProperty.IsEmployeeDU;
			IsEmployeeDUTime = SKDReaderDUProperty.IsEmployeeDUTime;
			IsEmployeeDUZone = SKDReaderDUProperty.IsEmployeeDUZone;
			IsGuestDU = SKDReaderDUProperty.IsGuestDU;
			IsGuestDUTime = SKDReaderDUProperty.IsGuestDUTime;
			IsGuestDUZone = SKDReaderDUProperty.IsGuestDUZone;
		}

		bool _isDU;
		public bool IsDU
		{
			get { return _isDU; }
			set
			{
				_isDU = value;
				OnPropertyChanged("IsDU");
			}
		}

		bool _isEmployeeDU;
		public bool IsEmployeeDU
		{
			get { return _isEmployeeDU; }
			set
			{
				_isEmployeeDU = value;
				OnPropertyChanged("IsEmployeeDU");
			}
		}

		bool _isEmployeeDUTime;
		public bool IsEmployeeDUTime
		{
			get { return _isEmployeeDUTime; }
			set
			{
				_isEmployeeDUTime = value;
				OnPropertyChanged("IsEmployeeDUTime");
			}
		}

		bool _isEmployeeDUZone;
		public bool IsEmployeeDUZone
		{
			get { return _isEmployeeDUZone; }
			set
			{
				_isEmployeeDUZone = value;
				OnPropertyChanged("IsEmployeeDUZone");
			}
		}

		bool _isGuestDU;
		public bool IsGuestDU
		{
			get { return _isGuestDU; }
			set
			{
				_isGuestDU = value;
				OnPropertyChanged("IsGuestDU");
			}
		}

		bool _isGuestDUTime;
		public bool IsGuestDUTime
		{
			get { return _isGuestDUTime; }
			set
			{
				_isGuestDUTime = value;
				OnPropertyChanged("IsGuestDUTime");
			}
		}

		bool _isGuestDUZone;
		public bool IsGuestDUZone
		{
			get { return _isGuestDUZone; }
			set
			{
				_isGuestDUZone = value;
				OnPropertyChanged("IsGuestDUZone");
			}
		}

		public void Save()
		{
			SKDReaderDUProperty.IsDU = IsDU;
			SKDReaderDUProperty.IsEmployeeDU = IsEmployeeDU;
			SKDReaderDUProperty.IsEmployeeDUTime = IsEmployeeDUTime;
			SKDReaderDUProperty.IsEmployeeDUZone = IsEmployeeDUZone;
			SKDReaderDUProperty.IsGuestDU = IsGuestDU;
			SKDReaderDUProperty.IsGuestDUTime = IsGuestDUTime;
			SKDReaderDUProperty.IsGuestDUZone = IsGuestDUZone;
		}
	}
}