using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;

namespace SKDModule.ViewModels
{
	public class ControllerDetailsViewModel : SaveCancelDialogViewModel
	{
		public SKDDevice Device { get; private set; }

		public ControllerDetailsViewModel(SKDDevice device)
		{
			Device = device;
			if (Device.SKDControllerProperty == null)
				Device.SKDControllerProperty = new SKDControllerProperty();

			IsDUControl = Device.SKDControllerProperty.IsDUControl;
			IsEmployeeDU = Device.SKDControllerProperty.IsEmployeeDU;
			IsEmployeeDUTime = Device.SKDControllerProperty.IsEmployeeDUTime;
			IsEmployeeDUZone = Device.SKDControllerProperty.IsEmployeeDUZone;
			IsGuestDU = Device.SKDControllerProperty.IsGuestDU;
			IsGuestDUTime = Device.SKDControllerProperty.IsGuestDUTime;
			IsGuestDUZone = Device.SKDControllerProperty.IsGuestDUZone;
		}

		bool _isDUControl;
		public bool IsDUControl
		{
			get { return _isDUControl; }
			set
			{
				_isDUControl = value;
				OnPropertyChanged("IsDUControl");
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

		protected override bool Save()
		{
			Device.SKDControllerProperty.IsDUControl = IsDUControl;
			Device.SKDControllerProperty.IsEmployeeDU = IsEmployeeDU;
			Device.SKDControllerProperty.IsEmployeeDUTime = IsEmployeeDUTime;
			Device.SKDControllerProperty.IsEmployeeDUZone = IsEmployeeDUZone;
			Device.SKDControllerProperty.IsGuestDU = IsGuestDU;
			Device.SKDControllerProperty.IsGuestDUTime = IsGuestDUTime;
			Device.SKDControllerProperty.IsGuestDUZone = IsGuestDUZone;
			return true;
		}
	}
}