using System.Text;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.TreeList;

namespace ServerFS2.ConfigurationWriter
{
	public class ByteDescription : TreeItemViewModel<ByteDescription>
	{
		public TableBase TableHeader { get; set; }

		public int RelativeOffset { get; set; }
		public int Offset { get; set; }
		public int Value { get; set; }
		public string Description { get; set; }
		public string GroupName { get; set; }
		public string RealValue { get; set; }

		public ByteDescription AddressReference { get; set; }
		public TableBase TableBaseReference { get; set; }

		public bool IsBold { get; set; }
		public bool IsHeader { get; set; }
		public bool HasNoOffset { get; set; }

		public bool IsNotEqualToOriginal { get; set; }
		public int OriginalValue { get; set; }
		public string OriginalReference { get; set; }
		public bool IsReadOnly { get; set; }
		public bool IgnoreUnequal { get; set; }
		public string OriginalChar
		{
			get
			{
				var originalChar = Encoding.Default.GetString(new byte[] { (byte)OriginalValue }, 0, 1);
				return originalChar;
			}
		}

		public Device DeviceState { get; set; }
	}
}