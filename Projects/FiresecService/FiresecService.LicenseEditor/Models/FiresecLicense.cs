using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Defender;

namespace FiresecService.LicenseEditor.Models
{
    public class FiresecLicense : License
    {
        public string InitialKeyString { get; set; }

        public int NumberOfUsers { get; set; }
        public bool FireAlarm { get; set; }
        public bool SecurityAlarm { get; set; }
        public bool Skd { get; set; }
        public bool ControlScripts { get; set; }
        public bool OrsServer { get; set; }

        public bool Load(string fileName)
        {
            var license = LicenseProcessor.ProcessLoad(fileName, InitialKeyString);
            if (license == null)
            {
                return false;
            }
            else
            {
                var param = license.Parameters.FirstOrDefault(x => x.Id == "ProductId");
                if (param == null || param.Value != "Firesec Service")
                    return false;

                param = license.Parameters.FirstOrDefault(x => x.Id == "NumberOfUsers");
                NumberOfUsers = param == null ? 0 : (int)param.Value;
                param = license.Parameters.FirstOrDefault(x => x.Id == "FireAlarm");
                FireAlarm = param == null ? false : (bool)param.Value;
                param = license.Parameters.FirstOrDefault(x => x.Id == "SecurityAlarm");
                SecurityAlarm = param == null ? false : (bool)param.Value;
                param = license.Parameters.FirstOrDefault(x => x.Id == "Skd");
                Skd = param == null ? false : (bool)param.Value;
                param = license.Parameters.FirstOrDefault(x => x.Id == "ControlScripts");
                ControlScripts = param == null ? false : (bool)param.Value;
                param = license.Parameters.FirstOrDefault(x => x.Id == "OrsServer");
                OrsServer = param == null ? false : (bool)param.Value;

                return true;
            }
        }

        public bool Save(string fileName)
        {
            var license = License.Create(InitialKeyString);

            license.Parameters.Add(new LicenseParameter("ProductId", "Продукт", "Firesec Service"));
            license.Parameters.Add(new LicenseParameter("NumberOfUsers", "Количество пользователей", NumberOfUsers));
            license.Parameters.Add(new LicenseParameter("FireAlarm", "Пожарная сигнализация и пожаротушение", FireAlarm));
            license.Parameters.Add(new LicenseParameter("SecurityAlarm", "Охранная сигнализация", SecurityAlarm));
            license.Parameters.Add(new LicenseParameter("Skd", "СКД", Skd));
            license.Parameters.Add(new LicenseParameter("ControlScripts", "Сценарии управления", ControlScripts));
            license.Parameters.Add(new LicenseParameter("OrsServer", "ОРС-Сервер", OrsServer));

            return LicenseProcessor.ProcessSave(fileName, license, InitialKeyString);
        }
    }
}
