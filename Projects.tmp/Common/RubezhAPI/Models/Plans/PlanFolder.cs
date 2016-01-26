using System.Runtime.Serialization;

namespace RubezhAPI.Models
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