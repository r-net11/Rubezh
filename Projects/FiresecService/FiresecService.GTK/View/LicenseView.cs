using System;
using System.IO;
using FiresecService.Processor;
using Gtk;
using Infrastructure.Common;
using Infrastructure.Common.License;
using Pango;
using RubezhAPI;

public class LicenseView
{
	private VBox vbox;
	private VBox vbox11;
	private VBox vbox12;
	private VBox vbox21;
	private HBox hbox211;
	private HBox hbox212;

	private HBox hbox1;
	private HBox hbox2;

	private Label keyLabel;
	private Label fileLabel;
	private Entry keyEntry;
	private Entry filePathEntry;
	private Button loadLicenseButton;

	private Label licenseStatusLabel;
	private Label remotePlaceNameLabel;
	private Label fireFightingNameLabel;
	private Label guardNameLabel;
	private Label accessNameLabel;
	private Label videoNameLabel;
	private Label opcServerNameLabel;
	private Label remotePlaceValueLabel;
	private Label fireFightingValueLabel;
	private Label guardValueLabel;
	private Label accessValueLabel;
	private Label videoValueLabel;
	private Label opcServerValueLabel;

	void OnLicenseChanged()
	{
		var licenseInfo = LicenseManager.CurrentLicenseInfo;
		keyEntry.Text = LicenseManager.InitialKey.ToString();
		licenseStatusLabel.LabelProp = licenseInfo.LicenseMode.ToDescription();
		remotePlaceValueLabel.LabelProp = licenseInfo.RemoteClientsCount.ToString();
		fireFightingValueLabel.LabelProp = licenseInfo.HasFirefighting ? "Да" : "Нет";
		guardValueLabel.LabelProp = licenseInfo.HasGuard ? "Да" : "Нет";
		accessValueLabel.LabelProp = licenseInfo.HasSKD ? "Да" : "Нет";
		videoValueLabel.LabelProp = licenseInfo.HasVideo ? "Да" : "Нет";
		opcServerValueLabel.LabelProp = licenseInfo.HasOpcServer ? "Да" : "Нет";
	}

	public Widget Create()
	{
		var licenseInfo = LicenseManager.CurrentLicenseInfo;
		LicenseManager.LicenseChanged += OnLicenseChanged;

		vbox = new VBox { Spacing = 6 };

		hbox1 = new HBox { Spacing = 16, Margin = 10};
		vbox11 = new VBox { Spacing = 6 };
		vbox12 = new VBox { Spacing = 6 };

		hbox2 = new HBox { Spacing = 6, MarginBottom = 10, MarginStart = 10 };
		vbox21 = new VBox { Spacing = 6, MarginEnd = 10};
		hbox211 = new HBox { Spacing = 4 };
		hbox212 = new HBox { Spacing = 6 };

		WidgetHelper.AddWidget(hbox1, vbox11, 0);
		WidgetHelper.AddWidget(hbox1, vbox12, 1);

		WidgetHelper.AddWidget(hbox2, vbox21, 0, true);
		WidgetHelper.AddWidget(vbox21, hbox211, 0);
		WidgetHelper.AddWidget(vbox21, hbox212, 1);

		WidgetHelper.AddWidget(vbox, hbox1, 1, true);
		WidgetHelper.AddWidget(vbox, hbox2, 2);

		licenseStatusLabel = new Label { LabelProp = "Статус лицензии: " + licenseInfo.LicenseMode.ToDescription(), Halign = Align.Start, Margin = 10 };
		licenseStatusLabel.ModifyFont(FontDescription.FromString("Arial 12"));
		WidgetHelper.AddWidget(vbox, licenseStatusLabel, 0);

		remotePlaceNameLabel = new Label { LabelProp = "GLOBAL Удаленное рабочее место (количество)" };
		remotePlaceNameLabel.ModifyFont(FontDescription.FromString("Arial 12"));
		WidgetHelper.AddWidget(vbox11, remotePlaceNameLabel, 0);

		fireFightingNameLabel = new Label { LabelProp = "GLOBAL Пожаротушение" };
		fireFightingNameLabel.ModifyFont(FontDescription.FromString("Arial 12"));
		WidgetHelper.AddWidget(vbox11, fireFightingNameLabel, 1);

		guardNameLabel = new Label { LabelProp = "GLOBAL Охрана" };
		guardNameLabel.ModifyFont(FontDescription.FromString("Arial 12"));
		WidgetHelper.AddWidget(vbox11, guardNameLabel, 2);

		accessNameLabel = new Label { LabelProp = "GLOBAL Доступ" };
		accessNameLabel.ModifyFont(FontDescription.FromString("Arial 12"));
		WidgetHelper.AddWidget(vbox11, accessNameLabel, 3);

		videoNameLabel = new Label { LabelProp = "GLOBAL Видео" };
		videoNameLabel.ModifyFont(FontDescription.FromString("Arial 12"));
		WidgetHelper.AddWidget(vbox11, videoNameLabel, 4);

		opcServerNameLabel = new Label { LabelProp = "GLOBAL OPC Сервер" };
		opcServerNameLabel.ModifyFont(FontDescription.FromString("Arial 12"));
		WidgetHelper.AddWidget(vbox11, opcServerNameLabel, 5);

		remotePlaceValueLabel = new Label { LabelProp = licenseInfo.RemoteClientsCount.ToString() };
		remotePlaceValueLabel.ModifyFont(FontDescription.FromString("Arial 12"));
		WidgetHelper.AddWidget(vbox12, remotePlaceValueLabel, 0);

		fireFightingValueLabel = new Label { LabelProp = licenseInfo.HasFirefighting ? "Да" : "Нет" };
		fireFightingValueLabel.ModifyFont(FontDescription.FromString("Arial 12"));
		WidgetHelper.AddWidget(vbox12, fireFightingValueLabel, 1);

		guardValueLabel = new Label { LabelProp = licenseInfo.HasGuard ? "Да" : "Нет" };
		guardValueLabel.ModifyFont(FontDescription.FromString("Arial 12"));
		WidgetHelper.AddWidget(vbox12, guardValueLabel, 2);

		accessValueLabel = new Label { LabelProp = licenseInfo.HasSKD ? "Да" : "Нет" };
		accessValueLabel.ModifyFont(FontDescription.FromString("Arial 12"));
		WidgetHelper.AddWidget(vbox12, accessValueLabel, 3);

		videoValueLabel = new Label { LabelProp = licenseInfo.HasVideo ? "Да" : "Нет" };
		videoValueLabel.ModifyFont(FontDescription.FromString("Arial 12"));
		WidgetHelper.AddWidget(vbox12, videoValueLabel, 4);

		opcServerValueLabel = new Label { LabelProp = licenseInfo.HasOpcServer ? "Да" : "Нет" };
		opcServerValueLabel.ModifyFont(FontDescription.FromString("Arial 12"));
		WidgetHelper.AddWidget(vbox12, opcServerValueLabel, 5);

		keyLabel = new Label { LabelProp = "Ключ:" };
		WidgetHelper.AddWidget(hbox211, keyLabel, 0);

		keyEntry = new Entry
		{
			Text = LicenseManager.InitialKey.ToString(),
			IsEditable = false,
			CanFocus = false
		};

		WidgetHelper.AddWidget(hbox211, keyEntry, 1, true);
		
		fileLabel = new Label
		{
			Name = "fileLabel",
			LabelProp = "Файл:"
		};

		WidgetHelper.AddWidget(hbox212, fileLabel, 0);

		filePathEntry = new Entry();
		WidgetHelper.AddWidget(hbox212, filePathEntry, 1, true);

		loadLicenseButton = new Button
		{
			CanFocus = true,
			Label = "Загрузить лицензию"
		};
		loadLicenseButton.Clicked += LoadLicenseButtonOnClicked;
		WidgetHelper.AddWidget(hbox212, loadLicenseButton, 3);

		return vbox;
	}

	private void LoadLicenseButtonOnClicked(object sender, EventArgs eventArgs)
	{
		if (!File.Exists(filePathEntry.Text))
		{
			var md = new MessageDialog(null, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "Файл не найден");
			md.Run();
			md.Destroy();
			return;
		}

		if (Path.GetExtension(filePathEntry.Text) != ".license")
		{
			var md = new MessageDialog(null, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "Файл имеет неверный формат");
			md.Run();
			md.Destroy();
			return;
		}

		if (!LicenseManager.CheckLicense(filePathEntry.Text))
		{
			var md = new MessageDialog(null, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "Некорректный файл лицензии");
			md.Run();
			md.Destroy();
			return;
		}

		try
		{
			File.Copy(filePathEntry.Text, AppDataFolderHelper.GetFile("FiresecService.license"), true);
		}
		catch (Exception e)
		{
			var md = new MessageDialog(null, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "Ошибка копирования файла лицензии.\n" + e.Message);
			md.Run();
			md.Destroy();
			return;
		}
		FiresecLicenseProcessor.SetLicense(LicenseManager.TryLoad(AppDataFolderHelper.GetFile("FiresecService.license")));
	}

	public class LicenseTreeNode : TreeNode
	{
		public LicenseTreeNode(string licenseName, string status)
		{
			LicenseName = licenseName;
			Status = status;
		}

		[TreeNodeValue(Column = 0)]
		public string LicenseName;

		[TreeNodeValue(Column = 1)]
		public string Status;
	}
}
