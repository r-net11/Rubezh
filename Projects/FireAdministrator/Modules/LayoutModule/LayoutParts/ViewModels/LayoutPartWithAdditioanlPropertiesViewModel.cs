using Infrastructure.Client.Layout.ViewModels;
using Infrastructure.Common.Windows.Services.Layout;
using RubezhAPI.Models.Layouts;
using System.Collections.Generic;

namespace LayoutModule.LayoutParts.ViewModels
{
	public class LayoutPartWithAdditioanlPropertiesViewModel : LayoutPartTitleViewModel
	{
		LayoutPartAdditionalProperties _properties;
		public LayoutPartWithAdditioanlPropertiesViewModel(LayoutPartAdditionalProperties properties)
		{
			_properties = properties ?? new LayoutPartAdditionalProperties();
		}
		public override ILayoutProperties Properties
		{
			get { return _properties; }
		}
		public override IEnumerable<LayoutPartPropertyPageViewModel> PropertyPages
		{
			get
			{
				yield return new LayoutPartPropertyAdditionalPageViewModel(this);
			}
		}
	}
}