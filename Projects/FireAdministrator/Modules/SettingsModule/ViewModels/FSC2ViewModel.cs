using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using Microsoft.Win32;
using FiresecClient;
using Infrastructure;
using Infrastructure.Events;
using Common;
using System.Windows;
using Infrastructure.Common.Windows;
using FiresecAPI.Models;
using System.IO;
using System.Runtime.Serialization;

namespace SettingsModule.ViewModels
{
	public class FSC2ViewModel : BaseViewModel
	{
		public FSC2ViewModel()
		{
			LoadFromFileOldCommand = new RelayCommand(OnLoadFromFileOld);
		}

		public RelayCommand LoadFromFileOldCommand { get; private set; }
		public static void OnLoadFromFileOld()
		{
			try
			{
				var openDialog = new OpenFileDialog()
				{
					Filter = "firesec2 files|*.fsc2",
					DefaultExt = "firesec2 files|*.fsc2"
				};
				if (openDialog.ShowDialog().Value)
				{
					WaitHelper.Execute(() =>
					{
						CopyTo(LoadFromFile(openDialog.FileName));

						FiresecManager.UpdateConfiguration();
						XManager.UpdateConfiguration();
						ServiceFactory.Events.GetEvent<ConfigurationChangedEvent>().Publish(null);

						ServiceFactory.Events.GetEvent<ConfigurationChangedEvent>().Publish(null);
						ServiceFactory.Layout.Close();
						ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(Guid.Empty);

						ServiceFactory.SaveService.FSChanged = true;
						ServiceFactory.SaveService.PlansChanged = true;
						ServiceFactory.SaveService.GKChanged = true;
						ServiceFactory.Layout.ShowFooter(null);
					});
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "FSC2ViewModel.LoadFromFileOld");
				MessageBoxService.ShowError(e.Message, "Ошибка при выполнении операции");
			}
		}

		static FullConfiguration LoadFromFile(string fileName)
		{
			try
			{
				FullConfiguration fullConfiguration = null;
				var dataContractSerializer = new DataContractSerializer(typeof(FullConfiguration));
				using (var fileStream = new FileStream(fileName, FileMode.Open))
				{
					fullConfiguration = (FullConfiguration)dataContractSerializer.ReadObject(fileStream);
				}
				if (!fullConfiguration.ValidateVersion())
					SaveToFile(fullConfiguration, fileName);
				return fullConfiguration;
			}
			catch (Exception e)
			{
				Logger.Error(e, "FSC2ViewModel.LoadFromFile");
				return new FullConfiguration();
			}
		}
		static void CopyTo(FullConfiguration fullConfiguration)
		{
			try
			{
				FiresecManager.FiresecConfiguration.DeviceConfiguration = fullConfiguration.DeviceConfiguration;
				if (FiresecManager.FiresecConfiguration.DeviceConfiguration == null)
					FiresecManager.FiresecConfiguration.SetEmptyConfiguration();
				FiresecManager.PlansConfiguration = fullConfiguration.PlansConfiguration;
				FiresecManager.SystemConfiguration = fullConfiguration.SystemConfiguration;
				XManager.DeviceConfiguration = fullConfiguration.XDeviceConfiguration;
				if (XManager.DeviceConfiguration == null)
					XManager.SetEmptyConfiguration();
			}
			catch (Exception e)
			{
				Logger.Error(e, "FSC2ViewModel.CopyTo");
			}
		}
		static void SaveToFile(FullConfiguration fullConfiguration, string fileName)
		{
			try
			{
				var dataContractSerializer = new DataContractSerializer(typeof(FullConfiguration));
				using (var fileStream = new FileStream(fileName, FileMode.Create))
				{
					dataContractSerializer.WriteObject(fileStream, fullConfiguration);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "FSC2ViewModel.SaveToFile");
			}
		}
	}
}