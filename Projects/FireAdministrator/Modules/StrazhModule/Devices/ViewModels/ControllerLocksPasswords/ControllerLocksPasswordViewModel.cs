using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StrazhAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace StrazhModule.ViewModels
{
	public class ControllerLocksPasswordViewModel : BaseViewModel
	{
		private readonly ControllerLocksPasswordsViewModel _locksPasswordsViewModel;

		public ControllerLocksPasswordViewModel(ControllerLocksPasswordsViewModel locksPasswordsViewModel)
		{
			_locksPasswordsViewModel = locksPasswordsViewModel;
			EnableLocks();
		}

		private void EnableLocks()
		{
			switch (_locksPasswordsViewModel.Controller.DriverType)
			{
				case SKDDriverType.ChinaController_1:
					_isLock1Enabled = false;
					_isLock2Enabled = false;
					_isLock3Enabled = false;
					_isLock4Enabled = false;
					break;
				case SKDDriverType.ChinaController_2:
					_isLock1Enabled = true;
					_isLock2Enabled = _locksPasswordsViewModel.Controller.DoorType == DoorType.OneWay;
					_isLock3Enabled = false;
					_isLock4Enabled = false;
					break;
				case SKDDriverType.ChinaController_4:
					_isLock1Enabled = true;
					_isLock2Enabled = true;
					_isLock3Enabled = _locksPasswordsViewModel.Controller.DoorType == DoorType.OneWay;
					_isLock4Enabled = _locksPasswordsViewModel.Controller.DoorType == DoorType.OneWay;
					break;
			}
		}

		public bool ValidateIfPasswordAlreadyExists(string password)
		{
			return _locksPasswordsViewModel.ValidateIfPasswordAlreadyExists(this, password);
		}

		private string _password;
		public string Password {
			get { return _password; }
			set
			{
				if (_password == value)
					return;
				_password = value;
				OnPropertyChanged(() => Password);
			}
		}

		private bool _isAppliedToLock1;
		public bool IsAppliedToLock1
		{
			get { return _isAppliedToLock1; }
			set
			{
				if (_isAppliedToLock1 == value)
					return;
				_isAppliedToLock1 = value;
				OnPropertyChanged(() => IsAppliedToLock1);
				_locksPasswordsViewModel.HasChanged = true;
			}
		}

		private bool _isAppliedToLock2;
		public bool IsAppliedToLock2
		{
			get { return _isAppliedToLock2; }
			set
			{
				if (_isAppliedToLock2 == value)
					return;
				_isAppliedToLock2 = value;
				OnPropertyChanged(() => IsAppliedToLock2);
				_locksPasswordsViewModel.HasChanged = true;
			}
		}

		private bool _isAppliedToLock3;
		public bool IsAppliedToLock3
		{
			get { return _isAppliedToLock3; }
			set
			{
				if (_isAppliedToLock3 == value)
					return;
				_isAppliedToLock3 = value;
				OnPropertyChanged(() => IsAppliedToLock3);
				_locksPasswordsViewModel.HasChanged = true;
			}
		}

		private bool _isAppliedToLock4;
		public bool IsAppliedToLock4
		{
			get { return _isAppliedToLock4; }
			set
			{
				if (_isAppliedToLock4 == value)
					return;
				_isAppliedToLock4 = value;
				OnPropertyChanged(() => IsAppliedToLock4);
				_locksPasswordsViewModel.HasChanged = true;
			}
		}

		private bool _isLock1Enabled;
		public bool IsLock1Enabled
		{
			get { return _isLock1Enabled; }
			set
			{
				if (_isLock1Enabled == value)
					return;
				_isLock1Enabled = value;
				OnPropertyChanged(() => IsLock1Enabled);
			}
		}

		private bool _isLock2Enabled;
		public bool IsLock2Enabled
		{
			get { return _isLock2Enabled; }
			set
			{
				if (_isLock2Enabled == value)
					return;
				_isLock2Enabled = value;
				OnPropertyChanged(() => IsLock2Enabled);
			}
		}

		private bool _isLock3Enabled;
		public bool IsLock3Enabled
		{
			get { return _isLock3Enabled; }
			set
			{
				if (_isLock3Enabled == value)
					return;
				_isLock3Enabled = value;
				OnPropertyChanged(() => IsLock3Enabled);
			}
		}

		private bool _isLock4Enabled;
		public bool IsLock4Enabled
		{
			get { return _isLock4Enabled; }
			set
			{
				if (_isLock4Enabled == value)
					return;
				_isLock4Enabled = value;
				OnPropertyChanged(() => IsLock4Enabled);
			}
		}
	}
}
