using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace AssadProcessor
{
    // в этом классе хранится модель устройств ассада
    // рутовый элемент доступен через свойство RootModelInfo
    // все модели для каждого устройства доступны через свойство ModelInfos
    // класс доступен для всех через клас AssadServices

    public class DeviceModelManager
    {
        public List<Assad.modelInfoType> ModelInfos { get; private set; }
        public Assad.modelInfoType RootModelInfo { get; private set; }

        public DeviceModelManager()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Assad.modelInfoType));
            FileStream fileStream = new FileStream("DeviceModel.xml", FileMode.Open);
            RootModelInfo = (Assad.modelInfoType)serializer.Deserialize(fileStream);
            fileStream.Close();

            ModelInfos = new List<Assad.modelInfoType>();
            ModelInfos.Add(RootModelInfo);
            AddChild(RootModelInfo);
        }

        void AddChild(Assad.modelInfoType parent)
        {
            if (parent.type != null)
                foreach (Assad.modelInfoType child in parent.type)
                {
                    ModelInfos.Add(child);
                    AddChild(child);
                }
        }
    }
}
