using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.ViewModels;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using PlansModule.Designer;

namespace PlansModule.ViewModels
{
	public partial class PlansViewModel : MenuViewPartViewModel
	{
		private List<ElementBase> _buffer;

		private void InitializeCopyPaste()
		{
			CopyCommand = new RelayCommand(OnCopy, CanCopyCut);
			CutCommand = new RelayCommand(OnCut, CanCopyCut);
			PasteCommand = new RelayCommand<IInputElement>(OnPaste, CanPaste);
			_buffer = new List<ElementBase>();
		}

		public RelayCommand CopyCommand { get; private set; }
		private void OnCopy()
		{
			PlanDesignerViewModel.Save();
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
			ServiceFactory.SaveService.PlansChanged = true;
		}
		private bool CanCopyCut(object obj)
		{
			return DesignerCanvas.SelectedItems.Count() > 0;
		}

		public RelayCommand<IInputElement> PasteCommand { get; private set; }
		private void OnPaste(IInputElement container)
		{
			if (NormalizeBuffer(container))
			{
				DesignerCanvas.Toolbox.SetDefault();
				DesignerCanvas.DeselectAll();
				foreach (var elementBase in _buffer)
				{
					var element = elementBase.Clone();
					element.UID = Guid.NewGuid();
					var designerItem = DesignerCanvas.AddElement(element);
					designerItem.IsSelected = true;
				}
				PlanDesignerViewModel.MoveToFrontCommand.Execute();
				ServiceFactory.Events.GetEvent<ElementAddedEvent>().Publish(DesignerCanvas.SelectedElements);
				ServiceFactory.SaveService.PlansChanged = true;
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
				if (border.Width > PlanDesignerViewModel.Plan.Width || border.Height > PlanDesignerViewModel.Plan.Height)
				{
					MessageBoxService.Show("Размер вставляемого содержимого больше размеров плана");
					return false;
				}
				if (border.X < 0)
					border.X = 0;
				if (border.Y < 0)
					border.Y = 0;
				if (border.X + border.Width > PlanDesignerViewModel.Plan.Width)
					border.X = PlanDesignerViewModel.Plan.Width - border.Width;
				if (border.Y + border.Height > PlanDesignerViewModel.Plan.Height)
					border.Y = PlanDesignerViewModel.Plan.Height - border.Height;
				if (point.HasValue)
				{
					border.X = point.Value.X + border.Width <= PlanDesignerViewModel.Plan.Width ? point.Value.X : PlanDesignerViewModel.Plan.Width - border.Width;
					border.Y = point.Value.Y + border.Height <= PlanDesignerViewModel.Plan.Height ? point.Value.Y : PlanDesignerViewModel.Plan.Height - border.Height;
				}
				Vector shift = new Vector(border.X - minLeft, border.Y - minTop);
				//if (shift.X == 0 && shift.Y == 0)
				//    shift = new Vector(-minLeft / 2, -minTop / 2);
				foreach (var elementBase in _buffer)
					elementBase.Position += shift;
			}
			return true;
		}
	}
}