using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.IO;

namespace Infrastructure.Client.Images
{
	public class WMFImage
	{
		public WMFImage()
		{
			Resources = new Dictionary<Guid, byte[]>();
		}

		public Canvas Canvas { get; set; }
		public Dictionary<Guid, byte[]> Resources { get; set; }
	}
}
