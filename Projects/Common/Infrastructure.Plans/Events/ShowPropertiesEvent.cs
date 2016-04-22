using Microsoft.Practices.Prism.Events;
using RubezhAPI.Plans.Elements;

namespace Infrastructure.Plans.Events
{
	public class ShowPropertiesEvent : CompositePresentationEvent<ShowPropertiesEventArgs>
	{
	}
	public class ShowPropertiesEventArgs
	{
		public ShowPropertiesEventArgs(ElementBase element)
		{
			Element = element;
		}

		public ElementBase Element { get; private set; }
		public object PropertyViewModel { get; set; }
	}
}