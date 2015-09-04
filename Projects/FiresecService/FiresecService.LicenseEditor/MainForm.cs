using FiresecLicense;
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
                var licenseInfo = FiresecLicenseManager.TryLoad(openFileDialog.FileName, key);
                if (licenseInfo == null)
                {
                    MessageBox.Show("Лицензия не загружена", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
					numericUpDownRemoteWorkplacesCount.Value = licenseInfo.RemoteWorkplacesCount;
					checkBoxFire.Checked = licenseInfo.Fire;
					checkBoxSecurity.Checked = licenseInfo.Security;
					checkBoxAccess.Checked = licenseInfo.Access;
					checkBoxVideo.Checked = licenseInfo.Video;
					checkBoxOpcServer.Checked = licenseInfo.OpcServer;						
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
					Fire = checkBoxFire.Checked,
					Security = checkBoxSecurity.Checked,
					Access = checkBoxAccess.Checked,
					Video = checkBoxVideo.Checked,
					OpcServer = checkBoxOpcServer.Checked
				};

				if (FiresecLicenseManager.TrySave(saveFileDialog.FileName, licenseInfo, key))
                    MessageBox.Show("Лицензия успешно сохранена!");
                else
                    MessageBox.Show("Лицензия не сохранена!", "Ошибка сохранения", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
