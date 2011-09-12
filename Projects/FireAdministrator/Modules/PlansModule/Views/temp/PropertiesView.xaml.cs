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

        public PropertiesView()
        {
            InitializeComponent();
            PlanCanvasView.PropertiesName = ListName;
            PlanCanvasView.PropertiesValue = ListValue;
            PlanCanvasView.TabItem = Properties;
        }

        private void ListValue_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var test = sender as ListBox;
            object Selected = test.SelectedItem;
            int index = test.SelectedIndex;
            ListValue.UnselectAll();


            TextBox textbox = new TextBox();
            textbox.Text = Selected.ToString();

            ListValue.Items[index] = textbox;
            textbox.Focus();
            textbox.SelectAll();
            DataGridTextColumn textname = new DataGridTextColumn();
            Binding b = new Binding("name");
            textname.Binding = b;
            textname.Header = "Наименование";
            datagrid.Columns.Add(textname);
            DataGridTextColumn textval = new DataGridTextColumn();
            b = new Binding("value");
            b.Source = ListValue;
            textval.Binding = b;
            textval.Header = "Значение";
            
            datagrid.Columns.Add(textval);
            DataGridRow row = new DataGridRow();
            
        }


    }
}