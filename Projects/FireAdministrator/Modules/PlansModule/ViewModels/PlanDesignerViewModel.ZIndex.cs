using System.Linq;
using System.Windows.Controls;
using FiresecAPI.Models;
using Infrastructure.Common;
using PlansModule.Designer;
using Infrastructure;

namespace PlansModule.ViewModels
{
    public partial class PlanDesignerViewModel : BaseViewModel
    {
        void InitializeZIndexCommands()
        {
            MoveToFrontCommand = new RelayCommand(OnMoveToFront, CanMoveExecute);
            SendToBackCommand = new RelayCommand(OnSendToBack, CanMoveExecute);
            MoveForwardCommand = new RelayCommand(OnMoveForward, CanMoveExecute);
            MoveBackwardCommand = new RelayCommand(OnMoveBackward, CanMoveExecute);
        }

        public bool CanMoveExecute(object obj)
        {
            return DesignerCanvas.SelectedItems.Count() > 0;
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

            ServiceFactory.SaveService.PlansChanged = true;
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

            ServiceFactory.SaveService.PlansChanged = true;
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

            ServiceFactory.SaveService.PlansChanged = true;
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

            ServiceFactory.SaveService.PlansChanged = true;
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
