using System;
using System.Linq;
using FiresecAPI.Models;

namespace ServerFS2.ConfigurationWriter
{
	public class TableBase
	{
		public FlashDatabase PanelDatabase { get; set; }
		public Device ParentPanel
		{
			get { return PanelDatabase.ParentPanel; }
		}
		public BytesDatabase BytesDatabase { get; set; }

		public TableBase(FlashDatabase panelDatabase, string name = null)
		{
			PanelDatabase = panelDatabase;
			BytesDatabase = new BytesDatabase(name);
		}

		public virtual void Create() { }

		Guid uid = Guid.NewGuid();
		public virtual Guid UID
		{
			get { return uid; }
		}

		public Guid ReferenceUID = Guid.NewGuid();

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