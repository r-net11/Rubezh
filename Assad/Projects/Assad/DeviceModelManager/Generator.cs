using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Windows;

namespace DeviveModelManager
{
    /*
     Генерирование модели устройств для Ассада
     * на основе метаданных, полученных из метаданных из ком - сервера
     */

    public class Generator
    {
        public TreeItem RootTreeItem { get; private set; }

        public void CreateTree()
        {
            CreateRootTreeItem();
            AddTreeItem(RootTreeItem);
            RootTreeItem.Children.Add(ZoneHelper.CreateZone());
            AddAssadChild(RootTreeItem);
            SaveToFile();
        }

        void CreateRootTreeItem()
        {
            RootTreeItem = RootHelper.CreateRoot();
            Firesec.Metadata.classType rootClass = ComServerHelper.Metadata.@class.First(x => x.parent == null);
            RootTreeItem.Clsid = rootClass.clsid;
            RootTreeItem.Name = ComServerHelper.Metadata.drv.First(x => x.clsid == rootClass.clsid).name;
        }

        void AddTreeItem(TreeItem parentTreeItem)
        {
            List<Firesec.Metadata.classType> childClasses = new List<Firesec.Metadata.classType>();

            foreach (Firesec.Metadata.classType child in ComServerHelper.Metadata.@class)
            {
                bool IsChild = true;
                try
                {
                    child.parent.First(x => x.clsid == parentTreeItem.Clsid);
                }
                catch
                {
                    IsChild = false;
                }
                if (IsChild)
                {
                    childClasses.Add(child);
                }
            }

            foreach (Firesec.Metadata.classType child in childClasses)
            {
                foreach (Firesec.Metadata.drvType driver in ComServerHelper.Metadata.drv)
                {
                    // игнорировать все старые приборы
                    if (Common.DriversHelper.IsIgnore(driver.id))
                    {
                        continue;
                    }

                    if (driver.clsid == child.clsid)
                    {
                        if (parentTreeItem.Clsid != child.clsid)
                        {
                            TreeItem childTree = new TreeItem();
                            childTree.Clsid = child.clsid;
                            childTree.Name = driver.name;
                            childTree.ParentName = parentTreeItem.Name;
                            childTree.Parent = parentTreeItem;
                            childTree.SetDriver(driver);

                            // хак - добавлять каналы только к возможным каналам
                            if ((childTree.DriverId == "780DE2E6-8EDD-4CFA-8320-E832EB699544") && (parentTreeItem.DriverId != "FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6"))
                            {
                                continue;
                            }
                            if ((childTree.DriverId == "F36B2416-CAF3-4A9D-A7F1-F06EB7AAA76E") && (parentTreeItem.DriverId != "CD0E9AA0-FD60-48B8-B8D7-F496448FADE6"))
                            {
                                continue;
                            }
                            if ((childTree.DriverId == "B9680002-511D-4505-9EF6-0C322E61135F") && (parentTreeItem.DriverId != "2863E7A3-5122-47F8-BB44-4358450CD0EE"))
                            {
                                continue;
                            }

                            if ((childTree.Parent.DriverId == "780DE2E6-8EDD-4CFA-8320-E832EB699544") || (childTree.Parent.DriverId == "F36B2416-CAF3-4A9D-A7F1-F06EB7AAA76E"))
                            {
                                if (parentTreeItem.Parent.Parent.DriverId == "C2E0F845-D836-4AAE-9894-D5CBE2B9A7DD")
                                    continue;
                            }

                            // хак - модуль плжаротушения может быть только у Рубеж-10
                            if ((childTree.DriverId == "FD200EDF-94A4-4560-81AA-78C449648D45") && (parentTreeItem.DriverId != "E750EF8F-54C3-4B00-8C72-C7BEC9E59BFC"))
                            {
                                continue;
                            }

                            parentTreeItem.Children.Add(childTree);
                            AddTreeItem(childTree);
                        }
                    }
                }
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

        public void SaveToFile()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Assad.modelInfoType));
            StreamWriter fileStream = File.CreateText("DeviceModel.xml");
            serializer.Serialize(fileStream, RootTreeItem.ModelInfo);
            fileStream.Close();
        }
    }
}
