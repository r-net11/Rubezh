using System.IO;
using System.Windows.Forms;
using GenerateKeyApplication.Controls.Interfaces;
using GenerateKeyApplication.Controls.Properties;
using GenerateKeyApplication.Model.Entity;
using Licensing.Services;
using Microsoft.Win32;
using ReactiveUI;
using System;
using System.ComponentModel.Composition;

namespace GenerateKeyApplication.Controls.ViewModels
{
	[Export(typeof(IShell))]
	public class ShellViewModel : Caliburn.Micro.PropertyChangedBase, IShell
	{
		private readonly LicenseGeneratorService _licGenerator;
		private const int BaseLicenseUsersCount = 100;
		private const string LicFileExc = ".lic";
		private const string Dellimiter = "_";
		private const string FolderDellimiter = @"\";
		#region Properties

		private string _resultMessage;

		public string ResultMessage
		{
			get { return _resultMessage; }
			set
			{
				_resultMessage = value;
				NotifyOfPropertyChange(() => ResultMessage);
			}
		}

		private string _resultPathToSave;

		public string ResultPathToSave
		{
			get { return _resultPathToSave; }
			set
			{
				_resultPathToSave = value;
				NotifyOfPropertyChange(() => ResultPathToSave);
			}
		}

		private string _organisationName;

		public string OrganisationName
		{
			get { return _organisationName; }
			set
			{
				_organisationName = value;
				NotifyOfPropertyChange(() => OrganisationName);
			}
		}

		private string _pathToSave;

		public string PathToSave
		{
			get { return _pathToSave; }
			set
			{
				_pathToSave = value;
				NotifyOfPropertyChange(() => PathToSave);
			}
		}

		private ClientLicense _currentLicense;

		public ClientLicense CurrentLicense
		{
			get { return _currentLicense; }
			set
			{
				_currentLicense = value;
				NotifyOfPropertyChange(() => CurrentLicense);
			}
		}

		private string _serialKey;

		public string SerialKey
		{
			get { return _serialKey; }
			private set
			{
				_serialKey = value;
				NotifyOfPropertyChange(() => SerialKey);
			}
		}

		private int? _twoHundredRate;

		public int? TwoHundredRate
		{
			get { return _twoHundredRate; }
			set
			{
				_twoHundredRate = value;
				NotifyOfPropertyChange(() => TwoHundredRate);
			}
		}

		private int? _oneThousandRate;

		public int? OneThousandRate
		{
			get { return _oneThousandRate; }
			set
			{
				_oneThousandRate = value;
				NotifyOfPropertyChange(() => OneThousandRate);
			}
		}

		private int? _fiveThousandRate;

		public int? FiveThousandRate
		{
			get { return _fiveThousandRate; }
			set
			{
				_fiveThousandRate = value;
				NotifyOfPropertyChange(() => FiveThousandRate);
			}
		}

		private int? _tenThousandRate;

		public int? TenThousandRate
		{
			get { return _tenThousandRate; }
			set
			{
				_tenThousandRate = value;
				NotifyOfPropertyChange(() => TenThousandRate);
			}
		}

		private int? _totalUsers;
		public int? TotalUsers
		{
			get { return _totalUsers; }
			private set
			{
				_totalUsers = value;
				NotifyOfPropertyChange(() => TotalUsers);
			}
		}
		#endregion

		[ImportingConstructor]
		public ShellViewModel()
		{
			_licGenerator = new LicenseGeneratorService();
			CurrentLicense = new ClientLicense();
			PathToSave = @"C:\";

			this.WhenAny(x => x.TwoHundredRate, x => x.OneThousandRate, x => x.FiveThousandRate, x => x.TenThousandRate,
				(twoHundredRate, oneThousandRate, fiveThousandRate, tenThousandRate) => new
				{
					Total = 200 * twoHundredRate.Value.GetValueOrDefault(0)
							+ 1000 * oneThousandRate.Value.GetValueOrDefault(0)
							+ 5000 * fiveThousandRate.Value.GetValueOrDefault(0)
							+ 10000 * tenThousandRate.Value.GetValueOrDefault(0)
							+ BaseLicenseUsersCount
				})
				.Subscribe(value =>
				{
					if (value.Total == default(int))
					{
						TotalUsers = null;
						return;
					}

					TotalUsers = value.Total;
				});

			this.WhenAny(x => x.TotalUsers, x => x.Value)
				.Subscribe(value =>
				{
					if (CurrentLicense == null || value == null) return;

					CurrentLicense.TotalUsers = value.GetValueOrDefault(0);
				});

			this.WhenAny(x => x.OrganisationName, x => x.PathToSave, x => x.CurrentLicense.UID,
				(organisationName, pathToSave, licenseUID) =>
					new {OrganisationName = organisationName.Value, PathToSave = pathToSave.Value, LicenseUID = licenseUID.Value})
				.Subscribe(value =>
				{
					if (string.IsNullOrEmpty(value.OrganisationName) && string.IsNullOrEmpty(value.LicenseUID))
					{
						ResultPathToSave = string.Empty;
						return;
					}

					if (!string.IsNullOrEmpty(value.OrganisationName) && !string.IsNullOrEmpty(value.LicenseUID)) //set format like: C:\\folder\orgName_key.lic
					{
						ResultPathToSave = PathToSave + FolderDellimiter + OrganisationName + Dellimiter + CurrentLicense.UID + LicFileExc;
						return;
					}

					ResultPathToSave = PathToSave + FolderDellimiter + OrganisationName + CurrentLicense.UID + LicFileExc; //set format like: C:\\folder\key.lic or C:\\folder\orgName.lic
				});
		}

		public void GenerateKey()
		{
		//	SerialKey = _licGenerator.TestLicenseOutput(CurrentLicense);
			try
			{
				_licGenerator.GenerateLicenseFile(CurrentLicense, ResultPathToSave);
				ResultMessage = Resources.SaveFileSuccess;
			}
			catch (Exception)
			{
				ResultMessage = Resources.SaveFileFault;
			}
		}

		public void ShowSaveDialog()
		{
			var saveDialog = new FolderBrowserDialog();

			if (saveDialog.ShowDialog() == DialogResult.OK)
				PathToSave = saveDialog.SelectedPath;
		}
	}
}