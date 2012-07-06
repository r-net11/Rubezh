using System.Linq;
using System.Windows.Controls;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using PlansModule.Designer;
using Infrustructure.Plans.Elements;

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
		private void OnMoveToFront()
		{
			int maxZIndex = 0;
			foreach (var designerItem in DesignerCanvas.Items)
			{
				IElementZIndex iZIndexedElement = designerItem.Element as IElementZIndex;
				if (iZIndexedElement != null)
				{
					maxZIndex = System.Math.Max(iZIndexedElement.ZIndex, maxZIndex);
				}
			}

			foreach (var designerItem in DesignerCanvas.SelectedItems)
			{
				IElementZIndex iZIndexedElement = designerItem.Element as IElementZIndex;
				if (iZIndexedElement != null)
				{
					iZIndexedElement.ZIndex = maxZIndex + 1;
					Panel.SetZIndex(designerItem, maxZIndex + 1);
				}
			}

			ServiceFactory.SaveService.PlansChanged = true;
		}
		public RelayCommand SendToBackCommand { get; private set; }
		private void OnSendToBack()
		{
			int minZIndex = 0;
			foreach (var designerItem in DesignerCanvas.Items)
			{
				IElementZIndex iZIndexedElement = designerItem.Element as IElementZIndex;
				if (iZIndexedElement != null)
				{
					minZIndex = System.Math.Min(iZIndexedElement.ZIndex, minZIndex);
				}
			}

			foreach (var designerItem in DesignerCanvas.SelectedItems)
			{
				IElementZIndex iZIndexedElement = designerItem.Element as IElementZIndex;
				if (iZIndexedElement != null)
				{
					iZIndexedElement.ZIndex = minZIndex - 1;
					Panel.SetZIndex(designerItem, minZIndex - 1);
				}
			}

			ServiceFactory.SaveService.PlansChanged = true;
		}
		public RelayCommand MoveForwardCommand { get; private set; }
		private void OnMoveForward()
		{
			foreach (var designerItem in DesignerCanvas.SelectedItems)
			{
				IElementZIndex iZIndexedElement = designerItem.Element as IElementZIndex;
				if (iZIndexedElement != null)
				{
					iZIndexedElement.ZIndex++;
					Panel.SetZIndex(designerItem, iZIndexedElement.ZIndex);
				}
			}

			ServiceFactory.SaveService.PlansChanged = true;
		}
		public RelayCommand MoveBackwardCommand { get; private set; }
		private void OnMoveBackward()
		{
			foreach (var designerItem in DesignerCanvas.SelectedItems)
			{
				IElementZIndex iZIndexedElement = designerItem.Element as IElementZIndex;
				if (iZIndexedElement != null)
				{
					iZIndexedElement.ZIndex--;
					Panel.SetZIndex(designerItem, iZIndexedElement.ZIndex);
				}
			}

			ServiceFactory.SaveService.PlansChanged = true;
		}

		private void NormalizeZIndex()
		{
			var items = DesignerCanvas.Items.OfType<IElementZIndex>().OrderBy(item => item.ZIndex).ToList();
			for (int i = 0; i < items.Count; i++)
				items[i].ZIndex = i;
		}
	}
}
