using System;
using Infrastructure;
using Infrastructure.Common;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Collections.Generic;
using FiresecClient;
using FiresecAPI.Models;
using PlansModule.ViewModels;
using System.Windows.Media.Imaging;
using FiresecAPI.Models.Plans;
using PlansModule.Resize;
using System.Windows.Data;


namespace PlansModule.Views
{

    public partial class PropertiesView : UserControl
    {
        bool EditMode = false;
        public static int IndexElement;
        int IndexProperties = 0;
        UIElement element;
        public static Plan plan;

        public PropertiesView()
        {
            InitializeComponent();
            PlanCanvasView.PropertiesCaption = ListName;
            PlanCanvasView.PropertiesValue = ListValue;
            PlanCanvasView.TabItem = Properties;

        }

        private void ListValue_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
            if (!EditMode)
            {
                var test = sender as ListBox;
                object Selected = test.SelectedItem;
                Type t = Selected.GetType();
                IndexProperties = test.SelectedIndex;
                ListValue.UnselectAll();
                string type = PlanCanvasView.PropertiesType.Items[IndexProperties].ToString();
                switch (type)
                {
                    case "number":
                        {
                            TextBox textbox = new TextBox();
                            textbox.KeyUp += new KeyEventHandler(textbox_KeyUp);
                            textbox.Text = Selected.ToString();
                            ListValue.Items[IndexProperties] = textbox;
                            EditMode = true;
                        }
                        break;
                    case "string":
                        {
                            TextBox textbox = new TextBox();
                            textbox.KeyUp += new KeyEventHandler(textbox_KeyUp);
                            textbox.Text = Selected.ToString();
                            ListValue.Items[IndexProperties] = textbox;
                            EditMode = true;
                        }
                        break;
                }
            }

        }
        void UpdateElementPolygon(Polygon polygon)
        {
            string properties = PlanCanvasView.PropertiesName.Items[IndexProperties].ToString();
            object value = ListValue.Items[IndexProperties];
            switch (properties)
            {
                case "zoneno":
                    {
                        polygon.ToolTip = "Зона №" + value.ToString();
                    };
                    break;
                case "polygonpoints":
                    {
                    };
                    break;
            }
        }

        void UpdateElementRect(Rectangle rect)
        {
            string properties = PlanCanvasView.PropertiesName.Items[IndexProperties].ToString();
            object value = ListValue.Items[IndexProperties];
            switch (properties)
            {
                case "width":
                    {
                        rect.Width = double.Parse(value.ToString());
                    };
                    break;
                case "height":
                    {
                        rect.Height = double.Parse(value.ToString());
                    };
                    break;
                case "left":
                    {
                        Canvas.SetLeft(rect, double.Parse(value.ToString()));
                    };
                    break;
                case "top":
                    {
                        Canvas.SetTop(rect, double.Parse(value.ToString()));
                    };
                    break;
                case "background":
                    {
                    };
                    break;
            }
        }

        void UpdateElementTextBox(TextBox textBox)
        {
            string properties = PlanCanvasView.PropertiesName.Items[IndexProperties].ToString();
            object value = ListValue.Items[IndexProperties];
            switch (properties)
            {
                case "text":
                    {
                        textBox.Text = value.ToString();
                    };
                    break;
                case "fontsize":
                    {
                        textBox.FontSize = double.Parse(value.ToString());
                    };
                    break;
                case "left":
                    {
                        Canvas.SetLeft(textBox, double.Parse(value.ToString()));
                    };
                    break;
                case "top":
                    {
                        Canvas.SetTop(textBox, double.Parse(value.ToString()));
                    };
                    break;
                case "color":
                    {
                    };
                    break;
                case "bordercolor":
                    {
                    };
                    break;
            }
        }


        void textbox_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case (Key.Return):
                    {
                       OffEditMode(ListValue.Items);
                        element = MainCanvasViewModel.canvas.Children[IndexElement];
                        if (element is TextBox)
                        {
                            TextBox textbox = element as TextBox;
                            UpdateElementTextBox(textbox);
                        }
                        if (element is Rectangle)
                        {
                            Rectangle rect = element as Rectangle;
                            UpdateElementRect(rect);
                        }
                        if (element is Polygon)
                        {
                            Polygon polygon = element as Polygon;
                            UpdateElementPolygon(polygon);
                        }
                        PlanCanvasView.UpdateResizePlan(element, plan);
                    };
                    break;
                case (Key.Escape):
                    {
                        OffEditMode(ListValue.Items);
                    };
                    break;
            }

        }

        void OffEditMode(ItemCollection items)
        {
            int index = 0;
            foreach (var item in items)
            {
                if (item is TextBox)
                {
                    string str = (item as TextBox).Text;
                    items[index] = str;
                    OffEditMode(items);
                    break;
                }
                index++;
            }
            EditMode = false;
        }
        private void ListValue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EditMode)
            {
                OffEditMode(ListValue.Items);
                element = MainCanvasViewModel.canvas.Children[IndexElement];
                if (element is TextBox)
                {
                    TextBox textbox = element as TextBox;
                    UpdateElementTextBox(textbox);
                }
                if (element is Rectangle)
                {
                    Rectangle rect = element as Rectangle;
                    UpdateElementRect(rect);
                }
                if (element is Polygon)
                {
                    Polygon polygon = element as Polygon;
                    UpdateElementPolygon(polygon);
                }
                PlanCanvasView.UpdateResizePlan(element, plan);
            }
            else
            {
                OffEditMode(ListValue.Items);
            }
        }

        private void ListValue_GotFocus(object sender, RoutedEventArgs e)
        {
            UIElement element = MainCanvasViewModel.canvas.Children[IndexElement];
            MainCanvasViewModel mainCanvasViewModel = new MainCanvasViewModel(element);
        }
    }
}