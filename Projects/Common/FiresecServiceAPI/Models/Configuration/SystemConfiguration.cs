using Entities.DeviceOriented;
using System.Collections.Generic;
using System.Runtime.Serialization;
using StrazhAPI.Integration.OPC;
using StrazhAPI.SKD.ReportFilters;
using StrazhAPI.Automation;
using StrazhAPI.Journal;

namespace StrazhAPI.Models
{
	[DataContract]
	public class SystemConfiguration : VersionedConfiguration
	{
		public SystemConfiguration()
		{
			Sounds = new List<Sound>();
			JournalFilters = new List<JournalFilter>();
			Cameras = new List<Camera>();
			Devices = new List<Device>();
			EmailData = new EmailData();
			AutomationConfiguration = new AutomationConfiguration();
			RviSettings = new RviSettings();
		}

		[DataMember]
		public List<Sound> Sounds { get; set; }

		[DataMember]
		public List<JournalFilter> JournalFilters { get; set; }

		[DataMember]
		public RviSettings RviSettings { get; set; }

		[DataMember]
		public List<Camera> Cameras { get; set; }

		[DataMember]
		public List<Device> Devices { get; set; }

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
	}
}