using Infrastructure.Client.Layout.ViewModels;
using Infrastructure.Common.Services.Layout;
using RubezhAPI.Models.Layouts;
using RubezhClient;
using System.Collections.Generic;
using System.Linq;

namespace AutomationModule.Layout.ViewModels
{
	public class LayoutPartProcedureViewModel : LayoutPartTitleViewModel
	{
		private LayoutPartProcedureProperties _properties;

		public LayoutPartProcedureViewModel(LayoutPartProcedureProperties properties)
		{
			_properties = properties ?? new LayoutPartProcedureProperties();
			UpdateLayoutPart();
		}

		public string ProcedureTitle { get; private set; }

		public void UpdateLayoutPart()
		{
			var procedureName = ClientManager.SystemConfiguration.AutomationConfiguration.Procedures.Where(item => item.Uid == _properties.ReferenceUID).Select(item => item.Name).FirstOrDefault();
			ProcedureTitle = procedureName == null ? "" : string.Concat(" (", procedureName, ")");
			OnPropertyChanged(() => ProcedureTitle);
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