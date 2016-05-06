using System.ComponentModel;
using System.Runtime.Serialization;

namespace StrazhAPI.Automation
{
	[DataContract]
	public class IncrementValueArguments
	{
		public IncrementValueArguments()
		{
			ResultArgument = new Argument();
		}

		[DataMember]
		public Argument ResultArgument { get; set; }

		[DataMember]
		public IncrementType IncrementType { get; set; }
	}

	public enum IncrementType
	{
		[Description("Инкремент")]
		Inc,

		[Description("Декремент")]
		Dec,
	}
}