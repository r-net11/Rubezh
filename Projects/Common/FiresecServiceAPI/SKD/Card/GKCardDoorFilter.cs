using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class GKCardDoorFilter : IsDeletedFilter
	{
		public GKCardDoorFilter()
			: base()
		{
		}
	}
}