﻿namespace Infrastructure.Common.Windows.ViewModels
{
	internal class DialogHeaderViewModel : BaseViewModel, IHeaderViewModel
	{
		public DialogHeaderViewModel(DialogViewModel content)
		{
			Content = content;
			ShowIconAndTitle = true;
		}

		#region IHeaderViewModel Members

		private HeaderedWindowViewModel _content;

		public HeaderedWindowViewModel Content
		{
			get { return _content; }
			set
			{
				_content = value;
				OnPropertyChanged("Content");
			}
		}

		public bool ShowIconAndTitle { get; set; }

		#endregion IHeaderViewModel Members
	}
}