using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Events;

namespace Infrustructure.Plans.Elements
{
	public class ElementError
	{
		public Guid PlanUID { get; set; }
		public ElementBase Element { get; set; }
		public string Error { get; set; }
		public string ImageSource { get; set; }
		public bool IsCritical { get; set; }
		public Action Navigate { get; set; }
	}
}