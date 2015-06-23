using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Common;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;

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
			return DesignerCanvas.SelectedItems.Count() > 0;
		}

		public RelayCommand<IInputElement> PasteCommand { get; private set; }
		private void OnPaste(IInputElement container)
		{
			using (new TimeCounter("Command.Paste: {0}"))
				if (NormalizeBuffer(container))
				{
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
		}
		private bool CanPaste(IInputElement obj)
		{
			return _buffer.Count > 0;
		}

		private bool NormalizeBuffer(IInputElement container)
		{
			if (_buffer.Count > 0)
			{
				Point? point = container == null ? null : (Point?)Mouse.GetPosition(container);
				double minLeft = double.MaxValue;
				double minTop = double.MaxValue;
				double maxRight = 0;
				double maxBottom = 0;
				foreach (var elementBase in _buffer)
				{
					Rect rect = elementBase.GetRectangle();
					if (minLeft > rect.Left)
						minLeft = rect.Left;
					if (minTop > rect.Top)
						minTop = rect.Top;
					if (maxBottom < rect.Top + rect.Height)
						maxBottom = rect.Top + rect.Height;
					if (maxRight < rect.Left + rect.Width)
						maxRight = rect.Left + rect.Width;
				}
				Rect border = new Rect(minLeft, minTop, maxRight - minLeft, maxBottom - minTop);
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
				Vector shift = new Vector(border.X - minLeft, border.Y - minTop);
				//if (shift.X == 0 && shift.Y == 0)
				//	shift = new Vector(-minLeft / 2, -minTop / 2);
				foreach (var elementBase in _buffer)
					elementBase.Position += shift;
			}
			return true;
		}
	}
}
