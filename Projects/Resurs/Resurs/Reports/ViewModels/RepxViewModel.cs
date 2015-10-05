using DevExpress.XtraReports.UI;
using Infrastructure.Common.Windows.ViewModels;

namespace Resurs.ViewModels
{
	public class RepxViewModel : BaseViewModel
	{
		public RepxViewModel(XtraReport model, string name)
		{
			Model = model;
			Name = name;
		}
		private string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}
		public XtraReport Model { get; private set; }
	}
}