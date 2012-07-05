using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Infrustructure.Plans.Elements;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementRectangleZone : ElementBaseRectangle, IElementZone
	{
		public Zone Zone { get; set; }

		[DataMember]
		public ulong? ZoneNo { get; set; }

		public override ElementBase Clone()
		{
			ElementBase elementBase = new ElementRectangleZone()
			{
				ZoneNo = ZoneNo
			};
			Copy(elementBase);
			return elementBase;
		}
	}
}