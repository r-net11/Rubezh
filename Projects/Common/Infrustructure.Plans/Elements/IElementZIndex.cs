using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	public interface IElementZIndex
	{
		[DataMember]
		int ZIndex { get; set; }
	}
}