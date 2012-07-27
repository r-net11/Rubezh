using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class DriversConfiguration : VersionedConfiguration
	{
		public DriversConfiguration()
		{
			Drivers = new List<Driver>();
		}

		[DataMember]
		public List<Driver> Drivers { get; set; }
	}
}