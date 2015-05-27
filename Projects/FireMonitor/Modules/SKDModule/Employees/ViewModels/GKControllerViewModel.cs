﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.GK;

namespace SKDModule.ViewModels
{
	public class GKControllerViewModel : BaseViewModel
	{
		public GKControllerViewModel(GKDevice device)
		{
			Device = device;
		}

		public GKDevice Device { get; private set; }

		bool _isChecked;
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