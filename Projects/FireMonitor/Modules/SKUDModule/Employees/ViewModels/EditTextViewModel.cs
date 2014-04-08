using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class EditTextViewModel : SaveCancelDialogViewModel
	{
		string _text;
		public string Text
		{
			get { return _text; }
			set
			{
				_text = value;
				OnPropertyChanged(() => Text);
			}
		}
		
		public EditTextViewModel(string name = "", string text = "")
		{
			Title = string.Format("Редактировать колонку {0}", name);
			Text = text;
		}
	}
}
