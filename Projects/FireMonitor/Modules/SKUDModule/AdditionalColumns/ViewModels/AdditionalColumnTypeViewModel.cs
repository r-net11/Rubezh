using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;

namespace SKDModule.ViewModels
{
	public class AdditionalColumnTypeViewModel : BaseViewModel
	{
		public AdditionalColumnType AdditionalColumnType { get; private set; }

		public AdditionalColumnTypeViewModel(AdditionalColumnType additionalColumnType)
		{
			AdditionalColumnType = additionalColumnType;
		}

		public void Update(AdditionalColumnType additionalColumn)
		{
			AdditionalColumnType = additionalColumn;
			OnPropertyChanged(() => AdditionalColumnType);
		}
	}
}