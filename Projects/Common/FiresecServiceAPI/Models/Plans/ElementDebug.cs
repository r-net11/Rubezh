using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrustructure.Plans.Elements;
using System.Runtime.Serialization;

namespace FiresecAPI.Models.Plans
{
	public class ElementDebug : ElementBaseRectangle, IElementZIndex
	{
		public override Infrustructure.Plans.Elements.ElementBase Clone()
		{
			var element = new ElementDebug();
			element.ZIndex = ZIndex;
			Copy(element);
			return element;
		}

		#region IElementZIndex Members

		[DataMember]
		public int ZIndex { get; set; }

		#endregion
	}
}
