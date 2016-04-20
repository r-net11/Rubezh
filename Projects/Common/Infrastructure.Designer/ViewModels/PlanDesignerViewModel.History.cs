using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Services;
using Infrustructure.Plans.Designer;
using RubezhAPI.Plans.Elements;
using Infrustructure.Plans.Events;
using System.Collections.Generic;

namespace Infrastructure.Designer.ViewModels
{
	public partial class PlanDesignerViewModel
	{
		private List<HistoryItem> _historyItems;
		private int _offset;
		private bool _historyAction = false;

		private void InitializeHistory()
		{
			UndoCommand = new RelayCommand(OnUndo, CanUndo);
			RedoCommand = new RelayCommand(OnRedo, CanRedo);

			ServiceFactoryBase.Events.GetEvent<ElementAddedEvent>().Unsubscribe(OnElementsAdded);
			ServiceFactoryBase.Events.GetEvent<ElementRemovedEvent>().Unsubscribe(OnElementsRemoved);
			ServiceFactoryBase.Events.GetEvent<ElementChangedEvent>().Unsubscribe(OnElementsChanged);

			ServiceFactoryBase.Events.GetEvent<ElementAddedEvent>().Subscribe(OnElementsAdded);
			ServiceFactoryBase.Events.GetEvent<ElementRemovedEvent>().Subscribe(OnElementsRemoved);
			ServiceFactoryBase.Events.GetEvent<ElementChangedEvent>().Subscribe(OnElementsChanged);

			ResetHistory();
		}
		protected void ResetHistory()
		{
			_historyItems = new List<HistoryItem>();
			_offset = 0;
		}
		private void AddHistoryItem(List<ElementBase> elementsBefore, List<ElementBase> elementsAfter, ActionType actionType)
		{
			var historyItem = new HistoryItem()
			{
				ActionType = actionType
			};

			foreach (var elementBase in elementsBefore)
				historyItem.ElementsBefore.Add(elementBase.Clone());

			foreach (var elementBase in elementsAfter)
				historyItem.ElementsAfter.Add(elementBase.Clone());

			if (_historyItems.Count > _offset)
				_historyItems.RemoveRange(_offset, _historyItems.Count - _offset);
			_historyItems.Add(historyItem);
			_offset = _historyItems.Count;
		}

		private void OnElementsAdded(List<ElementBase> elementsBefore)
		{
			if (!_historyAction)
				AddHistoryItem(new List<ElementBase>(), elementsBefore, ActionType.Added);
		}
		private void OnElementsRemoved(List<ElementBase> elementsBefore)
		{
			if (!_historyAction)
			{
				Save();
				AddHistoryItem(elementsBefore, new List<ElementBase>(), ActionType.Removed);
			}
		}
		private void OnElementsChanged(List<ElementBase> elementsBefore)
		{
		}
		public List<ElementBase> AddHistoryItem(List<ElementBase> elementsBefore)
		{
			var elementsAfter = DesignerCanvas.CloneElements(DesignerCanvas.SelectedItems);
			if (!_historyAction)
				AddHistoryItem(elementsBefore, elementsAfter, ActionType.Edited);
			return elementsAfter;
		}

		private void DoUndo()
		{
			var historyItem = _historyItems[_offset];
			_historyAction = true;
			var list = new List<DesignerItem>();
			switch (historyItem.ActionType)
			{
				case ActionType.Edited:
					foreach (var elementBase in historyItem.ElementsBefore)
					{
						var designerItem = DesignerCanvas.UpdateElement(elementBase);
						if (designerItem != null)
							list.Add(designerItem);
					}
					DesignerCanvas.UpdateZoom();
					ServiceFactoryBase.Events.GetEvent<ElementChangedEvent>().Publish(historyItem.ElementsBefore);
					break;
				case ActionType.Added:
					DesignerCanvas.RemoveDesignerItems(historyItem.ElementsAfter);
					break;
				case ActionType.Removed:
					foreach (var elementBase in historyItem.ElementsBefore)
					{
						var designerItem = DesignerCanvas.CreateElement(elementBase);
						list.Add(designerItem);
					}
					ServiceFactoryBase.Events.GetEvent<ElementAddedEvent>().Publish(historyItem.ElementsBefore);
					break;
			}
			DesignerCanvas.DeselectAll();
			list.ForEach(item => item.IsSelected = true);
			DesignerCanvas.Refresh();
			_historyAction = false;
			DesignerCanvas.Toolbox.SetDefault();
		}
		private void DoRedo()
		{
			var historyItem = _historyItems[_offset];
			_historyAction = true;
			var list = new List<DesignerItem>();
			switch (historyItem.ActionType)
			{
				case ActionType.Edited:
					foreach (var elementBase in historyItem.ElementsAfter)
					{
						var designerItem = DesignerCanvas.UpdateElement(elementBase);
						if (designerItem != null)
							list.Add(designerItem);
					}
					DesignerCanvas.UpdateZoom();
					ServiceFactoryBase.Events.GetEvent<ElementChangedEvent>().Publish(historyItem.ElementsAfter);
					break;
				case ActionType.Added:
					foreach (var elementBase in historyItem.ElementsAfter)
					{
						var designerItem = DesignerCanvas.CreateElement(elementBase);
						list.Add(designerItem);
					}
					ServiceFactoryBase.Events.GetEvent<ElementAddedEvent>().Publish(historyItem.ElementsAfter);
					break;
				case ActionType.Removed:
					DesignerCanvas.RemoveDesignerItems(historyItem.ElementsBefore);
					break;
			}
			DesignerCanvas.DeselectAll();
			list.ForEach(item => item.IsSelected = true);
			_historyAction = false;
			DesignerCanvas.Toolbox.SetDefault();
		}

		public RelayCommand UndoCommand { get; private set; }
		private void OnUndo()
		{
			_offset--;
			DoUndo();
		}
		private bool CanUndo()
		{
			return _offset > 0;
		}

		public RelayCommand RedoCommand { get; private set; }
		private void OnRedo()
		{
			DoRedo();
			_offset++;
		}
		private bool CanRedo()
		{
			return _historyItems.Count > _offset;
		}

		public void RevertLastAction()
		{
			if (UndoCommand.CanExecute(null))
			{
				UndoCommand.Execute();
				if (_historyItems.Count > _offset)
					_historyItems.RemoveAt(_offset);
			}
		}
	}

	class HistoryItem
	{
		public HistoryItem()
		{
			ElementsBefore = new List<ElementBase>();
			ElementsAfter = new List<ElementBase>();
		}

		public ActionType ActionType { get; set; }
		public List<ElementBase> ElementsBefore { get; set; }
		public List<ElementBase> ElementsAfter { get; set; }
	}

	enum ActionType
	{
		Added,
		Removed,
		Edited
	}
}
