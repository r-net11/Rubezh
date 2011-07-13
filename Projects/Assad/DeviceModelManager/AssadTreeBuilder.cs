using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using FiresecClient;

namespace DeviveModelManager
{
    public class AssadTreeBuilder
    {
        public TreeItem RootTreeItem { get; private set; }

        public void Build()
        {
            TreeBuilder treeBuilder = new TreeBuilder();
            treeBuilder.Buid();

            CreateRootTreeItem();
            AddAssad(treeBuilder.RootDriver, RootTreeItem);
            RootTreeItem.Children.Add(MonitorHelper.CreateMonitor());
            RootTreeItem.Children.Add(ZoneHelper.CreateZone());
            AddAssadChild(RootTreeItem);
            SaveToFile();
        }

        void CreateRootTreeItem()
        {
            RootTreeItem = RootHelper.CreateRoot();
            var rootClass = FiresecManager.Configuration.Metadata.@class.First(x => x.parent == null);
            RootTreeItem.Name = FiresecManager.Configuration.Metadata.drv.First(x => x.clsid == rootClass.clsid).name;
        }

        void AddAssad(DriverItem parentDriverItem, TreeItem parentTreeItem)
        {
            foreach (var childDriverItem in parentDriverItem.Children)
            {
                var driver = FiresecManager.Configuration.Drivers.FirstOrDefault(x => x.Id == childDriverItem.DriverId);
                TreeItem childTree = new TreeItem();
                childTree.Parent = parentTreeItem;
                childTree.SetDriver(driver);
                parentTreeItem.Children.Add(childTree);

                AddAssad(childDriverItem, childTree);
            }
        }

        void AddAssadChild(TreeItem parentTreeItem)
        {
            if (parentTreeItem.Children.Count > 0)
            {
                parentTreeItem.ModelInfo.type = new Assad.modelInfoType[parentTreeItem.Children.Count];
                for (int i = 0; i < parentTreeItem.Children.Count; i++)
                {
                    parentTreeItem.ModelInfo.type[i] = parentTreeItem.Children[i].ModelInfo;
                    AddAssadChild(parentTreeItem.Children[i]);
                }
            }
        }

        void SaveToFile()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Assad.modelInfoType));
            StreamWriter fileStream = File.CreateText("DeviceModel.xml");
            serializer.Serialize(fileStream, RootTreeItem.ModelInfo);
            fileStream.Close();
        }
    }
}
