using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Common.Services.Layout
{
	public class LayoutPartContainerCollection : List<ILayoutPartContainer>, ILayoutPartContainer
	{
		#region ILayoutPartContainer Members

		public Guid UID
		{
			get { throw new NotSupportedException(); }
		}

		public string Title
		{
			get { throw new NotSupportedException(); }
			set { ForEach(item => item.Title = value); }
		}

		public string IconSource
		{
			get { throw new NotSupportedException(); }
			set { ForEach(item => item.IconSource = value); }
		}

		public bool IsActive
		{
			get { throw new NotSupportedException(); }
			set { ForEach(item => item.IsActive = value); }
		}

		public bool IsSelected
		{
			get { throw new NotSupportedException(); }
			set { ForEach(item => item.IsSelected = value); }
		}

		#endregion
	}
}
