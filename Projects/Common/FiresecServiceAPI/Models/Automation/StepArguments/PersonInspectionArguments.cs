using System;
using System.Runtime.Serialization;


namespace FiresecAPI.Automation
{
	[DataContract]
	public class PersonInspectionArguments
	{
		public PersonInspectionArguments()
		{
			Uid = new Guid();
			CardNumber = new ArithmeticParameter();
		}

		[DataMember]
		public Guid Uid { get; set; }
		
		[DataMember]
		public ArithmeticParameter CardNumber { get; set; }
	}
}
