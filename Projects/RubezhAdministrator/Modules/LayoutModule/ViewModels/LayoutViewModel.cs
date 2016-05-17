using RubezhAPI.Models.Layouts;
using Infrastructure.Common.Windows.ViewModels;

namespace LayoutModule.ViewModels
{
	public class LayoutViewModel : BaseViewModel
	{
		public Layout Layout { get; private set; }

		public LayoutViewModel(Layout layout)
		{
			Layout = layout;
		}

		public void Update()
		{
			OnPropertyChanged(() => Caption);
			OnPropertyChanged(() => Description);
		}

		public string Caption
		{
			get { return Layout.Caption; }
		}
		public string Description
		{
			get { return Layout.Description; }
		}

		public override string ToString()
		{
			return base.ToString() + " [" + Caption + "]";
		}
	}
}