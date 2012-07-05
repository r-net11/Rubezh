using System.Runtime.Serialization;
using System.Windows.Media;

namespace Infrustructure.Plans.Elements
{
	public interface IElementZone
	{
		ulong? ZoneNo { get; set; }
		Color BackgroundColor { get; set; }
	}
}