﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.TreeList;
using FiresecAPI.SKD;

namespace SKDModule.Reports.ViewModels
{
	public class TreeNodeItemViewModel : TreeNodeViewModel<TreeNodeItemViewModel>
	{
		public TreeNodeItemViewModel(SKDIsDeletedModel item, bool canCheck)
		{
			Item = item;
			CanCheck = canCheck;
		}

		public SKDIsDeletedModel Item { get; private set; }
		public bool CanCheck { get; private set; }
		public bool IsDeleted
		{
			get { return Item.IsDeleted; }
		}
		private bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged(() => IsChecked);
			}
		}
	}
}