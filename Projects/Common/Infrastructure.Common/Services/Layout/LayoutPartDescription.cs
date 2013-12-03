using System;
using System.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Common.Services.Layout
{
	public class LayoutPartDescription : ILayoutPartDescription
	{
		public LayoutPartDescription()
		{
			PreferedSize = new Size(100, 100);
		}

		#region ILayoutPartDescription Members

		public Guid UID { get; set; }
		public int Index { get; set; }
		public string Name { get; set; }
		public string IconSource { get; set; }
		public string Description { get; set; }
		public bool AllowMultiple { get; set; }
		public BaseViewModel Content { get; set; }
		public Size PreferedSize { get; private set; }

		#endregion
	}
}
