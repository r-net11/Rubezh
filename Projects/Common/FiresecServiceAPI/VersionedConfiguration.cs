using System.Runtime.Serialization;

namespace StrazhAPI
{
	[DataContract]
	public class VersionedConfiguration
	{
		public VersionedConfiguration()
		{
			Version = new ConfigurationVersion();
		}

		public virtual void AfterLoad()
		{
		}

		public virtual void BeforeSave()
		{
		}

		public virtual bool ValidateVersion()
		{
			return true;
		}

		[DataMember]
		public ConfigurationVersion Version { get; set; }
	}
}