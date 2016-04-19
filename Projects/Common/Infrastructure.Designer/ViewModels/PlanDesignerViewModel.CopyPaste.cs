using Common;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans;
using Infrustructure.Plans.Events;
using RubezhAPI.Plans.Elements;
using RubezhAPI.Plans.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Infrastructure.Designer.ViewModels
{
	public partial class PlanDesignerViewModel : BaseViewModel
	{
		private DesignerClipboard clipboard = new DesignerClipboard();

		private void InitializeCopyPasteCommands()
		{
			CopyCommand = new RelayCommand(OnCopy, CanCopyCut);
			CutCommand = new RelayCommand(OnCut, CanCopyCut);
			PasteCommand = new RelayCommand<IInputElement>(OnPaste, CanPaste);
		}

		public RelayCommand CopyCommand { get; private set; }
		private void OnCopy()
		{
			using (new WaitWrapper())
			{
				NormalizeZIndex();
				this.clipboard.Clear();
				this.clipboard.SourceAction = ClipboardSourceAction.Copy;
				foreach (var designerItem in DesignerCanvas.SelectedItems)
				{
					designerItem.UpdateElementProperties();
					this.clipboard.Buffer.Add(designerItem.Element);
				}
			}
		}
		public RelayCommand CutCommand { get; private set; }
		private void OnCut()
		{
			using (new WaitWrapper())
			{
				OnCopy();
				this.clipboard.SourceAction = ClipboardSourceAction.Cut;
				DesignerCanvas.RemoveAllSelected();
			}
		}
		private bool CanCopyCut(object obj)
		{
			return DesignerCanvas.SelectedItems.Count() > 0;
		}

		public RelayCommand<IInputElement> PasteCommand { get; private set; }
		private void OnPaste(IInputElement container)
		{
			using (new WaitWrapper())
			using (new TimeCounter("Command.Paste: {0}"))
				if (this.FitsCanvas(this.clipboard.Buffer))
				{
					DesignerCanvas.Toolbox.SetDefault();
					DesignerCanvas.DeselectAll();

					var newElements = this.clipboard.Buffer
						.Where(element => AllowPaste(element))
						.Select(element =>
						{
							var newElement = element.Clone();
							newElement.UID = Guid.NewGuid();
							return newElement;
						})
						.ToList();

					this.NormalizeBuffer(container, newElements);
					var newItems = newElements.Select(element => DesignerCanvas.CreateElement(element)).ToList();

					newItems.ForEach(item => item.IsSelected = true);
					ServiceFactoryBase.Events.GetEvent<ElementAddedEvent>().Publish(DesignerCanvas.SelectedElements.ToList());
					newItems.ForEach(item => item.IsSelected = true);
					MoveToFrontCommand.Execute();
					DesignerCanvas.DesignerChanged();
					if (this.clipboard.SourceAction == ClipboardSourceAction.Cut)
						this.clipboard.Clear();
					this.clipboard.SourceAction = ClipboardSourceAction.Copy;
				}
		}

		private bool AllowPaste(ElementBase element)
		{
			var vizualizationElement = element as IMultipleVizualization;
			if (vizualizationElement != null && !vizualizationElement.AllowMultipleVizualization && this.clipboard.SourceAction == ClipboardSourceAction.Copy)
				return false;
			return true;
		}

		private bool CanPaste(IInputElement obj)
		{
			if (this.DesignerCanvas != null && this.DesignerCanvas.Toolbox != null)
			{
				if (!this.DesignerCanvas.Toolbox.IsEnabled)
					return false;
			}
			return this.clipboard.Buffer.Count > 0;
		}

		private Rect GetBoundingRectangle(IEnumerable<ElementBase> elements)
		{
			IEnumerable<Rect> rectangles = this.clipboard.Buffer.Select(element => element.GetRectangle());
			double minLeft = rectangles.Min(rect => rect.Left);
			double minTop = rectangles.Min(rect => rect.Top);
			double maxRight = rectangles.Max(rect => rect.Left + rect.Width);
			double maxBottom = rectangles.Max(rect => rect.Top + rect.Height);
			return new Rect(minLeft, minTop, maxRight - minLeft, maxBottom - minTop);
		}

		private bool FitsCanvas(IEnumerable<ElementBase> elements)
		{
			Rect border = this.GetBoundingRectangle(elements);
			if (border.Width > DesignerCanvas.CanvasWidth || border.Height > DesignerCanvas.CanvasHeight)
			{
				MessageBoxService.Show("Размер вставляемого содержимого больше размеров плана");
				return false;
			}
			return true;
		}

		private void NormalizeBuffer(IInputElement container, IEnumerable<ElementBase> elements)
		{
			if (elements.Count() > 0)
			{
				Rect border = this.GetBoundingRectangle(elements);
				double minLeft = border.Left;
				double minTop = border.Top;
				double maxRight = border.Right;
				double maxBottom = border.Bottom;
				if (border.X < 0)
					border.X = 0;
				if (border.Y < 0)
					border.Y = 0;
				if (border.X + border.Width > DesignerCanvas.CanvasWidth)
					border.X = DesignerCanvas.CanvasWidth - border.Width;
				if (border.Y + border.Height > DesignerCanvas.CanvasHeight)
					border.Y = DesignerCanvas.CanvasHeight - border.Height;

				Point? point = container == null ? null : (Point?)Mouse.GetPosition(container);
				if (point.HasValue)
				{
					border.X = point.Value.X + border.Width <= DesignerCanvas.CanvasWidth ? point.Value.X : DesignerCanvas.CanvasWidth - border.Width;
					border.Y = point.Value.Y + border.Height <= DesignerCanvas.CanvasHeight ? point.Value.Y : DesignerCanvas.CanvasHeight - border.Height;
				}
				Vector shift = new Vector(border.X - minLeft, border.Y - minTop);
				foreach (var elementBase in elements)
					elementBase.SetPosition(elementBase.GetPosition() + shift);
			}
		}
	}
}
