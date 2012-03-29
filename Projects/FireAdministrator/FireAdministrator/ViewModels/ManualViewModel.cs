using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FireAdministrator.ViewModels
{
    public class ManualViewModel
    {
        public ManualViewModel()
        {
            process = new Process();
            process.StartInfo = new ProcessStartInfo("Manual.pdf");
        }

        Process process;
        void LaunchManual()
        {
            process.Start();
        }
    }
}
