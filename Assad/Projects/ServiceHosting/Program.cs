using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using System.Configuration.Install;
using System.ComponentModel;
using System.IO;
using System.Diagnostics;
using Processor;

namespace ServiceHosting
{
    public class WCFServiceHost : ServiceBase
    {
        Controller controller;

        public WCFServiceHost()
        {
            ServiceName = "AssadService";
            controller = new Controller();
        }

        public static void Main()
        {
            ServiceBase.Run(new WCFServiceHost());
        }

        protected override void OnStart(string[] args)
        {
            Process pc = Process.GetCurrentProcess();
            Directory.SetCurrentDirectory
                (pc.MainModule.FileName.Substring(0, pc.MainModule.FileName.LastIndexOf(@"\")));

            base.OnStart(args);
            try
            {
                TestComServer testComServer = new TestComServer();
                testComServer.Test();

                //var x1 = ComServer.ComServer.GetCoreConfig();
                //Controller.Current.Start();
            }
            catch (Exception e)
            {
                WriteLog(e.ToString());
            }
        }

        protected override void OnStop()
        {
            base.OnStop();
            try
            {
                //Controller.Current.Start();
            }
            catch (Exception e)
            {
                WriteLog(e.ToString());
            }
        }

        public void WriteLog(string message)
        {
            StreamWriter writer;
            writer = File.AppendText(@"C:\log.txt");
            writer.WriteLine(message);
            writer.Close();
        }
    }

    // installutil -u "D:\Personal\Assad\Projects\ServiceHosting\bin\Debug\ServiceHosting.exe"
    // installutil -i "D:\Personal\Assad\Projects\ServiceHosting\bin\Debug\ServiceHosting.exe"

    [RunInstaller(true)]
    public class ProjectInstaller : Installer
    {
        private ServiceProcessInstaller process;
        private ServiceInstaller service;

        public ProjectInstaller()
        {
            process = new ServiceProcessInstaller();
            process.Account = ServiceAccount.LocalSystem;
            service = new ServiceInstaller();
            service.ServiceName = "AssadService";
            Installers.Add(process);
            Installers.Add(service);
        }
    }
}
