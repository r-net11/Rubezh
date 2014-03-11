using System.Collections.Generic;
using System.Windows.Media;
using FiresecAPI.Models.Layouts;
using Infrastructure.Common.Services.Layout;
using System;
using Infrastructure.Common.Services;
using Common;
using Infrastructure.Common.Windows;
using Infrastructure.Client.Layout.ViewModels;

namespace LayoutModule.ViewModels
{
	public class LayoutPartEmptyViewModel : LayoutPartTitleViewModel
	{
		public LayoutPartEmptyViewModel()
		{
			Title = "Заглушка";
			IconSource = LayoutPartDescription.IconPath + "BExit.png";
		}
	}
}
