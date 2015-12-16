using Infrastructure.Client.Layout.ViewModels;
using Infrastructure.Common.Services.Layout;
using RubezhAPI.Models.Layouts;
using System.Collections.Generic;
namespace AutomationModule.Layout.ViewModels
{
	public class LayoutPartProcedureViewModel : LayoutPartTitleViewModel
	{
		private LayoutPartProcedureProperties _properties;

		public LayoutPartProcedureViewModel(LayoutPartProcedureProperties properties)
		{
			_properties = properties ?? new LayoutPartProcedureProperties();
		}

		public override ILayoutProperties Properties
		{
			get { return _properties; }
		}
		public override IEnumerable<LayoutPartPropertyPageViewModel> PropertyPages
		{
			get
			{
				yield return new LayoutPartPropertyProcedurePageViewModel(this);
				yield return new LayoutPartPropertyProcedurePageStyleViewModel(this);
			}
		}
	}
}