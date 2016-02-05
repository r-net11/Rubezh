using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Threading;

namespace FiresecService.Views
{
	public partial class MainView : Form, IMainView
	{
		#region Constructors

		public MainView()
		{
			InitializeComponent();

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
		}

		#endregion

		#region Fields And Properties

		//TabPageConnectionsView _tabPageConnectionsView;
		//TabPageGKView _tabPageGKView;
		//TabPageLicenceView _tabPageLicenceView;
		//TabPageLogView _tabPageLogView;
		//TabPageOperationsView _tabPageOperationsView;
		//TabPagePollingView _tabPagePollingView;
		//TabPageStatusView _tabPageStatusView;

		TabPage _tabPageConnections;
		DataGridView _dataGridViewConnections;
		ContextMenuStrip _contexMenuStripConnections;
		ToolStripMenuItem _toolStripMenuItemDisconnect;

		public string Title
		{
			get { return Text; }
			set { Text = value; }
		}

		public string LastLog
		{
			get { return _toolStripStatusLabelLastLog.Text; }
			set 
			{ 
				_toolStripStatusLabelLastLog.Text = value;
			}
		}

		public ITabPageView SelectedTabView 
		{
			get 
			{ 
				return (ITabPageView)_tabControlMain.SelectedTab;
			}
			private set 
			{
				OnTabChanged();
			}
		}

		public BindingSource Clients
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

		#endregion

		#region Event handlers for form

		private void EventHandler_MainWinFormView_Load(object sender, EventArgs e)
		{
			// Инициализация _tabControlMain
			//_tabPageConnectionsView = new TabPageConnectionsView()
			//{
			//	Name = "_tabPageConnections",
			//	Text = "Соединения"
			//};
			//_tabControlMain.TabPages.Add(_tabPageConnectionsView);

			//_tabPageLogView = new TabPageLogView()
			//{
			//	Name = "_tabPageLog",
			//	Text = "Лог"
			//};
			//_tabControlMain.TabPages.Add(_tabPageLogView);

			//_tabPageStatusView = new TabPageStatusView()
			//{
			//	Name = "_tabPageStatus",
			//	Text = "Статус"
			//};
			//_tabControlMain.TabPages.Add(_tabPageStatusView);

			//_tabPageGKView = new TabPageGKView()
			//{
			//	Name = "_tabPageGK",
			//	Text = "ГК"
			//};
			//_tabControlMain.TabPages.Add(_tabPageGKView);

			//_tabPagePollingView = new TabPagePollingView()
			//{
			//	Name = "_tabPagePolling",
			//	Text = "Поллинг"
			//};
			//_tabControlMain.TabPages.Add(_tabPagePollingView);

			//_tabPageOperationsView = new TabPageOperationsView()
			//{
			//	Name = "_tabPageOperations",
			//	Text = "Операции"
			//};
			//_tabControlMain.TabPages.Add(_tabPageOperationsView);

			//_tabPageLicenceView = new TabPageLicenceView()
			//{
			//	Name = "_tabPageLicence",
			//	Text = "Лицензирование"
			//};
			//_tabControlMain.TabPages.Add(_tabPageLicenceView);
		}

		private void EventHandler_MainWinFormView_Shown(object sender, EventArgs e)
		{
		}

		private void EventHandler_MainWinFormView_FormClosing(object sender, FormClosingEventArgs e)
		{
			ShowInTaskbar = false;
			Visible = false;
			e.Cancel = true;
		}

		private void MainWinFormView_Activated(object sender, EventArgs e)
		{
		}

		#endregion

		#region Event handlers for _tabControlMain
		
		private void EventHandler_tabControlMain_Selected(object sender, TabControlEventArgs e)
		{
			if (e.Action == TabControlAction.Selected)
			{
				//if (e.TabPage.Equals(_tabPageConnectionsView))
				//{
				//	SelectedTabView = (ITabPageConnectionsView)e.TabPage;
				//}
				//else if (e.TabPage.Equals(_tabPageLogView))
				//{
				//	SelectedTabView = (ITabPageLogView)e.TabPage;
				//}
				//else if (e.TabPage.Equals(_tabPageStatusView))
				//{
				//	SelectedTabView = (ITabPageStatusView)e.TabPage;
				//}
				//else if (e.TabPage.Equals(_tabPageGKView))
				//{
				//	SelectedTabView = (ITabPageGKView)e.TabPage;
				//}
				//else if (e.TabPage.Equals(_tabPagePollingView))
				//{
				//	SelectedTabView = (ITabPagePollingView)e.TabPage;
				//}
				//else if (e.TabPage.Equals(_tabPageOperationsView))
				//{
				//	SelectedTabView = (ITabPageOperationsView)e.TabPage;
				//}
				//else if (e.TabPage.Equals(_tabPageLicenceView))
				//{
				//	SelectedTabView = (ITabPageLicenceView)e.TabPage;
				//}
			}
		}

		#endregion

		#region Event handlers for context menu _toolStripMenuItemDisconnect

		void EventHandler_toolStripMenuItemDisconnect_Click(object sender, EventArgs e)
		{
			OnCommandDisconnectActivated();
		}

		#endregion

		#region Methods

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

		void OnTabChanged()
		{
			if (TabChanged != null)
			{
				TabChanged(this, new EventArgs());
			}
		}

		void OnCommandDisconnectActivated()
		{
			if (CommandDisconnectActivated != null)
				CommandDisconnectActivated(this, new EventArgs());
		}

		#endregion

		#region Events

		public event EventHandler TabChanged;
		public event EventHandler CommandDisconnectActivated;

		#endregion
	}
}
