using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.Automation;

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

		public override bool ValidateVersion()
		{
			var result = true;
			if (Instructions == null)
			{
				Instructions = new List<Instruction>();
				result = false;
			}
			if (EmailData == null)
			{
				EmailData = new EmailData();
				result = false;
			}
			if (AutomationConfiguration == null)
			{
				AutomationConfiguration = new AutomationConfiguration();
				result = false;
			}
			return result;
		}

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