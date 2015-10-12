using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.GK;

namespace GKImitator.ViewModels
{
	public class CodePropertiesViewModel : SaveCancelDialogViewModel
	{
		public GKCode Code { get; private set; }

		public CodePropertiesViewModel(GKCode code)
		{
			Title = "Параметры кода";
			Code = code;
			Password = Code.Password;
		}

		int _password;
		public int Password
		{
			get { return _password; }
			set
			{
				_password = value;
				OnPropertyChanged(() => Password);
			}
		}

		protected override bool Save()
		{
			Code.Password = Password;
			return base.Save();
		}
	}
}