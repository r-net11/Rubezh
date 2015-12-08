﻿using System.Diagnostics;
using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Windows;

namespace CustomAction
{
	public class CustomActions
	{
		[CustomAction]
		public static ActionResult CloseApplications(Session session)
		{
			session.Log("Begin Kill terminate processes");
			Process[] processes = Process.GetProcesses();
			foreach (var process in processes)
			{
				try
				{
					if ((process.ProcessName == "FiresecService")
						|| (process.ProcessName == "FireMonitor")
						|| (process.ProcessName == "FireAdministrator")
						|| (process.ProcessName == "Revisor")
						|| (process.ProcessName == "GKOPCServer")
						|| (process.ProcessName == "FiresecNTService"))
					{
						process.Kill();
					}
				}
				catch(Exception e)
				{
					//MessageBox.Show(e.ToString());
				}
			}
			return ActionResult.Success;
		}
	}
}