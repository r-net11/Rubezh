using System;
using System.IO;
using System.Xml.Serialization;

namespace Logger
{
    public class LogEngine : IDisposable
    {
        LogCollection Logs;
        DateTime startTime;
        DateTime endTime;

        public LogEngine()
        {
            Logs = new LogCollection();
            startTime = DateTime.Now;
        }

        public void AddEntry(LogEntry logEntry)
        {
            Logs.AddEntry(logEntry);
        }

        void SaveFile(string fileName)
        {
            Directory.CreateDirectory("Logs");
            StreamWriter writer = new StreamWriter("Logs/" + fileName);
            XmlSerializer serializer = new XmlSerializer(typeof(LogCollection));
            serializer.Serialize(writer, Logs);
            writer.Close();
        }

        void LoadFile()
        {
            StreamReader reader = new StreamReader("log.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(LogCollection));
            Logs = (LogCollection) serializer.Deserialize(reader);
            reader.Close();
        }

        public void Save()
        {
            endTime = DateTime.Now;
            string fileName = startTime.ToString() + " - " + endTime.ToString() + ".xml";
            fileName = fileName.Replace(":", ".");
            SaveFile(fileName);
        }

        public void Dispose()
        {
            Save();
        }
    }
}