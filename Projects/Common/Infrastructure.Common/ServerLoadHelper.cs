using System;
using System.Diagnostics;

namespace Infrastructure.Common
{
    public static class ServerLoadHelper
    {
        public static void Load()
        {
            Process[] procs = Process.GetProcessesByName("FiresecService");
            if (procs.Length == 0)
            {
                try
                {
                    System.Diagnostics.Process proc = new System.Diagnostics.Process();
                    proc.StartInfo.FileName = "..\\..\\..\\FiresecService\\bin\\Debug\\" + "FiresecService.exe";
                    proc.Start();
                }
                catch (Exception)
                { }
            }
        }
    }
}
