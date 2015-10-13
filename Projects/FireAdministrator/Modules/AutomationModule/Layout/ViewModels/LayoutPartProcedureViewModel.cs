using System;
using System.Linq;
using System.Collections.Generic;
using Infrastructure.Client.Layout.ViewModels;
using RubezhAPI.Models.Layouts;
using Infrastructure.Common.Services.Layout;
using RubezhClient;
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

		public string ProcedureTitle { get; private set; }

		public void UpdateLayoutPart()
		{
			ProcedureTitle = ClientManager.SystemConfiguration.AutomationConfiguration.Procedures.Where(item => item.Uid == _properties.ReferenceUID).Select(item => item.Name).FirstOrDefault() ?? "Процедура";
			OnPropertyChanged(() => ProcedureTitle);
		}
	}
}