using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace ClientFS2.ConfigurationWriter
{
	public class TableBase
	{
		public PanelDatabase2 PanelDatabase { get; set; }
		public Device ParentPanel
		{
			get { return PanelDatabase.ParentPanel; }
		}
		public BytesDatabase BytesDatabase { get; set; }
		public List<TableBase> ReferenceTables { get; set; }

		public TableBase(PanelDatabase2 panelDatabase, string name = null)
		{
			PanelDatabase = panelDatabase;
			BytesDatabase = new BytesDatabase(name);
			ReferenceTables = new List<TableBase>();
		}

		public virtual void Create()
		{

		}

		public virtual Guid UID
		{
			get { return Guid.Empty; }
		}

		public ByteDescription GetTreeRootByteDescription()
		{
			var rootByteDescription = new ByteDescription()
			{
				Description = BytesDatabase.Name,
				IsHeader = true
			};
			foreach (var byteDescription in BytesDatabase.ByteDescriptions)
			{
				rootByteDescription.Children.Add(byteDescription);
			}
			var tableChild = rootByteDescription.Children.FirstOrDefault();
			if (tableChild != null)
			{
				rootByteDescription.Offset = tableChild.Offset;
			}
			else
			{
				rootByteDescription.HasNoOffset = true;
			}
			return rootByteDescription;
		}
	}
}