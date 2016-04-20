using Infrastructure.Common.Windows.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Common.Windows.Services
{
	public interface IDialogService
	{
		bool ShowModalWindow(WindowBaseViewModel windowBaseViewModel);
		void ShowWindow(WindowBaseViewModel windowBaseViewModel);
	}
}