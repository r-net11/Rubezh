using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class DriverState
	{
		[DataMember]
		public string Code { get; set; }

		[DataMember]
		public string Id { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public StateType StateType { get; set; }

		[DataMember]
		public bool AffectChildren { get; set; }

		[DataMember]
		public bool AffectParent { get; set; }

		[DataMember]
		public bool IsManualReset { get; set; }

		[DataMember]
		public bool CanResetOnPanel { get; set; }

		[DataMember]
		public bool IsAutomatic { get; set; }

		public DriverState Clone()
		{
			return new DriverState()
			{
				Code = Code,
				Id = Id,
				Name = Name,
				StateType = StateType,
				AffectChildren = AffectChildren,
				AffectParent = AffectParent,
				IsManualReset = IsManualReset,
				CanResetOnPanel = CanResetOnPanel,
				IsAutomatic = IsAutomatic
			};
		}
	}
}