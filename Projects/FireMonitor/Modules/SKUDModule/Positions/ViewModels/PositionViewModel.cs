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
		public Position Position { get; private set; }

		public PositionViewModel(Position position)
		{
			Position = position;
		}

		public void Update(Position position)
		{
			Position = position;
		}
	}
}