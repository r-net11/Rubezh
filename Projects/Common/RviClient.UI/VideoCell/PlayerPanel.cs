using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;
using UserControl = System.Windows.Forms.UserControl;

namespace RviClient.UI.VideoCell
{
	/// <summary>
	/// Панель для плеера
	/// </summary>
	public partial class PlayerPanel : UserControl
	{
		private bool _alwaysShowTollbars;

		/// <summary>
		/// ctor
		/// </summary>
		public PlayerPanel()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			UpdateLabelPositionAndSize();
			UpdateBottomPanelPositionAndSize();
		}

		/// <summary>
		/// 
		/// </summary>
		public UIElement HostedElement
		{
			get { return backgroundElementHost.Child; }
			set
			{
				if (backgroundElementHost.Child != null)
				{
					backgroundElementHost.Child.MouseMove -= ChildOnMouseMove;
					backgroundElementHost.Child.MouseLeave -= Child_MouseLeave;
				}
				backgroundElementHost.Child = value;
				backgroundElementHost.Child.MouseMove += ChildOnMouseMove;
				backgroundElementHost.Child.MouseLeave += Child_MouseLeave;

			}
		}

		/// <summary>
		/// Показать текст сообщения статуса канала
		/// </summary>
		public void ShowStatusTitle()
		{
			label1.Visible = true;
		}

		/// <summary>
		/// Скрыть текст сообщения статуса канала
		/// </summary>
		public void HideStatusTitle()
		{
			label1.Visible = true;
		}

		/// <summary>
		/// Поучить/Задать текст сообщения о статусе канала
		/// </summary>
		public String StatusTitleText
		{
			get { return label1.Text; }
			set
			{
				label1.Text = value;
				UpdateLabelPositionAndSize();
			}
		}

		public int ToolBarHeight
		{
			get
			{
				return Math.Max(bottomPanel.Height, topPanel.Height);
			}
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			UpdateLabelPositionAndSize();
			UpdateBottomPanelPositionAndSize();
		}

		/// <summary>
		/// Обновляет расположение метки в соответсвии с текущими размерами род. компонента
		/// </summary>
		private void UpdateLabelPositionAndSize()
		{
			label1.MaximumSize = new Size(Size.Width - 4, Size.Height / 2); // устанавливается максимальная ширина метки статуса, совместно с AutoSize = true - осуществляет автоперенос по словам
			var labelSize = label1.Size;
			label1.Location = new Point(Size.Width / 2 - labelSize.Width / 2, Size.Height / 2 - labelSize.Height / 2);
		}

		private void UpdateBottomPanelPositionAndSize()
		{
			if (Size.Width == 0 || Size.Height == 0) return;
			bottomPanel.Location = new Point(0, Size.Height - bottomPanel.Height);
			bottomPanel.Size = new Size(Size.Width, bottomPanel.Height);
		}

		private void Child_MouseLeave(object sender, MouseEventArgs e)
		{
			HideToolbars();
		}

		private void ChildOnMouseMove(object sender, MouseEventArgs mouseEventArgs)
		{
			ShowToolbars();
		}

		/// <summary>
		/// Добавляет содержимое на верхнюю панель
		/// </summary>
		/// <value></value>
		public UIElement TopToolbarContent
		{
			get { return topElementHost.Child; }

			set
			{
				if (topElementHost.Child != null)
				{
					topElementHost.Child.MouseLeave -= ContentElement_MouseLeave;
				}

				topElementHost.Child = value;

				if (value != null)
				{
					value.MouseLeave += ContentElement_MouseLeave;
				}
			}
		}

		public UIElement BottomToolbarContent
		{
			get { return bottomElementHost.Child; }
			set
			{
				if (bottomElementHost.Child != null)
				{
					bottomElementHost.Child.MouseLeave -= ContentElement_MouseLeave;
				}

				bottomElementHost.Child = value;

				if (value != null)
				{
					value.MouseLeave += ContentElement_MouseLeave;
				}
			}
		}

		public bool AlwaysShowTollbars
		{
			get { return _alwaysShowTollbars; }
			set
			{
				_alwaysShowTollbars = value;
				if (_alwaysShowTollbars)
				{
					ShowToolbars();
				}
				else
				{
					HideToolbars();
				}
			}
		}

		private void ContentElement_MouseLeave(object sender, MouseEventArgs e)
		{
			HideToolbars();
		}

		public void DisposeElementHosts()
		{
			topElementHost.Child = null;
			topElementHost.Dispose();
			bottomElementHost.Child = null;
			bottomElementHost.Dispose();
			backgroundElementHost.Child = null;
			backgroundElementHost.Dispose();
		}

		private void ShowToolbars()
		{
			if (topElementHost.Child == null || bottomElementHost.Child == null) return;
			topPanel.Visible = true;
			bottomPanel.Visible = true;
		}

		private void HideToolbars()
		{
			if (topElementHost.Child == null ||
				bottomElementHost.Child == null ||
				topElementHost.Child.IsMouseOver ||
				bottomElementHost.Child.IsMouseOver ||
				AlwaysShowTollbars)
				return;
			topPanel.Visible = false;
			bottomPanel.Visible = false;
		}

		private void label1_Leave(object sender, EventArgs e)
		{
			HideToolbars();
		}

		private void label1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			ShowToolbars();
		}
	}
}
