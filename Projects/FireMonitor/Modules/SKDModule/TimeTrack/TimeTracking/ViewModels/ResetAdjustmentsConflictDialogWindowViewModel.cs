using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class ResetAdjustmentsConflictDialogWindowViewModel : SaveCancelDialogViewModel
	{
		public ResetAdjustmentsConflictDialogWindowViewModel()
		{
			SaveCaption = "Установить границы без пересечений с другими интервалами";
			CancelCaption = "Удалить корректировки пересекающихся интервалов";
		}

		private string _saveCaption;
		public override string SaveCaption
		{
			get { return _saveCaption; }
			set
			{
				if (string.Equals(_saveCaption, value)) return;
				_saveCaption = value;
				OnPropertyChanged(() => SaveCaption);
			}
		}

		private string _cancelCaption;
		public override string CancelCaption
		{
			get { return _cancelCaption; }
			set
			{
				if (string.Equals(_cancelCaption, value)) return;
				_cancelCaption = value;
				OnPropertyChanged(() => CancelCaption);
			}
		}

		protected override bool Save()
		{
			return base.Save();
		}

		protected override bool Cancel()
		{
			return base.Cancel();
		}
	}
}
