using System;
using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Common.Services.Layout
{
	public class LayoutPartDescription : ILayoutPartDescription
	{
		#region ILayoutPartDescription Members

		public Guid UID { get; set; }
		public int Index { get; set; }
		public string Name { get; set; }
		public string IconSource { get; set; }
		public string Description { get; set; }
		public bool AllowMultiple { get; set; }
		public BaseViewModel Content { get; set; }

		#endregion
	}
}
