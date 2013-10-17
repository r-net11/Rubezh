using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;

namespace LayoutModule.ViewModels
{
	public class LayoutDesignerViewModel : BaseViewModel
	{
		public LayoutDesignerViewModel()
		{
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
	}
}
