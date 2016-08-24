using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows.Views;
using Localization.SKD.Common;

namespace SKDModule.ViewModels
{
	public class ForceClosingQuestionDialogWindowViewModel : SaveCancelDialogViewModel
	{
		public ForceClosingQuestionDialogWindowViewModel()
		{
			Title = CommonResources.ForcedClose;
		}
	}
}
