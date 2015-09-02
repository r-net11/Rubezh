using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows.Views;

namespace SKDModule.ViewModels
{
	public class ForceClosingQuestionDialogWindowViewModel : SaveCancelDialogViewModel
	{
		public ForceClosingQuestionDialogWindowViewModel()
		{
			Title = "Принудительное закрытие интервала";
		}
	}
}
