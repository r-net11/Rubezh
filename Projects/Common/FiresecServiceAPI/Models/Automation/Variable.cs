using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.GK;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class Variable
	{
		public Variable()
		{
			Uid = Guid.NewGuid();
			VariableItem = new VariableItem();
			VariableItems = new List<VariableItem>();
		}

		public Variable(string name) : base()
		{
			Name = name;
		}
		
		[DataMember]
		public bool IsList { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public ExplicitType ExplicitType { get; set; }

		[DataMember]
		public ObjectType ObjectType { get; set; }

		[DataMember]
		public EnumType EnumType { get; set; }

		[DataMember]
		public bool IsGlobal { get; set; }

		[DataMember]
		public VariableItem VariableItem { get; set; }

		[DataMember]
		public VariableItem DefaultVariableItem { get; set; }

		[DataMember]
		public List<VariableItem> VariableItems { get; set; }

		public void ResetValue()
		{
			VariableItem = (VariableItem)DefaultVariableItem.Clone();
		}

		public void ResetValue(Argument argument)
		{
			VariableItem = argument.VariableItem;
		}
	}

	public class VariableItem : ICloneable
	{
		public VariableItem()
		{
			DateTimeValue = DateTime.Now;
			IntValue = 0;
			StringValue = "";
			UidValue = Guid.Empty;
		}

		public object Clone()
		{
			return this.MemberwiseClone();
		}

		[DataMember]
		public int IntValue { get; set; }

		[DataMember]
		public bool BoolValue { get; set; }

		[DataMember]
		public DateTime DateTimeValue { get; set; }

		[DataMember]
		public Guid UidValue { get; set; }

		[DataMember]
		public string StringValue { get; set; }

		[DataMember]
		public XStateClass StateClassValue { get; set; }

		[DataMember]
		public string DeviceType { get; set; }

		[DataMember]
		public ExplicitType ExplicitType { get; set; }
	}
}