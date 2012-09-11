using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using FiresecAPI.Models;
using FiresecClient;

namespace DeviveModelManager
{
	public class TreeBuilder
	{
		public TreeItem RootTreeItem { get; private set; }

		public void Build()
		{
            var FS_Address = ConfigurationManager.AppSettings["FS_Address"] as string;
            var FS_Port = Convert.ToInt32(ConfigurationManager.AppSettings["FS_Port"] as string);
            var FS_Login = ConfigurationManager.AppSettings["FS_Login"] as string;
            var FS_Password = ConfigurationManager.AppSettings["FS_Password"] as string;
            var serverAddress = ConfigurationManager.AppSettings["ServiceAddress"] as string;
            var Login = ConfigurationManager.AppSettings["Login"] as string;
            var Password = ConfigurationManager.AppSettings["Password"] as string;

            FiresecManager.Connect(ClientType.Assad, serverAddress, Login, Password);
            FiresecManager.GetConfiguration(false, FS_Address, FS_Port, FS_Login, FS_Password);

			RootTreeItem = RootHelper.CreateRoot();
			RootTreeItem.Name = "Компьютер";

			var rootDriverId = FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.Computer).UID;
			AddDriver(rootDriverId, RootTreeItem);
			RootTreeItem.Children.Add(MonitorHelper.CreateMonitor());
			RootTreeItem.Children.Add(ZoneHelper.CreateZone());
			InitializeModelInfo(RootTreeItem);
			SaveToFile();
		}

		void AddDriver(Guid parentDriverUID, TreeItem parentTreeItem)
		{
			var parentDriver = FiresecManager.Drivers.FirstOrDefault(x => x.UID == parentDriverUID);

			if (parentDriver != null)
			{
				if (parentDriver.DriverType == DriverType.MPT)
					return;

				foreach (var driver in parentDriver.Children)
				{
					var childDriver = FiresecManager.Drivers.FirstOrDefault(x => x.UID == driver);
					var childTree = new TreeItem();
					childTree.SetDriver(childDriver);
					parentTreeItem.Children.Add(childTree);

					AddDriver(driver, childTree);
				}
			}
		}

		void InitializeModelInfo(TreeItem parentTreeItem)
		{
			if (parentTreeItem.Children.Count > 0)
			{
				parentTreeItem.ModelInfo.type = new Assad.modelInfoType[parentTreeItem.Children.Count];
				for (int i = 0; i < parentTreeItem.Children.Count; i++)
				{
					parentTreeItem.ModelInfo.type[i] = parentTreeItem.Children[i].ModelInfo;
					InitializeModelInfo(parentTreeItem.Children[i]);
				}
			}
		}

		void SaveToFile()
		{
			var serializer = new XmlSerializer(typeof(Assad.modelInfoType));
			StreamWriter fileStream = File.CreateText("DeviceModel.xml");
			serializer.Serialize(fileStream, RootTreeItem.ModelInfo);
			fileStream.Close();
		}
	}
}