using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StrazhAPI;
using Infrastructure.Common.SKDReports;
using System.Collections.Specialized;

namespace ReportsModule.ViewModels
{
	public class SKDReportGroupViewModel : SKDReportBaseViewModel
	{
		public SKDReportGroupViewModel(SKDReportGroup group)
		{
			SKDReportGroup = group;
			_title = group.ToDescription();
			IsExpanded = true;
		}

		public SKDReportGroup SKDReportGroup { get; private set; }
		public override string IconSource
		{
			get { return "/Controls;component/Images/CFolder.png"; }
		}
		private string _title;
		public override string Title
		{
			get { return _title; }
		}
		public override int SortIndex
		{
			get { return (int)SKDReportGroup; }
		}

		protected override void OnChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			HasGroupChildren = HasChildren;
			OnPropertyChanged(() => HasGroupChildren);
		}
	}
}
