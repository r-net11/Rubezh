using System.Runtime.Serialization;
using StrazhAPI.Enums;

namespace StrazhAPI.Automation
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
