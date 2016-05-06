using System.ComponentModel;
using System.Runtime.Serialization;

namespace StrazhAPI.Automation
{
	[DataContract]
	public class GetListItemArguments
	{
		public GetListItemArguments()
		{
			ListArgument = new Argument();
			ItemArgument = new Argument();
			IndexArgument = new Argument();
		}

		[DataMember]
		public Argument ListArgument { get; set; }

		[DataMember]
		public Argument ItemArgument { get; set; }

		[DataMember]
		public Argument IndexArgument { get; set; }

		[DataMember]
		public PositionType PositionType { get; set; }
	}

	public enum PositionType
	{
		[Description("Первый")]
		First,

		[Description("Последний")]
		Last,

		[Description("По индексу")]
		ByIndex
	}
}