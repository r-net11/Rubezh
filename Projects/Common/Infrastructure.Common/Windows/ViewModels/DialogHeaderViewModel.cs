using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Common.Windows.ViewModels
{
	class DialogHeaderViewModel : BaseViewModel, IHeaderViewModel
	{
		private string _title;
		#region ICaptionedHeaderViewModel Members

		public double Height
		{
			get { return 30; }
		}

		public string Title
		{
			get { return _title; }
			set
			{
				_title = value;
				OnPropertyChanged("Title");
			}
		}

		#endregion
	}
}
