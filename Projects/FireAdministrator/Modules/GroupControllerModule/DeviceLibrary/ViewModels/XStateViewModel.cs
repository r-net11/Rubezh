using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class StateViewModel : BaseViewModel
	{
		public StateViewModel(LibraryXState libraryState, XDriver driver)
		{
			AddFrameCommand = new RelayCommand(OnAddFrame);
			RemoveFrameCommand = new RelayCommand(OnRemoveFrame, CanRemoveFrame);

			State = libraryState;
			Driver = driver;

			Frames = new ObservableCollection<FrameViewModel>(
				State.XFrames.Select(frame => new FrameViewModel(frame))
			);
			SelectedFrame = Frames.FirstOrDefault();
		}

		public LibraryXState State { get; private set; }
		public XDriver Driver { get; private set; }

		public string Name
		{
			get
			{
				if (Driver != null && Driver.DriverType == XDriverType.Valve)
				{
					switch (State.XStateClass)
					{
						case XStateClass.On:
							return "Открыто";

						case XStateClass.Off:
							return "Закрыто";

						case XStateClass.TurningOn:
							return "Открывается";

						case XStateClass.TurningOff:
							return "Закрывается";
					}
				}
				if (State.XStateClass == XStateClass.Fire1)
				{
					return "Сработка 1";
				}
				if (State.XStateClass == XStateClass.Fire2)
				{
					return "Сработка 2";
				}
				return State.XStateClass.ToDescription();
			}
		}

		public bool IsLayerEditingVisible
		{
			get { return GlobalSettingsHelper.GlobalSettings.IsDebug || GlobalSettingsHelper.GlobalSettings.Administrator_IsExpertMode; }
		}

		public int Layer
		{
			get { return State.Layer; }
			set
			{
				if (value != State.Layer)
				{
					State.Layer = value;
					ServiceFactory.SaveService.XLibraryChanged = true;
				}
			}
		}

		public ObservableCollection<FrameViewModel> Frames { get; private set; }

		FrameViewModel _selectedFrame;
		public FrameViewModel SelectedFrame
		{
			get { return _selectedFrame; }
			set
			{
				_selectedFrame = value;
				OnPropertyChanged("SelectedFrame");
			}
		}

		public RelayCommand AddFrameCommand { get; private set; }
		void OnAddFrame()
		{
			var libraryFrame = new LibraryXFrame()
			{
				Id = Frames.Count
			};
			State.XFrames.Add(libraryFrame);
			Frames.Add(new FrameViewModel(libraryFrame));
			SelectedFrame = Frames.LastOrDefault();
			ServiceFactory.SaveService.XLibraryChanged = true;
		}

		public RelayCommand RemoveFrameCommand { get; private set; }
		void OnRemoveFrame()
		{
			State.XFrames.Remove(SelectedFrame.Frame);
			Frames.Remove(SelectedFrame);
			SelectedFrame = Frames.FirstOrDefault();
			ServiceFactory.SaveService.XLibraryChanged = true;
		}
		bool CanRemoveFrame()
		{
			return SelectedFrame != null && Frames.Count > 1;
		}
	}
}