using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrustructure.Plans.Devices
{
	public interface ILibraryFrame
	{
		int Id { get; set; }
		int Duration { get; set; }
		string Image { get; set; }
	}
}
