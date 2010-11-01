using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting;
using System.IO;
using System.Xml.Serialization;
using System.Windows;

namespace ComServer
{
    public class ComServer
    {
        public static CoreConfig.config GetCoreConfig()
        {
            string metadataString = NativeComServer.GetCoreConfig();
            byte[] bytes = Encoding.Default.GetBytes(metadataString);
            MemoryStream memoryStream = new MemoryStream(bytes);

            XmlSerializer serializer = new XmlSerializer(typeof(CoreConfig.config));
            CoreConfig.config coreConfig = (CoreConfig.config)serializer.Deserialize(memoryStream);
            memoryStream.Close();
            return coreConfig;
        }

        public static CoreState.config GetCoreState()
        {
            string metadataString = NativeComServer.GetCoreState();
            byte[] bytes = Encoding.Default.GetBytes(metadataString);
            MemoryStream memoryStream = new MemoryStream(bytes);

            XmlSerializer serializer = new XmlSerializer(typeof(CoreState.config));
            CoreState.config coreConfig = (CoreState.config)serializer.Deserialize(memoryStream);
            memoryStream.Close();
            return coreConfig;
        }

        public static Metadata.config GetGetMetaData()
        {
            string metadataString = NativeComServer.GetMetaData();
            byte[] bytes = Encoding.Default.GetBytes(metadataString);
            MemoryStream memoryStream = new MemoryStream(bytes);

            XmlSerializer serializer = new XmlSerializer(typeof(Metadata.config));
            Metadata.config coreConfig = (Metadata.config)serializer.Deserialize(memoryStream);
            memoryStream.Close();
            return coreConfig;
        }
    }

    public class NotificationCallBack : FS_Types.IFS_CallBack
    {
        public void NewEventsAvailable(int EventMask)
        {
            MessageBox.Show("CallBack");
        }
    }

    public class NativeComServer
    {
        static FS_Types.IFSC_Connection connectoin;

        public static string GetCoreConfig()
        {
            if (connectoin == null)
                connectoin = GetConnection();
            mscoree.IStream stream = connectoin.GetCoreConfig();

            string message = ReadFromStream(stream);
            return message;
        }

        public static string GetCoreState()
        {
            if (connectoin == null)
                connectoin = GetConnection();
            mscoree.IStream stream = connectoin.GetCoreState();

            string message = ReadFromStream(stream);
            return message;
        }

        public static string GetMetaData()
        {
            if (connectoin == null)
                connectoin = GetConnection();
            mscoree.IStream stream = connectoin.GetMetaData();

            string message = ReadFromStream(stream);
            return message;
        }

        public static void StartListening()
        {
            ObjectHandle objectHandle = Activator.CreateComInstanceFrom("Interop.FS_Types.dll", "FS_Types.FSC_LIBRARY_CLASSClass");
            FS_Types.FSC_LIBRARY_CLASSClass library = (FS_Types.FSC_LIBRARY_CLASSClass)objectHandle.Unwrap();
            FS_Types.TFSC_ServerInfo serverInfo = new FS_Types.TFSC_ServerInfo();
            serverInfo.Port = 211;
            serverInfo.ServerName = "localhost";

            NotificationCallBack notificationCallBack = new NotificationCallBack();

            FS_Types.IFSC_Connection connectoin = library.Connect2("adm", "", serverInfo, notificationCallBack);
        }

        static FS_Types.IFSC_Connection GetConnection()
        {
            ObjectHandle objectHandle = Activator.CreateComInstanceFrom("Interop.FS_Types.dll", "FS_Types.FSC_LIBRARY_CLASSClass");
            FS_Types.FSC_LIBRARY_CLASSClass library = (FS_Types.FSC_LIBRARY_CLASSClass)objectHandle.Unwrap();
            FS_Types.TFSC_ServerInfo serverInfo = new FS_Types.TFSC_ServerInfo();
            serverInfo.Port = 211;
            serverInfo.ServerName = "localhost";

            //NotificationCallBack notificationCallBack = new NotificationCallBack();

            //FS_Types.IFSC_Connection connectoin = library.Connect2("adm", "", serverInfo, notificationCallBack);
            FS_Types.IFSC_Connection connectoin = library.Connect("adm", "", serverInfo);

            return connectoin;
        }

        static string ReadFromStream(mscoree.IStream stream)
        {
            string message = "";

            unsafe
            {
                while (true)
                {
                    byte* unsafeBytes = stackalloc byte[1024];
                    IntPtr _intPtr = new IntPtr(unsafeBytes);
                    uint bytesRead = 0;
                    stream.Read(_intPtr, 1024, out bytesRead);
                    if (bytesRead == 0)
                        break;

                    byte[] bytes = new byte[bytesRead];
                    for (int i = 0; i < bytesRead; i++)
                    {
                        bytes[i] = unsafeBytes[i];
                    }
                    message += Encoding.Default.GetString(bytes);
                }
            }
            return message;
        }
    }
}
