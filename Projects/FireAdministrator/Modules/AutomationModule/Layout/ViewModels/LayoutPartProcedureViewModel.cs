using System;
using System.Linq;
using System.Collections.Generic;
using Infrastructure.Client.Layout.ViewModels;
using FiresecAPI.Models.Layouts;
using Infrastructure.Common.Services.Layout;
using FiresecClient;
namespace AutomationModule.Layout.ViewModels
{
	public class LayoutPartProcedureViewModel : LayoutPartTitleViewModel
	{
		private LayoutPartReferenceProperties _properties;

		public LayoutPartProcedureViewModel(LayoutPartReferenceProperties properties)
		{
			_properties = properties ?? new LayoutPartReferenceProperties();
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
			ProcedureTitle = FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures.Where(item => item.Uid == _properties.ReferenceUID).Select(item => item.Name).FirstOrDefault() ?? "Процедура";
			OnPropertyChanged(() => ProcedureTitle);
		}
	}
}
