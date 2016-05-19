using Gtk;

public static class WidgetHelper
{
	public static void AddWidget(Container box, Widget widget, int position, bool expandAndFill = false)
	{
		box.Add(widget);
		((Box.BoxChild)box[widget]).Position = position;
		((Box.BoxChild)box[widget]).Expand = expandAndFill;
		((Box.BoxChild)box[widget]).Fill = expandAndFill;
	}
}
