using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;

namespace JournalModule.ViewModels
{
	public class ConfirmationViewModel : DialogViewModel
	{
		public ConfirmationViewModel()
		{
			Title = "Подтверждение критических событий";
		}
	}
}