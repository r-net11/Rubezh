using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Win32;

namespace Infrastructure.Common
{
    public static class RevisorLoadHelper
    {
          public static void Load()
          {
              var saveKey = Registry.LocalMachine.CreateSubKey("software\\rubezh\\Firesec-2");
              if (saveKey != null)
              {
                  var path = saveKey.GetValue("FireMonitorPath");
                  saveKey.SetValue("IsException", false);
              }
              var proc = Process.GetProcessesByName("Revisor");
              var readKey = Registry.LocalMachine.OpenSubKey("software\\rubezh\\Firesec-2");
              if (readKey != null)
              {
                  var revisorpathobj = readKey.GetValue("RevisorPath");
                  string revisorpath = null;
                  if (revisorpathobj != null)
                      revisorpath = revisorpathobj.ToString();
                  if (String.IsNullOrEmpty(revisorpath))
                  {
                      revisorpath = @"Revisor.exe";
#if DEBUG
                    revisorpath = @"..\..\..\Revisor\Revisor\bin\Debug\Revisor.exe";
#endif
                  }
                  if ((proc.Count() == 0) && (Process.GetCurrentProcess().ProcessName != "FireMonitor.vshost"))
                  {
                      Process.Start(revisorpath);
                  }
              }

              if (saveKey != null) saveKey.Close();
          }
    }
}
