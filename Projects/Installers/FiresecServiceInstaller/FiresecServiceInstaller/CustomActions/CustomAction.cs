using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Win32;
using System.Diagnostics;

namespace CustomActions
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult CreateSQLInstanceList(Session session)
        {
            session.Log("Begin CustomAction1");
            //Debugger.Break();
            var a = GenSQLInstanceList();
            if (a != null)
            {
                session["SQLInstanceList"] = a;
                return ActionResult.Success;
            }
            else
            {
                session["SQLInstanceList"] = "0";
                return ActionResult.Failure;
            }
            
        }

        public static string GenSQLInstanceList()
        {
            StringBuilder instances = new StringBuilder();
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names");
            foreach (string sk in key.GetSubKeyNames())
            {
                RegistryKey rkey = key.OpenSubKey(sk);

                foreach (string s in rkey.GetValueNames())
                {
                    instances.Append(s);
                    instances.Append("|");
                }
            }
            return instances.ToString();
        }
    }
}
