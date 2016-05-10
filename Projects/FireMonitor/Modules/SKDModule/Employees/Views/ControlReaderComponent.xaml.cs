using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using StrazhAPI.Models;
using Infrastructure.Common;

namespace SKDModule.Views
{
	/// <summary>
	/// Interaction logic for ControlReaderComponent.xaml
	/// </summary>
	public partial class ControlReaderComponent : UserControl
	{
		#region Dependency Properties

		public static readonly DependencyProperty IsReaderSelectedProperty = DependencyProperty.Register("IsReaderSelected",
			typeof (bool), typeof (ControlReaderComponent));

		public static readonly DependencyProperty IsLinkEnabledProperty = DependencyProperty.Register("IsLinkEnabled",
			typeof (bool), typeof (ControlReaderComponent));

		public static readonly DependencyProperty ClickCommandProperty = DependencyProperty.Register("ClickCommand",
			typeof (ICommand), typeof (ControlReaderComponent));

		public static readonly DependencyProperty LinkTextProperty = DependencyProperty.Register("LinkText", typeof (string),
			typeof (ControlReaderComponent));

		public static readonly DependencyProperty ContentTextProperty = DependencyProperty.Register("ContentText",
			typeof (string), typeof (ControlReaderComponent));

		public static readonly DependencyProperty TextBoxWidthProperty = DependencyProperty.Register("TextBoxWidth",
			typeof (double), typeof (ControlReaderComponent));

		public double TextBoxWidth
		{
			get { return (double) GetValue(TextBoxWidthProperty); }
			set { SetValue(TextBoxWidthProperty, value); }
		}

		public string ContentText
		{
			get { return (string) GetValue(ContentTextProperty); }
			set { SetValue(ContentTextProperty, value); }
		}

		public string LinkText
		{
			get { return (string) GetValue(LinkTextProperty); }
			set { SetValue(LinkTextProperty, value); }
		}

		public ICommand ClickCommand
		{
			get { return (ICommand) GetValue(ClickCommandProperty); }
			set { SetValue(ClickCommandProperty, value); }
		}

		public bool IsLinkEnabled
		{
			get { return (bool) GetValue(IsLinkEnabledProperty); }
			set { SetValue(IsLinkEnabledProperty, value); }
		}

		public bool IsReaderSelected
		{
			get { return (bool) GetValue(IsReaderSelectedProperty); }
			set { SetValue(IsReaderSelectedProperty, value); }
		}

		public RelayCommand ClearCommand { get; set; }

		#endregion
		#region Constructors

		public ControlReaderComponent()
		{
			InitializeComponent();
			ClearCommand = new RelayCommand(ButtonBase_OnClick);
		}

		private void ButtonBase_OnClick()
		{
			ContentTextBox.Text = null;
			IsReaderSelected = default(bool);
		}

		#endregion

		#region Events

		private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
		{
			ContentTextBox.Text = null;
			IsReaderSelected = default(bool);
		}

		protected override void OnContentChanged(object oldContent, object newContent)
		{
			if(oldContent != null)
				throw new InvalidOperationException("You can not change the content");
		}

		#endregion
	}
}
