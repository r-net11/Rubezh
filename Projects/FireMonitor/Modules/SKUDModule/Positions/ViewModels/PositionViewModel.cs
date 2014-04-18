using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;

namespace SKDModule.ViewModels
{
	public class PositionViewModel : BaseViewModel
	{
		public ShortPosition Position { get; private set; }

		public PositionViewModel(ShortPosition position)
		{
			Position = position;
		}

		public void Update(ShortPosition position)
		{
			Position = position;
			OnPropertyChanged("Position");
		}
	}
}