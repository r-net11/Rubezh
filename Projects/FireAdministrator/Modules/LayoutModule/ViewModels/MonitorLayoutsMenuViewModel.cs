using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using Common;
using FiresecAPI.Models.Layouts;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;

namespace LayoutModule.ViewModels
{
	public class MonitorLayoutsMenuViewModel : BaseViewModel
	{
		public MonitorLayoutsMenuViewModel(MonitorLayoutsViewModel context)
		{
			Context = context;
		}

		public MonitorLayoutsViewModel Context { get; private set; }
	}
}