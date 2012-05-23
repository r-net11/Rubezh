using System.Windows;

namespace Infrastructure.Common
{
	public class DialogContent : BaseViewModel, IDialogContent
	{
		public string Title { get; set; }

		public object InternalViewModel
		{
			get { return this; }
		}

		public Window Surface { get; set; }

		public virtual void Close(bool result)
		{
			if (Surface != null)
			{
				Surface.DialogResult = result;
				Surface.Close();
			}
		}

		public RelayCommand SaveCommand { get; protected set; }
		public RelayCommand CancelCommand { get; protected set; }
	}
}