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
