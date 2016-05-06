using System.ComponentModel;
using System.Runtime.Serialization;

namespace StrazhAPI.Automation
{
	[DataContract]
	public class ChangeListArguments
	{
		public ChangeListArguments()
		{
			ListArgument = new Argument();
			ItemArgument = new Argument();
		}

		[DataMember]
		public Argument ListArgument { get; set; }

		[DataMember]
		public Argument ItemArgument { get; set; }

		[DataMember]
		public ChangeType ChangeType { get; set; }
	}

	public enum ChangeType
	{
		[Description("Добавить в конец")]
		AddLast,

		[Description("Удалить первый")]
		RemoveFirst,

		[Description("Удалить все")]
		RemoveAll
	}
}