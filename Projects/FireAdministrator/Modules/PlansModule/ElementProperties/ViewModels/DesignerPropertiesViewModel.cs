using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FiresecAPI.Models;
using Infrastructure.Common;
using Microsoft.Win32;

namespace PlansModule.ViewModels
{
    public class DesignerPropertiesViewModel : SaveCancelDialogContent
    {
        public Plan Plan { get; private set; }
        public ImagePropertiesViewModel ImagePropertiesViewModel { get; private set; }

        public DesignerPropertiesViewModel(Plan plan)
        {
            Title = "Свойства элемента: План";
            ImagePropertiesViewModel = new ViewModels.ImagePropertiesViewModel();
            Plan = plan;
            CopyProperties();
        }

        void CopyProperties()
        {
            BackgroundColor = Plan.BackgroundColor;
            ImagePropertiesViewModel.BackgroundPixels = Plan.BackgroundPixels;
            ImagePropertiesViewModel.UpdateImage();
        }

        Color _backgroundColor;
        public Color BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                _backgroundColor = value;
                OnPropertyChanged("BackgroundColor");
            }
        }

        protected override void Save(ref bool cancel)
        {
            Plan.BackgroundColor = BackgroundColor;
            Plan.BackgroundPixels = ImagePropertiesViewModel.BackgroundPixels;
        }
    }
}
