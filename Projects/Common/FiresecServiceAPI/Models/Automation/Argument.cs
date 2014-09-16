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
			VariableItem = new VariableItem();
			PropertyCopy.Copy<VariableItem, VariableItem>(variable.DefaultVariableItem, VariableItem);
			VariableUid = variable.Uid;
		}

		[DataMember]
		public Guid VariableUid { get; set; }

		[DataMember]
		public VariableItem VariableItem { get; set; }

		[DataMember]
		public string Description { get; set; }
	}
}