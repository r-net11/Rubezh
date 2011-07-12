using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecClient;
using Firesec;
using FiresecClient.Models;

namespace DeviveModelManager
{
    public class TreeBuilder
    {
        public DriverItem RootDriver { get; set; }

        public void Buid()
        {
            FiresecManager.Connect("adm", "");
            var rootClass = FiresecManager.Configuration.Metadata.@class.First(x => x.parent == null);

            RootDriver = new DriverItem();
            var rootDriver = FiresecManager.Configuration.Drivers.FirstOrDefault(x=>x.DriverName == "Компьютер");
            RootDriver.Parent = null;
            RootDriver.DriverId = rootDriver.Id;
            AddDriver(rootDriver.Id, RootDriver);
        }

        void AddDriver(string parentDriverId, DriverItem parentDriverItem)
        {
            Driver parentDriver = FiresecManager.Configuration.Drivers.FirstOrDefault(x => x.Id == parentDriverId);

            if (parentDriver != null)
            {
                if (parentDriver.DriverName == "Модуль пожаротушения")
                    return;

                foreach (var driver in parentDriver.Children)
                {
                    DriverItem driverItem = new DriverItem();
                    driverItem.DriverId = driver;
                    driverItem.Parent = parentDriverItem;
                    parentDriverItem.Children.Add(driverItem);
                    AddDriver(driver, driverItem);
                }
            }
        }
    }

    public class DriverItem
    {
        public DriverItem()
        {
            Children = new List<DriverItem>();
        }
        public string DriverId { get; set; }
        public List<DriverItem> Children { get; set; }
        public DriverItem Parent { get; set; }
    }
}
