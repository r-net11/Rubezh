using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Client.Layout.ViewModels;
using RubezhAPI.Models.Layouts;
using Infrastructure.Common.Services.Layout;

namespace LayoutModule.LayoutParts.ViewModels
{
	public class LayoutPartTextViewModel : LayoutPartTitleViewModel
	{
		private LayoutPartTextProperties _properties;
		private bool _isEditable;

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
