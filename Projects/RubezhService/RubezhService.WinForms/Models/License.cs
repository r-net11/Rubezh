using RubezhService.Processor;
using Infrastructure.Common;
using Infrastructure.Common.License;
using RubezhAPI.License;
using System;
using System.IO;
using System.Windows.Forms;

namespace RubezhService.Models
{
	public class License
	{
		public License()
		{
			InitialKey = LicenseManager.InitialKey.ToString();
			LicenseInfo = LicenseManager.CurrentLicenseInfo;
			LicenseManager.LicenseChanged += RubezhLicenseManager_LicenseChanged;
		}

		public string InitialKey { get; set; }
		public RubezhLicenseInfo LicenseInfo { get; set; }

		string GetLicensePath()
		{
			return AppDataFolderHelper.GetFile("RubezhService.license");
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
				RubezhLicenseProcessor.SetLicense(LicenseManager.TryLoad(GetLicensePath()));
			}
		}

		void RubezhLicenseManager_LicenseChanged()
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