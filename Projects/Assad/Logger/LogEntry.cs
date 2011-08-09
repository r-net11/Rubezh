using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Logger
{
    public class LogEntry
    {
        public LogEntry()
        {
        }

        public LogEntry(LogType logType, string Message)
        {
            this.logType = logType;
            this.Message = String.Copy(Message);

            Id = IdManager.Next;
            Time = DateTime.Now;
        }

        public int Id { get; set; }
        public DateTime Time { get; set; }
        public LogType logType { get; set; }
        public string Message { get; set; }

        public string Description
        {
            get
            {
                byte[] bytes = Encoding.UTF8.GetBytes(Message);
                MemoryStream memoryStream = new MemoryStream(bytes);
                XmlSerializer serializer = new XmlSerializer(typeof(Assad.MessageType));
                Assad.MessageType messageType = (Assad.MessageType) serializer.Deserialize(memoryStream);
                memoryStream.Close();

                string type = messageType.Item.GetType().ToString();
                type = type.Replace("Assad.", "");
                type = type.Replace("Type", "");
                string description = "" + type;
                return description;
            }
        }
    }
}