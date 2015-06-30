using System.Runtime.Serialization;

namespace Infrastructure.Models
{
	[DataContract]
	public class AutoActivationSettings
	{
		public AutoActivationSettings()
		{
			IsAutoActivation = false;
			IsPlansAutoActivation = false;
		}

		[DataMember]
		public bool IsAutoActivation { get; set; }

		[DataMember]
		public bool IsPlansAutoActivation { get; set; }
	}
}