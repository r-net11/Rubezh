using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Common.Windows.ViewModels
{
	class DialogHeaderViewModel : BaseViewModel, IHeaderViewModel
	{
		public DialogHeaderViewModel(DialogViewModel content)
		{
			Content = content;
		}

		#region ICaptionedHeaderViewModel Members

		public double Height
		{
			get { return 30; }
		}

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

		#endregion
	}
}
