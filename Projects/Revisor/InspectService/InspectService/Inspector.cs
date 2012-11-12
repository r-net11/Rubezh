using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using Microsoft.Win32;

namespace InspectService
{
    public partial class Inspector : ServiceBase
    {
        public Inspector()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            StartLifetimeThread();
        }

        protected override void OnStop()
        {
            LifetimeThread.Abort();
            LifetimeThread = null;
        }

        public void Inspect()
        {
            while (true)
            {
                try
                {
                    RegistryKey readKey = Registry.LocalMachine.OpenSubKey("software\\rubezh\\Firesec-2");
                    var firemonitorpath = (string)readKey.GetValue("FireMonitorPath");
                    var isException = readKey.GetValue("isException");
                    readKey.Close();

                    if (!String.IsNullOrEmpty(firemonitorpath))
                    {
                        var processes = Process.GetProcessesByName("FireMonitor");
                        var processes2 = Process.GetProcessesByName("FireMonitor.vshost");
                        if (isException != null)
                            if ((processes.Count() == 0) && (isException.Equals("True")))// && (processes2.Count() == 0))
                            {
                                RegistryKey savekey = Registry.LocalMachine.CreateSubKey("software\\rubezh\\Firesec-2");
                                savekey.SetValue("isAutoConnect", true);
                                Process.Start(firemonitorpath);
                                savekey.Close();
                            }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                if (StopLifetimeEvent.WaitOne(20000))
                    break;
            }
        }

        static AutoResetEvent StopLifetimeEvent;
        Thread LifetimeThread;

        void StartLifetimeThread()
        {
            if (LifetimeThread == null)
            {
                StopLifetimeEvent = new AutoResetEvent(false);
                LifetimeThread = new Thread(Inspect);
                LifetimeThread.Start();
            }
        }

        public void StopLifetimeThread()
        {
            if (StopLifetimeEvent != null)
            {
                StopLifetimeEvent.Set();
            }
            if (LifetimeThread != null)
            {
                LifetimeThread.Join(TimeSpan.FromSeconds(1));
            }
        }
    }
}
