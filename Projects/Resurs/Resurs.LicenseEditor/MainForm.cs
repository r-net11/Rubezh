using ResursAPI.License;
using RubezhLicense;
using System;
using System.Windows.Forms;

namespace Resurs.LicenseEditor
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
					numericUpDownDevicesCount.Value = licenseInfo.DevicesCount;
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
				var licenseInfo = new ResursLicenseInfo { DevicesCount = (int)numericUpDownDevicesCount.Value };

				if (LicenseManager.TrySave(saveFileDialog.FileName, licenseInfo, key))
                    MessageBox.Show("Лицензия успешно сохранена!");
                else
                    MessageBox.Show("Лицензия не сохранена!", "Ошибка сохранения", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
