using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;

namespace FireMonitor.ViewModels
{
	public class MulticlientControlViewModel : BaseViewModel
	{
		public MulticlientControlViewModel()
		{
			Multiclients = new List<MulticlientViewModel>();
			foreach (var multiclientData in MulticlientHelper.MulticlientDatas)
			{
				var multiclientViewModel = new MulticlientViewModel(multiclientData);
				Multiclients.Add(multiclientViewModel);
			}
		}

		public List<MulticlientViewModel> Multiclients { get; private set; }
	}
}