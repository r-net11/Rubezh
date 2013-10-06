using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Models.Skud;

namespace SkudModule.ViewModels
{
	public class PassCardTemplateViewModel : BaseViewModel
	{
		public PassCardTemplate PassCardTemplate { get; private set; }

		public PassCardTemplateViewModel(PassCardTemplate passCardTemplate)
		{
			PassCardTemplate = passCardTemplate;
		}

		public void Update()
		{
			OnPropertyChanged(() => Caption);
			OnPropertyChanged(() => Description);
		}

		public string Caption
		{
			get { return PassCardTemplate.Caption; }
		}
		public string Description
		{
			get { return PassCardTemplate.Description; }
		}
	}
}
