using Infrastructure.Plans.Designer;
using Microsoft.Practices.Prism.Events;
using RubezhAPI.Plans.Elements;

namespace Infrastructure.Plans.Events
{
	public class DesignerItemFactoryEvent : CompositePresentationEvent<DesignerItemFactoryEventArgs>
	{
	}

	public class DesignerItemFactoryEventArgs
	{
		public DesignerItemFactoryEventArgs(ElementBase element)
		{
			Element = element;
		}

		public ElementBase Element { get; private set; }
		public DesignerItem DesignerItem { get; set; }
	}
}