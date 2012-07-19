using System;
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
			FiresecManager.Connect(ClientType.Assad, "net.tcp://localhost:8000/FiresecService/", "adm", "");
			FiresecManager.GetConfiguration();

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