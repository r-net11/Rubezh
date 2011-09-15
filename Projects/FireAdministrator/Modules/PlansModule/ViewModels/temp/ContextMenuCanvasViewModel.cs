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

        public ContextMenuCanvasViewModel(RoutedEventHandler MouseDownEventHandler)
        {
            this.MouseDownEventHandler = MouseDownEventHandler;
        }

        public ContextMenu GetElement(UIElement ActiveElement, object element)
        {
            ContextMenu contextMenu = null;
            if (ActiveElement is Polygon)
            {
                PlanCanvasView.ElementProperties = (ActiveElement as Polygon).Name;
                contextMenu = CreateContextMenu(element, "Свойства зоны");
            }

            if (ActiveElement is Rectangle)
            {
                PlanCanvasView.ElementProperties = (ActiveElement as Rectangle).Name;
                contextMenu = CreateContextMenu(element, "Свойства прямоугольника");
            }

            if (ActiveElement is TextBox)
            {
                PlanCanvasView.ElementProperties = (ActiveElement as TextBox).Name;
                contextMenu = CreateContextMenu(element, "Свойства текста");
            }
            return contextMenu;
        }

        ContextMenu CreateContextMenu(object element, string name)
        {
            var contextMenu = new ContextMenu();
            var items = new MenuItem();
            items.Tag = element;
            items.Click += MouseDownEventHandler;
            items.Header = name;
            contextMenu.Items.Add(items);
            return contextMenu;
        }
    }
}