using FiresecService.Service;
using FiresecService.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FiresecService.Views
{
	public class TabPageConnectionsView: TabPage, ITabPageConnectionsView
	{
		#region Constructors

		public TabPageConnectionsView(): base()
		{
			_contextMenuStrip = new ContextMenuStrip()
			{
				Name = "_contextMenuStrip"
			};
			_toolStripMenuItemDisconnect = new ToolStripMenuItem(
				"Разорвать соединение", 
				null, 
				EventHandler_toolStripMenuItemDisconnect_Click);
			_contextMenuStrip.Items.Add(_toolStripMenuItemDisconnect);

			_dataGridView = new DataGridView()
			{
				Name = "_dataGridView",
				Dock = DockStyle.Fill,
				MultiSelect = false,
				SelectionMode = DataGridViewSelectionMode.FullRowSelect,
				ContextMenuStrip = _contextMenuStrip
			};
			Controls.Add(_dataGridView);
		}

		#endregion

		#region Fields And Properties

		ContextMenuStrip _contextMenuStrip;
		ToolStripMenuItem _toolStripMenuItemDisconnect;
		DataGridView _dataGridView;

		public IList<ClientViewModel> Clients
		{
			set 
			{
				_dataGridView.DataSource = null;
				_dataGridView.DataSource = value;
			}
		}

		//public ClientViewModel SelectedClient
		//{
		//	get { return _dataGridView.SelectedRows[0].
		//}

		#endregion

		#region Event handlers for context menu _toolStripMenuItemDisconnect
		
		void EventHandler_toolStripMenuItemDisconnect_Click(object sender, EventArgs e)
		{
		}

		#endregion
	}
}