using System.Collections.Generic;
using FiresecAPI.Models;

namespace ServerFS2.ConfigurationWriter
{
	public class NonPanelDatabase
	{
		public SystemDatabaseCreator ConfigurationWriterHelper { get; set; }

		public Device ParentPanel { get; set; }
		public BytesDatabase BytesDatabase { get; set; }
		public List<ByteDescription> RootBytes { get; set; }

		public List<BytesDatabase> Tables = new List<BytesDatabase>();
		public BytesDatabase FirstTable;
		public ByteDescription Crc16ByteDescription;

		public void CreateRootBytes()
		{
			RootBytes = new List<ByteDescription>();
			var startOffset = 0;
			BytesDatabase.Order(startOffset);
			BytesDatabase.ResolveTableReferences();
			BytesDatabase.ResolverReferences();
		}
	}
}