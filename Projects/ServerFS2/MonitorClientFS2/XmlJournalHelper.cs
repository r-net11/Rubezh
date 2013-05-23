using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using FiresecAPI.Models;
using Infrastructure.Common;
using ServerFS2;

namespace MonitorClientFS2
{
	public static class XmlJournalHelper
	{
		static XDocument lastRecordsDocument;
		static readonly string fileName = Path.Combine(AppDataFolderHelper.GetFolder("Server2"), "LastRecords.xml");

		static XmlJournalHelper()
		{
			if (File.Exists(fileName))
				lastRecordsDocument = XDocument.Load(fileName);
			else
			{
				lastRecordsDocument = new XDocument();
				lastRecordsDocument.Add(new XElement("Root"));
				lastRecordsDocument.Save(fileName);
			}

			foreach (var device in ConfigurationManager.DeviceConfiguration.Devices.Where(x => x.Driver.IsPanel))
			{
				XElement deviceElement = lastRecordsDocument.Root.Elements("Device").FirstOrDefault(x => x.Attribute("UID").Value == device.UID.ToString());
				if (deviceElement == null)
				{
					deviceElement = new XElement("Device");
					deviceElement.Add(new XAttribute("UID", device.UID.ToString()));
					deviceElement.Add(new XAttribute("LastId", "-1"));
					if (device.Driver.DriverType == FiresecAPI.Models.DriverType.Rubezh_2OP)
						deviceElement.Add(new XAttribute("LastSecId", "-1"));
					lastRecordsDocument.Root.Add(deviceElement);
				}
			}
			lastRecordsDocument.Save(fileName);
		}

		public static void SetLastId(OldMonitoringDevice monitoringDevice)
		{
			XElement deviceElement = lastRecordsDocument.Root.Elements("Device").FirstOrDefault(x => x.Attribute("UID").Value == monitoringDevice.Device.UID.ToString());
			deviceElement.Attribute("LastId").Value = monitoringDevice.LastDisplayedRecord.ToString();
			lastRecordsDocument.Save(fileName);
		}

		public static void SetLastSecId(OldMonitoringDevice monitoringDevice)
		{
			XElement deviceElement = lastRecordsDocument.Root.Elements("Device").FirstOrDefault(x => x.Attribute("UID").Value == monitoringDevice.Device.UID.ToString());
			deviceElement.Attribute("LastSecId").Value = monitoringDevice.LastDisplayedSecRecord.ToString();
			lastRecordsDocument.Save(fileName);
		}

		public static int GetLastSecId(OldMonitoringDevice monitoringDevice)
		{
			XElement deviceElement = lastRecordsDocument.Root.Elements("Device").FirstOrDefault(x => x.Attribute("UID").Value == monitoringDevice.Device.UID.ToString());
			return Int32.Parse(deviceElement.Attribute("LastSecId").Value);
		}

		public static int GetLastId(OldMonitoringDevice monitoringDevice)
		{
			XElement deviceElement = lastRecordsDocument.Root.Elements("Device").FirstOrDefault(x => x.Attribute("UID").Value == monitoringDevice.Device.UID.ToString());
			return Int32.Parse(deviceElement.Attribute("LastId").Value);
		}

		public static void SetLastId(Device device, int lastId)
		{
			XElement deviceElement = lastRecordsDocument.Root.Elements("Device").FirstOrDefault(x => x.Attribute("UID").Value == device.UID.ToString());
			deviceElement.Attribute("LastId").Value = lastId.ToString();
			lastRecordsDocument.Save(fileName);
		}

		public static int GetLastId(Device device)
		{
			XElement deviceElement = lastRecordsDocument.Root.Elements("Device").FirstOrDefault(x => x.Attribute("UID").Value == device.UID.ToString());
			return Int32.Parse(deviceElement.Attribute("LastId").Value);
		}

		public static void SetLastSecId(Device device, int lastId)
		{
			XElement deviceElement = lastRecordsDocument.Root.Elements("Device").FirstOrDefault(x => x.Attribute("UID").Value == device.UID.ToString());
			deviceElement.Attribute("LastSecId").Value = lastId.ToString();
			lastRecordsDocument.Save(fileName);
		}

		public static int GetLastSecId(Device device)
		{
			XElement deviceElement = lastRecordsDocument.Root.Elements("Device").FirstOrDefault(x => x.Attribute("UID").Value == device.UID.ToString());
			return Int32.Parse(deviceElement.Attribute("LastSecId").Value);
		}
	}
}