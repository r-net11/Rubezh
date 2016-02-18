using RubezhAPI.License;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Threading;
using FiresecService.Views.TypeConverters;
using System.Windows.Data;

namespace FiresecService.Views
{
	public partial class MainView : Form, IMainView
	{
		#region Constructors

		public MainView()
		{
			InitializeComponent();
			Initialize();
		}

		#endregion

		#region Fields And Properties

		public string Title
		{
			get { return Text; }
			set { Text = value; }
		}

		// Вкладка "Соедининия"
		TabPage _tabPageConnections;
		DataGridView _dataGridViewConnections;
		ContextMenuStrip _contexMenuStripConnections;
		ToolStripMenuItem _toolStripMenuItemDisconnect;

		public BindingSource ClientsContext
		{
			set
			{
				_dataGridViewConnections.DataSource = null;
				_dataGridViewConnections.DataSource = value;
			}
		}

		public bool EnableMenuDisconnect
		{
			get
			{
				return _toolStripMenuItemDisconnect.Enabled;
			}
			set
			{
				_toolStripMenuItemDisconnect.Enabled = value;
			}
		}

		// Вкладка "Лог"
		TabPage _tabPageLog;
		DataGridView _dataGridViewLog;

		public BindingSource LogsContext
		{
			set
			{
				_dataGridViewLog.DataSource = null;
				_dataGridViewLog.DataSource = value;
			}
		}
		
		// Вкладка "Статус"
		TabPage _tabPageStatus;
		Label _labelServerLocalAddress;
		Label _labelServerRemoteAddress;
		Label _labelReportServerAddress;

		public string LocalAddress
		{
			set { _labelServerLocalAddress.Text = value; }
		}

		public string RemoteAddress
		{
			set { _labelServerRemoteAddress.Text = value; }
		}

		public string ReportAddress
		{
			set { _labelReportServerAddress.Text = value; }
		}

		// Вкладка ГК
		TabPage _tabPageGK;
		DataGridView _dataGridViewGkLifecycles;
		public BindingSource GkLifecyclesContext 
		{
			set 
			{
				_dataGridViewGkLifecycles.DataSource = null;
				_dataGridViewGkLifecycles.DataSource = value;
			} 
		}

		// Вкладка Поллинг
		TabPage _tabPagePolling;
		DataGridView _dataGridViewPolling;
		public BindingSource ClientPollsContext 
		{
			set
			{
				_dataGridViewPolling.DataSource = null;
				_dataGridViewPolling.DataSource = value;
			}
		}

		// Вкладка Операции
		TabPage _tabPageOperations;
		DataGridView _dataGridViewOperations;
		public BindingSource OperationsContext 
		{
			set
			{
				_dataGridViewOperations.DataSource = null;
				_dataGridViewOperations.DataSource = value;
			}
		}

		// Вкладка Лицензирование 
		TabPage _tabPageLicense;
		Label _labelLicenseStatus;
		Label _labelRemoteWorkStation;
		Label _labelFirefighting;
		Label _labelSecurity;
		Label _labelAccess;
		Label _labelVideo;
		Label _labelOpcServer;
		Label _labelLicenseKey;
		Button _buttonLoadLicense;
		Button _buttonCopyLicense;

		public LicenseMode LicenseMode 
		{
			set
			{
				var converter = new EnumTypeConverter(typeof(LicenseMode));
				_labelLicenseStatus.Text = converter.ConvertToString(value);
			}
		}
		public int RemoteClientsCount 
		{
			set 
			{
				_labelRemoteWorkStation.Text = value.ToString();
			} 
		}
		public bool HasFirefighting 
		{
			set 
			{
				var converter = new BooleanTypeConverter();
				_labelFirefighting.Text = converter.ConvertToString(value);
			} 
		}
		public bool HasGuard 
		{
			set
			{
				var converter = new BooleanTypeConverter();
				_labelSecurity.Text = converter.ConvertToString(value);
			}
		}
		public bool HasSKD 
		{
			set
			{
				var converter = new BooleanTypeConverter();
				_labelAccess.Text = converter.ConvertToString(value);
			} 
		}
		public bool HasVideo 
		{
			set
			{
				var converter = new BooleanTypeConverter();
				_labelVideo.Text = converter.ConvertToString(value);
			} 
		}
		public bool HasOpcServer 
		{
			set
			{
				var converter = new BooleanTypeConverter();
				_labelOpcServer.Text = converter.ConvertToString(value);
			} 
		}
		public string InitialKey 
		{
			set { _labelLicenseKey.Text = value; }
		}
		
		// Сторока состояния окна
		public string LastLog
		{
			get { return _toolStripStatusLabelLastLog.Text; }
			set
			{
				_toolStripStatusLabelLastLog.Text = value;
			}
		}

		#endregion

		#region Event handlers for form

		private void EventHandler_MainWinFormView_FormClosing(object sender, FormClosingEventArgs e)
		{
			ShowInTaskbar = false;
			Visible = false;
			e.Cancel = true;
		}

		#endregion

		#region Event handlers for context menu _toolStripMenuItemDisconnect

		void EventHandler_toolStripMenuItemDisconnect_Click(object sender, EventArgs e)
		{
			OnCommandDisconnectActivated();
		}

		#endregion

		#region Methods

		void Initialize()
		{
			#region Tab Connections

			_tabPageConnections = new TabPage() { Name = "_tabPageConnections", Text = "Подключения" };
			_tabControlMain.TabPages.Add(_tabPageConnections);

			_dataGridViewConnections = new DataGridView()
			{
				Name = "_dataGridViewConnections",
				MultiSelect = false,
				SelectionMode = DataGridViewSelectionMode.FullRowSelect,
				Dock = DockStyle.Fill,
				AutoGenerateColumns = false,
				AllowUserToAddRows = false,
				AllowUserToDeleteRows = false,
				AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
			};
			_dataGridViewConnections.Columns.Add(new DataGridViewTextBoxColumn()
			{
				Name = "_dataGridViewColumnType",
				HeaderText = "Тип",
				DataPropertyName = "ClientType"
			});
			_dataGridViewConnections.Columns.Add(new DataGridViewTextBoxColumn()
			{
				Name = "_dataGridViewColumnAddress",
				HeaderText = "Адрес",
				DataPropertyName = "IpAddress",
			});
			_dataGridViewConnections.Columns.Add(new DataGridViewTextBoxColumn()
			{
				Name = "_dataGridViewColumnFriendlyUserName",
				HeaderText = "Пользователь",
				DataPropertyName = "FriendlyUserName"
			});
			_contexMenuStripConnections = new ContextMenuStrip()
			{
				Name = "_contexMenuStripConnections"
			};
			_toolStripMenuItemDisconnect = new ToolStripMenuItem(
					"Разорвать соединение",
					null,
					EventHandler_toolStripMenuItemDisconnect_Click,
					"_toolStripMenuItemDisconnect");
			_contexMenuStripConnections.Items.Add(_toolStripMenuItemDisconnect);
			_dataGridViewConnections.ContextMenuStrip = _contexMenuStripConnections;

			_tabPageConnections.Controls.Add(_dataGridViewConnections);

			#endregion

			#region Log

			_tabPageLog = new TabPage() { Name = "_tabPageLog", Text = "Лог" };
			_tabControlMain.TabPages.Add(_tabPageLog);

			_dataGridViewLog = new DataGridView()
			{
				Name = "_dataGridViewLog",
				MultiSelect = false,
				SelectionMode = DataGridViewSelectionMode.FullRowSelect,
				Dock = DockStyle.Fill,
				AutoGenerateColumns = false,
				AllowUserToAddRows = false,
				AllowUserToDeleteRows = false,
				AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
			};
			_dataGridViewLog.Columns.Add(new DataGridViewTextBoxColumn()
			{
				Name = "_dataGridViewColumnMessage",
				HeaderText = "Название",
				DataPropertyName = "Message"
			});
			_dataGridViewLog.Columns.Add(new DataGridViewTextBoxColumn()
			{
				Name = "_dataGridViewColumnDate",
				HeaderText = "Дата",
				DataPropertyName = "DateTime"
			});
			_tabPageLog.Controls.Add(_dataGridViewLog);

			#endregion

			#region Status

			_tabPageStatus = new TabPage() { Name = "_tabPageStatus", Text = "Статус" };
			_tabControlMain.TabPages.Add(_tabPageStatus);

			var font = new Font(Label.DefaultFont.FontFamily, 10, FontStyle.Bold); 

			var label = new Label()
			{
				Name = "_labelServerLocalAddressTitle",
				Text = "Локальный адрес сервера:",
				Font = font,
				AutoSize = true,
				Location = new Point(10, 10)
			};
			_tabPageStatus.Controls.Add(label);

			_labelServerLocalAddress = new Label()
			{
				Name = "_labelServerLocalAddressValue",
				Text = String.Empty,
				Font = font,
				AutoSize = true,
				Location = new Point(label.Location.X + label.Width + 10, label.Location.Y)
			};
			_tabPageStatus.Controls.Add(_labelServerLocalAddress);

			label = new Label()
			{
				Name = "_labelServerRemoteAddressTitle",
				Text = "Удалённый адрес сервера:",
				Font = font,
				AutoSize = true,
				Location = new Point(10, 40)
			};
			_tabPageStatus.Controls.Add(label);

			_labelServerRemoteAddress = new Label()
			{
				Name = "_labelServerRemoteAddressValue",
				Text = String.Empty,
				Font = font,
				AutoSize = true,
				Location = new Point(label.Location.X + label.Width + 10, label.Location.Y)
			};
			_tabPageStatus.Controls.Add(_labelServerRemoteAddress);

			label = new Label()
			{
				Name = "_labelReportServerAddressTitle",
				Text = "Адрес сервера отчётов:",
				Font = font,
				AutoSize = true,
				Location = new Point(10, 70)
			};
			_tabPageStatus.Controls.Add(label);

			_labelReportServerAddress = new Label()
			{
				Name = "_labelReportServerAddressValue",
				Text = String.Empty,
				Font = font,
				AutoSize = true,
				Location = new Point(label.Location.X + label.Width + 10, label.Location.Y)
			};
			_tabPageStatus.Controls.Add(_labelReportServerAddress);

			#endregion

			#region GK

			_tabPageGK = new TabPage() { Name = "_tabPageGK", Text = "ГК" };
			_tabControlMain.TabPages.Add(_tabPageGK);

			_dataGridViewGkLifecycles = new DataGridView()
			{
				Name = "_dataGridViewGkLifecycles",
				MultiSelect = false,
				SelectionMode = DataGridViewSelectionMode.FullRowSelect,
				Dock = DockStyle.Fill,
				AutoGenerateColumns = false,
				AllowUserToAddRows = false,
				AllowUserToDeleteRows = false,
				AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
			};
			_dataGridViewGkLifecycles.Columns.Add(new DataGridViewTextBoxColumn()
			{
				Name = "_dataGridViewColumnTime",
				HeaderText = "Время",
				DataPropertyName = "Time"
			});
			_dataGridViewGkLifecycles.Columns.Add(new DataGridViewTextBoxColumn()
			{
				Name = "_dataGridViewColumnAddress",
				HeaderText = "Адрес",
				DataPropertyName = "Address"
			});
			_dataGridViewGkLifecycles.Columns.Add(new DataGridViewTextBoxColumn()
			{
				Name = "_dataGridViewColumnName",
				HeaderText = "Название",
				DataPropertyName = "Name"
			});
			_dataGridViewGkLifecycles.Columns.Add(new DataGridViewTextBoxColumn()
			{
				Name = "_dataGridViewColumnProgress",
				HeaderText = "Прогресс",
				DataPropertyName = "Progress"
			});
			_tabPageGK.Controls.Add(_dataGridViewGkLifecycles);

			#endregion

			#region Polling

			_tabPagePolling = new TabPage() { Name = "_tabPagePolling", Text = "Поллинг" };
			_tabControlMain.TabPages.Add(_tabPagePolling);

			_dataGridViewPolling = new DataGridView()
			{
				Name = "_dataGridViewPolling",
				MultiSelect = false,
				SelectionMode = DataGridViewSelectionMode.FullRowSelect,
				Dock = DockStyle.Fill,
				AutoGenerateColumns = false,
				AllowUserToAddRows = false,
				AllowUserToDeleteRows = false,
				AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
			};
			_dataGridViewPolling.Columns.Add(new DataGridViewTextBoxColumn()
			{
				Name = "_dataGridViewColumnClient",
				HeaderText = "Клиент",
				DataPropertyName = "Client"
			});
			_dataGridViewPolling.Columns.Add(new DataGridViewTextBoxColumn()
			{
				Name = "_dataGridViewColumnUID",
				HeaderText = "Идентификатор",
				DataPropertyName = "UID"
			});
			_dataGridViewPolling.Columns.Add(new DataGridViewTextBoxColumn()
			{
				Name = "_dataGridViewColumnFirstPollTime",
				HeaderText = "Первый полинг",
				DataPropertyName = "FirstPollTime"
			});
			_dataGridViewPolling.Columns.Add(new DataGridViewTextBoxColumn()
			{
				Name = "_dataGridViewColumnLastPollTime",
				HeaderText = "Последний полинг",
				DataPropertyName = "LastPollTime"
			});
			_dataGridViewPolling.Columns.Add(new DataGridViewTextBoxColumn()
			{
				Name = "_dataGridViewColumnCallbackIndex",
				HeaderText = "Индекс",
				DataPropertyName = "CallbackIndex"
			});
			_tabPagePolling.Controls.Add(_dataGridViewPolling);

			#endregion

			#region Operations

			_tabPageOperations = new TabPage() { Name = "_tabPageOperations", Text = "Операции" };
			_tabControlMain.TabPages.Add(_tabPageOperations);

			_dataGridViewOperations = new DataGridView()
			{
				Name = "_dataGridViewOperations",
				MultiSelect = false,
				SelectionMode = DataGridViewSelectionMode.FullRowSelect,
				Dock = DockStyle.Fill,
				AutoGenerateColumns = false,
				AllowUserToAddRows = false,
				AllowUserToDeleteRows = false,
				AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
			};
			_dataGridViewOperations.Columns.Add(new DataGridViewTextBoxColumn()
			{
				Name = "_dataGridViewColumnName",
				HeaderText = "Название",
				DataPropertyName = "Name"
			});
			_tabPageOperations.Controls.Add(_dataGridViewOperations);

			#endregion

			#region License

			_tabPageLicense = new TabPage { Name = "_tabPageLicense", Text = "Лицензирование", Padding = new Padding(5) };
			_tabPageLicense.Resize += EventHandler_tabPageLicense_Resize;
			_tabControlMain.TabPages.Add(_tabPageLicense);

			var groupBox = new GroupBox 
			{ 
				Name = "groupBoxLicenseStatus", 
				Text = String.Empty,
				Left = _tabPageLicense.Padding.Left,
 				Padding = new Padding(10),
			};
			_tabPageLicense.Controls.Add(groupBox);

			label = new Label 
			{ 
				Name = "_labelLicenseStatus", 
				Text = "Статус лицензии:",
				AutoSize = true,
				Font = font,
			};
			groupBox.Controls.Add(label);
			_labelLicenseStatus = new Label
			{
				Name = "_labelLicenseStatusValue",
				Text = "Value", //String.Empty, 
				AutoSize = true,
				Font = font
			};
			groupBox.Controls.Add(_labelLicenseStatus);
			//groupBox.Height = label.Height + groupBox.Padding.Top + groupBox.Padding.Bottom;

			groupBox = new GroupBox
			{
				Name = "groupBoxLicense",
				Text = String.Empty,
				Width = groupBox.Width,
				Padding = new Padding(10),
				//Margin = new Padding(5)
			};
			_tabPageLicense.Controls.Add(groupBox);

			label = new Label
			{
				Name = "_labelRemoteWorkStation",
				Text = "GLOBAL Удалённое рабочее место (количество):",
				AutoSize = true,
				Font = font,
			};
			groupBox.Controls.Add(label);

			_labelRemoteWorkStation = new Label
			{
				Name = "_labelRemoteWorkStationValue",
				Text = "1",
				AutoSize = true,
				Font = font,
			};
			groupBox.Controls.Add(_labelRemoteWorkStation);

			label = new Label
			{
				Name = "_labelFirefighting",
				Text = "GLOBAL Пожаротушение:",
				AutoSize = true,
				Font = font,
			};
			groupBox.Controls.Add(label);

			_labelFirefighting = new Label
			{
				Name = "_labelFirefightingValue",
				Text = "Нет",
				AutoSize = true,
				Font = font,
			};
			groupBox.Controls.Add(_labelFirefighting);

			label = new Label
			{
				Name = "_labelSecurity",
				Text = "GLOBAL Охрана:",
				AutoSize = true,
				Font = font,
			};
			groupBox.Controls.Add(label);

			_labelSecurity = new Label
			{
				Name = "_labelSecurityValue",
				Text = "Нет",
				AutoSize = true,
				Font = font,
			};
			groupBox.Controls.Add(_labelSecurity);

			label = new Label
			{
				Name = "_labelAccess",
				Text = "GLOBAL Доступ:",
				AutoSize = true,
				Font = font,
			};
			groupBox.Controls.Add(label);

			_labelAccess = new Label
			{
				Name = "_labelAccessValue",
				Text = "Нет",
				AutoSize = true,
				Font = font,
			};
			groupBox.Controls.Add(_labelAccess);

			label = new Label
			{
				Name = "_labelVideo",
				Text = "GLOBAL Видео:",
				AutoSize = true,
				Font = font,
			};
			groupBox.Controls.Add(label);

			_labelVideo = new Label
			{
				Name = "_labelVideoValue",
				Text = "Нет",
				AutoSize = true,
				Font = font,
			};
			groupBox.Controls.Add(_labelVideo);

			label = new Label
			{
				Name = "_labelOpcServer",
				Text = "GLOBAL OPC Сервер:",
				AutoSize = true,
				Font = font,
			};
			groupBox.Controls.Add(label);

			_labelOpcServer = new Label
			{
				Name = "_labelOpcServerValue",
				Text = "Нет",
				AutoSize = true,
				Font = font,
			};
			groupBox.Controls.Add(_labelOpcServer);

			groupBox = new GroupBox
			{
				Name = "groupBoxLoadingLicense",
				Text = String.Empty,
				Width = groupBox.Width,
				Padding = new Padding(10),
			};
			_tabPageLicense.Controls.Add(groupBox);

			label = new Label
			{
				Name = "_labelLicenseKey",
				Text = "Ключ:",
				AutoSize = true,
				Font = font,
			};
			groupBox.Controls.Add(label);

			_labelLicenseKey = new Label
			{
				Name = "_labelLicenseKeyValue",
				Text = "License Key",
				AutoSize = true,
				Font = font,
			};
			groupBox.Controls.Add(_labelLicenseKey);

			_buttonLoadLicense = new Button
			{
				Name = "_buttonLoadLicense",
				Text = "Загрузить лицензию",
				AutoSize = true
			};
			_buttonLoadLicense.Click += EventHandler_buttonLoadLicense_Click;
			groupBox.Controls.Add(_buttonLoadLicense);

			_buttonCopyLicense = new Button
			{
				Name = "_buttonCopyLicense",
				Text = "Копировать ключ",
				AutoSize = true
			};
			_buttonCopyLicense.Click += EventHandler_buttonCopyLicense_Click;
			groupBox.Controls.Add(_buttonCopyLicense);

			#endregion
		}

		void EventHandler_buttonCopyLicense_Click(object sender, EventArgs e)
		{
			Clipboard.SetText(_labelLicenseKey.Text);
		}

		void EventHandler_buttonLoadLicense_Click(object sender, EventArgs e)
		{
			OnClickLoadLicense();
		}

		void EventHandler_tabPageLicense_Resize(object sender, EventArgs e)
		{
			var control = (TabPage)sender;
			var groupBoxLicenseStatus = (GroupBox)control.Controls["groupBoxLicenseStatus"];

			groupBoxLicenseStatus.Top = 0;
			groupBoxLicenseStatus.Left = control.Padding.Left;
			groupBoxLicenseStatus.Width = control.Width - (control.Padding.Left + control.Padding.Right);
			
			var labelLicenseStatus = (Label)groupBoxLicenseStatus.Controls["_labelLicenseStatus"];
			labelLicenseStatus.Location = new Point(groupBoxLicenseStatus.Left, groupBoxLicenseStatus.Padding.Top);
			_labelLicenseStatus.Location = new Point(labelLicenseStatus.Location.X + labelLicenseStatus.Width + 10,
				labelLicenseStatus.Location.Y);
			groupBoxLicenseStatus.Height = _labelLicenseStatus.Height + groupBoxLicenseStatus.Padding.Top
				+ groupBoxLicenseStatus.Padding.Bottom;

			var groupBoxLicense = (GroupBox)control.Controls["groupBoxLicense"];
			groupBoxLicense.Location = groupBoxLicenseStatus.Location;
			groupBoxLicense.Top += groupBoxLicenseStatus.Height;
			groupBoxLicense.Width = groupBoxLicenseStatus.Width;

			var labelRemoteWorkStation = (Label)groupBoxLicense.Controls["_labelRemoteWorkStation"];
			labelRemoteWorkStation.Location = new Point(groupBoxLicense.Left, groupBoxLicense.Padding.Top);

			_labelRemoteWorkStation.Top = labelRemoteWorkStation.Top;
			_labelRemoteWorkStation.Left = labelRemoteWorkStation.Left + labelRemoteWorkStation.Width + 10;

			var labelFirefighting = (Label)groupBoxLicense.Controls["_labelFirefighting"];
			labelFirefighting.Location = new Point(groupBoxLicense.Left,
				groupBoxLicense.Padding.Top + labelRemoteWorkStation.Height);

			_labelFirefighting.Top = labelFirefighting.Top;
			_labelFirefighting.Left = labelFirefighting.Left + labelFirefighting.Width + 10;

			var labelSecurity = (Label)groupBoxLicense.Controls["_labelSecurity"];
			labelSecurity.Location = new Point(groupBoxLicense.Left,
				groupBoxLicense.Padding.Top + labelRemoteWorkStation.Height * 2);

			_labelSecurity.Top = labelSecurity.Top;
			_labelSecurity.Left = labelSecurity.Left + labelSecurity.Width + 10;
			
			var labelAccess = (Label)groupBoxLicense.Controls["_labelAccess"];
			labelAccess.Location = new Point(groupBoxLicense.Left,
				groupBoxLicense.Padding.Top + labelRemoteWorkStation.Height * 3);

			_labelAccess.Top = labelAccess.Top;
			_labelAccess.Left = labelAccess.Left + labelAccess.Width + 10;

			var labelVideo = (Label)groupBoxLicense.Controls["_labelVideo"];
			labelVideo.Location = new Point(groupBoxLicense.Left,
				groupBoxLicense.Padding.Top + labelRemoteWorkStation.Height * 4);

			_labelVideo.Top = labelVideo.Top;
			_labelVideo.Left = labelVideo.Left + labelVideo.Width + 10;

			var labelOpcServer = (Label)groupBoxLicense.Controls["_labelOpcServer"];
			labelOpcServer.Location = new Point(groupBoxLicense.Left,
				groupBoxLicense.Padding.Top + labelRemoteWorkStation.Height * 5);

			_labelOpcServer.Top = labelOpcServer.Top;
			_labelOpcServer.Left = labelOpcServer.Left + labelOpcServer.Width + 10;

			groupBoxLicense.Height = groupBoxLicense.Padding.Top + groupBoxLicense.Padding.Bottom 
				+ labelRemoteWorkStation.Height * 6;

			var groupBoxLoadingLicense = (GroupBox)control.Controls["groupBoxLoadingLicense"];
			groupBoxLoadingLicense.Location = groupBoxLicense.Location;
			groupBoxLoadingLicense.Top += groupBoxLicense.Height;
			groupBoxLoadingLicense.Width = groupBoxLicense.Width;

			var labelLicenseKey = (Label)groupBoxLoadingLicense.Controls["_labelLicenseKey"];
			labelLicenseKey.Location = new Point(groupBoxLoadingLicense.Left,
				groupBoxLoadingLicense.Padding.Top);
			var button = (Button)groupBoxLoadingLicense.Controls["_buttonLoadLicense"];
			_labelLicenseKey.Location = new Point(labelLicenseKey.Left + labelLicenseKey.Width + 10,
				labelLicenseKey.Top);
			button.Location = new Point(labelLicenseKey.Left,
				_labelLicenseKey.Top + _labelLicenseKey.Height);
			button = (Button)groupBoxLoadingLicense.Controls["_buttonCopyLicense"];
			button.Location = new Point(labelLicenseKey.Left + _buttonLoadLicense.Width + 10,
				_labelLicenseKey.Top + _labelLicenseKey.Height);
			groupBoxLoadingLicense.Height = _labelLicenseKey.Height + button.Height + groupBoxLoadingLicense.Padding.Top
				+ groupBoxLoadingLicense.Padding.Bottom;
		}

		protected override void SetVisibleCore(bool value)
		{
			// этот код необходим что бы скрыть форму при запуске приложения
			// http://stackoverflow.com/questions/4556411/how-to-hide-a-window-in-start-in-c-sharp-desktop-application
			if (!IsHandleCreated && value)
			{
				value = false;
				CreateHandle();
			}
			base.SetVisibleCore(value);
		}

		void OnCommandDisconnectActivated()
		{
			if (CommandDisconnectActivated != null)
				CommandDisconnectActivated(this, new EventArgs());
		}

		void OnClickLoadLicense()
		{
			if (ClickLoadLicense != null)
				ClickLoadLicense(this, new EventArgs());
		}
		#endregion

		#region Events

		public event EventHandler CommandDisconnectActivated;
		public event EventHandler ClickLoadLicense;

		#endregion

	}
}
