using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class ArchiveDirectionViewModel : CheckBoxItemViewModel
	{
		public ArchiveDirectionViewModel(XDirection direction)
		{
			Direction = direction;
			Name = direction.PresentationName;
		}

		public XDirection Direction { get; private set; }
		public string Name { get; private set; }
	}
}