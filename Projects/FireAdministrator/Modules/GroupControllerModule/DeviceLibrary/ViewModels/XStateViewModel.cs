using System.Collections.ObjectModel;
using System.Linq;
using XFiresecAPI;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;

namespace GKModule.ViewModels
{
    public class XStateViewModel : BaseViewModel
    {
        public XStateViewModel(LibraryXState libraryXState, XDriver xdriver)
        {
            AddXFrameCommand = new RelayCommand(OnAddXFrame);
            RemoveXFrameCommand = new RelayCommand(OnRemoveXFrame, CanRemoveXFrame);

            XState = libraryXState;
            XDriver = xdriver;

            XFrames = new ObservableCollection<XFrameViewModel>(
                XState.XFrames.Select(xframe => new XFrameViewModel(xframe))
            );
            SelectedXFrame = XFrames.FirstOrDefault();
        }

        public LibraryXState XState { get; private set; }
        public XDriver XDriver { get; private set; }

        public string ClassName
        {
            get { return XState.XStateClass.ToDescription(); }
        }

        public string AdditionalName { get; set; }

        public bool IsLayerEditingVisible
        {
            get { return ServiceFactory.AppSettings.IsDebug || ServiceFactory.AppSettings.IsExpertMode; }
        }

        public int Layer
        {
            get { return XState.Layer; }
            set
            {
                if (value != XState.Layer)
                {
                    XState.Layer = value;
                    ServiceFactory.SaveService.XLibraryChanged = true;
                }
            }
        }

        public ObservableCollection<XFrameViewModel> XFrames { get; private set; }

        XFrameViewModel _selectedXFrame;
        public XFrameViewModel SelectedXFrame
        {
            get { return _selectedXFrame; }
            set
            {
                _selectedXFrame = value;
                OnPropertyChanged("SelectedXFrame");
            }
        }

        public RelayCommand AddXFrameCommand { get; private set; }
        void OnAddXFrame()
        {
            var libraryXFrame = new LibraryXFrame()
            {
                Id = XFrames.Count
            };
            XState.XFrames.Add(libraryXFrame);
            XFrames.Add(new XFrameViewModel(libraryXFrame));
            SelectedXFrame = XFrames.LastOrDefault();
            ServiceFactory.SaveService.XLibraryChanged = true;
        }

        public RelayCommand RemoveXFrameCommand { get; private set; }
        void OnRemoveXFrame()
        {
            XState.XFrames.Remove(SelectedXFrame.XFrame);
            XFrames.Remove(SelectedXFrame);
            SelectedXFrame = XFrames.FirstOrDefault();
            ServiceFactory.SaveService.XLibraryChanged = true;
        }
        bool CanRemoveXFrame()
        {
            return SelectedXFrame != null && XFrames.Count > 1;
        }
    }
}
