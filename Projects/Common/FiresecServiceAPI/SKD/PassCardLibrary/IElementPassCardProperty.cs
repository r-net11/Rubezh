using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecAPI.SKD.PassCardLibrary
{
	public interface IElementPassCardProperty
	{
		PassCardPropertyType PropertyType { get; set; }
	}
}
