using System;
using System.Linq;
using System.Windows.Media;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PlansModule.Designer
{
	public static class Helper
	{
		public static Plan GetPlan(ElementSubPlan element)
		{
			return FiresecManager.PlansConfiguration.AllPlans.FirstOrDefault(x => x.UID == element.PlanUID);
		}
		public static string GetSubPlanTitle(ElementSubPlan element)
		{
			Plan plan = GetPlan(element);
			if (plan == null && element.PlanUID != Guid.Empty)
				SetSubPlan(element, null);
			return plan == null ? "Несвязанный подплан" : plan.Caption;
		}
		public static void UpgradeBackground(IElementBackground element)
		{
			if (element.BackgroundPixels != null)
			{
				var guid = ServiceFactory.ContentService.AddContent(element.BackgroundPixels);
				element.BackgroundImageSource = guid;
				element.BackgroundPixels = null;
				ServiceFactory.SaveService.PlansChanged = true;
			}
			PainterCache.CacheBrush(element);
		}
		public static void SetSubPlan(ElementSubPlan element)
		{
			Plan plan = GetPlan(element);
			SetSubPlan(element, plan);
		}
		public static void SetSubPlan(ElementSubPlan element, Plan plan)
		{
			element.PlanUID = plan == null ? Guid.Empty : plan.UID;
			element.Caption = plan == null ? string.Empty : plan.Caption;
			element.BackgroundColor = GetSubPlanColor(plan);
		}
		public static Color GetSubPlanColor(Plan plan)
		{
			Color color = Colors.Black;
			if (plan != null)
				color = Colors.Green;
			return color;
		}

		public static StackPanel SetHeader(string title, string imageSourceUri)
		{
			TextBlock textBlock = new TextBlock();
			textBlock.Text = title;
			textBlock.VerticalAlignment = VerticalAlignment.Center;

			Image image = new Image();
			image.Width = 16;
			image.VerticalAlignment = VerticalAlignment.Center;
			BitmapImage sourceImage = new BitmapImage();
			sourceImage.BeginInit();
			sourceImage.UriSource = new Uri(imageSourceUri);
			sourceImage.EndInit();
			image.Source = sourceImage;

			StackPanel stackPanel = new StackPanel();
			stackPanel.Orientation = Orientation.Horizontal;
			stackPanel.Children.Add(image);
			stackPanel.Children.Add(textBlock);

			return stackPanel;

		}
	}
}