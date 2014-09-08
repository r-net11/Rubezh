using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	public class ArithmeticParameter
	{
		public ArithmeticParameter()
		{
			VariableUid = new Guid();
			BoolValue = false;
			DateTimeValue = DateTime.Now;
			IntValue = 0;
			StringValue = "";
			TypeValue = "";
		}

		[DataMember]
		public VariableType VariableType { get; set; }

		[DataMember]
		public bool BoolValue { get; set; }

		[DataMember]
		public DateTime DateTimeValue { get; set; }

		[DataMember]
		public int IntValue { get; set; }

		[DataMember]
		public string StringValue { get; set; }

		[DataMember]
		public Guid UidValue { get; set; }

		[DataMember]
		public string TypeValue { get; set; }

		[DataMember]
		public Guid VariableUid { get; set; }
	}
}
