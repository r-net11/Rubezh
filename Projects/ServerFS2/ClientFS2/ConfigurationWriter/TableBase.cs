using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace ClientFS2.ConfigurationWriter
{
	public class TableBase
	{
		public Device PanelDevice;
		public BytesDatabase BytesDatabase;
		public List<BytesDatabase> ReferenceBytesDatabase { get; set; }

		public TableBase(Device panelDevice)
		{
			PanelDevice = panelDevice;
			BytesDatabase = new BytesDatabase();
			ReferenceBytesDatabase = new List<BytesDatabase>();
		}

		public virtual void Create()
		{

		}
	}
}