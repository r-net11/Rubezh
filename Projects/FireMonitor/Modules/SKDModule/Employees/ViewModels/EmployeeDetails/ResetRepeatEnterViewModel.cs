using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class ResetRepeatEnterViewModel : SaveCancelDialogViewModel
	{
		private bool _isForAllAccessPoints;

		public bool IsForAllAccessPoints
		{
			get { return _isForAllAccessPoints; }
			set
			{
				if (_isForAllAccessPoints == value) return;
				_isForAllAccessPoints = value;
				OnPropertyChanged(() => IsForAllAccessPoints);
			}
		}

		public ResetRepeatEnterViewModel()
		{
			IsForAllAccessPoints = true;
			Title = "Сброс ограничения на повторный проход";
		}
	}
}
