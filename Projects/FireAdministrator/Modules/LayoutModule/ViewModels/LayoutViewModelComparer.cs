using System.Collections;
using System.Collections.Generic;

namespace LayoutModule.ViewModels
{
	public class LayoutViewModelComparer : IComparer, IComparer<LayoutViewModel>
	{
		public int Compare(LayoutViewModel x, LayoutViewModel y)
		{
			return string.Compare(x.Caption, y.Caption);
		}

		#region IComparer Members

		int IComparer.Compare(object x, object y)
		{
			return Compare((LayoutViewModel)x, (LayoutViewModel)y);
		}

		#endregion
	}
}