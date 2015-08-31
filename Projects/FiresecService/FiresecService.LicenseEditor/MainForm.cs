using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Defender;

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
                MessageBox.Show("Неверный формат ключа!");
                return;
            }

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var license = LicenseProcessor.ProcessLoad(openFileDialog.FileName, key);
                if (license == null)
                {
                    MessageBox.Show("Лицензия не загружена!", "Ошибка загрузки", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    var param = license.Parameters.FirstOrDefault(x => x.Id == "ProductId");
                    if (param == null || param.Value.ToString() != "Firesec Service")
                    {
                        MessageBox.Show("Неподходящая лицензия!");
                        return;
                    }

                    param = license.Parameters.FirstOrDefault(x => x.Id == "NumberOfUsers");
                    numericUpDownNumberOfUsers.Value = param == null ? 0 : (int)param.Value;
                    param = license.Parameters.FirstOrDefault(x => x.Id == "FireAlarm");
                    checkBoxFireAlarm.Checked = param == null ? false : (bool)param.Value;
                    param = license.Parameters.FirstOrDefault(x => x.Id == "SecurityAlarm");
                    checkBoxSecurityAlarm.Checked = param == null ? false : (bool)param.Value;
                    param = license.Parameters.FirstOrDefault(x => x.Id == "Skd");
                    checkBoxSkd.Checked = param == null ? false : (bool)param.Value;
                    param = license.Parameters.FirstOrDefault(x => x.Id == "ControlScripts");
                    checkBoxControlScripts.Checked = param == null ? false : (bool)param.Value;
                    param = license.Parameters.FirstOrDefault(x => x.Id == "OrsServer");
                    checkBoxOpcServer.Checked = param == null ? false : (bool)param.Value;
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
                var license = License.Create(key);

                license.Parameters.Add(new LicenseParameter("ProductId", "Продукт", "Firesec Service"));
                license.Parameters.Add(new LicenseParameter("NumberOfUsers", "Количество пользователей", Convert.ToInt32(numericUpDownNumberOfUsers.Value)));
                license.Parameters.Add(new LicenseParameter("FireAlarm", "Пожарная сигнализация и пожаротушение", checkBoxFireAlarm.Checked));
                license.Parameters.Add(new LicenseParameter("SecurityAlarm", "Охранная сигнализация", checkBoxSecurityAlarm.Checked));
                license.Parameters.Add(new LicenseParameter("Skd", "СКД", checkBoxSkd.Checked));
                license.Parameters.Add(new LicenseParameter("ControlScripts", "Сценарии управления", checkBoxControlScripts.Checked));
                license.Parameters.Add(new LicenseParameter("OrsServer", "ОРС-Сервер", checkBoxOpcServer.Checked));

                if (LicenseProcessor.ProcessSave(saveFileDialog.FileName, license, key))
                {
                    MessageBox.Show("Лицензия успешно сохранена!");
                }
                else
                {
                    MessageBox.Show("Лицензия не сохранена!", "Ошибка сохранения", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
