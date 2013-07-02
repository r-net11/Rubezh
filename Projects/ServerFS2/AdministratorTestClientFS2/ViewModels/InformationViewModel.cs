using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace AdministratorTestClientFS2.ViewModels
{
	public class InformationViewModel : DialogViewModel
	{
		public InformationViewModel(string info)
		{
			Title = "Информация об устройстве";
			information = info;
		}

		string information = "";
		public string Information
		{
			get { return information; }
			set
			{
				information = value;
				OnPropertyChanged("Information");
			}
		}
	}
}
