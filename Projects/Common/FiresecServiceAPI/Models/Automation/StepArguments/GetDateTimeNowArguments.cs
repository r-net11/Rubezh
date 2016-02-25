using System.Runtime.Serialization;
using FiresecAPI.Enums;

namespace FiresecAPI.Automation
{
	public class GetDateTimeNowArguments
	{
		public GetDateTimeNowArguments()
		{
			Result = new Argument();
		}

		[DataMember]
		public Argument Result { get; set; }

		[DataMember]
		public RoundingType RoundingType { get; set; }
	}
}
