﻿using System.Collections.Generic;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Models.Layouts;
using Infrastructure.Client.Startup;
using Infrastructure.Common.Windows;

namespace FireMonitor.Layout.ViewModels
{
	public class SelectLayoutViewModel : SaveCancelDialogViewModel
	{
		public SelectLayoutViewModel(List<FiresecAPI.Models.Layouts.Layout> layouts)
		{
			Layouts = layouts;
			TopMost = true;
			Sizable = false;
			Title = "Выберите макет";
			SaveCaption = "Выбрать";
			CancelCaption = "Выйти";
		}

		public List<FiresecAPI.Models.Layouts.Layout> Layouts { get; private set; }

		FiresecAPI.Models.Layouts.Layout _selectedLayout;
		public FiresecAPI.Models.Layouts.Layout SelectedLayout
		{
			get { return _selectedLayout; }
			set
			{
				_selectedLayout = value;
				OnPropertyChanged(() => SelectedLayout);
			}
		}

		public override void OnLoad()
		{
			base.OnLoad();
			Surface.ShowInTaskbar = false;
		}

		protected override bool Save()
		{
			return true;
		}
		protected override bool Cancel()
		{
			SelectedLayout = null;
			return base.Cancel();
		}
		protected override bool CanSave()
		{
			return SelectedLayout != null;
		}
	}
}