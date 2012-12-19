using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using Ionic.Zip;
using Microsoft.Win32;
using XFiresecAPI;

namespace FireAdministrator
{
	public static class FileConfigurationHelper
	{
		public static void SaveToFile()
		{
			try
			{
				var saveDialog = new SaveFileDialog()
				{
					Filter = "firesec2 files|*.fscp",
					DefaultExt = "firesec2 files|*.fscp"
				};
				if (saveDialog.ShowDialog().Value)
				{
					WaitHelper.Execute(() =>
					{
						var deviceConfigurationStream = SerializeHelper.Serialize(FiresecManager.FiresecConfiguration.DeviceConfiguration);
						var plansConfigurationStream = SerializeHelper.Serialize(FiresecManager.PlansConfiguration);
						var systemConfigurationStream = SerializeHelper.Serialize(FiresecManager.SystemConfiguration);
						var xDeviceConfigurationStream = SerializeHelper.Serialize(XManager.DeviceConfiguration);

						if (File.Exists(saveDialog.FileName))
							File.Delete(saveDialog.FileName);
						var zip = new ZipFile(saveDialog.FileName);

						if (zip.Entries.FirstOrDefault(x => x.FileName == "DeviceConfiguration.xml") != null)
							zip.RemoveEntry("DeviceConfiguration.xml");
						deviceConfigurationStream.Position = 0;
						zip.AddEntry("DeviceConfiguration.xml", deviceConfigurationStream);

						if (zip.Entries.FirstOrDefault(x => x.FileName == "PlansConfiguration.xml") != null)
							zip.RemoveEntry("PlansConfiguration.xml");
						plansConfigurationStream.Position = 0;
						zip.AddEntry("PlansConfiguration.xml", plansConfigurationStream);

						if (zip.Entries.FirstOrDefault(x => x.FileName == "SystemConfiguration.xml") != null)
							zip.RemoveEntry("SystemConfiguration.xml");
						systemConfigurationStream.Position = 0;
						zip.AddEntry("SystemConfiguration.xml", systemConfigurationStream);

						if (zip.Entries.FirstOrDefault(x => x.FileName == "XDeviceConfiguration.xml") != null)
							zip.RemoveEntry("XDeviceConfiguration.xml");
						xDeviceConfigurationStream.Position = 0;
						zip.AddEntry("XDeviceConfiguration.xml", xDeviceConfigurationStream);

						var configList = new ConfigurationsList();
						configList.Configurations.Add(new FiresecAPI.Models.Configuration() { MajorVersion = 1, MinorVersion = 1, Name = "DeviceConfiguration" });
						configList.Configurations.Add(new FiresecAPI.Models.Configuration() { MajorVersion = 1, MinorVersion = 1, Name = "PlansConfiguration" });
						configList.Configurations.Add(new FiresecAPI.Models.Configuration() { MajorVersion = 1, MinorVersion = 1, Name = "SystemConfiguration" });
						configList.Configurations.Add(new FiresecAPI.Models.Configuration() { MajorVersion = 1, MinorVersion = 1, Name = "XDeviceConfiguration" });
						var ms = SerializeHelper.Serialize(configList);
						ms.Position = 0;
						if (zip.Entries.FirstOrDefault(x => x.FileName == "Info.xml") != null)
							zip.RemoveEntry("Info.xml");
						zip.AddEntry("Info.xml", ms);
						zip.Save(saveDialog.FileName);
					});
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "MenuView.SaveToFile");
				MessageBox.Show(e.Message, "Ошибка при выполнении операции");
			}
		}

		//public static void SaveToFile()
		//{
		//    try
		//    {
		//        var saveDialog = new SaveFileDialog()
		//        {
		//            Filter = "firesec2 files|*.fsc2",
		//            DefaultExt = "firesec2 files|*.fsc2"
		//        };
		//        if (saveDialog.ShowDialog().Value)
		//        {
		//            WaitHelper.Execute(() =>
		//            {
		//                SaveToFile(CopyFrom(), saveDialog.FileName);
		//            });
		//        }
		//    }
		//    catch (Exception e)
		//    {
		//        Logger.Error(e, "MenuView.SaveToFile");
		//        MessageBox.Show(e.Message, "Ошибка при выполнении операции");
		//    }
		//}

		public static void LoadFromFile()
		{
			try
			{
				var openDialog = new OpenFileDialog()
									 {
										 Filter = "firesec2 files|*.fscp",
										 DefaultExt = "firesec2 files|*.fscp"
									 };
				if (openDialog.ShowDialog().Value)
				{
					var ms = new FileStream(openDialog.FileName, FileMode.Open, FileAccess.Read);
					ms.Close();
					WaitHelper.Execute(
						() =>
						GetConfiguration(openDialog.FileName, ms)
							);
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
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "MenuView.LoadFromFile");
				MessageBox.Show(e.Message, "Ошибка при выполнении операции");
			}
		}

		public static void GetConfiguration(string fileFullName, Stream stream)
		{
			var configurationsList = new ConfigurationsList();
			var unzip = ZipFile.Read(fileFullName, new ReadOptions { Encoding = Encoding.GetEncoding("cp866") });
			var xmlstream = new MemoryStream();
			var entry = unzip["Info.xml"];
			if (entry != null)
			{
				entry.Extract(xmlstream);
				xmlstream.Position = 0;
				configurationsList = SerializeHelper.DeSerialize<ConfigurationsList>(xmlstream);
			}

			if (configurationsList == null)
				return;
			if (configurationsList.Configurations.FirstOrDefault(x => (x.Name == "SystemConfiguration") && (x.MajorVersion == 1) && (x.MinorVersion == 1)) != null)
			{
				entry = unzip["SystemConfiguration.xml"];
				if (entry != null)
				{
					xmlstream = new MemoryStream();
					entry.Extract(xmlstream);
					xmlstream.Position = 0;
					FiresecManager.SystemConfiguration = SerializeHelper.DeSerialize<SystemConfiguration>(xmlstream);
				}
				if (FiresecManager.SystemConfiguration == null)
				{
					FiresecManager.SystemConfiguration = new SystemConfiguration();
					Logger.Error("FiresecManager.SystemConfiguration = null");
					LoadingErrorManager.Add("Нулевая системная конфигурация");
				}
			}

			if (configurationsList.Configurations.FirstOrDefault(x => (x.Name == "PlansConfiguration") && (x.MajorVersion == 1) && (x.MinorVersion == 1)) != null)
			{
				entry = unzip["PlansConfiguration.xml"];
				if (entry != null)
				{
					xmlstream = new MemoryStream();
					entry.Extract(xmlstream);
					xmlstream.Position = 0;
					FiresecManager.PlansConfiguration = SerializeHelper.DeSerialize<PlansConfiguration>(xmlstream);
				}

				if (FiresecManager.PlansConfiguration == null)
				{
					FiresecManager.PlansConfiguration = new PlansConfiguration();
					Logger.Error("FiresecManager.PlansConfiguration = null");
					LoadingErrorManager.Add("Нулевая конфигурация графических планов");
				}
			}

			if (configurationsList.Configurations.FirstOrDefault(x => (x.Name == "DeviceConfiguration") && (x.MajorVersion == 1) && (x.MinorVersion == 1)) != null)
			{
				entry = unzip["DeviceConfiguration.xml"];
				if (entry != null)
				{
					xmlstream = new MemoryStream();
					entry.Extract(xmlstream);
					xmlstream.Position = 0;
					var deviceConfiguration = SerializeHelper.DeSerialize<DeviceConfiguration>(xmlstream);

					if (deviceConfiguration == null)
					{
						deviceConfiguration = new DeviceConfiguration();
						Logger.Error("FiresecManager.deviceConfiguration = null");
						LoadingErrorManager.Add("Нулевая конфигурация устройств");
					}
					FiresecManager.FiresecConfiguration.DeviceConfiguration = deviceConfiguration;
				}
			}


			if (configurationsList.Configurations.FirstOrDefault(x => (x.Name == "XDeviceConfiguration") && (x.MajorVersion == 1) && (x.MinorVersion == 1)) != null)
			{
				entry = unzip["XDeviceConfiguration.xml"];
				if (entry != null)
				{
					xmlstream = new MemoryStream();
					entry.Extract(xmlstream);
					xmlstream.Position = 0;
					var xdeviceConfiguration = SerializeHelper.DeSerialize<XDeviceConfiguration>(xmlstream);

					if (xdeviceConfiguration == null)
					{
						xdeviceConfiguration = new XDeviceConfiguration();
						Logger.Error("FiresecManager.xdeviceConfiguration = null");
						LoadingErrorManager.Add("Нулевая конфигурация устройств");
					}
					XManager.DeviceConfiguration = xdeviceConfiguration;
				}
			}
		}

		public static void LoadFromFileOld()
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
				Logger.Error(e, "MenuView.LoadFromFileOld");
				MessageBox.Show(e.Message, "Ошибка при выполнении операции");
			}
		}

		static FullConfiguration CopyFrom()
		{
			try
			{
				return new FullConfiguration()
				{
					DeviceConfiguration = FiresecManager.FiresecConfiguration.DeviceConfiguration,
					PlansConfiguration = FiresecManager.PlansConfiguration,
					SystemConfiguration = FiresecManager.SystemConfiguration,
					XDeviceConfiguration = XManager.DeviceConfiguration,
					Version = new ConfigurationVersion() { MajorVersion = 1, MinorVersion = 1 }
				};
			}
			catch (Exception e)
			{
				Logger.Error(e, "MenuView.CopyFrom");
				throw e;
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
				Logger.Error(e, "MenuView.CopyTo");
				throw e;
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
				Logger.Error(e, "MenuView.LoadFromFile");
				throw e;
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
				Logger.Error(e, "MenuView.SaveToFile");
				throw e;
			}
		}
	}
}