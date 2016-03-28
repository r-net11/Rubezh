using RubezhAPI.Automation;
using RubezhAPI.Journal;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace RubezhAPI.Models
{
	[DataContract]
	public class SystemConfiguration : VersionedConfiguration
	{
		public SystemConfiguration()
		{
			Sounds = new List<Sound>();
			JournalFilters = new List<JournalFilter>();
			EmailData = new EmailData();
			AutomationConfiguration = new AutomationConfiguration();
			RviSettings = new RviSettings();
			RviServers = new List<RviServer>();
			RviDevices = new List<RviDevice>();
			Cameras = new List<Camera>();
		}

		[DataMember]
		public List<Sound> Sounds { get; set; }

		[DataMember]
		public List<JournalFilter> JournalFilters { get; set; }

		[DataMember]
		public RviSettings RviSettings { get; set; }

		[DataMember]
		public List<RviServer> RviServers { get; set; }

		[DataMember]
		public EmailData EmailData { get; set; }

		[DataMember]
		public AutomationConfiguration AutomationConfiguration { get; set; }

		public void UpdateConfiguration()
		{
			AutomationConfiguration.UpdateConfiguration();
			UpdateRviConfiguration();
		}

		[XmlIgnore]
		public List<Camera> Cameras { get; set; }
		[XmlIgnore]
		public List<RviDevice> RviDevices { get; set; }
		public void UpdateRviConfiguration()
		{
			RviDevices = new List<RviDevice>();
			RviServers.ForEach(server => RviDevices.AddRange(server.RviDevices));
			Cameras = new List<Camera>();
			RviDevices.ForEach(device => Cameras.AddRange(device.Cameras));
		}
	}
}