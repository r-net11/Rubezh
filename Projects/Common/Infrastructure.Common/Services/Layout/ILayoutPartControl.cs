using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Automation;

namespace Infrastructure.Common.Services.Layout
{
	public interface ILayoutPartControl
	{
		object GetProperty(LayoutPartPropertyName property);
		void SetProperty(LayoutPartPropertyName property, object value);
	}
}
