using Gtk;

namespace FiresecService
{
	class StatusView
	{
		VBox vBox;
		Entry localAddressEntry;
		Entry remoteAddressEntry;
		Entry reportAddressEntry;

		public Widget Create()
		{
			vBox = new VBox{ Spacing = 6 };
			var hBox1 = new HBox { Spacing = 6, MarginLeft = 5, MarginTop = 10, MarginRight = 5 };
			var vBox11 = new VBox();
			var vBox12 = new VBox();
			var hBox2 = new HBox { Spacing = 7, MarginLeft = 5, MarginRight = 5 };
			var vBox21 = new VBox();
			var vBox22 = new VBox();
			var hBox3 = new HBox { Spacing = 24, MarginLeft = 5, MarginRight = 5 };
			var vBox31 = new VBox();
			var vBox32 = new VBox();
			WidgetHelper.AddWidget(vBox, hBox1, 0);
			WidgetHelper.AddWidget(hBox1, vBox11, 0);
			WidgetHelper.AddWidget(hBox1, vBox12, 1, true);
			WidgetHelper.AddWidget(vBox, hBox2, 1);
			WidgetHelper.AddWidget(hBox2, vBox21, 0);
			WidgetHelper.AddWidget(hBox2, vBox22, 1, true);
			WidgetHelper.AddWidget(vBox, hBox3, 2);
			WidgetHelper.AddWidget(hBox3, vBox31, 0);
			WidgetHelper.AddWidget(hBox3, vBox32, 1, true);

			localAddressEntry = new Entry { IsEditable = false, CanFocus = false };
			remoteAddressEntry = new Entry { IsEditable = false, CanFocus = false };
			reportAddressEntry = new Entry { IsEditable = false, CanFocus = false };

			WidgetHelper.AddWidget(vBox11, new Label("Локальный адрес сервера"){ MarginTop = 3}, 0);
			WidgetHelper.AddWidget(vBox12, localAddressEntry, 0);

			WidgetHelper.AddWidget(vBox21, new Label("Удаленный адрес сервера") { MarginTop = 3 }, 0);
			WidgetHelper.AddWidget(vBox22, remoteAddressEntry, 0);

			WidgetHelper.AddWidget(vBox31, new Label("Адрес сервера отчетов") { MarginTop = 3 }, 0);
			WidgetHelper.AddWidget(vBox32, reportAddressEntry, 0);
			return vBox;
		}

		public string LocalAddress
		{
			set { localAddressEntry.Text = value; }
		}
		public string RemoteAddress
		{
			set { remoteAddressEntry.Text = value; }
		}
		public string ReportAddress
		{
			set { reportAddressEntry.Text = value; }
		}
	}
}
