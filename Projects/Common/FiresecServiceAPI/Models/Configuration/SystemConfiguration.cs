using System.Collections.Generic;
using System.Runtime.Serialization;
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
			Instructions = new List<Instruction>();
			Cameras = new List<Camera>();
			EmailData = new EmailData();
			AutomationConfiguration = new AutomationConfiguration();
		}

		[DataMember]
		public List<Sound> Sounds { get; set; }
	
		[DataMember]
		public List<JournalFilter> JournalFilters { get; set; }

		[DataMember]
		public List<Instruction> Instructions { get; set; }

		[DataMember]
		public List<Camera> Cameras { get; set; }

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
			var result = true;
			if (Instructions == null)
			{
				Instructions = new List<Instruction>();
				result = false;
			}
			if (AutomationConfiguration == null)
			{
				AutomationConfiguration = new AutomationConfiguration();
				result = false;
			}
			if (AutomationConfiguration.GlobalVariables == null)
			{
				AutomationConfiguration.GlobalVariables = new List<Variable>();
				result = false;
			}
			if (JournalFilters == null)
			{
				JournalFilters = new List<JournalFilter>();
				result = false;
			}
			return result;
		}

		[XmlIgnore]
		public List<Camera> AllCameras
		{
			get
			{
				var AllCameras = new List<Camera>();
				foreach (var camera in Cameras)
				{
					AllCameras.Add(camera);
					if (camera.CameraType == CameraType.Dvr)
						AllCameras.AddRange(camera.Children);
				}
				return AllCameras;
			}
		}
	}
}