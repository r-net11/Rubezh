using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Firesec.Helper
{
    public class TreeBuilder
    {
        public DriverItem rootDriver { get; set; }
        public List<DriverItem> Drivers { get; set; }

        public void Buid()
        {
            Drivers = new List<DriverItem>();

            Firesec.Metadata.classType rootClass = MetadataHelper.Metadata.@class.First(x => x.parent == null);

            ClassItem rootClassItem = new ClassItem();
            rootClassItem.Clsid = rootClass.clsid;
            rootClassItem.Drivers.Add(MetadataHelper.Metadata.drv.FirstOrDefault(x => x.clsid == rootClass.clsid).id);

            AddClass(rootClassItem);

            DriverItem rootDriverItem = new DriverItem();
            AddDriver(rootClassItem, rootDriverItem);
            Drivers.Add(rootDriverItem);

            rootDriver = rootDriverItem.Children[0];
        }

        void AddClass(ClassItem parentClassItem)
        {
            foreach (Firesec.Metadata.classType childClass in MetadataHelper.Metadata.@class)
            {
                if (childClass.parent != null)
                {
                    if (childClass.parent.Any(x => x.clsid == parentClassItem.Clsid))
                    {
                        ClassItem childClassItem = new ClassItem();
                        childClassItem.Clsid = childClass.clsid;

                        if (MetadataHelper.Metadata.drv.Any(x => x.clsid == childClassItem.Clsid))
                        {
                            List<Firesec.Metadata.drvType> drivers = MetadataHelper.Metadata.drv.ToList().FindAll(x => x.clsid == childClassItem.Clsid);
                            foreach (Firesec.Metadata.drvType driver in drivers)
                            {
                                string driverId = driver.id;
                                if (!childClassItem.Drivers.Contains(driverId))
                                    childClassItem.Drivers.Add(driverId);
                            }
                        }

                        if (childClassItem.Clsid != parentClassItem.Clsid)
                        {
                            parentClassItem.Childern.Add(childClassItem);
                            AddClass(childClassItem);
                        }
                    }
                }
            }
        }

        void AddDriver(ClassItem parentClassItem, DriverItem parentDriverItem)
        {
            foreach (string driverId in parentClassItem.Drivers)
            {
                DriverItem childDriverItem = new DriverItem();
                childDriverItem.Parent = parentDriverItem;
                childDriverItem.DriverId = driverId;

                // хак - добавлять каналы только к возможным каналам

                bool canAdd = true;
                if ((childDriverItem.DriverId == "780DE2E6-8EDD-4CFA-8320-E832EB699544") && (parentDriverItem.DriverId != "FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6"))
                {
                    canAdd = false;
                }
                if ((childDriverItem.DriverId == "F36B2416-CAF3-4A9D-A7F1-F06EB7AAA76E") && (parentDriverItem.DriverId != "CD0E9AA0-FD60-48B8-B8D7-F496448FADE6"))
                {
                    canAdd = false;
                }
                if ((childDriverItem.DriverId == "B9680002-511D-4505-9EF6-0C322E61135F") && (parentDriverItem.DriverId != "2863E7A3-5122-47F8-BB44-4358450CD0EE"))
                {
                    canAdd = false;
                }
                if ((childDriverItem.Parent.DriverId == "780DE2E6-8EDD-4CFA-8320-E832EB699544") || (childDriverItem.Parent.DriverId == "F36B2416-CAF3-4A9D-A7F1-F06EB7AAA76E"))
                {
                    if (parentDriverItem.Parent.Parent.DriverId == "C2E0F845-D836-4AAE-9894-D5CBE2B9A7DD")
                        continue;
                }
                // хак - модуль плжаротушения может быть только у Рубеж-10
                if ((childDriverItem.DriverId == "FD200EDF-94A4-4560-81AA-78C449648D45") && (parentDriverItem.DriverId != "E750EF8F-54C3-4B00-8C72-C7BEC9E59BFC"))
                {
                    canAdd = false;
                }
                if (DriversHelper.IsIgnore(driverId))
                {
                    canAdd = false;
                }

                if (canAdd)
                {
                    parentDriverItem.Children.Add(childDriverItem);
                }

                foreach (ClassItem childClassItem in parentClassItem.Childern)
                {
                    AddDriver(childClassItem, childDriverItem);
                    Drivers.Add(childDriverItem);
                }
            }
        }
    }

    class ClassItem
    {
        public ClassItem()
        {
            Childern = new List<ClassItem>();
            Drivers = new List<string>();
        }

        public string Clsid { get; set; }
        public List<ClassItem> Childern { get; set; }
        public List<string> Drivers { get; set; }
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
