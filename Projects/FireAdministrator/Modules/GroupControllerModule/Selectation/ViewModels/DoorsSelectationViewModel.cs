using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	[SaveSizeAttribute]
	public class DoorsSelectationViewModel : SaveCancelDialogViewModel
	{
		public RelayCommand<object> AddCommand { get; private set; }
		public IList SelectedSourceDoors;

		public RelayCommand<object> RemoveCommand { get; private set; }
		public IList SelectedTargetDoors;

		protected override bool Save()
		{
			return base.Save();
		}
	}
}