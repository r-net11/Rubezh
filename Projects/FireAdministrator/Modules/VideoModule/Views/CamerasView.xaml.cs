using System.Windows.Controls;

namespace VideoModule.Views
{
	public partial class CamerasView : UserControl
	{
		public static CamerasView Current { get; private set; }
		public CamerasView()
		{
			InitializeComponent();
			Current = this;
		}
	}
}