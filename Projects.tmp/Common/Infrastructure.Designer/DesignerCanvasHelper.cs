using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Controls.Converters;

namespace Infrastructure.Designer
{
	public static class DesignerCanvasHelper
	{
		public static MenuItem BuildMenuItem(string header, string imageSourceUri, ICommand command)
		{
			var menuItem = new MenuItem();
			var grid = new Grid();
			grid.Width = 16;
			grid.Height = 16;
			grid.Background = ConverterHelper.GetResource(imageSourceUri) as Brush;
			menuItem.Icon = grid;
			menuItem.Header = header;
			menuItem.Command = command;
			return menuItem;
		}
	}
}