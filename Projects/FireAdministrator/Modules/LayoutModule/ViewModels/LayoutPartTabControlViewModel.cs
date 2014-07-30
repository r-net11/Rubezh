using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using FiresecAPI.Models.Layouts;
using Infrastructure.Common.Services.Layout;

namespace LayoutModule.ViewModels
{
	public class LayoutPartTabControlViewModel : BaseLayoutPartViewModel
	{
		public LayoutPartTabControlViewModel()
		{
			LayoutParts1 = new ObservableCollection<LayoutPartViewModel>();
			LayoutParts2 = new ObservableCollection<LayoutPartViewModel>();
		}

		private ObservableCollection<LayoutPartViewModel> _layoutParts1;
		public ObservableCollection<LayoutPartViewModel> LayoutParts1
		{
			get { return _layoutParts1; }
			set
			{
				if (_layoutParts1 != null)
					_layoutParts1.CollectionChanged -= LayoutPartsChanged;
				_layoutParts1 = value;
				_layoutParts1.CollectionChanged += LayoutPartsChanged;
				OnPropertyChanged(() => LayoutParts1);
			}
		}
		private ObservableCollection<LayoutPartViewModel> _layoutParts2;
		public ObservableCollection<LayoutPartViewModel> LayoutParts2
		{
			get { return _layoutParts2; }
			set
			{
				if (_layoutParts2 != null)
					_layoutParts2.CollectionChanged -= LayoutPartsChanged;
				_layoutParts2 = value;
				_layoutParts2.CollectionChanged += LayoutPartsChanged;
				OnPropertyChanged(() => LayoutParts2);
			}
		}

		private LayoutPartViewModel _activeLayoutPart1;
		public LayoutPartViewModel ActiveLayoutPart1
		{
			get { return _activeLayoutPart1; }
			set
			{
				_activeLayoutPart1 = value;
				OnPropertyChanged(() => ActiveLayoutPart1);
			}
		}
		private LayoutPartViewModel _activeLayoutPart2;
		public LayoutPartViewModel ActiveLayoutPart2
		{
			get { return _activeLayoutPart2; }
			set
			{
				_activeLayoutPart2 = value;
				OnPropertyChanged(() => ActiveLayoutPart2);
			}
		}

		private void LayoutPartsChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			//switch (e.Action)
			//{
			//    case NotifyCollectionChangedAction.Add:
			//        foreach (LayoutPartViewModel layoutPartViewModel in e.NewItems)
			//        {
			//            _layout.Parts.Add(layoutPartViewModel.LayoutPart);
			//            layoutPartViewModel.LayoutPartDescriptionViewModel.Count++;
			//        }
			//        break;
			//    case NotifyCollectionChangedAction.Remove:
			//        foreach (LayoutPartViewModel layoutPartViewModel in e.OldItems)
			//        {
			//            _layout.Parts.Remove(layoutPartViewModel.LayoutPart);
			//            layoutPartViewModel.LayoutPartDescriptionViewModel.Count--;
			//        }
			//        break;
			//}
		}

		public override ILayoutProperties Properties
		{
			get { return null; }
		}
		public override IEnumerable<LayoutPartPropertyPageViewModel> PropertyPages
		{
			get { return Enumerable.Empty<LayoutPartPropertyPageViewModel>(); }
		}
	}
}