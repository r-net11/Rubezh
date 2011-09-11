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


    }
}