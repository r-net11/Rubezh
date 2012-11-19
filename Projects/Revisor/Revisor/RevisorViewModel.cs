using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Win32;
using Infrastructure.Common;

namespace Revisor
{
    public class RevisorViewModel
    {
        public RevisorViewModel()
        {
            RegistryKey saveKey = Registry.LocalMachine.CreateSubKey("software\\rubezh\\Firesec-2");
            var path = System.Reflection.Assembly.GetExecutingAssembly();
            saveKey.SetValue("RevisorPath", path.Location);
            saveKey.Close();
            StartLifetimeThread();
            StartCommand = new RelayCommand(OnStart);
            StopCommand = new RelayCommand(OnStop);
        }
        public void Inspect()
        {
            while (true)
            {
                try
                {
                    RegistryKey readKey = Registry.LocalMachine.CreateSubKey("software\\rubezh\\Firesec-2");
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

        public RelayCommand StartCommand { get; private set; }
        public void OnStart()
        {
            StartLifetimeThread();
        }

        public RelayCommand StopCommand { get; private set; }
        public void OnStop()
        {
            LifetimeThread.Abort();
            LifetimeThread = null;
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
