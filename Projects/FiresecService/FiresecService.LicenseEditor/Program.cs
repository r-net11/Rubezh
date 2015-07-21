using Defender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FiresecService.LicenseEditor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length >= 3)
            {
                var key = InitialKey.FromHexString(args[1]);
                if (key.BinaryValue == null)
                    return;

                int NumberOfUsers;
                if (!int.TryParse(args[2], out NumberOfUsers))
                    return;

                var license = License.Create(key);
                license.Parameters.Add(new LicenseParameter("ProductId", "Продукт", "Firesec Service"));
                license.Parameters.Add(new LicenseParameter("NumberOfUsers", "Количество пользователей", NumberOfUsers));
                license.Parameters.Add(new LicenseParameter("FireAlarm", "Пожарная сигнализация и пожаротушение", args.Any(x => x.Trim().ToLower() == "fa")));
                license.Parameters.Add(new LicenseParameter("SecurityAlarm", "Охранная сигнализация", args.Any(x => x.Trim().ToLower() == "sa")));
                license.Parameters.Add(new LicenseParameter("Skd", "СКД", args.Any(x => x.Trim().ToLower() == "skd")));
                license.Parameters.Add(new LicenseParameter("ControlScripts", "Сценарии управления", args.Any(x => x.Trim().ToLower() == "cs")));
                license.Parameters.Add(new LicenseParameter("OrsServer", "ОРС-Сервер", args.Any(x => x.Trim().ToLower() == "ors")));

                LicenseProcessor.ProcessSave(args[0], license, key);

                return;
            }

            if (args.Length == 1 && args[0].ToLower().Replace("/", "-").Replace(" ", "") == "-gui")
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
        }
    }
}
