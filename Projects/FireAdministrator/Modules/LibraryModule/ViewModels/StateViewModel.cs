using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace LibraryModule.ViewModels
{
	public class StateViewModel : BaseViewModel
	{
		public StateViewModel(LibraryState libraryState, Driver driver)
		{
			AddFrameCommand = new RelayCommand(OnAddFrame);
			RemoveFrameCommand = new RelayCommand(OnRemoveFrame, CanRemoveFrame);

			State = libraryState;
			Driver = driver;

			Frames = new ObservableCollection<FrameViewModel>(
				State.Frames.Select(frame => new FrameViewModel(frame))
			);
			SelectedFrame = Frames.FirstOrDefault();
		}

		public LibraryState State { get; private set; }
		public Driver Driver { get; private set; }

		public bool IsAdditional
		{
			get { return State.Code != null; }
		}

		public string ClassName
		{
			get { return State.StateType.ToDescription(); }
		}

		public string AdditionalName
		{
			get
			{
				var driverState = Driver.States.FirstOrDefault(x => x.Code != null && x.Code == State.Code);
				if (driverState != null)
				{
					return IsAdditional ? driverState.Name : null;
				}
				return null;
			}
		}

		public bool IsLayerEditingVisible
		{
			get { return GlobalSettingsHelper.GlobalSettings.IsDebug; }
		}

		public int Layer
		{
			get { return State.Layer; }
			set
			{
				if (value != State.Layer)
				{
					State.Layer = value;
					ServiceFactory.SaveService.LibraryChanged = true;
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
				OnPropertyChanged(() => SelectedFrame);
			}
		}

		public RelayCommand AddFrameCommand { get; private set; }
		void OnAddFrame()
		{
			var libraryFrame = new LibraryFrame()
			{
				Id = Frames.Count
			};
			State.Frames.Add(libraryFrame);
			Frames.Add(new FrameViewModel(libraryFrame));
			SelectedFrame = Frames.LastOrDefault();
			ServiceFactory.SaveService.LibraryChanged = true;
		}

		public RelayCommand RemoveFrameCommand { get; private set; }
		void OnRemoveFrame()
		{
			State.Frames.Remove(SelectedFrame.Frame);
			Frames.Remove(SelectedFrame);
			SelectedFrame = Frames.FirstOrDefault();
			ServiceFactory.SaveService.LibraryChanged = true;
		}
		bool CanRemoveFrame()
		{
			return SelectedFrame != null && Frames.Count > 1;
		}
	}
}