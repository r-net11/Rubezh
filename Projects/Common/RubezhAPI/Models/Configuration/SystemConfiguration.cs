using RubezhAPI.Automation;
using RubezhAPI.Journal;
using System.Collections.Generic;
using System.Runtime.Serialization;

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
		}

		public override bool ValidateVersion()
		{
			return true;
		}
		public List<Camera> Cameras
		{
			get
			{
				var cameras = new List<Camera>();
				RviServers.ForEach(server => server.RviDevices.ForEach(device => device.RviChannels.ForEach(channel => cameras.AddRange(channel.Cameras))));
				return cameras;
			}
		}
		public List<RviDevice> RviDevices
		{
			get
			{
				var devices = new List<RviDevice>();
				RviServers.ForEach(server => devices.AddRange(server.RviDevices));
				return devices;
			}
		}
	}
}