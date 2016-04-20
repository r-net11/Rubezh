using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI;
using RubezhAPI.GK;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class StateViewModel : BaseViewModel
	{
		public StateViewModel(GKLibraryState libraryState, GKDriver driver)
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

		public GKLibraryState State { get; private set; }
		public GKDriver Driver { get; private set; }

		public string Name
		{
			get
			{
				if (State.StateClass == XStateClass.Fire1)
				{
					return "Сработка 1";
				}
				if (State.StateClass == XStateClass.Fire2)
				{
					return "Сработка 2";
				}
				if (State.StateClass == XStateClass.No)
				{
					return "Базовый рисунок";
				}
				return State.StateClass.ToDescription();
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
					ServiceFactory.SaveService.GKLibraryChanged = true;
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
			var libraryFrame = new GKLibraryFrame()
			{
				Id = Frames.Count
			};
			State.Frames.Add(libraryFrame);
			Frames.Add(new FrameViewModel(libraryFrame));
			SelectedFrame = Frames.LastOrDefault();
			ServiceFactory.SaveService.GKLibraryChanged = true;
		}

		public RelayCommand RemoveFrameCommand { get; private set; }
		void OnRemoveFrame()
		{
			State.Frames.Remove(SelectedFrame.Frame);
			Frames.Remove(SelectedFrame);
			SelectedFrame = Frames.FirstOrDefault();
			ServiceFactory.SaveService.GKLibraryChanged = true;
		}
		bool CanRemoveFrame()
		{
			return SelectedFrame != null && Frames.Count > 1;
		}
	}
}