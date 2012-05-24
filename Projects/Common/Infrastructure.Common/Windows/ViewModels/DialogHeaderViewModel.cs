using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Common.Windows.ViewModels
{
	class DialogHeaderViewModel : ICaptionedHeaderViewModel
	{
		#region ICaptionedHeaderViewModel Members

		public double Height
		{
			get { return 45; }
		}

		#endregion
	}
}
