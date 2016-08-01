using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Events;
using StrazhAPI.Plans.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Infrastructure.Designer.ViewModels
{
	public partial class PlanDesignerViewModel : BaseViewModel
	{
		private List<ElementBase> _buffer;

		private void InitializeCopyPasteCommands()
		{
			CopyCommand = new RelayCommand(OnCopy, CanCopyCut);
			CutCommand = new RelayCommand(OnCut, CanCopyCut);
			PasteCommand = new RelayCommand<IInputElement>(OnPaste, CanPaste);
			_buffer = new List<ElementBase>();
		}

		public RelayCommand CopyCommand { get; private set; }
		private void OnCopy()
		{
			NormalizeZIndex();
			_buffer = new List<ElementBase>();
			foreach (var designerItem in DesignerCanvas.SelectedItems)
			{
				designerItem.UpdateElementProperties();
				_buffer.Add(designerItem.Element.Clone());
			}
		}
		public RelayCommand CutCommand { get; private set; }
		private void OnCut()
		{
			OnCopy();
			DesignerCanvas.RemoveAllSelected();
			DesignerCanvas.DesignerChanged();
		}
		private bool CanCopyCut(object obj)
		{
			return DesignerCanvas.SelectedItems.Any();
		}

		public RelayCommand<IInputElement> PasteCommand { get; private set; }
		private void OnPaste(IInputElement container)
		{
			if (!NormalizeBuffer(container)) return;

			var designerItems = new List<DesignerItem>();
			DesignerCanvas.Toolbox.SetDefault();
			DesignerCanvas.DeselectAll();
			var newItems = new List<DesignerItem>();

			foreach (var elementBase in _buffer)
			{
				var element = elementBase.Clone();
				element.UID = Guid.NewGuid();
				var designerItem = DesignerCanvas.CreateElement(element);
				designerItems.Add(designerItem);
				newItems.Add(designerItem);
			}

			newItems.ForEach(item => item.IsSelected = true);
			ServiceFactoryBase.Events.GetEvent<ElementAddedEvent>().Publish(DesignerCanvas.SelectedElements.ToList());
			newItems.ForEach(item => item.IsSelected = true);
			MoveToFrontCommand.Execute();
			DesignerCanvas.DesignerChanged();
		}
		private bool CanPaste(IInputElement obj)
		{
			return _buffer.Count > 0;
		}

		private bool NormalizeBuffer(IInputElement container)
		{
			if (_buffer.Count <= 0) return true;

			var point = container == null ? null : (Point?)Mouse.GetPosition(container);
			var minLeft = double.MaxValue;
			var minTop = double.MaxValue;
			double maxRight = 0;
			double maxBottom = 0;

			foreach (var elementBase in _buffer)
			{
				var rect = elementBase.GetRectangle();
				if (minLeft > rect.Left)
					minLeft = rect.Left;
				if (minTop > rect.Top)
					minTop = rect.Top;
				if (maxBottom < rect.Top + rect.Height)
					maxBottom = rect.Top + rect.Height;
				if (maxRight < rect.Left + rect.Width)
					maxRight = rect.Left + rect.Width;
			}

			var border = new Rect(minLeft, minTop, maxRight - minLeft, maxBottom - minTop);
			if (border.Width > DesignerCanvas.CanvasWidth || border.Height > DesignerCanvas.CanvasHeight)
			{
				MessageBoxService.Show("Размер вставляемого содержимого больше размеров плана");
				return false;
			}
			if (border.X < 0)
				border.X = 0;
			if (border.Y < 0)
				border.Y = 0;
			if (border.X + border.Width > DesignerCanvas.CanvasWidth)
				border.X = DesignerCanvas.CanvasWidth - border.Width;
			if (border.Y + border.Height > DesignerCanvas.CanvasHeight)
				border.Y = DesignerCanvas.CanvasHeight - border.Height;
			if (point.HasValue)
			{
				border.X = point.Value.X + border.Width <= DesignerCanvas.CanvasWidth ? point.Value.X : DesignerCanvas.CanvasWidth - border.Width;
				border.Y = point.Value.Y + border.Height <= DesignerCanvas.CanvasHeight ? point.Value.Y : DesignerCanvas.CanvasHeight - border.Height;
			}

			var shift = new Vector(border.X - minLeft, border.Y - minTop);

			foreach (var elementBase in _buffer)
				elementBase.Position += shift;

			return true;
		}
	}
}
