using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace ClientFS2.ConfigurationWriter.Classes
{
	public class TableGroup
	{
		public TableGroup(string name, int length = 0)
		{
			Tables = new List<TableBase>();
			Name = name;
			Length = length;
		}

		public List<TableBase> Tables { get; set; }
		public string Name { get; set; }

		public int Count
		{
			get { return Tables.Count; }
		}

		public int Length { get; set; }

		public int ComputedLength
		{
			get
			{
				if (Length == -1)
					return 0;

				var lenght = 0;
				foreach(var table in Tables)
				{
					var tableLength = table.BytesDatabase.ByteDescriptions.Count;
					if (tableLength != lenght && lenght != 0)
						return 0;
					lenght = tableLength;
				}
				return lenght;
			}
		}

		public ByteDescription GetTreeRootByteDescription()
		{
			var rootByteDescription = new ByteDescription()
			{
				Description = Name,
				IsHeader = true
			};

			foreach (var table in Tables)
			{
				var tableByteDescription = table.GetTreeRootByteDescription();
				rootByteDescription.Children.Add(tableByteDescription);
			}
			var rootChild = rootByteDescription.Children.FirstOrDefault();
			if (rootChild != null)
			{
				rootByteDescription.Offset = rootChild.Offset;
			}
			else
			{
				rootByteDescription.HasNoOffset = true;
			}

			return rootByteDescription;
		}
	}
}