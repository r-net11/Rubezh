using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using MuliclientAPI;
using Infrastructure.Common;
using Infrastructure.Common.Windows;

namespace FireMonitor
{
	public class MulticlientViewModel : BaseViewModel
	{
		public MulticlientData MulticlientData { get; private set; }

		public MulticlientViewModel(MulticlientData multiclientData)
		{
			MulticlientData = multiclientData;
			SelectCommand = new RelayCommand(OnSelect);
		}

		public RelayCommand SelectCommand { get; private set; }
		void OnSelect()
		{
            var windowSize = new WindowSize()
            {
                Left = ApplicationService.ApplicationWindow.Left,
                Top = ApplicationService.ApplicationWindow.Top,
                Width = ApplicationService.ApplicationWindow.Width,
                Height = ApplicationService.ApplicationWindow.Height
            };

			if (MulticlientHelper.MulticlientClientId != MulticlientData.Id)
			{
				ApplicationService.ApplicationWindow.Hide();
				MulticlientClient.Muliclient.Activate(MulticlientHelper.MulticlientClientId, MulticlientData.Id, windowSize);
			}
		}
	}
}