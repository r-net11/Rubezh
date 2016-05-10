using System.Collections.ObjectModel;
using System.Linq;
using StrazhAPI;
using StrazhAPI.GK;
using StrazhAPI.SKD;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace StrazhModule.ViewModels
{
	public class StateViewModel : BaseViewModel
	{
		public StateViewModel(SKDLibraryState skdLibraryState, SKDDriver driver)
		{
			AddFrameCommand = new RelayCommand(OnAddFrame);
			RemoveFrameCommand = new RelayCommand(OnRemoveFrame, CanRemoveFrame);

			State = skdLibraryState;
			Driver = driver;

			Frames = new ObservableCollection<FrameViewModel>(
				State.Frames.Select(frame => new FrameViewModel(frame))
			);
			SelectedFrame = Frames.FirstOrDefault();
		}

		public SKDLibraryState State { get; private set; }
		public SKDDriver Driver { get; private set; }

		public string Name
		{
			get
			{
				if (State.StateClass == XStateClass.No)
				{
					return "Базовый рисунок";
				}
				if (State.StateClass == XStateClass.On)
				{
					return "Открыто";
				}
				if (State.StateClass == XStateClass.Off)
				{
					return "Закрыто";
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
					ServiceFactory.SaveService.SKDLibraryChanged = true;
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
			var skdLibraryFrame = new SKDLibraryFrame()
			{
				Id = Frames.Count
			};
			State.Frames.Add(skdLibraryFrame);
			Frames.Add(new FrameViewModel(skdLibraryFrame));
			SelectedFrame = Frames.LastOrDefault();
			ServiceFactory.SaveService.SKDLibraryChanged = true;
		}

		public RelayCommand RemoveFrameCommand { get; private set; }
		void OnRemoveFrame()
		{
			State.Frames.Remove(SelectedFrame.Frame);
			Frames.Remove(SelectedFrame);
			SelectedFrame = Frames.FirstOrDefault();
			ServiceFactory.SaveService.SKDLibraryChanged = true;
		}
		bool CanRemoveFrame()
		{
			return SelectedFrame != null && Frames.Count > 1;
		}
	}
}