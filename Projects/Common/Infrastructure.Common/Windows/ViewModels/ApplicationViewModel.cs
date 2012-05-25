using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Common.Windows.ViewModels
{
	public class ApplicationViewModel : HeaderedWindowViewModel
	{
		public ApplicationViewModel()
		{
			Header = new ApplicationHeaderViewModel(this);
			CloseOnEscape = false;
		}
	}
}
