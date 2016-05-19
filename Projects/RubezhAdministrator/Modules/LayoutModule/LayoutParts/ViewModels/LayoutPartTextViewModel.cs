using Infrastructure.Client.Layout.ViewModels;
using Infrastructure.Common.Services.Layout;
using RubezhAPI.Models.Layouts;
using System.Collections.Generic;

namespace LayoutModule.LayoutParts.ViewModels
{
	public class LayoutPartTextViewModel : LayoutPartTitleViewModel
	{
		LayoutPartTextProperties _properties;
		bool _isEditable;

		public LayoutPartTextViewModel(LayoutPartTextProperties properties, bool isEditable)
		{
			_properties = properties ?? new LayoutPartTextProperties();
			_isEditable = isEditable;
		}
		public override ILayoutProperties Properties
		{
			get { return _properties; }
		}
		public override IEnumerable<LayoutPartPropertyPageViewModel> PropertyPages
		{
			get
			{
				yield return new LayoutPartPropertyTextPageViewModel(this, _isEditable);
			}
		}
	}
}
