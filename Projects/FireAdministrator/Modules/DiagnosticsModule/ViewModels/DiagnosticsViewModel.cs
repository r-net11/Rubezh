using System;
using System.IO;
using System.Runtime.Serialization;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Video;
using Infrastructure.Common.Windows.ViewModels;

namespace DiagnosticsModule.ViewModels
{
	public class DiagnosticsViewModel : ViewPartViewModel
	{
		private bool IsNowPlaying { get; set; }
		CameraFramesWatcher CameraFramesWatcher { get; set; }
		public DiagnosticsViewModel()
		{
			var camera = new Camera();
			camera.Login = "admin";
			camera.Password = "admin";
			camera.Ip = "172.16.7.88";
			CameraFramesWatcher = new CameraFramesWatcher(camera);
			TestXMLCommand = new RelayCommand(OnTestXML);
			StartCommand = new RelayCommand(OnStart, () => !IsNowPlaying);
			StopCommand = new RelayCommand(OnStop, () => IsNowPlaying);
			SaveCommand = new RelayCommand(OnSave);
		}

		public RelayCommand TestXMLCommand { get; private set; }
		void OnTestXML()
		{
			//XmlDocument query_xml = new XmlDocument();
			//query_xml.Load("D://Config/XDeviceConfiguration.xml");
			//query_xml = RemoveXmlns(query_xml);
			//query_xml.Save("D://Config/XDeviceConfiguration_1.xml");

			//var configuration = DeSerialize<FiresecAPI.GK.XDeviceConfiguration>("D://Config/XDeviceConfiguration_3.xml");
		}

		//public static XmlDocument RemoveXmlns(XmlDocument doc)
		//{
			//XDocument d;
			//using (var nodeReader = new XmlNodeReader(doc))
			//	d = XDocument.Load(nodeReader);

			//d.Root.Attributes().Where(x => x.IsNamespaceDeclaration).Remove();
			//d.Root.Descendants().Attributes().Where(x => x.IsNamespaceDeclaration).Remove();

			//foreach (var elem in d.Descendants())
			//	elem.Name = elem.Name.LocalName;

			//d.Save("D://Config/XDeviceConfiguration_2.xml");

			//var xmlDocument = new XmlDocument();
			//using (var xmlReader = d.CreateReader())
			//	xmlDocument.Load(xmlReader);

			//return xmlDocument;
		//}

		public static T DeSerialize<T>(string fileName)
			 where T : VersionedConfiguration, new()
		{
			using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
			{
				T configuration = null;
				var dataContractSerializer = new DataContractSerializer(typeof(T));
				configuration = (T)dataContractSerializer.ReadObject(fileStream);
				fileStream.Close();
				configuration.ValidateVersion();
				configuration.AfterLoad();
				return configuration;
			}
		}

		public static T DeSerialize<T>(MemoryStream memoryStream)
			 where T : VersionedConfiguration, new()
		{
			T configuration = null;
			var dataContractSerializer = new DataContractSerializer(typeof(T));
			configuration = (T)dataContractSerializer.ReadObject(memoryStream);
			configuration.ValidateVersion();
			configuration.AfterLoad();
			return configuration;
		}

		public RelayCommand StartCommand { get; private set; }
		void OnStart()
		{
			Logger.Error("Test Error");
			IsNowPlaying = true;
			CameraFramesWatcher.StartVideo();
		}

		public RelayCommand StopCommand { get; private set; }
		void OnStop()
		{
			CameraFramesWatcher.StopVideo();
			IsNowPlaying = false;
		}

		public RelayCommand SaveCommand { get; private set; }
		void OnSave()
		{
			var guid = Guid.NewGuid();
			CameraFramesWatcher.Save(guid);
		}
		
		public void StopThreads()
		{
			IsThreadStoping = true;
		}
		bool IsThreadStoping = false;

		string _text;
		public string Text
		{
			get { return _text; }
			set
			{
				_text = value;
				OnPropertyChanged("Text");
			}
		}
	}
}