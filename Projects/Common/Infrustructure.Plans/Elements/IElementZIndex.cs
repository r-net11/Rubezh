using System.Runtime.Serialization;

namespace Infrustructure.Plans.Elements
{
	public interface IElementZIndex
	{
		[DataMember]
		int ZIndex { get; set; }
	}
}