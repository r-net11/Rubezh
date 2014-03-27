using System.Windows;
using System.Windows.Controls;
using VideoModule.ViewModels;

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