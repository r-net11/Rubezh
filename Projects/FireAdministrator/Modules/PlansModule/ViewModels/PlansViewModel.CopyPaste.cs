using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using FiresecAPI.Models;
using Infrastructure.Common;
using PlansModule.Designer;
using PlansModule.Events;
using Infrastructure;

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
            DesignerCanvas.RemoveAllSelected();
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
                    elementBase.UID = Guid.NewGuid();
                    var designerItem = DesignerCanvas.AddElement(elementBase);
                    designerItem.IsSelected = true;
                }
                ServiceFactory.Events.GetEvent<ElementAddedEvent>().Publish(DesignerCanvas.SelectedElements);
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
