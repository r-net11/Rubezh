using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Infrastructure.Common
{

    public partial class DialogWindow : Window
    {
        public DialogWindow()
        {
            InitializeComponent();
            Closed += new EventHandler(DialogWindow_Closed);
        }

        void DialogWindow_Closed(object sender, EventArgs e)
        {
            var content = Content as IDialogContent;
            if (content != null)
            {
                content.Close(false);
            }
        }

        public void SetContent(IDialogContent content)
        {
            if (!string.IsNullOrEmpty(content.Title))
                Title = content.Title;

            Content = content.InternalViewModel;
            content.Surface = this;
        }
    }
}
