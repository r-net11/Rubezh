using System;
using System.Linq;
using System.Collections.Generic;
using Infrastructure.Common;
using System.Windows;
using FiresecAPI.Models;
using PlansModule.Designer;

namespace PlansModule.ViewModels
{
    public partial class PlansViewModel : RegionViewModel
    {
        List<ElementBase> Buffer;

        public RelayCommand CopyCommand { get; private set; }
        void OnCopy()
        {
            PlanDesignerViewModel.Save();
            Buffer = new List<ElementBase>();
            foreach (var designerItem in DesignerCanvas.SelectedItems)
            {
                Buffer.Add(designerItem.ElementBase.Clone());
            }
        }

        public RelayCommand CutCommand { get; private set; }
        void OnCut()
        {
            OnCopy();

            for (int i = DesignerCanvas.Items.Count(); i > 0 ; i--)
            {
                var designerItem = DesignerCanvas.Children[i - 1] as DesignerItem;
                if (designerItem.IsSelected)
                {
                    DesignerCanvas.Children.RemoveAt(i - 1);
                }
            }
            PlansModule.HasChanges = true;
        }

        public RelayCommand PasteCommand { get; private set; }
        void OnPaste()
        {
            if (Buffer != null)
            {
                NormalizeBuffer();

                DesignerCanvas.DeselectAll();
                foreach (var elementBase in Buffer)
                {
                    var designerItem = DesignerCanvas.AddElementBase(elementBase);
                    designerItem.IsSelected = true;
                }
                PlansModule.HasChanges = true;
            }
        }

        void NormalizeBuffer()
        {
            if (Buffer != null)
            {
                double minLeft = double.MaxValue;
                double minTop = double.MaxValue;
                double maxRight = 0;
                double maxBottom = 0;
                foreach (var elementBase in Buffer)
                {
                    minLeft = Math.Min(elementBase.Left, minLeft);
                    minTop = Math.Min(elementBase.Top, minTop);
                    maxRight = Math.Max(elementBase.Left + elementBase.Width, maxRight);
                    maxBottom = Math.Max(elementBase.Top + elementBase.Height, maxBottom);
                }
                foreach (var elementBase in Buffer)
                {
                    elementBase.Left -= minLeft;
                    elementBase.Top -= minTop;
                }
                maxRight -= minLeft;
                maxBottom -= minTop;

                if ((maxRight > PlanDesignerViewModel.Plan.Width) || (maxBottom > PlanDesignerViewModel.Plan.Height))
                {
                    MessageBox.Show("Размер вставляемого содержимого больше размеров плана");
                }
            }
        }
    }
}
