using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.SKD.ReportFilters;
using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Common.SKDReports
{
	public abstract class FilterContainerViewModel : BaseViewModel
	{
		string _title;
		public string Title
		{
			get { return _title; }
			set
			{
				_title = value;
				OnPropertyChanged(() => Title);
			}
		}
		string _imageSource;
		public string ImageSource
		{
			get { return _imageSource; }
			set
			{
				_imageSource = value;
				OnPropertyChanged(() => ImageSource);
			}
		}

		public abstract void LoadFilter(SKDReportFilter filter);
		public abstract void UpdateFilter(SKDReportFilter filter);
	}
}