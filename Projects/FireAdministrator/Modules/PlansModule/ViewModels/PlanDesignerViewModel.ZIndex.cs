using System.Windows.Controls;
using FiresecAPI.Models;
using Infrastructure.Common;
using PlansModule.Designer;

namespace PlansModule.ViewModels
{
    public partial class PlanDesignerViewModel : BaseViewModel
    {
        void InitializeZIndexCommands()
        {
            MoveToFrontCommand = new RelayCommand(OnMoveToFront);
            SendToBackCommand = new RelayCommand(OnSendToBack);
            MoveForwardCommand = new RelayCommand(OnMoveForward);
            MoveBackwardCommand = new RelayCommand(OnMoveBackward);
        }

        public RelayCommand MoveToFrontCommand { get; private set; }
        void OnMoveToFront()
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

            PlansModule.HasChanges = true;
        }

        public RelayCommand SendToBackCommand { get; private set; }
        void OnSendToBack()
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

            PlansModule.HasChanges = true;
        }

        public RelayCommand MoveForwardCommand { get; private set; }
        void OnMoveForward()
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

            PlansModule.HasChanges = true;
        }

        public RelayCommand MoveBackwardCommand { get; private set; }
        void OnMoveBackward()
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

            PlansModule.HasChanges = true;
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
