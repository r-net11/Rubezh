using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using PlansModule.Views;
using System.Windows.Controls.Primitives;
using System;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Documents;
using PlansModule.Resize;
using System.Collections;


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
            ContextMenu res=null;
            if (ActiveElement is Polygon)
            {
                //var polygon = 
                PlanCanvasView.ElementProperties = (ActiveElement as Polygon).Name;
                res=CreateContextMenuForZona(element);
            };
            if (ActiveElement is Rectangle)
            {
                PlanCanvasView.ElementProperties = (ActiveElement as Rectangle).Name;
                res=CreateContextMenuForRectangle(element);
            }

            if (ActiveElement is TextBox)
            {
                PlanCanvasView.ElementProperties = (ActiveElement as TextBox).Name;
                res = CreateContextMenuForText(element);
            }
            return res;
        }

        ContextMenu CreateContextMenuForZona(object element)
        {

            ContextMenu contextMenu = new ContextMenu();
            MenuItem items = new MenuItem();
            items.Tag = element;
            items.Click += MouseDownEventHandler;
            items.Header = "Свойства зоны";
            
            contextMenu.Items.Add(items);
            return contextMenu;
        }

        ContextMenu CreateContextMenuForRectangle(object element)
        {
            ContextMenu contextMenu = new ContextMenu();
            MenuItem items = new MenuItem();
            items.Tag = element;
            items.Click += MouseDownEventHandler;
            items.Header = "Свойства прямоугольника";
            contextMenu.Items.Add(items);
            return contextMenu;
        }

        ContextMenu CreateContextMenuForText(object element)
        {
            ContextMenu contextMenu = new ContextMenu();
            MenuItem items = new MenuItem();
            items.Tag = element;
            items.Click += MouseDownEventHandler;
            items.Header = "Свойства текста";
            contextMenu.Items.Add(items);
            return contextMenu;
        }
    }
}