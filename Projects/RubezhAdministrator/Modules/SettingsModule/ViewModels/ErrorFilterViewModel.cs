using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;
using Infrastructure.Common;

namespace SettingsModule.ViewModels
{
	public class ErrorFilterViewModel : BaseViewModel
	{
		public ErrorFilterViewModel(ValidationErrorType errorType)
		{
			ErrorType = errorType;
		}

		public ValidationErrorType ErrorType { get; private set; }

		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged(() => IsChecked);
			}
		}
	}
}