using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;

namespace DiagnosticsModule.ViewModels
{
	public class FS2ViewModel : BaseViewModel
	{
		public FS2ViewModel()
		{
			CreateConfigurationCommand = new RelayCommand(OnCreateConfiguration);
		}

		public RelayCommand CreateConfigurationCommand { get; private set; }
		void OnCreateConfiguration()
		{
			;
		}
	}
}