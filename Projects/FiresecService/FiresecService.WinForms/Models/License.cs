using FiresecService.Processor;
using Infrastructure.Common;
using Infrastructure.Common.License;
using RubezhAPI.License;
using System;
using System.IO;
using System.Windows.Forms;

namespace FiresecService.Models
{
	public class License
	{
		public License()
		{
			InitialKey = LicenseManager.InitialKey.ToString();
			LicenseInfo = LicenseManager.CurrentLicenseInfo;
			LicenseManager.LicenseChanged += FiresecLicenseManager_LicenseChanged;
		}

		public string InitialKey { get; set; }
		public FiresecLicenseInfo LicenseInfo { get; set; }

		string GetLicensePath()
		{
			return AppDataFolderHelper.GetFile("FiresecService.license");
		}

		public void OnLoadLicenseCommand()
		{
			var openFileDialog = new OpenFileDialog()
			{
				Filter = "Файл лицензии (*.license)|*.license"
			};
			if (DialogResult.OK == openFileDialog.ShowDialog())
			{
				if (!LicenseManager.CheckLicense(openFileDialog.FileName))
				{
					MessageBox.Show("Некорректный файл лицензии", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				try
				{
					File.Copy(openFileDialog.FileName, GetLicensePath(), true);
				}
				catch (Exception e)
				{
					MessageBox.Show("Ошибка копирования файла лицензии.\n" + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				FiresecLicenseProcessor.SetLicense(LicenseManager.TryLoad(GetLicensePath()));
			}
		}

		void FiresecLicenseManager_LicenseChanged()
		{
			LicenseInfo = LicenseManager.CurrentLicenseInfo;
			OnLicenseChanged();
		}

		void OnLicenseChanged()
		{
			if (LicenseChanged != null)
			{
				LicenseChanged(this, new EventArgs());
			}
		}

		public event EventHandler LicenseChanged;
	}
}