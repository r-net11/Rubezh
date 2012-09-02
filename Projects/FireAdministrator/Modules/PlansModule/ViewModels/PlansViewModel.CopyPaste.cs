using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using PlansModule.Designer;
using Infrastructure.ViewModels;

namespace PlansModule.ViewModels
{
	public partial class PlansViewModel : MenuViewPartViewModel
	{
		List<ElementBase> Buffer;

		void InitializeCopyPaste()
		{
			CopyCommand = new RelayCommand(OnCopy, CanCopyCut);
			CutCommand = new RelayCommand(OnCut, CanCopyCut);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
			Buffer = new List<ElementBase>();
		}

		bool CanCopyCut(object obj)
		{
			return DesignerCanvas.SelectedItems.Count() > 0;
		}

		public RelayCommand CopyCommand { get; private set; }
		void OnCopy()
		{
			PlanDesignerViewModel.Save();
			Buffer = new List<ElementBase>();
			foreach (var designerItem in DesignerCanvas.SelectedItems)
			{
				designerItem.UpdateElementProperties();
				Buffer.Add(designerItem.Element.Clone());
			}
		}

		public RelayCommand CutCommand { get; private set; }
		void OnCut()
		{
			OnCopy();
			DesignerCanvas.RemoveAllSelected();
			ServiceFactory.SaveService.PlansChanged = true;
		}

		bool CanPaste(object obj)
		{
			return Buffer.Count > 0;
		}

		public RelayCommand PasteCommand { get; private set; }
		void OnPaste()
		{
			if (NormalizeBuffer())
			{
				DesignerCanvas.Toolbox.SetDefault();
				DesignerCanvas.DeselectAll();
				foreach (var elementBase in Buffer)
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

		private bool NormalizeBuffer()
		{
			if (Buffer.Count > 0)
			{
				double minLeft = double.MaxValue;
				double minTop = double.MaxValue;
				double maxRight = 0;
				double maxBottom = 0;
				foreach (var elementBase in Buffer)
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
					border.X -= PlanDesignerViewModel.Plan.Width - border.Width;
				if (border.Y + border.Height > PlanDesignerViewModel.Plan.Height)
					border.Y -= PlanDesignerViewModel.Plan.Height - border.Height;
				Vector shift = new Vector(minLeft - border.X, minTop - border.Y);
				//if (shift.X == 0 && shift.Y == 0)
				//    shift = new Vector(-minLeft / 2, -minTop / 2);
				foreach (var elementBase in Buffer)
					elementBase.Position += shift;
			}
			return true;
		}
	}
}