using Infrustructure.Plans.Designer;
using StrazhAPI.Plans.Elements;
using Microsoft.Practices.Prism.Events;

namespace Infrustructure.Plans.Events
{
	public class ContextMenuFactoryEvent : CompositePresentationEvent<ContextMenuFactoryEventArgs>
	{
	}

	public class ContextMenuFactoryEventArgs
	{
		public ContextMenuFactoryEventArgs(ElementBase element)
		{
			Element = element;
		}

		public ElementBase Element { get; private set; }

		public DesignerItem DesignerItem { get; set; }
	}
}