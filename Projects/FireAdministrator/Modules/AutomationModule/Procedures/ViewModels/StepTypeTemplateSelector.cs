using System.Windows;
using System.Windows.Controls;
using AutomationModule.ViewModels;

namespace AutomationModule.ViewModels
{
	public class StepTypeTemplateSelector : DataTemplateSelector
	{
		public DataTemplate SoundsTemplate { get; set; }
		public DataTemplate JournalTemplate { get; set; }

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if (item is SoundStepViewModel)
			{
				return SoundsTemplate;
			}
			else if (item is JournalStepViewModel)
			{
				return JournalTemplate;
			}

			return null;
		}
	}
}