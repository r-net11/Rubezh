using System.Runtime.Serialization;

namespace StrazhAPI.Models
{
	[DataContract]
	[KnownType(typeof(Plan))]
	[KnownType(typeof(PlanFolder))]
	public class PlanFolder : Plan
	{
		public PlanFolder()
		{
            Caption = Resources.Language.Models.Plans.PlanFolder.Caption;
		}
	}
}