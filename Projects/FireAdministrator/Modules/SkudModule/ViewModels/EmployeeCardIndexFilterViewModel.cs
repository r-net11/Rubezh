using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using FiresecClient;
using Infrastructure;
using FiresecAPI.Models.Skud;
using Controls.MessageBox;
using System.Windows;
using SkudModule.Properties;

namespace SkudModule.ViewModels
{
	public class EmployeeCardIndexFilterViewModel : SaveCancelDialogContent
	{
		public EmployeeCardIndexFilter Filter { get; private set; }

		public EmployeeCardIndexFilterViewModel(EmployeeCardIndexFilter filter)
		{
			Title = Resources.EmployeeCardIndexFilterTitle;
			Filter = filter;
		}

		public void Update()
		{
			OnPropertyChanged("EmployeeCardIndexFilter");
		}
		
		protected override void Save(ref bool cancel)
		{
			base.Save(ref cancel);
			if (cancel)
				return;
			//
			Update();
		}
	}
}