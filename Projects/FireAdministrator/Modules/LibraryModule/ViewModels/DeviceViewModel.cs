using System.Linq;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.TreeList;
using System.Collections.Generic;

namespace LibraryModule.ViewModels
{
	public class DeviceViewModel : TreeNodeViewModel<DeviceViewModel>
	{
		public LibraryDevice LibraryDevice { get; private set; }
		public LibraryDevicePresenter Presenter { get; private set; }
		public Driver Driver
		{
			get { return LibraryDevice.Driver; }
		}
		public string Title
		{
			get { return Presenter == null ? LibraryDevice.PresentationName : Presenter.Key; }
		}
		public List<LibraryState> States
		{
			get { return Presenter == null ? LibraryDevice.States : Presenter.States; }
		}

		public DeviceViewModel(LibraryDevice libraryDevice, LibraryDevicePresenter presenter = null)
		{
			Presenter = presenter;
			LibraryDevice = libraryDevice;
			AddChildren();
		}

		private void AddChildren()
		{
			if (Presenter == null && LibraryDevice.Presenters != null)
				foreach (var presenter in LibraryDevice.Presenters)
					AddChild(new DeviceViewModel(LibraryDevice, presenter));
		}

		public string AlternativeName
		{
			get { return LibraryDevice.AlternativeName; }
			set
			{
				LibraryDevice.AlternativeName = value;
				//OnPropertyChanged("AlternativeName");
				OnPropertyChanged("LibraryDevice");
				ServiceFactory.SaveService.LibraryChanged = true;
			}
		}
	}
}