using System;
using Common;
using FiresecAPI.Models;
using Infrastructure.Client.Plans;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using PlansModule.Designer;

namespace PlansModule.ViewModels
{
	public partial class PlanDesignerViewModel : BaseViewModel, IPlanDesignerViewModel
	{
		public event EventHandler Updated;
		public DesignerCanvas DesignerCanvas { get; set; }
		public Plan Plan { get; private set; }

		public PlanDesignerViewModel()
		{
			InitializeZIndexCommands();
			InitializeAlignCommands();
		}

		public void Initialize(Plan plan)
		{
			using (new TimeCounter("\tPlanDesignerViewModel.Initialize: {0}"))
			{
				Plan = plan;
				OnPropertyChanged("Plan");
				if (Plan != null)
				{
					using (new WaitWrapper())
					{
						using (new TimeCounter("\t\tDesignerCanvas.Clear: {0}"))
							DesignerCanvas.Clear();
						ChangeZoom(1);
						DesignerCanvas.Plan = plan;
						DesignerCanvas.PlanDesignerViewModel = this;
						using (new TimeCounter("\t\tDesignerCanvas.Background: {0}"))
							DesignerCanvas.Update();

						using (new TimeCounter("\t\tDesignerItem.Create: {0}"))
						{

							// 1. Painter -> return Visual					+
							// 2. DesignerItem inherite FramworkElement		+/- Control

							// 3. DesignerItem cache (use ResetElement)		???
							// 4. Change ResizeDecorator/ResizeAdorner		-


							foreach (var elementBase in PlanEnumerator.Enumerate(plan))
								DesignerCanvas.Create(elementBase);
							foreach (var element in DesignerCanvas.Toolbox.PlansViewModel.LoadPlan(plan))
								DesignerCanvas.Create(element);


							//using (new TimeCounter("\t\t\tPainter.Draw: {0}"))
							//    foreach (var designerItem in DesignerCanvas.Items)
							//    designerItem.Painter.Draw(designerItem.Element);

							//int count = 1000;
							//    //for (int i = 0; i < count; i++)
							//    //{
							//    //    var element = new ElementRectangle()
							//    //    {
							//    //        Left = 3 * i,
							//    //        Top = 3 * i,
							//    //        Width = 3,
							//    //        Height = 3,
							//    //    };
							//    //    var painter = PainterFactory.Create(element);
							//    //    var content = painter.Draw(element);

							//    //    var container = new ContentControl();
							//    //    container.Content = content;
							//    //    container.Width = element.Width;
							//    //    container.Height = element.Height;

							//    //    System.Windows.Controls.Canvas.SetLeft(container, 3 * i);
							//    //    System.Windows.Controls.Canvas.SetTop(container, 3 * i);
							//    //    DesignerCanvas.Children.Add(container);
							//    //}
							//    for (int i = 0; i < count; i++)
							//    {
							//        var element = new ElementRectangle()
							//        {
							//            Left = 3 * i,
							//            Top = 3 * i,
							//            Width = 300,
							//            Height = 300,
							//        };
							//        DesignerCanvas.Create(element);
							//        //var designerItem = DesignerItemFactory.Create(element);
							//        //System.Windows.Controls.Canvas.SetLeft(designerItem, 3 * i);
							//        //System.Windows.Controls.Canvas.SetTop(designerItem, 3 * i);
							//        //DesignerCanvas.Children.Add(designerItem);
							//    }
						}
					}
					OnUpdated();
				}
			}
		}

		public void Save()
		{
			if (Plan == null)
				return;
			NormalizeZIndex();
		}

		public void Update()
		{
			OnUpdated();
		}

		private void OnUpdated()
		{
			if (Updated != null)
				Updated(this, EventArgs.Empty);
		}

		#region IPlanDesignerViewModel Members

		public object Toolbox
		{
			get { return DesignerCanvas.Toolbox; }
		}

		public object Canvas
		{
			get { return DesignerCanvas; }
		}

		#endregion
	}
}