using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	[KnownType(typeof(Plan))]
	[KnownType(typeof(PlanFolder))]
	public class PlanFolder : Plan
	{
		public PlanFolder()
		{
			Caption = "Папка";
		}
	}
}
