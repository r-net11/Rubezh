using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecService.ViewModels
{
	public class GKViewModel : BaseViewModel
	{
		public GKViewModel()
		{
			Name = "";
		}

		public string Name { get; private set; }

		string _state;
		public string State
		{
			get { return _state; }
			set
			{
				_state = value;
				OnPropertyChanged(() => State);
			}
		}
	}
}