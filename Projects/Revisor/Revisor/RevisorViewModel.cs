using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Win32;

namespace Revisor
{
    public class RevisorViewModel
    {
        public RevisorViewModel()
        {
            Inspect();
        }
        public void Inspect()
        {
            while(true)
            {
                System.Threading.Thread.Sleep(10000);
                RegistryKey readKey = Registry.LocalMachine.OpenSubKey("software\\rubezh\\Firesec-2");
                var firemonitorpath = (string)readKey.GetValue("FireMonitorPath");
                readKey.Close();
                if (!String.IsNullOrEmpty(firemonitorpath))
                {
                    var processes = Process.GetProcessesByName("FireMonitor");
                    var processes2 = Process.GetProcessesByName("FireMonitor.vshost");
                    if ((processes.Count() == 0))// && (processes2.Count() == 0))
                    {
                        var proc = new Process();
                        proc.StartInfo.FileName = firemonitorpath;
                        proc.Start();
                    }
                }
            }
        }
    }
}
