using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Common.Windows.ViewModels
{
	public sealed class ConnectionViewModel : WindowBaseViewModel
	{
		public ConnectionViewModel()
		{
			MinHeight = 100;
			MinWidth = 300;
			MaxHeight = MinHeight;
			MaxWidth = MinWidth;
		}
	}
}
