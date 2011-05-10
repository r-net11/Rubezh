using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using Firesec.Metadata;
using System.Collections.ObjectModel;

namespace DeviceLibrary
{
    public static class LibraryManager
    {
        public static string EmptyFrame = "<?xml version=\"1.0\" encoding=\"utf-16\"?><Canvas Width=\"500\" Height=\"500\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"/>";
        public static string ErrorFrame = "<?xml version=\"1.0\" encoding=\"utf-16\"?><Canvas Width=\"500\" Height=\"500\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"><TextBlock Text=\"Error Xaml Code\" FontSize=\"100\" /> </Canvas>";
        public static List<Device> Devices { get; set; }
        /// <summary>
        /// Список всех устройств, полученный из файла metadata.xml
        /// </summary>
        public static drvType[] Drivers { get; set; }
        public static ObservableCollection<string> BaseStatesList { get; set; }
        static LibraryManager()
        {
            Load();
            LoadMetadata();
            LoadBaseStates();
        }

        static void LoadBaseStates()
        {
            BaseStatesList = new
            ObservableCollection<string>
            {
                "Тревога",
                "Внимание (предтревожное)",
                "Неисправность",
                "Требуется обслуживание",
                "Обход устройств",
                "Неопределено",
                "Норма(*)",
                "Норма",
                "Базовый рисунок"
            };
        }

        static void LoadMetadata()
        {
            var fileXml = new FileStream(ResourceHelper.MetadataXml, FileMode.Open, FileAccess.Read, FileShare.Read);
            var serializer = new XmlSerializer(typeof(config));
            var metadata = (config)serializer.Deserialize(fileXml);
            fileXml.Close();
            Drivers = metadata.drv;
        }

        static void Load()
        {
            FileStream fileStream = new FileStream(ResourceHelper.DeviceLibraryXml, FileMode.Open, FileAccess.Read, FileShare.Read);
            XmlSerializer serializer = new XmlSerializer(typeof(DeviceManager));
            DeviceManager deviceManager = (DeviceManager)serializer.Deserialize(fileStream);
            fileStream.Close();

            Devices = deviceManager.Devices;
        }

        public static void Save()
        {
            DeviceManager deviceManager = new DeviceManager();
            deviceManager.Devices = Devices;

            FileStream fileStream = new FileStream(ResourceHelper.DeviceLibraryXml, FileMode.Create, FileAccess.Write, FileShare.Write);
            XmlSerializer serializer = new XmlSerializer(typeof(DeviceManager));
            serializer.Serialize(fileStream, deviceManager);
            fileStream.Close();
        }
    }
}
