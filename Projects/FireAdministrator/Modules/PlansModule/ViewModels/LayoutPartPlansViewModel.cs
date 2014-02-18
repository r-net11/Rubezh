using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Services.Layout;
using FiresecAPI.Models.Layouts;
using Infrastructure.Client.Layout.ViewModels;

namespace PlansModule.ViewModels
{
	public class LayoutPartPlansViewModel : LayoutPartTitleViewModel
	{
		private LayoutPartPlansProperties _properties;
		private LayoutPartPropertyPageViewModel _plansPage;

		public LayoutPartPlansViewModel(LayoutPartPlansProperties properties)
		{
			Title = "Планы";
			IconSource = LayoutPartDescription.IconPath + "CMap.png";
			_plansTitle = null;
			_properties = properties ?? new LayoutPartPlansProperties();
			_plansPage = new LayoutPartPropertyPlansPageViewModel(this);
		}

		public override ILayoutProperties Properties
		{
			get { return _properties; }
		}
		public override IEnumerable<LayoutPartPropertyPageViewModel> PropertyPages
		{
			get
			{
				yield return _plansPage;
			}
		}

		private string _plansTitle;
		public string PlansTitle
		{
			get { return _plansTitle; }
			set
			{
				_plansTitle = value;
				OnPropertyChanged(() => PlansTitle);
			}
		}
	}
}