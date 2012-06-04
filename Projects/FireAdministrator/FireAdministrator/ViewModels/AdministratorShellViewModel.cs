using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;

namespace FireAdministrator.ViewModels
{
	public class AdministratorShellViewModel : ShellViewModel
	{
		public AdministratorShellViewModel()
		{
			Title = "Администратор ОПС Firesec-2";
			Height = 700;
			Width = 1100;
			MinWidth = 800;
			MinHeight = 550;
			Toolbar = new MenuViewModel();
		}
	}
}
