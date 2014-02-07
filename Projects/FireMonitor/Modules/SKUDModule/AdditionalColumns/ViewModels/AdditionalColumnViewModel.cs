using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;

namespace SKDModule.ViewModels
{
	public class AdditionalColumnViewModel : BaseViewModel
	{
		public AdditionalColumn AdditionalColumn { get; private set; }

		public AdditionalColumnViewModel(AdditionalColumn additionalColumn)
		{
			AdditionalColumn = additionalColumn;
		}

		public void Update(AdditionalColumn additionalColumn)
		{
			AdditionalColumn = additionalColumn;
			OnPropertyChanged("AdditionalColumn");
		}
	}
}