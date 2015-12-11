using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using System;
using LayoutModel = RubezhAPI.Models.Layouts.Layout;

namespace AutomationModule.ViewModels
{
	public class LayoutViewModel : BaseViewModel
	{
		public LayoutModel Layout { get; private set; }

		public LayoutViewModel(LayoutModel layout)
		{
			Layout = layout;
			NavigateCommand = new RelayCommand(OnNavigate, CanNavigate);
		}

		public string Name
		{
			get { return Layout.Caption; }
		}

		public RelayCommand NavigateCommand { get; private set; }
		void OnNavigate()
		{
			ServiceFactoryBase.Events.GetEvent<ShowMonitorLayoutEvent>().Publish(Layout.UID);
		}
		bool CanNavigate()
		{
			return Layout.UID != Guid.Empty;
		}
	}
}