using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;
using System.ComponentModel;
using System.AddIn.Contract;
using System.Windows;
using System.AddIn.Pipeline;
using System.Windows.Shapes;
using System.Windows.Media;

namespace FireMonitor.Multiclient.ViewModels
{
	[Serializable]
	public class HostViewModel : ViewPartViewModel
	{
		private AppDomain _domain;
		public int Index { get; private set; }
		public MulticlientControllerWrapper Controller { get; private set; }
		public FrameworkElement Content { get; private set; }

		public HostViewModel(int index)
		{
			App.Current.Exit += (s, e) => Controller.ShutDown();
			Index = index;
			_domain = AppDomain.CreateDomain("Instance" + index.ToString());
			Type type = typeof(MulticlientController);
			var controller = (MulticlientController)_domain.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName);
			Controller = new MulticlientControllerWrapper(controller);
			Controller.ControlChanged += ControlChanged;
			Controller.Start();
		}
		private void ControlChanged(INativeHandleContract contract)
		{
			ApplicationService.Invoke(() =>
				{
					Content = FrameworkElementAdapters.ContractToViewAdapter(contract);
					OnPropertyChanged(() => Content);
				});
		}
	}
}
