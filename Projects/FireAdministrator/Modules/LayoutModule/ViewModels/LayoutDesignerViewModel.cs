using System;
using System.Collections.ObjectModel;
using FiresecAPI.Models.Layouts;
using Infrastructure.Common.Windows.ViewModels;

namespace LayoutModule.ViewModels
{
	public class LayoutDesignerViewModel : BaseViewModel
	{
		private Layout _layout;
		public LayoutDesignerViewModel()
		{
			Update();
			LayoutParts = new ObservableCollection<LayoutPartViewModel>();
			for (int i = 0; i < 10; i++)
				LayoutParts.Add(new LayoutPartViewModel() { Title = Guid.NewGuid().ToString() });
		}

		private ObservableCollection<LayoutPartViewModel> _layoutParts;
		public ObservableCollection<LayoutPartViewModel> LayoutParts
		{
			get { return _layoutParts; }
			set
			{
				_layoutParts = value;
				OnPropertyChanged(() => LayoutParts);
			}
		}

		private LayoutPartViewModel _activeLayoutPart;
		public LayoutPartViewModel ActiveLayoutPart
		{
			get { return _activeLayoutPart; }
			set
			{
				_activeLayoutPart = value;
				OnPropertyChanged(() => ActiveLayoutPart);
			}
		}

		public void Update(Layout layout)
		{
			_layout = layout;
			//foreach (var layoutPart in LayoutParts)
			//    layoutPart.IsPresented = _layout != null && _layout.Parts.Contains(layoutPart.LayoutPartDescription.UID);
			//SelectedLayoutPart = LayoutParts.FirstOrDefault();
		}
		public void Update()
		{

		}
	}
}