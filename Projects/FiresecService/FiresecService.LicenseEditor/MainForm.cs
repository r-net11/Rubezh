﻿using RubezhAPI.License;
using RubezhLicense;
using System;
using System.Windows.Forms;

namespace FiresecService.LicenseEditor
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            var key = InitialKey.FromHexString(textBoxKey.Text);
            if (key.BinaryValue == null)
            {
                MessageBox.Show("Неверный формат ключа");
                return;
            }

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var licenseInfo = LicenseManager.TryLoad(openFileDialog.FileName, key);
                if (licenseInfo == null)
                {
                    MessageBox.Show("Лицензия не загружена", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
					numericUpDownRemoteWorkplacesCount.Value = licenseInfo.RemoteWorkplacesCount;
					checkBoxFirefighting.Checked = licenseInfo.HasFirefighting;
					checkBoxGuard.Checked = licenseInfo.HasGuard;
					checkBoxSKD.Checked = licenseInfo.HasSKD;
					checkBoxVideo.Checked = licenseInfo.HasVideo;
					checkBoxOpcServer.Checked = licenseInfo.HasOpcServer;						
                }
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            var key = InitialKey.FromHexString(textBoxKey.Text);
            if (key.BinaryValue == null)
            {
                MessageBox.Show("Неверный формат ключа!");
                return;
            }
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
				var licenseInfo = new FiresecLicenseInfo()
				{
					RemoteWorkplacesCount = (int)numericUpDownRemoteWorkplacesCount.Value,
					HasFirefighting = checkBoxFirefighting.Checked,
					HasGuard = checkBoxGuard.Checked,
					HasSKD = checkBoxSKD.Checked,
					HasVideo = checkBoxVideo.Checked,
					HasOpcServer = checkBoxOpcServer.Checked
				};

				if (LicenseManager.TrySave(saveFileDialog.FileName, licenseInfo, key))
                    MessageBox.Show("Лицензия успешно сохранена!");
                else
                    MessageBox.Show("Лицензия не сохранена!", "Ошибка сохранения", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
