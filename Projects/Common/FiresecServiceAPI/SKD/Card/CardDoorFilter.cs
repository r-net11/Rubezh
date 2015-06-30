using System.Runtime.Serialization;

namespace FiresecAPI.SKD
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