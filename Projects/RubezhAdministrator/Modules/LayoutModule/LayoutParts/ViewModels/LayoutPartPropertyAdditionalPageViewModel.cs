using Infrastructure.Client.Layout.ViewModels;
using Infrastructure.Common.Services.Layout;
using RubezhAPI.Models.Layouts;

namespace LayoutModule.LayoutParts.ViewModels
{
	public class LayoutPartPropertyAdditionalPageViewModel : LayoutPartPropertyPageViewModel
	{
		LayoutPartTitleViewModel _layoutPartViewModel;
		bool _haveChanges;
		public LayoutPartPropertyAdditionalPageViewModel(LayoutPartTitleViewModel layoutPartViewModel)
		{
			_layoutPartViewModel = (LayoutPartWithAdditioanlPropertiesViewModel)layoutPartViewModel;
		}
		bool _isVisibleBottomPanel;
		public bool IsVisibleBottomPanel
		{
			get { return _isVisibleBottomPanel; }
			set
			{
				_isVisibleBottomPanel = value;
				OnPropertyChanged(() => IsVisibleBottomPanel);
				_haveChanges = true;
			}
		}
		public override string Header
		{
			get { return "Дополнительно"; }
		}

		public override void CopyProperties()
		{
			var properties = (LayoutPartAdditionalProperties)_layoutPartViewModel.Properties;
			if (properties != null)
			{
				IsVisibleBottomPanel = properties.IsVisibleBottomPanel;
			}
			_haveChanges = false;
		}

		public override bool CanSave()
		{
			return true;
		}

		public override bool Save()
		{
			var properties = (LayoutPartAdditionalProperties)_layoutPartViewModel.Properties;
			if(_haveChanges)
			{
				properties.IsVisibleBottomPanel = IsVisibleBottomPanel;
				return true;
			}
			return false;
		}
	}
}