using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Events;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Designer;

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
