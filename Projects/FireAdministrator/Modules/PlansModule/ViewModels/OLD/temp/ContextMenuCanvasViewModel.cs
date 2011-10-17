using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using Infrastructure.Common;
using PlansModule.Views;

namespace PlansModule.ViewModels
{
    public class ContextMenuCanvasViewModel : RegionViewModel
    {
        RoutedEventHandler MouseDownEventHandler;

        public ContextMenuCanvasViewModel(RoutedEventHandler mouseDownEventHandler)
        {
            MouseDownEventHandler = mouseDownEventHandler;
        }

        public ContextMenu GetElement(UIElement activeElement, object element)
        {
            if (activeElement is Polygon)
            {
                PlanCanvasView.ElementProperties = (activeElement as Polygon).Name;
                return CreateContextMenu(element, "Свойства зоны");
            }
            else if (activeElement is Rectangle)
            {
                PlanCanvasView.ElementProperties = (activeElement as Rectangle).Name;
                return CreateContextMenu(element, "Свойства прямоугольника");
            }
            else if (activeElement is TextBox)
            {
                PlanCanvasView.ElementProperties = (activeElement as TextBox).Name;
                return CreateContextMenu(element, "Свойства текста");
            }
            return null;
        }

        ContextMenu CreateContextMenu(object element, string name)
        {
            var menuItem = new MenuItem()
            {
                Tag = element,
                Header = name
            };
            menuItem.Click += MouseDownEventHandler;

            var contextMenu = new ContextMenu();
            contextMenu.Items.Add(menuItem);
            return contextMenu;
        }
    }
}