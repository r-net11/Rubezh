using Infrastructure.Common.Windows.ViewModels;
using Localization.Automation.ViewModels;

namespace AutomationModule.ViewModels
{
	public class StringDetailsViewModel : SaveCancelDialogViewModel
	{
		public StringDetailsViewModel(string stringValue)
		{
			Title = CommonViewModel.StringDetailsViewModel_Title;
			StringValue = stringValue;
		}

		string _stringValue;
		public string StringValue
		{
			get { return _stringValue; }
			set
			{
				_stringValue = value;
				OnPropertyChanged(() => StringValue);
			}
		}
	}
}
