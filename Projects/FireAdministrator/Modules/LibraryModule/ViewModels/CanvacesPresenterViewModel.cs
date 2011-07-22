using System.Collections.Generic;
using System.Collections.ObjectModel;
using Infrastructure.Common;

namespace LibraryModule.ViewModels
{
    public class CanvasesPresenterViewModel : BaseViewModel
    {
        public CanvasesPresenterViewModel(StateViewModel parentState)
        {
            ParentState = parentState;
        }

        public StateViewModel ParentState { get; private set; }

        public CanvasesPresenter CanvasesPresenter
        {
            get
            {
                var canvasesPresenter = new CanvasesPresenter(ParentState);
                if (!ParentState.IsAdditional)
                {
                    foreach (var stateViewModel in GetCheckedAdditionalStates(ParentState.ParentDevice.States))
                    {
                        canvasesPresenter.AddCanvacesFrom(stateViewModel);
                    }
                }
                return canvasesPresenter;
            }
        }

        IEnumerable<StateViewModel> GetCheckedAdditionalStates(ObservableCollection<StateViewModel> stateViewModels)
        {
            foreach (var stateViewModel in stateViewModels)
            {
                if (stateViewModel.IsAdditional && stateViewModel.IsChecked && stateViewModel.Class == ParentState.Class)
                {
                    yield return stateViewModel;
                }
            }
        }
    }
}
