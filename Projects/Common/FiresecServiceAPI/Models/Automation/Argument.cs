using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class Argument
	{
		public Argument()
		{

		}

		public Argument(Variable variable)
		{
			VariableItem = variable.VariableItem;
		}

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public bool IsList { get; set; }

		[DataMember]
		public Guid VariableUid { get; set; }

		[DataMember]
		public ExplicitType ExplicitType { get; set; }

		[DataMember]
		public ObjectType ObjectType { get; set; }

		[DataMember]
		public EnumType EnumType { get; set; }

		[DataMember]
		public VariableItem VariableItem { get; set; }
	}
}