using Infrastructure.Common;
using Infrastructure.Common.Services.Layout;

namespace LayoutModule.ViewModels
{
	public class LayoutPartDescriptionViewModel : LayoutPartDescriptionGroupViewModel
	{
		public ILayoutPartDescription LayoutPartDescription { get; private set; }
		public LayoutPartDescriptionViewModel(ILayoutPartDescription layoutPartDescription)
		{
			LayoutPartDescription = layoutPartDescription;
			VisualizationState = VisualizationState.Prohibit;
			AddCommand = new RelayCommand(OnAddCommand, CanAddCommand);
			DragCommand = new RelayCommand(OnDragCommand, CanAddCommand);
		}

		public string Name
		{
			get { return LayoutPartDescription.Name; }
		}
		public string Description
		{
			get { return LayoutPartDescription.Description; }
		}
		public string IconSource
		{
			get { return LayoutPartDescription.IconSource; }
		}
		public VisualizationState VisualizationState { get; private set; }
		private int _count;
		public int Count
		{
			get { return _count; }
			set
			{
				_count = value;
				OnPropertyChanged(() => Count);
				switch (Count)
				{
					case -1:
						VisualizationState = VisualizationState.Prohibit;
						break;
					case 0:
						VisualizationState = VisualizationState.NotPresent;
						break;
					default:
						VisualizationState = LayoutPartDescription.AllowMultiple ? VisualizationState.Multiple : VisualizationState.Single;
						break;
				}
				OnPropertyChanged(() => VisualizationState);
				OnPropertyChanged(() => IsPresented);
			}
		}

		public bool IsPresented
		{
			get { return VisualizationState != VisualizationState.NotPresent; }
		}

		public RelayCommand AddCommand { get; private set; }
		private void OnAddCommand()
		{
			LayoutDesignerViewModel.Instance.AddLayoutPart(this, false);
		}
		public RelayCommand DragCommand { get; private set; }
		private void OnDragCommand()
		{
			LayoutDesignerViewModel.Instance.AddLayoutPart(this, true);
		}
		private bool CanAddCommand()
		{
			return VisualizationState != VisualizationState.Prohibit && (VisualizationState == VisualizationState.NotPresent || LayoutPartDescription.AllowMultiple);
		}
	}
}