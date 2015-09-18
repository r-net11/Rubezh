using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResursAPI
{
	public class ModelBase
	{
		public ModelBase()
		{
			UID = Guid.NewGuid();
		}

		public Guid UID { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
	}
}