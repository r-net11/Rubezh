using Infrustructure.Plans.Designer;
using StrazhAPI.Plans.Elements;
using Microsoft.Practices.Prism.Events;

namespace Infrustructure.Plans.Events
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