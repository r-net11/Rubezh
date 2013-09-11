using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Client.Plans.ViewModels;

namespace GKModule.Plans.ViewModels
{
	public class XDeviceTooltipViewModel : BaseViewModel
	{
		public XDeviceTooltipViewModel()
		{
			TitleViewModel = new ImageTextTooltipViewModel();
			StateViewModel = new ImageTextTooltipViewModel();
		}

		public ImageTextTooltipViewModel TitleViewModel { get; private set; }
		public ImageTextTooltipViewModel StateViewModel { get; private set; }

		public void Update()
		{
			OnPropertyChanged(() => TitleViewModel);
			OnPropertyChanged(() => StateViewModel);
		}
	}
}
