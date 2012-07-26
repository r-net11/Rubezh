using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Events;

namespace Infrustructure.Plans.Events
{
	public class RegisterPlanExtensionEvent<T> : CompositePresentationEvent<IPlanExtension<T>>
	{
	}
}
