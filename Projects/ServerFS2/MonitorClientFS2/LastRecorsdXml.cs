using System.Linq;
using System.Xml.Linq;

namespace MonitorClientFS2
{
	public static class LastRecorsdXml
	{
		static XDocument lastRecordsDocument;
		static string fileName = "C:\\LastRecords.xml";

		static LastRecorsdXml()
		{
			lastRecordsDocument = XDocument.Load(fileName);
		}

		public static void SetLastId(MonitoringDevice monitoringDevice)
		{
			XElement deviceElement = lastRecordsDocument.Root.Elements("Device").FirstOrDefault(x => x.Attribute("GUID").Value == monitoringDevice.Device.UID.ToString());
			if (deviceElement != null)
			{
				deviceElement.Attribute("LastId").Value = monitoringDevice.LastDisplayedRecord.ToString();
			}
			else
			{
				deviceElement = new XElement("Device");
				deviceElement.Add(new XAttribute("LastId", monitoringDevice.LastDisplayedRecord.ToString()));
				lastRecordsDocument.Save(fileName);
			}
		}
	}
}