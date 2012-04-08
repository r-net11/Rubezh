using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using FiresecClient;
using Infrastructure;

namespace SkudModule.ViewModels
{
	public class SkudViewModel : RegionViewModel
	{
		public SkudViewModel()
		{
		}

		public void Initialize()
		{

		}

		public override void OnShow()
		{
		}

		public override void OnHide()
		{
			ServiceFactory.Layout.ShowMenu(null);
		}
	}
}