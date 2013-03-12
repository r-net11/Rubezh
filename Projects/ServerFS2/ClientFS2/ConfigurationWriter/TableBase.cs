using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace ClientFS2.ConfigurationWriter
{
	public class TableBase
	{
		public PanelDatabase PanelDatabase { get; set; }
		public Device ParentPanel
		{
			get { return PanelDatabase.ParentPanel; }
		}
		public BytesDatabase BytesDatabase { get; set; }
		public List<BytesDatabase> ReferenceBytesDatabase { get; set; }

		public TableBase(PanelDatabase panelDatabase)
		{
			PanelDatabase = panelDatabase;
			BytesDatabase = new BytesDatabase();
			ReferenceBytesDatabase = new List<BytesDatabase>();
		}

		public virtual void Create()
		{

		}

		public virtual Guid UID
		{
			get { return Guid.Empty; }
		}
	}
}