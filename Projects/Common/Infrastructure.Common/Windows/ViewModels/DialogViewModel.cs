using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Common.Windows.ViewModels
{
	public class DialogViewModel : HeaderedWindowViewModel
	{
		public DialogViewModel()
		{
			Header = new DialogHeaderViewModel(this);
			CloseOnEscape = true;
		}
	}
}
