﻿using System.Collections.Generic;
using System.Runtime.Serialization;
using Entities.DeviceOriented;
using FiresecAPI.Automation;
using FiresecAPI.Journal;
using System.Xml.Serialization;

namespace FiresecAPI.Models
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