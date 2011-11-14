using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using DeviceControls;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using PlansModule.Designer;
using PlansModule.Events;
using PlansModule.Views;


namespace PlansModule.ViewModels
{
    public partial class PlanDesignerViewModel : BaseViewModel
    {
        public void MoveToFront()
        {
            int maxZIndex = 0;
            foreach (var designerItem in DesignerCanvas.Items)
            {
                IZIndexedElement iZIndexedElement = designerItem.ElementBase as IZIndexedElement;
                if (iZIndexedElement != null)
                {
                    maxZIndex = System.Math.Max(iZIndexedElement.ZIndex, maxZIndex);
                }
            }

            foreach (var designerItem in DesignerCanvas.SelectedItems)
            {
                IZIndexedElement iZIndexedElement = designerItem.ElementBase as IZIndexedElement;
                if (iZIndexedElement != null)
                {
                    iZIndexedElement.ZIndex = maxZIndex + 1;
                    Panel.SetZIndex(designerItem, maxZIndex + 1);
                }
            }
        }

        public void SendToBack()
        {
            int minZIndex = 0;
            foreach (var designerItem in DesignerCanvas.Items)
            {
                IZIndexedElement iZIndexedElement = designerItem.ElementBase as IZIndexedElement;
                if (iZIndexedElement != null)
                {
                    minZIndex = System.Math.Min(iZIndexedElement.ZIndex, minZIndex);
                }
            }

            foreach (var designerItem in DesignerCanvas.SelectedItems)
            {
                IZIndexedElement iZIndexedElement = designerItem.ElementBase as IZIndexedElement;
                if (iZIndexedElement != null)
                {
                    iZIndexedElement.ZIndex = minZIndex - 1;
                    Panel.SetZIndex(designerItem, minZIndex - 1);
                }
            }
        }

        public void MoveForward()
        {
            foreach (var designerItem in DesignerCanvas.SelectedItems)
            {
                IZIndexedElement iZIndexedElement = designerItem.ElementBase as IZIndexedElement;
                if (iZIndexedElement != null)
                {
                    iZIndexedElement.ZIndex++;
                    Panel.SetZIndex(designerItem, iZIndexedElement.ZIndex);
                }
            }
        }

        public void MoveBackward()
        {
            foreach (var designerItem in DesignerCanvas.SelectedItems)
            {
                IZIndexedElement iZIndexedElement = designerItem.ElementBase as IZIndexedElement;
                if (iZIndexedElement != null)
                {
                    iZIndexedElement.ZIndex--;
                    Panel.SetZIndex(designerItem, iZIndexedElement.ZIndex);
                }
            }
        }

        void NormalizeZIndex()
        {
            int tempZIndex = 300000;
            while (true)
            {
                int minZIndex = 300000;
                foreach (var designerItem in DesignerCanvas.Items)
                {
                    IZIndexedElement iZIndexedElement = designerItem.ElementBase as IZIndexedElement;
                    if (iZIndexedElement != null)
                    {
                        minZIndex = System.Math.Min(iZIndexedElement.ZIndex, minZIndex);
                    }
                }

                if (minZIndex >= 300000)
                    break;

                foreach (var designerItem in DesignerCanvas.Items)
                {
                    IZIndexedElement iZIndexedElement = designerItem.ElementBase as IZIndexedElement;
                    if (iZIndexedElement != null)
                    {
                        if (iZIndexedElement.ZIndex == minZIndex)
                        {
                            iZIndexedElement.ZIndex = tempZIndex;
                            tempZIndex++;
                            break;
                        }
                    }
                }
            }

            foreach (var designerItem in DesignerCanvas.Items)
            {
                IZIndexedElement iZIndexedElement = designerItem.ElementBase as IZIndexedElement;
                if (iZIndexedElement != null)
                {
                    iZIndexedElement.ZIndex -= 3000000;
                    Panel.SetZIndex(designerItem, iZIndexedElement.ZIndex);
                }
            }
        }
    }
}
