using System;
using System.IO;
using System.Windows;
using FiresecService;
using FiresecService.ViewModels;
using Infrastructure.Common;
using Controls.MessageBox;
using System.Configuration;
using System.Diagnostics;
using Common;
namespace FiresecService
{
	public class Bootstrapper
	{
		public bool Run()
		{
			if (!SingleLaunchHelper.Check("FiresecService"))
			{
				//Application.Current.Shutdown();
				System.Environment.Exit(1);
				return false;
			}
			
			try
			{
				InitializeAppSettings();
				var directoryInfo = new DirectoryInfo(Environment.GetCommandLineArgs()[0]);
				Environment.CurrentDirectory = directoryInfo.FullName.Replace(directoryInfo.Name, "");

				var resourceService = new ResourceService();
				resourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));

				var result = FiresecServiceManager.Open();
			}
			catch (Exception e)
			{
				MessageBoxService.ShowException(e);
				//Application.Current.Shutdown();
				System.Environment.Exit(1);
				return false;
			}

			SingleLaunchHelper.KeepAlive();
			return true;
		}

		void InitializeAppSettings()
		{
			AppSettings.OldFiresecLogin = System.Configuration.ConfigurationManager.AppSettings["OldFiresecLogin"] as string;
			AppSettings.OldFiresecPassword = System.Configuration.ConfigurationManager.AppSettings["OldFiresecPassword"] as string;
			AppSettings.ServiceAddress = System.Configuration.ConfigurationManager.AppSettings["ServiceAddress"] as string;
			AppSettings.OverrideFiresec1Config = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["OverrideFiresec1Config"] as string);
#if DEBUG
			AppSettings.IsDebug = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsDebug"] as string);
#endif
		}
	}
}