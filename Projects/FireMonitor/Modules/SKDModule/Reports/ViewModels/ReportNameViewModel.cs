using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class ReportNameViewModel:SaveCancelDialogViewModel
	{
		public ReportNameViewModel(string text)
		{
			Title = "Введите название отчёта";
			Text = text;
		}

		string _Text;
		public string Text
		{
			get { return _Text; }
			set
			{
				_Text = value;
				OnPropertyChanged(() => Text);
			}
		}
	}
}
