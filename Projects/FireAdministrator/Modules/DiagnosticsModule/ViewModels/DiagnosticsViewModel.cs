using System;
using System.Linq;
using Common;
using DiagnosticsModule.Views;
using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Video;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient;
using System.Xml;
using System.Xml.Linq;
using FiresecAPI;
using System.IO;
using System.Runtime.Serialization;
using XFiresecAPI;
using System.Collections.Generic;

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
			camera.Address = "172.16.7.88";
			CameraFramesWatcher = new CameraFramesWatcher(camera);
			TestXMLCommand = new RelayCommand(OnTestXML);
			StartCommand = new RelayCommand(OnStart, () => !IsNowPlaying);
			StopCommand = new RelayCommand(OnStop, () => IsNowPlaying);
			SaveCommand = new RelayCommand(OnSave);
		}

		public RelayCommand TestXMLCommand { get; private set; }
		void OnTestXML()
		{
			XmlDocument query_xml = new XmlDocument();
			query_xml.Load("D://Config/XDeviceConfiguration.xml");
			query_xml = RemoveXmlns(query_xml);
			query_xml.Save("D://Config/XDeviceConfiguration_1.xml");

			var configuration = DeSerialize<FiresecAPI.GK.XDeviceConfiguration>("D://Config/XDeviceConfiguration_3.xml");
		}

		public static XmlDocument RemoveXmlns(XmlDocument doc)
		{
			XDocument d;
			using (var nodeReader = new XmlNodeReader(doc))
				d = XDocument.Load(nodeReader);

			d.Root.Attributes().Where(x => x.IsNamespaceDeclaration).Remove();
			d.Root.Descendants().Attributes().Where(x => x.IsNamespaceDeclaration).Remove();

			foreach (var elem in d.Descendants())
				elem.Name = elem.Name.LocalName;

			d.Save("D://Config/XDeviceConfiguration_2.xml");

			var xmlDocument = new XmlDocument();
			using (var xmlReader = d.CreateReader())
				xmlDocument.Load(xmlReader);

			return xmlDocument;
		}

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

namespace FiresecAPI.GK
{
	[DataContract(Namespace="")]
	public class XDeviceConfiguration : VersionedConfiguration
	{
		public XDeviceConfiguration()
		{
			Devices = new List<XDevice>();
			Zones = new List<XZone>();
			Directions = new List<XDirection>();
			PumpStations = new List<XPumpStation>();
			JournalFilters = new List<XJournalFilter>();
			Instructions = new List<XInstruction>();
			GuardUsers = new List<XGuardUser>();
			ParameterTemplates = new List<XParameterTemplate>();
		}

		public List<XDevice> Devices { get; set; }

		[DataMember]
		public XDevice RootDevice { get; set; }

		[DataMember]
		public List<XZone> Zones { get; set; }

		[DataMember]
		public List<XDirection> Directions { get; set; }

		[DataMember]
		public List<XPumpStation> PumpStations { get; set; }

		[DataMember]
		public List<XJournalFilter> JournalFilters { get; set; }

		[DataMember]
		public List<XInstruction> Instructions { get; set; }

		[DataMember]
		public List<XGuardUser> GuardUsers { get; set; }

		[DataMember]
		public List<XParameterTemplate> ParameterTemplates { get; set; }

		public void Update()
		{
			Devices = new List<XDevice>();
			if (RootDevice != null)
			{
				RootDevice.Parent = null;
				Devices.Add(RootDevice);
				AddChild(RootDevice);
			}
		}

		void AddChild(XDevice parentDevice)
		{
			foreach (var device in parentDevice.Children)
			{
				device.Parent = parentDevice;
				Devices.Add(device);
				AddChild(device);
			}
		}

		public List<XZone> SortedZones
		{
			get
			{
				return (
				from XZone zone in Zones
				orderby zone.No
				select zone).ToList();
			}
		}

		public void Reorder()
		{
			foreach (var device in Devices)
			{
				device.SynchronizeChildern();
			}
		}

		public override bool ValidateVersion()
		{
			bool result = true;

			if (RootDevice == null)
			{
				var device = new XDevice();
				device.DriverUID = new Guid("938947C5-4624-4A1A-939C-60AEEBF7B65C");
				RootDevice = device;
				result = false;
			}

			Update();

			if (PumpStations == null)
			{
				PumpStations = new List<XPumpStation>();
				result = false;
			}

			foreach (var journalFilter in JournalFilters)
			{
			}

			foreach (var device in Devices)
			{
				device.BaseUID = device.UID;
				if (device.DeviceLogic.OffClauses == null)
				{
					result = false;
					device.DeviceLogic.OffClauses = new List<XClause>();
				}
			}
			foreach (var zone in Zones)
			{
				zone.BaseUID = zone.UID;
			}

			foreach (var parameterTemplate in ParameterTemplates)
			{
				foreach (var deviceParameterTemplate in parameterTemplate.DeviceParameterTemplates)
				{
				}
			}
			foreach (var direction in Directions)
			{
				direction.BaseUID = direction.UID;
			}

			foreach (var journalFilter in JournalFilters)
			{
			}

			return result;
		}
	}
}