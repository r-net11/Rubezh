using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class ArchivePimViewModel : CheckBoxItemViewModel
	{
		public ArchivePimViewModel(XPim pim)
		{
			Pim = pim;
			Name = pim.PresentationName;
		}

		public XPim Pim { get; private set; }
		public string Name { get; private set; }
	}
}