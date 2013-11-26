using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;

namespace FireMonitor.Layout.ViewModels
{
	public class SelectLayoutViewModel : SaveCancelDialogViewModel
	{
		public SelectLayoutViewModel(List<FiresecAPI.Models.Layouts.Layout> layouts)
		{
			Layouts = layouts;
			TopMost = true;
			Sizable = false;
			Title = "Выберете макет";
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