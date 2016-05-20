using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.TreeList;
using System.Collections.Specialized;

namespace ReportsModule.ViewModels
{
	public abstract class SKDReportBaseViewModel : TreeNodeViewModel<SKDReportBaseViewModel>
	{
		public SKDReportBaseViewModel()
		{
		}

		public bool HasGroupChildren { get; protected set; }

		public abstract string Title { get; }
		public abstract string IconSource { get; }
		public abstract int SortIndex { get; }

		public virtual void Reset()
		{
		}
	}
	public class SKDReportViewModelComparer : TreeNodeComparer<SKDReportBaseViewModel>
	{
		protected override int Compare(SKDReportBaseViewModel x, SKDReportBaseViewModel y)
		{
			return x.SortIndex - y.SortIndex;
		}
	}
}
