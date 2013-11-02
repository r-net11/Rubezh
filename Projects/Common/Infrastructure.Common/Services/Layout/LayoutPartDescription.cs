using System;

namespace Infrastructure.Common.Services.Layout
{
	public class LayoutPartDescription : ILayoutPartDescription
	{
		#region ILayoutPartDescription Members

		public Guid UID { get; set; }
		public int Index { get; set; }
		public string Name { get; set; }
		public string ImageSource { get; set; }
		public string Description { get; set; }
		public bool AllowMultiple { get; set; }

		#endregion
	}
}
