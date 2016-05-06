using System.Runtime.Serialization;

namespace StrazhAPI.SKD
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