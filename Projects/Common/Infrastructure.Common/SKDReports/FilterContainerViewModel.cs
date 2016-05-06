using StrazhAPI.SKD.ReportFilters;
using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Common.SKDReports
{
	public abstract class FilterContainerViewModel : BaseViewModel
	{
		public FilterContainerViewModel()
		{
			IsActive = true;
		}

		private string _title;

		public string Title
		{
			get { return _title; }
			set
			{
				_title = value;
				OnPropertyChanged(() => Title);
			}
		}

		private string _imageSource;

		public string ImageSource
		{
			get { return _imageSource; }
			set
			{
				_imageSource = value;
				OnPropertyChanged(() => ImageSource);
			}
		}

		public bool IsActive { get; set; }

		public abstract void LoadFilter(SKDReportFilter filter);

		public abstract void UpdateFilter(SKDReportFilter filter);
	}
}