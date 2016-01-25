using System.Runtime.Serialization;

namespace RubezhAPI.SKD
{
	[DataContract]
	public class CardDoorFilter : IsDeletedFilter
	{
		public CardDoorFilter()
			: base()
		{
		}
	}
}