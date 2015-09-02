using System;
using System.Windows.Forms;
using Defender;
using Infrastructure.Common;

namespace FiresecService.LicenseEditor
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
			
			var licenseWrapper = new FiresecLicenseWrapper(InitialKey.Generate());
			labelVersion.Text = "ver. " + licenseWrapper.Version;
			labelVersion.Visible = licenseWrapper.Version != null;
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
                var license = LicenseProcessor.ProcessLoad(openFileDialog.FileName, key);
                if (license == null)
                {
                    MessageBox.Show("Лицензия не загружена", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
					var licenseWrapper = new FiresecLicenseWrapper(license);

					labelVersion.Text = "ver. " + licenseWrapper.Version;
					labelVersion.Visible = licenseWrapper.Version != null;

                    numericUpDownRemoteWorkplacesCount.Value = licenseWrapper.RemoteWorkplacesCount;
					checkBoxFire.Checked = licenseWrapper.Fire;
					checkBoxSecurity.Checked = licenseWrapper.Security;
					checkBoxAccess.Checked = licenseWrapper.Access;
					checkBoxVideo.Checked = licenseWrapper.Video;
					checkBoxOpcServer.Checked = licenseWrapper.OpcServer;						
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
				var licenseWrapper = new FiresecLicenseWrapper(key)
				{
					RemoteWorkplacesCount = (int)numericUpDownRemoteWorkplacesCount.Value,
					Fire = checkBoxFire.Checked,
					Security = checkBoxSecurity.Checked,
					Access = checkBoxAccess.Checked,
					Video = checkBoxVideo.Checked,
					OpcServer = checkBoxOpcServer.Checked
				};
				                
                if (LicenseProcessor.ProcessSave(saveFileDialog.FileName, licenseWrapper.License, key))
                    MessageBox.Show("Лицензия успешно сохранена!");
                else
                    MessageBox.Show("Лицензия не сохранена!", "Ошибка сохранения", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
