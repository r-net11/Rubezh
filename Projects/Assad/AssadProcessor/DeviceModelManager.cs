using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace AssadProcessor
{
	public class DeviceModelManager
	{
		public List<Assad.modelInfoType> ModelInfos { get; private set; }
		public Assad.modelInfoType RootModelInfo { get; private set; }

		public DeviceModelManager()
		{
			var serializer = new XmlSerializer(typeof(Assad.modelInfoType));
			var fileStream = new FileStream("DeviceModel.xml", FileMode.Open);
			RootModelInfo = (Assad.modelInfoType)serializer.Deserialize(fileStream);
			fileStream.Close();

			ModelInfos = new List<Assad.modelInfoType>();
			ModelInfos.Add(RootModelInfo);
			AddChild(RootModelInfo);
		}

		void AddChild(Assad.modelInfoType parent)
		{
			if (parent.type != null)
				foreach (var child in parent.type)
				{
					ModelInfos.Add(child);
					AddChild(child);
				}
		}
	}
}