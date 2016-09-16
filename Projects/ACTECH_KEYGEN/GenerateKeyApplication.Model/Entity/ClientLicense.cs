using System;
using GenerateKeyApplication.Model.Enums;
using ReactiveUI;

namespace GenerateKeyApplication.Model.Entity
{
	public class ClientLicense : ReactiveObject
	{
		private string _uid;
		public string UID
		{
			get { return _uid; }
			set { this.RaiseAndSetIfChanged(ref _uid, value); }
		}

		private int? _operatorConnectionsNumber;
		public int? OperatorConnectionsNumber
		{
			get { return _operatorConnectionsNumber; }
			set { this.RaiseAndSetIfChanged(ref _operatorConnectionsNumber, value); }
		}

		private int _totalUsers;
		public int TotalUsers
		{
			get { return _totalUsers; }
			set { this.RaiseAndSetIfChanged(ref _totalUsers, value); }
		}

		private bool _isUnlimitedUsers;
		public bool IsUnlimitedUsers
		{
			get { return _isUnlimitedUsers; }
			set { this.RaiseAndSetIfChanged(ref _isUnlimitedUsers, value); }
		}

		private bool _isEnableURV;
		public bool IsEnabledURV
		{
			get { return _isEnableURV; }
			set { this.RaiseAndSetIfChanged(ref _isEnableURV, value); }
		}

		private bool _isEnabledPhotoVerification;
		public bool IsEnabledPhotoVerification
		{
			get { return _isEnabledPhotoVerification; }
			set { this.RaiseAndSetIfChanged(ref _isEnabledPhotoVerification, value); }
		}

		private bool _isEnabledRVI;
		public bool IsEnabledRVI
		{
			get { return _isEnabledRVI; }
			set { this.RaiseAndSetIfChanged(ref _isEnabledRVI, value); }
		}

		private bool _isEnabledAutomation;
		public bool IsEnabledAutomation
		{
			get { return _isEnabledAutomation; }
			set { this.RaiseAndSetIfChanged(ref _isEnabledAutomation, value); }
		}

		private LicenseType _licenseType;
		public LicenseType LicenseType
		{
			get { return _licenseType; }
			set { this.RaiseAndSetIfChanged(ref _licenseType, value); }
		}

		public bool IsEnabledServer { get { return true; } }


		public DateTime CreateDateTime { get; set; }
	}
}
