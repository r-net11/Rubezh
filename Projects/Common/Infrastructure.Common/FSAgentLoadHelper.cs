using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Common;
using Microsoft.Win32;

namespace Infrastructure.Common
{
    public class FSAgentLoadHelper
    {
        public static void SetLocation(string path)
        {
            RegistryKey registryKey = Registry.LocalMachine.CreateSubKey("software\\rubezh\\Firesec-2");
			var x = System.Reflection.Assembly.GetExecutingAssembly().Location;
			registryKey.SetValue("FSAgentServerPath", path);
        }

        public static string GetLocation()
        {
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("software\\rubezh\\Firesec-2");
            if (registryKey != null)
            {
                var value = registryKey.GetValue("FSAgentServerPath");
                if (value != null)
                {
                    if (File.Exists((string)value))
                        return (string)value;
                }
            }
            return null;
        }

        public static void SetStatus(FSAgentState fsAgentState)
        {
            RegistryKey registryKey = Registry.LocalMachine.CreateSubKey("software\\rubezh\\Firesec-2");
            registryKey.SetValue("FSAgentServerState", (int)fsAgentState);
        }

        public static FSAgentState GetStatus()
        {
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("software\\rubezh\\Firesec-2");
            if (registryKey != null)
            {
                var value = registryKey.GetValue("FSAgentServerState");
                if (value != null)
                {
                    return (FSAgentState)value;
                }
            }

            return FSAgentState.Closed;
        }

        public static bool WaitUntinlStarted()
        {
            for (int i = 0; i < 100; i++)
            {
                var fsAgentState = GetStatus();
                if (fsAgentState == FSAgentState.Opened)
                    return true;
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
            return false;
        }

        public static bool Load()
        {
            Process[] processes = Process.GetProcessesByName("FSAgentServer");
            if (processes.Count() == 0)
            {
                try
                {
                    SetStatus(FSAgentState.Closed);
                    if (!Start())
                        return false;
                    if (!WaitUntinlStarted())
                        return false;
                }
                catch (Exception e)
                {
                    Logger.Error(e, "FSAgentLoadHelper.Load");
                    return false;
                }
            }
            return true;
        }

        static bool Start()
        {
			var fileName = @"..\FSAgent\FSAgentServer.exe";
			if (!File.Exists(fileName))
			{
				fileName = GetLocation();
				if (fileName == null)
					return false;
			}
            if (!File.Exists(fileName))
            {
				Logger.Error("FSAgentLoadHelper.Start File Not Exist " + fileName);
                return false;
            }
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = fileName;
            process.Start();
            return true;
        }
    }

    public enum FSAgentState
    {
        Closed = 0,
        Opening = 1,
        Opened = 2
    }
}