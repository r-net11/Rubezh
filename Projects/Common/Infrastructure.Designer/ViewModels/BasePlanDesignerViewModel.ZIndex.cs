using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Plans.Designer;
using RubezhAPI.Plans.Elements;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Designer.ViewModels
{
	public partial class BasePlanDesignerViewModel : BaseViewModel
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
			// Computing the minimum Z-Index for the Group of selected Items:
			int baseGroupZIndex = DesignerCanvas.Items.Max(item => item.Element.ZIndex) + 1;
			// Ordering Items by their Z-Index and getting Index Increment for each Item:
			var elementsWithIndices = DesignerCanvas.SelectedItems
				.OrderBy(item => item.Element.ZIndex)
				.Select((item, index) => new
				{
					Index = index,
					Element = item.Element,
				});
			// Incrementing Z-Index for selected Items:
			foreach (var elementWithIndex in elementsWithIndices)
			{
				elementWithIndex.Element.ZIndex = baseGroupZIndex + elementWithIndex.Index;
			}
			// Updating the Designer:
			DesignerCanvas.UpdateZIndex();
			DesignerCanvas.DesignerChanged();
		}
		public RelayCommand SendToBackCommand { get; private set; }
		private void OnSendToBack()
		{
			// Computing the maximum Z-Index for the Group of selected Items:
			int baseGroupZIndex = DesignerCanvas.Items.Min(item => item.Element.ZIndex) - 1;
			// Ordering Items by their Z-Index and getting Index Decrement for each Item:
			var elementWithIndices = DesignerCanvas.SelectedItems
				.OrderByDescending(item => item.Element.ZIndex)
				.Select((item, index) => new
				{
					Index = index,
					Element = item.Element,
				});
			// Decrementing Z-Index for selected Items:
			foreach (var elementWithIndex in elementWithIndices)
			{
				elementWithIndex.Element.ZIndex = baseGroupZIndex - elementWithIndex.Index;
			}
			// Updating the Designer:
			DesignerCanvas.UpdateZIndex();
			DesignerCanvas.DesignerChanged();
		}
		public RelayCommand MoveForwardCommand { get; private set; }
		private void OnMoveForward()
		{
			foreach (var designerItem in this.DesignerCanvas.SelectedItems.OrderBy(x => x.Element.ZIndex))
				this.OnMoveForward(designerItem);
			DesignerCanvas.UpdateZIndex();
			DesignerCanvas.DesignerChanged();
		}
		private void OnMoveForward(DesignerItem item)
		{
			IEnumerable<DesignerItem> upperItems = this.DesignerCanvas.Items
				.Where(x => x.Element.ZIndex >= item.Element.ZIndex && x != item)
				.OrderBy(x => x.Element.ZIndex);
			DesignerItem closestItem = upperItems.FirstOrDefault();
			// Switching current Item and the closest one:
			if (closestItem != null)
				closestItem.Element.ZIndex = item.Element.ZIndex;
			item.Element.ZIndex++;
			foreach (DesignerItem upperItem in upperItems.Skip(1))
				upperItem.Element.ZIndex++;
		}

		public RelayCommand MoveBackwardCommand { get; private set; }
		private void OnMoveBackward()
		{
			foreach (var designerItem in this.DesignerCanvas.SelectedItems.OrderByDescending(x => x.Element.ZIndex))
				this.OnMoveBackward(designerItem);
			DesignerCanvas.UpdateZIndex();
			DesignerCanvas.DesignerChanged();
		}
		private void OnMoveBackward(DesignerItem item)
		{
			IEnumerable<DesignerItem> lowerItems = this.DesignerCanvas.Items
				.Where(x => x.Element.ZIndex <= item.Element.ZIndex && x != item)
				.OrderByDescending(x => x.Element.ZIndex);
			DesignerItem closestItem = lowerItems.FirstOrDefault();
			// Switching current Item and the closest one:
			if (closestItem != null)
				closestItem.Element.ZIndex = item.Element.ZIndex;
			item.Element.ZIndex--;
			// Moving all the lower items down to keep Order:
			foreach (DesignerItem lowerItem in lowerItems.Skip(1)) // Skipping the closest Item
				lowerItem.Element.ZIndex--;
		}

		protected void NormalizeZIndex()
		{
			Dictionary<int, List<ElementBase>> map = new Dictionary<int, List<ElementBase>>();
			foreach (var item in DesignerCanvas.Items)
			{
				if (!map.ContainsKey(item.Element.ZLayer))
					map.Add(item.Element.ZLayer, new List<ElementBase>());
				map[item.Element.ZLayer].Add(item.Element);
			}
			foreach (int key in map.Keys)
			{
				var list = map[key].OrderBy(item => item.ZIndex).ToList();
				for (int i = 0; i < list.Count; i++)
					list[i].ZIndex = i;
			}
		}
	}
}
